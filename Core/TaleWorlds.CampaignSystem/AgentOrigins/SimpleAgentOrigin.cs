using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.AgentOrigins
{
	// Token: 0x02000421 RID: 1057
	public class SimpleAgentOrigin : IAgentOriginBase
	{
		// Token: 0x17000D45 RID: 3397
		// (get) Token: 0x06003E6E RID: 15982 RVA: 0x0012A0FF File Offset: 0x001282FF
		public BasicCharacterObject Troop
		{
			get
			{
				return this._troop;
			}
		}

		// Token: 0x17000D46 RID: 3398
		// (get) Token: 0x06003E6F RID: 15983 RVA: 0x0012A108 File Offset: 0x00128308
		public bool IsUnderPlayersCommand
		{
			get
			{
				PartyBase party = this.Party;
				return party != null && (party == PartyBase.MainParty || party.Owner == Hero.MainHero || party.MapFaction.Leader == Hero.MainHero);
			}
		}

		// Token: 0x17000D47 RID: 3399
		// (get) Token: 0x06003E70 RID: 15984 RVA: 0x0012A14A File Offset: 0x0012834A
		public uint FactionColor
		{
			get
			{
				if (this.Party != null)
				{
					return this.Party.MapFaction.Color;
				}
				if (this._troop.IsHero)
				{
					return this._troop.HeroObject.MapFaction.Color;
				}
				return 0U;
			}
		}

		// Token: 0x17000D48 RID: 3400
		// (get) Token: 0x06003E71 RID: 15985 RVA: 0x0012A189 File Offset: 0x00128389
		public uint FactionColor2
		{
			get
			{
				if (this.Party != null)
				{
					return this.Party.MapFaction.Color2;
				}
				if (this._troop.IsHero)
				{
					return this._troop.HeroObject.MapFaction.Color2;
				}
				return 0U;
			}
		}

		// Token: 0x17000D49 RID: 3401
		// (get) Token: 0x06003E72 RID: 15986 RVA: 0x0012A1C8 File Offset: 0x001283C8
		public int Seed
		{
			get
			{
				if (this.Party != null)
				{
					return CharacterHelper.GetPartyMemberFaceSeed(this.Party, this._troop, this.Rank);
				}
				return CharacterHelper.GetDefaultFaceSeed(this._troop, this.Rank);
			}
		}

		// Token: 0x17000D4A RID: 3402
		// (get) Token: 0x06003E73 RID: 15987 RVA: 0x0012A1FB File Offset: 0x001283FB
		public PartyBase Party
		{
			get
			{
				if (!this._troop.IsHero || this._troop.HeroObject.PartyBelongedTo == null)
				{
					return null;
				}
				return this._troop.HeroObject.PartyBelongedTo.Party;
			}
		}

		// Token: 0x17000D4B RID: 3403
		// (get) Token: 0x06003E74 RID: 15988 RVA: 0x0012A233 File Offset: 0x00128433
		public IBattleCombatant BattleCombatant
		{
			get
			{
				return this.Party;
			}
		}

		// Token: 0x17000D4C RID: 3404
		// (get) Token: 0x06003E75 RID: 15989 RVA: 0x0012A23B File Offset: 0x0012843B
		public Banner Banner
		{
			get
			{
				return this._banner;
			}
		}

		// Token: 0x17000D4D RID: 3405
		// (get) Token: 0x06003E76 RID: 15990 RVA: 0x0012A243 File Offset: 0x00128443
		// (set) Token: 0x06003E77 RID: 15991 RVA: 0x0012A24B File Offset: 0x0012844B
		public int Rank { get; private set; }

		// Token: 0x17000D4E RID: 3406
		// (get) Token: 0x06003E78 RID: 15992 RVA: 0x0012A254 File Offset: 0x00128454
		public int UniqueSeed
		{
			get
			{
				return this._descriptor.UniqueSeed;
			}
		}

		// Token: 0x06003E79 RID: 15993 RVA: 0x0012A261 File Offset: 0x00128461
		public SimpleAgentOrigin(BasicCharacterObject troop, int rank = -1, Banner banner = null, UniqueTroopDescriptor descriptor = default(UniqueTroopDescriptor))
		{
			this._troop = (CharacterObject)troop;
			this._descriptor = descriptor;
			this.Rank = ((rank == -1) ? MBRandom.RandomInt(10000) : rank);
			this._banner = banner;
		}

		// Token: 0x06003E7A RID: 15994 RVA: 0x0012A29B File Offset: 0x0012849B
		public void SetWounded()
		{
		}

		// Token: 0x06003E7B RID: 15995 RVA: 0x0012A29D File Offset: 0x0012849D
		public void SetKilled()
		{
			if (!MBNetwork.IsClient && this._troop.IsHero)
			{
				KillCharacterAction.ApplyByBattle(this._troop.HeroObject, null, true);
			}
		}

		// Token: 0x06003E7C RID: 15996 RVA: 0x0012A2C5 File Offset: 0x001284C5
		public void SetRouted()
		{
		}

		// Token: 0x06003E7D RID: 15997 RVA: 0x0012A2C7 File Offset: 0x001284C7
		public void OnAgentRemoved(float agentHealth)
		{
		}

		// Token: 0x06003E7E RID: 15998 RVA: 0x0012A2CC File Offset: 0x001284CC
		void IAgentOriginBase.OnScoreHit(BasicCharacterObject victim, BasicCharacterObject captain, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
			if (isTeamKill)
			{
				CharacterObject troop = this._troop;
				int num;
				Campaign.Current.Models.CombatXpModel.GetXpFromHit(troop, (CharacterObject)captain, (CharacterObject)victim, this.Party, damage, isFatal, CombatXpModel.MissionTypeEnum.Battle, out num);
				if (troop.IsHero && attackerWeapon != null)
				{
					SkillObject skillForWeapon = Campaign.Current.Models.CombatXpModel.GetSkillForWeapon(attackerWeapon, false);
					troop.HeroObject.AddSkillXp(skillForWeapon, (float)num);
				}
			}
		}

		// Token: 0x06003E7F RID: 15999 RVA: 0x0012A343 File Offset: 0x00128543
		public void SetBanner(Banner banner)
		{
			this._banner = banner;
		}

		// Token: 0x040012B7 RID: 4791
		private CharacterObject _troop;

		// Token: 0x040012B8 RID: 4792
		private Banner _banner;

		// Token: 0x040012BA RID: 4794
		private UniqueTroopDescriptor _descriptor;
	}
}
