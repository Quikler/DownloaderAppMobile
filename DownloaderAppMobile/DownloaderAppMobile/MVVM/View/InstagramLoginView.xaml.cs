using DownloaderAppMobile.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DownloaderAppMobile.MVVM.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InstagramLoginView : ContentPage
	{
		public InstagramLoginView()
		{
			InitializeComponent();
		}

        private void CustomEntry_Completed(object sender, EventArgs e)
			=> _ = (CustomEntry)sender == usernameEntry ? passwordEntry.Focus() : usernameEntry.Focus();
    }
}