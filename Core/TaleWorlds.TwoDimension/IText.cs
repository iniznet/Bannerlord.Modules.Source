using System;
using System.Numerics;

namespace TaleWorlds.TwoDimension
{
	// Token: 0x02000021 RID: 33
	public interface IText
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000132 RID: 306
		// (set) Token: 0x06000133 RID: 307
		string Value { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000134 RID: 308
		// (set) Token: 0x06000135 RID: 309
		TextHorizontalAlignment HorizontalAlignment { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000136 RID: 310
		// (set) Token: 0x06000137 RID: 311
		TextVerticalAlignment VerticalAlignment { get; set; }

		// Token: 0x06000138 RID: 312
		Vector2 GetPreferredSize(bool fixedWidth, float widthSize, bool fixedHeight, float heightSize, SpriteData spriteData, float renderScale);
	}
}
