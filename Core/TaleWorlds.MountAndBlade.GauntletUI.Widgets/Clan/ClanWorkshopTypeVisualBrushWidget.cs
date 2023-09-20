using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Clan
{
	public class ClanWorkshopTypeVisualBrushWidget : BrushWidget
	{
		public ClanWorkshopTypeVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void SetVisualState(string type)
		{
			this.RegisterBrushStatesOfWidget();
			this.SetState(type);
		}

		[Editor(false)]
		public string WorkshopType
		{
			get
			{
				return this._workshopType;
			}
			set
			{
				if (this._workshopType != value)
				{
					this._workshopType = value;
					base.OnPropertyChanged<string>(value, "WorkshopType");
					this.SetVisualState(value);
				}
			}
		}

		private string _workshopType = "";
	}
}
