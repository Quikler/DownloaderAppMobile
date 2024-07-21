using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using DownloaderAppMobile.Controls;
using DownloaderAppMobile.Droid.Renderers;
using System;
using System.IO;
using Android.Text.Method;
using Android.Hardware.Lights;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(SearchEntryRenderer))]
namespace DownloaderAppMobile.Droid.Renderers
{
    public class SearchEntryRenderer : EntryRenderer, Android.Views.View.IOnTouchListener
    {
        public SearchEntryRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            InitializeCotrol();
        }

        private CustomEntry _customEntry;
        private void InitializeCotrol()
        {
            if (Element is null || Control is null)
                return;

            _customEntry = (CustomEntry)Element;

            if (_customEntry.RightIcon != null ||
                _customEntry.EyeSlashIcon != null ||
                _customEntry.EyeVisibleIcon != null)
            {
                _customEntry.RightIconClickOption = 
                    _customEntry.EyeSlashIcon != null && _customEntry.EyeVisibleIcon != null
                    ? CustomEntry.RightIconClickOptions.HideShow
                    : CustomEntry.RightIconClickOptions.Clear;

                // Setting on touch listener on the control (to check if click occured on the right side of right drawable area)
                Control.SetOnTouchListener(this);
            }
            
            Drawable left = GetDrawable(_customEntry.LeftIcon);
            Drawable right = _customEntry.RightIconClickOption == CustomEntry.RightIconClickOptions.Clear
                ? GetDrawable(_customEntry.RightIcon) : GetDrawable(_customEntry.EyeSlashIcon);

            // set entry underline to transparent
            Control.Background = new ColorDrawable(Android.Graphics.Color.Transparent);

            // set search sign in the left and clear sign in the right
            Control.SetCompoundDrawablesWithIntrinsicBounds(left, null, right, null);

            // set padding between drawables(right and left)
            Control.CompoundDrawablePadding = _customEntry.CompoundDrawablePadding;

            Control.SetPadding(
                (int)_customEntry.Padding.Left,
                (int)_customEntry.Padding.Top,
                (int)_customEntry.Padding.Right,
                (int)_customEntry.Padding.Bottom);
        }

        private Drawable GetDrawable(string source)
        {
            if (source is null)
                return null;

            Drawable drawable = null;
            if (File.Exists(source))
            {
                drawable = Drawable.CreateFromPath(source);
            }
            else
            {
                int resID = Resources.GetIdentifier(source, "drawable", Context.PackageName);
                if (resID != 0)
                {
                    drawable = Context.GetDrawable(resID);
                }
            }
            return drawable;
        }

        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            if (_customEntry.RightIconClickOption == CustomEntry.RightIconClickOptions.Clear)
            {
                return ClearEntry(v, e);
            }

            return HideShowRightIcon(v, e);
        }

        private bool HideShowRightIcon(Android.Views.View v, MotionEvent e)
        {
            var editText = (EditText)v;
            if (e.Action != MotionEventActions.Up || !IsRightIconPressed(editText, e))
                return false;

            Drawable left = GetDrawable(_customEntry.LeftIcon);
            Drawable right;

            if (editText.TransformationMethod == null)
            {
                editText.TransformationMethod = PasswordTransformationMethod.Instance;
                right = GetDrawable(_customEntry.EyeVisibleIcon);
            }
            else
            {
                editText.TransformationMethod = null;
                right = GetDrawable(_customEntry.EyeSlashIcon);
            }
            Control.SetCompoundDrawablesWithIntrinsicBounds(left, null, right, null);
            return true;
        }

        private bool ClearEntry(Android.Views.View v, MotionEvent e)
        {
            if (e.Action != MotionEventActions.Up)
                return false;
            var editText = (EditText)v;

            if (IsRightIconPressed(editText, e))
            {
                editText.Text = string.Empty;
                return true;
            }
            return false;
        }

        private bool IsRightIconPressed(EditText editText, MotionEvent e)
        {
            float x = e.GetX(), y = e.GetY();
            int width = editText.Width, height = editText.Height;

            Drawable icon = editText.GetCompoundDrawables()[2];

            bool isXPositionBeforeIcon = x < (width - editText.PaddingRight - icon.IntrinsicWidth);
            bool isXPositionAfterIcon = x > (width - editText.PaddingRight);

            bool isYPositionAboveIcon = y < (height - editText.PaddingTop - icon.IntrinsicHeight);
            bool isYPositionBelowIcon = y > (height - editText.PaddingBottom);

            // Clicked on right icon drawable if true
            return !isXPositionBeforeIcon && !isXPositionAfterIcon
                && !isYPositionAboveIcon && !isYPositionBelowIcon;
        }
    }
}