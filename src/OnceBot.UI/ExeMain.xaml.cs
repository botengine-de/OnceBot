using Bib3;
using Bib3.Synchronization;
using BotEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace OnceBot.UI
{
	/// <summary>
	/// Interaction logic for ExeMain.xaml
	/// </summary>
	public partial class ExeMain : UserControl
	{
		readonly object TimerLock = new object();

		static public string AssemblyDirectoryPath => Bib3.FCL.Glob.ZuProcessSelbsctMainModuleDirectoryPfaadBerecne().EnsureEndsWith(@"\");

		readonly OnceBot.Bot Bot = new OnceBot.Bot();

		DispatcherTimer Timer;

		int BotStepDistance = 1444;
		Int64 BotStepLastTime = 0;

		public bool BotMotionEnable
		{
			set
			{
				if (value)
					ToggleButtonMotionEnable?.RightButtonDown();
				else
					ToggleButtonMotionEnable?.LeftButtonDown();
			}

			get
			{
				return ToggleButtonMotionEnable?.ButtonReczIsChecked ?? false;
			}
		}

		public ExeMain()
		{
			InitializeComponent();

			AddHandler(Button.ClickEvent, (RoutedEventHandler)ButtonClickEventHandler);

			Timer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Normal, new EventHandler(TimerElapsed), Dispatcher);
		}

		void ButtonClickEventHandler(object sender, RoutedEventArgs e)
		{
			if (e.OriginalSource == BotControl.EnableDebugToolButton)
				DebugTab.Visibility = Visibility.Visible;
		}

		public IEnumerable<IEnumerable<Key>> SetKeyBotMotionDisable()
		{
			yield return new[] { Key.LeftCtrl, Key.LeftAlt };
			yield return new[] { Key.RightCtrl, Key.RightAlt };
		}

		void TimerElapsed(object sender, EventArgs e)
		{
			ProcessInput();

			var BotStepLastAge = Bib3.Glob.StopwatchZaitMiliSictInt() - BotStepLastTime;

			if (!(BotStepLastAge < BotStepDistance))
				BotStep();

			PickWindowControl?.PickLastUpdate();
		}

		bool TakeScreenshotWhenPaused;

		public void ProcessInput()
		{
			if (SetKeyBotMotionDisable()?.Any(setKey => setKey?.All(key => Keyboard.IsKeyDown(key)) ?? false) ?? false)
			{
				BotMotionEnable = false;
			}

			TakeScreenshotWhenPaused = TakeScreenshotWhenPausedCheckBox?.IsChecked ?? false;
		}

		readonly object BotStepLock = new object();

		static public Func<Service.Request, Service.Response> ServiceDelegate;

		public void BotStep()
		{
			BotStepLastTime = Bib3.Glob.StopwatchZaitMiliSictInt();

			var BotMotionEnable = this.BotMotionEnable;

			Bot.WindowHandle = PickWindowControl.PickLastWindowHandle;

			var serviceDelegate = ServiceDelegate;

			if (null == serviceDelegate)
				return;

			if (!(BotMotionEnable || TakeScreenshotWhenPaused))
				return;

			Task.Run(() =>
			{
				BotStepLock.InvokeIfNotLocked(() =>
				{
					Bot.Step(BotMotionEnable, serviceDelegate);

					BotStepLastTime = Bib3.Glob.StopwatchZaitMiliSictInt();

					Dispatcher.Invoke(() =>
					{
						BotControl.Present(Bot);
					});
				});
			});
		}
	}
}
