namespace WebCrashr
open workCommon

    module inputCommon =
        
        type webCrashrConfiguration =
            {
               workList : workAction list
               executionTimeInMilliseconds : int
               amountOfWorkersToOrder : int
               rpmInMilliseconds : int64
            }
