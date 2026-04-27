using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Validation;
using System;

namespace ClearBank.DeveloperTest.Services;

public class PaymentService : IPaymentService
{
    private readonly IAccountDataStore accountDataStore;

    public PaymentService(AccountDataStoreType accountDataStoreType, IAccountDataStoreFactory accountDataStoreFactory)
    {
        ArgumentNullException.ThrowIfNull(accountDataStoreFactory);

        this.accountDataStore = accountDataStoreFactory.Create(accountDataStoreType);
    }

    public MakePaymentResult MakePayment(MakePaymentRequest request)
    {
        Account account = this.accountDataStore.GetAccount(request.DebtorAccountNumber);

        if (!PaymentRequestValidator.IsPaymentRequestValidForAccount(request, account))
        {
            return MakePaymentResult.FailResult();
        }

        this.ExecutePaymentRequest(request, account);

        return MakePaymentResult.SuccessResult();
    }

    private void ExecutePaymentRequest(MakePaymentRequest request, Account account)
    {
        account.Debit(request.Amount);
        this.accountDataStore.UpdateAccount(account);
    }
}
