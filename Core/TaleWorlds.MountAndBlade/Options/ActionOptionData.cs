using System;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade.Options
{
	public class ActionOptionData : IOptionData
	{
		public Action OnAction { get; private set; }

		public ActionOptionData(ManagedOptions.ManagedOptionsType managedType, Action onAction)
		{
			this._managedType = managedType;
			this.OnAction = onAction;
		}

		public ActionOptionData(NativeOptions.NativeOptionsType nativeType, Action onAction)
		{
			this._nativeType = nativeType;
			this.OnAction = onAction;
		}

		public ActionOptionData(string optionTypeId, Action onAction)
		{
			this._actionOptionTypeId = optionTypeId;
			this._nativeType = NativeOptions.NativeOptionsType.None;
			this.OnAction = onAction;
		}

		public void Commit()
		{
		}

		public float GetDefaultValue()
		{
			return 0f;
		}

		public object GetOptionType()
		{
			if (this._nativeType != NativeOptions.NativeOptionsType.None)
			{
				return this._nativeType;
			}
			if (this._managedType != ManagedOptions.ManagedOptionsType.Language)
			{
				return this._managedType;
			}
			return this._actionOptionTypeId;
		}

		public float GetValue(bool forceRefresh)
		{
			return 0f;
		}

		public bool IsNative()
		{
			return this._nativeType != NativeOptions.NativeOptionsType.None;
		}

		public void SetValue(float value)
		{
		}

		public bool IsAction()
		{
			return this._nativeType == NativeOptions.NativeOptionsType.None && this._managedType == ManagedOptions.ManagedOptionsType.Language;
		}

		public ValueTuple<string, bool> GetIsDisabledAndReasonID()
		{
			return new ValueTuple<string, bool>(string.Empty, false);
		}

		private ManagedOptions.ManagedOptionsType _managedType;

		private NativeOptions.NativeOptionsType _nativeType;

		private string _actionOptionTypeId;
	}
}
