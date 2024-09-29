
using Core;

var acc = new Account
{
    Name = "",
    Password = ""
};


try {
    Console.WriteLine("start login");
    WindowUtils.Login(acc,1);
} catch (Exception e) {
    Console.WriteLine(e);
}