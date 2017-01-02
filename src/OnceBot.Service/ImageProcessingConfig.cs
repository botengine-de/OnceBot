using Bib3;
using Bib3.Geometrik;
using OnceBot.ImageProcessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnceBot.Service
{
	public class ImageProcessingConfig
	{
		public IEnumerable<KeyValuePair<string, ImagePattern[]>> setTypeSetPattern;

		public IEnumerable<KeyValuePair<string, ImagePatternMatch>> SetMatchInImage(KeyValuePair<UInt32[], int> raster)
		{
			if (null == raster.Key)
				return null;

			var rasterSizeA = raster.Value;
			var rasterSizeB = raster.Key.Length / rasterSizeA;

			var setTypeIdAndPattern =
				setTypeSetPattern
				.SelectMany(typeIdAndSetPattern => typeIdAndSetPattern.Value.Select(pattern => new KeyValuePair<string, ImagePattern>(typeIdAndSetPattern.Key, pattern)))
				.ToArray();

			var setPatternMatch =
				setTypeIdAndPattern
				.AsParallel()
				.Select(typeIdAndPattern =>
				{
					var patternFirstElement = typeIdAndPattern.Value.ListElement.FirstOrDefault();

					var patternSetOffset = typeIdAndPattern.Value.ListElement.SelectMany(elem => elem.locationOption).ToArray();

					var boundRect = patternSetOffset.BoundingRectangle();

					var tolerance = 0x20;

					var patternSetMatchLocation = new List<Vektor2DInt>();

					for (int rasterLocB = Math.Max(0, (int)-boundRect.Min1); rasterLocB < rasterSizeB - Math.Max(0, boundRect.Max1 + 1); rasterLocB++)
					{
						for (int rasterLocA = Math.Max(0, (int)-boundRect.Min0); rasterLocA < rasterSizeA - Math.Max(0, boundRect.Max1 + 1); rasterLocA++)
						{
							var rasterLoc = new Vektor2DInt(rasterLocA, rasterLocB);

							bool match = true;

							for (int patternElemIndex = 0; patternElemIndex < typeIdAndPattern.Value.ListElement.Length; ++patternElemIndex)
							{
								var elem = typeIdAndPattern.Value.ListElement[patternElemIndex];

								var elemMatch = false;

								foreach (var elemLoc in elem.locationOption)
								{
									var inRasterLocation = rasterLoc + elemLoc;

									var rasterPixelIndex = inRasterLocation.A + inRasterLocation.B * rasterSizeA;

									var rasterColorAggr = raster.Key[rasterPixelIndex];

									var rasterR = (rasterColorAggr >> 16) & 0xff;
									var rasterG = (rasterColorAggr >> 8) & 0xff;
									var rasterB = (rasterColorAggr >> 0) & 0xff;

									var ColorConstraint = elem.ColorConstraintDelegate;

									if (null == ColorConstraint)
									{
										var DiffR = Math.Abs(rasterR - elem.ColorR);
										var DiffG = Math.Abs(rasterG - elem.ColorG);
										var DiffB = Math.Abs(rasterB - elem.ColorB);

										if (!(tolerance < DiffR || tolerance < DiffG || tolerance < DiffB))
										{
											elemMatch = true;
										}
									}
									else
									{
										elemMatch = ColorConstraint((int)rasterR, (int)rasterG, (int)rasterB);
									}

									if (elemMatch)
										break;
								}

								if (!elemMatch)
								{
									match = false;
									break;
								}
							}

							if (match)
								patternSetMatchLocation.Add(rasterLoc);
						}
					}

					return new KeyValuePair<string, IEnumerable<ImagePatternMatch>>(typeIdAndPattern.Key,
						patternSetMatchLocation.Select(matchLocation =>
						new ImagePatternMatch()
						{
							SourcePatternId = typeIdAndPattern.Value.Id,
							Area = RectInt.FromCenterAndSize(matchLocation, new Vektor2DInt()),
						}));
				})
				.ToArray();

			var setTypeIdAndSetMatchAggregated =
				setPatternMatch
				.GroupBy(typeIdAndSetMatch => typeIdAndSetMatch.Key)
				.Select(group => new KeyValuePair<string, ImagePatternMatch[]>(group.Key, group.Values().ConcatNullable().PatternMatchLocationAggregated(4).ToArray()))
				.ToArray();

			return
				setTypeIdAndSetMatchAggregated
				.SelectMany(typeIdAndSetMatchAggregated => typeIdAndSetMatchAggregated.Value
					.Select(match => new KeyValuePair<string, ImagePatternMatch>(typeIdAndSetMatchAggregated.Key, match)));
		}
	}

	static public class ImageProcessingExtension
	{
		static public IEnumerable<ImagePatternMatch> MapIdFromKey(
			this IEnumerable<KeyValuePair<string, ImagePatternMatch>> source) =>
			source?.Select(match => new ImagePatternMatch()
			{
				Area = match.Value.Area,
				SourcePatternId = match.Key,
			});

		static public IEnumerable<ImagePatternMatch> PatternMatchLocationAggregated(
			this IEnumerable<ImagePatternMatch> source,
			int inGroupDistanceMax)
		{
			var setLocationRemaining = source.ToList();

			var setGroup = new List<ImagePatternMatch[]>();

			while (0 < setLocationRemaining.Count)
			{
				var next = setLocationRemaining.First();

				var inGroupSet = new List<ImagePatternMatch>(new[] { next });

				setLocationRemaining.Remove(next);

				while (true)
				{
					var setNear = setLocationRemaining.Where(t => inGroupSet.Any(inGroup => (inGroup.Area.Center() - t.Area.Center()).LengthSquared() <= inGroupDistanceMax * inGroupDistanceMax)).ToArray();

					if (setNear.Length <= 0)
						break;

					inGroupSet.AddRange(setNear);
					setLocationRemaining.RemoveAll(t => setNear.Contains(t));
				}

				setGroup.Add(inGroupSet.ToArray());
			}

			return
				setGroup.Select(group =>
				{
					var groupRegion = group.Select(match => match.Area.Center()).BoundingRectangle();

					return group.OrderBy(match => (match.Area.Center() - groupRegion.Center()).LengthSquared()).First();
				});
		}
	}
}
