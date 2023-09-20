using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.AgentOrigins
{
	// Token: 0x0200041F RID: 1055
	public class PartyAgentOrigin : IAgentOriginBase
	{
		// Token: 0x17000D2F RID: 3375
		// (get) Token: 0x06003E47 RID: 15943 RVA: 0x00129AF8 File Offset: 0x00127CF8
		// (set) Token: 0x06003E48 RID: 15944 RVA: 0x00129B59 File Offset: 0x00127D59
		public PartyBase Party
		{
			get
			{
				PartyBase partyBase = this._party;
				if (this._troop.IsHero && this._troop.HeroObject.PartyBelongedTo != null && this._troop.HeroObject.PartyBelongedTo.Party != null)
				{
					partyBase = this._troop.HeroObject.PartyBelongedTo.Party;
				}
				return partyBase;
			}
			set
			{
				this._party = value;
			}
		}

		// Token: 0x17000D30 RID: 3376
		// (get) Token: 0x06003E49 RID: 15945 RVA: 0x00129B62 File Offset: 0x00127D62
		public IBattleCombatant BattleCombatant
		{
			get
			{
				return this.Party;
			}
		}

		// Token: 0x17000D31 RID: 3377
		// (get) Token: 0x06003E4A RID: 15946 RVA: 0x00129B6C File Offset: 0x00127D6C
		public Banner Banner
		{
			get
			{
				if (this.Party == null)
				{
					if (!this._troop.IsHero)
					{
						return null;
					}
					return this._troop.HeroObject.MapFaction.Banner;
				}
				else
				{
					if (this.Party.LeaderHero == null)
					{
						return this.Party.MapFaction.Banner;
					}
					return this.Party.LeaderHero.ClanBanner;
				}
			}
		}

		// Token: 0x17000D32 RID: 3378
		// (get) Token: 0x06003E4B RID: 15947 RVA: 0x00129BD4 File Offset: 0x00127DD4
		public BasicCharacterObject Troop
		{
			get
			{
				return this._troop;
			}
		}

		// Token: 0x17000D33 RID: 3379
		// (get) Token: 0x06003E4C RID: 15948 RVA: 0x00129BDC File Offset: 0x00127DDC
		// (set) Token: 0x06003E4D RID: 15949 RVA: 0x00129BE4 File Offset: 0x00127DE4
		public int Rank { get; private set; }

		// Token: 0x17000D34 RID: 3380
		// (get) Token: 0x06003E4E RID: 15950 RVA: 0x00129BF0 File Offset: 0x00127DF0
		public bool IsUnderPlayersCommand
		{
			get
			{
				PartyBase party = this.Party;
				return (party != null && party == PartyBase.MainParty) || party.Owner == Hero.MainHero || party.MapFaction.Leader == Hero.MainHero;
			}
		}

		// Token: 0x17000D35 RID: 3381
		// (get) Token: 0x06003E4F RID: 15951 RVA: 0x00129C30 File Offset: 0x00127E30
		public uint FactionColor
		{
			get
			{
				if (this.Party == null)
				{
					return this._troop.HeroObject.MapFaction.Color;
				}
				return this.Party.MapFaction.Color2;
			}
		}

		// Token: 0x17000D36 RID: 3382
		// (get) Token: 0x06003E50 RID: 15952 RVA: 0x00129C60 File Offset: 0x00127E60
		public uint FactionColor2
		{
			get
			{
				if (this.Party == null)
				{
					return this._troop.HeroObject.MapFaction.Color2;
				}
				return this.Party.MapFaction.Color2;
			}
		}

		// Token: 0x17000D37 RID: 3383
		// (get) Token: 0x06003E51 RID: 15953 RVA: 0x00129C90 File Offset: 0x00127E90
		public int Seed
		{
			get
			{
				if (this.Party == null)
				{
					return 0;
				}
				return CharacterHelper.GetPartyMemberFaceSeed(this.Party, this._troop, this.Rank);
			}
		}

		// Token: 0x17000D38 RID: 3384
		// (get) Token: 0x06003E52 RID: 15954 RVA: 0x00129CB4 File Offset: 0x00127EB4
		public int UniqueSeed
		{
			get
			{
				return this._descriptor.UniqueSeed;
			}
		}

		// Token: 0x06003E53 RID: 15955 RVA: 0x00129CD0 File Offset: 0x00127ED0
		public PartyAgentOrigin(PartyBase partyBase, CharacterObject characterObject, int rank = -1, UniqueTroopDescriptor uniqueNo = default(UniqueTroopDescriptor), bool alwaysWounded = false)
		{
			this.Party = partyBase;
			this._troop = characterObject;
			this._descriptor = ((!uniqueNo.IsValid) ? new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed) : uniqueNo);
			this.Rank = ((rank == -1) ? MBRandom.RandomInt(10000) : rank);
			this._alwaysWounded = alwaysWounded;
		}

		// Token: 0x06003E54 RID: 15956 RVA: 0x00129D34 File Offset: 0x00127F34
		public void SetWounded()
		{
			if (this._troop.IsHero)
			{
				this._troop.HeroObject.MakeWounded(null, KillCharacterAction.KillCharacterActionDetail.None);
			}
			if (this.Party != null)
			{
				this.Party.MemberRoster.AddToCounts(this._troop, 0, false, 1, 0, true, -1);
			}
		}

		// Token: 0x06003E55 RID: 15957 RVA: 0x00129D88 File Offset: 0x00127F88
		public void SetKilled()
		{
			if (this._alwaysWounded)
			{
				this.SetWounded();
				return;
			}
			if (this._troop.IsHero)
			{
				KillCharacterAction.ApplyByBattle(this._troop.HeroObject, null, true);
				return;
			}
			if (!this._troop.IsHero)
			{
				PartyBase party = this.Party;
				if (party == null)
				{
					return;
				}
				party.MemberRoster.AddToCounts(this._troop, -1, false, 0, 0, true, -1);
			}
		}

		// Token: 0x06003E56 RID: 15958 RVA: 0x00129DF3 File Offset: 0x00127FF3
		public void SetRouted()
		{
		}

		// Token: 0x06003E57 RID: 15959 RVA: 0x00129DF5 File Offset: 0x00127FF5
		public void OnAgentRemoved(float agentHealth)
		{
			if (this._troop.IsHero && this._troop.HeroObject.HeroState != Hero.CharacterStates.Dead)
			{
				this._troop.HeroObject.HitPoints = MathF.Max(1, MathF.Round(agentHealth));
			}
		}

		// Token: 0x06003E58 RID: 15960 RVA: 0x00129E33 File Offset: 0x00128033
		void IAgentOriginBase.OnScoreHit(BasicCharacterObject victim, BasicCharacterObject captain, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
		}

		// Token: 0x06003E59 RID: 15961 RVA: 0x00129E35 File Offset: 0x00128035
		public void SetBanner(Banner banner)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040012AE RID: 4782
		private PartyBase _party;

		// Token: 0x040012AF RID: 4783
		private CharacterObject _troop;

		// Token: 0x040012B1 RID: 4785
		private readonly UniqueTroopDescriptor _descriptor;

		// Token: 0x040012B2 RID: 4786
		private readonly bool _alwaysWounded;
	}
}
