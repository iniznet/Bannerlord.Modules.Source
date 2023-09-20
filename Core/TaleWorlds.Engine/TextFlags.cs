using System;

namespace TaleWorlds.Engine
{
	[Flags]
	public enum TextFlags
	{
		RglTfHAlignLeft = 1,
		RglTfHAlignRight = 2,
		RglTfHAlignCenter = 3,
		RglTfVAlignTop = 4,
		RglTfVAlignDown = 8,
		RglTfVAlignCenter = 12,
		RglTfSingleLine = 16,
		RglTfMultiline = 32,
		RglTfItalic = 64,
		RglTfCutTextFromLeft = 128,
		RglTfDoubleSpace = 256,
		RglTfWithOutline = 512,
		RglTfHalfSpace = 1024
	}
}
