using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	public sealed class PollHotkeyCategory : GameKeyContext
	{
		public PollHotkeyCategory()
			: base("PollHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterGameKeys();
		}

		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(106, "AcceptPoll", "PollHotkeyCategory", InputKey.F10, InputKey.ControllerLBumper, GameKeyMainCategories.PollCategory), true);
			base.RegisterGameKey(new GameKey(107, "DeclinePoll", "PollHotkeyCategory", InputKey.F11, InputKey.ControllerRBumper, GameKeyMainCategories.PollCategory), true);
		}

		public const string CategoryId = "PollHotkeyCategory";

		public const int AcceptPoll = 106;

		public const int DeclinePoll = 107;
	}
}
