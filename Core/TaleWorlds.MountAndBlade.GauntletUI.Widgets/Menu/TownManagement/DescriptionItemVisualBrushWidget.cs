using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Menu.TownManagement
{
	// Token: 0x020000E9 RID: 233
	public class DescriptionItemVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000C10 RID: 3088 RVA: 0x00021B3A File Offset: 0x0001FD3A
		public DescriptionItemVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x00021B4A File Offset: 0x0001FD4A
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

		// Token: 0x06000C12 RID: 3090 RVA: 0x00021B74 File Offset: 0x0001FD74
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

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06000C13 RID: 3091 RVA: 0x00021C07 File Offset: 0x0001FE07
		// (set) Token: 0x06000C14 RID: 3092 RVA: 0x00021C0F File Offset: 0x0001FE0F
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

		// Token: 0x04000591 RID: 1425
		private bool _determinedVisual;

		// Token: 0x04000592 RID: 1426
		private int _type = -1;
	}
}
