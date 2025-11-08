namespace Application.Services
{
    public class FileService
    {

        public string Rename(string path, string newName)
        {
            var basePath = Path.GetDirectoryName(path);
            var newPath = Path.Combine(basePath, newName);
            File.Move(path, newPath, overwrite: false);
            return newPath;
        }
    }
}
