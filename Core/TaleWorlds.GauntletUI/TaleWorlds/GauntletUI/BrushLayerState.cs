using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public struct BrushLayerState : IBrushAnimationState, IDataSource
	{
		public void FillFrom(IBrushLayerData styleLayer)
		{
			this.ColorFactor = styleLayer.ColorFactor;
			this.AlphaFactor = styleLayer.AlphaFactor;
			this.HueFactor = styleLayer.HueFactor;
			this.SaturationFactor = styleLayer.SaturationFactor;
			this.ValueFactor = styleLayer.ValueFactor;
			this.Color = styleLayer.Color;
			this.OverlayXOffset = styleLayer.OverlayXOffset;
			this.OverlayYOffset = styleLayer.OverlayYOffset;
			this.XOffset = styleLayer.XOffset;
			this.YOffset = styleLayer.YOffset;
			this.Sprite = styleLayer.Sprite;
		}

		void IBrushAnimationState.FillFrom(IDataSource source)
		{
			StyleLayer styleLayer = (StyleLayer)source;
			this.FillFrom(styleLayer);
		}

		void IBrushAnimationState.LerpFrom(IBrushAnimationState start, IDataSource end, float ratio)
		{
			BrushLayerState brushLayerState = (BrushLayerState)start;
			IBrushLayerData brushLayerData = (IBrushLayerData)end;
			this.LerpFrom(brushLayerState, brushLayerData, ratio);
		}

		public void LerpFrom(BrushLayerState start, IBrushLayerData end, float ratio)
		{
			this.ColorFactor = Mathf.Lerp(start.ColorFactor, end.ColorFactor, ratio);
			this.AlphaFactor = Mathf.Lerp(start.AlphaFactor, end.AlphaFactor, ratio);
			this.HueFactor = Mathf.Lerp(start.HueFactor, end.HueFactor, ratio);
			this.SaturationFactor = Mathf.Lerp(start.SaturationFactor, end.SaturationFactor, ratio);
			this.ValueFactor = Mathf.Lerp(start.ValueFactor, end.ValueFactor, ratio);
			this.Color = Color.Lerp(start.Color, end.Color, ratio);
			this.OverlayXOffset = Mathf.Lerp(start.OverlayXOffset, end.OverlayXOffset, ratio);
			this.OverlayYOffset = Mathf.Lerp(start.OverlayYOffset, end.OverlayYOffset, ratio);
			this.XOffset = Mathf.Lerp(start.XOffset, end.XOffset, ratio);
			this.YOffset = Mathf.Lerp(start.YOffset, end.YOffset, ratio);
			this.Sprite = ((ratio > 0.9f) ? end.Sprite : start.Sprite);
		}

		public void SetValueAsFloat(BrushAnimationProperty.BrushAnimationPropertyType propertyType, float value)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.ColorFactor:
				this.ColorFactor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.Color:
			case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
				break;
			case BrushAnimationProperty.BrushAnimationPropertyType.AlphaFactor:
				this.AlphaFactor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.HueFactor:
				this.HueFactor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.SaturationFactor:
				this.SaturationFactor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.ValueFactor:
				this.ValueFactor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayXOffset:
				this.OverlayXOffset = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayYOffset:
				this.OverlayYOffset = value;
				return;
			default:
				if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.XOffset)
				{
					this.XOffset = value;
					return;
				}
				if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.YOffset)
				{
					this.YOffset = value;
					return;
				}
				break;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayerState.cs", "SetValueAsFloat", 109);
		}

		public void SetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType, in Color value)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Color)
			{
				this.Color = value;
				return;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayerState.cs", "SetValueAsColor", 122);
		}

		public void SetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType, Sprite value)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Sprite)
			{
				this.Sprite = value;
				return;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayerState.cs", "SetValueAsSprite", 135);
		}

		public float GetValueAsFloat(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.ColorFactor:
				return this.ColorFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.Color:
			case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
				break;
			case BrushAnimationProperty.BrushAnimationPropertyType.AlphaFactor:
				return this.AlphaFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.HueFactor:
				return this.HueFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.SaturationFactor:
				return this.SaturationFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.ValueFactor:
				return this.ValueFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayXOffset:
				return this.OverlayXOffset;
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayYOffset:
				return this.OverlayYOffset;
			default:
				if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.XOffset)
				{
					return this.XOffset;
				}
				if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.YOffset)
				{
					return this.YOffset;
				}
				break;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayerState.cs", "GetValueAsFloat", 163);
			return 0f;
		}

		public Color GetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Color)
			{
				return this.Color;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayerState.cs", "GetValueAsColor", 175);
			return Color.Black;
		}

		public Sprite GetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Sprite)
			{
				return this.Sprite;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayerState.cs", "GetValueAsSprite", 187);
			return null;
		}

		public static void SetValueAsLerpOfValues(ref BrushLayerState currentState, in BrushAnimationKeyFrame startValue, in BrushAnimationKeyFrame endValue, BrushAnimationProperty.BrushAnimationPropertyType propertyType, float ratio)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.ColorFactor:
			case BrushAnimationProperty.BrushAnimationPropertyType.AlphaFactor:
			case BrushAnimationProperty.BrushAnimationPropertyType.HueFactor:
			case BrushAnimationProperty.BrushAnimationPropertyType.SaturationFactor:
			case BrushAnimationProperty.BrushAnimationPropertyType.ValueFactor:
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayXOffset:
			case BrushAnimationProperty.BrushAnimationPropertyType.OverlayYOffset:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineAmount:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowRadius:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextBlur:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowOffset:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowAngle:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextColorFactor:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextAlphaFactor:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextHueFactor:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextSaturationFactor:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextValueFactor:
			case BrushAnimationProperty.BrushAnimationPropertyType.XOffset:
			case BrushAnimationProperty.BrushAnimationPropertyType.YOffset:
				currentState.SetValueAsFloat(propertyType, MathF.Lerp(startValue.GetValueAsFloat(), endValue.GetValueAsFloat(), ratio, 1E-05f));
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.Color:
			case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineColor:
			{
				Color color = Color.Lerp(startValue.GetValueAsColor(), endValue.GetValueAsColor(), ratio);
				currentState.SetValueAsColor(propertyType, color);
				return;
			}
			case BrushAnimationProperty.BrushAnimationPropertyType.Sprite:
				currentState.SetValueAsSprite(propertyType, ((double)ratio > 0.9) ? endValue.GetValueAsSprite() : startValue.GetValueAsSprite());
				break;
			case BrushAnimationProperty.BrushAnimationPropertyType.IsHidden:
				break;
			default:
				return;
			}
		}

		void IBrushAnimationState.SetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType, in Color value)
		{
			this.SetValueAsColor(propertyType, value);
		}

		public Color Color;

		public float ColorFactor;

		public float AlphaFactor;

		public float HueFactor;

		public float SaturationFactor;

		public float ValueFactor;

		public float OverlayXOffset;

		public float OverlayYOffset;

		public float XOffset;

		public float YOffset;

		public Sprite Sprite;
	}
}
