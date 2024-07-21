using System;
using System.Collections.Generic;
using System.Text;

namespace DownloaderAppMobile.MVVM.Model
{
    public enum MediaType : byte
    {
        Audio,
        Video,
    }

    public class MediaTypeItem
    {
        public string Title { get; set; }
        public MediaType MediaType { get; set; }
    }
}
