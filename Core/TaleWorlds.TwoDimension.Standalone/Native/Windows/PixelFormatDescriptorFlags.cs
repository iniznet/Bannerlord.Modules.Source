using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	[Flags]
	internal enum PixelFormatDescriptorFlags : uint
	{
		DoubleBuffer = 1U,
		Stereo = 2U,
		DrawToWindow = 4U,
		DrawToBitmap = 8U,
		SupportGDI = 16U,
		SupportOpengl = 32U,
		GenericFormat = 64U,
		NeedPalette = 128U,
		NeedSystemPalette = 256U,
		SwapExchange = 512U,
		SwapCopy = 1024U,
		SwapLayerBuffers = 2048U,
		GenericAccelerated = 4096U,
		SupportDirectDraw = 8192U,
		Direct3DAccelerated = 16384U,
		SupportComposition = 32768U
	}
}
