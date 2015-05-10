namespace WebCrashr
open actorCommon
    module worker =      

        type workerActions = 
            | Work of command

        type worker =
            {
                id : int
                actor : actor<workerActions>
            }
        
        let getWorkerActor id : actor<workerActions> =
            actor.Start(fun inbox ->
                let rec loop id =
                    async {
                        let! msg = inbox.Receive ()
                        match msg with
                        | workerActions.Work command ->
                          command.work () |> command.reportResultsOfWork <| id
                          return! loop id
                    }
                loop id
            )
        
        let createWorker id =
            {
                id = id
                actor = getWorkerActor id
            }