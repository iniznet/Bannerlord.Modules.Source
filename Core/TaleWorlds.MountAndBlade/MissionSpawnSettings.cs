using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000278 RID: 632
	public struct MissionSpawnSettings
	{
		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x060021A4 RID: 8612 RVA: 0x0007AE13 File Offset: 0x00079013
		// (set) Token: 0x060021A5 RID: 8613 RVA: 0x0007AE1B File Offset: 0x0007901B
		public float GlobalReinforcementInterval
		{
			get
			{
				return this._globalReinforcementInterval;
			}
			set
			{
				this._globalReinforcementInterval = MathF.Max(value, 1f);
			}
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x060021A6 RID: 8614 RVA: 0x0007AE2E File Offset: 0x0007902E
		// (set) Token: 0x060021A7 RID: 8615 RVA: 0x0007AE36 File Offset: 0x00079036
		public float DefenderAdvantageFactor
		{
			get
			{
				return this._defenderAdvantageFactor;
			}
			set
			{
				this._defenderAdvantageFactor = MathF.Clamp(value, 0.1f, 10f);
			}
		}

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x060021A8 RID: 8616 RVA: 0x0007AE4E File Offset: 0x0007904E
		// (set) Token: 0x060021A9 RID: 8617 RVA: 0x0007AE56 File Offset: 0x00079056
		public float MaximumBattleSideRatio
		{
			get
			{
				return this._maximumBattleSizeRatio;
			}
			set
			{
				this._maximumBattleSizeRatio = MathF.Clamp(value, 0.5f, 0.99f);
			}
		}

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x060021AA RID: 8618 RVA: 0x0007AE6E File Offset: 0x0007906E
		// (set) Token: 0x060021AB RID: 8619 RVA: 0x0007AE76 File Offset: 0x00079076
		public MissionSpawnSettings.InitialSpawnMethod InitialTroopsSpawnMethod { get; private set; }

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x060021AC RID: 8620 RVA: 0x0007AE7F File Offset: 0x0007907F
		// (set) Token: 0x060021AD RID: 8621 RVA: 0x0007AE87 File Offset: 0x00079087
		public MissionSpawnSettings.ReinforcementTimingMethod ReinforcementTroopsTimingMethod { get; private set; }

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x060021AE RID: 8622 RVA: 0x0007AE90 File Offset: 0x00079090
		// (set) Token: 0x060021AF RID: 8623 RVA: 0x0007AE98 File Offset: 0x00079098
		public MissionSpawnSettings.ReinforcementSpawnMethod ReinforcementTroopsSpawnMethod { get; private set; }

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x060021B0 RID: 8624 RVA: 0x0007AEA1 File Offset: 0x000790A1
		// (set) Token: 0x060021B1 RID: 8625 RVA: 0x0007AEA9 File Offset: 0x000790A9
		public float ReinforcementBatchPercentage
		{
			get
			{
				return this._reinforcementBatchPercentage;
			}
			set
			{
				this._reinforcementBatchPercentage = MathF.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x060021B2 RID: 8626 RVA: 0x0007AEC1 File Offset: 0x000790C1
		// (set) Token: 0x060021B3 RID: 8627 RVA: 0x0007AEC9 File Offset: 0x000790C9
		public float DesiredReinforcementPercentage
		{
			get
			{
				return this._desiredReinforcementPercentage;
			}
			set
			{
				this._desiredReinforcementPercentage = MathF.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x060021B4 RID: 8628 RVA: 0x0007AEE1 File Offset: 0x000790E1
		// (set) Token: 0x060021B5 RID: 8629 RVA: 0x0007AEE9 File Offset: 0x000790E9
		public float ReinforcementWavePercentage
		{
			get
			{
				return this._reinforcementWavePercentage;
			}
			set
			{
				this._reinforcementWavePercentage = MathF.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x060021B6 RID: 8630 RVA: 0x0007AF01 File Offset: 0x00079101
		// (set) Token: 0x060021B7 RID: 8631 RVA: 0x0007AF09 File Offset: 0x00079109
		public int MaximumReinforcementWaveCount
		{
			get
			{
				return this._maximumReinforcementWaveCount;
			}
			set
			{
				this._maximumReinforcementWaveCount = MathF.Max(value, 0);
			}
		}

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x060021B8 RID: 8632 RVA: 0x0007AF18 File Offset: 0x00079118
		// (set) Token: 0x060021B9 RID: 8633 RVA: 0x0007AF20 File Offset: 0x00079120
		public float DefenderReinforcementBatchPercentage
		{
			get
			{
				return this._defenderReinforcementBatchPercentage;
			}
			set
			{
				this._defenderReinforcementBatchPercentage = MathF.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x060021BA RID: 8634 RVA: 0x0007AF38 File Offset: 0x00079138
		// (set) Token: 0x060021BB RID: 8635 RVA: 0x0007AF40 File Offset: 0x00079140
		public float AttackerReinforcementBatchPercentage
		{
			get
			{
				return this._attackerReinforcementBatchPercentage;
			}
			set
			{
				this._attackerReinforcementBatchPercentage = MathF.Clamp(value, 0f, 1f);
			}
		}

		// Token: 0x060021BC RID: 8636 RVA: 0x0007AF58 File Offset: 0x00079158
		public MissionSpawnSettings(MissionSpawnSettings.InitialSpawnMethod initialTroopsSpawnMethod, MissionSpawnSettings.ReinforcementTimingMethod reinforcementTimingMethod, MissionSpawnSettings.ReinforcementSpawnMethod reinforcementTroopsSpawnMethod, float globalReinforcementInterval = 0f, float reinforcementBatchPercentage = 0f, float desiredReinforcementPercentage = 0f, float reinforcementWavePercentage = 0f, int maximumReinforcementWaveCount = 0, float defenderReinforcementBatchPercentage = 0f, float attackerReinforcementBatchPercentage = 0f, float defenderAdvantageFactor = 1f, float maximumBattleSizeRatio = 0.75f)
		{
			this = default(MissionSpawnSettings);
			this.InitialTroopsSpawnMethod = initialTroopsSpawnMethod;
			this.ReinforcementTroopsTimingMethod = reinforcementTimingMethod;
			this.ReinforcementTroopsSpawnMethod = reinforcementTroopsSpawnMethod;
			this.GlobalReinforcementInterval = globalReinforcementInterval;
			this.ReinforcementBatchPercentage = reinforcementBatchPercentage;
			this.DesiredReinforcementPercentage = desiredReinforcementPercentage;
			this.ReinforcementWavePercentage = reinforcementWavePercentage;
			this.MaximumReinforcementWaveCount = maximumReinforcementWaveCount;
			this.DefenderReinforcementBatchPercentage = defenderReinforcementBatchPercentage;
			this.AttackerReinforcementBatchPercentage = attackerReinforcementBatchPercentage;
			this.DefenderAdvantageFactor = defenderAdvantageFactor;
			this.MaximumBattleSideRatio = maximumBattleSizeRatio;
		}

		// Token: 0x060021BD RID: 8637 RVA: 0x0007AFCC File Offset: 0x000791CC
		public static MissionSpawnSettings CreateDefaultSpawnSettings()
		{
			return new MissionSpawnSettings(MissionSpawnSettings.InitialSpawnMethod.BattleSizeAllocating, MissionSpawnSettings.ReinforcementTimingMethod.GlobalTimer, MissionSpawnSettings.ReinforcementSpawnMethod.Balanced, 10f, 0.05f, 0.166f, 0f, 0, 0f, 0f, 1f, 0.75f);
		}

		// Token: 0x04000C6C RID: 3180
		public const float MinimumReinforcementInterval = 1f;

		// Token: 0x04000C6D RID: 3181
		public const float MinimumDefenderAdvantageFactor = 0.1f;

		// Token: 0x04000C6E RID: 3182
		public const float MaximumDefenderAdvantageFactor = 10f;

		// Token: 0x04000C6F RID: 3183
		public const float MinimumBattleSizeRatioLimit = 0.5f;

		// Token: 0x04000C70 RID: 3184
		public const float MaximumBattleSizeRatioLimit = 0.99f;

		// Token: 0x04000C71 RID: 3185
		public const float DefaultMaximumBattleSizeRatio = 0.75f;

		// Token: 0x04000C72 RID: 3186
		public const float DefaultDefenderAdvantageFactor = 1f;

		// Token: 0x04000C76 RID: 3190
		private float _globalReinforcementInterval;

		// Token: 0x04000C77 RID: 3191
		private float _defenderAdvantageFactor;

		// Token: 0x04000C78 RID: 3192
		private float _maximumBattleSizeRatio;

		// Token: 0x04000C79 RID: 3193
		private float _reinforcementBatchPercentage;

		// Token: 0x04000C7A RID: 3194
		private float _desiredReinforcementPercentage;

		// Token: 0x04000C7B RID: 3195
		private float _reinforcementWavePercentage;

		// Token: 0x04000C7C RID: 3196
		private int _maximumReinforcementWaveCount;

		// Token: 0x04000C7D RID: 3197
		private float _defenderReinforcementBatchPercentage;

		// Token: 0x04000C7E RID: 3198
		private float _attackerReinforcementBatchPercentage;

		// Token: 0x02000583 RID: 1411
		public enum ReinforcementSpawnMethod
		{
			// Token: 0x04001D6A RID: 7530
			Balanced,
			// Token: 0x04001D6B RID: 7531
			Wave,
			// Token: 0x04001D6C RID: 7532
			Fixed
		}

		// Token: 0x02000584 RID: 1412
		public enum ReinforcementTimingMethod
		{
			// Token: 0x04001D6E RID: 7534
			GlobalTimer,
			// Token: 0x04001D6F RID: 7535
			CustomTimer
		}

		// Token: 0x02000585 RID: 1413
		public enum InitialSpawnMethod
		{
			// Token: 0x04001D71 RID: 7537
			BattleSizeAllocating,
			// Token: 0x04001D72 RID: 7538
			FreeAllocation
		}
	}
}
