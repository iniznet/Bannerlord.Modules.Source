using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party
{
	// Token: 0x02000059 RID: 89
	public class PartyHealthFillBarWidget : FillBar
	{
		// Token: 0x060004A3 RID: 1187 RVA: 0x0000E4DC File Offset: 0x0000C6DC
		public PartyHealthFillBarWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0000E528 File Offset: 0x0000C728
		private void HealthUpdated()
		{
			if (this.brushLayer == null)
			{
				this.brushLayer = base.Brush.GetLayer("DefaultFill");
			}
			base.CurrentAmount = (base.InitialAmount = this.Health);
			if (this.IsWounded)
			{
				this.brushLayer.Color = this.WoundedColor;
			}
			else if (this.Health >= this.FullHealthyLimit)
			{
				this.brushLayer.Color = this.FullHealthyColor;
			}
			else
			{
				this.brushLayer.Color = this.HealthyColor;
			}
			if (this.HealthText != null)
			{
				this.HealthText.Text = this.Health + "%";
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x060004A5 RID: 1189 RVA: 0x0000E5DD File Offset: 0x0000C7DD
		// (set) Token: 0x060004A6 RID: 1190 RVA: 0x0000E5E5 File Offset: 0x0000C7E5
		[Editor(false)]
		public int Health
		{
			get
			{
				return this._health;
			}
			set
			{
				if (this._health != value)
				{
					this._health = value;
					base.OnPropertyChanged(value, "Health");
					this.HealthUpdated();
				}
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x060004A7 RID: 1191 RVA: 0x0000E609 File Offset: 0x0000C809
		// (set) Token: 0x060004A8 RID: 1192 RVA: 0x0000E611 File Offset: 0x0000C811
		[Editor(false)]
		public bool IsWounded
		{
			get
			{
				return this._isWounded;
			}
			set
			{
				if (this._isWounded != value)
				{
					this._isWounded = value;
					base.OnPropertyChanged(value, "IsWounded");
					this.HealthUpdated();
				}
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060004A9 RID: 1193 RVA: 0x0000E635 File Offset: 0x0000C835
		// (set) Token: 0x060004AA RID: 1194 RVA: 0x0000E63D File Offset: 0x0000C83D
		[Editor(false)]
		public TextWidget HealthText
		{
			get
			{
				return this._healthText;
			}
			set
			{
				if (this._healthText != value)
				{
					this._healthText = value;
					base.OnPropertyChanged<TextWidget>(value, "HealthText");
					this.HealthUpdated();
				}
			}
		}

		// Token: 0x04000201 RID: 513
		private readonly int FullHealthyLimit = 90;

		// Token: 0x04000202 RID: 514
		private readonly Color WoundedColor = Color.FromUint(4290199102U);

		// Token: 0x04000203 RID: 515
		private readonly Color HealthyColor = Color.FromUint(4291732560U);

		// Token: 0x04000204 RID: 516
		private readonly Color FullHealthyColor = Color.FromUint(4284921662U);

		// Token: 0x04000205 RID: 517
		private BrushLayer brushLayer;

		// Token: 0x04000206 RID: 518
		private int _health;

		// Token: 0x04000207 RID: 519
		private bool _isWounded;

		// Token: 0x04000208 RID: 520
		private TextWidget _healthText;
	}
}
