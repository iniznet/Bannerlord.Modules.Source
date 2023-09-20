using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x0200033C RID: 828
	public class KingdomState : GameState
	{
		// Token: 0x17000B08 RID: 2824
		// (get) Token: 0x06002E59 RID: 11865 RVA: 0x000BFF1A File Offset: 0x000BE11A
		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x06002E5A RID: 11866 RVA: 0x000BFF1D File Offset: 0x000BE11D
		// (set) Token: 0x06002E5B RID: 11867 RVA: 0x000BFF25 File Offset: 0x000BE125
		public Army InitialSelectedArmy { get; private set; }

		// Token: 0x17000B0A RID: 2826
		// (get) Token: 0x06002E5C RID: 11868 RVA: 0x000BFF2E File Offset: 0x000BE12E
		// (set) Token: 0x06002E5D RID: 11869 RVA: 0x000BFF36 File Offset: 0x000BE136
		public Settlement InitialSelectedSettlement { get; private set; }

		// Token: 0x17000B0B RID: 2827
		// (get) Token: 0x06002E5E RID: 11870 RVA: 0x000BFF3F File Offset: 0x000BE13F
		// (set) Token: 0x06002E5F RID: 11871 RVA: 0x000BFF47 File Offset: 0x000BE147
		public Clan InitialSelectedClan { get; private set; }

		// Token: 0x17000B0C RID: 2828
		// (get) Token: 0x06002E60 RID: 11872 RVA: 0x000BFF50 File Offset: 0x000BE150
		// (set) Token: 0x06002E61 RID: 11873 RVA: 0x000BFF58 File Offset: 0x000BE158
		public PolicyObject InitialSelectedPolicy { get; private set; }

		// Token: 0x17000B0D RID: 2829
		// (get) Token: 0x06002E62 RID: 11874 RVA: 0x000BFF61 File Offset: 0x000BE161
		// (set) Token: 0x06002E63 RID: 11875 RVA: 0x000BFF69 File Offset: 0x000BE169
		public Kingdom InitialSelectedKingdom { get; private set; }

		// Token: 0x17000B0E RID: 2830
		// (get) Token: 0x06002E64 RID: 11876 RVA: 0x000BFF72 File Offset: 0x000BE172
		// (set) Token: 0x06002E65 RID: 11877 RVA: 0x000BFF7A File Offset: 0x000BE17A
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

		// Token: 0x06002E66 RID: 11878 RVA: 0x000BFF83 File Offset: 0x000BE183
		public KingdomState()
		{
		}

		// Token: 0x06002E67 RID: 11879 RVA: 0x000BFF8B File Offset: 0x000BE18B
		public KingdomState(Army initialSelectedArmy)
		{
			this.InitialSelectedArmy = initialSelectedArmy;
		}

		// Token: 0x06002E68 RID: 11880 RVA: 0x000BFF9A File Offset: 0x000BE19A
		public KingdomState(Settlement initialSelectedSettlement)
		{
			this.InitialSelectedSettlement = initialSelectedSettlement;
		}

		// Token: 0x06002E69 RID: 11881 RVA: 0x000BFFAC File Offset: 0x000BE1AC
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

		// Token: 0x06002E6A RID: 11882 RVA: 0x000BFFE2 File Offset: 0x000BE1E2
		public KingdomState(PolicyObject initialSelectedPolicy)
		{
			this.InitialSelectedPolicy = initialSelectedPolicy;
		}

		// Token: 0x04000DF3 RID: 3571
		private IKingdomStateHandler _handler;
	}
}
