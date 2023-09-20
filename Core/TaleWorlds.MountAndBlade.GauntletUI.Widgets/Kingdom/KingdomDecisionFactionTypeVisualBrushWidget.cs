using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	public class KingdomDecisionFactionTypeVisualBrushWidget : BrushWidget
	{
		public KingdomDecisionFactionTypeVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void SetVisualState(string type)
		{
			this.RegisterBrushStatesOfWidget();
			this.SetState(type);
		}

		[Editor(false)]
		public string FactionName
		{
			get
			{
				return this._factionName;
			}
			set
			{
				if (this._factionName != value)
				{
					this._factionName = value;
					base.OnPropertyChanged<string>(value, "FactionName");
					if (value != null)
					{
						this.SetVisualState(value);
					}
				}
			}
		}

		private string _factionName = "";
	}
}
