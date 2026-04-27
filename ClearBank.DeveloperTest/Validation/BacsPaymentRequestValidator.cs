using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation;

internal static class BacsPaymentRequestValidator
{
    public static bool IsPaymentRequestValidForAccount(MakePaymentRequest request, Account account)
    {
        return AccountValidator.AccountExistsAndAllowsScheme(account, request.PaymentScheme);
    }
}
