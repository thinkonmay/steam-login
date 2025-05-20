using System.Buffers.Text;
using System.Diagnostics;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using Microsoft.Win32;
using Win32Interop.WinHandles;


var path = GetSteamPath();
var dir = Path.GetDirectoryName(path);

IEnumerable<Process> GetChildProcesses(Process process)
{
    List<Process> children = new List<Process>();
    try
    {
        ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));
        foreach (ManagementObject mo in mos.Get())
        {
            children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
        }
    }
    catch(Exception e) { 
        Console.WriteLine("get child process failed");
    }

    return children;
}

IEnumerable<IntPtr> EnumerateProcessWindowHandles(Process process)
{
    var handles = new List<IntPtr>();

    try {
        foreach (ProcessThread thread in process.Threads)
            DLL.EnumThreadWindows(thread.Id, (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);
    } catch {
        Console.WriteLine("get window handle failed");
    }

    return handles;
}

void ClearSteamUserDataFolder(string steamPath)
{
    string path = steamPath + "\\userdata";
    if (Directory.Exists(path)) {
        Console.WriteLine("Deleting userdata files...");
        Directory.Delete(path, true);
        Console.WriteLine("userdata files deleted!");
    } else {
        Console.WriteLine("userdata directory not found.");
    }
}

void ClearAutoLoginUserKeyValues()
{
    RegistryKey localKey;
    if (Environment.Is64BitOperatingSystem) {
        localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
    } else {
        localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
    }

    localKey = localKey.OpenSubKey(@"Software\\Valve\\Steam", true);
    localKey.SetValue("AutoLoginUser", "", RegistryValueKind.String);
    localKey.SetValue("RememberPassword", 0, RegistryValueKind.DWord);
    localKey.Close();
}

string GetSteamPath()
{
    RegistryKey localKey = Registry.ClassesRoot;
    var local = localKey.OpenSubKey(@"steam\\Shell\\Open\\Command",RegistryRights.QueryValues);
    var val = local.GetValue("");
    local.Close();
    return val.ToString().Split("\"")[1];
}

void RegisterCustomURL()
{
    var  path = Process.GetCurrentProcess().MainModule.FileName;
    var ukey = Registry.CurrentUser.OpenSubKey("Software", true);
    ukey = ukey.OpenSubKey("Classes", true);

    var key = ukey.CreateSubKey("thinkmay");
    key.SetValue("URL Protocol", "");
    key.SetValue("", "thinkmay procotol");
    key.CreateSubKey(@"shell\open\command").SetValue(string.Empty, path + " customurl %1");

    key.Close();
    ukey.Close();
    return;
}

bool IterateElements(AutomationElement element, Func<AutomationElement,bool> fun) {
    try {
        if (element == null)
            return true;
        else if (!fun(element))
            return false;
    } catch {
        return true;
    }

    AutomationElement[] elements = [];
    try {
        elements = element.FindAllChildren();
    } catch {
        return true;
    }

    foreach (var item in elements)
    {
        if (!IterateElements(item,fun))
            return false;
    }

    return true;
}

void InvokeButton(string name) {
    var automation = new UIA3Automation();
    foreach (var proc in Process.GetProcessesByName("steam"))
    {
        foreach (var child in GetChildProcesses(proc))
        {
            foreach (var hndl in EnumerateProcessWindowHandles(child))
            {
                try {
                    var window = automation.FromHandle(new WindowHandle(hndl).RawPtr);
                    if (window.Name == "")
                        continue;
                    else if (window.Name == "Steam Settings" || window.Name == "Friends List") {
                        window.AsWindow().Close();
                        continue;
                    }

                    window.Focus();
                    IterateElements(window,element => {
                        if (element.Name.ToLower()  == (name)) {
                            Console.WriteLine(element.Name);
                            element.Click();
                            return false;
                        }

                        return true;
                    });
                } catch (Exception e){
                    Console.WriteLine(e.Message + e.StackTrace);

                }
            }
        }
    }
}

bool SteamIsRunning() {
    return Process.GetProcessesByName("steam").Length > 0;
}

List<string> GetAllElements() {
    var res = new List<string>();
    var automation = new UIA3Automation();
    foreach (var proc in Process.GetProcessesByName("steam"))
    {
        foreach (var child in GetChildProcesses(proc))
        {
            foreach (var hndl in EnumerateProcessWindowHandles(child))
            {
                try {
                    var window = automation.FromHandle(new WindowHandle(hndl).RawPtr);
                    if (window == null)
                        continue;
                    else if (window.Name == "")
                        continue;
                    else if (window.Name == "MSCTFIME UI" || 
                             window.Name == "Default IME") 
                        continue;
                    else if (window.Name == "Steam Settings" || 
                             window.Name == "Friends List") {
                        window.AsWindow().Close();
                        continue;
                    }

                    window.Focus();
                    IterateElements(window,element => {
                        if (element.Name != null){
                            res.Add(element.Name);
                        }

                        return true;
                    });
                } catch (Exception e){}
            }
        }
    }

    return res;
}


void FillTextBox(string prev_element,string val) {
    var automation = new UIA3Automation();
    foreach (var proc in Process.GetProcessesByName("steam"))
    {
        foreach (var child in GetChildProcesses(proc))
        {
            foreach (var hndl in EnumerateProcessWindowHandles(child))
            {
                try {
                    var window = automation.FromHandle(new WindowHandle(hndl).RawPtr);
                    if (window.Name == "")
                        continue;
                    else if (window.Name == "MSCTFIME UI" || 
                             window.Name == "Default IME") 
                        continue;
                    else if (window.Name == "Steam Settings" || 
                             window.Name == "Friends List") {
                        window.AsWindow().Close();
                        continue;
                    }


                    window.Focus();

                    AutomationElement? prev = window;
                    IterateElements(window,element => {
                        if (element.ControlType == ControlType.Edit && prev.Name.Contains(prev_element)) {
                            TextBox box = element.AsTextBox();
                            box.WaitUntilEnabled();
                            box.Text = val;
                            return false;
                        }


                        prev = element;
                        return true;
                    });
                } catch (Exception e){
                    Console.WriteLine(e.Message + e.StackTrace);

                }
            }
        }
    }
}

void WaitProcessExit() {
    foreach (var proc in Process.GetProcessesByName("steam"))
    {
        foreach (var child in GetChildProcesses(proc))
        {
            child.WaitForExit();
        }
        proc.WaitForExit();
    }
}
void ClickButton(string name) {
    var automation = new UIA3Automation();
    foreach (var proc in Process.GetProcessesByName("steam"))
    {
        foreach (var child in GetChildProcesses(proc))
        {
            foreach (var hndl in EnumerateProcessWindowHandles(child))
            {
                try {
                    var window = automation.FromHandle(new WindowHandle(hndl).RawPtr);
                    if (window.Name == "")
                        continue;
                    else if (window.Name == "MSCTFIME UI" || 
                             window.Name == "Default IME") 
                        continue;
                    else if (window.Name == "Steam Settings" || 
                             window.Name == "Friends List") {
                        window.AsWindow().Close();
                        continue;
                    }


                    window.Focus();

                    IterateElements(window,element => {
                        if (element.ControlType == ControlType.Button && element.Name.Contains(name)) {
                            
                            var box = element.AsButton();
                            box.Focus();
                            box.WaitUntilEnabled();
                            box.Invoke();
                            return false;
                        }

                        return true;
                    });
                } catch (Exception e){
                    Console.WriteLine(e.Message + e.StackTrace);

                }
            }
        }
    }
}
void ClickButtonPrev(string name) {
    var automation = new UIA3Automation();
    foreach (var proc in Process.GetProcessesByName("steam"))
    {
        foreach (var child in GetChildProcesses(proc))
        {
            foreach (var hndl in EnumerateProcessWindowHandles(child))
            {
                try {
                    var window = automation.FromHandle(new WindowHandle(hndl).RawPtr);
                    if (window.Name == "")
                        continue;
                    else if (window.Name == "MSCTFIME UI" || 
                             window.Name == "Default IME") 
                        continue;
                    else if (window.Name == "Steam Settings" || 
                             window.Name == "Friends List") {
                        window.AsWindow().Close();
                        continue;
                    }


                    window.Focus();

                    AutomationElement? group = null;
                    IterateElements(window,element => {
                        if (element.ControlType == ControlType.Group && element.Name.Contains(name)) {
                            group = element;
                            return false;
                        }

                        return true;
                    });

                    if (group == null) {
                        Console.WriteLine("group not found");
                        return;
                    }

                    IterateElements(group,element => {
                        if (element.ControlType == ControlType.Image) {
                            var box = element.AsButton();
                            box.Invoke();
                            return false;
                        }

                        return true;
                    });


                } catch (Exception e){
                    Console.WriteLine(e.Message + e.StackTrace);

                }
            }
        }
    }
}

void Close() {
    var stopInfo = new ProcessStartInfo{
        FileName =  path,
        WorkingDirectory = dir,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        Arguments = "-shutdown",
    };


    Process.Start(stopInfo).WaitForExit();
    WaitProcessExit();
}

void Localize(string elementId){
    // read all file in list /localization
    // find the id in json file match with elementId
    // get specific value for every language
    // => find match value and return true;
}


bool
Login(string username, string password) {
    if (SteamIsRunning())
        Close();

    ClearSteamUserDataFolder(dir);
    Process.Start(new ProcessStartInfo{
        FileName =  path,
        WorkingDirectory = dir,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        Arguments = "",
    });


    var time = DateTime.Now;
    while ((DateTime.Now - time) < TimeSpan.FromMinutes(3)) {
        Thread.Sleep(100);
        var all = GetAllElements();
        if (all.Count == 0)
            continue;
        if (CheckExistKey(all, "Login_CheckCredentials") || 
            CheckExistKey(all, "Login_Error_Network_Title")){
            Console.WriteLine("Login failed");
            Close();
            return false;
        } else if (CheckExistKey(all, "Login_AddAccount")){
            Console.WriteLine("Picking account");
            ClickButtonPrev(FindKey(all, "Login_AddAccount"));
        } else if (CheckExistKey(all, "Login_LoadingLibrary")){
            Console.WriteLine("Logging in");
        } else if (CheckExistKey(all, "Login_WaitingForServer")){ 
            Console.WriteLine("Logging in"); 
        } else if (CheckExistKey(all, "Login_SignIn_WithAccountName", true)){
            Console.WriteLine("Filling signin window");
            FillTextBox(FindKey(all, "Login_SignIn_WithAccountName", true), username);
            FillTextBox(FindKey(all, "Login_Password", true), password);
            ClickButton(FindKey(all, "Login_SignIn")); 
        } else if (all.Any(x => x.Contains("LIBRARY")) || all.Any(x => x.Contains("STORE"))){


            InvokeButton("LIBRARY"); //todo: find the new way to invoke this button, such as script import
            Console.WriteLine("Login success");
            return true;
        }
    }

    Console.WriteLine("Timeout login to account");
    return false;
}

string Base64Encode(string plainText) 
{
    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
    return System.Convert.ToBase64String(plainTextBytes);
}

string Base64Decode(string base64EncodedData) 
{
    var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
    return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
}

 var translations = LocalizationHelper.GetLocalizedValues("SignIn_Title");


bool CheckExistKey(List<string> all, string key, bool isUpperCase = false)
{
    if (all.Count == 0)
        return false;

    var trans = LocalizationHelper.GetLocalizedValues(key);
    if (isUpperCase)
        trans = trans.Select(x => x.ToUpper()).ToArray();

    for (int i = 0; i < all.Count; i++){
        string content = all.ElementAt(i);

        if (content == "")
            continue;

        if(trans.Any(x => x.Equals(content))){
            return true;
        }
    }

    return false;
}

string FindKey(List<string> all, string key, bool isUpperCase = false)
{

    if (all.Count == 0)
        return "";

    var trans = LocalizationHelper.GetLocalizedValues(key);
    if (isUpperCase)
        trans = trans.Select(x => x.ToUpper()).ToArray();

    for (int i = 0; i < all.Count; i++){
        string content = all.ElementAt(i);

        if (content == "")
            continue;

        if(trans.Any(x => x.Equals(content))){
            return content;
        }

    }

    return "";
}

if (args.Length == 3 && args[0] == "login"){
    if (Login(args[1],args[2]))
        Environment.Exit(0);
    else 
        Environment.Exit(-1);
} else if (args.Length == 1 && args[0] == "logout")
    Close();