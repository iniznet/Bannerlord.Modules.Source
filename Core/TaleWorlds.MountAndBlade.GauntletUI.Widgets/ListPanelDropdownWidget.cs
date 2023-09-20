using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000026 RID: 38
	public class ListPanelDropdownWidget : DropdownWidget
	{
		// Token: 0x060001D5 RID: 469 RVA: 0x000072F8 File Offset: 0x000054F8
		public ListPanelDropdownWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00007301 File Offset: 0x00005501
		protected override void OpenPanel()
		{
			base.OpenPanel();
			if (this.ListPanelContainer != null)
			{
				this.ListPanelContainer.IsVisible = true;
			}
			base.Button.IsSelected = true;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x00007329 File Offset: 0x00005529
		protected override void ClosePanel()
		{
			if (this.ListPanelContainer != null)
			{
				this.ListPanelContainer.IsVisible = false;
			}
			base.Button.IsSelected = false;
			base.ClosePanel();
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x00007351 File Offset: 0x00005551
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x00007359 File Offset: 0x00005559
		[Editor(false)]
		public Widget ListPanelContainer
		{
			get
			{
				return this._listPanelContainer;
			}
			set
			{
				if (this._listPanelContainer != value)
				{
					this._listPanelContainer = value;
					base.OnPropertyChanged<Widget>(value, "ListPanelContainer");
				}
			}
		}

		// Token: 0x040000E5 RID: 229
		private Widget _listPanelContainer;
	}
}
