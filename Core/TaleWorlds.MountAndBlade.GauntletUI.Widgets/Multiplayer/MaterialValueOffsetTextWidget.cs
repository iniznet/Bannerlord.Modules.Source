using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MaterialValueOffsetTextWidget : TextWidget
	{
		public MaterialValueOffsetTextWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _visualDirty;

		private float _valueOffset;

		private float _saturationOffset;

		private float _hueOffset;
	}
}
