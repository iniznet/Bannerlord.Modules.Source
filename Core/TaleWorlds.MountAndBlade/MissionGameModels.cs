using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000247 RID: 583
	public sealed class MissionGameModels : GameModelsManager
	{
		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06001FA7 RID: 8103 RVA: 0x0007084F File Offset: 0x0006EA4F
		// (set) Token: 0x06001FA8 RID: 8104 RVA: 0x00070856 File Offset: 0x0006EA56
		public static MissionGameModels Current { get; private set; }

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x06001FA9 RID: 8105 RVA: 0x0007085E File Offset: 0x0006EA5E
		// (set) Token: 0x06001FAA RID: 8106 RVA: 0x00070866 File Offset: 0x0006EA66
		public AgentStatCalculateModel AgentStatCalculateModel { get; private set; }

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x06001FAB RID: 8107 RVA: 0x0007086F File Offset: 0x0006EA6F
		// (set) Token: 0x06001FAC RID: 8108 RVA: 0x00070877 File Offset: 0x0006EA77
		public ApplyWeatherEffectsModel ApplyWeatherEffectsModel { get; private set; }

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06001FAD RID: 8109 RVA: 0x00070880 File Offset: 0x0006EA80
		// (set) Token: 0x06001FAE RID: 8110 RVA: 0x00070888 File Offset: 0x0006EA88
		public AgentApplyDamageModel AgentApplyDamageModel { get; private set; }

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06001FAF RID: 8111 RVA: 0x00070891 File Offset: 0x0006EA91
		// (set) Token: 0x06001FB0 RID: 8112 RVA: 0x00070899 File Offset: 0x0006EA99
		public AgentDecideKilledOrUnconsciousModel AgentDecideKilledOrUnconsciousModel { get; private set; }

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x06001FB1 RID: 8113 RVA: 0x000708A2 File Offset: 0x0006EAA2
		// (set) Token: 0x06001FB2 RID: 8114 RVA: 0x000708AA File Offset: 0x0006EAAA
		public MissionDifficultyModel MissionDifficultyModel { get; private set; }

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x06001FB3 RID: 8115 RVA: 0x000708B3 File Offset: 0x0006EAB3
		// (set) Token: 0x06001FB4 RID: 8116 RVA: 0x000708BB File Offset: 0x0006EABB
		public BattleMoraleModel BattleMoraleModel { get; private set; }

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x06001FB5 RID: 8117 RVA: 0x000708C4 File Offset: 0x0006EAC4
		// (set) Token: 0x06001FB6 RID: 8118 RVA: 0x000708CC File Offset: 0x0006EACC
		public BattleInitializationModel BattleInitializationModel { get; private set; }

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x06001FB7 RID: 8119 RVA: 0x000708D5 File Offset: 0x0006EAD5
		// (set) Token: 0x06001FB8 RID: 8120 RVA: 0x000708DD File Offset: 0x0006EADD
		public BattleSpawnModel BattleSpawnModel { get; private set; }

		// Token: 0x17000637 RID: 1591
		// (get) Token: 0x06001FB9 RID: 8121 RVA: 0x000708E6 File Offset: 0x0006EAE6
		// (set) Token: 0x06001FBA RID: 8122 RVA: 0x000708EE File Offset: 0x0006EAEE
		public BattleBannerBearersModel BattleBannerBearersModel { get; private set; }

		// Token: 0x17000638 RID: 1592
		// (get) Token: 0x06001FBB RID: 8123 RVA: 0x000708F7 File Offset: 0x0006EAF7
		// (set) Token: 0x06001FBC RID: 8124 RVA: 0x000708FF File Offset: 0x0006EAFF
		public FormationArrangementModel FormationArrangementsModel { get; private set; }

		// Token: 0x17000639 RID: 1593
		// (get) Token: 0x06001FBD RID: 8125 RVA: 0x00070908 File Offset: 0x0006EB08
		// (set) Token: 0x06001FBE RID: 8126 RVA: 0x00070910 File Offset: 0x0006EB10
		public AutoBlockModel AutoBlockModel { get; private set; }

		// Token: 0x1700063A RID: 1594
		// (get) Token: 0x06001FBF RID: 8127 RVA: 0x00070919 File Offset: 0x0006EB19
		// (set) Token: 0x06001FC0 RID: 8128 RVA: 0x00070921 File Offset: 0x0006EB21
		public DamageParticleModel DamageParticleModel { get; private set; }

		// Token: 0x1700063B RID: 1595
		// (get) Token: 0x06001FC1 RID: 8129 RVA: 0x0007092A File Offset: 0x0006EB2A
		// (set) Token: 0x06001FC2 RID: 8130 RVA: 0x00070932 File Offset: 0x0006EB32
		public ItemPickupModel ItemPickupModel { get; private set; }

		// Token: 0x06001FC3 RID: 8131 RVA: 0x0007093C File Offset: 0x0006EB3C
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

		// Token: 0x06001FC4 RID: 8132 RVA: 0x000709E5 File Offset: 0x0006EBE5
		private void MakeGameComponentBindings()
		{
		}

		// Token: 0x06001FC5 RID: 8133 RVA: 0x000709E7 File Offset: 0x0006EBE7
		public MissionGameModels(IEnumerable<GameModel> inputComponents)
			: base(inputComponents)
		{
			MissionGameModels.Current = this;
			this.GetSpecificGameBehaviors();
			this.MakeGameComponentBindings();
		}

		// Token: 0x06001FC6 RID: 8134 RVA: 0x00070A02 File Offset: 0x0006EC02
		public static void Clear()
		{
			MissionGameModels.Current = null;
		}
	}
}
