using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class FiefProfitTypeVisualBrushWidget : BrushWidget
	{
		public FiefProfitTypeVisualBrushWidget(UIContext context)
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
				this.SetState("None");
				return;
			case 1:
				this.SetState("Tax");
				return;
			case 2:
				this.SetState("Tariff");
				return;
			case 3:
				this.SetState("Garrison");
				return;
			case 4:
				this.SetState("Village");
				return;
			case 5:
				this.SetState("Governor");
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
