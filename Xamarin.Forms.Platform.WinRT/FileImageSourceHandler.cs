using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public sealed class FileImageSourceHandler : IImageSourceHandler
	{
		public async Task<Windows.UI.Xaml.Media.ImageSource> LoadImageAsync(ImageSource imagesource, CancellationToken cancellationToken = new CancellationToken())
		{
			Windows.UI.Xaml.Media.ImageSource image = null;
			var filesource = imagesource as FileImageSource;
			if (filesource != null)
			{
				string file = filesource.File;
                if (System.IO.Path.IsPathRooted(file))
                {
                    using (Windows.Storage.Streams.IRandomAccessStream fileStream = await (await Windows.Storage.StorageFile.GetFileFromPathAsync(file)).OpenAsync(Windows.Storage.FileAccessMode.Read))
                    {
                        // Set the image source to the selected bitmap.
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(fileStream);
                        image = bitmapImage;
                    }
                }
                else
                    image = new BitmapImage(new Uri("ms-appx:///" + file));
            }

            return image;
		}
	}
}