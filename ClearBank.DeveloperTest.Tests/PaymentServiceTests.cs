using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using ClearBank.DeveloperTest.Services;
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
}
