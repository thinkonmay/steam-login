using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SteamAuth;
using System.Management;
using Win32Interop.WinHandles;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

namespace Core
{
    class SAMSettings
    {
        public const string FILE_NAME = "SAMSettings.ini";

        public const string SECTION_SYSTEM = "System";
        public const string SECTION_GENERAL = "Settings";
        public const string SECTION_AUTOLOG = "AutoLog";
        public const string SECTION_CUSTOMIZE = "Customize";
        public const string SECTION_STEAM = "Steam";
        public const string SECTION_PARAMETERS = "Parameters";
        public const string SECTION_LOCATION = "Location";
        public const string SECTION_COLUMNS = "Columns";

        public List<string> globalParameters;

        public const string VERSION = "Version";

        public const string CLEAR_USER_DATA = "ClearUserData";
        public const string HIDE_ADD_BUTTON = "HideAddButton";
        public const string PASSWORD_PROTECT = "PasswordProtect";
        public const string MINIMIZE_TO_TRAY = "MinimizeToTray";
        public const string REMEMBER_PASSWORD = "RememberPassword";
        public const string START_MINIMIZED = "StartMinimized";
        public const string START_WITH_WINDOWS = "StartWithWindows";
        public const string ACCOUNTS_PER_ROW = "AccountsPerRow";
        public const string SLEEP_TIME = "SleepTime";
        public const string CHECK_FOR_UPDATES = "CheckForUpdates";
        public const string CLOSE_ON_LOGIN = "CloseOnLogin";
        public const string LIST_VIEW = "ListView";
        public const string SANDBOX_MODE = "SandboxMode";
        public const string HEADERLESS_WINDOW = "HeaderlessWindow";
        public const string TRANSPARENT_WINDOW = "TransparentWindow";

        public const string LOGIN_RECENT_ACCOUNT = "LoginRecentAccount";
        public const string RECENT_ACCOUNT_INDEX = "RecentAccountIndex";
        public const string LOGIN_SELECTED_ACCOUNT = "LoginSelectedAccount";
        public const string SELECTED_ACCOUNT_INDEX = "SelectedAccountIndex";
        public const string INPUT_METHOD = "InputMethod";
        public const string HANDLE_IME = "HandleIME";
        public const string IME_2FA_ONLY = "IME_2FA_ONLY";

        public const string THEME = "Theme";
        public const string ACCENT = "Accent";
        public const string BUTTON_SIZE = "ButtonSize";
        public const string BUTTON_COLOR = "ButtonColor";
        public const string BUTTON_FONT_SIZE = "ButtonFontSize";
        public const string BUTTON_FONT_COLOR = "ButtonFontColor";
        public const string BUTTON_BANNER_COLOR = "ButtonBannerColor";
        public const string BUTTON_BANNER_FONT_SIZE = "ButtonBannerFontSize";
        public const string BUTTON_BANNER_FONT_COLOR = "ButtonBannerFontColor";
        public const string HIDE_BAN_ICONS = "HideBanIcons";

        public const string STEAM_PATH = "Path";
        public const string STEAM_API_KEY = "ApiKey";
        public const string AUTO_RELOAD_ENABLED = "AutoReloadEnabled";
        public const string AUTO_RELOAD_INTERVAL = "AutoReloadInterval";
        public const string LAST_AUTO_RELOAD = "LastAutoReload";

        public const string CAFE_APP_LAUNCH_PARAMETER = "cafeapplaunch";
        public const string CLEAR_BETA_PARAMETER = "clearbeta";
        public const string CONSOLE_PARAMETER = "console";
        public const string DEVELOPER_PARAMETER = "dev";
        public const string FORCE_SERVICE_PARAMETER = "forceservice";
        public const string LOGIN_PARAMETER = "login";
        public const string NO_CACHE_PARAMETER = "nocache";
        public const string NO_VERIFY_FILES_PARAMETER = "noverifyfiles";
        public const string SILENT_PARAMETER = "silent";
        public const string SINGLE_CORE_PARAMETER = "single_core";
        public const string TCP_PARAMETER = "tcp";
        public const string TEN_FOOT_PARAMETER = "tenfoot";
        public const string CUSTOM_PARAMETERS = "customParameters";
        public const string CUSTOM_PARAMETERS_VALUE = "customParametersValue";

