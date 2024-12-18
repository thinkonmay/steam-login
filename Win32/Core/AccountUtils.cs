using System.Security.AccessControl;
using Microsoft.Win32;
       
namespace SAM.Core
{
    class AccountUtils
    { 
        
        public static void ClearAutoLoginUserKeyValues()
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

            try
            {
                localKey = localKey.OpenSubKey(@"Software\\Valve\\Steam", true);
                localKey.SetValue("AutoLoginUser", "", RegistryValueKind.String);
                localKey.SetValue("RememberPassword", 0, RegistryValueKind.DWord);
                localKey.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public static string GetSteamPath()
        {
            RegistryKey localKey = Microsoft.Win32.Registry.ClassesRoot;
            var local = localKey.OpenSubKey(@"steam\\Shell\\Open\\Command",RegistryRights.QueryValues);
            var val = local.GetValue("");
            local.Close();
            return val.ToString().Split("\"")[1];
        }
    }
}