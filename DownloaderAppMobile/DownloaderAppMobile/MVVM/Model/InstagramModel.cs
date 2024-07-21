using InstagramApiSharp.API;
using InstagramService.Classes;
using System.IO;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using InstagramApiSharp.Logger;
using InstagramService.Classes.Models;
using System.Collections.Generic;
using DownloaderAppMobile.Helpers;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes.SessionHandlers;

namespace DownloaderAppMobile.MVVM.Model
{
    public class InstagramModel
    {
#if DEBUG
        public static string AccountSessionFileName { get; } = "acses_debuge.dat";
#else
        public static string AccountSessionFileName { get; } = "account_session.dat";
#endif
        public static string AccountSessionFilePath { get; } = Path.Combine(FileSystem.AppDataDirectory, AccountSessionFileName);
        public static Regex Regex { get; } = new Regex(
                @"(?:https?://)(?:www\.)instagram\.com/(?:p|reel|reels)/([a-zA-Z0-9_-]{1,11})(\/\?\S*)?\/?",
                RegexOptions.IgnoreCase);

        public InstaService InstagramService { get; set; }
        public IEnumerable<InstaMediaInfo> Infos { get; set; }

        public InstagramModel()
        {
            if (!File.Exists(AccountSessionFilePath))
                PathHelper.CopyEmbeddedResourceToFile($"{nameof(DownloaderAppMobile)}.{AccountSessionFileName}", AccountSessionFilePath);

            var data = File.ReadAllText(AccountSessionFilePath);
            if (data.Length == 0)
            {
                IInstaApi instaApi = InstaApiBuilder.CreateBuilder().SetSessionHandler(new FileSessionHandler
                {
                    FilePath = AccountSessionFilePath
                }).UseLogger(new DebugLogger(LogLevel.Exceptions))
                   .Build();

                InstagramService = new InstaService(instaApi);
            }
            else
            {
                InstagramService = InstaService.BuildAndLoad(
                    AccountSessionFilePath, new DebugLogger(LogLevel.Exceptions));
            }
        }
    }
}
