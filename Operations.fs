module FSharpAccounting.Operations

open System
open FSharpAccounting.Domain

/// Withdraws an amount of an account (if there are sufficient funds)
let withdraw amount account =
  if amount > account.Balance then
    account
  else
    { account with Balance = account.Balance - amount }

/// Deposits an amount into an account
let deposit amount account =
  { account with Balance = account.Balance + amount }

/// Runs some account operation such as withdraw or deposit with auditing.
let auditAs operationName audit operation amount account =
  let updatedAccount =
    operation amount account

  let transaction =
    {
      Operation = operationName
      Amount = amount
      Timestamp = DateTime.UtcNow
      Accepted = updatedAccount <> account
    }

  audit account.AccountId account.Owner.Name transaction

  updatedAccount