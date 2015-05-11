open WebCrashr

[<EntryPoint>]
let main argv = 
    let webCrashrConfig = "C:\projects\WebCrashr\WebCrashr\SampleXmlInput.xml" |> xmlInputReader.getPropertiesFromXml 
    webCrashrConfig |> WebCrashr.startWebCrashr
    0 // return an integer exit code
