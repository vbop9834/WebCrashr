namespace WebCrashr
    open actorCommon
    open workCommon
    open worker
    open reporter

    module manager =
    
        
        type manager =
            | OrderMoreWorkers of int
            | SendCommandToWorker of workAction
            | WorkerHasFinishedWork of int
         
        let sendWorkToWorker worker command =
            command 
            |> workerActions.Work 
            |> worker.actor.Post

        let doINeedToHireMoreWorkers idleWorkers nextId =
            if (idleWorkers |> List.length = 0) then
               worker.createWorker nextId :: idleWorkers
            else
                idleWorkers

        let reporterActor = reporter.getReporterActor ()

        let reportToMeMinion (managerInbox: MailboxProcessor<manager>) result id  = 
            id |> manager.WorkerHasFinishedWork |> managerInbox.Post 
            result |> reporter.Report |> reporterActor.Post

        let createCommand work managerInbox =
            {                
                work = work
                reportResultsOfWork = managerInbox |> reportToMeMinion
            }

        let getManagerActor () : actor<manager> =
            actor.Start(fun inbox ->
                let rec loop idleWorkers busyWorkers =
                    async {
                        let! msg = inbox.Receive ()
                        match msg with
                        | manager.OrderMoreWorkers numberOfWorkers ->
                            let idleWorkers = [1 .. numberOfWorkers] |> List.map (fun id -> id |> worker.createWorker)
                            return! loop idleWorkers []
                        | manager.SendCommandToWorker work ->
                            let worker = idleWorkers |> doINeedToHireMoreWorkers <| busyWorkers.Length |> List.head 
                            worker |> sendWorkToWorker <| createCommand work inbox
                            return! idleWorkers |> List.tail |> loop <| worker :: busyWorkers
                        | manager.WorkerHasFinishedWork workerId ->
                            let selectedWorker = busyWorkers |> List.find (fun oneWorker -> oneWorker.id = workerId)
                            let busyWorkers = busyWorkers |> List.filter (fun oneWorker -> oneWorker.id <> workerId)
                            let idleWorkers = [selectedWorker] @ idleWorkers
                            return! idleWorkers |> loop <| busyWorkers
                    }
                loop [] []
            )


