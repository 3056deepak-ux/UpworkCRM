namespace UpworkERP.Core.Enums;

public enum TransactionType
{
    Debit = 1,
    Credit = 2
}

public enum AccountType
{
    Asset = 1,
    Liability = 2,
    Equity = 3,
    Revenue = 4,
    Expense = 5
}

public enum BudgetStatus
{
    Draft = 1,
    Active = 2,
    Completed = 3
}
