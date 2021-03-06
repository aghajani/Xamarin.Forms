﻿using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using WApplication = Windows.UI.Xaml.Application;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public static class FontExtensions
	{
        private static string _FixFontPath(string fontPath)
        {
            return $@"/Assets/{fontPath}";
        }

		public static void ApplyFont(this Control self, Font font)
		{
            string fontFamily = font.FontFamily ?? _FixFontPath(Application.DefaultFontFamily);
			self.FontSize = font.UseNamedSize ? font.NamedSize.GetFontSize() : font.FontSize;
			self.FontFamily = !string.IsNullOrEmpty(fontFamily) ? new FontFamily(fontFamily) : (FontFamily)WApplication.Current.Resources["ContentControlThemeFontFamily"];
			self.FontStyle = font.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = font.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

        public static void ApplyFont(this TextBlock self, Font font)
		{
            string fontFamily = font.FontFamily ?? _FixFontPath(Application.DefaultFontFamily);
            self.FontSize = font.UseNamedSize ? font.NamedSize.GetFontSize() : font.FontSize;
			self.FontFamily = !string.IsNullOrEmpty(fontFamily) ? new FontFamily(fontFamily) : (FontFamily)WApplication.Current.Resources["ContentControlThemeFontFamily"];
			self.FontStyle = font.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = font.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		public static void ApplyFont(this TextElement self, Font font)
		{
            string fontFamily = font.FontFamily ?? _FixFontPath(Application.DefaultFontFamily);
            self.FontSize = font.UseNamedSize ? font.NamedSize.GetFontSize() : font.FontSize;
			self.FontFamily = !string.IsNullOrEmpty(fontFamily) ? new FontFamily(fontFamily) : (FontFamily)WApplication.Current.Resources["ContentControlThemeFontFamily"];
			self.FontStyle = font.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = font.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		internal static void ApplyFont(this Control self, IFontElement element)
		{
            string fontFamily = element.FontFamily ?? _FixFontPath(Application.DefaultFontFamily);
			self.FontSize = element.FontSize;
			self.FontFamily = !string.IsNullOrEmpty(fontFamily) ? new FontFamily(fontFamily) : (FontFamily)WApplication.Current.Resources["ContentControlThemeFontFamily"];
			self.FontStyle = element.FontAttributes.HasFlag(FontAttributes.Italic) ? FontStyle.Italic : FontStyle.Normal;
			self.FontWeight = element.FontAttributes.HasFlag(FontAttributes.Bold) ? FontWeights.Bold : FontWeights.Normal;
		}

		internal static double GetFontSize(this NamedSize size)
		{
            if (Application.FontSize_Get == null)
            {
                // These are values pulled from the mapped sizes on Windows Phone, WinRT has no equivalent sizes, only intents.
                switch (size)
                {
                    case NamedSize.Default:
                        return (double)WApplication.Current.Resources["ControlContentThemeFontSize"];
                    case NamedSize.Micro:
                        return 18.667 - 3;
                    case NamedSize.Small:
                        return 18.667;
                    case NamedSize.Medium:
                        return 22.667;
                    case NamedSize.Large:
                        return 32;
                    default:
                        throw new ArgumentOutOfRangeException("size");
                }
            }
            else
                return Application.FontSize_Get(size);
		}

		internal static bool IsDefault(this IFontElement self)
		{
			return string.IsNullOrWhiteSpace(Application.DefaultFontFamily) && Application.FontSize_Get == null && self.FontFamily == null && self.FontSize == Device.GetNamedSize(NamedSize.Default, typeof(Label), true) && self.FontAttributes == FontAttributes.None;
		}
	}
}