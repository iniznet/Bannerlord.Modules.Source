using System;

namespace TaleWorlds.Engine.Options
{
	// Token: 0x020000A0 RID: 160
	public class NativeNumericOptionData : NativeOptionData, INumericOptionData, IOptionData
	{
		// Token: 0x06000BBA RID: 3002 RVA: 0x0000CFFC File Offset: 0x0000B1FC
		public NativeNumericOptionData(NativeOptions.NativeOptionsType type)
			: base(type)
		{
			this._minValue = NativeNumericOptionData.GetLimitValue(this.Type, true);
			this._maxValue = NativeNumericOptionData.GetLimitValue(this.Type, false);
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x0000D029 File Offset: 0x0000B229
		public float GetMinValue()
		{
			return this._minValue;
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x0000D031 File Offset: 0x0000B231
		public float GetMaxValue()
		{
			return this._maxValue;
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x0000D03C File Offset: 0x0000B23C
		private static float GetLimitValue(NativeOptions.NativeOptionsType type, bool isMin)
		{
			if (type <= NativeOptions.NativeOptionsType.Brightness)
			{
				switch (type)
				{
				case NativeOptions.NativeOptionsType.MouseSensitivity:
					if (!isMin)
					{
						return 1f;
					}
					return 0.3f;
				case NativeOptions.NativeOptionsType.InvertMouseYAxis:
					break;
				case NativeOptions.NativeOptionsType.MouseYMovementScale:
					if (!isMin)
					{
						return 4f;
					}
					return 0.25f;
				case NativeOptions.NativeOptionsType.TrailAmount:
					if (!isMin)
					{
						return 1f;
					}
					return 0f;
				default:
					switch (type)
					{
					case NativeOptions.NativeOptionsType.ResolutionScale:
						if (!isMin)
						{
							return 100f;
						}
						return 50f;
					case NativeOptions.NativeOptionsType.FrameLimiter:
						if (!isMin)
						{
							return 360f;
						}
						return 30f;
					case NativeOptions.NativeOptionsType.Brightness:
						if (!isMin)
						{
							return 100f;
						}
						return 0f;
					}
					break;
				}
			}
			else if (type != NativeOptions.NativeOptionsType.SharpenAmount)
			{
				switch (type)
				{
				case NativeOptions.NativeOptionsType.BrightnessMin:
					if (!isMin)
					{
						return 0.3f;
					}
					return 0f;
				case NativeOptions.NativeOptionsType.BrightnessMax:
					if (!isMin)
					{
						return 1f;
					}
					return 0.7f;
				case NativeOptions.NativeOptionsType.ExposureCompensation:
					if (!isMin)
					{
						return 2f;
					}
					return -2f;
				case NativeOptions.NativeOptionsType.DynamicResolutionTarget:
					if (!isMin)
					{
						return 240f;
					}
					return 30f;
				}
			}
			else
			{
				if (!isMin)
				{
					return 100f;
				}
				return 0f;
			}
			if (!isMin)
			{
				return 1f;
			}
			return 0f;
		}

		// Token: 0x06000BBE RID: 3006 RVA: 0x0000D168 File Offset: 0x0000B368
		public bool GetIsDiscrete()
		{
			NativeOptions.NativeOptionsType type = this.Type;
			if (type <= NativeOptions.NativeOptionsType.Brightness)
			{
				if (type - NativeOptions.NativeOptionsType.ResolutionScale > 1 && type != NativeOptions.NativeOptionsType.Brightness)
				{
					return false;
				}
			}
			else if (type != NativeOptions.NativeOptionsType.SharpenAmount)
			{
				switch (type)
				{
				case NativeOptions.NativeOptionsType.BrightnessMin:
				case NativeOptions.NativeOptionsType.BrightnessMax:
				case NativeOptions.NativeOptionsType.ExposureCompensation:
				case NativeOptions.NativeOptionsType.DynamicResolutionTarget:
					break;
				case NativeOptions.NativeOptionsType.BrightnessCalibrated:
				case NativeOptions.NativeOptionsType.DynamicResolution:
					return false;
				default:
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x0000D1BC File Offset: 0x0000B3BC
		public int GetDiscreteIncrementInterval()
		{
			NativeOptions.NativeOptionsType type = this.Type;
			if (type == NativeOptions.NativeOptionsType.SharpenAmount)
			{
				return 5;
			}
			return 1;
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x0000D1D8 File Offset: 0x0000B3D8
		public bool GetShouldUpdateContinuously()
		{
			return true;
		}

		// Token: 0x040001F7 RID: 503
		private readonly float _minValue;

		// Token: 0x040001F8 RID: 504
		private readonly float _maxValue;
	}
}
