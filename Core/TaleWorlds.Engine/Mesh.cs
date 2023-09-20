using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000063 RID: 99
	[EngineClass("rglMesh")]
	public sealed class Mesh : Resource
	{
		// Token: 0x060007B6 RID: 1974 RVA: 0x00007391 File Offset: 0x00005591
		internal Mesh(UIntPtr meshPointer)
			: base(meshPointer)
		{
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x0000739A File Offset: 0x0000559A
		public static Mesh CreateMeshWithMaterial(Material material)
		{
			return EngineApplicationInterface.IMesh.CreateMeshWithMaterial(material.Pointer);
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x000073AC File Offset: 0x000055AC
		public static Mesh CreateMesh(bool editable = true)
		{
			return EngineApplicationInterface.IMesh.CreateMesh(editable);
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x000073B9 File Offset: 0x000055B9
		public Mesh GetBaseMesh()
		{
			return EngineApplicationInterface.IMesh.GetBaseMesh(base.Pointer);
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x000073CB File Offset: 0x000055CB
		public static Mesh GetFromResource(string meshName)
		{
			return EngineApplicationInterface.IMesh.GetMeshFromResource(meshName);
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x000073D8 File Offset: 0x000055D8
		public static Mesh GetRandomMeshWithVdecl(int inputLayout)
		{
			return EngineApplicationInterface.IMesh.GetRandomMeshWithVdecl(inputLayout);
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x000073E5 File Offset: 0x000055E5
		public void SetColorAndStroke(uint color, uint strokeColor, bool drawStroke)
		{
			this.Color = color;
			this.Color2 = strokeColor;
			EngineApplicationInterface.IMesh.SetColorAndStroke(base.Pointer, drawStroke);
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x00007406 File Offset: 0x00005606
		public void SetMeshRenderOrder(int renderOrder)
		{
			EngineApplicationInterface.IMesh.SetMeshRenderOrder(base.Pointer, renderOrder);
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x00007419 File Offset: 0x00005619
		public bool HasTag(string str)
		{
			return EngineApplicationInterface.IMesh.HasTag(base.Pointer, str);
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x0000742C File Offset: 0x0000562C
		public Mesh CreateCopy()
		{
			return EngineApplicationInterface.IMesh.CreateMeshCopy(base.Pointer);
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x0000743E File Offset: 0x0000563E
		public void SetMaterial(string newMaterialName)
		{
			EngineApplicationInterface.IMesh.SetMaterialByName(base.Pointer, newMaterialName);
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x00007451 File Offset: 0x00005651
		public void SetVectorArgument(float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3)
		{
			EngineApplicationInterface.IMesh.SetVectorArgument(base.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x00007468 File Offset: 0x00005668
		public void SetVectorArgument2(float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3)
		{
			EngineApplicationInterface.IMesh.SetVectorArgument2(base.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x0000747F File Offset: 0x0000567F
		public void SetMaterial(Material material)
		{
			EngineApplicationInterface.IMesh.SetMaterial(base.Pointer, material.Pointer);
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x00007497 File Offset: 0x00005697
		public Material GetMaterial()
		{
			return EngineApplicationInterface.IMesh.GetMaterial(base.Pointer);
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x000074A9 File Offset: 0x000056A9
		public Material GetSecondMaterial()
		{
			return EngineApplicationInterface.IMesh.GetSecondMaterial(base.Pointer);
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x000074BB File Offset: 0x000056BB
		public int AddFaceCorner(Vec3 position, Vec3 normal, Vec2 uvCoord, uint color, UIntPtr lockHandle)
		{
			if (base.IsValid)
			{
				return EngineApplicationInterface.IMesh.AddFaceCorner(base.Pointer, position, normal, uvCoord, color, lockHandle);
			}
			return -1;
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x000074DE File Offset: 0x000056DE
		public int AddFace(int patchNode0, int patchNode1, int patchNode2, UIntPtr lockHandle)
		{
			if (base.IsValid)
			{
				return EngineApplicationInterface.IMesh.AddFace(base.Pointer, patchNode0, patchNode1, patchNode2, lockHandle);
			}
			return -1;
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x000074FF File Offset: 0x000056FF
		public void ClearMesh()
		{
			if (base.IsValid)
			{
				EngineApplicationInterface.IMesh.ClearMesh(base.Pointer);
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060007C9 RID: 1993 RVA: 0x00007519 File Offset: 0x00005719
		// (set) Token: 0x060007CA RID: 1994 RVA: 0x00007539 File Offset: 0x00005739
		public string Name
		{
			get
			{
				if (base.IsValid)
				{
					return EngineApplicationInterface.IMesh.GetName(base.Pointer);
				}
				return string.Empty;
			}
			set
			{
				EngineApplicationInterface.IMesh.SetName(base.Pointer, value);
			}
		}

		// Token: 0x17000046 RID: 70
		// (set) Token: 0x060007CB RID: 1995 RVA: 0x0000754C File Offset: 0x0000574C
		public MBMeshCullingMode CullingMode
		{
			set
			{
				if (base.IsValid)
				{
					EngineApplicationInterface.IMesh.SetCullingMode(base.Pointer, (uint)value);
				}
			}
		}

		// Token: 0x17000047 RID: 71
		// (set) Token: 0x060007CC RID: 1996 RVA: 0x00007567 File Offset: 0x00005767
		public float MorphTime
		{
			set
			{
				if (base.IsValid)
				{
					EngineApplicationInterface.IMesh.SetMorphTime(base.Pointer, value);
				}
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060007CE RID: 1998 RVA: 0x000075B7 File Offset: 0x000057B7
		// (set) Token: 0x060007CD RID: 1997 RVA: 0x00007582 File Offset: 0x00005782
		public uint Color
		{
			get
			{
				return EngineApplicationInterface.IMesh.GetColor(base.Pointer);
			}
			set
			{
				if (base.IsValid)
				{
					EngineApplicationInterface.IMesh.SetColor(base.Pointer, value);
					return;
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\Mesh.cs", "Color", 301);
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060007D0 RID: 2000 RVA: 0x000075FE File Offset: 0x000057FE
		// (set) Token: 0x060007CF RID: 1999 RVA: 0x000075C9 File Offset: 0x000057C9
		public uint Color2
		{
			get
			{
				return EngineApplicationInterface.IMesh.GetColor2(base.Pointer);
			}
			set
			{
				if (base.IsValid)
				{
					EngineApplicationInterface.IMesh.SetColor2(base.Pointer, value);
					return;
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Engine\\TaleWorlds.Engine\\Mesh.cs", "Color2", 324);
			}
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x00007610 File Offset: 0x00005810
		public void SetColorAlpha(uint newAlpha)
		{
			if (base.IsValid)
			{
				EngineApplicationInterface.IMesh.SetColorAlpha(base.Pointer, newAlpha);
			}
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x0000762B File Offset: 0x0000582B
		public uint GetFaceCount()
		{
			if (!base.IsValid)
			{
				return 0U;
			}
			return EngineApplicationInterface.IMesh.GetFaceCount(base.Pointer);
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x00007647 File Offset: 0x00005847
		public uint GetFaceCornerCount()
		{
			if (!base.IsValid)
			{
				return 0U;
			}
			return EngineApplicationInterface.IMesh.GetFaceCornerCount(base.Pointer);
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x00007663 File Offset: 0x00005863
		public void ComputeNormals()
		{
			if (base.IsValid)
			{
				EngineApplicationInterface.IMesh.ComputeNormals(base.Pointer);
			}
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x0000767D File Offset: 0x0000587D
		public void ComputeTangents()
		{
			if (base.IsValid)
			{
				EngineApplicationInterface.IMesh.ComputeTangents(base.Pointer);
			}
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x00007698 File Offset: 0x00005898
		public void AddMesh(string meshResourceName, MatrixFrame meshFrame)
		{
			if (base.IsValid)
			{
				Mesh fromResource = Mesh.GetFromResource(meshResourceName);
				EngineApplicationInterface.IMesh.AddMeshToMesh(base.Pointer, fromResource.Pointer, ref meshFrame);
			}
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x000076CC File Offset: 0x000058CC
		public void AddMesh(Mesh mesh, MatrixFrame meshFrame)
		{
			if (base.IsValid)
			{
				EngineApplicationInterface.IMesh.AddMeshToMesh(base.Pointer, mesh.Pointer, ref meshFrame);
			}
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x000076F0 File Offset: 0x000058F0
		public MatrixFrame GetLocalFrame()
		{
			if (base.IsValid)
			{
				MatrixFrame matrixFrame = default(MatrixFrame);
				EngineApplicationInterface.IMesh.GetLocalFrame(base.Pointer, ref matrixFrame);
				return matrixFrame;
			}
			return default(MatrixFrame);
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0000772A File Offset: 0x0000592A
		public void SetLocalFrame(MatrixFrame meshFrame)
		{
			if (base.IsValid)
			{
				EngineApplicationInterface.IMesh.SetLocalFrame(base.Pointer, ref meshFrame);
			}
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x00007746 File Offset: 0x00005946
		public void SetVisibilityMask(VisibilityMaskFlags visibilityMask)
		{
			EngineApplicationInterface.IMesh.SetVisibilityMask(base.Pointer, visibilityMask);
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x00007759 File Offset: 0x00005959
		public void UpdateBoundingBox()
		{
			if (base.IsValid)
			{
				EngineApplicationInterface.IMesh.UpdateBoundingBox(base.Pointer);
			}
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x00007773 File Offset: 0x00005973
		public void SetAsNotEffectedBySeason()
		{
			EngineApplicationInterface.IMesh.SetAsNotEffectedBySeason(base.Pointer);
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x00007785 File Offset: 0x00005985
		public float GetBoundingBoxWidth()
		{
			if (!base.IsValid)
			{
				return 0f;
			}
			return EngineApplicationInterface.IMesh.GetBoundingBoxWidth(base.Pointer);
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x000077A5 File Offset: 0x000059A5
		public float GetBoundingBoxHeight()
		{
			if (!base.IsValid)
			{
				return 0f;
			}
			return EngineApplicationInterface.IMesh.GetBoundingBoxHeight(base.Pointer);
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x000077C5 File Offset: 0x000059C5
		public Vec3 GetBoundingBoxMin()
		{
			return EngineApplicationInterface.IMesh.GetBoundingBoxMin(base.Pointer);
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x000077D7 File Offset: 0x000059D7
		public Vec3 GetBoundingBoxMax()
		{
			return EngineApplicationInterface.IMesh.GetBoundingBoxMax(base.Pointer);
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x000077EC File Offset: 0x000059EC
		public void AddTriangle(Vec3 p1, Vec3 p2, Vec3 p3, Vec2 uv1, Vec2 uv2, Vec2 uv3, uint color, UIntPtr lockHandle)
		{
			EngineApplicationInterface.IMesh.AddTriangle(base.Pointer, p1, p2, p3, uv1, uv2, uv3, color, lockHandle);
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x00007818 File Offset: 0x00005A18
		public void AddTriangleWithVertexColors(Vec3 p1, Vec3 p2, Vec3 p3, Vec2 uv1, Vec2 uv2, Vec2 uv3, uint c1, uint c2, uint c3, UIntPtr lockHandle)
		{
			EngineApplicationInterface.IMesh.AddTriangleWithVertexColors(base.Pointer, p1, p2, p3, uv1, uv2, uv3, c1, c2, c3, lockHandle);
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x00007846 File Offset: 0x00005A46
		public void HintIndicesDynamic()
		{
			EngineApplicationInterface.IMesh.HintIndicesDynamic(base.Pointer);
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x00007858 File Offset: 0x00005A58
		public void HintVerticesDynamic()
		{
			EngineApplicationInterface.IMesh.HintVerticesDynamic(base.Pointer);
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x0000786A File Offset: 0x00005A6A
		public void RecomputeBoundingBox()
		{
			EngineApplicationInterface.IMesh.RecomputeBoundingBox(base.Pointer);
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060007E6 RID: 2022 RVA: 0x0000787C File Offset: 0x00005A7C
		// (set) Token: 0x060007E7 RID: 2023 RVA: 0x0000788E File Offset: 0x00005A8E
		public BillboardType Billboard
		{
			get
			{
				return EngineApplicationInterface.IMesh.GetBillboard(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IMesh.SetBillboard(base.Pointer, value);
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060007E8 RID: 2024 RVA: 0x000078A1 File Offset: 0x00005AA1
		// (set) Token: 0x060007E9 RID: 2025 RVA: 0x000078B3 File Offset: 0x00005AB3
		public VisibilityMaskFlags VisibilityMask
		{
			get
			{
				return EngineApplicationInterface.IMesh.GetVisibilityMask(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IMesh.SetVisibilityMask(base.Pointer, value);
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060007EA RID: 2026 RVA: 0x000078C6 File Offset: 0x00005AC6
		public int EditDataFaceCornerCount
		{
			get
			{
				return EngineApplicationInterface.IMesh.GetEditDataFaceCornerCount(base.Pointer);
			}
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x000078D8 File Offset: 0x00005AD8
		public void SetEditDataFaceCornerVertexColor(int index, uint color)
		{
			EngineApplicationInterface.IMesh.SetEditDataFaceCornerVertexColor(base.Pointer, index, color);
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x000078EC File Offset: 0x00005AEC
		public uint GetEditDataFaceCornerVertexColor(int index)
		{
			return EngineApplicationInterface.IMesh.GetEditDataFaceCornerVertexColor(base.Pointer, index);
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x000078FF File Offset: 0x00005AFF
		public void PreloadForRendering()
		{
			EngineApplicationInterface.IMesh.PreloadForRendering(base.Pointer);
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x00007911 File Offset: 0x00005B11
		public void SetContourColor(Vec3 color, bool alwaysVisible, bool maskMesh)
		{
			EngineApplicationInterface.IMesh.SetContourColor(base.Pointer, color, alwaysVisible, maskMesh);
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x00007926 File Offset: 0x00005B26
		public void DisableContour()
		{
			EngineApplicationInterface.IMesh.DisableContour(base.Pointer);
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x00007938 File Offset: 0x00005B38
		public void SetExternalBoundingBox(BoundingBox bbox)
		{
			EngineApplicationInterface.IMesh.SetExternalBoundingBox(base.Pointer, ref bbox);
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0000794C File Offset: 0x00005B4C
		public void AddEditDataUser()
		{
			EngineApplicationInterface.IMesh.AddEditDataUser(base.Pointer);
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0000795E File Offset: 0x00005B5E
		public void ReleaseEditDataUser()
		{
			EngineApplicationInterface.IMesh.ReleaseEditDataUser(base.Pointer);
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x00007970 File Offset: 0x00005B70
		public void SetEditDataPolicy(EditDataPolicy policy)
		{
			EngineApplicationInterface.IMesh.SetEditDataPolicy(base.Pointer, policy);
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x00007983 File Offset: 0x00005B83
		public UIntPtr LockEditDataWrite()
		{
			return EngineApplicationInterface.IMesh.LockEditDataWrite(base.Pointer);
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x00007995 File Offset: 0x00005B95
		public void UnlockEditDataWrite(UIntPtr handle)
		{
			EngineApplicationInterface.IMesh.UnlockEditDataWrite(base.Pointer, handle);
		}
	}
}