        public const string WINDOW_TOP = "WindowTop";
        public const string WINDOW_LEFT = "WindowLeft";
        public const string LIST_VIEW_HEIGHT = "ListViewHeight";
        public const string LIST_VIEW_WIDTH = "ListViewWidth";

        public const string LIGHT_THEME = "Light";
        public const string DARK_THEME = "Dark";

        public const string NAME_COLUMN_INDEX = "NameColumnIndex";
        public const string DESCRIPTION_COLUMN_INDEX = "DescriptionColumnIndex";
        public const string TIMEOUT_COLUMN_INDEX = "TimeoutColumnIndex";
        public const string VAC_BANS_COLUMN_INDEX = "VacBansColumnIndex";
        public const string GAME_BANS_COLUMN_INDEX = "GameBansColumnIndex";
        public const string ECO_BAN_COLUMN_INDEX = "EcoBanColumnIndex";
        public const string LAST_BAN_COLUMN_INDEX = "LastBanColumnIndex";

        public Dictionary<string, string> KeyValuePairs = new Dictionary<string, string>()
        {
            { VERSION, SECTION_SYSTEM },

            { CLEAR_USER_DATA, SECTION_GENERAL },
            { HIDE_ADD_BUTTON,  SECTION_GENERAL },
            { PASSWORD_PROTECT, SECTION_GENERAL },
            { MINIMIZE_TO_TRAY, SECTION_GENERAL },
            { REMEMBER_PASSWORD, SECTION_GENERAL },
            { START_MINIMIZED, SECTION_GENERAL },
            { START_WITH_WINDOWS, SECTION_GENERAL },
            { ACCOUNTS_PER_ROW, SECTION_GENERAL },
            { SLEEP_TIME, SECTION_GENERAL },
            { CHECK_FOR_UPDATES, SECTION_GENERAL },
            { CLOSE_ON_LOGIN, SECTION_GENERAL },
            { LIST_VIEW, SECTION_GENERAL },
            { SANDBOX_MODE, SECTION_GENERAL },
            { HEADERLESS_WINDOW, SECTION_GENERAL},
            { TRANSPARENT_WINDOW, SECTION_GENERAL },

            { LOGIN_RECENT_ACCOUNT, SECTION_AUTOLOG },
            { RECENT_ACCOUNT_INDEX, SECTION_AUTOLOG },
            { LOGIN_SELECTED_ACCOUNT, SECTION_AUTOLOG },
            { SELECTED_ACCOUNT_INDEX, SECTION_AUTOLOG },
            { INPUT_METHOD, SECTION_AUTOLOG },
            { HANDLE_IME, SECTION_AUTOLOG },
            { IME_2FA_ONLY, SECTION_AUTOLOG },

            { THEME, SECTION_CUSTOMIZE },
            { ACCENT, SECTION_CUSTOMIZE },
            { BUTTON_SIZE, SECTION_CUSTOMIZE },
            { BUTTON_COLOR, SECTION_CUSTOMIZE },
            { BUTTON_FONT_SIZE, SECTION_CUSTOMIZE },
            { BUTTON_FONT_COLOR, SECTION_CUSTOMIZE },
            { BUTTON_BANNER_COLOR, SECTION_CUSTOMIZE },
            { BUTTON_BANNER_FONT_SIZE, SECTION_CUSTOMIZE },
            { BUTTON_BANNER_FONT_COLOR, SECTION_CUSTOMIZE },
            { HIDE_BAN_ICONS, SECTION_CUSTOMIZE },

            { STEAM_PATH, SECTION_STEAM },
            { STEAM_API_KEY, SECTION_STEAM },
            { AUTO_RELOAD_ENABLED, SECTION_STEAM},
            { AUTO_RELOAD_INTERVAL, SECTION_STEAM },
            { LAST_AUTO_RELOAD, SECTION_STEAM },

            { CAFE_APP_LAUNCH_PARAMETER, SECTION_PARAMETERS },
            { CLEAR_BETA_PARAMETER, SECTION_PARAMETERS },
            { CONSOLE_PARAMETER, SECTION_PARAMETERS },
            { DEVELOPER_PARAMETER, SECTION_PARAMETERS },
            { FORCE_SERVICE_PARAMETER, SECTION_PARAMETERS },
            { LOGIN_PARAMETER, SECTION_PARAMETERS },
            { NO_CACHE_PARAMETER, SECTION_PARAMETERS },
            { NO_VERIFY_FILES_PARAMETER, SECTION_PARAMETERS },
            { SILENT_PARAMETER, SECTION_PARAMETERS },
            { SINGLE_CORE_PARAMETER, SECTION_PARAMETERS },
            { TCP_PARAMETER, SECTION_PARAMETERS },
            { TEN_FOOT_PARAMETER, SECTION_PARAMETERS },
            { CUSTOM_PARAMETERS, SECTION_PARAMETERS },
            { CUSTOM_PARAMETERS_VALUE, SECTION_PARAMETERS },

            { LIST_VIEW_HEIGHT, SECTION_LOCATION },
            { LIST_VIEW_WIDTH, SECTION_LOCATION },

            { NAME_COLUMN_INDEX, SECTION_COLUMNS },
            { DESCRIPTION_COLUMN_INDEX, SECTION_COLUMNS },
            { TIMEOUT_COLUMN_INDEX, SECTION_COLUMNS },
            { VAC_BANS_COLUMN_INDEX, SECTION_COLUMNS },
            { GAME_BANS_COLUMN_INDEX, SECTION_COLUMNS },
            { ECO_BAN_COLUMN_INDEX, SECTION_COLUMNS },
            { LAST_BAN_COLUMN_INDEX, SECTION_COLUMNS }
        };

