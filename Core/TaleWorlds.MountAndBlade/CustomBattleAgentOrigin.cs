using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000F1 RID: 241
	public class CustomBattleAgentOrigin : IAgentOriginBase
	{
		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06000BEF RID: 3055 RVA: 0x00016170 File Offset: 0x00014370
		// (set) Token: 0x06000BF0 RID: 3056 RVA: 0x00016178 File Offset: 0x00014378
		public CustomBattleCombatant CustomBattleCombatant { get; private set; }

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06000BF1 RID: 3057 RVA: 0x00016181 File Offset: 0x00014381
		IBattleCombatant IAgentOriginBase.BattleCombatant
		{
			get
			{
				return this.CustomBattleCombatant;
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06000BF2 RID: 3058 RVA: 0x00016189 File Offset: 0x00014389
		// (set) Token: 0x06000BF3 RID: 3059 RVA: 0x00016191 File Offset: 0x00014391
		public BasicCharacterObject Troop { get; private set; }

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06000BF4 RID: 3060 RVA: 0x0001619A File Offset: 0x0001439A
		// (set) Token: 0x06000BF5 RID: 3061 RVA: 0x000161A2 File Offset: 0x000143A2
		public int Rank { get; private set; }

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06000BF6 RID: 3062 RVA: 0x000161AB File Offset: 0x000143AB
		public Banner Banner
		{
			get
			{
				return this.CustomBattleCombatant.Banner;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06000BF7 RID: 3063 RVA: 0x000161B8 File Offset: 0x000143B8
		public bool IsUnderPlayersCommand
		{
			get
			{
				return this._isPlayerSide;
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06000BF8 RID: 3064 RVA: 0x000161C0 File Offset: 0x000143C0
		public uint FactionColor
		{
			get
			{
				return this.CustomBattleCombatant.BasicCulture.Color;
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06000BF9 RID: 3065 RVA: 0x000161D2 File Offset: 0x000143D2
		public uint FactionColor2
		{
			get
			{
				return this.CustomBattleCombatant.BasicCulture.Color2;
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06000BFA RID: 3066 RVA: 0x000161E4 File Offset: 0x000143E4
		public int Seed
		{
			get
			{
				return this.Troop.GetDefaultFaceSeed(this.Rank);
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06000BFB RID: 3067 RVA: 0x000161F8 File Offset: 0x000143F8
		public int UniqueSeed
		{
			get
			{
				return this._descriptor.UniqueSeed;
			}
		}

		// Token: 0x06000BFC RID: 3068 RVA: 0x00016214 File Offset: 0x00014414
		public CustomBattleAgentOrigin(CustomBattleCombatant customBattleCombatant, BasicCharacterObject characterObject, CustomBattleTroopSupplier troopSupplier, bool isPlayerSide, int rank = -1, UniqueTroopDescriptor uniqueNo = default(UniqueTroopDescriptor))
		{
			this.CustomBattleCombatant = customBattleCombatant;
			this.Troop = characterObject;
			this._descriptor = ((!uniqueNo.IsValid) ? new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed) : uniqueNo);
			this.Rank = ((rank == -1) ? MBRandom.RandomInt(10000) : rank);
			this._troopSupplier = troopSupplier;
			this._isPlayerSide = isPlayerSide;
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x0001627F File Offset: 0x0001447F
		public void SetWounded()
		{
			if (!this._isRemoved)
			{
				this._troopSupplier.OnTroopWounded();
				this._isRemoved = true;
			}
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x0001629B File Offset: 0x0001449B
		public void SetKilled()
		{
			if (!this._isRemoved)
			{
				this._troopSupplier.OnTroopKilled();
				this._isRemoved = true;
			}
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x000162B7 File Offset: 0x000144B7
		public void SetRouted()
		{
			if (!this._isRemoved)
			{
				this._troopSupplier.OnTroopRouted();
				this._isRemoved = true;
			}
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x000162D3 File Offset: 0x000144D3
		public void OnAgentRemoved(float agentHealth)
		{
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x000162D5 File Offset: 0x000144D5
		void IAgentOriginBase.OnScoreHit(BasicCharacterObject victim, BasicCharacterObject captain, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x000162D7 File Offset: 0x000144D7
		public void SetBanner(Banner banner)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000295 RID: 661
		private readonly UniqueTroopDescriptor _descriptor;

		// Token: 0x04000296 RID: 662
		private readonly bool _isPlayerSide;

		// Token: 0x04000297 RID: 663
		private CustomBattleTroopSupplier _troopSupplier;

		// Token: 0x04000298 RID: 664
		private bool _isRemoved;
	}
}
