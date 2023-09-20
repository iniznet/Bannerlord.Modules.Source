using System;

namespace TaleWorlds.Engine.Options
{
	public abstract class NativeOptionData : IOptionData
	{
		protected NativeOptionData(NativeOptions.NativeOptionsType type)
		{
			this.Type = type;
			this._value = NativeOptions.GetConfig(type);
		}

		public virtual float GetDefaultValue()
		{
			return NativeOptions.GetDefaultConfig(this.Type);
		}

		public void Commit()
		{
			NativeOptions.SetConfig(this.Type, this._value);
		}

		public float GetValue(bool forceRefresh)
		{
			if (forceRefresh)
			{
				this._value = NativeOptions.GetConfig(this.Type);
			}
			return this._value;
		}

		public void SetValue(float value)
		{
			this._value = value;
		}

		public object GetOptionType()
		{
			return this.Type;
		}

		public bool IsNative()
		{
			return true;
		}

		public bool IsAction()
		{
			return false;
		}

		public ValueTuple<string, bool> GetIsDisabledAndReasonID()
		{
			NativeOptions.NativeOptionsType type = this.Type;
			if (type <= NativeOptions.NativeOptionsType.ResolutionScale)
			{
				if (type != NativeOptions.NativeOptionsType.GyroAimSensitivity)
				{
					if (type == NativeOptions.NativeOptionsType.ResolutionScale)
					{
						if (NativeOptions.GetConfig(NativeOptions.NativeOptionsType.DLSS) != 0f)
						{
							return new ValueTuple<string, bool>("str_dlss_enabled", true);
						}
						if (NativeOptions.GetConfig(NativeOptions.NativeOptionsType.DynamicResolution) != 0f)
						{
							return new ValueTuple<string, bool>("str_dynamic_resolution_enabled", true);
						}
					}
				}
				else if (NativeOptions.GetConfig(NativeOptions.NativeOptionsType.EnableGyroAssistedAim) != 1f)
				{
					return new ValueTuple<string, bool>("str_gyro_disabled", true);
				}
			}
			else if (type != NativeOptions.NativeOptionsType.DLSS)
			{
				if (type != NativeOptions.NativeOptionsType.DynamicResolution)
				{
					if (type == NativeOptions.NativeOptionsType.DynamicResolutionTarget)
					{
						if (NativeOptions.GetConfig(NativeOptions.NativeOptionsType.DynamicResolution) == 0f)
						{
							return new ValueTuple<string, bool>("str_dynamic_resolution_disabled", true);
						}
						if (NativeOptions.GetConfig(NativeOptions.NativeOptionsType.DLSS) != 0f)
						{
							return new ValueTuple<string, bool>("str_dlss_enabled", true);
						}
					}
				}
				else if (NativeOptions.GetConfig(NativeOptions.NativeOptionsType.DLSS) != 0f)
				{
					return new ValueTuple<string, bool>("str_dlss_enabled", true);
				}
			}
			else if (!NativeOptions.GetIsDLSSAvailable())
			{
				return new ValueTuple<string, bool>("str_dlss_not_available", true);
			}
			return new ValueTuple<string, bool>(string.Empty, false);
		}

		public readonly NativeOptions.NativeOptionsType Type;

		private float _value;
	}
}
