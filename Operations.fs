module FSharpAccounting.Operations

open System
open FSharpAccounting.Domain

/// Withdraws an amount of an account (if there are sufficient funds)
let classifyAccount account =
  if account.Balance >= 0M then
    account
    |> CreditAccount
    |> InCredit
  else
    Overdrawn account

let withdraw amount (CreditAccount account) =
  { account with Balance = account.Balance - amount }
  |> classifyAccount

let withdrawSafe amount ratedAccount =
  match ratedAccount with
  | InCredit account ->
      account |> withdraw amount

  | Overdrawn _ ->
      printfn "Your account is overdrawn - withdrawal rejected!"
      ratedAccount // return input back out

let deposit amount account =
  let account =
    match account with
    | InCredit (CreditAccount account) ->
        account

    | Overdrawn account ->
        account

  { account with Balance = account.Balance + amount }
  |> classifyAccount


/// Runs some account operation such as withdraw or deposit with auditing.
let auditAs operationName audit operation amount account =
  let unpacked =
    match account with
    | InCredit (CreditAccount account) ->
        account

    | Overdrawn account ->
        account

  let updatedAccount =
     operation amount account

  let transaction =
    {
      Operation = operationName
      Amount = amount
      Timestamp = DateTime.UtcNow
      Accepted = updatedAccount <> account
    }

  audit unpacked.AccountId unpacked.Owner.Name transaction

  updatedAccount
