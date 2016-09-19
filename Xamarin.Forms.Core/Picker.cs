using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_PickerRenderer))]
	public class Picker : View, IElementConfiguration<Picker>, IFontElement
    {
		public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(Picker), Color.Default);

		public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(Picker), default(string));

		public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(Picker), -1, BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				EventHandler eh = ((Picker)bindable).SelectedIndexChanged;
				if (eh != null)
					eh(bindable, EventArgs.Empty);
			}, coerceValue: CoerceSelectedIndex);

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create("FontFamily", typeof(string), typeof(Picker), default(string));

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(double), typeof(Picker), -1.0,
            defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Default, (Picker)bindable));

        public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create("FontAttributes", typeof(FontAttributes), typeof(Picker), FontAttributes.None);

        public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create("HorizontalTextAlignment", typeof(TextAlignment), typeof(Picker), TextAlignment.Start);
        public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create("VerticalTextAlignment", typeof(TextAlignment), typeof(Picker), TextAlignment.Start);

        public static readonly BindableProperty AdjustsFontSizeToFitWidthProperty = BindableProperty.Create("AdjustsFontSizeToFitWidth", typeof(bool), typeof(Picker), false);
        
        readonly Lazy<PlatformConfigurationRegistry<Picker>> _platformConfigurationRegistry;

		public Picker()
		{
			Items = new ObservableList<string>();
			((ObservableList<string>)Items).CollectionChanged += OnItemsCollectionChanged;
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Picker>>(() => new PlatformConfigurationRegistry<Picker>(this));
		}

		public IList<string> Items { get; }

		public int SelectedIndex
		{
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		public string Title
		{
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

		static object CoerceSelectedIndex(BindableObject bindable, object value)
		{
			var picker = (Picker)bindable;
			return picker.Items == null ? -1 : ((int)value).Clamp(-1, picker.Items.Count - 1);
		}

		void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			SelectedIndex = SelectedIndex.Clamp(-1, Items.Count - 1);
		}

		public IPlatformElementConfiguration<T, Picker> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}
	}
}