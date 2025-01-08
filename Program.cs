using System.Diagnostics;
using System.Management;
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
                        if (element.Name != null)
                            res.Add(element.Name);

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

if (args.Length == 3 && args[0] == "login") {
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


    for (;;) {
        Thread.Sleep(100);
        var all = GetAllElements();
        if (all.Any(x => x.Contains("Please check your password")) || 
            all.Any(x => x.Contains("Connection Problem"))){
            Console.WriteLine("Login failed");
            Close();
            break;
        } else if (all.Any(x => x.Contains("Add Account"))){
            Console.WriteLine("Picking account");
            ClickButtonPrev("Add Account");
        } else if (all.Any(x => x.Contains("Loading user data"))){
            Console.WriteLine("Logging in");
        } else if (all.Any(x => x.Contains("Logging in"))){
            Console.WriteLine("Logging in");
        } else if (all.Any(x => x.Contains("SIGN IN WITH ACCOUNT NAME"))){
            Console.WriteLine("Filling signin window");
            FillTextBox("SIGN IN WITH ACCOUNT NAME",args[1]);
            FillTextBox("PASSWORD",args[2]);
            ClickButton("Sign in");
        } else if (all.Any(x => x.Contains("LIBRARY")) || all.Any(x => x.Contains("STORE"))){
            InvokeButton("LIBRARY");
            Console.WriteLine("Login success");
            break;
        }
    }
} else if (args.Length == 1 && args[0] == "logout"){
    Close();
    Console.WriteLine("Shutdown success");
} else 
    Console.WriteLine("login or logout");