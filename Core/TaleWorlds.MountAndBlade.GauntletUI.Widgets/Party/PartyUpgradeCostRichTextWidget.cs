using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x02000060 RID: 96
	public class PartyUpgradeCostRichTextWidget : RichTextWidget
	{
		// Token: 0x06000524 RID: 1316 RVA: 0x0000FAD8 File Offset: 0x0000DCD8
		public PartyUpgradeCostRichTextWidget(UIContext context)
			: base(context)
		{
			this.NormalColor = new Color(1f, 1f, 1f, 1f);
			this.InsufficientColor = new Color(0.753f, 0.071f, 0.098f, 1f);
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0000FB31 File Offset: 0x0000DD31
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._requiresRefresh)
			{
				base.Brush.FontColor = (this.IsSufficient ? this.NormalColor : this.InsufficientColor);
				this._requiresRefresh = false;
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000526 RID: 1318 RVA: 0x0000FB6A File Offset: 0x0000DD6A
		// (set) Token: 0x06000527 RID: 1319 RVA: 0x0000FB72 File Offset: 0x0000DD72
		[Editor(false)]
		public bool IsSufficient
		{
			get
			{
				return this._isSufficient;
			}
			set
			{
				if (value != this._isSufficient)
				{
					this._isSufficient = value;
					base.OnPropertyChanged(value, "IsSufficient");
					this._requiresRefresh = true;
				}
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000528 RID: 1320 RVA: 0x0000FB97 File Offset: 0x0000DD97
		// (set) Token: 0x06000529 RID: 1321 RVA: 0x0000FB9F File Offset: 0x0000DD9F
		public Color NormalColor
		{
			get
			{
				return this._normalColor;
			}
			set
			{
				if (value != this._normalColor)
				{
					this._normalColor = value;
					this._requiresRefresh = true;
				}
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x0600052A RID: 1322 RVA: 0x0000FBBD File Offset: 0x0000DDBD
		// (set) Token: 0x0600052B RID: 1323 RVA: 0x0000FBC5 File Offset: 0x0000DDC5
		public Color InsufficientColor
		{
			get
			{
				return this._insufficientColor;
			}
			set
			{
				if (value != this._insufficientColor)
				{
					this._insufficientColor = value;
					this._requiresRefresh = true;
				}
			}
		}

		// Token: 0x04000239 RID: 569
		private bool _requiresRefresh = true;

		// Token: 0x0400023A RID: 570
		private bool _isSufficient;

		// Token: 0x0400023B RID: 571
		private Color _normalColor;

		// Token: 0x0400023C RID: 572
		private Color _insufficientColor;
	}
}
