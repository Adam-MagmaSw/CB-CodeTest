using System;

namespace ClearBank.DeveloperTest.Types;

public class Account
{
    public Account()
    {
    }

    public Account(string accountNumber, decimal balance, AccountStatus status, AllowedPaymentSchemes allowedPaymentSchemes)
    {
        AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
        Balance = balance;
        Status = status;
        AllowedPaymentSchemes = allowedPaymentSchemes;
    }

    public string AccountNumber { get; }

    public decimal Balance { get; private set; }

    public AccountStatus Status { get; }

    public AllowedPaymentSchemes AllowedPaymentSchemes { get; }

    public void Debit(decimal amount)
    {
        this.Balance -= amount;
    }
}
