using ClearBank.DeveloperTest.Data;

namespace ClearBank.DeveloperTest.Factories;

public class AccountDataStoreFactory : IAccountDataStoreFactory
{
    public IAccountDataStore Create(AccountDataStoreType accountDataStoreType)
    {
        if (accountDataStoreType == AccountDataStoreType.Backup)
        {
            return new BackupAccountDataStore();
        }

        return new AccountDataStore();
    }
}
