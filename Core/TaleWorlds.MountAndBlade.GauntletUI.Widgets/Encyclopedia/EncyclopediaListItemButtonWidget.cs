using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Encyclopedia
{
	public class EncyclopediaListItemButtonWidget : ButtonWidget
	{
		public TextWidget ListItemNameTextWidget { get; set; }

		public TextWidget ListComparedValueTextWidget { get; set; }

		public Brush InfoAvailableItemNameBrush { get; set; }

		public Brush InfoUnvailableItemNameBrush { get; set; }

		public bool IsInfoAvailable { get; set; }

		public EncyclopediaListItemButtonWidget(UIContext context)
			: base(context)
		{
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.OnThisLateUpdate), 1);
		}

		public void OnThisLateUpdate(float dt)
		{
			this.ListItemNameTextWidget.Brush = (this.IsInfoAvailable ? this.InfoAvailableItemNameBrush : this.InfoUnvailableItemNameBrush);
			this.ListComparedValueTextWidget.Brush = (this.IsInfoAvailable ? this.InfoAvailableItemNameBrush : this.InfoUnvailableItemNameBrush);
		}

		[Editor(false)]
		public string ListItemId
		{
			get
			{
				return this._listItemId;
			}
			set
			{
				if (this._listItemId != value)
				{
					this._listItemId = value;
					base.OnPropertyChanged<string>(value, "ListItemId");
				}
			}
		}

		private string _listItemId;
	}
}
