#load "Domain.fs"
#load "Operations.fs"

open FSharpAccounting.Operations
open FSharpAccounting.Domain
open System

let openingAccount =
  {
    Owner = { Name = "Roman" }
    AccountId = Guid.NewGuid()
    Balance = 1000M
  }

let isValidCommand command =
  match command with
  | 'd' | 'w' | 'x' -> true
  | _ -> false

let isStopCommand command =
  command = 'x'

let getAmount command =
  // seq {
  //     while true do
  //         let amount = Console.ReadKey().KeyChar
  //         yield (command, amount)
  // }
  command,0M

let processCommand account (command:char, amount: decimal) =
  account

let account =
  let commands = [ 'd'; 'w'; 'z'; 'f'; 'd'; 'x'; 'w' ]

  commands
  |> Seq.filter isValidCommand
  |> Seq.takeWhile (not << isStopCommand)
  |> Seq.map getAmount
  |> Seq.fold processCommand openingAccount


let replay account transaction =
  match transaction.Operation with
  | "deposit" ->
      account |> deposit transaction.Amount

  | "withdraw" ->
      account |> withdraw transaction.Amount

  | _ -> account

let loadAccount owner accountId transactions =
 let account = Account.init accountId owner

 transactions
 |> Seq.sortBy (fun tx -> tx.Timestamp)
 |> Seq.fold replay account
