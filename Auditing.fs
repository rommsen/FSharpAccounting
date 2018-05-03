module FSharpAccounting.Auditing

/// Logs to the console
let printTransaction _ accountId transaction =
  printfn "Account %O: %A" accountId transaction

// Logs to both console and file system
let composedLogger =
  let loggers =
    [
      FileRepository.writeTransaction
      printTransaction
    ]

  fun accountId owner transaction ->
    loggers
    |> List.iter(fun logger -> logger accountId owner transaction)
