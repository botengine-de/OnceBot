using System.Windows.Controls;

namespace OnceBot.UI
{
	/// <summary>
	/// Interaction logic for Bot.xaml
	/// </summary>
	public partial class Bot : UserControl
	{
		public Bot()
		{
			InitializeComponent();
		}

		public void Present(OnceBot.Bot bot)
		{
			StepLastView?.Present(bot?.StepLast);
			StepMotionEnabledLastView?.Present(bot?.MotionEnabledStepLast);
		}
	}
}
