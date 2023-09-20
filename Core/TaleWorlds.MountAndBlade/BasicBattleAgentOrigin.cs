using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000F0 RID: 240
	public class BasicBattleAgentOrigin : IAgentOriginBase
	{
		// Token: 0x170002EF RID: 751
		// (get) Token: 0x06000BE0 RID: 3040 RVA: 0x00016138 File Offset: 0x00014338
		bool IAgentOriginBase.IsUnderPlayersCommand
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x06000BE1 RID: 3041 RVA: 0x0001613B File Offset: 0x0001433B
		uint IAgentOriginBase.FactionColor
		{
			get
			{
				return 0U;
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06000BE2 RID: 3042 RVA: 0x0001613E File Offset: 0x0001433E
		uint IAgentOriginBase.FactionColor2
		{
			get
			{
				return 0U;
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06000BE3 RID: 3043 RVA: 0x00016141 File Offset: 0x00014341
		IBattleCombatant IAgentOriginBase.BattleCombatant
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06000BE4 RID: 3044 RVA: 0x00016144 File Offset: 0x00014344
		int IAgentOriginBase.UniqueSeed
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x06000BE5 RID: 3045 RVA: 0x00016147 File Offset: 0x00014347
		int IAgentOriginBase.Seed
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x0001614A File Offset: 0x0001434A
		Banner IAgentOriginBase.Banner
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06000BE7 RID: 3047 RVA: 0x0001614D File Offset: 0x0001434D
		BasicCharacterObject IAgentOriginBase.Troop
		{
			get
			{
				return this._troop;
			}
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x00016155 File Offset: 0x00014355
		public BasicBattleAgentOrigin(BasicCharacterObject troop)
		{
			this._troop = troop;
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x00016164 File Offset: 0x00014364
		void IAgentOriginBase.SetWounded()
		{
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x00016166 File Offset: 0x00014366
		void IAgentOriginBase.SetKilled()
		{
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x00016168 File Offset: 0x00014368
		void IAgentOriginBase.SetRouted()
		{
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x0001616A File Offset: 0x0001436A
		void IAgentOriginBase.OnAgentRemoved(float agentHealth)
		{
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x0001616C File Offset: 0x0001436C
		void IAgentOriginBase.OnScoreHit(BasicCharacterObject victim, BasicCharacterObject captain, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x0001616E File Offset: 0x0001436E
		void IAgentOriginBase.SetBanner(Banner banner)
		{
		}

		// Token: 0x04000291 RID: 657
		private BasicCharacterObject _troop;
	}
}
