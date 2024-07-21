using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DownloaderAppMobile.MVVM.Model
{
    public class FlyoutMenuItem
    {
        public ImageSource ImageSource { get; set; }
        public string Title { get; set; }
        public Type PageType { get; set; }
    }
}
