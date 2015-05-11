namespace WebCrashr
open workCommon

    module workKnob =
        
        type knob = 
            {
                percentage : int
                work : workAction
            }
        let createKnob work percentage =
            {
                percentage = percentage
                work = work
            }

        let getWorkList knobList =
            knobList 
            |> List.map (fun knob -> [1 .. 100/knob.percentage] |> List.map (fun _ -> knob.work)) 
            |> List.concat