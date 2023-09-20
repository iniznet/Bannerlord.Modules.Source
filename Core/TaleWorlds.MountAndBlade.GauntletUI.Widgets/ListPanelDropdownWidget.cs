using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ListPanelDropdownWidget : DropdownWidget
	{
		public ListPanelDropdownWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OpenPanel()
		{
			base.OpenPanel();
			if (this.ListPanelContainer != null)
			{
				this.ListPanelContainer.IsVisible = true;
			}
			base.Button.IsSelected = true;
		}

		protected override void ClosePanel()
		{
			if (this.ListPanelContainer != null)
			{
				this.ListPanelContainer.IsVisible = false;
			}
			base.Button.IsSelected = false;
			base.ClosePanel();
		}

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

		private Widget _listPanelContainer;
	}
}
