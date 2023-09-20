using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TroopSuppliers;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.AgentOrigins
{
	// Token: 0x02000420 RID: 1056
	public class PartyGroupAgentOrigin : IAgentOriginBase
	{
		// Token: 0x06003E5A RID: 15962 RVA: 0x00129E3C File Offset: 0x0012803C
		internal PartyGroupAgentOrigin(PartyGroupTroopSupplier supplier, UniqueTroopDescriptor descriptor, int rank)
		{
			this._supplier = supplier;
			this._descriptor = descriptor;
			this._rank = rank;
		}

		// Token: 0x17000D39 RID: 3385
		// (get) Token: 0x06003E5B RID: 15963 RVA: 0x00129E59 File Offset: 0x00128059
		public PartyBase Party
		{
			get
			{
				return this._supplier.GetParty(this._descriptor);
			}
		}

		// Token: 0x17000D3A RID: 3386
		// (get) Token: 0x06003E5C RID: 15964 RVA: 0x00129E6C File Offset: 0x0012806C
		public IBattleCombatant BattleCombatant
		{
			get
			{
				return this.Party;
			}
		}

		// Token: 0x17000D3B RID: 3387
		// (get) Token: 0x06003E5D RID: 15965 RVA: 0x00129E74 File Offset: 0x00128074
		public Banner Banner
		{
			get
			{
				if (this.Party.LeaderHero == null)
				{
					return this.Party.MapFaction.Banner;
				}
				return this.Party.LeaderHero.ClanBanner;
			}
		}

		// Token: 0x17000D3C RID: 3388
		// (get) Token: 0x06003E5E RID: 15966 RVA: 0x00129EA4 File Offset: 0x001280A4
		public int UniqueSeed
		{
			get
			{
				return this._descriptor.UniqueSeed;
			}
		}

		// Token: 0x17000D3D RID: 3389
		// (get) Token: 0x06003E5F RID: 15967 RVA: 0x00129EBF File Offset: 0x001280BF
		public CharacterObject Troop
		{
			get
			{
				return this._supplier.GetTroop(this._descriptor);
			}
		}

		// Token: 0x17000D3E RID: 3390
		// (get) Token: 0x06003E60 RID: 15968 RVA: 0x00129ED2 File Offset: 0x001280D2
		BasicCharacterObject IAgentOriginBase.Troop
		{
			get
			{
				return this.Troop;
			}
		}

		// Token: 0x17000D3F RID: 3391
		// (get) Token: 0x06003E61 RID: 15969 RVA: 0x00129EDA File Offset: 0x001280DA
		public UniqueTroopDescriptor TroopDesc
		{
			get
			{
				return this._descriptor;
			}
		}

		// Token: 0x17000D40 RID: 3392
		// (get) Token: 0x06003E62 RID: 15970 RVA: 0x00129EE2 File Offset: 0x001280E2
		public int Rank
		{
			get
			{
				return this._rank;
			}
		}

		// Token: 0x17000D41 RID: 3393
		// (get) Token: 0x06003E63 RID: 15971 RVA: 0x00129EEA File Offset: 0x001280EA
		public bool IsUnderPlayersCommand
		{
			get
			{
				return this.Troop == Hero.MainHero.CharacterObject || PartyGroupAgentOrigin.IsPartyUnderPlayerCommand(this.Party);
			}
		}

		// Token: 0x17000D42 RID: 3394
		// (get) Token: 0x06003E64 RID: 15972 RVA: 0x00129F0B File Offset: 0x0012810B
		public uint FactionColor
		{
			get
			{
				return this.Party.MapFaction.Color;
			}
		}

		// Token: 0x17000D43 RID: 3395
		// (get) Token: 0x06003E65 RID: 15973 RVA: 0x00129F1D File Offset: 0x0012811D
		public uint FactionColor2
		{
			get
			{
				return this.Party.MapFaction.Color2;
			}
		}

		// Token: 0x17000D44 RID: 3396
		// (get) Token: 0x06003E66 RID: 15974 RVA: 0x00129F2F File Offset: 0x0012812F
		public int Seed
		{
			get
			{
				return CharacterHelper.GetPartyMemberFaceSeed(this.Party, this.Troop, this.Rank);
			}
		}

		// Token: 0x06003E67 RID: 15975 RVA: 0x00129F48 File Offset: 0x00128148
		public void SetWounded()
		{
			if (!this._isRemoved)
			{
				this._supplier.OnTroopWounded(this._descriptor);
				this._isRemoved = true;
			}
		}

		// Token: 0x06003E68 RID: 15976 RVA: 0x00129F6C File Offset: 0x0012816C
		public void SetKilled()
		{
			if (!this._isRemoved)
			{
				this._supplier.OnTroopKilled(this._descriptor);
				if (this.Troop.IsHero)
				{
					KillCharacterAction.ApplyByBattle(this.Troop.HeroObject, null, true);
				}
				this._isRemoved = true;
			}
		}

		// Token: 0x06003E69 RID: 15977 RVA: 0x00129FB8 File Offset: 0x001281B8
		public void SetRouted()
		{
			if (!this._isRemoved)
			{
				this._supplier.OnTroopRouted(this._descriptor);
				this._isRemoved = true;
			}
		}

		// Token: 0x06003E6A RID: 15978 RVA: 0x00129FDA File Offset: 0x001281DA
		public void OnAgentRemoved(float agentHealth)
		{
			if (this.Troop.IsHero)
			{
				this.Troop.HeroObject.HitPoints = MathF.Max(1, MathF.Round(agentHealth));
			}
		}

		// Token: 0x06003E6B RID: 15979 RVA: 0x0012A005 File Offset: 0x00128205
		void IAgentOriginBase.OnScoreHit(BasicCharacterObject victim, BasicCharacterObject captain, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
			this._supplier.OnTroopScoreHit(this._descriptor, victim, damage, isFatal, isTeamKill, attackerWeapon);
		}

		// Token: 0x06003E6C RID: 15980 RVA: 0x0012A020 File Offset: 0x00128220
		public void SetBanner(Banner banner)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06003E6D RID: 15981 RVA: 0x0012A028 File Offset: 0x00128228
		public static bool IsPartyUnderPlayerCommand(PartyBase party)
		{
			if (party == PartyBase.MainParty)
			{
				return true;
			}
			if (party.Side != PartyBase.MainParty.Side)
			{
				return false;
			}
			bool flag = party.Owner == Hero.MainHero;
			bool flag2 = party.MapFaction.Leader == Hero.MainHero;
			bool flag3 = party.MobileParty != null && party.MobileParty.DefaultBehavior == AiBehavior.EscortParty && party.MobileParty.TargetParty == MobileParty.MainParty;
			bool flag4 = party.MobileParty != null && party.MobileParty.Army != null && party.MobileParty.Army.LeaderParty == MobileParty.MainParty;
			Settlement mapEventSettlement = party.MapEvent.MapEventSettlement;
			bool flag5 = mapEventSettlement != null && mapEventSettlement.OwnerClan.Leader == Hero.MainHero;
			return flag || flag2 || flag3 || flag4 || flag5;
		}

		// Token: 0x040012B3 RID: 4787
		private readonly PartyGroupTroopSupplier _supplier;

		// Token: 0x040012B4 RID: 4788
		private readonly UniqueTroopDescriptor _descriptor;

		// Token: 0x040012B5 RID: 4789
		private readonly int _rank;

		// Token: 0x040012B6 RID: 4790
		private bool _isRemoved;
	}
}
