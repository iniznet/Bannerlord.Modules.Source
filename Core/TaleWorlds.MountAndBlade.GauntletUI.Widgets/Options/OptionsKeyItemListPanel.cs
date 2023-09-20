using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options
{
	// Token: 0x0200006B RID: 107
	public class OptionsKeyItemListPanel : ListPanel
	{
		// Token: 0x060005C9 RID: 1481 RVA: 0x000114FA File Offset: 0x0000F6FA
		public OptionsKeyItemListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00011504 File Offset: 0x0000F704
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._screenWidget == null)
			{
				this._screenWidget = base.EventManager.Root.GetChild(0).FindChild("Options") as OptionsScreenWidget;
			}
			if (!this._eventsRegistered)
			{
				this.RegisterHoverEvents();
				this._eventsRegistered = true;
			}
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x0001155B File Offset: 0x0000F75B
		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this.SetCurrentOption(false, false, -1);
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x0001156C File Offset: 0x0000F76C
		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this.ResetCurrentOption();
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x0001157A File Offset: 0x0000F77A
		private void SetCurrentOption(bool fromHoverOverDropdown, bool fromBooleanSelection, int hoverDropdownItemIndex = -1)
		{
			OptionsScreenWidget screenWidget = this._screenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentOption(this, null);
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x0001158E File Offset: 0x0000F78E
		private void ResetCurrentOption()
		{
			OptionsScreenWidget screenWidget = this._screenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentOption(null, null);
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x000115A4 File Offset: 0x0000F7A4
		private void RegisterHoverEvents()
		{
			foreach (Widget widget in base.AllChildren)
			{
				widget.boolPropertyChanged += this.Child_PropertyChanged;
			}
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x000115FC File Offset: 0x0000F7FC
		private void Child_PropertyChanged(PropertyOwnerObject childWidget, string propertyName, bool propertyValue)
		{
			if (propertyName == "IsHovered")
			{
				if (propertyValue)
				{
					this.SetCurrentOption(false, false, -1);
					return;
				}
				this.ResetCurrentOption();
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x0001161E File Offset: 0x0000F81E
		// (set) Token: 0x060005D2 RID: 1490 RVA: 0x00011626 File Offset: 0x0000F826
		public string OptionTitle
		{
			get
			{
				return this._optionTitle;
			}
			set
			{
				if (this._optionTitle != value)
				{
					this._optionTitle = value;
				}
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x0001163D File Offset: 0x0000F83D
		// (set) Token: 0x060005D4 RID: 1492 RVA: 0x00011645 File Offset: 0x0000F845
		public string OptionDescription
		{
			get
			{
				return this._optionDescription;
			}
			set
			{
				if (this._optionDescription != value)
				{
					this._optionDescription = value;
				}
			}
		}

		// Token: 0x0400027F RID: 639
		private OptionsScreenWidget _screenWidget;

		// Token: 0x04000280 RID: 640
		private bool _eventsRegistered;

		// Token: 0x04000281 RID: 641
		private string _optionDescription;

		// Token: 0x04000282 RID: 642
		private string _optionTitle;
	}
}
