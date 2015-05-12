namespace WebCrashr

    module reporterTypes =

        type reporters =
            | FileReporter 
            | ConsoleReporter

        let matchStringWithReporterType inputString =
            match inputString with
            | "FileReporter" -> reporters.FileReporter
            | "ConsoleReporter" -> reporters.ConsoleReporter
            | _ -> 
                printf "REPORTER TYPE PARSER: Unable to find a reporter type with provided config: %s" inputString
                printf "REPORTER TYPE PARSER: Using default console reporter"
                reporters.ConsoleReporter