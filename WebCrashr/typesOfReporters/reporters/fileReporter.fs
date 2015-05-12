namespace WebCrashr
    open IReporter
    open System.IO
    open System.Text

    module fileReporter =
        
        type fileReporter (savePath, outputFileName) =
            let stringBuilder = 
                let stringBuilder = new StringBuilder()
                stringBuilder.AppendLine "dateTime,endpoint,responseTime" 

            member private this.addRowToStringBuilder (workResult : workCommon.workResult) =
                let getNewStringBuilder = 
                    let dateString = System.DateTime.Now.ToString()
                    let stringToAppend = sprintf "%s,%s,%i" dateString workResult.subject workResult.workTime 
                    stringToAppend |> stringBuilder.AppendLine

                stringBuilder = getNewStringBuilder
                                        
            interface IReporter with
                member this.report workResult =                
                    this.addRowToStringBuilder workResult |> ignore
                member this.die () =
                    let filePath = sprintf "%s\\%s" savePath outputFileName
                    System.IO.File.WriteAllText(filePath, stringBuilder.ToString())