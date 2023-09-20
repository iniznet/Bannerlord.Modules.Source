using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x02000331 RID: 817
	public class ClanState : GameState
	{
		// Token: 0x17000AF5 RID: 2805
		// (get) Token: 0x06002E17 RID: 11799 RVA: 0x000BFCD0 File Offset: 0x000BDED0
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000AF6 RID: 2806
		// (get) Token: 0x06002E18 RID: 11800 RVA: 0x000BFCD3 File Offset: 0x000BDED3
		// (set) Token: 0x06002E19 RID: 11801 RVA: 0x000BFCDB File Offset: 0x000BDEDB
		public Hero InitialSelectedHero { get; private set; }

		// Token: 0x17000AF7 RID: 2807
		// (get) Token: 0x06002E1A RID: 11802 RVA: 0x000BFCE4 File Offset: 0x000BDEE4
		// (set) Token: 0x06002E1B RID: 11803 RVA: 0x000BFCEC File Offset: 0x000BDEEC
		public PartyBase InitialSelectedParty { get; private set; }

		// Token: 0x17000AF8 RID: 2808
		// (get) Token: 0x06002E1C RID: 11804 RVA: 0x000BFCF5 File Offset: 0x000BDEF5
		// (set) Token: 0x06002E1D RID: 11805 RVA: 0x000BFCFD File Offset: 0x000BDEFD
		public Settlement InitialSelectedSettlement { get; private set; }

		// Token: 0x17000AF9 RID: 2809
		// (get) Token: 0x06002E1E RID: 11806 RVA: 0x000BFD06 File Offset: 0x000BDF06
		// (set) Token: 0x06002E1F RID: 11807 RVA: 0x000BFD0E File Offset: 0x000BDF0E
		public Workshop InitialSelectedWorkshop { get; private set; }

		// Token: 0x17000AFA RID: 2810
		// (get) Token: 0x06002E20 RID: 11808 RVA: 0x000BFD17 File Offset: 0x000BDF17
		// (set) Token: 0x06002E21 RID: 11809 RVA: 0x000BFD1F File Offset: 0x000BDF1F
		public Alley InitialSelectedAlley { get; private set; }

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x06002E22 RID: 11810 RVA: 0x000BFD28 File Offset: 0x000BDF28
		// (set) Token: 0x06002E23 RID: 11811 RVA: 0x000BFD30 File Offset: 0x000BDF30
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

		// Token: 0x06002E24 RID: 11812 RVA: 0x000BFD39 File Offset: 0x000BDF39
		public ClanState()
		{
		}

		// Token: 0x06002E25 RID: 11813 RVA: 0x000BFD41 File Offset: 0x000BDF41
		public ClanState(Hero initialSelectedHero)
		{
			this.InitialSelectedHero = initialSelectedHero;
		}

		// Token: 0x06002E26 RID: 11814 RVA: 0x000BFD50 File Offset: 0x000BDF50
		public ClanState(PartyBase initialSelectedParty)
		{
			this.InitialSelectedParty = initialSelectedParty;
		}

		// Token: 0x06002E27 RID: 11815 RVA: 0x000BFD5F File Offset: 0x000BDF5F
		public ClanState(Settlement initialSelectedSettlement)
		{
			this.InitialSelectedSettlement = initialSelectedSettlement;
		}

		// Token: 0x06002E28 RID: 11816 RVA: 0x000BFD6E File Offset: 0x000BDF6E
		public ClanState(Workshop initialSelectedWorkshop)
		{
			this.InitialSelectedWorkshop = initialSelectedWorkshop;
		}

		// Token: 0x06002E29 RID: 11817 RVA: 0x000BFD7D File Offset: 0x000BDF7D
		public ClanState(Alley initialSelectedAlley)
		{
			this.InitialSelectedAlley = initialSelectedAlley;
		}

		// Token: 0x04000DE5 RID: 3557
		private IClanStateHandler _handler;
	}
}
