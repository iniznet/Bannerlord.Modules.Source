using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	public class DescriptionItemVisualBrushWidget : BrushWidget
	{
		public DescriptionItemVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._determinedVisual)
			{
				this.RegisterBrushStatesOfWidget();
				this.UpdateVisual(this.Type);
				this._determinedVisual = true;
			}
		}

		private void UpdateVisual(int type)
		{
			switch (type)
			{
			case 0:
				this.SetState("Gold");
				return;
			case 1:
				this.SetState("Production");
				return;
			case 2:
				this.SetState("Militia");
				return;
			case 3:
				this.SetState("Prosperity");
				return;
			case 4:
				this.SetState("Food");
				return;
			case 5:
				this.SetState("Loyalty");
				return;
			case 6:
				this.SetState("Security");
				return;
			case 7:
				this.SetState("Garrison");
				return;
			default:
				return;
			}
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
				}
			}
		}

		private bool _determinedVisual;

		private int _type = -1;
	}
}
