using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Factories;
using NUnit.Framework;

namespace ClearBank.DeveloperTest.Tests;

[TestFixture]
public class AccountDataStoreFactoryTests
{
    [Test]
    public void Create_WhenAccountDataStoreTypeIsBackup_ReturnsBackupAccountDataStore()
    {
        var factory = new AccountDataStoreFactory();

        var result = factory.Create(AccountDataStoreType.Backup);

        Assert.That(result, Is.TypeOf<BackupAccountDataStore>());
    }

    [Test]
    public void Create_WhenAccountDataStoreTypeIsDefault_ReturnsAccountDataStore()
    {
        var factory = new AccountDataStoreFactory();

        var result = factory.Create(AccountDataStoreType.Default);

        Assert.That(result, Is.TypeOf<AccountDataStore>());
    }
}
