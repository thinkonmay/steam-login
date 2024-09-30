using Core;

if (args.Length >= 2) // Check if at least two arguments are provided   
{
    string username = args[0]; // First argument as username
    string password = args[1]; // Second argument as password

    var acc = new Account
    {
        Name = username,
        Password = password
    };

    try {
        WindowUtils.Login(acc,1);
    } catch (Exception e) {
        Console.WriteLine(e);
    }
}
else
{
    Console.WriteLine("Please provide both username and password.");
}