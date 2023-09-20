using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	public class KingdomClanTypeVisualBrushWidget : BrushWidget
	{
		public KingdomClanTypeVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateTypeVisual()
		{
			if (this.Type == 0)
			{
				this.SetState("Normal");
				return;
			}
			if (this.Type == 1)
			{
				this.SetState("Leader");
				return;
			}
			if (this.Type == 2)
			{
				this.SetState("Mercenary");
				return;
			}
			Debug.FailedAssert("This clan type is not defined in widget", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Kingdom\\KingdomClanTypeVisualBrushWidget.cs", "UpdateTypeVisual", 37);
		}

		[Editor(false)]
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (this._type != value)
				{
					this._type = value;
					base.OnPropertyChanged(value, "Type");
					this.UpdateTypeVisual();
				}
			}
		}

		private int _type = -1;
	}
}
