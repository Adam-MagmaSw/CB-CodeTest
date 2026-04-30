namespace ClearBank.DeveloperTest.Types;

public class MakePaymentResult
{
    public MakePaymentResult(bool success)
    {
        Success = success;
    }

    public bool Success { get; }

    public static MakePaymentResult SuccessResult() => new MakePaymentResult(true);

    public static MakePaymentResult FailResult() => new MakePaymentResult(false);
}
