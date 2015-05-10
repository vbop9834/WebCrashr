namespace WebCrashr
open timer
open unitOfTimeMeasurements

    module WebCrashr =
        
        let startWebCrashr amountOfWorkersOrdered workList (timeToRun : int) =
            printf "Starting Web Crashr\n"
            timer.startTimer amountOfWorkersOrdered workList second
            System.Threading.Thread.Sleep(timeToRun)
