using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	// Token: 0x02000077 RID: 119
	public class MaterialValueOffsetImageWidget : ImageWidget
	{
		// Token: 0x060006B4 RID: 1716 RVA: 0x000140DC File Offset: 0x000122DC
		public MaterialValueOffsetImageWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x000140E8 File Offset: 0x000122E8
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._visualDirty)
			{
				foreach (Style style in base.Brush.Styles)
				{
					foreach (StyleLayer styleLayer in style.Layers)
					{
						styleLayer.ValueFactor += this.ValueOffset;
						styleLayer.SaturationFactor += this.SaturationOffset;
						styleLayer.HueFactor += this.HueOffset;
					}
				}
				this._visualDirty = false;
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x000141C4 File Offset: 0x000123C4
		// (set) Token: 0x060006B7 RID: 1719 RVA: 0x000141CC File Offset: 0x000123CC
		public float ValueOffset
		{
			get
			{
				return this._valueOffset;
			}
			set
			{
				this._valueOffset = value;
				this._visualDirty = true;
			}
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x000141DC File Offset: 0x000123DC
		// (set) Token: 0x060006B9 RID: 1721 RVA: 0x000141E4 File Offset: 0x000123E4
		public float SaturationOffset
		{
			get
			{
				return this._saturationOffset;
			}
			set
			{
				this._saturationOffset = value;
				this._visualDirty = true;
			}
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x060006BA RID: 1722 RVA: 0x000141F4 File Offset: 0x000123F4
		// (set) Token: 0x060006BB RID: 1723 RVA: 0x000141FC File Offset: 0x000123FC
		public float HueOffset
		{
			get
			{
				return this._hueOffset;
			}
			set
			{
				this._hueOffset = value;
				this._visualDirty = true;
			}
		}

		// Token: 0x040002F5 RID: 757
		private bool _visualDirty;

		// Token: 0x040002F6 RID: 758
		private float _valueOffset;

		// Token: 0x040002F7 RID: 759
		private float _saturationOffset;

		// Token: 0x040002F8 RID: 760
		private float _hueOffset;
	}
}
