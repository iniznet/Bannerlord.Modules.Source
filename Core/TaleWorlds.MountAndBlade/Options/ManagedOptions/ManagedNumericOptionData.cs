using System;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade.Options.ManagedOptions
{
	// Token: 0x0200039B RID: 923
	public class ManagedNumericOptionData : ManagedOptionData, INumericOptionData, IOptionData
	{
		// Token: 0x0600327F RID: 12927 RVA: 0x000D137C File Offset: 0x000CF57C
		public ManagedNumericOptionData(ManagedOptions.ManagedOptionsType type)
			: base(type)
		{
			this._minValue = ManagedNumericOptionData.GetLimitValue(this.Type, true);
			this._maxValue = ManagedNumericOptionData.GetLimitValue(this.Type, false);
		}

		// Token: 0x06003280 RID: 12928 RVA: 0x000D13A9 File Offset: 0x000CF5A9
		public float GetMinValue()
		{
			return this._minValue;
		}

		// Token: 0x06003281 RID: 12929 RVA: 0x000D13B1 File Offset: 0x000CF5B1
		public float GetMaxValue()
		{
			return this._maxValue;
		}

		// Token: 0x06003282 RID: 12930 RVA: 0x000D13BC File Offset: 0x000CF5BC
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

		// Token: 0x06003283 RID: 12931 RVA: 0x000D1448 File Offset: 0x000CF648
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

		// Token: 0x06003284 RID: 12932 RVA: 0x000D147B File Offset: 0x000CF67B
		public int GetDiscreteIncrementInterval()
		{
			return 1;
		}

		// Token: 0x06003285 RID: 12933 RVA: 0x000D1480 File Offset: 0x000CF680
		public bool GetShouldUpdateContinuously()
		{
			ManagedOptions.ManagedOptionsType type = this.Type;
			return type != ManagedOptions.ManagedOptionsType.UIScale;
		}

		// Token: 0x0400154C RID: 5452
		private readonly float _minValue;

		// Token: 0x0400154D RID: 5453
		private readonly float _maxValue;
	}
}
