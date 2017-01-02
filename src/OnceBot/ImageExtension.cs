using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace OnceBot
{
	static public class ImageExtension
	{
		static public KeyValuePair<UInt32[], int> RasterFromImage(this BitmapImage image)
		{
			if (null == image)
				return default(KeyValuePair<UInt32[], int>);

			var Array = new UInt32[image.PixelWidth * image.PixelHeight];

			image.CopyPixels(Array, image.PixelWidth * 4, 0);

			return new KeyValuePair<uint[], int>(Array, image.PixelWidth);
		}

		static public KeyValuePair<UInt32[], int> RasterFromFilePath(this string filePath)
		{
			if (null == filePath)
				return default(KeyValuePair<UInt32[], int>);

			var fileContent = Bib3.Glob.InhaltAusDataiMitPfaad(filePath);

			var image = Bib3.FCL.Glob.SictBitmapImageBerecne(fileContent);

			var raster = image.RasterFromImage();

			return raster;
		}
	}
}
