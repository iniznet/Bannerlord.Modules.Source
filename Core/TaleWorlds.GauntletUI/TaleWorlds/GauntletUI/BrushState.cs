using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000014 RID: 20
	public struct BrushState : IBrushAnimationState, IDataSource
	{
		// Token: 0x0600013E RID: 318 RVA: 0x00008904 File Offset: 0x00006B04
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

		// Token: 0x0600013F RID: 319 RVA: 0x000089B0 File Offset: 0x00006BB0
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

		// Token: 0x06000140 RID: 320 RVA: 0x00008AF8 File Offset: 0x00006CF8
		void IBrushAnimationState.FillFrom(IDataSource source)
		{
			Style style = (Style)source;
			this.FillFrom(style);
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00008B14 File Offset: 0x00006D14
		void IBrushAnimationState.LerpFrom(IBrushAnimationState start, IDataSource end, float ratio)
		{
			BrushState brushState = (BrushState)start;
			Style style = (Style)end;
			this.LerpFrom(brushState, style, ratio);
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00008B38 File Offset: 0x00006D38
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

		// Token: 0x06000143 RID: 323 RVA: 0x00008C08 File Offset: 0x00006E08
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

		// Token: 0x06000144 RID: 324 RVA: 0x00008CB0 File Offset: 0x00006EB0
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

		// Token: 0x06000145 RID: 325 RVA: 0x00008D00 File Offset: 0x00006F00
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

		// Token: 0x06000146 RID: 326 RVA: 0x00008DD8 File Offset: 0x00006FD8
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

		// Token: 0x06000147 RID: 327 RVA: 0x00008E88 File Offset: 0x00007088
		public void SetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType, Sprite value)
		{
			if (propertyType == BrushAnimationProperty.BrushAnimationPropertyType.FontColor || propertyType - BrushAnimationProperty.BrushAnimationPropertyType.TextGlowColor <= 11)
			{
				Debug.FailedAssert("Invalid value type for BrushState.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "SetValueAsSprite", 262);
				return;
			}
			Debug.FailedAssert("Invalid BrushState property.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.GauntletUI\\Brush\\BrushState.cs", "SetValueAsSprite", 265);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00008ED4 File Offset: 0x000070D4
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

		// Token: 0x06000149 RID: 329 RVA: 0x00008F83 File Offset: 0x00007183
		void IBrushAnimationState.SetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType, in Color value)
		{
			this.SetValueAsColor(propertyType, value);
		}

		// Token: 0x04000086 RID: 134
		public Color FontColor;

		// Token: 0x04000087 RID: 135
		public Color TextGlowColor;

		// Token: 0x04000088 RID: 136
		public Color TextOutlineColor;

		// Token: 0x04000089 RID: 137
		public float TextOutlineAmount;

		// Token: 0x0400008A RID: 138
		public float TextGlowRadius;

		// Token: 0x0400008B RID: 139
		public float TextBlur;

		// Token: 0x0400008C RID: 140
		public float TextShadowOffset;

		// Token: 0x0400008D RID: 141
		public float TextShadowAngle;

		// Token: 0x0400008E RID: 142
		public float TextColorFactor;

		// Token: 0x0400008F RID: 143
		public float TextAlphaFactor;

		// Token: 0x04000090 RID: 144
		public float TextHueFactor;

		// Token: 0x04000091 RID: 145
		public float TextSaturationFactor;

		// Token: 0x04000092 RID: 146
		public float TextValueFactor;
	}
}
