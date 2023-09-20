using System;

namespace TaleWorlds.Library
{
	public abstract class TooltipVM : ViewModel
	{
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
					this.OnIsExtendedChanged();
				}
			}
		}

		public TooltipVM()
		{
			InformationManager.OnShowTooltip += this.OnShowTooltip;
			InformationManager.OnHideTooltip += this.OnHideTooltip;
			InformationManager.RegisterIsAnyTooltipActiveCallback(new Func<bool>(this.IsAnyTooltipActive));
			InformationManager.RegisterIsAnyTooltipExtendedCallback(new Func<bool>(this.IsAnyTooltipExtended));
		}

		public override void OnFinalize()
		{
			InformationManager.OnShowTooltip -= this.OnShowTooltip;
			InformationManager.OnHideTooltip -= this.OnHideTooltip;
			InformationManager.UnregisterIsAnyTooltipActiveCallback(new Func<bool>(this.IsAnyTooltipActive));
			InformationManager.UnregisterIsAnyTooltipExtendedCallback(new Func<bool>(this.IsAnyTooltipExtended));
		}

		public virtual void Tick(float dt)
		{
		}

		public virtual void OnShowTooltip(Type type, object[] args)
		{
		}

		public virtual void OnHideTooltip()
		{
		}

		protected virtual void OnIsExtendedChanged()
		{
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

		private bool _isExtended;

		private bool _isActive;
	}
}
