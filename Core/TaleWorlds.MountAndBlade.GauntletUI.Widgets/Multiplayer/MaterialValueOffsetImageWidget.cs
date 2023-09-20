using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer
{
	public class MaterialValueOffsetImageWidget : ImageWidget
	{
		public MaterialValueOffsetImageWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _visualDirty;

		private float _valueOffset;

		private float _saturationOffset;

		private float _hueOffset;
	}
}
