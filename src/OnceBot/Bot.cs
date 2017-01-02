using Bib3;
using Bib3.Geometrik;
using BotEngine.Common;
using BotEngine.Common.Motor;
using BotEngine.Motor;
using OnceBot.ImageProcessing;
using OnceBot.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace OnceBot
{
	static public class MotorExtension
	{
		static public void MouseMoveLinearContinuous(
			this IMotor motor,
			Vektor2DInt origin,
			Vektor2DInt destiniation)
		{
			var Distance = destiniation - origin;
			var DistanceLength = Distance.Length();

			for (int i = 0; i < DistanceLength; i++)
			{
				var interpolated = origin + (Distance * i) / DistanceLength;
				motor?.MouseMove(interpolated);
			}

			motor?.MouseMove(destiniation);
		}
	}

	public class BotStepResult
	{
		public DateTime StartTime;

		public DateTime EndTime;

		public Exception Exception;

		public string UIMessageText;

		public KeyValuePair<UInt32[], int> WindowClientRaster;

		public ImagePatternMatch[] RasterSetPatternMatch;

		public ImagePatternMatch[] RasterSetPatterMatchTriggered;

		public IconHandling[] SetIconCandidate;

		public bool MotionEnabled;
	}

	public class IconHandling
	{
		public string PatternId;

		public Vektor2DInt Location;

		public double? LastVisitAge;
	}

	public class Bot
	{
		Vektor2DInt MouseRestLocation = new Vektor2DInt(16, 100);

		Vektor2DInt FromProductIconToBuildingOffset = new Vektor2DInt(0, 55);

		const int CollectDragDistance = 44;

		public const int SecurityMarginThickness = 55;

		public const int SecurityMarginThicknessTop = 90;

		int VisitDistanceMin = 300;

		readonly object Lock = new object();

		public IntPtr? WindowHandle;

		readonly Queue<KeyValuePair<IconHandling, DateTime>> ListIconVisited = new Queue<KeyValuePair<IconHandling, DateTime>>();

		public BotStepResult StepLast
		{
			private set;
			get;
		}

		public BotStepResult MotionEnabledStepLast
		{
			private set;
			get;
		}

		static public bool DistanceInBoundProductIconAnim(Vektor2DInt iconDistance) =>
			Math.Abs(iconDistance.A) < 4 && Math.Abs(iconDistance.B) < 30;

		public BotStepResult Step(
			bool motionEnabled,
			Func<Request, Response> service)
		{
			lock (Lock)
			{
				var StartTime = DateTime.Now;

				Exception Exception = null;
				string UIMessageText = null;
				KeyValuePair<UInt32[], int> windowClientRaster = default(KeyValuePair<UInt32[], int>);
				ImagePatternMatch[] RasterSetPatternMatch = null;
				ImagePatternMatch[] RasterSetPatterMatchTriggered = null;
				IconHandling[] SetIconCandidate = null;

				var ImageSearchDelegate = new Func<KeyValuePair<UInt32[], int>, IEnumerable<ImagePatternMatch>>(raster =>
					service?.Invoke(new Request
					{
						ImagePatternMatchSearch = new ImagePatternMatchSearchRequest
						{
							Raster = raster,
						},
					})?.ImagePatternMatchSearch?.SetMatch);

				try
				{
					var WindowHandle = this.WindowHandle;

					if (!WindowHandle.HasValue)
						throw new ArgumentException("no window selected");

					BotEngine.Windows.RECT WindowClientRect;

					BotEngine.WinApi.User32.GetClientRect(WindowHandle.Value, out WindowClientRect);

					var WindowClientSize = new Vektor2DInt(WindowClientRect.Width, WindowClientRect.Height);

					var Motor = new Motor.WindowMotor(WindowHandle.Value);

					BotEngine.WinApi.User32.SetForegroundWindow(WindowHandle.Value);

					windowClientRaster = WinApi.Raster32BitFromClientRectFromWindowMitHandleOverDesktop(WindowHandle.Value);

					if (windowClientRaster.Value < 1)
						throw new Exception("failed to take image of window.");

					RasterSetPatternMatch =
						ImageSearchDelegate?.Invoke(windowClientRaster)?.ToArray();

					var MouseMoveToRest = new Action(() =>
					{
						UIMessageText += "move mouse to rest location." + Environment.NewLine;
						Motor.MouseMoveLinearContinuous(MouseRestLocation + new Vektor2DInt(44, 44), MouseRestLocation);
					});

					var subsetMatchWithPatternIdContaining = new Func<string, IEnumerable<ImagePatternMatch>>(idPortion =>
						RasterSetPatternMatch?.Where(match => match?.SourcePatternId?.RegexMatchSuccessIgnoreCase(Regex.Escape(idPortion)) ?? false));

					var hourglassMatch =
						subsetMatchWithPatternIdContaining(PatternIdConfig.Hourglass)?.ToArray();

					var closeButtonMatch =
						subsetMatchWithPatternIdContaining("close.button")?.FirstOrDefault();

					var progressBarMatch =
						subsetMatchWithPatternIdContaining("progress")?.FirstOrDefault();

					var clickToCollectMatch =
						subsetMatchWithPatternIdContaining("collect")?.FirstOrDefault();

					var IdleIconMatch =
						subsetMatchWithPatternIdContaining(PatternIdConfig.Idle);

					SetIconCandidate = new[]
						{
							subsetMatchWithPatternIdContaining(PatternIdConfig.Coin),
							subsetMatchWithPatternIdContaining(PatternIdConfig.WorkshopProductReady),
							subsetMatchWithPatternIdContaining(PatternIdConfig.Idle),
						}
						.ConcatNullable()
						?.WhereNotDefault()
						?.Select(match => new IconHandling
						{
							PatternId = match.SourcePatternId,
							Location = match.Area.Center(),
						})
						?.Where(icon =>
						SecurityMarginThickness < icon.Location.A && Math.Max(SecurityMarginThicknessTop, SecurityMarginThickness) < icon.Location.B &&
						icon.Location.A + SecurityMarginThickness < WindowClientSize.A && icon.Location.B + SecurityMarginThickness < WindowClientSize.B)
						?.ToArray();

					foreach (var Icon in SetIconCandidate.EmptyIfNull())
					{
						var lastVisit =
							ListIconVisited?.CastToNullable()?.LastOrDefault(visit =>
							visit.Value.Key.PatternId == Icon.PatternId &&
							DistanceInBoundProductIconAnim(visit.Value.Key.Location - Icon.Location));

						Icon.LastVisitAge = (DateTime.Now - lastVisit?.Value)?.TotalSeconds;
					}

					var ListToVisit =
						SetIconCandidate

						//	only consider icons which were already present in last step to prevent distraction by temporary floating icons triggered by collection.
						?.Where(icon =>
						StepLast?.SetIconCandidate?.Any(lastStepIcon => (lastStepIcon.Location - icon.Location).Length() < 3) ?? false)

						?.OrderByDescending(t => t.LastVisitAge ?? int.MaxValue)
						?.ThenBy(t => t.Location.B)
						?.ToArray();

					var IconToVisitNext = ListToVisit?.FirstOrDefault();

					//	production button contains a hourglass icon.
					//	we want the shortest time and assume that is the one closest to the upper left.
					var productionPreferredButtonLocation =
						hourglassMatch
						?.OrderBy(match => match.Area.Center().Length())
						?.FirstOrDefault();

					//	Baracks dialog has an hourglass too.
					var DialogIsProduction = 3 < hourglassMatch?.Length;

					UIMessageText += "next icon to visit: " +
						((null == IconToVisitNext) ? "null" :
						("'" + IconToVisitNext?.PatternId + "'" + " at " + IconToVisitNext.Location.RenderForUI() + " (last visited " + (((int?)IconToVisitNext.LastVisitAge)?.ToString() ?? "????") + "s ago)"))
						+ Environment.NewLine;

					if (motionEnabled)
					{
						if (null != closeButtonMatch)
						{
							if (null != productionPreferredButtonLocation && DialogIsProduction)
							{
								UIMessageText += "click on production start button.";
								Motor.MouseClickLeft(productionPreferredButtonLocation.Area.Center());
								goto StepEnd;
							}

							UIMessageText += "click on dialog close button.";
							Motor.MouseClickLeft((closeButtonMatch?.Area.Center()).Value);
							goto StepEnd;
						}

						if (null != progressBarMatch || null != clickToCollectMatch)
						{
							UIMessageText += "tooltip detected, move mouse to rest location.";
							MouseMoveToRest();
							goto StepEnd;
						}

						if (null != IconToVisitNext &&
							!(IconToVisitNext?.LastVisitAge < VisitDistanceMin))
						{
							var BuildingProductIconReadyLocation = IconToVisitNext.Location;

							var IconIdMessage = "under icon of type " + IconToVisitNext?.PatternId + " at " + BuildingProductIconReadyLocation.RenderForUI();

							UIMessageText += IconIdMessage + ":" + Environment.NewLine;

							var mouseDestination = BuildingProductIconReadyLocation + FromProductIconToBuildingOffset;

							ListIconVisited.Enqueue(new KeyValuePair<IconHandling, DateTime>(IconToVisitNext, DateTime.Now));
							ListIconVisited.ListeKürzeBegin(400);

							BotEngine.WinApi.User32.SetForegroundWindow(WindowHandle.Value);

							if (IconToVisitNext.PatternId == PatternIdConfig.Idle)
							{
								UIMessageText += "open dialog at " + mouseDestination.RenderForUI() + "." + Environment.NewLine;

								Motor.MouseClick(mouseDestination, MouseButtonIdEnum.Left);
							}
							else
							{
								UIMessageText += "collect at " + mouseDestination.RenderForUI() + "." + Environment.NewLine;

								var PathStretch = new Vektor2DInt(CollectDragDistance, 0).RotatedByMikro((int)new Random((int)Bib3.Glob.StopwatchZaitMiliSictInt()).Next().SictUmgebrocen(0, 1000) * 1000);

								var collectLocationDistant = mouseDestination + PathStretch;

								var middle = (mouseDestination + collectLocationDistant) / 2;

								var listWaypoint = Extension.ListPointInterpolated(mouseDestination, collectLocationDistant, (int)Extension.RandomInt().SictUmgebrocen(7, 16)).ToArray();

								listWaypoint =
									listWaypoint.Concat(listWaypoint.Reversed()).ToArray();

								var LocationInScreenFromLocationInWindow = new Func<Vektor2DInt, Vektor2DInt>(windowClient =>
							   {
								   var PointInScreen = new BotEngine.Windows.POINT { x = (int)windowClient.A, y = (int)windowClient.B };

								   BotEngine.WinApi.User32.ClientToScreen(Motor.WindowHandle, ref PointInScreen);

								   return new Vektor2DInt(PointInScreen.x, PointInScreen.y);
							   });

								var simulator = new WindowsInput.InputSimulator();

								simulator.Mouse.LeftButtonUp();

								bool mouseButtonDown = false;

								foreach (var waypointInWindow in listWaypoint)
								{
									var locationOnScreen = LocationInScreenFromLocationInWindow(waypointInWindow);

									Thread.Sleep((int)Extension.RandomInt().SictUmgebrocen(0, 33));
									BotEngine.WinApi.User32.SetCursorPos((int)locationOnScreen.A, (int)locationOnScreen.B);

									if (!mouseButtonDown)
									{
										mouseButtonDown = true;
										simulator.Mouse.LeftButtonDown();
									}
								}

								simulator.Mouse.LeftButtonUp();
								Thread.Sleep(111);
							}

							MouseMoveToRest();
							goto StepEnd;
						}
					}

					StepEnd:;
				}
				catch (Exception tException)
				{
					Exception = tException;
				}

				var StepResult = new BotStepResult
				{
					StartTime = StartTime,
					Exception = Exception,
					UIMessageText = UIMessageText + " " + Bib3.Glob.SictString(Exception, true),
					WindowClientRaster = windowClientRaster,
					RasterSetPatternMatch = RasterSetPatternMatch,
					RasterSetPatterMatchTriggered = RasterSetPatterMatchTriggered,
					SetIconCandidate = SetIconCandidate,
					MotionEnabled = motionEnabled,
					EndTime = DateTime.Now,
				};

				StepLast = StepResult;

				if (motionEnabled)
					MotionEnabledStepLast = StepResult;

				return StepResult;
			}
		}
	}
}
