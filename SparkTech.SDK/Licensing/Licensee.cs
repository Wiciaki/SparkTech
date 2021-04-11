namespace SparkTech.SDK.Licensing
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Management;
    using System.Text;
    using System.Security.Cryptography;

    using Microsoft.Win32;

    using SDK.Logging;

    public static class Licensee
    {
        private static string hwid;

        public static string GetUserId()
        {
            if (hwid == null)
            {
                Log.Info("Getting hardware data...");

                try
                {
                    hwid = GetHwid();
                }
                catch (Exception ex)
                {
                    hwid = string.Empty;
                    Log.Error(ex);
                }

                Log.Info($"License id for your machine: \"{hwid}\"");
            }

            return hwid;
        }

        private static string GetHwid()
        {
            var uniq = GetWmi("UniqueId", "Processor");
            var proc = GetWmi("ProcessorId", "Processor");
            var mobo = GetWmi("SerialNumber", "BIOS");
            var disk = GetWmi("Signature", "DiskDrive");

            var reg = GetRegistryHwid();

            var input = Shuffle(uniq + proc + mobo + reg + disk, 25);

            var i = 5;
            return string.Join("-", input.ToLookup(c => Math.Floor(i++ / 5f)).Select(e => new string(e.ToArray())));
        }

        private static string GetRegistryHwid()
        {
            using (var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var key = localMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                {
                    return key?.GetValue("MachineGuid")?.ToString();
                }
            }
        }

        private static string GetWmi(string value, string from)
        {
            var query = $"select {value} from Win32_{from}";

            using (var mos = new ManagementObjectSearcher(query))
            {
                using (var moc = mos.Get())
                {
                    var results = moc.Cast<ManagementBaseObject>().Select(mbo => mbo[value]?.ToString());
                    
                    return results.SingleOrDefault(str => !string.IsNullOrEmpty(str));
                }
            }
        }

        private static string Shuffle(string str, int length)
        {
            using (var algo = new SHA512CryptoServiceProvider())
            {
                var hash = algo.ComputeHash(Encoding.UTF8.GetBytes(str));
                var result = Convert.ToBase64String(hash).Substring(0, length);

                result = result.Replace('/', '0');
                result = result.Replace('+', '1');

                return result.ToUpper(CultureInfo.InvariantCulture);
            }
        }
    }
}