        public Dictionary<string, string> ListViewColumns = new Dictionary<string, string>
        {
            { "Name", NAME_COLUMN_INDEX },
            { "Description", DESCRIPTION_COLUMN_INDEX },
            { "Timeout", TIMEOUT_COLUMN_INDEX },
            { "VAC Bans", VAC_BANS_COLUMN_INDEX },
            { "Game Bans", GAME_BANS_COLUMN_INDEX},
            { "Economy Ban", ECO_BAN_COLUMN_INDEX },
            { "Last Ban (Days)", LAST_BAN_COLUMN_INDEX }
        };





    }

    class SteamExitCode
    {
        public const int SUCCESS = 42;
        public const int CANCELLED = -2;
    }
    class Account 
    {
        public string Name { get; set; }

        public string Password { get; set; }

    }
    enum VirtualInputMethod
    {
        SendMessage,
        PostMessage,
        SendWait
    }
    enum LoginWindowState
    {
        None,
        Invalid,
        Error,
        Selection,
        Login,
        Code,
        Loading,
        Success
    }


    class WindowUtils
    {
        #region dll imports

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [Out] StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "PostMessageA")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        #endregion

        public const int WM_GETTEXT = 0xD;
        public const int WM_GETTEXTLENGTH = 0xE;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;
        public const int VK_RETURN = 0x0D;
        public const int VK_TAB = 0x09;
        public const int VK_SPACE = 0x20;

        readonly static char[] specialChars = { '{', '}', '(', ')', '[', ']', '+', '^', '%', '~' };
        private static bool loginAllCancelled = false;
        private static bool steamUpdateDetected = false;
        

        private static SAMSettings settings;

