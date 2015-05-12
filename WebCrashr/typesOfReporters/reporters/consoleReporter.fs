namespace WebCrashr
    open IReporter

    module consoleReporter =
        
        type consoleReporter () =
            
            interface IReporter with
                member this.report workResult =
                    printf "%s responded in %i" workResult.subject workResult.workTime
                member this.die () =
                    ()