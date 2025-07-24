﻿using System.Diagnostics;
using System.Management;
using System.Security.AccessControl;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using Microsoft.Win32;
using Win32Interop.WinHandles;


var path = GetSteamPath();
var dir = Path.GetDirectoryName(path);
var locale = new LocalizationHelper("localization");

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
    if (Environment.Is64BitOperatingSystem)
    {
        localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
    }
    else
    {
        localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
    }

    localKey = localKey.OpenSubKey(@"Software\\Valve\\Steam", true);
    localKey.SetValue("AutoLoginUser", "", RegistryValueKind.String);
    localKey.SetValue("RememberPassword", 0, RegistryValueKind.DWord);
    localKey.Close();
}

void DeleteSteamLoginUsers()
{
    string steamPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "config");
    string loginUsersFile = Path.Combine(steamPath, "loginusers.vdf");
    if (File.Exists(loginUsersFile)) {
        File.Delete(loginUsersFile);
        Console.WriteLine("Deleted loginusers.vdf");
    } else Console.WriteLine($"unable to find steam at path {loginUsersFile}");
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
                        if (element.Name.ToLower()  == name) {
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

AutomationElement? GetRawElement(string name) {
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

                    AutomationElement? ele = null;
                    IterateElements(window,element => {
                        if (element.ControlType == ControlType.Button && element.Name.Contains(name)) {
                            ele = element;
                            return false;
                        }

                        return true;
                    });

                    if (ele != null) {
                        return ele;
                    }
                } catch (Exception e){
                    Console.WriteLine(e.Message + e.StackTrace);
                }
            }
        }
    }

    return null;
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
    DeleteSteamLoginUsers();
    ClearAutoLoginUserKeyValues();
    var stopInfo = new ProcessStartInfo{
        FileName =  path,
        WorkingDirectory = dir,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        Arguments = "-shutdown",
    };


    Process.Start(stopInfo)?.WaitForExit();
    WaitProcessExit();
}



bool
Login(string username, string password) {
    DeleteSteamLoginUsers();
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
        } else if (all.Any(x => x.Contains("LIBRARY")) || all.Any(x => x.Contains("STORE")) || all.Any(x => x.Contains("Menu"))){

            
            InvokeButton("LIBRARY"); //todo: find the new way to invoke this button, such as script import
            DeleteSteamLoginUsers();
            Console.WriteLine("Login success");
            return true;
        }
    }

    Console.WriteLine("Timeout login to account");
    return false;
}


bool CheckExistKey(List<string> all, string key, bool isUpperCase = false)
{
    if (all.Count == 0)
        return false;

    var trans = locale.GetLocalizedValues(key);
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

    var trans = locale.GetLocalizedValues(key);
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

    Console.WriteLine($"failed to find key {key}");
    return "";
}

void PrintAllElements() 
{
    var all = GetAllElements();

    // var name = FindKey(all,"Login_RememberMe_Short");
    // if(name != "") {
    //     var element = GetRawElement(name);
    //     if (element != null) Console.WriteLine($"{element.ControlType}");
    // }

    all.ForEach(x => Console.WriteLine($"Element name: {x}"));
}


if (args.Length == 3 && args[0] == "login"){
    if (Login(args[1],args[2]))
        Environment.Exit(0);
    else 
        Environment.Exit(-1);
} else if (args.Length == 1 && args[0] == "logout")
    Close();
else if (args.Length == 1 && args[0] == "path")
    Console.WriteLine($"{path}");
else if (args.Length == 1 && args[0] == "print")
    PrintAllElements();