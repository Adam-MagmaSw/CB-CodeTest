using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation;

internal static class FasterPaymentPaymentRequestValidator
{
    public static bool IsPaymentRequestValidForAccount(MakePaymentRequest request, Account account)
    {
        return AccountValidator.AccountExistsAndAllowsScheme(account, request.PaymentScheme) && account.Balance >= request.Amount;
    }
}
