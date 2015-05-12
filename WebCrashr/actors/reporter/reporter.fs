namespace WebCrashr
    open actorCommon
    open inputCommon
    open workCommon
    open reporterTypes

    open IReporter
    open fileReporter
    open consoleReporter

    open System.IO

    module reporter =
    
        let getReporterFromType (reporterConfiguration : reporterConfiguration) : IReporter =
            match reporterConfiguration.reporterType with
                | reporters.FileReporter -> 
                    new fileReporter(reporterConfiguration.outputDir, reporterConfiguration.fileName) :> IReporter
                | reporters.ConsoleReporter ->
                    new consoleReporter () :> IReporter

        type reporterActions =
            | Report of workResult
            | Die of AsyncReplyChannel<unit>
        
        let getReporterActor reporterConfiguration : actor<reporterActions> =
            let reporter = getReporterFromType reporterConfiguration
            actor.Start(fun inbox ->
                let rec loop () =
                    async {
                        let! msg = inbox.Receive ()
                        match msg with
                            | reporterActions.Report workResult ->
                              workResult |> reporter.report
                              return! loop ()
                            | reporterActions.Die syncChannel ->
                              reporter.die ()
                              syncChannel.Reply ()
                              return ()
                        }
                loop ()
            )
            