using Android.Views;
using av = Android.Views;

namespace Xamarin.Forms.Platform.Android
{
	internal static class AlignmentExtensions
	{
		internal static GravityFlags ToHorizontalGravityFlags(this TextAlignment alignment)
		{
			switch (alignment)
			{
				case TextAlignment.Center:
					return GravityFlags.CenterHorizontal;
				case TextAlignment.End:
					return GravityFlags.Right;
				default:
					return GravityFlags.Left;
			}
		}

		internal static GravityFlags ToVerticalGravityFlags(this TextAlignment alignment)
		{
			switch (alignment)
			{
				case TextAlignment.Start:
					return GravityFlags.Top;
				case TextAlignment.End:
					return GravityFlags.Bottom;
				default:
					return GravityFlags.CenterVertical;
			}
		}

        internal static av.TextAlignment ToNativeTextAlignment(this TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Center:
                    return av.TextAlignment.Center;
                case TextAlignment.End:
                    return av.TextAlignment.TextEnd;
                default:
                    return av.TextAlignment.TextStart;
            }
        }
    }
}