﻿using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Settlements.Workshops
{
	// Token: 0x02000368 RID: 872
	public class Workshop : SettlementArea
	{
		// Token: 0x0600328B RID: 12939 RVA: 0x000D1DF7 File Offset: 0x000CFFF7
		internal static void AutoGeneratedStaticCollectObjectsWorkshop(object o, List<object> collectedObjects)
		{
			((Workshop)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		// Token: 0x0600328C RID: 12940 RVA: 0x000D1E08 File Offset: 0x000D0008
		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this._settlement);
			collectedObjects.Add(this._owner);
			collectedObjects.Add(this._customName);
			collectedObjects.Add(this._productionProgress);
			collectedObjects.Add(this.WorkshopType);
			collectedObjects.Add(this.InsideParty);
		}

		// Token: 0x0600328D RID: 12941 RVA: 0x000D1E64 File Offset: 0x000D0064
		internal static object AutoGeneratedGetMemberValueWorkshopType(object o)
		{
			return ((Workshop)o).WorkshopType;
		}

		// Token: 0x0600328E RID: 12942 RVA: 0x000D1E71 File Offset: 0x000D0071
		internal static object AutoGeneratedGetMemberValueConstructionTimeRemained(object o)
		{
			return ((Workshop)o).ConstructionTimeRemained;
		}

		// Token: 0x0600328F RID: 12943 RVA: 0x000D1E83 File Offset: 0x000D0083
		internal static object AutoGeneratedGetMemberValueInsideParty(object o)
		{
			return ((Workshop)o).InsideParty;
		}

		// Token: 0x06003290 RID: 12944 RVA: 0x000D1E90 File Offset: 0x000D0090
		internal static object AutoGeneratedGetMemberValueNotRunnedDays(object o)
		{
			return ((Workshop)o).NotRunnedDays;
		}

		// Token: 0x06003291 RID: 12945 RVA: 0x000D1EA2 File Offset: 0x000D00A2
		internal static object AutoGeneratedGetMemberValueCapital(object o)
		{
			return ((Workshop)o).Capital;
		}

		// Token: 0x06003292 RID: 12946 RVA: 0x000D1EB4 File Offset: 0x000D00B4
		internal static object AutoGeneratedGetMemberValueInitialCapital(object o)
		{
			return ((Workshop)o).InitialCapital;
		}

		// Token: 0x06003293 RID: 12947 RVA: 0x000D1EC6 File Offset: 0x000D00C6
		internal static object AutoGeneratedGetMemberValueLevel(object o)
		{
			return ((Workshop)o).Level;
		}

		// Token: 0x06003294 RID: 12948 RVA: 0x000D1ED8 File Offset: 0x000D00D8
		internal static object AutoGeneratedGetMemberValueUpgradable(object o)
		{
			return ((Workshop)o).Upgradable;
		}

		// Token: 0x06003295 RID: 12949 RVA: 0x000D1EEA File Offset: 0x000D00EA
		internal static object AutoGeneratedGetMemberValue_settlement(object o)
		{
			return ((Workshop)o)._settlement;
		}

		// Token: 0x06003296 RID: 12950 RVA: 0x000D1EF7 File Offset: 0x000D00F7
		internal static object AutoGeneratedGetMemberValue_tag(object o)
		{
			return ((Workshop)o)._tag;
		}

		// Token: 0x06003297 RID: 12951 RVA: 0x000D1F04 File Offset: 0x000D0104
		internal static object AutoGeneratedGetMemberValue_owner(object o)
		{
			return ((Workshop)o)._owner;
		}

		// Token: 0x06003298 RID: 12952 RVA: 0x000D1F11 File Offset: 0x000D0111
		internal static object AutoGeneratedGetMemberValue_customName(object o)
		{
			return ((Workshop)o)._customName;
		}

		// Token: 0x06003299 RID: 12953 RVA: 0x000D1F1E File Offset: 0x000D011E
		internal static object AutoGeneratedGetMemberValue_productionProgress(object o)
		{
			return ((Workshop)o)._productionProgress;
		}

		// Token: 0x17000C55 RID: 3157
		// (get) Token: 0x0600329A RID: 12954 RVA: 0x000D1F2B File Offset: 0x000D012B
		public override Settlement Settlement
		{
			get
			{
				return this._settlement;
			}
		}

		// Token: 0x17000C56 RID: 3158
		// (get) Token: 0x0600329B RID: 12955 RVA: 0x000D1F33 File Offset: 0x000D0133
		public override string Tag
		{
			get
			{
				return this._tag;
			}
		}

		// Token: 0x17000C57 RID: 3159
		// (get) Token: 0x0600329C RID: 12956 RVA: 0x000D1F3B File Offset: 0x000D013B
		public override Hero Owner
		{
			get
			{
				return this._owner;
			}
		}

		// Token: 0x17000C58 RID: 3160
		// (get) Token: 0x0600329D RID: 12957 RVA: 0x000D1F43 File Offset: 0x000D0143
		public override TextObject Name
		{
			get
			{
				TextObject textObject;
				if ((textObject = this._customName) == null)
				{
					WorkshopType workshopType = this.WorkshopType;
					textObject = ((workshopType != null) ? workshopType.Name : null) ?? new TextObject("{=xWoXL2FG}Empty Workshop", null);
				}
				return textObject;
			}
		}

		// Token: 0x17000C59 RID: 3161
		// (get) Token: 0x0600329E RID: 12958 RVA: 0x000D1F70 File Offset: 0x000D0170
		// (set) Token: 0x0600329F RID: 12959 RVA: 0x000D1F78 File Offset: 0x000D0178
		[SaveableProperty(105)]
		public WorkshopType WorkshopType { get; private set; }

		// Token: 0x17000C5A RID: 3162
		// (get) Token: 0x060032A0 RID: 12960 RVA: 0x000D1F81 File Offset: 0x000D0181
		// (set) Token: 0x060032A1 RID: 12961 RVA: 0x000D1F89 File Offset: 0x000D0189
		[SaveableProperty(106)]
		public int ConstructionTimeRemained { get; private set; }

		// Token: 0x17000C5B RID: 3163
		// (get) Token: 0x060032A2 RID: 12962 RVA: 0x000D1F92 File Offset: 0x000D0192
		// (set) Token: 0x060032A3 RID: 12963 RVA: 0x000D1F9A File Offset: 0x000D019A
		[SaveableProperty(107)]
		public MobileParty InsideParty { get; private set; }

		// Token: 0x17000C5C RID: 3164
		// (get) Token: 0x060032A4 RID: 12964 RVA: 0x000D1FA3 File Offset: 0x000D01A3
		public int ProfitMade
		{
			get
			{
				return MathF.Max(this.Capital - this.InitialCapital, 0);
			}
		}

		// Token: 0x17000C5D RID: 3165
		// (get) Token: 0x060032A5 RID: 12965 RVA: 0x000D1FB8 File Offset: 0x000D01B8
		public int Expense
		{
			get
			{
				return Campaign.Current.Models.WorkshopModel.GetDailyExpense(this.Level);
			}
		}

		// Token: 0x17000C5E RID: 3166
		// (get) Token: 0x060032A6 RID: 12966 RVA: 0x000D1FD4 File Offset: 0x000D01D4
		// (set) Token: 0x060032A7 RID: 12967 RVA: 0x000D1FDC File Offset: 0x000D01DC
		[SaveableProperty(110)]
		public int NotRunnedDays { get; private set; }

		// Token: 0x17000C5F RID: 3167
		// (get) Token: 0x060032A8 RID: 12968 RVA: 0x000D1FE5 File Offset: 0x000D01E5
		// (set) Token: 0x060032A9 RID: 12969 RVA: 0x000D1FED File Offset: 0x000D01ED
		[SaveableProperty(111)]
		public int Capital { get; private set; }

		// Token: 0x17000C60 RID: 3168
		// (get) Token: 0x060032AA RID: 12970 RVA: 0x000D1FF6 File Offset: 0x000D01F6
		// (set) Token: 0x060032AB RID: 12971 RVA: 0x000D1FFE File Offset: 0x000D01FE
		[SaveableProperty(112)]
		public int InitialCapital { get; private set; }

		// Token: 0x17000C61 RID: 3169
		// (get) Token: 0x060032AC RID: 12972 RVA: 0x000D2007 File Offset: 0x000D0207
		// (set) Token: 0x060032AD RID: 12973 RVA: 0x000D200F File Offset: 0x000D020F
		[SaveableProperty(113)]
		public int Level { get; private set; }

		// Token: 0x17000C62 RID: 3170
		// (get) Token: 0x060032AE RID: 12974 RVA: 0x000D2018 File Offset: 0x000D0218
		// (set) Token: 0x060032AF RID: 12975 RVA: 0x000D2020 File Offset: 0x000D0220
		[SaveableProperty(114)]
		public bool Upgradable { get; private set; }

		// Token: 0x17000C63 RID: 3171
		// (get) Token: 0x060032B0 RID: 12976 RVA: 0x000D2029 File Offset: 0x000D0229
		public bool CanBeUpgraded
		{
			get
			{
				return this.Upgradable && this.Level < Campaign.Current.Models.WorkshopModel.MaxWorkshopLevel;
			}
		}

		// Token: 0x17000C64 RID: 3172
		// (get) Token: 0x060032B1 RID: 12977 RVA: 0x000D2051 File Offset: 0x000D0251
		public bool CanBeDowngraded
		{
			get
			{
				return this.Level > 1;
			}
		}

		// Token: 0x17000C65 RID: 3173
		// (get) Token: 0x060032B2 RID: 12978 RVA: 0x000D205C File Offset: 0x000D025C
		public bool IsRunning
		{
			get
			{
				return this.WorkshopType != null && this.ConstructionTimeRemained == 0;
			}
		}

		// Token: 0x060032B3 RID: 12979 RVA: 0x000D2071 File Offset: 0x000D0271
		public Workshop(Settlement settlement, string tag)
		{
			this._customName = null;
			this._settlement = settlement;
			this._tag = tag;
			this.ConstructionTimeRemained = 0;
			this.Capital = 0;
			this.InitialCapital = 0;
			this.Level = 1;
			this.Upgradable = false;
		}

		// Token: 0x060032B4 RID: 12980 RVA: 0x000D20B1 File Offset: 0x000D02B1
		public override int GetHashCode()
		{
			return this.Settlement.GetHashCode() + this._tag.GetHashCode();
		}

		// Token: 0x060032B5 RID: 12981 RVA: 0x000D20CC File Offset: 0x000D02CC
		public void SetWorkshop(Hero newOwner, WorkshopType workshopType, int capital, bool upgradable = true, int constructionTimeRemained = 0, int level = 1, TextObject customName = null)
		{
			Hero owner = this._owner;
			WorkshopType workshopType2 = this.WorkshopType;
			this.WorkshopType = workshopType;
			this._customName = customName;
			Hero owner2 = this._owner;
			if (owner2 != null)
			{
				owner2.RemoveOwnedWorkshop(this);
			}
			this._owner = newOwner;
			Hero owner3 = this._owner;
			if (owner3 != null)
			{
				owner3.AddOwnedWorkshop(this);
			}
			this._productionProgress = new float[workshopType.Productions.Count];
			this.Upgradable = upgradable;
			this.ConstructionTimeRemained = constructionTimeRemained;
			this.Capital = capital;
			this.InitialCapital = capital;
			this.Level = level;
			CampaignEventDispatcher.Instance.OnWorkshopChanged(this, owner, workshopType2);
		}

		// Token: 0x060032B6 RID: 12982 RVA: 0x000D216A File Offset: 0x000D036A
		public void OnPartyEnters(MobileParty mobileParty)
		{
			this.InsideParty = mobileParty;
		}

		// Token: 0x060032B7 RID: 12983 RVA: 0x000D2173 File Offset: 0x000D0373
		public void ResetNotRunnedDays()
		{
			this.NotRunnedDays = 0;
		}

		// Token: 0x060032B8 RID: 12984 RVA: 0x000D217C File Offset: 0x000D037C
		public void IncreaseNotRunnedDays()
		{
			int notRunnedDays = this.NotRunnedDays;
			this.NotRunnedDays = notRunnedDays + 1;
		}

		// Token: 0x060032B9 RID: 12985 RVA: 0x000D2199 File Offset: 0x000D0399
		public void SetProgress(int i, float value)
		{
			this._productionProgress[i] = value;
		}

		// Token: 0x060032BA RID: 12986 RVA: 0x000D21A4 File Offset: 0x000D03A4
		internal void AfterLoad()
		{
			if (this._productionProgress.Length != this.WorkshopType.Productions.Count)
			{
				this._productionProgress = new float[this.WorkshopType.Productions.Count];
			}
		}

		// Token: 0x060032BB RID: 12987 RVA: 0x000D21DC File Offset: 0x000D03DC
		public void ApplyDailyConstruction()
		{
			int constructionTimeRemained = this.ConstructionTimeRemained;
			this.ConstructionTimeRemained = constructionTimeRemained - 1;
		}

		// Token: 0x060032BC RID: 12988 RVA: 0x000D21F9 File Offset: 0x000D03F9
		public void ChangeGold(int goldChange)
		{
			this.Capital += goldChange;
		}

		// Token: 0x060032BD RID: 12989 RVA: 0x000D220C File Offset: 0x000D040C
		public void Upgrade()
		{
			int upgradeCost = Campaign.Current.Models.WorkshopModel.GetUpgradeCost(this.Level);
			this.Capital = MathF.Max(0, this.Capital - upgradeCost);
			int level = this.Level;
			this.Level = level + 1;
		}

		// Token: 0x060032BE RID: 12990 RVA: 0x000D2258 File Offset: 0x000D0458
		public void Downgrade()
		{
			int level = this.Level;
			this.Level = level - 1;
			this.Capital += Campaign.Current.Models.WorkshopModel.GetUpgradeCost(this.Level) / 2;
		}

		// Token: 0x060032BF RID: 12991 RVA: 0x000D229E File Offset: 0x000D049E
		public float GetProductionProgress(int index)
		{
			return this._productionProgress[index];
		}

		// Token: 0x0400106F RID: 4207
		[SaveableField(100)]
		private readonly Settlement _settlement;

		// Token: 0x04001070 RID: 4208
		[SaveableField(101)]
		private readonly string _tag;

		// Token: 0x04001071 RID: 4209
		[SaveableField(102)]
		private Hero _owner;

		// Token: 0x04001072 RID: 4210
		[SaveableField(103)]
		private TextObject _customName;

		// Token: 0x04001073 RID: 4211
		[SaveableField(104)]
		private float[] _productionProgress;
	}
}
