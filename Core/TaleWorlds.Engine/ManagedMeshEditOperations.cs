using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000055 RID: 85
	[EngineClass("rglManaged_mesh_edit_operations")]
	public sealed class ManagedMeshEditOperations : NativeObject
	{
		// Token: 0x060006FA RID: 1786 RVA: 0x00005B28 File Offset: 0x00003D28
		internal ManagedMeshEditOperations(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x00005B37 File Offset: 0x00003D37
		public static ManagedMeshEditOperations Create(Mesh meshToEdit)
		{
			return EngineApplicationInterface.IManagedMeshEditOperations.Create(meshToEdit.Pointer);
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x00005B49 File Offset: 0x00003D49
		public void Weld()
		{
			EngineApplicationInterface.IManagedMeshEditOperations.Weld(base.Pointer);
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x00005B5B File Offset: 0x00003D5B
		public int AddVertex(Vec3 vertexPos)
		{
			return EngineApplicationInterface.IManagedMeshEditOperations.AddVertex(base.Pointer, ref vertexPos);
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x00005B6F File Offset: 0x00003D6F
		public int AddFaceCorner(int vertexIndex, Vec2 uv0, Vec3 color, Vec3 normal)
		{
			return EngineApplicationInterface.IManagedMeshEditOperations.AddFaceCorner1(base.Pointer, vertexIndex, ref uv0, ref color, ref normal);
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x00005B88 File Offset: 0x00003D88
		public int AddFaceCorner(int vertexIndex, Vec2 uv0, Vec2 uv1, Vec3 color, Vec3 normal)
		{
			return EngineApplicationInterface.IManagedMeshEditOperations.AddFaceCorner2(base.Pointer, vertexIndex, ref uv0, ref uv1, ref color, ref normal);
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x00005BA3 File Offset: 0x00003DA3
		public int AddFace(int patchNode0, int patchNode1, int patchNode2)
		{
			return EngineApplicationInterface.IManagedMeshEditOperations.AddFace(base.Pointer, patchNode0, patchNode1, patchNode2);
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x00005BB8 File Offset: 0x00003DB8
		public void AddTriangle(Vec3 p1, Vec3 p2, Vec3 p3, Vec2 uv1, Vec2 uv2, Vec2 uv3, Vec3 color)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddTriangle1(base.Pointer, ref p1, ref p2, ref p3, ref uv1, ref uv2, ref uv3, ref color);
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x00005BE4 File Offset: 0x00003DE4
		public void AddTriangle(Vec3 p1, Vec3 p2, Vec3 p3, Vec3 n1, Vec3 n2, Vec3 n3, Vec2 uv1, Vec2 uv2, Vec2 uv3, Vec3 c1, Vec3 c2, Vec3 c3)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddTriangle2(base.Pointer, ref p1, ref p2, ref p3, ref n1, ref n2, ref n3, ref uv1, ref uv2, ref uv3, ref c1, ref c2, ref c3);
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x00005C19 File Offset: 0x00003E19
		public void AddRectangle3(Vec3 o, Vec2 size, Vec2 uv_origin, Vec2 uvSize, Vec3 color)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddRectangle3(base.Pointer, ref o, ref size, ref uv_origin, ref uvSize, ref color);
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x00005C35 File Offset: 0x00003E35
		public void AddRectangleWithInverseUV(Vec3 o, Vec2 size, Vec2 uv_origin, Vec2 uvSize, Vec3 color)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddRectangleWithInverseUV(base.Pointer, ref o, ref size, ref uv_origin, ref uvSize, ref color);
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x00005C51 File Offset: 0x00003E51
		public void AddRect(Vec3 originBegin, Vec3 originEnd, Vec2 uvBegin, Vec2 uvEnd, Vec3 color)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddRect(base.Pointer, ref originBegin, ref originEnd, ref uvBegin, ref uvEnd, ref color);
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x00005C6D File Offset: 0x00003E6D
		public void AddRectWithZUp(Vec3 originBegin, Vec3 originEnd, Vec2 uvBegin, Vec2 uvEnd, Vec3 color)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddRectWithZUp(base.Pointer, ref originBegin, ref originEnd, ref uvBegin, ref uvEnd, ref color);
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x00005C89 File Offset: 0x00003E89
		public void InvertFacesWindingOrder()
		{
			EngineApplicationInterface.IManagedMeshEditOperations.InvertFacesWindingOrder(base.Pointer);
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x00005C9B File Offset: 0x00003E9B
		public void ScaleVertices(float newScale)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.ScaleVertices1(base.Pointer, newScale);
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00005CAE File Offset: 0x00003EAE
		public void MoveVerticesAlongNormal(float moveAmount)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.MoveVerticesAlongNormal(base.Pointer, moveAmount);
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x00005CC1 File Offset: 0x00003EC1
		public void ScaleVertices(Vec3 newScale, bool keepUvX = false, float maxUvSize = 1f)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.ScaleVertices2(base.Pointer, ref newScale, keepUvX, maxUvSize);
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x00005CD7 File Offset: 0x00003ED7
		public void TranslateVertices(Vec3 newOrigin)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.TranslateVertices(base.Pointer, ref newOrigin);
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x00005CEC File Offset: 0x00003EEC
		public void AddMeshAux(Mesh mesh, MatrixFrame frame, sbyte boneNo, Vec3 color, bool transformNormal, bool heightGradient, bool addSkinData, bool useDoublePrecision = true)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddMeshAux(base.Pointer, mesh.Pointer, ref frame, boneNo, ref color, transformNormal, heightGradient, addSkinData, useDoublePrecision);
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x00005D1C File Offset: 0x00003F1C
		public int ComputeTangents(bool checkFixedNormals)
		{
			return EngineApplicationInterface.IManagedMeshEditOperations.ComputeTangents(base.Pointer, checkFixedNormals);
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x00005D2F File Offset: 0x00003F2F
		public void GenerateGrid(Vec2i numEdges, Vec2 edgeScale)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.GenerateGrid(base.Pointer, ref numEdges, ref edgeScale);
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x00005D45 File Offset: 0x00003F45
		public void RescaleMesh2d(Vec2 scaleSizeMin, Vec2 scaleSizeMax)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2d(base.Pointer, ref scaleSizeMin, ref scaleSizeMax);
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x00005D5B File Offset: 0x00003F5B
		public void RescaleMesh2dRepeatX(Vec2 scaleSizeMin, Vec2 scaleSizeMax, float frameThickness = 0f, int frameSide = 0)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2dRepeatX(base.Pointer, ref scaleSizeMin, ref scaleSizeMax, frameThickness, frameSide);
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x00005D74 File Offset: 0x00003F74
		public void RescaleMesh2dRepeatY(Vec2 scaleSizeMin, Vec2 scaleSizeMax, float frameThickness = 0f, int frameSide = 0)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2dRepeatY(base.Pointer, ref scaleSizeMin, ref scaleSizeMax, frameThickness, frameSide);
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x00005D8D File Offset: 0x00003F8D
		public void RescaleMesh2dRepeatXWithTiling(Vec2 scaleSizeMin, Vec2 scaleSizeMax, float frameThickness = 0f, int frameSide = 0, float xyRatio = 0f)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2dRepeatXWithTiling(base.Pointer, ref scaleSizeMin, ref scaleSizeMax, frameThickness, frameSide, xyRatio);
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x00005DA8 File Offset: 0x00003FA8
		public void RescaleMesh2dRepeatYWithTiling(Vec2 scaleSizeMin, Vec2 scaleSizeMax, float frameThickness = 0f, int frameSide = 0, float xyRatio = 0f)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2dRepeatYWithTiling(base.Pointer, ref scaleSizeMin, ref scaleSizeMax, frameThickness, frameSide, xyRatio);
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x00005DC3 File Offset: 0x00003FC3
		public void RescaleMesh2dWithoutChangingUV(Vec2 scaleSizeMin, Vec2 scaleSizeMax, float remaining)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2dRepeatYWithTiling(base.Pointer, ref scaleSizeMin, ref scaleSizeMax, remaining, 0, 0f);
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x00005DE0 File Offset: 0x00003FE0
		public void AddLine(Vec3 start, Vec3 end, Vec3 color, float lineWidth = 0.004f)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddLine(base.Pointer, ref start, ref end, ref color, lineWidth);
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x00005DFA File Offset: 0x00003FFA
		public void ComputeCornerNormals(bool checkFixedNormals = false, bool smoothCornerNormals = true)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.ComputeCornerNormals(base.Pointer, checkFixedNormals, smoothCornerNormals);
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x00005E0E File Offset: 0x0000400E
		public void ComputeCornerNormalsWithSmoothingData()
		{
			EngineApplicationInterface.IManagedMeshEditOperations.ComputeCornerNormalsWithSmoothingData(base.Pointer);
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x00005E20 File Offset: 0x00004020
		public void AddMesh(Mesh mesh, MatrixFrame frame)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddMesh(base.Pointer, mesh.Pointer, ref frame);
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x00005E3A File Offset: 0x0000403A
		public void AddMeshWithSkinData(Mesh mesh, MatrixFrame frame, sbyte boneIndex)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddMeshWithSkinData(base.Pointer, mesh.Pointer, ref frame, boneIndex);
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x00005E55 File Offset: 0x00004055
		public void AddMeshWithColor(Mesh mesh, MatrixFrame frame, Vec3 vertexColor, bool useDoublePrecision = true)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddMeshWithColor(base.Pointer, mesh.Pointer, ref frame, ref vertexColor, useDoublePrecision);
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00005E73 File Offset: 0x00004073
		public void AddMeshToBone(Mesh mesh, MatrixFrame frame, sbyte boneIndex)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddMeshToBone(base.Pointer, mesh.Pointer, ref frame, boneIndex);
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x00005E8E File Offset: 0x0000408E
		public void AddMeshWithFixedNormals(Mesh mesh, MatrixFrame frame)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddMeshWithFixedNormals(base.Pointer, mesh.Pointer, ref frame);
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x00005EA8 File Offset: 0x000040A8
		public void AddMeshWithFixedNormalsWithHeightGradientColor(Mesh mesh, MatrixFrame frame)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddMeshWithFixedNormalsWithHeightGradientColor(base.Pointer, mesh.Pointer, ref frame);
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x00005EC2 File Offset: 0x000040C2
		public void AddSkinnedMeshWithColor(Mesh mesh, MatrixFrame frame, Vec3 vertexColor, bool useDoublePrecision = true)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.AddSkinnedMeshWithColor(base.Pointer, mesh.Pointer, ref frame, ref vertexColor, useDoublePrecision);
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x00005EE0 File Offset: 0x000040E0
		public void SetCornerVertexColor(int cornerNo, Vec3 vertexColor)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.SetCornerVertexColor(base.Pointer, cornerNo, ref vertexColor);
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x00005EF5 File Offset: 0x000040F5
		public void SetCornerUV(int cornerNo, Vec2 newUV, int uvNumber = 0)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.SetCornerUV(base.Pointer, cornerNo, ref newUV, uvNumber);
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x00005F0B File Offset: 0x0000410B
		public void ReserveVertices(int count)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.ReserveVertices(base.Pointer, count);
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x00005F1E File Offset: 0x0000411E
		public void ReserveFaceCorners(int count)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.ReserveFaceCorners(base.Pointer, count);
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x00005F31 File Offset: 0x00004131
		public void ReserveFaces(int count)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.ReserveFaces(base.Pointer, count);
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x00005F44 File Offset: 0x00004144
		public int RemoveDuplicatedCorners()
		{
			return EngineApplicationInterface.IManagedMeshEditOperations.RemoveDuplicatedCorners(base.Pointer);
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x00005F56 File Offset: 0x00004156
		public void TransformVerticesToParent(MatrixFrame frame)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.TransformVerticesToParent(base.Pointer, ref frame);
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x00005F6A File Offset: 0x0000416A
		public void TransformVerticesToLocal(MatrixFrame frame)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.TransformVerticesToLocal(base.Pointer, ref frame);
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x00005F7E File Offset: 0x0000417E
		public void SetVertexColor(Vec3 color)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.SetVertexColor(base.Pointer, ref color);
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x00005F92 File Offset: 0x00004192
		public Vec3 GetVertexColor(int faceCornerIndex)
		{
			return EngineApplicationInterface.IManagedMeshEditOperations.GetVertexColor(base.Pointer, faceCornerIndex);
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x00005FA5 File Offset: 0x000041A5
		public void SetVertexColorAlpha(float newAlpha)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.SetVertexColorAlpha(base.Pointer, newAlpha);
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x00005FB8 File Offset: 0x000041B8
		public float GetVertexColorAlpha()
		{
			return EngineApplicationInterface.IManagedMeshEditOperations.GetVertexColorAlpha(base.Pointer);
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x00005FCA File Offset: 0x000041CA
		public void EnsureTransformedVertices()
		{
			EngineApplicationInterface.IManagedMeshEditOperations.EnsureTransformedVertices(base.Pointer);
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x00005FDC File Offset: 0x000041DC
		public void ApplyCPUSkinning(Skeleton skeleton)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.ApplyCPUSkinning(base.Pointer, skeleton.Pointer);
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x00005FF4 File Offset: 0x000041F4
		public void UpdateOverlappedVertexNormals(Mesh attachedToMesh, MatrixFrame attachFrame, float mergeRadiusSQ = 0.0025f)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.UpdateOverlappedVertexNormals(base.Pointer, attachedToMesh.Pointer, ref attachFrame, mergeRadiusSQ);
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x0000600F File Offset: 0x0000420F
		public void ClearAll()
		{
			EngineApplicationInterface.IManagedMeshEditOperations.ClearAll(base.Pointer);
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x00006021 File Offset: 0x00004221
		public void SetTangentsOfFaceCorner(int faceCornerIndex, Vec3 tangent, Vec3 binormal)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.SetTangentsOfFaceCorner(base.Pointer, faceCornerIndex, ref tangent, ref binormal);
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x00006038 File Offset: 0x00004238
		public void SetPositionOfVertex(int vertexIndex, Vec3 position)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.SetPositionOfVertex(base.Pointer, vertexIndex, ref position);
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x0000604D File Offset: 0x0000424D
		public Vec3 GetPositionOfVertex(int vertexIndex)
		{
			return EngineApplicationInterface.IManagedMeshEditOperations.GetPositionOfVertex(base.Pointer, vertexIndex);
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x00006060 File Offset: 0x00004260
		public void RemoveFace(int faceIndex)
		{
			EngineApplicationInterface.IManagedMeshEditOperations.RemoveFace(base.Pointer, faceIndex);
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00006073 File Offset: 0x00004273
		public void FinalizeEditing()
		{
			EngineApplicationInterface.IManagedMeshEditOperations.FinalizeEditing(base.Pointer);
		}
	}
}
