using System;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers
{
	// Token: 0x020003F9 RID: 1017
	public interface IBoardGameHandler
	{
		// Token: 0x060034EE RID: 13550
		void SwitchTurns();

		// Token: 0x060034EF RID: 13551
		void DiceRoll(int roll);

		// Token: 0x060034F0 RID: 13552
		void Install();

		// Token: 0x060034F1 RID: 13553
		void Uninstall();

		// Token: 0x060034F2 RID: 13554
		void Activate();
	}
}
