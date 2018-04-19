module FSharpAccounting.Program

open System
open FSharpAccounting.Domain
open FSharpAccounting.Operations
open FSharpAccounting.FileRepository

let withdrawWithAudit = auditAs "withdraw" Auditing.composedLogger withdraw
let depositWithAudit = auditAs "deposit" Auditing.composedLogger deposit

let isValidCommand command =
    match command with
    | 'd' | 'w' | 'x' -> true
    | _ -> false

let isStopCommand command =
    command = 'x'

let getAmount command =
    Console.WriteLine "\nAmount: "
    let amount = Console.ReadLine() |> Decimal.Parse
    
    command,amount



let processCommand account (command:char, amount: decimal) =
     match command with
     | 'd' ->
        account |> depositWithAudit amount

     | 'w' -> 
        account |> withdrawWithAudit amount

     | _ -> 
        account

let replay account transaction =
    match transaction.Operation with
    | "deposit" ->
        account |> deposit transaction.Amount

    | "withdraw" ->
        account |> withdraw transaction.Amount

    | _ -> account

let loadAccount (owner,accountId,transactions) =
   let account = Account.init accountId owner

   transactions
   |> Seq.sortBy (fun tx -> tx.Timestamp)
   |> Seq.fold replay account



[<EntryPoint>]
let main _ =
    let name =
        Console.Write "Please enter your name: "
        Console.ReadLine()

    let openingAccount =
        name
        |> findTransactionsOnDisk
        |> loadAccount

    let consoleCommands =
        seq {
            while true do
                Console.Write "(d)eposit, (w)ithdraw or e(x)it: "
                yield Console.ReadKey().KeyChar 
            }

    let closingAccount =
        consoleCommands
        |> Seq.filter isValidCommand
        |> Seq.takeWhile (not << isStopCommand)
        |> Seq.map getAmount
        |> Seq.fold processCommand openingAccount

    Console.Clear()
    printfn "Closing Balance:\r\n %A" closingAccount
    Console.ReadKey() |> ignore

    0