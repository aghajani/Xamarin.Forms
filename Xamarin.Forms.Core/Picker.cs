using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_PickerRenderer))]
	public class Picker : View, IElementConfiguration<Picker>, IFontElement
    {
		public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(Picker), Color.Default);

		public static readonly BindableProperty TitleProperty =
			BindableProperty.Create(nameof(Title), typeof(string), typeof(Picker), default(string));

		public static readonly BindableProperty SelectedIndexProperty =
			BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(Picker), -1, BindingMode.TwoWay,
									propertyChanged: OnSelectedIndexChanged, coerceValue: CoerceSelectedIndex);

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create("FontFamily", typeof(string), typeof(Picker), default(string));

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(double), typeof(Picker), -1.0,
            defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Default, (Picker)bindable));

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create("FontAttributes", typeof(FontAttributes), typeof(Picker), FontAttributes.None);

        public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create("HorizontalTextAlignment", typeof(TextAlignment), typeof(Picker), TextAlignment.Start);
        public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create("VerticalTextAlignment", typeof(TextAlignment), typeof(Picker), TextAlignment.Start);

        public static readonly BindableProperty AdjustsFontSizeToFitWidthProperty = BindableProperty.Create("AdjustsFontSizeToFitWidth", typeof(bool), typeof(Picker), false);

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(Picker), default(IList),
                                    propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(Picker), null, BindingMode.TwoWay,
                                    propertyChanged: OnSelectedItemChanged);

        readonly Lazy<PlatformConfigurationRegistry<Picker>> _platformConfigurationRegistry;

		public Picker()
		{
			((INotifyCollectionChanged)Items).CollectionChanged += OnItemsCollectionChanged;
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Picker>>(() => new PlatformConfigurationRegistry<Picker>(this));
		}

		public IList<string> Items { get; } = new LockableObservableListWrapper();

		public IList ItemsSource
		{
			get { return (IList)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public int SelectedIndex
		{
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}

		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public Color TextColor {
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		public string Title {
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

        public bool AdjustsFontSizeToFitWidth
        {
            get { return (bool)GetValue(AdjustsFontSizeToFitWidthProperty); }
            set { SetValue(AdjustsFontSizeToFitWidthProperty, value); }
        }

        public FontAttributes FontAttributes
        {
            get { return (FontAttributes)GetValue(FontAttributesProperty); }
            set { SetValue(FontAttributesProperty, value); }
        }

        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public TextAlignment HorizontalTextAlignment
        {
            get { return (TextAlignment)GetValue(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        public TextAlignment VerticalTextAlignment
        {
            get { return (TextAlignment)GetValue(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        public event EventHandler SelectedIndexChanged;
		BindingBase _itemDisplayBinding;
		public BindingBase ItemDisplayBinding {
			get { return _itemDisplayBinding; }
			set {
				if (_itemDisplayBinding == value)
					return;

				OnPropertyChanging();
				var oldValue = value;
				_itemDisplayBinding = value;
				OnItemDisplayBindingChanged(oldValue, _itemDisplayBinding);
				OnPropertyChanged();
			}
		}

		static readonly BindableProperty s_displayProperty =
			BindableProperty.Create("Display", typeof(string), typeof(Picker), default(string));

		string GetDisplayMember(object item)
		{
			if (ItemDisplayBinding == null)
				return item.ToString();

			ItemDisplayBinding.Apply(item, this, s_displayProperty);
			ItemDisplayBinding.Unapply();
			return (string)GetValue(s_displayProperty);
		}

		static object CoerceSelectedIndex(BindableObject bindable, object value)
		{
			var picker = (Picker)bindable;
			return picker.Items == null ? -1 : ((int)value).Clamp(-1, picker.Items.Count - 1);
		}

		void OnItemDisplayBindingChanged(BindingBase oldValue, BindingBase newValue)
		{
			ResetItems();
		}

		void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SelectedIndex = SelectedIndex.Clamp(-1, Items.Count - 1);
			UpdateSelectedItem();
		}

		static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((Picker)bindable).OnItemsSourceChanged((IList)oldValue, (IList)newValue);
		}

		void OnItemsSourceChanged(IList oldValue, IList newValue)
		{ 
			var oldObservable = oldValue as INotifyCollectionChanged;
			if (oldObservable != null)
				oldObservable.CollectionChanged -= CollectionChanged;

			var newObservable = newValue as INotifyCollectionChanged;
			if (newObservable != null) {
				newObservable.CollectionChanged += CollectionChanged;
			}

			if (newValue != null) {
				((LockableObservableListWrapper)Items).IsLocked = true;
				ResetItems();
			} else {
				((LockableObservableListWrapper)Items).Clear();
				((LockableObservableListWrapper)Items).IsLocked = false;
			}
		}

		void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
			case NotifyCollectionChangedAction.Add:
				AddItems(e);
				break;
			case NotifyCollectionChangedAction.Remove:
				RemoveItems(e);
				break;
			default: //Move, Replace, Reset
				ResetItems();
				break;
			}
		}
		void AddItems(NotifyCollectionChangedEventArgs e)
		{
			int index = e.NewStartingIndex < 0 ? Items.Count : e.NewStartingIndex;
            ((LockableObservableListWrapper)Items).RunInternal(() =>
            {
                foreach (object newItem in e.NewItems)
                    ((LockableObservableListWrapper)Items).Insert(index++, GetDisplayMember(newItem));
            });
		}

		void RemoveItems(NotifyCollectionChangedEventArgs e)
		{
			int index = e.OldStartingIndex < Items.Count ? e.OldStartingIndex : Items.Count;
            ((LockableObservableListWrapper)Items).RunInternal(() =>
            {
                foreach (object _ in e.OldItems)
                    ((LockableObservableListWrapper)Items).RemoveAt(index--);
            });
		}

		void ResetItems()
		{
			if (ItemsSource == null)
				return;
            ((LockableObservableListWrapper)Items).RunInternal(() =>
            {
                ((LockableObservableListWrapper)Items).Clear();
                foreach (object item in ItemsSource)
                    ((LockableObservableListWrapper)Items).Add(GetDisplayMember(item));
                UpdateSelectedItem();
            });
		}

		static void OnSelectedIndexChanged(object bindable, object oldValue, object newValue)
		{
			var picker = (Picker)bindable;
			picker.UpdateSelectedItem();
            picker.SelectedIndexChanged?.Invoke(bindable, EventArgs.Empty);
        }

        static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var picker = (Picker)bindable;
			picker.UpdateSelectedIndex(newValue);
		}

		void UpdateSelectedIndex(object selectedItem)
		{
			if (ItemsSource != null) {
				SelectedIndex = ItemsSource.IndexOf(selectedItem);
				return;
			}
			SelectedIndex = Items.IndexOf(selectedItem);
		}

		void UpdateSelectedItem()
		{
			if (SelectedIndex == -1) {
				SelectedItem = null;
				return;
			}

			if (ItemsSource != null) {
				SelectedItem = ItemsSource [SelectedIndex];
				return;
			}

			SelectedItem = Items [SelectedIndex];
		}

		public IPlatformElementConfiguration<T, Picker> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

        class LockableObservableListWrapper : ObservableList<string>
        {
			public bool IsLocked { get; set; }

			void ThrowOnLocked()
			{
				if (!IsInternal && IsLocked)
					throw new InvalidOperationException("The Items list can not be manipulated if the ItemsSource property is set");			
			}

            private bool IsInternal = false;

            private object _RunInternalLock = new object();
            internal void RunInternal(Action toRun)
            {
                lock (_RunInternalLock)
                {
                    IsInternal = true;
                    toRun();
                    IsInternal = false;
                }
            }

            protected override void ClearItems()
            {
                ThrowOnLocked();
                base.ClearItems();
            }

            protected override void SetItem(int index, string item)
            {
                ThrowOnLocked();
                base.SetItem(index, item);
            }

            protected override void InsertItem(int index, string item)
            {
                ThrowOnLocked();
                base.InsertItem(index, item);
            }

            protected override void MoveItem(int oldIndex, int newIndex)
            {
                ThrowOnLocked();
                base.MoveItem(oldIndex, newIndex);
            }

            protected override void RemoveItem(int index)
            {
                ThrowOnLocked();
                base.RemoveItem(index);
            }
		}
	}
}