using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Validation;

internal static class AccountValidator
{
    public static bool AccountExistsAndAllowsScheme(Account account, PaymentScheme paymentScheme)
    {
        if (account == null)
        {
            return false;
        }

        if (!account.AllowedPaymentSchemes.HasFlag(paymentScheme.ToAllowedPaymentSchemes()))
        {
            return false;
        }

        return true;
    }
}
