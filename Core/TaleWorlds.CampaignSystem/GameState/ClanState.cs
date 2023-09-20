using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class ClanState : GameState
	{
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public Hero InitialSelectedHero { get; private set; }

		public PartyBase InitialSelectedParty { get; private set; }

		public Settlement InitialSelectedSettlement { get; private set; }

		public Workshop InitialSelectedWorkshop { get; private set; }

		public Alley InitialSelectedAlley { get; private set; }

		public IClanStateHandler Handler
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

		public ClanState()
		{
		}

		public ClanState(Hero initialSelectedHero)
		{
			this.InitialSelectedHero = initialSelectedHero;
		}

		public ClanState(PartyBase initialSelectedParty)
		{
			this.InitialSelectedParty = initialSelectedParty;
		}

		public ClanState(Settlement initialSelectedSettlement)
		{
			this.InitialSelectedSettlement = initialSelectedSettlement;
		}

		public ClanState(Workshop initialSelectedWorkshop)
		{
			this.InitialSelectedWorkshop = initialSelectedWorkshop;
		}

		public ClanState(Alley initialSelectedAlley)
		{
			this.InitialSelectedAlley = initialSelectedAlley;
		}

		private IClanStateHandler _handler;
	}
}
