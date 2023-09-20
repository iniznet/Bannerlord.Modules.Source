using System;

namespace TaleWorlds.Library
{
	public abstract class TooltipBaseVM : ViewModel
	{
		public TooltipBaseVM(Type invokedType, object[] invokedArgs)
		{
			this._invokedType = invokedType;
			this._invokedArgs = invokedArgs;
			this.RegisterCallbacks();
		}

		public override void OnFinalize()
		{
			this.UnregisterCallbacks();
			this.OnFinalizeInternal();
		}

		protected virtual void OnFinalizeInternal()
		{
		}

		public virtual void Tick(float dt)
		{
			if (this.IsActive && this._isPeriodicRefreshEnabled)
			{
				this._periodicRefreshTimer -= dt;
				if (this._periodicRefreshTimer < 0f)
				{
					this.OnPeriodicRefresh();
					this._periodicRefreshTimer = this._periodicRefreshDelay;
					return;
				}
			}
			else
			{
				this._periodicRefreshTimer = this._periodicRefreshDelay;
			}
		}

		protected void InvokeRefreshData<T>(T tooltip) where T : TooltipBaseVM
		{
			ValueTuple<Type, object, string> valueTuple;
			Action<T, object[]> action;
			if (InformationManager.RegisteredTypes.TryGetValue(this._invokedType, out valueTuple) && (action = valueTuple.Item2 as Action<T, object[]>) != null)
			{
				action(tooltip, this._invokedArgs);
			}
		}

		protected virtual void OnPeriodicRefresh()
		{
		}

		protected virtual void OnIsExtendedChanged()
		{
		}

		private void RegisterCallbacks()
		{
			InformationManager.RegisterIsAnyTooltipActiveCallback(new Func<bool>(this.IsAnyTooltipActive));
			InformationManager.RegisterIsAnyTooltipExtendedCallback(new Func<bool>(this.IsAnyTooltipExtended));
		}

		private void UnregisterCallbacks()
		{
			InformationManager.UnregisterIsAnyTooltipActiveCallback(new Func<bool>(this.IsAnyTooltipActive));
			InformationManager.UnregisterIsAnyTooltipExtendedCallback(new Func<bool>(this.IsAnyTooltipExtended));
		}

		private bool IsAnyTooltipActive()
		{
			return this.IsActive;
		}

		private bool IsAnyTooltipExtended()
		{
			return this.IsExtended;
		}

		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		[DataSourceProperty]
		public bool IsExtended
		{
			get
			{
				return this._isExtended;
			}
			set
			{
				if (this._isExtended != value)
				{
					this._isExtended = value;
					base.OnPropertyChangedWithValue(value, "IsExtended");
					this.OnIsExtendedChanged();
				}
			}
		}

		protected readonly Type _invokedType;

		protected readonly object[] _invokedArgs;

		protected bool _isPeriodicRefreshEnabled;

		protected float _periodicRefreshDelay;

		private float _periodicRefreshTimer;

		private bool _isActive;

		private bool _isExtended;
	}
}
