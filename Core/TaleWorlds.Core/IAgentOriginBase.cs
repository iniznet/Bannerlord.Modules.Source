using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000078 RID: 120
	public interface IAgentOriginBase
	{
		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000773 RID: 1907
		bool IsUnderPlayersCommand { get; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06000774 RID: 1908
		uint FactionColor { get; }

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000775 RID: 1909
		uint FactionColor2 { get; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000776 RID: 1910
		IBattleCombatant BattleCombatant { get; }

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000777 RID: 1911
		int UniqueSeed { get; }

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000778 RID: 1912
		int Seed { get; }

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000779 RID: 1913
		Banner Banner { get; }

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x0600077A RID: 1914
		BasicCharacterObject Troop { get; }

		// Token: 0x0600077B RID: 1915
		void SetWounded();

		// Token: 0x0600077C RID: 1916
		void SetKilled();

		// Token: 0x0600077D RID: 1917
		void SetRouted();

		// Token: 0x0600077E RID: 1918
		void OnAgentRemoved(float agentHealth);

		// Token: 0x0600077F RID: 1919
		void OnScoreHit(BasicCharacterObject victim, BasicCharacterObject formationCaptain, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon);

		// Token: 0x06000780 RID: 1920
		void SetBanner(Banner banner);
	}
}
