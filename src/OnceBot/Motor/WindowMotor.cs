using Bib3;
using Bib3.Geometrik;
using BotEngine.Motor;
using BotEngine.WinApi;
using BotEngine.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OnceBot.Motor
{
	public class WindowMotor : IMotor
	{
		readonly public IntPtr WindowHandle;

		public int MouseMoveDelay;

		public int MouseEventDelay;

		public int KeyboardEventTimeDistanceMilli = 40;

		/// <summary>
		/// For some reason, the mouse positions seem to be offset when moving the mouse in the window client area.
		/// </summary>
		static public Vektor2DInt MouseOffsetStatic = new Vektor2DInt(2, 2);

		public WindowMotor(IntPtr windowHandle)
		{
			this.WindowHandle = windowHandle;
		}

		static public void EnsureWindowIsForeground(
			IntPtr windowHandle)
		{
			var PreviousForegroundWindowHandle = BotEngine.WinApi.User32.GetForegroundWindow();

			if (PreviousForegroundWindowHandle == windowHandle)
				return;

			BotEngine.WinApi.User32.SetForegroundWindow(windowHandle);
		}

		void EnsureWindowIsForeground() => EnsureWindowIsForeground(WindowHandle);

		static public void MouseMoveToPointInClientRect(
			IntPtr windowHandle,
			Vektor2DInt destinationPointInClientRect,
			out POINT destinationPointInScreen)
		{
			destinationPointInScreen = destinationPointInClientRect.AsWindowsPoint();

			// get screen coordinates
			BotEngine.WinApi.User32.ClientToScreen(windowHandle, ref destinationPointInScreen);

			var lParam = (IntPtr)((((int)destinationPointInClientRect.B) << 16) | ((int)destinationPointInClientRect.A));
			var wParam = IntPtr.Zero;

			BotEngine.WinApi.User32.SetCursorPos(destinationPointInScreen.x, destinationPointInScreen.y);

			BotEngine.WinApi.User32.SendMessage(windowHandle, (uint)SictMessageTyp.WM_MOUSEMOVE, wParam, lParam);
		}

		/// <summary>
		/// https://msdn.microsoft.com/en-us/library/windows/desktop/ms646260(v=vs.85).aspx
		/// </summary>
		static KeyValuePair<MouseButtonIdEnum, int>[] MouseEventButtonDownFlag = new[]
		{
			new KeyValuePair<MouseButtonIdEnum, int>( MouseButtonIdEnum.Left, (int)User32.MouseEventFlagEnum.MOUSEEVENTF_LEFTDOWN),
			new KeyValuePair<MouseButtonIdEnum, int>( MouseButtonIdEnum.Right, (int)User32.MouseEventFlagEnum.MOUSEEVENTF_RIGHTDOWN),
			new KeyValuePair<MouseButtonIdEnum, int>( MouseButtonIdEnum.Left, (int)User32.MouseEventFlagEnum.MOUSEEVENTF_MIDDLEDOWN),
		};

		/// <summary>
		/// https://msdn.microsoft.com/en-us/library/windows/desktop/ms646260(v=vs.85).aspx
		/// </summary>
		static KeyValuePair<MouseButtonIdEnum, int>[] MouseEventButtonUpFlag = new[]
		{
			new KeyValuePair<MouseButtonIdEnum, int>( MouseButtonIdEnum.Left, (int)User32.MouseEventFlagEnum.MOUSEEVENTF_LEFTUP),
			new KeyValuePair<MouseButtonIdEnum, int>( MouseButtonIdEnum.Right, (int)User32.MouseEventFlagEnum.MOUSEEVENTF_RIGHTUP),
			new KeyValuePair<MouseButtonIdEnum, int>( MouseButtonIdEnum.Left, (int)User32.MouseEventFlagEnum.MOUSEEVENTF_MIDDLEUP),
		};

		static int User32MouseEventFlagAggregate(
			IEnumerable<MouseButtonIdEnum> mouseButtonDown,
			IEnumerable<MouseButtonIdEnum> mouseButtonUp) =>
			MouseEventButtonDownFlag.Where(buttonIdAndFlag => mouseButtonDown?.Contains(buttonIdAndFlag.Key) ?? false)
				.Concat(
				MouseEventButtonUpFlag.Where(buttonIdAndFlag => mouseButtonUp?.Contains(buttonIdAndFlag.Key) ?? false))
				.Select(buttonIdAndFlag => buttonIdAndFlag.Value)
				.Aggregate(0, (a, b) => a | b);

		static public void User32MouseEvent(
			Vektor2DInt mousePositionOnScreen,
			IEnumerable<MouseButtonIdEnum> mouseButtonDown,
			IEnumerable<MouseButtonIdEnum> mouseButtonUp)
		{
			var MouseEventFlag = User32MouseEventFlagAggregate(mouseButtonDown, mouseButtonUp);

			User32.mouse_event(
				(uint)MouseEventFlag | (uint)User32.MouseEventFlagEnum.MOUSEEVENTF_ABSOLUTE,
				(uint)mousePositionOnScreen.A,
				(uint)mousePositionOnScreen.B,
				0,
				UIntPtr.Zero);
		}

		public MotionResult ActSequenceMotion(IEnumerable<Motion> seqMotion)
		{
			try
			{
				if (null == seqMotion)
				{
					return null;
				}

				var MousePosition = BotEngine.Windows.Extension.User32GetCursorPos() ?? new Vektor2DInt(0, 0);

				var InputSimulator = new WindowsInput.InputSimulator();

				foreach (var Motion in seqMotion.WhereNotDefault())
				{
					var MotionMousePosition = Motion?.MousePosition;
					var MotionTextEntry = Motion?.TextEntry;

					if (MotionMousePosition.HasValue || (Motion.WindowToForeground ?? false))
						EnsureWindowIsForeground();

					if (MotionMousePosition.HasValue)
					{
						POINT PositionOnScreen;

						MouseMoveToPointInClientRect(WindowHandle, MotionMousePosition.Value + MouseOffsetStatic, out PositionOnScreen);

						MousePosition = PositionOnScreen.AsVektor2DInt();

						Thread.Sleep(MouseMoveDelay);
					}

					if (0 < Motion?.MouseButtonDown?.Count() || 0 < Motion?.MouseButtonUp?.Count())
					{
						EnsureWindowIsForeground();

						User32MouseEvent(MousePosition, Motion?.MouseButtonDown, Motion?.MouseButtonUp);

						Thread.Sleep(MouseEventDelay);
					}

					Motion?.KeyDown?.ForEach(keyDown =>
					{
						EnsureWindowIsForeground();
						InputSimulator.Keyboard.KeyDown(keyDown);
						Thread.Sleep(KeyboardEventTimeDistanceMilli);
					});

					Motion?.KeyUp?.ForEach(keyUp =>
					{
						EnsureWindowIsForeground();
						InputSimulator.Keyboard.KeyUp(keyUp);
						Thread.Sleep(KeyboardEventTimeDistanceMilli);
					});

					if (0 < MotionTextEntry?.Length)
					{
						EnsureWindowIsForeground();
						InputSimulator.Keyboard.TextEntry(MotionTextEntry);
					}
				}

				return new MotionResult(true);
			}
			catch (Exception Exception)
			{
				return new MotionResult(Exception);
			}
		}
	}
}
