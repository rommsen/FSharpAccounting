module FSharpAccounting.Program

open System
open FSharpAccounting.Domain
open FSharpAccounting.Operations
open FSharpAccounting.FileRepository

let withdrawWithAudit amount account =
  auditAs Withdraw Auditing.composedLogger withdrawSafe amount account

let depositWithAudit amount account =
  auditAs Deposit Auditing.composedLogger deposit amount account

let tryParseCommand command =
  match command with
  | 'd' -> Deposit |> BankOperation |> Some
  | 'w' -> Withdraw |> BankOperation |> Some
  | 'x' -> Exit |> Some
  | _ -> None

let isStopCommand command =
  command = Exit

let getAmount command =
  Console.Write "\nAmount: "
  match Console.ReadLine() |> Decimal.TryParse with
  | true, amount -> Some (command,amount)
  | false, _ -> None

let processCommand account (command, amount: decimal) =
  match command with
  | Deposit ->
    account |> depositWithAudit amount

  | Withdraw ->
    account |> withdrawWithAudit amount

let tryGetBankOperation cmd =
  match cmd with
  | Exit -> None
  | BankOperation op -> Some op

let replay account transaction =
  match transaction.Operation, account with
  | Deposit, _ ->
      account |> deposit transaction.Amount

  | Withdraw, InCredit account ->
      account |> withdraw transaction.Amount

  | Withdraw, Overdrawn _ ->
      account

let loadAccount (owner,accountId,transactions) =
  let account =
    Account.init accountId owner
    |> classifyAccount

  transactions
  |> Seq.choose id
  |> Seq.sortBy (fun tx -> tx.Timestamp)
  |> Seq.fold replay account

let tryLoadAccountFromDisk =
  tryFindTransactionsOnDisk >> Option.map loadAccount

[<EntryPoint>]
let main _ =
  Console.Clear()

  let openingAccount =
    Console.Write "Please enter your name: "
    let owner = Console.ReadLine()

    match (tryLoadAccountFromDisk owner) with
    | Some account ->
        account

    | None ->
        {
          Balance = 0M
          AccountId = Guid.NewGuid()
          Owner = { Name = owner }
        }
        |> CreditAccount
        |> InCredit

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
    |> Seq.choose tryGetBankOperation
    |> Seq.choose getAmount
    |> Seq.fold processCommand openingAccount

  Console.Clear()
  printfn "Closing Balance:\r\n %A" closingAccount
  Console.ReadKey() |> ignore

  0
