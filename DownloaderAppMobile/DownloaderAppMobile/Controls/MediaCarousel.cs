using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace DownloaderAppMobile.Controls
{
    public class MediaCarousel : CarouselView
    {
        private readonly ObservableCollection<View> _selectedViews;
        private readonly ObservableCollection<Grid> _grids;
        private readonly Dictionary<CheckBox, View> _viewCheckBoxPairs;

        #region BindableProperties
        public IEnumerable SelectedViews
        {
            get { return (IEnumerable)GetValue(SelectedViewsProperty); }
            private set { SetValue(SelectedViewsProperty, value); }
        }

        public static readonly BindableProperty SelectedViewsProperty =
            BindableProperty.Create(
                "SelectedViews",
                typeof(IEnumerable), 
                typeof(MediaCarousel),
                new ObservableCollection<View>(),
                defaultBindingMode: BindingMode.OneWayToSource);

        // Override the ItemsSource property to add custom behavior
        public new IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        // Create a new BindableProperty for ItemsSource to trigger custom behavior
        public static readonly new BindableProperty ItemsSourceProperty =
            BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(MediaCarousel),
                new ObservableCollection<View>(),
                propertyChanged: OnItemsSourceChanged);
        #endregion

        public MediaCarousel()
        {
            // Bind _grids to parent (base) ItemsSource property to show grids in the UI
            base.ItemsSource = _grids = new ObservableCollection<Grid>();
            _viewCheckBoxPairs = new Dictionary<CheckBox, View>();
            _selectedViews = SelectedViews as ObservableCollection<View>;

            ItemTemplate = new DataTemplate(() =>
            {
                ContentView contentView = new ContentView();
                contentView.SetBinding(ContentView.ContentProperty, ".");
                return contentView;
            });
        }

        #region Events
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue is null)
                return;

            MediaCarousel mediaCarousel = (MediaCarousel)bindable;

            if (newValue is ObservableCollection<View> observable)
            {
                observable.CollectionChanged -= mediaCarousel.OnItemsSourceCollectionChanged;
                observable.CollectionChanged += mediaCarousel.OnItemsSourceCollectionChanged;
            }

            mediaCarousel.OnItemsSourceCollectionChanged(null, null);
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _selectedViews.Clear();
            _grids.Clear();

            foreach (View current in ItemsSource)
            {
                Grid grid = new Grid();

                var checkbox = new CheckBox
                {
                    Margin = new Thickness(0, 3, 3, 0),
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.End,
                    Scale = 1.5d,
                };
                checkbox.CheckedChanged += OnCheckBoxChecked;
                _viewCheckBoxPairs.Add(checkbox, current);

                grid.Children.Add(current);
                grid.Children.Add(checkbox);

                _grids.Add(grid);
            }
        }

        private void OnCheckBoxChecked(object sender, CheckedChangedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            View view = _viewCheckBoxPairs.FirstOrDefault(a => a.Key == checkBox).Value;

            if (view is null)
                return;

            if (e.Value)
                _selectedViews.Add(view);
            else
                _selectedViews.Remove(view);
        }
        #endregion
    }
}