module Logs

open Serilog
open Serilog.Events
open Values

let Init () =
    Log.Logger <-
        LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            // Handle Information and Warnings
            .WriteTo.Logger(fun cc ->
                cc.Filter
                    .ByIncludingOnly(fun event ->
                        event.Level = LogEventLevel.Information || event.Level = LogEventLevel.Warning)
                    .WriteTo.File(Values.Paths.logFile, rollingInterval = RollingInterval.Day)
                |> ignore)
            // Handle Errors
            .WriteTo.Logger(fun cc ->
                cc.Filter
                    .ByIncludingOnly(fun event -> event.Level = LogEventLevel.Error)
                    .WriteTo.File(Values.Paths.errorFile, rollingInterval = RollingInterval.Day)
                |> ignore)
            .CreateLogger()
