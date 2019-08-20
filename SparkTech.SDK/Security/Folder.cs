namespace SparkTech.SDK.Security
{
    using System.IO;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class Folder
    {
        public static Folder RootFolder { get; private set; }

        public static Folder ThirdPartyFolder { get; private set; }

        public static Folder MenuFolder { get; private set; }

        internal static void Initialize(string root)
        {
            RootFolder = new Folder(root);

            ThirdPartyFolder = RootFolder.GetFolder("ThirdParty");
            MenuFolder = RootFolder.GetFolder("Menu");
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

        internal static async Task SaveTokenAsync(string targetPath, JToken token)
        {
            using var fileStream = new FileStream(targetPath, File.Exists(targetPath) ? FileMode.Truncate : FileMode.Create, FileAccess.Write, FileShare.None);
            using var streamWriter = new StreamWriter(fileStream);
            using var testWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented };

            await token.WriteToAsync(testWriter);
        }
    }
}