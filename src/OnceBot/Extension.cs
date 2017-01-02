using Bib3.Geometrik;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnceBot
{
	static public class Extension
	{
		static public string RenderForUI(this Vektor2DInt vektor) =>
			vektor.A + "|" + vektor.B;

		static public int RandomInt() => new Random((int)Bib3.Glob.StopwatchZaitMiliSictInt()).Next();

		static public IEnumerable<Vektor2DInt> ListPointInterpolated(Vektor2DInt start, Vektor2DInt end, int count) =>
			Enumerable.Range(0, count).Select(i => start + (end - start) / Math.Max(0, count - 1) * i);

	}
}
