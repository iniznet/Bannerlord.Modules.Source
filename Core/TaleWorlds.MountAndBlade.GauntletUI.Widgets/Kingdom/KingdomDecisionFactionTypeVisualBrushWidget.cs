using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	// Token: 0x02000114 RID: 276
	public class KingdomDecisionFactionTypeVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000E04 RID: 3588 RVA: 0x000273CD File Offset: 0x000255CD
		public KingdomDecisionFactionTypeVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E05 RID: 3589 RVA: 0x000273E1 File Offset: 0x000255E1
		private void SetVisualState(string type)
		{
			this.RegisterBrushStatesOfWidget();
			this.SetState(type);
		}

		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06000E06 RID: 3590 RVA: 0x000273F0 File Offset: 0x000255F0
		// (set) Token: 0x06000E07 RID: 3591 RVA: 0x000273F8 File Offset: 0x000255F8
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

		// Token: 0x04000674 RID: 1652
		private string _factionName = "";
	}
}
