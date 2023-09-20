using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public struct MissionSpawnSettings
	{
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

		public MissionSpawnSettings.InitialSpawnMethod InitialTroopsSpawnMethod { get; private set; }

		public MissionSpawnSettings.ReinforcementTimingMethod ReinforcementTroopsTimingMethod { get; private set; }

		public MissionSpawnSettings.ReinforcementSpawnMethod ReinforcementTroopsSpawnMethod { get; private set; }

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

		public static MissionSpawnSettings CreateDefaultSpawnSettings()
		{
			return new MissionSpawnSettings(MissionSpawnSettings.InitialSpawnMethod.BattleSizeAllocating, MissionSpawnSettings.ReinforcementTimingMethod.GlobalTimer, MissionSpawnSettings.ReinforcementSpawnMethod.Balanced, 10f, 0.05f, 0.166f, 0f, 0, 0f, 0f, 1f, 0.75f);
		}

		public const float MinimumReinforcementInterval = 1f;

		public const float MinimumDefenderAdvantageFactor = 0.1f;

		public const float MaximumDefenderAdvantageFactor = 10f;

		public const float MinimumBattleSizeRatioLimit = 0.5f;

		public const float MaximumBattleSizeRatioLimit = 0.99f;

		public const float DefaultMaximumBattleSizeRatio = 0.75f;

		public const float DefaultDefenderAdvantageFactor = 1f;

		private float _globalReinforcementInterval;

		private float _defenderAdvantageFactor;

		private float _maximumBattleSizeRatio;

		private float _reinforcementBatchPercentage;

		private float _desiredReinforcementPercentage;

		private float _reinforcementWavePercentage;

		private int _maximumReinforcementWaveCount;

		private float _defenderReinforcementBatchPercentage;

		private float _attackerReinforcementBatchPercentage;

		public enum ReinforcementSpawnMethod
		{
			Balanced,
			Wave,
			Fixed
		}

		public enum ReinforcementTimingMethod
		{
			GlobalTimer,
			CustomTimer
		}

		public enum InitialSpawnMethod
		{
			BattleSizeAllocating,
			FreeAllocation
		}
	}
}
