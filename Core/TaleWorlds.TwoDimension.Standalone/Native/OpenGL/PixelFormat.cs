using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.OpenGL
{
	internal enum PixelFormat : uint
	{
		ColorIndex = 6400U,
		StencilIndex,
		DepthComponent,
		Red,
		Green,
		Blue,
		Alpha,
		RGB,
		RGBA,
		Luminance,
		LuminanceAlpha,
		BGR = 32992U,
		BGRA
	}
}
