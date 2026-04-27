using ClearBank.DeveloperTest.Types;
using System;

namespace ClearBank.DeveloperTest.Validation;

internal static class PaymentRequestValidator
{
    public static bool IsPaymentRequestValidForAccount (MakePaymentRequest request, Account account)
    {
        return request.PaymentScheme switch
        {
            PaymentScheme.Bacs => BacsPaymentRequestValidator.IsPaymentRequestValidForAccount(request, account),
            PaymentScheme.FasterPayments => FasterPaymentPaymentRequestValidator.IsPaymentRequestValidForAccount(request, account),
            PaymentScheme.Chaps => ChapsPaymentRequestValidator.IsPaymentRequestValidForAccount(request, account),
            _ => throw new ArgumentOutOfRangeException(nameof(request.PaymentScheme), $"Not expected payment scheme value: {request.PaymentScheme}"),
        };
    }
}
