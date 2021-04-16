namespace SparkTech.SDK.Logging
{
    using System;
    using System.IO;

    public class Folder
    {
        public static Folder Root { get; }

        public static Folder Logs { get; }

        public static Folder Menu { get; }

        public static Folder Scripts { get; }

        static Folder()
        {
            var appData = new Folder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            Root = appData.GetFolder("SparkTech.SDK");
            Logs = Root.GetFolder("Logs");
            Menu = Root.GetFolder("Menu");
            Scripts = Root.GetFolder("Scripts");
        }

        private readonly string path;

        public Folder(string path)
        {
            Directory.CreateDirectory(path);

            this.path = path;
        }

        public string GetFile(string fileName)
        {
            return this.GetPath(fileName);
        }

        public Folder GetFolder(string dirName)
        {
            return new Folder(this.GetPath(dirName));
        }

        private string GetPath(string itemName)
        {
            return Path.Combine(this, itemName);
        }

        public static implicit operator string(Folder folder)
        {
            return folder.ToString();
        }

        public override string ToString()
        {
            return this.path;
        }
    }
}