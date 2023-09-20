using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	// Token: 0x02000113 RID: 275
	public class KingdomClanTypeVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000E00 RID: 3584 RVA: 0x0002732E File Offset: 0x0002552E
		public KingdomClanTypeVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E01 RID: 3585 RVA: 0x00027340 File Offset: 0x00025540
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

		// Token: 0x170004F9 RID: 1273
		// (get) Token: 0x06000E02 RID: 3586 RVA: 0x000273A1 File Offset: 0x000255A1
		// (set) Token: 0x06000E03 RID: 3587 RVA: 0x000273A9 File Offset: 0x000255A9
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

		// Token: 0x04000673 RID: 1651
		private int _type = -1;
	}
}
