using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200021E RID: 542
	public class PollHotkeyCategory : GameKeyContext
	{
		// Token: 0x06001DEE RID: 7662 RVA: 0x0006C849 File Offset: 0x0006AA49
		public PollHotkeyCategory()
			: base("PollHotkeyCategory", 108, GameKeyContext.GameKeyContextType.Default)
		{
			this.RegisterGameKeys();
		}

		// Token: 0x06001DEF RID: 7663 RVA: 0x0006C860 File Offset: 0x0006AA60
		private void RegisterGameKeys()
		{
			base.RegisterGameKey(new GameKey(106, "AcceptPoll", "PollHotkeyCategory", InputKey.F10, InputKey.ControllerLBumper, GameKeyMainCategories.PollCategory), true);
			base.RegisterGameKey(new GameKey(107, "DeclinePoll", "PollHotkeyCategory", InputKey.F11, InputKey.ControllerRBumper, GameKeyMainCategories.PollCategory), true);
		}

		// Token: 0x04000AFC RID: 2812
		public const string CategoryId = "PollHotkeyCategory";

		// Token: 0x04000AFD RID: 2813
		public const int AcceptPoll = 106;

		// Token: 0x04000AFE RID: 2814
		public const int DeclinePoll = 107;
	}
}
