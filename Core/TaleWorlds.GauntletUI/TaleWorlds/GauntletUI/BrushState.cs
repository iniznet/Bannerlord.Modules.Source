using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public struct BrushState : IBrushAnimationState, IDataSource
	{
		public void FillFrom(Style style)
		{
			this.FontColor = style.FontColor;
			this.TextGlowColor = style.TextGlowColor;
			this.TextOutlineColor = style.TextOutlineColor;
			this.TextOutlineAmount = style.TextOutlineAmount;
			this.TextGlowRadius = style.TextGlowRadius;
			this.TextBlur = style.TextBlur;
			this.TextShadowOffset = style.TextShadowOffset;
			this.TextShadowAngle = style.TextShadowAngle;
			this.TextColorFactor = style.TextColorFactor;
			this.TextAlphaFactor = style.TextAlphaFactor;
			this.TextHueFactor = style.TextHueFactor;
			this.TextSaturationFactor = style.TextSaturationFactor;
			this.TextValueFactor = style.TextValueFactor;
		}

		public void LerpFrom(BrushState start, Style end, float ratio)
		{
			this.FontColor = Color.Lerp(start.FontColor, end.FontColor, ratio);
			this.TextGlowColor = Color.Lerp(start.TextGlowColor, end.TextGlowColor, ratio);
			this.TextOutlineColor = Color.Lerp(start.TextOutlineColor, end.TextOutlineColor, ratio);
			this.TextOutlineAmount = Mathf.Lerp(start.TextOutlineAmount, end.TextOutlineAmount, ratio);
			this.TextGlowRadius = Mathf.Lerp(start.TextGlowRadius, end.TextGlowRadius, ratio);
			this.TextBlur = Mathf.Lerp(start.TextBlur, end.TextBlur, ratio);
			this.TextShadowOffset = Mathf.Lerp(start.TextShadowOffset, end.TextShadowOffset, ratio);
			this.TextShadowAngle = Mathf.Lerp(start.TextShadowAngle, end.TextShadowAngle, ratio);
			this.TextColorFactor = Mathf.Lerp(start.TextColorFactor, end.TextColorFactor, ratio);
			this.TextAlphaFactor = Mathf.Lerp(start.TextAlphaFactor, end.TextAlphaFactor, ratio);
			this.TextHueFactor = Mathf.Lerp(start.TextHueFactor, end.TextHueFactor, ratio);
			this.TextSaturationFactor = Mathf.Lerp(start.TextSaturationFactor, end.TextSaturationFactor, ratio);
			this.TextValueFactor = Mathf.Lerp(start.TextValueFactor, end.TextValueFactor, ratio);
		}

		void IBrushAnimationState.FillFrom(IDataSource source)
		{
			Style style = (Style)source;
			this.FillFrom(style);
		}

		void IBrushAnimationState.LerpFrom(IBrushAnimationState start, IDataSource end, float ratio)
		{
			BrushState brushState = (BrushState)start;
			Style style = (Style)end;
			this.LerpFrom(brushState, style, ratio);
		}

		public float GetValueAsFloat(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineColor:
				Debug.FailedAssert("Invalid value type for BrushState.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "GetValueAsFloat", 102);
				return 0f;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineAmount:
				return this.TextOutlineAmount;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowRadius:
				return this.TextGlowRadius;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextBlur:
				return this.TextBlur;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowOffset:
				return this.TextShadowOffset;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowAngle:
				return this.TextShadowAngle;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextColorFactor:
				return this.TextColorFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextAlphaFactor:
				return this.TextAlphaFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextHueFactor:
				return this.TextHueFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextSaturationFactor:
				return this.TextSaturationFactor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextValueFactor:
				return this.TextValueFactor;
			}
			Debug.FailedAssert("Invalid BrushState property.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "GetValueAsFloat", 106);
			return 0f;
		}

		public Color GetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
				return this.FontColor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor:
				return this.TextGlowColor;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineColor:
				return this.TextOutlineColor;
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
				Debug.FailedAssert("Invalid value type for BrushState.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "GetValueAsColor", 132);
				return Color.Black;
			}
			Debug.FailedAssert("Invalid BrushState property.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "GetValueAsColor", 135);
			return Color.Black;
		}

		public Sprite GetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.FontColor || propertyType - BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor <= 11)
			{
				Debug.FailedAssert("Invalid value type for BrushState.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "GetValueAsSprite", 157);
				return null;
			}
			Debug.FailedAssert("Invalid BrushState property.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "GetValueAsSprite", 161);
			return null;
		}

		public void SetValueAsFloat(BrushAnimationProperty.BrushAnimationPropertyType propertyType, float value)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor:
			case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineColor:
				Debug.FailedAssert("Invalid value type for BrushState.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "SetValueAsFloat", 204);
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineAmount:
				this.TextOutlineAmount = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowRadius:
				this.TextGlowRadius = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextBlur:
				this.TextBlur = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowOffset:
				this.TextShadowOffset = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextShadowAngle:
				this.TextShadowAngle = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextColorFactor:
				this.TextColorFactor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextAlphaFactor:
				this.TextAlphaFactor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextHueFactor:
				this.TextHueFactor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextSaturationFactor:
				this.TextSaturationFactor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextValueFactor:
				this.TextValueFactor = value;
				return;
			}
			Debug.FailedAssert("Invalid BrushState property.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "SetValueAsFloat", 208);
		}

		public void SetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType, in Color value)
		{
			switch (propertyType)
			{
			case BrushAnimationProperty.BrushAnimationPropertyType.FontColor:
				this.FontColor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor:
				this.TextGlowColor = value;
				return;
			case BrushAnimationProperty.BrushAnimationPropertyType.TextOutlineColor:
				this.TextOutlineColor = value;
				return;
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
				Debug.FailedAssert("Invalid value type for BrushState.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "SetValueAsColor", 237);
				return;
			}
			Debug.FailedAssert("Invalid BrushState property.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "SetValueAsColor", 240);
		}

		public void SetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType, Sprite value)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.FontColor || propertyType - BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor <= 11)
			{
				Debug.FailedAssert("Invalid value type for BrushState.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "SetValueAsSprite", 262);
				return;
			}
			Debug.FailedAssert("Invalid BrushState property.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "SetValueAsSprite", 265);
		}

		public TextMaterial CreateTextMaterial(TwoDimensionDrawContext drawContext)
		{
			TextMaterial textMaterial = drawContext.CreateTextMaterial();
			textMaterial.Color = this.FontColor;
			textMaterial.GlowColor = this.TextGlowColor;
			textMaterial.OutlineColor = this.TextOutlineColor;
			textMaterial.OutlineAmount = this.TextOutlineAmount;
			textMaterial.GlowRadius = this.TextGlowRadius;
			textMaterial.Blur = this.TextBlur;
			textMaterial.ShadowOffset = this.TextShadowOffset;
			textMaterial.ShadowAngle = this.TextShadowAngle;
			textMaterial.ColorFactor = this.TextColorFactor;
			textMaterial.AlphaFactor = this.TextAlphaFactor;
			textMaterial.HueFactor = this.TextHueFactor;
			textMaterial.SaturationFactor = this.TextSaturationFactor;
			textMaterial.ValueFactor = this.TextValueFactor;
			return textMaterial;
		}

		void IBrushAnimationState.SetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType, in Color value)
		{
			this.SetValueAsColor(propertyType, value);
		}

		public Color FontColor;

		public Color TextGlowColor;

		public Color TextOutlineColor;

		public float TextOutlineAmount;

		public float TextGlowRadius;

		public float TextBlur;

		public float TextShadowOffset;

		public float TextShadowAngle;

		public float TextColorFactor;

		public float TextAlphaFactor;

		public float TextHueFactor;

		public float TextSaturationFactor;

		public float TextValueFactor;
	}
}
