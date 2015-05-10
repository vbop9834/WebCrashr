namespace WebCrashr
open actorCommon
open workCommon
open System.IO

module reporter =

    type reporterActions =
        | Report of workResult
        
    let getReporterActor () : actor<reporterActions> =
        actor.Start(fun inbox ->
            let rec loop () =
                async {
                    let! msg = inbox.Receive ()
                    match msg with
                        | reporterActions.Report workResult ->
                          printf "%s hit in %i milliseconds\n" workResult.subject workResult.workTime
                          return! loop ()
                    }
            loop ()
        )
        