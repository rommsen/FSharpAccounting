#load "Domain.fs"
#load "Operations.fs"

open FSharpAccounting.Operations
open FSharpAccounting.Domain
open System


printfn "%A" Withdraw

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




for number in 1 .. 10 do
    printfn "%d Hello!" number

for number in 10 .. -1 .. 1 do
    printfn "%d Hello!" number

let customerIds = [ 45 .. 99 ]
for customerId in customerIds do
    printfn "%d bought something!" customerId

for even in 2 .. 2 .. 10 do
    printfn "%d is an even number!" even

open System

let arrayOfChars = [| for c in 'a' .. 'z' -> Char.ToUpper c |]
let listOfSquares = [ for i in 1 .. 10 -> i * i ]
let seqOfStrings = seq { for i in 2 .. 4 .. 20 -> sprintf "Number %d" i }
