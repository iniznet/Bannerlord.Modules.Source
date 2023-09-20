using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class KingdomState : GameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public Army InitialSelectedArmy { get; private set; }

		public Settlement InitialSelectedSettlement { get; private set; }

		public Clan InitialSelectedClan { get; private set; }

		public PolicyObject InitialSelectedPolicy { get; private set; }

		public Kingdom InitialSelectedKingdom { get; private set; }

		public KingdomDecision InitialSelectedDecision { get; private set; }

		public IKingdomStateHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		public KingdomState()
		{
		}

		public KingdomState(KingdomDecision initialSelectedDecision)
		{
			this.InitialSelectedDecision = initialSelectedDecision;
		}

		public KingdomState(Army initialSelectedArmy)
		{
			this.InitialSelectedArmy = initialSelectedArmy;
		}

		public KingdomState(Settlement initialSelectedSettlement)
		{
			this.InitialSelectedSettlement = initialSelectedSettlement;
		}

		public KingdomState(IFaction initialSelectedFaction)
		{
			Clan clan;
			if ((clan = initialSelectedFaction as Clan) != null)
			{
				this.InitialSelectedClan = clan;
				return;
			}
			Kingdom kingdom;
			if ((kingdom = initialSelectedFaction as Kingdom) != null)
			{
				this.InitialSelectedKingdom = kingdom;
			}
		}

		public KingdomState(PolicyObject initialSelectedPolicy)
		{
			this.InitialSelectedPolicy = initialSelectedPolicy;
		}

		private IKingdomStateHandler _handler;
	}
}
