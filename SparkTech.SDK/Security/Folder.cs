namespace SparkTech.SDK.Security
{
    using System.IO;

    public class Folder
    {
        public static Folder RootFolder { get; private set; }

        public static Folder ScriptsFolder => RootFolder.GetFolder("Scripts");

        public static Folder MenuFolder => RootFolder.GetFolder("Scripts");

        internal void Initialize(string root)
        {
            RootFolder = new Folder(root);
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

        public static implicit operator string(Folder folder) => folder.path;
    }
}