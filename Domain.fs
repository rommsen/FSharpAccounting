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

type CreditAccount =
  CreditAccount of Account

type RatedAccount =
  | InCredit of CreditAccount
  | Overdrawn of Account

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
    | "Withdraw" ->
        Some Withdraw

    | "Deposit" ->
        Some Deposit

    | _ ->
        None

  let tryDeserialize (serialized:string) =
    let parts = serialized.Split([|"***"|], StringSplitOptions.None)

    match parts.[1] |> tryParseCommand with
    | Some operation ->
        {
          Timestamp = DateTime.Parse parts.[0]
          Operation = operation
          Amount = Decimal.Parse parts.[2]
          Accepted = Boolean.Parse parts.[3]
        } |> Some

    | None ->
        None

