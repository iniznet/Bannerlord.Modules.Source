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
	public class PartyGroupAgentOrigin : IAgentOriginBase
	{
		internal PartyGroupAgentOrigin(PartyGroupTroopSupplier supplier, UniqueTroopDescriptor descriptor, int rank)
		{
			this._supplier = supplier;
			this._descriptor = descriptor;
			this._rank = rank;
		}

		public PartyBase Party
		{
			get
			{
				return this._supplier.GetParty(this._descriptor);
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
				if (this.Party.LeaderHero == null)
				{
					return this.Party.MapFaction.Banner;
				}
				return this.Party.LeaderHero.ClanBanner;
			}
		}

		public int UniqueSeed
		{
			get
			{
				return this._descriptor.UniqueSeed;
			}
		}

		public CharacterObject Troop
		{
			get
			{
				return this._supplier.GetTroop(this._descriptor);
			}
		}

		BasicCharacterObject IAgentOriginBase.Troop
		{
			get
			{
				return this.Troop;
			}
		}

		public UniqueTroopDescriptor TroopDesc
		{
			get
			{
				return this._descriptor;
			}
		}

		public int Rank
		{
			get
			{
				return this._rank;
			}
		}

		public bool IsUnderPlayersCommand
		{
			get
			{
				return this.Troop == Hero.MainHero.CharacterObject || PartyGroupAgentOrigin.IsPartyUnderPlayerCommand(this.Party);
			}
		}

		public uint FactionColor
		{
			get
			{
				return this.Party.MapFaction.Color;
			}
		}

		public uint FactionColor2
		{
			get
			{
				return this.Party.MapFaction.Color2;
			}
		}

		public int Seed
		{
			get
			{
				return CharacterHelper.GetPartyMemberFaceSeed(this.Party, this.Troop, this.Rank);
			}
		}

		public void SetWounded()
		{
			if (!this._isRemoved)
			{
				this._supplier.OnTroopWounded(this._descriptor);
				this._isRemoved = true;
			}
		}

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

		public void SetRouted()
		{
			if (!this._isRemoved)
			{
				this._supplier.OnTroopRouted(this._descriptor);
				this._isRemoved = true;
			}
		}

		public void OnAgentRemoved(float agentHealth)
		{
			if (this.Troop.IsHero)
			{
				this.Troop.HeroObject.HitPoints = MathF.Max(1, MathF.Round(agentHealth));
			}
		}

		void IAgentOriginBase.OnScoreHit(BasicCharacterObject victim, BasicCharacterObject captain, int damage, bool isFatal, bool isTeamKill, WeaponComponentData attackerWeapon)
		{
			this._supplier.OnTroopScoreHit(this._descriptor, victim, damage, isFatal, isTeamKill, attackerWeapon);
		}

		public void SetBanner(Banner banner)
		{
			throw new NotImplementedException();
		}

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

		private readonly PartyGroupTroopSupplier _supplier;

		private readonly UniqueTroopDescriptor _descriptor;

		private readonly int _rank;

		private bool _isRemoved;
	}
}
