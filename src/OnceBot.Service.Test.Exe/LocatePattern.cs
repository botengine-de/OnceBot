using Bib3;
using Bib3.Geometrik;
using BotEngine;
using BotEngine.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace OnceBot.Service.Test.Exe
{
	static public class ServiceExtension
	{
		static public ImageProcessing.ImagePatternMatchSearchResponse Request(
			this Service service,
			ImageProcessing.ImagePatternMatchSearchRequest request) =>
			service?.ClientRequest(new Request
			{
				ImagePatternMatchSearch = request,
			}.SerializeToString())?.DeserializeFromString<Response>()?.ImagePatternMatchSearch;

		static public IEnumerable<ImageProcessing.ImagePatternMatch> RequestSetMatchFromRaster(
			this Service service,
			KeyValuePair<UInt32[], int> raster) =>
			service?.ClientRequest(new Request
			{
				ImagePatternMatchSearch = new ImageProcessing.ImagePatternMatchSearchRequest
				{
					Raster = raster,
				},
			}.SerializeToString())?.DeserializeFromString<Response>()?.ImagePatternMatchSearch?.SetMatch;
	}

	public class LocatePatternTestCase
	{
		public string FileName;

		public KeyValuePair<string, Vektor2DInt[]>[] setTypeMatch;

		public int ImagePatternMatchLocationTolerance = 4;

		public string[] setTypeIdIgnore;
	}

	class LocatePatternTestContainer
	{
		readonly Service service = new Service();

		const string DirectoryPath = null;

		static IDictionary<string, Vektor2DInt> patternTypeLocationTolerance = new[]
		{
			new KeyValuePair<string, Vektor2DInt>(PatternIdConfig.Coin, new Vektor2DInt(8,8)),
			new KeyValuePair<string, Vektor2DInt>(PatternIdConfig.CloseButton, new Vektor2DInt(8,8)),
			new KeyValuePair<string, Vektor2DInt>(PatternIdConfig.TooltipProgressMin20px, new Vektor2DInt(8,8)),
			new KeyValuePair<string, Vektor2DInt>(PatternIdConfig.TextClickToCollect, new Vektor2DInt(16,8)),
			new KeyValuePair<string, Vektor2DInt>(PatternIdConfig.Idle, new Vektor2DInt(8,8)),
			new KeyValuePair<string, Vektor2DInt>(PatternIdConfig.Hourglass, new Vektor2DInt(8,8)),
			new KeyValuePair<string, Vektor2DInt>(PatternIdConfig.WorkshopProductReady, new Vektor2DInt(8,8)),
		}.ToDictionary();

		static string testCaseFirst = "small[4]";

		static IEnumerable<LocatePatternTestCase> setTestCase = new[]
		{
			new LocatePatternTestCase
			{
				setTypeMatch    = new   []
				{
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.Idle,
						new [] {
							new Vektor2DInt(11,10),
						}),
				},
			},

			new LocatePatternTestCase
			{
				FileName    = "16.02.07.idle.min.png",

				setTypeMatch    = new   []
				{
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.Idle,
						new [] {
							new Vektor2DInt(453,440),
							new Vektor2DInt(391,473),
							new Vektor2DInt(369,572),

							new Vektor2DInt(730,604),
							new Vektor2DInt(611,613),
						}),
				},

				setTypeIdIgnore = new[]
				{
					PatternIdConfig.Coin,
					PatternIdConfig.WorkshopProductReady,
				},
			},

			new LocatePatternTestCase
			{
				FileName    = "16.04.27.workshop.produce.png",

				setTypeMatch    = new   []
				{
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.Hourglass,
						new [] {
							new Vektor2DInt(417,525),
							new Vektor2DInt(617,525),
							new Vektor2DInt(817,525),

							new Vektor2DInt(417,709),
							new Vektor2DInt(617,709),
							new Vektor2DInt(817,709),
						}),

					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.CloseButton,
						new [] {
							new Vektor2DInt(907,325),
						}),

				},
				setTypeIdIgnore = new[]
				{
					PatternIdConfig.Idle,
					PatternIdConfig.WorkshopProductReady,
				},
			},

			new LocatePatternTestCase
			{
				FileName    = "16.02.07.residence.collected.lesscoin.png",

				setTypeMatch    = new   []
				{
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.Idle,
						new [] {
							new Vektor2DInt(390,533),
							new Vektor2DInt(369,633),
						}),
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.Coin,
						new [] {
							new Vektor2DInt(1010,524),
							new Vektor2DInt(887,99),
							new Vektor2DInt(966,590),
						}),
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.WorkshopProductReady,
						new [] {
							new Vektor2DInt(988,100),
							new Vektor2DInt(326,563),
							new Vektor2DInt(265,597),
							new Vektor2DInt(672,633),
							new Vektor2DInt(610,676),
						}),
				},
				setTypeIdIgnore = new[]
				{
					PatternIdConfig.Idle,
				},
			},

			new LocatePatternTestCase()
			{
				FileName    = "coin.tooltip.png",

				setTypeMatch    = new   []
				{
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.Coin,
						new [] {
							new Vektor2DInt(10,11),
						}),
				},
			},
			new LocatePatternTestCase()
			{
				FileName    = "16.00.13.zoom.min.png",

				setTypeMatch    = new   []
				{
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.Coin,
						new [] {
							new Vektor2DInt(1140,79),
							new Vektor2DInt(713, 576),
							new Vektor2DInt(776, 543),
							new Vektor2DInt(934, 624),
							new Vektor2DInt(871, 655)
						}),
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.WorkshopProductReady,
						new [] {
							new Vektor2DInt(1241,80),
							new Vektor2DInt(877, 601),
							new Vektor2DInt(814, 632),
							new Vektor2DInt(659, 615),
						}),
				},
				setTypeIdIgnore = new[]
				{
					PatternIdConfig.Idle,
				},
			},
			new LocatePatternTestCase
			{
				FileName    = "16.00.14.residence.tooltip.prod.partial.png",

				setTypeMatch    = new   []
				{
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.Coin,
						new [] {
							new Vektor2DInt(1140,79),
							new Vektor2DInt(678,669),
							new Vektor2DInt(716,833),
						}),
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.TooltipProgressMin20px,
						new [] {
							new Vektor2DInt(735,863),
						}),
				},

				setTypeIdIgnore = new[]
				{
					PatternIdConfig.WorkshopProductReady,
					PatternIdConfig.Idle,
				},
			},
			new LocatePatternTestCase()
			{
				FileName    = "16.00.13.residence.tooltip.prod.finished.png",

				setTypeMatch    = new   []
				{
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.Coin,
						new [] {
							new Vektor2DInt(1140,79),
							new Vektor2DInt(776,543),
							new Vektor2DInt(714,577),
							new Vektor2DInt(934,624),
						}),
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.TextClickToCollect,
						new [] {
							new Vektor2DInt(838,758),
						}),
				},

				setTypeIdIgnore = new[]
				{
					PatternIdConfig.WorkshopProductReady,
					PatternIdConfig.Idle,
				},
			},
			new LocatePatternTestCase()
			{
				FileName    = "16.00.14.close.button.png",

				setTypeMatch    = new   []
				{
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.CloseButton,
						new [] {
							new Vektor2DInt(1270,388),
						}),
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.TooltipProgressMin20px,
						new [] {
							new Vektor2DInt(711,724),
						}),
				},

				setTypeIdIgnore = new[]
				{
					PatternIdConfig.WorkshopProductReady,
					PatternIdConfig.Idle,
				},
			},
			new LocatePatternTestCase()
			{
				FileName    = "16.00.20.residence.tooltip.prod.partial.coin.arrow.green.reduced.png",

				setTypeMatch    = new   []
				{
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.Coin,
						new [] {
							new Vektor2DInt(803,99),
							new Vektor2DInt(829,740),
							new Vektor2DInt(873,891),
						}),
					new KeyValuePair<string, Vektor2DInt[]>(
						PatternIdConfig.TooltipProgressMin20px,
						new [] {
							new Vektor2DInt(970,920),
						}),
				},

				setTypeIdIgnore = new[]
				{
					PatternIdConfig.WorkshopProductReady,
					PatternIdConfig.Idle,
				},
			},
		};

		[Test]
		public void One_match_for_image_containing_single_instance()
		{
			const string sampleSingleInstancePath = @"sample\single";

			var setTestCase = new[]
			{
				new
				{
					patternId = PatternIdConfig.WorkshopProductReady,
					directoryPath = "workshop.product.ready",
				},
				new
				{
					patternId = PatternIdConfig.Idle,
					directoryPath = "idle",
				},
				new
				{
					patternId = PatternIdConfig.CloseButton,
					directoryPath = "close.button",
				},
				new
				{
					patternId = PatternIdConfig.Coin,
					directoryPath = "coin.vp.min",
				},
				new
				{
					patternId = PatternIdConfig.Hourglass,
					directoryPath = "hourglass",
				},
			};

			var patternImageBasePath =
				System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(
				System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(
				System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)))))
				.PathToFilesysChild(sampleSingleInstancePath);

			patternImageBasePath = Regex.Replace(patternImageBasePath, @"^file\:\\", "");

			foreach (var testCase in setTestCase)
			{
				try
				{
					var directoryPath = patternImageBasePath.PathToFilesysChild(testCase.directoryPath);

					var setFilePath = System.IO.Directory.GetFiles(directoryPath, "*", System.IO.SearchOption.AllDirectories);

					if (setFilePath.Length < 1)
						throw new AssertionException("No files found under " + directoryPath);

					foreach (var sampleFilePath in setFilePath)
					{
						try
						{
							var raster = sampleFilePath.RasterFromFilePath();

							var setMatch =
								service?.RequestSetMatchFromRaster(raster)
								?.Select(match => new KeyValuePair<string, ImageProcessing.ImagePatternMatch>(match.SourcePatternId, match))
								?.ToArray();

							var foundPatternIds = setMatch.Select(match => match.Key).Distinct().ToArray();

							if (!foundPatternIds.Contains(testCase.patternId))
								throw new AssertionException("No match");

							var setErroneousMatchPatternId = foundPatternIds.Except(new[] { testCase.patternId }).ToArray();

							if (0 < setErroneousMatchPatternId.Length)
								throw new AssertionException("Too many matches: " + setErroneousMatchPatternId);
						}
						catch (Exception e)
						{
							throw new AssertionException("Failed for sample from path " + sampleFilePath, e);
						}
					}
				}
				catch (Exception e)
				{
					throw new AssertionException("Failed for test case " + testCase, e);
				}
			}
		}

		[Test]
		public void LocatePattern_InImage()
		{
			var listTestCase =
				setTestCase.OrderBy(testCase => testCase.FileName.RegexMatchSuccessIgnoreCase(Regex.Escape(testCaseFirst)) ? 0 : 1).ToArray();

			var setTestCaseDuration = new List<KeyValuePair<LocatePatternTestCase, int>>();

			foreach (var testCase in listTestCase)
			{
				var testCaseStopwatch = Stopwatch.StartNew();

				try
				{
					var filePath =
						System.IO.Path.IsPathRooted(testCase.FileName) ? testCase.FileName : DirectoryPath.PathToFilesysChild(testCase.FileName);

					var raster = filePath.RasterFromFilePath();

					var setMatch =
						service?.RequestSetMatchFromRaster(raster)
						?.Select(match => new KeyValuePair<string, ImageProcessing.ImagePatternMatch>(match.SourcePatternId, match))
						?.ToArray();

					var setMatchGroup =
						setMatch
						.GroupBy(match => match.Key)
						.ToArray();

					var setTypeId =
						testCase.setTypeMatch.Select(typeMatch => typeMatch.Key)
						.ConcatNullable(setMatch.Keys())
						.Except(testCase.setTypeIdIgnore.EmptyIfNull())
						.Distinct()
						.ToArray();

					foreach (var typeId in setTypeId)
					{
						try
						{
							var testCaseType = testCase.setTypeMatch?.FirstOrDefault(typeMatch => typeMatch.Key == typeId);

							var typeSetMatch = setMatchGroup?.FirstOrDefault(group => group.Key == typeId);

							foreach (var locationExpected in (testCaseType?.Value).EmptyIfNull())
							{
								var tolerance = patternTypeLocationTolerance.TryGetValueOrDefault(typeId);

								var regionExpected = RectInt.FromCenterAndSize(locationExpected, tolerance);

								var inRegionLocation = typeSetMatch?.Values().Where(c => regionExpected.ContainsPointForMinInclusiveAndMaxInclusive(c.Area.Center())).ToArray();

								if (!(0 < inRegionLocation?.Length))
									throw new System.Exception("no match at " + locationExpected.ToString());
							}

							Assert.AreEqual(testCaseType?.Value?.Length, typeSetMatch?.Count(), "count");
						}
						catch (System.Exception Exception)
						{
							throw new System.Exception("type " + typeId, Exception);
						}
					}

					setTestCaseDuration.Add(new KeyValuePair<LocatePatternTestCase, int>(testCase, (int)testCaseStopwatch.ElapsedMilliseconds));
				}
				catch (System.Exception Exception)
				{
					throw new System.Exception("test case " + testCase.FileName, Exception);
				}
			}

			var testCaseDurationLongest = setTestCaseDuration.OrderByDescending(t => t.Value).First();

			if (4444 < testCaseDurationLongest.Value)
				throw new System.Exception("test case " + testCaseDurationLongest.Key.FileName + " took " + testCaseDurationLongest.Value + "ms");
		}
	}
}
