using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	// Token: 0x0200001A RID: 26
	public class BasicGameModels : GameModelsManager
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00006279 File Offset: 0x00004479
		// (set) Token: 0x06000165 RID: 357 RVA: 0x00006281 File Offset: 0x00004481
		public RidingModel RidingModel { get; private set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000166 RID: 358 RVA: 0x0000628A File Offset: 0x0000448A
		// (set) Token: 0x06000167 RID: 359 RVA: 0x00006292 File Offset: 0x00004492
		public StrikeMagnitudeCalculationModel StrikeMagnitudeModel { get; private set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000168 RID: 360 RVA: 0x0000629B File Offset: 0x0000449B
		// (set) Token: 0x06000169 RID: 361 RVA: 0x000062A3 File Offset: 0x000044A3
		public BattleSurvivalModel BattleSurvivalModel { get; private set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600016A RID: 362 RVA: 0x000062AC File Offset: 0x000044AC
		// (set) Token: 0x0600016B RID: 363 RVA: 0x000062B4 File Offset: 0x000044B4
		public ItemCategorySelector ItemCategorySelector { get; private set; }

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600016C RID: 364 RVA: 0x000062BD File Offset: 0x000044BD
		// (set) Token: 0x0600016D RID: 365 RVA: 0x000062C5 File Offset: 0x000044C5
		public ItemValueModel ItemValueModel { get; private set; }

		// Token: 0x0600016E RID: 366 RVA: 0x000062CE File Offset: 0x000044CE
		public BasicGameModels(IEnumerable<GameModel> inputComponents)
			: base(inputComponents)
		{
			this.StrikeMagnitudeModel = base.GetGameModel<StrikeMagnitudeCalculationModel>();
			this.RidingModel = base.GetGameModel<RidingModel>();
			this.ItemCategorySelector = base.GetGameModel<ItemCategorySelector>();
			this.ItemValueModel = base.GetGameModel<ItemValueModel>();
		}
	}
}
