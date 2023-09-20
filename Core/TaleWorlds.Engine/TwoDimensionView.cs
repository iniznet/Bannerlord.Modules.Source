using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x0200008F RID: 143
	[EngineClass("rglTwo_dimension_view")]
	public sealed class TwoDimensionView : View
	{
		// Token: 0x06000ABD RID: 2749 RVA: 0x0000BCBD File Offset: 0x00009EBD
		internal TwoDimensionView(UIntPtr pointer)
			: base(pointer)
		{
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x0000BCC6 File Offset: 0x00009EC6
		public static TwoDimensionView CreateTwoDimension()
		{
			return EngineApplicationInterface.ITwoDimensionView.CreateTwoDimensionView();
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x0000BCD2 File Offset: 0x00009ED2
		public void BeginFrame()
		{
			EngineApplicationInterface.ITwoDimensionView.BeginFrame(base.Pointer);
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x0000BCE4 File Offset: 0x00009EE4
		public void EndFrame()
		{
			EngineApplicationInterface.ITwoDimensionView.EndFrame(base.Pointer);
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x0000BCF6 File Offset: 0x00009EF6
		public void Clear()
		{
			EngineApplicationInterface.ITwoDimensionView.Clear(base.Pointer);
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x0000BD08 File Offset: 0x00009F08
		public void CreateMeshFromDescription(float[] vertices, float[] uvs, uint[] indices, int indexCount, Material material, TwoDimensionMeshDrawData meshDrawData)
		{
			EngineApplicationInterface.ITwoDimensionView.AddNewMesh(base.Pointer, vertices, uvs, indices, vertices.Length / 2, indexCount, material.Pointer, ref meshDrawData);
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x0000BD38 File Offset: 0x00009F38
		public void CreateMeshFromDescription(Material material, TwoDimensionMeshDrawData meshDrawData)
		{
			EngineApplicationInterface.ITwoDimensionView.AddNewQuadMesh(base.Pointer, material.Pointer, ref meshDrawData);
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0000BD52 File Offset: 0x00009F52
		public bool CreateTextMeshFromCache(Material material, TwoDimensionTextMeshDrawData meshDrawData)
		{
			return EngineApplicationInterface.ITwoDimensionView.AddCachedTextMesh(base.Pointer, material.Pointer, ref meshDrawData);
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0000BD6C File Offset: 0x00009F6C
		public void CreateTextMeshFromDescription(float[] vertices, float[] uvs, uint[] indices, int indexCount, Material material, TwoDimensionTextMeshDrawData meshDrawData)
		{
			EngineApplicationInterface.ITwoDimensionView.AddNewTextMesh(base.Pointer, vertices, uvs, indices, vertices.Length / 2, indexCount, material.Pointer, ref meshDrawData);
		}
	}
}
