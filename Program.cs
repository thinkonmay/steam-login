using Core;


if (args.Length == 0) {
    Console.WriteLine("login or logout");
} else if (args.Length > 0 ) {
    if (args[0] == "login") {
        WindowUtils.Login(new Account {
            Name = args[1],
            Password = args[2]
        });
    } else if (args[0] == "logout"){
        WindowUtils.Logout();
    }
}