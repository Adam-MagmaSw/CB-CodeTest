### Test Description

In the 'PaymentService.cs' file you will find a method for making a payment. At a high level the steps for making a payment are:

- Lookup the account the payment is being made from
- Check the account is in a valid state to make the payment
- Deduct the payment amount from the account's balance and update the account in the database

What we’d like you to do is refactor the code with the following things in mind:

- Adherence to SOLID principals
- Testability
- Readability

We’d also like you to add some unit tests to the ClearBank.DeveloperTest.Tests project to show how you would test the code that you’ve produced. The only specific ‘rules’ are:

- The solution should build.
- The tests should all pass.
- You should not change the method signature of the MakePayment method.

You are free to use any frameworks/NuGet packages that you see fit.

You should plan to spend around 1 to 3 hours to complete the exercise.

### What I did

1. Initially I refactored the PaymentService to allow it to be unit tested. For this I added an interface to the AccountDataStore & BackupAccountDataStore and created a factory (with unit tests) which now owns the logic around mapping from the DataStoreType to the specific AccountDataStore. I felt it was a smell for a library to be dependent on the consuming applications config file directly (this is an implicit dependency of the library which wouldn't be obvious without consumers having access to the source code, which is dangerous. It's also very limiting and forces a consumer to have an old XML config file). Based on that I created an enum for the DataStoreType and would expect the consuming application to determine the value however it sees fit (i.e. from the config file it owns or from whatever means necessary).

2. I then created a constructor (with unit tests) for the PaymentService which utilised the new AccountDataStoreFactory. This constructor means the dependencies for the PaymentService can now all be injected.

3. At this point I did some minimal refactoring of the PaymentService such that it was using the injected dependencies so it could be unit tested. Now the logic within the PaymentService is unit testable I added unit tests covering all the code paths.

4. Now the PaymentService is fully covered with tests I could refactor freely. I split out the validation and debit-execution logic from the MakePayment method to make it more readable and to make it better adhere to SRP. I also consolidated the duplicated logic which checked the account exists and supports the payment scheme for each scheme.

5. Finally although I'd split the validation logic out from the MakePayment method it still resided in the PaymentService class. From an SRP point of view I didn't feel like it made sense for this class to own that. To improve this I added a set a validation classes such that there's a class that owns the validation logic for each payment scheme. I chose not to unit test this validation logic separately as these classes are merely internal utilities of the PaymentService and are still fully covered by those unit tests (at this point those unit tests are sociable rather than solitary unit tests, as was the case initially). I also chose not to inject the validators - in my experience DI setup is one of the biggest causes of bugs as it's difficult to unit test and also can make the code difficult to read (as the reader has to work out what would be injected) so unless it's absolutely necessary I avoid it. I didn't feel injecting the validation classes was necessary as I didn't feel there was enough value in Verifying any of the validation method calls. At this point I then tidied up the MakePayment method to make it as readable as possible. I also added a Debit method to the Account class so it owns the logic around how a debit is performed (Tell don't ask).
