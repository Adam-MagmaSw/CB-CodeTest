using System;

namespace ClearBank.DeveloperTest.Types;

internal static class PaymentSchemeConverter
{
    internal static AllowedPaymentSchemes ToAllowedPaymentSchemes(this PaymentScheme paymentScheme)
    {
        return paymentScheme switch
        {
            PaymentScheme.Bacs => AllowedPaymentSchemes.Bacs,
            PaymentScheme.FasterPayments => AllowedPaymentSchemes.FasterPayments,
            PaymentScheme.Chaps => AllowedPaymentSchemes.Chaps,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentScheme), $"Unexpected payment scheme value: {paymentScheme}"),
        };
    }
}
