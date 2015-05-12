namespace WebCrashr
    open System
    open actorCommon
    open manager
    open System.Diagnostics
    open unitOfTimeMeasurements
    open inputCommon

    module timer =

        type timerActions = 
            | Tick
            | Die of AsyncReplyChannel<unit>

        
        let getTimerActor (managerActor : actor<managerActions>) workList tickAtThisManyMilliseconds : actor<timerActions> =
            actor.Start(fun inbox ->
                let timerStopWatch = Stopwatch.StartNew ()

                let rec loop workList lastTickInMs =
                    async {
                        let! msg = inbox.Receive ()
                        match msg with
                        | timerActions.Tick ->
                          
                          let elapsedTimeInMs =  timerStopWatch.ElapsedMilliseconds
                          let timeSinceLastTick = elapsedTimeInMs - lastTickInMs
                         
                          if (timeSinceLastTick |> convertIntToMilliseconds >= tickAtThisManyMilliseconds) then                            
                            let workToDo = workList |> List.head
                            workToDo |> managerActions.SendCommandToWorker |> managerActor.Post
                            let workRemainingAfterSendingToWorker = workList |> List.tail |> List.append <| [workToDo]
                            timerActions.Tick |> inbox.Post
                            return! loop workRemainingAfterSendingToWorker elapsedTimeInMs
                          
                          timerActions.Tick |> inbox.Post
                          return! loop workList lastTickInMs

                        | timerActions.Die syncChannel ->
                            managerActions.Die |> managerActor.PostAndReply
                            syncChannel.Reply()
                            return ()
                    }
                loop workList timerStopWatch.ElapsedMilliseconds //Start the work
            )
        
        let stopTimer (timerActor : actor<timerActions>) =
            timerActions.Die |> timerActor.PostAndReply

        let sleepUntilTimerNeedsToDie (executionTimeInMilliseconds : int) =            
            System.Threading.Thread.Sleep(executionTimeInMilliseconds)


        let startTimer webCrashrConfig =
            let managerActor = manager.getManagerActor webCrashrConfig
            webCrashrConfig.amountOfWorkersToOrder |> manager.OrderMoreWorkers |> managerActor.Post
            
            let timerActor = getTimerActor managerActor webCrashrConfig.workList webCrashrConfig.rpmInMilliseconds
            
            timerActions.Tick |> timerActor.Post   

            webCrashrConfig.executionTimeInMilliseconds |> sleepUntilTimerNeedsToDie
            stopTimer timerActor         