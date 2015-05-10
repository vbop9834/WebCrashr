namespace WebCrashr

    module unitOfTimeMeasurements =

    
        [<Measure>]
        type millisecond
        
        let convertIntToMilliseconds (amountOfMilliseconds : int64) = int64(1<millisecond>) * amountOfMilliseconds
        
        let second = int64(1000<millisecond>)


        
