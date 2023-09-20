using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	public sealed class MissionGameModels : GameModelsManager
	{
		public static MissionGameModels Current { get; private set; }

		public AgentStatCalculateModel AgentStatCalculateModel { get; private set; }

		public ApplyWeatherEffectsModel ApplyWeatherEffectsModel { get; private set; }

		public AgentApplyDamageModel AgentApplyDamageModel { get; private set; }

		public AgentDecideKilledOrUnconsciousModel AgentDecideKilledOrUnconsciousModel { get; private set; }

		public MissionDifficultyModel MissionDifficultyModel { get; private set; }

		public BattleMoraleModel BattleMoraleModel { get; private set; }

		public BattleInitializationModel BattleInitializationModel { get; private set; }

		public BattleSpawnModel BattleSpawnModel { get; private set; }

		public BattleBannerBearersModel BattleBannerBearersModel { get; private set; }

		public FormationArrangementModel FormationArrangementsModel { get; private set; }

		public AutoBlockModel AutoBlockModel { get; private set; }

		public DamageParticleModel DamageParticleModel { get; private set; }

		public ItemPickupModel ItemPickupModel { get; private set; }

		private void GetSpecificGameBehaviors()
		{
			this.AgentStatCalculateModel = base.GetGameModel<AgentStatCalculateModel>();
			this.ApplyWeatherEffectsModel = base.GetGameModel<ApplyWeatherEffectsModel>();
			this.AgentApplyDamageModel = base.GetGameModel<AgentApplyDamageModel>();
			this.AgentDecideKilledOrUnconsciousModel = base.GetGameModel<AgentDecideKilledOrUnconsciousModel>();
			this.MissionDifficultyModel = base.GetGameModel<MissionDifficultyModel>();
			this.BattleMoraleModel = base.GetGameModel<BattleMoraleModel>();
			this.BattleInitializationModel = base.GetGameModel<BattleInitializationModel>();
			this.BattleSpawnModel = base.GetGameModel<BattleSpawnModel>();
			this.BattleBannerBearersModel = base.GetGameModel<BattleBannerBearersModel>();
			this.FormationArrangementsModel = base.GetGameModel<FormationArrangementModel>();
			this.AutoBlockModel = base.GetGameModel<AutoBlockModel>();
			this.DamageParticleModel = base.GetGameModel<DamageParticleModel>();
			this.ItemPickupModel = base.GetGameModel<ItemPickupModel>();
		}

		private void MakeGameComponentBindings()
		{
		}

		public MissionGameModels(IEnumerable<GameModel> inputComponents)
			: base(inputComponents)
		{
			MissionGameModels.Current = this;
			this.GetSpecificGameBehaviors();
			this.MakeGameComponentBindings();
		}

		public static void Clear()
		{
			MissionGameModels.Current = null;
		}
	}
}
