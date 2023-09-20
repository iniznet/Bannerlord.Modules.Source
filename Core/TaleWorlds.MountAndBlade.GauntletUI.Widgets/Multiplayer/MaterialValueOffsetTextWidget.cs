using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x02000078 RID: 120
	public class MaterialValueOffsetTextWidget : TextWidget
	{
		// Token: 0x060006BC RID: 1724 RVA: 0x0001420C File Offset: 0x0001240C
		public MaterialValueOffsetTextWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006BD RID: 1725 RVA: 0x00014218 File Offset: 0x00012418
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._visualDirty)
			{
				base.Brush.TextValueFactor += this.ValueOffset;
				base.Brush.TextSaturationFactor += this.SaturationOffset;
				base.Brush.TextHueFactor += this.HueOffset;
				foreach (Style style in base.Brush.Styles)
				{
					style.TextValueFactor += this.ValueOffset;
					style.TextSaturationFactor += this.SaturationOffset;
					style.TextHueFactor += this.HueOffset;
				}
				this._visualDirty = false;
			}
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x00014304 File Offset: 0x00012504
		// (set) Token: 0x060006BF RID: 1727 RVA: 0x0001430C File Offset: 0x0001250C
		public float ValueOffset
		{
			get
			{
				return this._valueOffset;
			}
			set
			{
				if (this._valueOffset != value)
				{
					this._valueOffset = value;
					this._visualDirty = true;
				}
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x00014325 File Offset: 0x00012525
		// (set) Token: 0x060006C1 RID: 1729 RVA: 0x0001432D File Offset: 0x0001252D
		public float SaturationOffset
		{
			get
			{
				return this._saturationOffset;
			}
			set
			{
				if (this._saturationOffset != value)
				{
					this._saturationOffset = value;
					this._visualDirty = true;
				}
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00014346 File Offset: 0x00012546
		// (set) Token: 0x060006C3 RID: 1731 RVA: 0x0001434E File Offset: 0x0001254E
		public float HueOffset
		{
			get
			{
				return this._hueOffset;
			}
			set
			{
				if (this._hueOffset != value)
				{
					this._hueOffset = value;
					this._visualDirty = true;
				}
			}
		}

		// Token: 0x040002F9 RID: 761
		private bool _visualDirty;

		// Token: 0x040002FA RID: 762
		private float _valueOffset;

		// Token: 0x040002FB RID: 763
		private float _saturationOffset;

		// Token: 0x040002FC RID: 764
		private float _hueOffset;
	}
}
