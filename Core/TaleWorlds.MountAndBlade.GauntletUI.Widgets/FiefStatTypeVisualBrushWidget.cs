using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200001A RID: 26
	public class FiefStatTypeVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000126 RID: 294 RVA: 0x00005419 File Offset: 0x00003619
		public FiefStatTypeVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00005429 File Offset: 0x00003629
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

		// Token: 0x06000128 RID: 296 RVA: 0x00005454 File Offset: 0x00003654
		private void UpdateVisual(int type)
		{
			switch (type)
			{
			case 0:
				this.SetState("None");
				return;
			case 1:
				this.SetState("Wall");
				return;
			case 2:
				this.SetState("Garrison");
				return;
			case 3:
				this.SetState("Militia");
				return;
			case 4:
				this.SetState("Prosperity");
				return;
			case 5:
				this.SetState("Food");
				return;
			case 6:
				this.SetState("Loyalty");
				return;
			case 7:
				this.SetState("Security");
				return;
			default:
				return;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000129 RID: 297 RVA: 0x000054E7 File Offset: 0x000036E7
		// (set) Token: 0x0600012A RID: 298 RVA: 0x000054EF File Offset: 0x000036EF
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

		// Token: 0x0400008C RID: 140
		private bool _determinedVisual;

		// Token: 0x0400008D RID: 141
		private int _type = -1;
	}
}
