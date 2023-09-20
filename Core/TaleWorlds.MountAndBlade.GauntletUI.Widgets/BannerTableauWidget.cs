using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class BannerTableauWidget : TextureWidget
	{
		public BannerTableauWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "BannerTableauTextureProvider";
		}

		protected override void OnMousePressed()
		{
		}

		protected override void OnMouseReleased()
		{
		}

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			this._isRenderRequestedPreviousFrame = true;
			if (base.TextureProvider != null)
			{
				base.Texture = base.TextureProvider.GetTexture(twoDimensionContext, string.Empty);
				SimpleMaterial simpleMaterial = drawContext.CreateSimpleMaterial();
				Brush readOnlyBrush = base.ReadOnlyBrush;
				StyleLayer styleLayer;
				if (readOnlyBrush == null)
				{
					styleLayer = null;
				}
				else
				{
					List<StyleLayer> layers = readOnlyBrush.GetStyleOrDefault(base.CurrentState).Layers;
					styleLayer = ((layers != null) ? layers.FirstOrDefault<StyleLayer>() : null);
				}
				StyleLayer styleLayer2 = styleLayer ?? null;
				simpleMaterial.OverlayEnabled = false;
				simpleMaterial.CircularMaskingEnabled = false;
				simpleMaterial.Texture = base.Texture;
				simpleMaterial.AlphaFactor = ((styleLayer2 != null) ? styleLayer2.AlphaFactor : 1f) * base.ReadOnlyBrush.GlobalAlphaFactor * base.Context.ContextAlpha;
				simpleMaterial.ColorFactor = ((styleLayer2 != null) ? styleLayer2.ColorFactor : 1f) * base.ReadOnlyBrush.GlobalColorFactor;
				simpleMaterial.HueFactor = ((styleLayer2 != null) ? styleLayer2.HueFactor : 0f);
				simpleMaterial.SaturationFactor = ((styleLayer2 != null) ? styleLayer2.SaturationFactor : 0f);
				simpleMaterial.ValueFactor = ((styleLayer2 != null) ? styleLayer2.ValueFactor : 0f);
				simpleMaterial.Color = ((styleLayer2 != null) ? styleLayer2.Color : Color.White) * base.ReadOnlyBrush.GlobalColor;
				Vector2 globalPosition = base.GlobalPosition;
				float x = globalPosition.X;
				float y = globalPosition.Y;
				Vector2 size = base.Size;
				Vector2 size2 = base.Size;
				DrawObject2D drawObject2D = null;
				if (this._cachedQuad != null && this._cachedQuadSize == base.Size)
				{
					drawObject2D = this._cachedQuad;
				}
				if (drawObject2D == null)
				{
					drawObject2D = DrawObject2D.CreateQuad(base.Size);
					this._cachedQuad = drawObject2D;
					this._cachedQuadSize = base.Size;
				}
				if (drawContext.CircularMaskEnabled)
				{
					simpleMaterial.CircularMaskingEnabled = true;
					simpleMaterial.CircularMaskingCenter = drawContext.CircularMaskCenter;
					simpleMaterial.CircularMaskingRadius = drawContext.CircularMaskRadius;
					simpleMaterial.CircularMaskingSmoothingRadius = drawContext.CircularMaskSmoothingRadius;
				}
				drawContext.Draw(x, y, simpleMaterial, drawObject2D, base.Size.X, base.Size.Y);
			}
		}

		[Editor(false)]
		public string BannerCodeText
		{
			get
			{
				return this._bannerCode;
			}
			set
			{
				if (value != this._bannerCode)
				{
					this._bannerCode = value;
					base.OnPropertyChanged<string>(value, "BannerCodeText");
					base.SetTextureProviderProperty("BannerCodeText", value);
				}
			}
		}

		[Editor(false)]
		public float CustomRenderScale
		{
			get
			{
				return this._customRenderScale;
			}
			set
			{
				if (value != this._customRenderScale)
				{
					this._customRenderScale = value;
					base.OnPropertyChanged(value, "CustomRenderScale");
					base.SetTextureProviderProperty("CustomRenderScale", value);
				}
			}
		}

		[Editor(false)]
		public bool IsNineGrid
		{
			get
			{
				return this._isNineGrid;
			}
			set
			{
				if (value != this._isNineGrid)
				{
					this._isNineGrid = value;
					base.OnPropertyChanged(value, "IsNineGrid");
					base.SetTextureProviderProperty("IsNineGrid", value);
				}
			}
		}

		[Editor(false)]
		public Vec2 UpdatePositionValueManual
		{
			get
			{
				return this._updatePositionRef;
			}
			set
			{
				if (value != this._updatePositionRef)
				{
					this._updatePositionRef = value;
					base.OnPropertyChanged(value, "UpdatePositionValueManual");
					base.SetTextureProviderProperty("UpdatePositionValueManual", value);
				}
			}
		}

		[Editor(false)]
		public Vec2 UpdateSizeValueManual
		{
			get
			{
				return this._updateSizeRef;
			}
			set
			{
				if (value != this._updateSizeRef)
				{
					this._updateSizeRef = value;
					base.OnPropertyChanged(value, "UpdateSizeValueManual");
					base.SetTextureProviderProperty("UpdateSizeValueManual", value);
				}
			}
		}

		[Editor(false)]
		public ValueTuple<float, bool> UpdateRotationValueManualWithMirror
		{
			get
			{
				return this._updateRotationWithMirrorRef;
			}
			set
			{
				if (value.Item1 != this._updateRotationWithMirrorRef.Item1 || value.Item2 != this._updateRotationWithMirrorRef.Item2)
				{
					this._updateRotationWithMirrorRef = value;
					base.OnPropertyChanged<string>("UpdateRotationValueManualWithMirror", "UpdateRotationValueManualWithMirror");
					base.SetTextureProviderProperty("UpdateRotationValueManualWithMirror", value);
				}
			}
		}

		[Editor(false)]
		public int MeshIndexToUpdate
		{
			get
			{
				return this._meshIndexToUpdate;
			}
			set
			{
				if (value != this._meshIndexToUpdate)
				{
					this._meshIndexToUpdate = value;
					base.OnPropertyChanged(value, "MeshIndexToUpdate");
					base.SetTextureProviderProperty("MeshIndexToUpdate", value);
				}
			}
		}

		private string _bannerCode;

		private float _customRenderScale;

		private bool _isNineGrid;

		private Vec2 _updatePositionRef;

		private Vec2 _updateSizeRef;

		private ValueTuple<float, bool> _updateRotationWithMirrorRef;

		private int _meshIndexToUpdate;
	}
}
