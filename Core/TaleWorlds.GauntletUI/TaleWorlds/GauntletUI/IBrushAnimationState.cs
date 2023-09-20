using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000010 RID: 16
	public interface IBrushAnimationState
	{
		// Token: 0x06000100 RID: 256
		void FillFrom(IDataSource source);

		// Token: 0x06000101 RID: 257
		void LerpFrom(IBrushAnimationState start, IDataSource end, float ratio);

		// Token: 0x06000102 RID: 258
		float GetValueAsFloat(BrushAnimationProperty.BrushAnimationPropertyType propertyType);

		// Token: 0x06000103 RID: 259
		Color GetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType);

		// Token: 0x06000104 RID: 260
		Sprite GetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType);

		// Token: 0x06000105 RID: 261
		void SetValueAsFloat(BrushAnimationProperty.BrushAnimationPropertyType propertyType, float value);

		// Token: 0x06000106 RID: 262
		void SetValueAsColor(BrushAnimationProperty.BrushAnimationPropertyType propertyType, in Color value);

		// Token: 0x06000107 RID: 263
		void SetValueAsSprite(BrushAnimationProperty.BrushAnimationPropertyType propertyType, Sprite value);
	}
}
