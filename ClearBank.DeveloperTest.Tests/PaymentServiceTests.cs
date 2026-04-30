using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using NSubstitute;
using NUnit.Framework;
using System;

namespace ClearBank.DeveloperTest.Tests;

[TestFixture]
public class PaymentServiceTests
{
    [Test]
    public void Constructor_WhenAccountDataStoreFactoryIsNull_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() =>
            _ = new PaymentService(AccountDataStoreType.Default, null));

        Assert.That(exception!.ParamName, Is.EqualTo("accountDataStoreFactory"));
    }

    [Test]
    public void Constructor_WhenConstructed_CallsFactoryWithProvidedAccountDataStoreType()
    {
        var expectedType = AccountDataStoreType.Backup;
        var expectedDataStore = Substitute.For<IAccountDataStore>();
        var factory = Substitute.For<IAccountDataStoreFactory>();
        factory.Create(expectedType).Returns(expectedDataStore);

        _ = new PaymentService(expectedType, factory);

        factory.Received(1).Create(expectedType);
    }

    [TestCase(PaymentScheme.Bacs)]
    [TestCase(PaymentScheme.FasterPayments)]
    [TestCase(PaymentScheme.Chaps)]
    public void MakePayment_WhenAccountDoesNotExist_ReturnsFailureAndDoesNotUpdateAccount(PaymentScheme paymentScheme)
    {
        var dataStore = Substitute.For<IAccountDataStore>();
        dataStore.GetAccount("missing").Returns((Account)null);
        var service = CreateService(dataStore);

        var result = service.MakePayment(new MakePaymentRequest
        {
            DebtorAccountNumber = "missing",
            Amount = 10m,
            PaymentScheme = paymentScheme
        });

        Assert.That(result.Success, Is.False);
        dataStore.DidNotReceive().UpdateAccount(Arg.Any<Account>());
    }

    [TestCase(PaymentScheme.Bacs, AllowedPaymentSchemes.FasterPayments)]
    [TestCase(PaymentScheme.Bacs, AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.Bacs, AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.FasterPayments, AllowedPaymentSchemes.Bacs)]
    [TestCase(PaymentScheme.FasterPayments, AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.FasterPayments, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.Chaps, AllowedPaymentSchemes.FasterPayments)]
    [TestCase(PaymentScheme.Chaps, AllowedPaymentSchemes.Bacs)]
    [TestCase(PaymentScheme.Chaps, AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Bacs)]
    public void MakePayment_WhenAccountDoesNotAllowPaymentScheme_ReturnsFailureAndDoesNotUpdateAccount(PaymentScheme paymentScheme, AllowedPaymentSchemes allowedPaymentSchemes)
    {
        var account = new Account("123ABC", 100m, AccountStatus.Live, allowedPaymentSchemes);
        var dataStore = Substitute.For<IAccountDataStore>();
        dataStore.GetAccount("debtor-1").Returns(account);
        var service = CreateService(dataStore);

        var result = service.MakePayment(new MakePaymentRequest
        {
            DebtorAccountNumber = "debtor-1",
            Amount = 25m,
            PaymentScheme = paymentScheme
        });

        Assert.That(result.Success, Is.False);
        Assert.That(account.Balance, Is.EqualTo(100m));
        dataStore.DidNotReceive().UpdateAccount(Arg.Any<Account>());
    }

    [Test]
    public void MakePayment_WhenFasterPaymentsBalanceIsInsufficient_ReturnsFailureAndDoesNotUpdateAccount()
    {
        var account = new Account("123ABC", 19.99m, AccountStatus.Live, AllowedPaymentSchemes.FasterPayments);
        var dataStore = Substitute.For<IAccountDataStore>();
        dataStore.GetAccount("debtor-2").Returns(account);
        var service = CreateService(dataStore);

        var result = service.MakePayment(new MakePaymentRequest
        {
            DebtorAccountNumber = "debtor-2",
            Amount = 20m,
            PaymentScheme = PaymentScheme.FasterPayments
        });

        Assert.That(result.Success, Is.False);
        Assert.That(account.Balance, Is.EqualTo(19.99m));
        dataStore.DidNotReceive().UpdateAccount(Arg.Any<Account>());
    }

    [TestCase(AccountStatus.InboundPaymentsOnly)]
    [TestCase(AccountStatus.Disabled)]
    public void MakePayment_WhenChapsAccountIsNotLive_ReturnsFailureAndDoesNotUpdateAccount(AccountStatus accountStatus)
    {
        var account = new Account("123ABC", 100m, accountStatus, AllowedPaymentSchemes.Chaps);
        var dataStore = Substitute.For<IAccountDataStore>();
        dataStore.GetAccount("debtor-3").Returns(account);
        var service = CreateService(dataStore);

        var result = service.MakePayment(new MakePaymentRequest
        {
            DebtorAccountNumber = "debtor-3",
            Amount = 20m,
            PaymentScheme = PaymentScheme.Chaps
        });

        Assert.That(result.Success, Is.False);
        Assert.That(account.Balance, Is.EqualTo(100m));
        dataStore.DidNotReceive().UpdateAccount(Arg.Any<Account>());
    }

    [TestCase(PaymentScheme.Bacs, AllowedPaymentSchemes.Bacs)]
    [TestCase(PaymentScheme.Bacs, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.Bacs, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.FasterPayments, AllowedPaymentSchemes.FasterPayments)]
    [TestCase(PaymentScheme.FasterPayments, AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.FasterPayments, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.Chaps, AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.Chaps, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps)]
    [TestCase(PaymentScheme.Chaps, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps)]
    public void MakePayment_WhenAccountAllowsPaymentSchemeHasEnoughBalanceAndIsLive_ReturnsSuccessAndUpdatesAccount(PaymentScheme paymentScheme, AllowedPaymentSchemes allowedPaymentSchemes)
    {
        var account = new Account("123ABC", 100m, AccountStatus.Live, allowedPaymentSchemes);
        var dataStore = Substitute.For<IAccountDataStore>();
        dataStore.GetAccount("debtor-1").Returns(account);
        var service = CreateService(dataStore);

        var result = service.MakePayment(new MakePaymentRequest
        {
            DebtorAccountNumber = "debtor-1",
            Amount = 99.99m,
            PaymentScheme = paymentScheme
        });

        Assert.That(result.Success, Is.True);
        Assert.That(account.Balance, Is.EqualTo(0.01m));
        dataStore.Received(1).GetAccount("debtor-1");
        dataStore.Received(1).UpdateAccount(account);
    }

    private static PaymentService CreateService(IAccountDataStore dataStore)
    {
        var factory = Substitute.For<IAccountDataStoreFactory>();
        factory.Create(AccountDataStoreType.Default).Returns(dataStore);
        return new PaymentService(AccountDataStoreType.Default, factory);
    }
}
