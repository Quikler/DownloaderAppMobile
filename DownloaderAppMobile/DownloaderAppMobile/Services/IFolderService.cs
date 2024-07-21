namespace DownloaderAppMobile.Services
{
    public interface IFolderService
    {
        string CreateFolder(string publicDir, string folderName, bool scanOnCreate = true);
    }
}
