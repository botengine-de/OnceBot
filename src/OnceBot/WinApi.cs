using Bib3.Geometrik;
using BotEngine.WinApi;
using BotEngine.Windows;
using System;
using System.Collections.Generic;

namespace OnceBot
{
	static public class WinApi
	{
		static public KeyValuePair<UInt32[], int> Raster32BitFromClientRectFromWindowMitHandleOverDesktop(this IntPtr hWnd, bool flagCaptureblt = false)
		{
			var desktopHWnd = BotEngine.WinApi.User32.GetDesktopWindow();

			RECT clientRect;
			User32.GetClientRect(hWnd, out clientRect);

			var pointLeftTop = clientRect.LeftTop;
			var pointRightBottom = clientRect.RightBottom;

			User32.ClientToScreen(hWnd, ref pointLeftTop);
			User32.ClientToScreen(hWnd, ref pointRightBottom);

			var windowClientRect = RectInt.FromMinPointAndMaxPoint(pointLeftTop.AsVektor2DInt(), pointRightBottom.AsVektor2DInt());

			var desktopRaster = BotEngine.Windows.Extension.Raster32BitVonClientRectVonWindowMitHandle(desktopHWnd, flagCaptureblt);

			if (desktopRaster.Key == null)
				return default(KeyValuePair<UInt32[], int>);

			var desktopRasterHeight = desktopRaster.Key.Length / desktopRaster.Value;

			var destPixelCount = clientRect.Width * clientRect.Height;

			var destRaster = new UInt32[destPixelCount];

			for (int rowInDest = 0; rowInDest < clientRect.Height; rowInDest++)
			{
				var rowInDesktop = rowInDest + pointLeftTop.y;

				if (rowInDesktop < 0 || desktopRasterHeight <= rowInDesktop)
					continue;

				var inRowOffsetPixelCount = Math.Max(0, -pointLeftTop.x);

				var inRowPixelCount = Math.Min(desktopRaster.Value, pointRightBottom.x) - Math.Max(0, pointLeftTop.x);

				Buffer.BlockCopy(
					desktopRaster.Key,
					(desktopRaster.Value * rowInDesktop + pointLeftTop.x + inRowOffsetPixelCount) * 4,
					destRaster,
					(clientRect.Width * rowInDest + inRowOffsetPixelCount) * 4,
					inRowPixelCount * 4);
			}

			return new KeyValuePair<uint[], int>(destRaster, clientRect.Width);
		}
	}
}
