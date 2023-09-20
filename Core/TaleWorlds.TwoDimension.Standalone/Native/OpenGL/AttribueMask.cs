using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.OpenGL
{
	internal enum AttribueMask : uint
	{
		CurrentBit = 1U,
		PointBit,
		LineBit = 4U,
		PolygonBit = 8U,
		PolygonStippleBit = 16U,
		PixelModeBit = 32U,
		LightingBit = 64U,
		FogBit = 128U,
		DepthBufferBit = 256U,
		AccumBufferBit = 512U,
		StencilBufferBit = 1024U,
		ViewportBit = 2048U,
		TransformBit = 4096U,
		EnableBit = 8192U,
		ColorBufferBit = 16384U,
		HintBit = 32768U,
		EvalBit = 65536U,
		ListBit = 131072U,
		TextureBit = 262144U,
		ScissorBit = 524288U,
		AllAttribBits = 1048575U
	}
}
