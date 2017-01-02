using Bib3.Geometrik;
using System.Collections.Generic;
using System.Linq;

namespace OnceBot.Service
{
	public class Config
	{
		public ImageProcessingConfig ImageProcessing;

		static public Config ConfigDefault =
			ConfigDefaultConstruct();

		static public ImagePattern PatternCoinVpMin => new ImagePattern()
		{
			Id = "coin.vp.min",
			ListElement = new[]
			{
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(9,9) },
					ColorR  = 254,
					ColorG = 197,
					ColorB = 73,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(7,12) },
					ColorR  = 205,
					ColorG  = 137,
					ColorB = 26,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(4,14) },
					ColorR  = 254,
					ColorG  = 182,
					ColorB = 31,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(17,10),new   Vektor2DInt(16,10) },
					ColorR  = 71,
					ColorG  = 27,
					ColorB = 3,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(8,2),new   Vektor2DInt(8,1) },
					ColorR  = 253,
					ColorG  = 222,
					ColorB = 104,
				},
			}
			.Select(elem =>
			{
				elem.locationOption = elem.locationOption.Select(location => location - new Vektor2DInt(9, 9)).ToArray();
				return elem;
			})
			.ToArray(),
		};

		static public ImagePattern PatternCoinTooltip => new ImagePattern()
		{
			Id = "coin.tooltip",
			ListElement = new[]
			{
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(9,11) },
					ColorR  = 255,
					ColorG = 209,
					ColorB = 105,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(10,2),new   Vektor2DInt(10,3) },
					ColorR  = 255,
					ColorG  = 225,
					ColorB = 101,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(10,14) },
					ColorR  = 203,
					ColorG  = 130,
					ColorB = 18,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(19,7),new   Vektor2DInt(20,7) },
					ColorR  = 76,
					ColorG  = 30,
					ColorB = 4,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(0,13),new   Vektor2DInt(1,13) },
					ColorR  = 57,
					ColorG  = 24,
					ColorB = 8,
				},
			}
			.Select(elem =>
			{
				elem.locationOption = elem.locationOption.Select(location => location - new Vektor2DInt(10, 11)).ToArray();
				return elem;
			})
			.ToArray(),
		};

		static public ImagePattern PatternTooltipProgressMin20px => new ImagePattern()
		{
			ListElement = new[]
			{
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(21,10) },
					ColorR  = 224,
					ColorG = 198,
					ColorB = 23,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(15,8) },
					ColorR  = 29,
					ColorG = 193,
					ColorB = 204,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(5,8) },
					ColorR  = 29,
					ColorG = 193,
					ColorB = 204,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(10,16) },
					ColorR  = 27,
					ColorG = 111,
					ColorB = 154,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(23,4) },
					ColorR  = 33,
					ColorG = 20,
					ColorB = 11,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(23,17) },
					ColorR  = 33,
					ColorG = 20,
					ColorB = 11,
				},
			}
			.Select(elem =>
			{
				elem.locationOption = elem.locationOption.Select(location => location - new Vektor2DInt(21, 10)).ToArray();
				return elem;
			})
			.ToArray(),
		};

		static public ImagePattern PatternCloseButton => new ImagePattern()
		{
			ListElement = new[]
			{
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(16,15) },
					ColorR  = 252,
					ColorG = 205,
					ColorB = 49,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(11,15) },
					ColorR  = 167,
					ColorG = 75,
					ColorB = 28,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(12,21) },
					ColorR  = 252,
					ColorG = 184,
					ColorB = 42,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(21,24) },
					ColorR  = 108,
					ColorG = 43,
					ColorB = 10,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(12,10) },
					ColorR  = 253,
					ColorG = 228,
					ColorB = 62,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(15,21) },
					ColorR  = 152,
					ColorG = 67,
					ColorB = 24,
				},
			}
			.Select(elem =>
			{
				elem.locationOption = elem.locationOption.Select(location => location - new Vektor2DInt(16, 15)).ToArray();
				return elem;
			})
			.ToArray(),
		};

		static public ImagePattern IdleSmall => new ImagePattern()
		{
			ListElement = new[]
			{
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(2,1) },
					ColorR  = 255,
					ColorG = 255,
					ColorB = 255,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(2,0),new   Vektor2DInt(2,11), new Vektor2DInt(2,10) },
					ColorR  = 128,
					ColorG = 128,
					ColorB = 128,
					ColorConstraintDelegate = (r,g,b) => r < 200 && g < 200 && b < 200,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(13 ,12) },
					ColorR  = 255,
					ColorG = 255,
					ColorB = 255,
				},
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(5 ,8),new   Vektor2DInt(6 ,8),new   Vektor2DInt(6 ,7) },
					ColorR  = 187,
					ColorG = 187,
					ColorB = 187,
				},
				/*
				new PatternElement()
				{
					locationOption    = new[] { new   Vektor2DInt(3 ,3), },
					ColorR  = 154,
					ColorG = 154,
					ColorB = 154,
				},
				*/
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(3 ,5), new Vektor2DInt(3,6), new Vektor2DInt(4,5) },
					ColorR  = 167,
					ColorG = 167,
					ColorB = 167,
				},
				/*
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(10 ,15), new Vektor2DInt(11, 15), new Vektor2DInt(10,16),new Vektor2DInt(11, 16) },
					ColorR  = 130,
					ColorG = 130,
					ColorB = 130,
				},
				*/
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(10 ,15), new Vektor2DInt(11, 15), new Vektor2DInt(10,16),new Vektor2DInt(11, 16) },
					ColorConstraintDelegate = new   System.Func<int, int, int, bool>((r, g, b) => r < 210 && g < 210 && b < 210),
				},
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(12,10), new Vektor2DInt(13, 10) },
					ColorConstraintDelegate = new   System.Func<int, int, int, bool>((r, g, b) => r < 160 && g < 160 && b < 160),
				},
			}
			.Select(elem =>
			{
				elem.locationOption = elem.locationOption.Select(location => location - new Vektor2DInt(10, 11)).ToArray();
				return elem;
			})
			.ToArray(),
		};

		static public ImagePattern Hourglass => new ImagePattern
		{
			ListElement = new[]
			{
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(7,16) },
					ColorR  = 250,
					ColorG = 219,
					ColorB = 106,
				},
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(5,15) },
					ColorR  = 250,
					ColorG = 250,
					ColorB = 250,
				},
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(13 ,4) },
					ColorR  = 155,
					ColorG = 97,
					ColorB = 38,
				},
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(2 ,17) },
					ColorR  = 133,
					ColorG = 64,
					ColorB = 20,
				},
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(20 ,8), },
					ColorR  = 20,
					ColorG = 18,
					ColorB = 14,
				},
			}
			.Select(elem =>
			{
				elem.locationOption = elem.locationOption.Select(location => location - new Vektor2DInt(10, 12)).ToArray();
				return elem;
			})
			.ToArray(),
		};

		static public ImagePattern WorkshopProductReadySmall => new ImagePattern()
		{
			ListElement = new[]
			{
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(17,11) },
					ColorR  = 187,
					ColorG = 113,
					ColorB = 72,
				},
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(6,5),new   Vektor2DInt(5,5) },
					ColorR  = 183,
					ColorG = 178,
					ColorB = 167,
				},
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(13 ,12),new   Vektor2DInt(12 ,12) },
					ColorR  = 8,
					ColorG = 8,
					ColorB = 32,
				},
				new PatternElement
				{
					locationOption    = new[] { new   Vektor2DInt(11 ,12), },
					ColorR  = 95,
					ColorG = 95,
					ColorB = 114,
				},
			}
			.Select(elem =>
			{
				elem.locationOption = elem.locationOption.Select(location => location - new Vektor2DInt(11, 8)).ToArray();
				return elem;
			})
			.ToArray(),
		};

		static public ImagePattern PatternTextClickToCollect => new ImagePattern()
		{
			ListElement = new[]
			{
				new KeyValuePair<bool, Vektor2DInt>(true, new Vektor2DInt(38, 5)),
				new KeyValuePair<bool, Vektor2DInt>(false, new Vektor2DInt(37, 5)),
				new KeyValuePair<bool, Vektor2DInt>(true, new Vektor2DInt(26, 2)),
				new KeyValuePair<bool, Vektor2DInt>(false, new Vektor2DInt(5, 0)),
				new KeyValuePair<bool, Vektor2DInt>(true, new Vektor2DInt(5, 1)),
				new KeyValuePair<bool, Vektor2DInt>(false, new Vektor2DInt(71, 2)),
				new KeyValuePair<bool, Vektor2DInt>(true, new Vektor2DInt(72, 2)),
				new KeyValuePair<bool, Vektor2DInt>(true, new Vektor2DInt(94, 4)),
				new KeyValuePair<bool, Vektor2DInt>(false, new Vektor2DInt(93, 5)),
			}
			.Select(foregroundAndLocation => foregroundAndLocation.Key ?
				new PatternElement()
				{
					locationOption = new[] { foregroundAndLocation.Value },
					ColorR = 124,
					ColorG = 114,
					ColorB = 86,
				} :
				new PatternElement()
				{
					locationOption = new[] { foregroundAndLocation.Value },
					ColorR = 50,
					ColorG = 42,
					ColorB = 22,
				})
			.Select(elem =>
			{
				elem.locationOption = elem.locationOption.Select(location => location - new Vektor2DInt(46, 7)).ToArray();
				return elem;
			})
			.ToArray(),
		};

		static public Config ConfigDefaultConstruct()
		{
			return new Config()
			{
				ImageProcessing = new ImageProcessingConfig()
				{
					setTypeSetPattern = new[]
					{
						new KeyValuePair<string,ImagePattern[]>(PatternIdConfig.Coin, new[] { PatternCoinVpMin,PatternCoinTooltip }),
						new KeyValuePair<string,ImagePattern[]>(PatternIdConfig.TooltipProgressMin20px, new[] { PatternTooltipProgressMin20px }),
						new KeyValuePair<string,ImagePattern[]>(PatternIdConfig.TextClickToCollect, new []{ PatternTextClickToCollect }),
						new KeyValuePair<string,ImagePattern[]>(PatternIdConfig.CloseButton, new []{ PatternCloseButton }),
						new KeyValuePair<string,ImagePattern[]>(PatternIdConfig.Idle, new []{ IdleSmall }),
						new KeyValuePair<string,ImagePattern[]>(PatternIdConfig.Hourglass, new []{ Hourglass }),
						new KeyValuePair<string,ImagePattern[]>(PatternIdConfig.WorkshopProductReady, new []{ WorkshopProductReadySmall }),
					},
				},
			};
		}
	}
}
