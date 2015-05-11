namespace WebCrashr
open timer
open unitOfTimeMeasurements
open inputCommon

    module WebCrashr =
        
        let startWebCrashr ( webCrashrConfig : webCrashrConfiguration ) =
            printf "Starting Web Crashr\n"
            printf "Execution Time Set To %i milliseconds\n" webCrashrConfig.executionTimeInMilliseconds
            printf "RPM set to %i milliseconds\n" webCrashrConfig.rpmInMilliseconds
            printf "%i Workers Ordered on Init\n" webCrashrConfig.amountOfWorkersToOrder
            timer.startTimer webCrashrConfig.amountOfWorkersToOrder webCrashrConfig.workList webCrashrConfig.rpmInMilliseconds
            System.Threading.Thread.Sleep(webCrashrConfig.executionTimeInMilliseconds)
