using System;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade.Options.ManagedOptions
{
	public abstract class ManagedOptionData : IOptionData
	{
		protected ManagedOptionData(ManagedOptions.ManagedOptionsType type)
		{
			this.Type = type;
			this._value = ManagedOptions.GetConfig(type);
		}

		public virtual float GetDefaultValue()
		{
			return ManagedOptions.GetDefaultConfig(this.Type);
		}

		public void Commit()
		{
			if (this._value != ManagedOptions.GetConfig(this.Type))
			{
				ManagedOptions.SetConfig(this.Type, this._value);
			}
		}

		public float GetValue(bool forceRefresh)
		{
			if (forceRefresh)
			{
				this._value = ManagedOptions.GetConfig(this.Type);
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
			return false;
		}

		public bool IsAction()
		{
			return false;
		}

		public ValueTuple<string, bool> GetIsDisabledAndReasonID()
		{
			ManagedOptions.ManagedOptionsType type = this.Type;
			if (type - ManagedOptions.ManagedOptionsType.ControlBlockDirection <= 1 && BannerlordConfig.GyroOverrideForAttackDefend)
			{
				return new ValueTuple<string, bool>("str_gyro_overrides_attack_block_direction", true);
			}
			return new ValueTuple<string, bool>(string.Empty, false);
		}

		public readonly ManagedOptions.ManagedOptionsType Type;

		private float _value;
	}
}
