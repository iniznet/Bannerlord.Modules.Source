using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.AgentOrigins
{
	public class PartyAgentOrigin : IAgentOriginBase
	{
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

		public IBattleCombatant BattleCombatant
		{
			get
			{
				return this.Party;
			}
		}

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

		public BasicCharacterObject Troop
		{
			get
			{
				return this._troop;
			}
		}

		public int Rank { get; private set; }

		public bool IsUnderPlayersCommand
		{
			get
			{
				PartyBase party = this.Party;
				return (party != null && party == PartyBase.MainParty) || party.Owner == Hero.MainHero || party.MapFaction.Leader == Hero.MainHero;
			}
		}

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

		public int UniqueSeed
		{
			get
			{
				return this._descriptor.UniqueSeed;
			}
		}

		public PartyAgentOrigin(PartyBase partyBase, CharacterObject characterObject, int rank = -1, UniqueTroopDescriptor uniqueNo = default(UniqueTroopDescriptor), bool alwaysWounded = false)
		{
			this.Party = partyBase;
			this._troop = characterObject;
			this._descriptor = ((!uniqueNo.IsValid) ? new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed) : uniqueNo);
			this.Rank = ((rank == -1) ? MBRandom.RandomInt(10000) : rank);
			this._alwaysWounded = alwaysWounded;
		}

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

		public void SetRouted()
		{
		}

		public void OnAgentRemoved(float agentHealth)
		{
			if (this._troop.IsHero && this._troop.HeroObject.HeroState != Hero.CharacterStates.Dead)
			{
				this._troop.HeroObject.HitPoints = MathF.Max(1, MathF.Round(agentHealth));
			}
		}

		void IAgentOriginBase.OnScoreHit(BasicCharacterObject victim, BasicCharacterObject captain, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
		}

		public void SetBanner(Banner banner)
		{
			throw new NotImplementedException();
		}

		private PartyBase _party;

		private CharacterObject _troop;

		private readonly UniqueTroopDescriptor _descriptor;

		private readonly bool _alwaysWounded;
	}
}
