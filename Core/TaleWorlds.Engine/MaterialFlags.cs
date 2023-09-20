using System;

namespace TaleWorlds.Engine
{
	[Flags]
	public enum MaterialFlags : uint
	{
		RenderFrontToBack = 1U,
		NoDepthTest = 2U,
		DontDrawToDepthRenderTarget = 4U,
		NoModifyDepthBuffer = 8U,
		CullFrontFaces = 16U,
		TwoSided = 32U,
		AlphaBlendSort = 64U,
		DontOptimizeMesh = 128U,
		AlphaBlendNone = 0U,
		AlphaBlendModulate = 256U,
		AlphaBlendAdd = 512U,
		AlphaBlendMultiply = 768U,
		AlphaBlendFactor = 1792U,
		AlphaBlendMask = 1792U,
		AlphaBlendBits = 8U,
		BillboardNone = 0U,
		Billboard2d = 4096U,
		Billboard3d = 8192U,
		BillboardMask = 12288U,
		Skybox = 131072U,
		MultiPassAlpha = 262144U,
		GbufferAlphaBlend = 524288U,
		RequiresForwardRendering = 1048576U,
		AvoidRecomputationOfNormals = 2097152U,
		RenderOrderPlus1 = 150994944U,
		RenderOrderPlus2 = 167772160U,
		RenderOrderPlus3 = 184549376U,
		RenderOrderPlus4 = 201326592U,
		RenderOrderPlus5 = 218103808U,
		RenderOrderPlus6 = 234881024U,
		RenderOrderPlus7 = 251658240U,
		GreaterDepthNoWrite = 268435456U,
		AlwaysDepthTest = 536870912U,
		RenderToAmbientOcclusionBuffer = 1073741824U
	}
}
