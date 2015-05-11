open WebCrashr
open System

[<EntryPoint>]
let main argv = 
    let url = "http://www.google.com"
    let work = url |> webPageWork.webPageWork
    let knob = work |> workKnob.createKnob <| 50
    let workList = [knob; knob] |> workKnob.getWorkList
    let WorkersNeeded = 10 //workers
    WebCrashr.startWebCrashr WorkersNeeded workList 10000
    0 // return an integer exit code
