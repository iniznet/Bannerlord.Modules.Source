using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.OpenGL
{
	internal enum BlendingDestinationFactor : uint
	{
		Zero,
		One,
		SourceColor = 768U,
		OneMinusSourceColor,
		SourceAlpha,
		OneMinusSourceAlpha,
		DestinationAlpha,
		OneMinusDestinationAlpha,
		DestinationColor,
		OneMinusDestinationColor,
		SourceAlphaSaturate
	}
}
