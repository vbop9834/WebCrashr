namespace WebCrashr
    open inputCommon
    open actorCommon
    open workCommon
    open worker
    open reporter

    module manager =
    
        
        type managerActions =
            | OrderMoreWorkers of int
            | SendCommandToWorker of workAction
            | WorkerHasFinishedWork of int
            | Die of AsyncReplyChannel<unit>
         
        let sendWorkToWorker worker command =
            command 
            |> workerActions.Work 
            |> worker.actor.Post

        let doINeedToHireMoreWorkers idleWorkers nextId =
            if (idleWorkers |> List.length = 0) then
               worker.createWorker nextId :: idleWorkers
            else
                idleWorkers

        let reportToMeMinion (managerInbox: MailboxProcessor<managerActions>) (reporterActor : actor<reporterActions>) result id  = 
            id |> managerActions.WorkerHasFinishedWork |> managerInbox.Post 
            result |> reporter.Report |> reporterActor.Post

        let createCommand work managerInbox reporterActor =
            {                
                work = work
                reportResultsOfWork = managerInbox |> reportToMeMinion <| reporterActor
            }

        let getManagerActor webCrashrConfig : actor<managerActions> =
            let reporterActor = reporter.getReporterActor webCrashrConfig.reporterConfiguration
            actor.Start(fun inbox ->
                let rec loop idleWorkers busyWorkers =
                    async {
                        let! msg = inbox.Receive ()
                        match msg with
                        | managerActions.OrderMoreWorkers numberOfWorkers ->
                            let idleWorkers = [1 .. numberOfWorkers] |> List.map (fun id -> id |> worker.createWorker)
                            return! loop idleWorkers []
                        | managerActions.SendCommandToWorker work ->
                            let worker = idleWorkers |> doINeedToHireMoreWorkers <| busyWorkers.Length |> List.head 
                            worker |> sendWorkToWorker <| createCommand work inbox reporterActor
                            return! idleWorkers |> List.tail |> loop <| worker :: busyWorkers
                        | managerActions.WorkerHasFinishedWork workerId ->
                            let selectedWorker = busyWorkers |> List.find (fun oneWorker -> oneWorker.id = workerId)
                            let busyWorkers = busyWorkers |> List.filter (fun oneWorker -> oneWorker.id <> workerId)
                            let idleWorkers = [selectedWorker] @ idleWorkers
                            return! idleWorkers |> loop <| busyWorkers
                        | managerActions.Die syncChannel ->
                            printf "Killing workers...\n"
                            idleWorkers |> List.append <| busyWorkers
                            |> List.iter (fun worker -> workerActions.Die |> worker.actor.PostAndReply)
                            printf "Workers are finished\n"
                            printf "Telling Reporter to finish\n"
                            reporterActions.Die |> reporterActor.PostAndReply
                            printf "Goodbye!\n"
                            syncChannel.Reply ()
                            return ()
                    }
                loop [] []
            )


