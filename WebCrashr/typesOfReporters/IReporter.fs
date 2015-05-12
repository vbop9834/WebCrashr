namespace WebCrashr
    open workCommon

    module IReporter =
        type IReporter =
            abstract member report : workCommon.workResult -> unit
            abstract member die : unit -> unit