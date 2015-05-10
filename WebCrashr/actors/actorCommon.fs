namespace WebCrashr
open workCommon

    module actorCommon =

        type actor<'t> = MailboxProcessor<'t>

        type command = 
            {
                work : workAction
                reportResultsOfWork : workResult -> int -> unit
            }