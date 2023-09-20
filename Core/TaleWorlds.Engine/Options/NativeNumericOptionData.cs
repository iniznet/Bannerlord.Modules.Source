using System;

namespace TaleWorlds.Engine.Options
{
	public class NativeNumericOptionData : NativeOptionData, INumericOptionData, IOptionData
	{
		public NativeNumericOptionData(NativeOptions.NativeOptionsType type)
			: base(type)
		{
			this._minValue = NativeNumericOptionData.GetLimitValue(this.Type, true);
			this._maxValue = NativeNumericOptionData.GetLimitValue(this.Type, false);
		}

		public float GetMinValue()
		{
			return this._minValue;
		}

		public float GetMaxValue()
		{
			return this._maxValue;
		}

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
				case NativeOptions.NativeOptionsType.EnableVibration:
				case NativeOptions.NativeOptionsType.EnableGyroAssistedAim:
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
				case NativeOptions.NativeOptionsType.GyroAimSensitivity:
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

		public int GetDiscreteIncrementInterval()
		{
			NativeOptions.NativeOptionsType type = this.Type;
			if (type == NativeOptions.NativeOptionsType.SharpenAmount)
			{
				return 5;
			}
			return 1;
		}

		public bool GetShouldUpdateContinuously()
		{
			return true;
		}

		private readonly float _minValue;

		private readonly float _maxValue;
	}
}
