using System;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade.Options.ManagedOptions
{
	public class ManagedNumericOptionData : ManagedOptionData, INumericOptionData, IOptionData
	{
		public ManagedNumericOptionData(ManagedOptions.ManagedOptionsType type)
			: base(type)
		{
			this._minValue = ManagedNumericOptionData.GetLimitValue(this.Type, true);
			this._maxValue = ManagedNumericOptionData.GetLimitValue(this.Type, false);
		}

		public float GetMinValue()
		{
			return this._minValue;
		}

		public float GetMaxValue()
		{
			return this._maxValue;
		}

		private static float GetLimitValue(ManagedOptions.ManagedOptionsType type, bool isMin)
		{
			if (type <= ManagedOptions.ManagedOptionsType.AutoSaveInterval)
			{
				if (type == ManagedOptions.ManagedOptionsType.BattleSize)
				{
					return (float)(isMin ? BannerlordConfig.MinBattleSize : BannerlordConfig.MaxBattleSize);
				}
				if (type == ManagedOptions.ManagedOptionsType.AutoSaveInterval)
				{
					if (!isMin)
					{
						return 60f;
					}
					return 4f;
				}
			}
			else if (type != ManagedOptions.ManagedOptionsType.FirstPersonFov)
			{
				if (type != ManagedOptions.ManagedOptionsType.CombatCameraDistance)
				{
					if (type == ManagedOptions.ManagedOptionsType.UIScale)
					{
						if (!isMin)
						{
							return 1f;
						}
						return 0.75f;
					}
				}
				else
				{
					if (!isMin)
					{
						return 2.4f;
					}
					return 0.7f;
				}
			}
			else
			{
				if (!isMin)
				{
					return 100f;
				}
				return 45f;
			}
			if (!isMin)
			{
				return 1f;
			}
			return 0f;
		}

		public bool GetIsDiscrete()
		{
			ManagedOptions.ManagedOptionsType type = this.Type;
			if (type <= ManagedOptions.ManagedOptionsType.AutoSaveInterval)
			{
				if (type != ManagedOptions.ManagedOptionsType.BattleSize && type != ManagedOptions.ManagedOptionsType.AutoSaveInterval)
				{
					return false;
				}
			}
			else if (type != ManagedOptions.ManagedOptionsType.FirstPersonFov)
			{
				if (type != ManagedOptions.ManagedOptionsType.UIScale)
				{
					return false;
				}
				return false;
			}
			return true;
		}

		public int GetDiscreteIncrementInterval()
		{
			return 1;
		}

		public bool GetShouldUpdateContinuously()
		{
			ManagedOptions.ManagedOptionsType type = this.Type;
			return type != ManagedOptions.ManagedOptionsType.UIScale;
		}

		private readonly float _minValue;

		private readonly float _maxValue;
	}
}
