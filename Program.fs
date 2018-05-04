module FSharpAccounting.Program

open System
open FSharpAccounting.Domain
open FSharpAccounting.Operations
open FSharpAccounting.FileRepository

let withdrawWithAudit =
  auditAs Withdraw Auditing.composedLogger withdraw

let depositWithAudit =
  auditAs Deposit Auditing.composedLogger deposit

let tryParseCommand command =
  match command with
  | 'd' -> Deposit |> Some
  | 'w' -> Withdraw |> Some
  | 'x' -> Exit |> Some
  | _ -> None

let isStopCommand command =
  command = Exit

let getAmount command =
  Console.Write "\nAmount: "
  let amount = Console.ReadLine() |> Decimal.Parse

  command,amount

let processCommand account (command, amount: decimal) =
  match command with
  | Deposit ->
    account |> depositWithAudit amount

  | Withdraw ->
    account |> withdrawWithAudit amount

  | Exit ->
    account

let replay account transaction =
  match transaction.Operation with
  | Deposit ->
      account |> deposit transaction.Amount

  | Withdraw ->
      account |> withdraw transaction.Amount

  | _ -> account

let loadAccount (owner,accountId,transactions) =
  let account = Account.init accountId owner

  transactions
  |> Seq.sortBy (fun tx -> tx.Timestamp)
  |> Seq.fold replay account

[<EntryPoint>]
let main _ =
  Console.Clear()

  let name =
    Console.Write "Please enter your name: "
    Console.ReadLine()

  let openingAccount =
    name
    |> findTransactionsOnDisk
    |> loadAccount

  printf "\r\nOpening Account %A\r\n" openingAccount

  let consoleCommands =
    seq {
      while true do
        Console.Write "\r\n(d)eposit, (w)ithdraw or e(x)it: "
        yield Console.ReadKey().KeyChar
    }

  let closingAccount =
    consoleCommands
    |> Seq.choose tryParseCommand
    |> Seq.takeWhile (not << isStopCommand)
    |> Seq.map getAmount
    |> Seq.fold processCommand openingAccount

  Console.Clear()
  printfn "Closing Balance:\r\n %A" closingAccount
  Console.ReadKey() |> ignore

  0