        private static IEnumerable<IntPtr> EnumerateProcessWindowHandles(Process process)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in process.Threads)
                EnumThreadWindows(thread.Id, (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }

        private static string GetWindowTextRaw(IntPtr hwnd)
        {
            // Allocate correct string length first
            int length = (int)SendMessage(hwnd, WM_GETTEXTLENGTH, 0, IntPtr.Zero);
            StringBuilder sb = new StringBuilder(length + 1);
            SendMessage(hwnd, WM_GETTEXT, (IntPtr)sb.Capacity, sb);
            return sb.ToString();
        }

        public static IEnumerable<Process> GetChildProcesses(Process process)
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
                Console.WriteLine(e.Message);
            }

            return children;
        }

        public static WindowHandle GetSteamLoginWindow(Process steamProcess)
        {
            IEnumerable<Process> children = GetChildProcesses(steamProcess);
            foreach (Process childProcess in children)
            {
                if (childProcess.ProcessName == "steamwebhelper")
                {
                    IEnumerable<IntPtr> windows = EnumerateProcessWindowHandles(childProcess);
                    return GetSteamLoginWindow(windows);
                }
            }

            return WindowHandle.Invalid;
        }

        public static Process GetSteamProcess()
        {
            Process[] steamProcess = Process.GetProcessesByName("Steam");
            if (steamProcess.Length > 0)
            {
                return steamProcess[0];
            }
            return null;
        }

        public static WindowHandle GetSteamLoginWindow()
        {
            Process[] steamProcess = Process.GetProcessesByName("Steam");
            foreach (Process process in steamProcess)
            {
                WindowHandle handle = GetSteamLoginWindow(process);
                if (handle.IsValid)
                {
                    return handle;
                }
            }

            return WindowHandle.Invalid;
        }

        private static WindowHandle GetSteamLoginWindow(IEnumerable<IntPtr> windows)
        {
            foreach (IntPtr windowHandle in windows)
            {
                string text = GetWindowTextRaw(windowHandle);

                if ((text.Contains("Steam") && text.Length > 5) || text.Equals("蒸汽平台登录"))
                {
                    return new WindowHandle(windowHandle);
                }
            }

            return WindowHandle.Invalid;
        }

        public static WindowHandle GetMainSteamClientWindow(Process steamProcess)
        {
            IEnumerable<IntPtr> windows = EnumerateProcessWindowHandles(steamProcess);
            return GetMainSteamClientWindow(windows);
        }

        public static WindowHandle GetMainSteamClientWindow(string processName)
        {
            Process[] steamProcess = Process.GetProcessesByName(processName);
            foreach (Process process in steamProcess)
            {
                IEnumerable<IntPtr> windows = EnumerateProcessWindowHandles(process);

                WindowHandle handle = GetMainSteamClientWindow(windows);
                if (handle.IsValid)
                {
                    return handle;
                }
            }

            return WindowHandle.Invalid;
        }

        private static WindowHandle GetMainSteamClientWindow(IEnumerable<IntPtr> windows)
        {
            foreach (IntPtr windowHandle in windows)
            {
                string text = GetWindowTextRaw(windowHandle);

                if (text.Equals("Steam") || text.Equals("蒸汽平台"))
                {
                   return new WindowHandle(windowHandle);
                }
            }

            return WindowHandle.Invalid;
        }

        public static bool IsSteamUpdating(Process process)
        {
            WindowHandle windowHandle = GetMainSteamClientWindow(process);

            if (windowHandle.IsValid)
            {
                using (var automation = new UIA3Automation())
                {
                    try
                    {
                        AutomationElement window = automation.FromHandle(windowHandle.RawPtr);

                        if (window == null)
                        {
                            return false;
                        }

                        if (window.Properties.ClassName.Equals("BootstrapUpdateUIClass") && window.Properties.BoundingRectangle.Value.X > 312)
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            return false;
        }

        public static WindowHandle GetLegacySteamLoginWindow()
        {
            return TopLevelWindowUtils.FindWindow(wh =>
            wh.GetClassName().Equals("vguiPopupWindow") &&
            ((wh.GetWindowText().Contains("Steam") &&
            !wh.GetWindowText().Contains("-") &&
            !wh.GetWindowText().Contains("—") &&
             wh.GetWindowText().Length > 5) ||
             wh.GetWindowText().Equals("蒸汽平台登录")));
        }

        public static WindowHandle GetLegacySteamGuardWindow()
        {
            // Also checking for vguiPopupWindow class name to avoid catching things like browser tabs.
            WindowHandle windowHandle = TopLevelWindowUtils.FindWindow(wh =>
            wh.GetClassName().Equals("vguiPopupWindow") &&
            (wh.GetWindowText().StartsWith("Steam Guard") ||
             wh.GetWindowText().StartsWith("Steam 令牌") ||
             wh.GetWindowText().StartsWith("Steam ガード")));
            return windowHandle;
        }

        public static WindowHandle GetLegacySteamWarningWindow()
        {
            return TopLevelWindowUtils.FindWindow(wh =>
            wh.GetClassName().Equals("vguiPopupWindow") &&
            (wh.GetWindowText().StartsWith("Steam - ") ||
             wh.GetWindowText().StartsWith("Steam — ")));
        }

        public static WindowHandle GetLegacyMainSteamClientWindow()
        {
            return TopLevelWindowUtils.FindWindow(wh =>
            wh.GetClassName().Equals("vguiPopupWindow") &&
            (wh.GetWindowText().Equals("Steam") ||
            wh.GetWindowText().Equals("蒸汽平台")));
        }

        public static LoginWindowState GetLoginWindowState(WindowHandle loginWindow)
        {
            if (!loginWindow.IsValid)
            {
                return LoginWindowState.Invalid;
            }

            using (var automation = new UIA3Automation())
            {
                try
                {
                    AutomationElement window = automation.FromHandle(loginWindow.RawPtr);

                    if (window == null)
                    {
                        return LoginWindowState.Invalid;
                    }

                    window.Focus();

                    AutomationElement document = window.FindFirstDescendant(e => e.ByControlType(ControlType.Document));
                    AutomationElement[] children = document.FindAllChildren(); 

                    if (children.Length == 0)
                    {
                        return LoginWindowState.Invalid;
                    }

                    if (children.Length == 2)
                    {
                        return LoginWindowState.Loading;
                    }

                    var inputs = new List<AutomationElement>();
                    var buttons = new List<AutomationElement>();
                    var groups = new List<AutomationElement>();
                    var images = new List<AutomationElement>();
                    var texts = new List<AutomationElement>();

                    foreach (AutomationElement element in children) {
                        switch (element.ControlType) {
                            case ControlType.Edit:
                                inputs.Add(element);
                                break;
                            case ControlType.Button:
                                buttons.Add(element);
                                break;
                            case ControlType.Group:
                                groups.Add(element);
                                break;
                            case ControlType.Image:
                                images.Add(element); 
                                break;
                            case ControlType.Text:
                                texts.Add(element);
                                break;
                        }
                    }

                    if (inputs.Count == 0 && images.Count == 1 && buttons.Count == 2 && texts.Count > 0)
                    {
                        foreach (var text in texts)
                        {
                            string content = text.Name.ToLower();

                            if (content.Contains("error") || content.Contains("problem"))
                            {
                                return LoginWindowState.Error;
                            }
                        }
                    }
                    if (inputs.Count == 0 && images.Count == 0 && buttons.Count == 1)
                    {
                        return LoginWindowState.Error;
                    }
                    else if (inputs.Count == 0 && images.Count >= 2 && buttons.Count > 0)
                    {
                        return LoginWindowState.Selection;
                    }
                    else if (inputs.Count == 5)
                    {
                        return LoginWindowState.Code;
                    }
                    else if (inputs.Count == 2 && buttons.Count == 1)
                    {
                        return LoginWindowState.Login;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return LoginWindowState.Invalid;
        }


        public static LoginWindowState TryCredentialsEntry(WindowHandle loginWindow, string username, string password, bool remember)
        {
            using (var automation = new UIA3Automation())
            {
                try
                {
                    AutomationElement window = automation.FromHandle(loginWindow.RawPtr);

                    window.Focus();

                    AutomationElement document = window.FindFirstDescendant(e => e.ByControlType(ControlType.Document));
                    AutomationElement[] children = document.FindAllChildren();

                    var inputs = new List<AutomationElement>();
                    var buttons = new List<AutomationElement>();
                    var groups = new List<AutomationElement>();

                    foreach (AutomationElement element in children)
                    {
                        switch (element.ControlType)
                        {
                            case ControlType.Edit:
                                inputs.Add(element);
                                break;
                            case ControlType.Button:
                                buttons.Add(element);
                                break;
                            case ControlType.Group:
                                groups.Add(element);
                                break;
                        }
                    }

                    Button signInButton = buttons[0].AsButton();

                    if (signInButton.IsEnabled)
                    {
                        TextBox usernameBox = inputs[0].AsTextBox();
                        usernameBox.WaitUntilEnabled();
                        usernameBox.Text = username;

                        TextBox passwordBox = inputs[1].AsTextBox();
                        passwordBox.WaitUntilEnabled();
                        passwordBox.Text = password;

                        Button checkBoxButton = groups[0].AsButton();
                        bool isChecked = checkBoxButton.FindFirstChild(e => e.ByControlType(ControlType.Image)) != null;

                        if (remember != isChecked)
                        {
                            checkBoxButton.Focus();
                            checkBoxButton.WaitUntilEnabled();
                            checkBoxButton.Invoke();
                        }

                        signInButton.Focus();
                        signInButton.WaitUntilEnabled();
                        signInButton.Invoke();

                        return LoginWindowState.Success;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return LoginWindowState.Invalid;
        }

        public static LoginWindowState TryCodeEntry(WindowHandle loginWindow, string secret)
        {
            using (var automation = new UIA3Automation())
            {
                try
                {
                    AutomationElement window = automation.FromHandle(loginWindow.RawPtr);

                    window.Focus();

                    AutomationElement document = window.FindFirstDescendant(e => e.ByControlType(ControlType.Document));
                    AutomationElement[] inputs = document.FindAllChildren(e => e.ByControlType(ControlType.Edit));

                    string code = Generate2FACode(secret);

                    try
                    {
                        for (int i = 0; i < inputs.Length; i++)
                        {
                            TextBox textBox = inputs[i].AsTextBox();
                            textBox.Text = code[i].ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return LoginWindowState.Code;
                    }

                    return LoginWindowState.Success;
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return LoginWindowState.Invalid;
        }

        public static Process WaitForSteamProcess(WindowHandle windowHandle)
        {
            Process process = null;

            // Wait for valid process to wait for input idle.
            Console.WriteLine("Waiting for it to be idle.");
            while (process == null)
            {
                GetWindowThreadProcessId(windowHandle.RawPtr, out int procId);

                // Wait for valid process id from handle.
                while (procId == 0)
                {
                    Thread.Sleep(100);
                    GetWindowThreadProcessId(windowHandle.RawPtr, out procId);
                }

                try
                {
                    process = Process.GetProcessById(procId);
                }
                catch
                {
                    process = null;
                }
            }

            return process;
        }

        public static WindowHandle WaitForSteamClientWindow()
        {
            WindowHandle steamClientWindow = WindowHandle.Invalid;

            Console.WriteLine("Waiting for full Steam client to initialize.");

            int waitCounter = 0;

            while (!steamClientWindow.IsValid && !loginAllCancelled)
            {
                if (waitCounter >= 600)
                {
                    return steamClientWindow;
                }

                steamClientWindow = GetMainSteamClientWindow("Steam");
                Thread.Sleep(100);
                waitCounter += 1;
            }

            loginAllCancelled = false;

            return steamClientWindow;
        }

        public static void CancelLoginAll()
        {
            loginAllCancelled = true;
        }

        /**
         * Because CapsLock is handled by system directly, thus sending
         * it to one particular window is invalid - a window could not
         * respond to CapsLock, only the system can.
         * 
         * For this reason, I break into a low-level API, which may cause
         * an inconsistency to the original `SendWait` method.
         * 
         * https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-keybd_event
         */
        public static void SendCapsLockGlobally()
        {
            // Press key down
            keybd_event((byte)System.Windows.Forms.Keys.CapsLock, 0, 0, 0);
            // Press key up
            keybd_event((byte)System.Windows.Forms.Keys.CapsLock, 0, 0x2, 0);
        }

        public static void SendCharacter(IntPtr hwnd, VirtualInputMethod inputMethod, char c)
        {
            switch (inputMethod)
            {
                case VirtualInputMethod.SendMessage:
                    SendMessage(hwnd, WM_CHAR, c, IntPtr.Zero);
                    break;

                case VirtualInputMethod.PostMessage:
                    PostMessage(hwnd, WM_CHAR, (IntPtr)c, IntPtr.Zero);
                    break;

                default:
                    if (IsSpecialCharacter(c))
                    {
                        if (inputMethod == VirtualInputMethod.SendWait)
                        {
                            System.Windows.Forms.SendKeys.SendWait("{" + c.ToString() + "}");
                        }
                        else
                        {
                            System.Windows.Forms.SendKeys.Send("{" + c.ToString() + "}");
                        }
                    }
                    else
                    {
                        if (inputMethod == VirtualInputMethod.SendWait)
                        {
                            System.Windows.Forms.SendKeys.SendWait(c.ToString());
                        }
                        else
                        {
                            System.Windows.Forms.SendKeys.Send(c.ToString());
                        }
                    }
                    break;
            }
        }

        public static void SendEnter(IntPtr hwnd, VirtualInputMethod inputMethod)
        {
            switch (inputMethod)
            {
                case VirtualInputMethod.SendMessage:
                    SendMessage(hwnd, WM_KEYDOWN, VK_RETURN, IntPtr.Zero);
                    SendMessage(hwnd, WM_KEYUP, VK_RETURN, IntPtr.Zero);
                    break;

                case VirtualInputMethod.PostMessage:
                    PostMessage(hwnd, WM_KEYDOWN, VK_RETURN, IntPtr.Zero);
                    PostMessage(hwnd, WM_KEYUP, VK_RETURN, IntPtr.Zero);
                    break;

                case VirtualInputMethod.SendWait:
                    SetForegroundWindow(hwnd);
                    System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                    break;
            }
        }

        public static void SendTab(IntPtr hwnd, VirtualInputMethod inputMethod)
        {
            switch (inputMethod)
            {
                case VirtualInputMethod.SendMessage:
                    SendMessage(hwnd, WM_KEYDOWN, VK_TAB, IntPtr.Zero);
                    SendMessage(hwnd, WM_KEYUP, VK_TAB, IntPtr.Zero);
                    break;

                case VirtualInputMethod.PostMessage:
                    PostMessage(hwnd, WM_KEYDOWN, (IntPtr)VK_TAB, IntPtr.Zero);
                    PostMessage(hwnd, WM_KEYUP, (IntPtr)VK_TAB, IntPtr.Zero);
                    break;

                case VirtualInputMethod.SendWait:
                    SetForegroundWindow(hwnd);
                    System.Windows.Forms.SendKeys.SendWait("{TAB}");
                    break;
            }
        }

        public static void SendSpace(IntPtr hwnd, VirtualInputMethod inputMethod)
        {
            switch (inputMethod)
            {
                case VirtualInputMethod.SendMessage:
                    SendMessage(hwnd, WM_KEYDOWN, VK_SPACE, IntPtr.Zero);
                    SendMessage(hwnd, WM_KEYUP, VK_SPACE, IntPtr.Zero);
                    break;

                case VirtualInputMethod.PostMessage:
                    PostMessage(hwnd, WM_KEYDOWN, (IntPtr)VK_SPACE, IntPtr.Zero);
                    PostMessage(hwnd, WM_KEYUP, (IntPtr)VK_SPACE, IntPtr.Zero);
                    break;

                case VirtualInputMethod.SendWait:
                    SetForegroundWindow(hwnd);
                    System.Windows.Forms.SendKeys.SendWait(" ");
                    break;
            }
        }

        public static void ClearSteamUserDataFolder(string steamPath, int sleepTime, int maxRetry)
        {
            WindowHandle steamLoginWindow = GetLegacySteamLoginWindow();
            int waitCount = 0;

            while (steamLoginWindow.IsValid && waitCount < maxRetry)
            {
                Thread.Sleep(sleepTime);
                waitCount++;
            }

            string path = steamPath + "\\userdata";

            if (Directory.Exists(path))
            {
                Console.WriteLine("Deleting userdata files...");
                Directory.Delete(path, true);
                Console.WriteLine("userdata files deleted!");
            }
            else
            {
                Console.WriteLine("userdata directory not found.");
            }
        }

        public static bool IsSpecialCharacter(char c)
        {
            foreach (char special in specialChars)
            {
                if (c.Equals(special))
                {
                    return true;
                }
            }

            return false;
        }

        public static string Generate2FACode(string shared_secret)
        {
            SteamGuardAccount authAccount = new SteamGuardAccount { SharedSecret = shared_secret };
            string code = authAccount.GenerateSteamGuardCode();
            return code;
        }

        
        private static void EnterCredentials(Process steamProcess, Account account, int tryCount)
        {
            if (steamProcess.HasExited)
            {
                return;
            }

            if (tryCount > 0 && WindowUtils.GetMainSteamClientWindow(steamProcess).IsValid)
            {
                PostLogin();
                return;
            }

            WindowHandle steamLoginWindow = WindowUtils.GetSteamLoginWindow(steamProcess);

            while (!steamLoginWindow.IsValid)
            {
                if (steamProcess.HasExited)
                {
                    if (steamUpdateDetected && steamProcess.ExitCode == SteamExitCode.SUCCESS)
                    {
                        // Update window creates a new steam process
                        Process process = WindowUtils.GetSteamProcess();
                        if (process != null)
                        {
                            steamProcess = process;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                if (WindowUtils.IsSteamUpdating(steamProcess))
                {
                    steamUpdateDetected = true;
                }

                Thread.Sleep(100);
                steamLoginWindow = WindowUtils.GetSteamLoginWindow(steamProcess);
            }

            LoginWindowState state = LoginWindowState.None;

            while (state != LoginWindowState.Success && state != LoginWindowState.Code)
            {
                if (steamProcess.HasExited || state == LoginWindowState.Error)
                {
                    return;
                }

                Thread.Sleep(100);

                state = WindowUtils.GetLoginWindowState(steamLoginWindow);

                if (state == LoginWindowState.Login)
                {
                    state = WindowUtils.TryCredentialsEntry(steamLoginWindow, account.Name, account.Password, false);
                }
            }

            Thread.Sleep(1000);
            state = LoginWindowState.Loading;

            while (state == LoginWindowState.Loading)
            {
                Thread.Sleep(100);
                state = WindowUtils.GetLoginWindowState(steamLoginWindow);
            }

            PostLogin();
        }

        private static void PostLogin()
        {
        }

        public static void Login(Account account, int tryCount)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName =  "C:\\Program Files (x86)\\Steam\\steam.exe",
                WorkingDirectory = "C:\\Program Files (x86)\\Steam\\",
                UseShellExecute = true,
                Arguments = "",
            };

            Process steamProcess;
            try
            {
                steamProcess = Process.Start(startInfo);
            }
            catch (Exception m)
            {
                Console.WriteLine(m.Message);
                return;
            }

            EnterCredentials(steamProcess, account, 0);
        }
    }

}
