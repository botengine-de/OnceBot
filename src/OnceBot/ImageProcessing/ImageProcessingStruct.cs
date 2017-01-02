using Bib3.Geometrik;
using System;
using System.Collections.Generic;

namespace OnceBot.ImageProcessing
{
	public class ImagePatternMatchSearchRequest
	{
		public KeyValuePair<UInt32[], int> Raster;

		public string[] SetPatternId;
	}

	public class ImagePatternMatchSearchResponse
	{
		public ImagePatternMatch[] SetMatch;
	}

	public class ImagePatternMatch
	{
		public string SourcePatternId;

		public RectInt Area;
	}

}
