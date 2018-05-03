namespace FSharpAccounting.Domain

open System

type Customer =
  { Name : string }

type Account =
  { AccountId : Guid; Owner : Customer; Balance : decimal }

type Transaction =
  {
    Timestamp : DateTime
    Operation : string
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
    sprintf "%O***%s***%M***%b"
      transaction.Timestamp
      transaction.Operation
      transaction.Amount
      transaction.Accepted

  let deserialize (serialized:string) =
    let parts = serialized.Split([|"***"|], StringSplitOptions.None)
    {
      Timestamp = DateTime.Parse parts.[0]
      Operation = parts.[1]
      Amount = Decimal.Parse parts.[2]
      Accepted = Boolean.Parse parts.[3]
    }
