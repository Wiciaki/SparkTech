namespace Surgical.SDK.Security
{
    using System.IO;

    public class Folder
    {
        public static Folder Root { get; private set; }

        public static Folder Logs { get; private set; }

        public static Folder Menu { get; private set; }

        internal static void Initialize(string root)
        {
            Root = new Folder(root);

            Logs = Root.GetFolder("Logs");
            Menu = Root.GetFolder("Menu");
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
            return folder?.ToString();
        }

        public override string ToString()
        {
            return this.path;
        }
    }
}