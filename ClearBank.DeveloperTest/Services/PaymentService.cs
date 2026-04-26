using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Types;
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
        Account account = this.accountDataStore.GetAccount(request.DebtorAccountNumber); ;

        var result = new MakePaymentResult();

        result.Success = ValidatePaymentRequestForAccount(request, account);

        if (result.Success)
        {
            this.ExecutePaymentRequest(request, account);
        }

        return result;
    }

    private void ExecutePaymentRequest(MakePaymentRequest request, Account account)
    {
        account.Balance -= request.Amount;
        this.accountDataStore.UpdateAccount(account);
    }

    private static bool ValidatePaymentRequestForAccount(MakePaymentRequest request, Account account)
    {
        if (account == null)
        {
            return false;
        }

        if (!account.AllowedPaymentSchemes.HasFlag(request.PaymentScheme.ToAllowedPaymentSchemes()))
        {
            return false;
        }

        if (request.PaymentScheme == PaymentScheme.FasterPayments && account.Balance < request.Amount)
        {
            return false;
        }

        if (request.PaymentScheme == PaymentScheme.Chaps && account.Status != AccountStatus.Live)
        {
            return false;
        }

        return true;
    }
}
