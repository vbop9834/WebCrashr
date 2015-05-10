namespace WebCrashr
open workCommon
open System.Diagnostics
open FSharp.Data

    module webPageWork =
        
        let performWebPageWork url =
            let stopwatch = Stopwatch.StartNew ()
            let webResult = url |> Http.Request
            stopwatch.Stop ()
            {
                workTime = stopwatch.ElapsedMilliseconds
                subject = url
            }

        let webPageWork url : workAction =
            fun _ -> url |> performWebPageWork

            

