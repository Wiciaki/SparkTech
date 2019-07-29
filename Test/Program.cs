namespace Test
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    using Microsoft.Win32;

    class Program
    {
        private static string hwid;

        public static string Hwid => hwid ??= GetHwid();

        private static string GetHwid()
        {
            using var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");

            return key?.GetValue("MachineGuid")?.ToString() ?? throw new NotSupportedException();
        }

        private static string machineId;

        public static string MachineId => machineId ??= GetMachineId();

        private static string GetMachineId()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Hwid);

            using (var sha1 = new SHA512CryptoServiceProvider())
            {
                bytes = sha1.ComputeHash(bytes);
            }

            return Convert.ToBase64String(bytes)[..9].ToUpper();
        }

        static void Main(string[] args)
        {
            Console.WriteLine(Hwid);
            Console.WriteLine(MachineId);
        }
    }
}