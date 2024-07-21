using System.Windows.Input;
using Xamarin.Forms;

namespace DownloaderAppMobile.Controls
{
    public class CustomEntry : Entry
    {
        public enum RightIconClickOptions
        {
            Clear, HideShow,
        }

        public RightIconClickOptions RightIconClickOption
        {
            get { return (RightIconClickOptions)GetValue(RightIconClickOptionProperty); }
            set { SetValue(RightIconClickOptionProperty, value); }
        }

        public static readonly BindableProperty RightIconClickOptionProperty =
            BindableProperty.Create("RightIconClickOption", typeof(RightIconClickOptions), typeof(CustomEntry));

        public string LeftIcon
        {
            get { return (string)GetValue(LeftIconProperty); }
            set { SetValue(LeftIconProperty, value); }
        }

        public static readonly BindableProperty LeftIconProperty =
            BindableProperty.Create("LeftIcon", typeof(string), typeof(CustomEntry));

        public string RightIcon
        {
            get { return (string)GetValue(RightIconIconProperty); }
            set { SetValue(RightIconIconProperty, value); }
        }

        public static readonly BindableProperty RightIconIconProperty =
            BindableProperty.Create("RightIconIcon", typeof(string), typeof(CustomEntry));

        public string EyeSlashIcon
        {
            get { return (string)GetValue(EyeSlashIconProperty); }
            set { SetValue(EyeSlashIconProperty, value); }
        }

        public static readonly BindableProperty EyeSlashIconProperty =
            BindableProperty.Create("EyeSlashIcon", typeof(string), typeof(CustomEntry));

        public string EyeVisibleIcon
        {
            get { return (string)GetValue(EyeVisibleIconProperty); }
            set { SetValue(EyeVisibleIconProperty, value); }
        }

        public static readonly BindableProperty EyeVisibleIconProperty =
            BindableProperty.Create("EyeVisibleIcon", typeof(string), typeof(CustomEntry));

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly BindableProperty PaddingProperty =
            BindableProperty.Create("Padding", typeof(Thickness), typeof(CustomEntry));

        public int CompoundDrawablePadding
        {
            get { return (int)GetValue(CompoundDrawablePaddingProperty); }
            set { SetValue(CompoundDrawablePaddingProperty, value); }
        }

        public static readonly BindableProperty CompoundDrawablePaddingProperty =
            BindableProperty.Create("CompoundDrawablePadding", typeof(int), typeof(CustomEntry), 32);
    }
}