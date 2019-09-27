namespace Surgical.SDK.Security
{
    using System;
    using System.Linq;
    using System.Management;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;
    using System.Text.RegularExpressions;

    using Microsoft.Win32;

    using Surgical.SDK.Logging;

    public static class Machine
    {
        public static readonly string Hwid, UserId;

        public static bool IsProcessElevated()
        {
            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static Machine()
        {
            Hwid = GetHwid();

            UserId = RandomizeStr(Hwid, 9);

            Log.Info("Your UserId is " + UserId);
        }

        private static string GetHwid()
        {
            Log.Info("Getting hardware data...");

            var uniq = GetWmi("UniqueId", "Processor");
            var proc = GetWmi("ProcessorId", "Processor");
            var mobo = GetWmi("SerialNumber", "BIOS");
            var disk = GetWmi("Signature", "DiskDrive");

            var reg = GetRegistryHwid();

            var user = Environment.UserName;

            Log.Info("UniqueId: " + uniq);
            Log.Info("ProcessorId: " + proc);
            Log.Info("SerialNumber: " + mobo);
            Log.Info("DiskSignature: " + disk);
            Log.Info("RegHwid: " + reg);
            Log.Info("Windows username: " + user);

            var input = RandomizeStr(uniq + proc + mobo + reg + disk + user, 25);

            return Regex.Replace(input, ".{5}", "$0-");
        }

        private static string GetRegistryHwid()
        {
            using var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");

            return key?.GetValue("MachineGuid")?.ToString() ?? throw new NotSupportedException("Hwid registry key doesn't exist!");
        }

        private static string GetWmi(string value, string from)
        {
            var query = "select " + value + " from Win32_" + from;

            using var mos = new ManagementObjectSearcher(query);
            using var result = mos.Get();

            return result.Cast<ManagementBaseObject>().Select(mbo => mbo[value]?.ToString()).SingleOrDefault(str => str != null);
        }

        private static string RandomizeStr(string str, int length)
        {
            byte[] bytes;

            using (HashAlgorithm sha = new SHA512CryptoServiceProvider())
            {
                bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(str));
            }

            return Convert.ToBase64String(bytes).Substring(0, length).Replace('/', '0').Replace('+', '1').ToUpper();
        }
    }
}