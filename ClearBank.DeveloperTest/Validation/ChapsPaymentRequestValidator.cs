using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation;

internal static class ChapsPaymentRequestValidator
{
    public static bool IsPaymentRequestValidForAccount(MakePaymentRequest request, Account account)
    {
        return AccountValidator.AccountExistsAndAllowsScheme(account, request.PaymentScheme) && account.Status == AccountStatus.Live;
    }
}
