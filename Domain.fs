namespace FSharpAccounting.Domain

open System

type Customer =
  { Name : string }

type Account =
  { AccountId : Guid; Owner : Customer; Balance : decimal }

type BankOperation =
  | Withdraw
  | Deposit

type Command =
  | BankOperation of BankOperation
  | Exit

type Transaction =
  {
    Timestamp : DateTime
    Operation : BankOperation
    Amount : decimal
    Accepted : bool
  }

module Account =
  let init accountId ownerName =
    {
      AccountId = accountId
      Owner = { Name = ownerName }
      Balance = 0M
    }

module Transaction =
  let serialize transaction =
    sprintf "%O***%A***%M***%b"
      transaction.Timestamp
      transaction.Operation
      transaction.Amount
      transaction.Accepted

  let tryParseCommand command =
    match command with
    | "Withdraw" -> Withdraw
    | "Deposit" -> Deposit
    | _ -> failwith "Command not known in deserialization"

  let deserialize (serialized:string) =
    let parts = serialized.Split([|"***"|], StringSplitOptions.None)
    {
      Timestamp = DateTime.Parse parts.[0]
      Operation = parts.[1] |> tryParseCommand
      Amount = Decimal.Parse parts.[2]
      Accepted = Boolean.Parse parts.[3]
    }
