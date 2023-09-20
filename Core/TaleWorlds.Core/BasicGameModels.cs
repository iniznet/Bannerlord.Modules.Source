using System;
using System.Collections.Generic;

namespace TaleWorlds.Core
{
	public class BasicGameModels : GameModelsManager
	{
		public RidingModel RidingModel { get; private set; }

		public StrikeMagnitudeCalculationModel StrikeMagnitudeModel { get; private set; }

		public BattleSurvivalModel BattleSurvivalModel { get; private set; }

		public ItemCategorySelector ItemCategorySelector { get; private set; }

		public ItemValueModel ItemValueModel { get; private set; }

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
