﻿/*namespace SparkTech.SDK.Security
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Text;

    using Microsoft.Win32;

    public static class Machine
    {
        public static readonly string Hwid, HardwareBasedUserId;

        public static bool IsProcessElevated()
        {
            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static Machine()
        {
            Hwid = GetHwid();

            HardwareBasedUserId = GetHardwareBasedUserId();
        }

        private static string GetHwid()
        {
            using var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");

            return key?.GetValue("MachineGuid")?.ToString() ?? throw new NotSupportedException();
        }

        private static string GetHardwareBasedUserId()
        {
            var bytes = Encoding.UTF8.GetBytes(Hwid);

            using (var sha512 = new SHA512CryptoServiceProvider())
            {
                bytes = sha512.ComputeHash(bytes);
            }

            return Convert.ToBase64String(bytes).Substring(0, 9).ToUpper();
        }
    }
}*/