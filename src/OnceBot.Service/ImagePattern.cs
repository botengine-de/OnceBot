using Bib3.Geometrik;
using System;

namespace OnceBot.Service
{
	public struct PatternElement
	{
		public Vektor2DInt[] locationOption;

		public int ColorR, ColorG, ColorB;

		public Func<int, int, int, bool> ColorConstraintDelegate;
	}

	public class ImagePattern
	{
		public string Id;

		public PatternElement[] ListElement;
	}
}
