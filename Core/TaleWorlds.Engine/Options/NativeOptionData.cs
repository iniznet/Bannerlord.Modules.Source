using System;

namespace TaleWorlds.Engine.Options
{
	// Token: 0x020000A1 RID: 161
	public abstract class NativeOptionData : IOptionData
	{
		// Token: 0x06000BC1 RID: 3009 RVA: 0x0000D1DB File Offset: 0x0000B3DB
		protected NativeOptionData(NativeOptions.NativeOptionsType type)
		{
			this.Type = type;
			this._value = NativeOptions.GetConfig(type);
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x0000D1F6 File Offset: 0x0000B3F6
		public virtual float GetDefaultValue()
		{
			return NativeOptions.GetDefaultConfig(this.Type);
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x0000D203 File Offset: 0x0000B403
		public void Commit()
		{
			NativeOptions.SetConfig(this.Type, this._value);
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x0000D216 File Offset: 0x0000B416
		public float GetValue(bool forceRefresh)
		{
			if (forceRefresh)
			{
				this._value = NativeOptions.GetConfig(this.Type);
			}
			return this._value;
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x0000D232 File Offset: 0x0000B432
		public void SetValue(float value)
		{
			this._value = value;
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x0000D23B File Offset: 0x0000B43B
		public object GetOptionType()
		{
			return this.Type;
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x0000D248 File Offset: 0x0000B448
		public bool IsNative()
		{
			return true;
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x0000D24B File Offset: 0x0000B44B
		public bool IsAction()
		{
			return false;
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x0000D250 File Offset: 0x0000B450
		public ValueTuple<string, bool> GetIsDisabledAndReasonID()
		{
			NativeOptions.NativeOptionsType type = this.Type;
			if (type <= NativeOptions.NativeOptionsType.DLSS)
			{
				if (type != NativeOptions.NativeOptionsType.ResolutionScale)
				{
					if (type == NativeOptions.NativeOptionsType.DLSS)
					{
						if (!NativeOptions.GetIsDLSSAvailable())
						{
							return new ValueTuple<string, bool>("str_dlss_not_available", true);
						}
					}
				}
				else
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
			else if (type != NativeOptions.NativeOptionsType.DynamicResolution)
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
			return new ValueTuple<string, bool>(string.Empty, false);
		}

		// Token: 0x040001F9 RID: 505
		public readonly NativeOptions.NativeOptionsType Type;

		// Token: 0x040001FA RID: 506
		private float _value;
	}
}
