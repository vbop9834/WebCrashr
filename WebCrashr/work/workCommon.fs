namespace WebCrashr
    module workCommon =

        type workResult =
            {
                workTime : int64
                subject : string
            }

        type workAction = unit -> workResult

