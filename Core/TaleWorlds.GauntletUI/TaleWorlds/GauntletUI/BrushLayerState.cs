using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000011 RID: 17
	public struct BrushLayerState : IBrushAnimationState, IDataSource
	{
		// Token: 0x06000108 RID: 264 RVA: 0x00006A44 File Offset: 0x00004C44
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

		// Token: 0x06000109 RID: 265 RVA: 0x00006AD8 File Offset: 0x00004CD8
		void IBrushAnimationState.FillFrom(IDataSource source)
		{
			StyleLayer styleLayer = (StyleLayer)source;
			this.FillFrom(styleLayer);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00006AF4 File Offset: 0x00004CF4
		void IBrushAnimationState.LerpFrom(IBrushAnimationState start, IDataSource end, float ratio)
		{
			BrushLayerState brushLayerState = (BrushLayerState)start;
			IBrushLayerData brushLayerData = (IBrushLayerData)end;
			this.LerpFrom(brushLayerState, brushLayerData, ratio);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00006B18 File Offset: 0x00004D18
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

		// Token: 0x0600010C RID: 268 RVA: 0x00006C34 File Offset: 0x00004E34
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

		// Token: 0x0600010D RID: 269 RVA: 0x00006CD7 File Offset: 0x00004ED7
		public void SetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType, in Color value)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Color)
			{
				this.Color = value;
				return;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayerState.cs", "SetValueAsColor", 122);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00006D00 File Offset: 0x00004F00
		public void SetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType, Sprite value)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Sprite)
			{
				this.Sprite = value;
				return;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayerState.cs", "SetValueAsSprite", 135);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00006D28 File Offset: 0x00004F28
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

		// Token: 0x06000110 RID: 272 RVA: 0x00006DCA File Offset: 0x00004FCA
		public Color GetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Color)
			{
				return this.Color;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayerState.cs", "GetValueAsColor", 175);
			return Color.Black;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00006DF5 File Offset: 0x00004FF5
		public Sprite GetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.Sprite)
			{
				return this.Sprite;
			}
			Debug.FailedAssert("Invalid value type or property name for data source.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushLayerState.cs", "GetValueAsSprite", 187);
			return null;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00006E20 File Offset: 0x00005020
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

		// Token: 0x06000113 RID: 275 RVA: 0x00006F01 File Offset: 0x00005101
		void IBrushAnimationState.SetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType, in Color value)
		{
			this.SetValueAsColor(propertyType, value);
		}

		// Token: 0x04000066 RID: 102
		public Color Color;

		// Token: 0x04000067 RID: 103
		public float ColorFactor;

		// Token: 0x04000068 RID: 104
		public float AlphaFactor;

		// Token: 0x04000069 RID: 105
		public float HueFactor;

		// Token: 0x0400006A RID: 106
		public float SaturationFactor;

		// Token: 0x0400006B RID: 107
		public float ValueFactor;

		// Token: 0x0400006C RID: 108
		public float OverlayXOffset;

		// Token: 0x0400006D RID: 109
		public float OverlayYOffset;

		// Token: 0x0400006E RID: 110
		public float XOffset;

		// Token: 0x0400006F RID: 111
		public float YOffset;

		// Token: 0x04000070 RID: 112
		public Sprite Sprite;
	}
}
