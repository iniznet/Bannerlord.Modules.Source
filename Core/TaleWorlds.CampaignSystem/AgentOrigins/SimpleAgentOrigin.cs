using System;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.AgentOrigins
{
	public class SimpleAgentOrigin : IAgentOriginBase
	{
		public BasicCharacterObject Troop
		{
			get
			{
				return this._troop;
			}
		}

		public bool IsUnderPlayersCommand
		{
			get
			{
				PartyBase party = this.Party;
				return party != null && (party == PartyBase.MainParty || party.Owner == Hero.MainHero || party.MapFaction.Leader == Hero.MainHero);
			}
		}

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
				return this._banner;
			}
		}

		public int Rank { get; private set; }

		public int UniqueSeed
		{
			get
			{
				return this._descriptor.UniqueSeed;
			}
		}

		public SimpleAgentOrigin(BasicCharacterObject troop, int rank = -1, Banner banner = null, UniqueTroopDescriptor descriptor = default(UniqueTroopDescriptor))
		{
			this._troop = (CharacterObject)troop;
			this._descriptor = descriptor;
			this.Rank = ((rank == -1) ? MBRandom.RandomInt(10000) : rank);
			this._banner = banner;
		}

		public void SetWounded()
		{
		}

		public void SetKilled()
		{
			if (!MBNetwork.IsClient && this._troop.IsHero)
			{
				KillCharacterAction.ApplyByBattle(this._troop.HeroObject, null, true);
			}
		}

		public void SetRouted()
		{
		}

		public void OnAgentRemoved(float agentHealth)
		{
		}

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

		public void SetBanner(Banner banner)
		{
			this._banner = banner;
		}

		private CharacterObject _troop;

		private Banner _banner;

		private UniqueTroopDescriptor _descriptor;
	}
}
