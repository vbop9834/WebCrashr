namespace WebCrashr
open workCommon
open reporterTypes

    module inputCommon =

        type reporterConfiguration =
            {
                reporterType : reporterTypes.reporters
                outputDir : string
                fileName : string
            }

        type webCrashrConfiguration =
            {
               workList : workAction list
               executionTimeInMilliseconds : int
               amountOfWorkersToOrder : int
               rpmInMilliseconds : int64
               reporterConfiguration : reporterConfiguration

            }
