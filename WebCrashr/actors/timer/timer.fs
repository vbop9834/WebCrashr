namespace WebCrashr
open actorCommon
open manager
open System.Diagnostics
open unitOfTimeMeasurements

    module timer =

        type timerActions = 
            | Tick

        
        let getTimerActor (managerActor : actor<manager>) workList tickAtThisManyMilliseconds : actor<timerActions> =
            actor.Start(fun inbox ->
                let timerStopWatch = Stopwatch.StartNew ()

                let rec loop remainingWork lastTickInMs =
                    async {
                        let! msg = inbox.Receive ()
                        match msg with
                        | timerActions.Tick ->
                          let elapsedTimeInMs =  timerStopWatch.ElapsedMilliseconds
                          //printf "%i\n" elapsedTimeInMs
                          let timeSinceLastTick = elapsedTimeInMs - lastTickInMs
                         
                          if (timeSinceLastTick |> convertIntToMilliseconds >= tickAtThisManyMilliseconds) then                            
                            let workToDo = remainingWork |> List.head
                            workToDo |> manager.SendCommandToWorker |> managerActor.Post
                            let workRemainingAfterSendingToWorker = remainingWork |> List.tail |> List.append <| [workToDo]
                            timerActions.Tick |> inbox.Post
                            return! loop workRemainingAfterSendingToWorker elapsedTimeInMs
                          
                          timerActions.Tick |> inbox.Post
                          return! loop remainingWork lastTickInMs
                    }
                loop workList timerStopWatch.ElapsedMilliseconds //Start the work
            )
        
        let startTimer amountOfWorkersOrdered workList tickAtThisManyMilliseconds =
            let managerActor = manager.getManagerActor ()
            amountOfWorkersOrdered |> manager.OrderMoreWorkers |> managerActor.Post
            
            let timerActor = getTimerActor managerActor workList tickAtThisManyMilliseconds
            
            timerActions.Tick |> timerActor.Post