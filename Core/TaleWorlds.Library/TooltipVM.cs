using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200003B RID: 59
	public abstract class TooltipVM : ViewModel
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x000069A7 File Offset: 0x00004BA7
		// (set) Token: 0x060001D6 RID: 470 RVA: 0x000069AF File Offset: 0x00004BAF
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

		// Token: 0x060001D7 RID: 471 RVA: 0x000069C8 File Offset: 0x00004BC8
		public TooltipVM()
		{
			InformationManager.OnShowTooltip += this.OnShowTooltip;
			InformationManager.OnHideTooltip += this.OnHideTooltip;
			InformationManager.RegisterIsAnyTooltipActiveCallback(new Func<bool>(this.IsAnyTooltipActive));
			InformationManager.RegisterIsAnyTooltipExtendedCallback(new Func<bool>(this.IsAnyTooltipExtended));
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00006A24 File Offset: 0x00004C24
		public override void OnFinalize()
		{
			InformationManager.OnShowTooltip -= this.OnShowTooltip;
			InformationManager.OnHideTooltip -= this.OnHideTooltip;
			InformationManager.UnregisterIsAnyTooltipActiveCallback(new Func<bool>(this.IsAnyTooltipActive));
			InformationManager.UnregisterIsAnyTooltipExtendedCallback(new Func<bool>(this.IsAnyTooltipExtended));
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00006A77 File Offset: 0x00004C77
		public virtual void Tick(float dt)
		{
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00006A79 File Offset: 0x00004C79
		public virtual void OnShowTooltip(Type type, object[] args)
		{
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00006A7B File Offset: 0x00004C7B
		public virtual void OnHideTooltip()
		{
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00006A7D File Offset: 0x00004C7D
		protected virtual void OnIsExtendedChanged()
		{
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00006A7F File Offset: 0x00004C7F
		private bool IsAnyTooltipActive()
		{
			return this.IsActive;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00006A87 File Offset: 0x00004C87
		private bool IsAnyTooltipExtended()
		{
			return this.IsExtended;
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060001DF RID: 479 RVA: 0x00006A8F File Offset: 0x00004C8F
		// (set) Token: 0x060001E0 RID: 480 RVA: 0x00006A97 File Offset: 0x00004C97
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

		// Token: 0x040000BB RID: 187
		private bool _isExtended;

		// Token: 0x040000BC RID: 188
		private bool _isActive;
	}
}
