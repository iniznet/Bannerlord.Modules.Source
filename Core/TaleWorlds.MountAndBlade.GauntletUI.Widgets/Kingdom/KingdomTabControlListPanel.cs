using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Kingdom
{
	// Token: 0x02000117 RID: 279
	public class KingdomTabControlListPanel : ListPanel
	{
		// Token: 0x06000E2C RID: 3628 RVA: 0x0002783B File Offset: 0x00025A3B
		public KingdomTabControlListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000E2D RID: 3629 RVA: 0x00027844 File Offset: 0x00025A44
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			this.FiefsButton.IsSelected = this.FiefsPanel.IsVisible;
			this.PoliciesButton.IsSelected = this.PoliciesPanel.IsVisible;
			this.ClansButton.IsSelected = this.ClansPanel.IsVisible;
			this.ArmiesButton.IsSelected = this.ArmiesPanel.IsVisible;
			this.DiplomacyButton.IsSelected = this.DiplomacyPanel.IsVisible;
		}

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x06000E2E RID: 3630 RVA: 0x000278C6 File Offset: 0x00025AC6
		// (set) Token: 0x06000E2F RID: 3631 RVA: 0x000278CE File Offset: 0x00025ACE
		[Editor(false)]
		public Widget DiplomacyPanel
		{
			get
			{
				return this._diplomacyPanel;
			}
			set
			{
				if (this._diplomacyPanel != value)
				{
					this._diplomacyPanel = value;
					base.OnPropertyChanged<Widget>(value, "DiplomacyPanel");
				}
			}
		}

		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06000E30 RID: 3632 RVA: 0x000278EC File Offset: 0x00025AEC
		// (set) Token: 0x06000E31 RID: 3633 RVA: 0x000278F4 File Offset: 0x00025AF4
		[Editor(false)]
		public Widget ArmiesPanel
		{
			get
			{
				return this._armiesPanel;
			}
			set
			{
				if (this._armiesPanel != value)
				{
					this._armiesPanel = value;
					base.OnPropertyChanged<Widget>(value, "ArmiesPanel");
				}
			}
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06000E32 RID: 3634 RVA: 0x00027912 File Offset: 0x00025B12
		// (set) Token: 0x06000E33 RID: 3635 RVA: 0x0002791A File Offset: 0x00025B1A
		[Editor(false)]
		public Widget ClansPanel
		{
			get
			{
				return this._clansPanel;
			}
			set
			{
				if (this._clansPanel != value)
				{
					this._clansPanel = value;
					base.OnPropertyChanged<Widget>(value, "ClansPanel");
				}
			}
		}

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06000E34 RID: 3636 RVA: 0x00027938 File Offset: 0x00025B38
		// (set) Token: 0x06000E35 RID: 3637 RVA: 0x00027940 File Offset: 0x00025B40
		[Editor(false)]
		public Widget PoliciesPanel
		{
			get
			{
				return this._policiesPanel;
			}
			set
			{
				if (this._policiesPanel != value)
				{
					this._policiesPanel = value;
					base.OnPropertyChanged<Widget>(value, "PoliciesPanel");
				}
			}
		}

		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x06000E36 RID: 3638 RVA: 0x0002795E File Offset: 0x00025B5E
		// (set) Token: 0x06000E37 RID: 3639 RVA: 0x00027966 File Offset: 0x00025B66
		[Editor(false)]
		public Widget FiefsPanel
		{
			get
			{
				return this._fiefsPanel;
			}
			set
			{
				if (this._fiefsPanel != value)
				{
					this._fiefsPanel = value;
					base.OnPropertyChanged<Widget>(value, "FiefsPanel");
				}
			}
		}

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x06000E38 RID: 3640 RVA: 0x00027984 File Offset: 0x00025B84
		// (set) Token: 0x06000E39 RID: 3641 RVA: 0x0002798C File Offset: 0x00025B8C
		[Editor(false)]
		public ButtonWidget FiefsButton
		{
			get
			{
				return this._fiefsButton;
			}
			set
			{
				if (this._fiefsButton != value)
				{
					this._fiefsButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "FiefsButton");
				}
			}
		}

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x06000E3A RID: 3642 RVA: 0x000279AA File Offset: 0x00025BAA
		// (set) Token: 0x06000E3B RID: 3643 RVA: 0x000279B2 File Offset: 0x00025BB2
		[Editor(false)]
		public ButtonWidget PoliciesButton
		{
			get
			{
				return this._policiesButton;
			}
			set
			{
				if (this._policiesButton != value)
				{
					this._policiesButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "PoliciesButton");
				}
			}
		}

		// Token: 0x1700050F RID: 1295
		// (get) Token: 0x06000E3C RID: 3644 RVA: 0x000279D0 File Offset: 0x00025BD0
		// (set) Token: 0x06000E3D RID: 3645 RVA: 0x000279D8 File Offset: 0x00025BD8
		[Editor(false)]
		public ButtonWidget ClansButton
		{
			get
			{
				return this._clansButton;
			}
			set
			{
				if (this._clansButton != value)
				{
					this._clansButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ClansButton");
				}
			}
		}

		// Token: 0x17000510 RID: 1296
		// (get) Token: 0x06000E3E RID: 3646 RVA: 0x000279F6 File Offset: 0x00025BF6
		// (set) Token: 0x06000E3F RID: 3647 RVA: 0x000279FE File Offset: 0x00025BFE
		[Editor(false)]
		public ButtonWidget ArmiesButton
		{
			get
			{
				return this._armiesButton;
			}
			set
			{
				if (this._armiesButton != value)
				{
					this._armiesButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "ArmiesButton");
				}
			}
		}

		// Token: 0x17000511 RID: 1297
		// (get) Token: 0x06000E40 RID: 3648 RVA: 0x00027A1C File Offset: 0x00025C1C
		// (set) Token: 0x06000E41 RID: 3649 RVA: 0x00027A24 File Offset: 0x00025C24
		[Editor(false)]
		public ButtonWidget DiplomacyButton
		{
			get
			{
				return this._diplomacyButton;
			}
			set
			{
				if (this._diplomacyButton != value)
				{
					this._diplomacyButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "DiplomacyButton");
				}
			}
		}

		// Token: 0x04000685 RID: 1669
		private Widget _armiesPanel;

		// Token: 0x04000686 RID: 1670
		private Widget _clansPanel;

		// Token: 0x04000687 RID: 1671
		private Widget _policiesPanel;

		// Token: 0x04000688 RID: 1672
		private Widget _fiefsPanel;

		// Token: 0x04000689 RID: 1673
		private Widget _diplomacyPanel;

		// Token: 0x0400068A RID: 1674
		private ButtonWidget _fiefsButton;

		// Token: 0x0400068B RID: 1675
		private ButtonWidget _clansButton;

		// Token: 0x0400068C RID: 1676
		private ButtonWidget _policiesButton;

		// Token: 0x0400068D RID: 1677
		private ButtonWidget _armiesButton;

		// Token: 0x0400068E RID: 1678
		private ButtonWidget _diplomacyButton;
	}
}
