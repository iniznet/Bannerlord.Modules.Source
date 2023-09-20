using System;

namespace TaleWorlds.Engine
{
	// Token: 0x02000058 RID: 88
	[Flags]
	public enum MaterialFlags : uint
	{
		// Token: 0x040000D3 RID: 211
		RenderFrontToBack = 1U,
		// Token: 0x040000D4 RID: 212
		NoDepthTest = 2U,
		// Token: 0x040000D5 RID: 213
		DontDrawToDepthRenderTarget = 4U,
		// Token: 0x040000D6 RID: 214
		NoModifyDepthBuffer = 8U,
		// Token: 0x040000D7 RID: 215
		CullFrontFaces = 16U,
		// Token: 0x040000D8 RID: 216
		TwoSided = 32U,
		// Token: 0x040000D9 RID: 217
		AlphaBlendSort = 64U,
		// Token: 0x040000DA RID: 218
		DontOptimizeMesh = 128U,
		// Token: 0x040000DB RID: 219
		AlphaBlendNone = 0U,
		// Token: 0x040000DC RID: 220
		AlphaBlendModulate = 256U,
		// Token: 0x040000DD RID: 221
		AlphaBlendAdd = 512U,
		// Token: 0x040000DE RID: 222
		AlphaBlendMultiply = 768U,
		// Token: 0x040000DF RID: 223
		AlphaBlendFactor = 1792U,
		// Token: 0x040000E0 RID: 224
		AlphaBlendMask = 1792U,
		// Token: 0x040000E1 RID: 225
		AlphaBlendBits = 8U,
		// Token: 0x040000E2 RID: 226
		BillboardNone = 0U,
		// Token: 0x040000E3 RID: 227
		Billboard2d = 4096U,
		// Token: 0x040000E4 RID: 228
		Billboard3d = 8192U,
		// Token: 0x040000E5 RID: 229
		BillboardMask = 12288U,
		// Token: 0x040000E6 RID: 230
		Skybox = 131072U,
		// Token: 0x040000E7 RID: 231
		MultiPassAlpha = 262144U,
		// Token: 0x040000E8 RID: 232
		GbufferAlphaBlend = 524288U,
		// Token: 0x040000E9 RID: 233
		RequiresForwardRendering = 1048576U,
		// Token: 0x040000EA RID: 234
		AvoidRecomputationOfNormals = 2097152U,
		// Token: 0x040000EB RID: 235
		RenderOrderPlus1 = 150994944U,
		// Token: 0x040000EC RID: 236
		RenderOrderPlus2 = 167772160U,
		// Token: 0x040000ED RID: 237
		RenderOrderPlus3 = 184549376U,
		// Token: 0x040000EE RID: 238
		RenderOrderPlus4 = 201326592U,
		// Token: 0x040000EF RID: 239
		RenderOrderPlus5 = 218103808U,
		// Token: 0x040000F0 RID: 240
		RenderOrderPlus6 = 234881024U,
		// Token: 0x040000F1 RID: 241
		RenderOrderPlus7 = 251658240U,
		// Token: 0x040000F2 RID: 242
		GreaterDepthNoWrite = 268435456U,
		// Token: 0x040000F3 RID: 243
		AlwaysDepthTest = 536870912U,
		// Token: 0x040000F4 RID: 244
		RenderToAmbientOcclusionBuffer = 1073741824U
	}
}
