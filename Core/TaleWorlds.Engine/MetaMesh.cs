using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000066 RID: 102
	[EngineClass("rglMeta_mesh")]
	public sealed class MetaMesh : GameEntityComponent
	{
		// Token: 0x06000803 RID: 2051 RVA: 0x00007CAA File Offset: 0x00005EAA
		internal MetaMesh(UIntPtr pointer)
			: base(pointer)
		{
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x00007CB3 File Offset: 0x00005EB3
		public static MetaMesh CreateMetaMesh(string name = null)
		{
			return EngineApplicationInterface.IMetaMesh.CreateMetaMesh(name);
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000805 RID: 2053 RVA: 0x00007CC0 File Offset: 0x00005EC0
		public bool IsValid
		{
			get
			{
				return base.Pointer != UIntPtr.Zero;
			}
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x00007CD2 File Offset: 0x00005ED2
		public int GetLodMaskForMeshAtIndex(int index)
		{
			return EngineApplicationInterface.IMetaMesh.GetLodMaskForMeshAtIndex(base.Pointer, index);
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x00007CE5 File Offset: 0x00005EE5
		public int GetTotalGpuSize()
		{
			return EngineApplicationInterface.IMetaMesh.GetTotalGpuSize(base.Pointer);
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x00007CF7 File Offset: 0x00005EF7
		public int RemoveMeshesWithTag(string tag)
		{
			return EngineApplicationInterface.IMetaMesh.RemoveMeshesWithTag(base.Pointer, tag);
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x00007D0A File Offset: 0x00005F0A
		public int RemoveMeshesWithoutTag(string tag)
		{
			return EngineApplicationInterface.IMetaMesh.RemoveMeshesWithoutTag(base.Pointer, tag);
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x00007D1D File Offset: 0x00005F1D
		public int GetMeshCountWithTag(string tag)
		{
			return EngineApplicationInterface.IMetaMesh.GetMeshCountWithTag(base.Pointer, tag);
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x00007D30 File Offset: 0x00005F30
		public bool HasVertexBufferOrEditDataOrPackageItem()
		{
			return EngineApplicationInterface.IMetaMesh.HasVertexBufferOrEditDataOrPackageItem(base.Pointer);
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x00007D42 File Offset: 0x00005F42
		public bool HasAnyGeneratedLods()
		{
			return EngineApplicationInterface.IMetaMesh.HasAnyGeneratedLods(base.Pointer);
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x00007D54 File Offset: 0x00005F54
		public bool HasAnyLods()
		{
			return EngineApplicationInterface.IMetaMesh.HasAnyLods(base.Pointer);
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x00007D66 File Offset: 0x00005F66
		public static MetaMesh GetCopy(string metaMeshName, bool showErrors = true, bool mayReturnNull = false)
		{
			return EngineApplicationInterface.IMetaMesh.CreateCopyFromName(metaMeshName, showErrors, mayReturnNull);
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x00007D75 File Offset: 0x00005F75
		public void CopyTo(MetaMesh res, bool copyMeshes = true)
		{
			EngineApplicationInterface.IMetaMesh.CopyTo(base.Pointer, res.Pointer, copyMeshes);
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x00007D8E File Offset: 0x00005F8E
		public void ClearMeshesForOtherLods(int lodToKeep)
		{
			EngineApplicationInterface.IMetaMesh.ClearMeshesForOtherLods(base.Pointer, lodToKeep);
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x00007DA1 File Offset: 0x00005FA1
		public void ClearMeshesForLod(int lodToClear)
		{
			EngineApplicationInterface.IMetaMesh.ClearMeshesForLod(base.Pointer, lodToClear);
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x00007DB4 File Offset: 0x00005FB4
		public void ClearMeshesForLowerLods(int lodToClear)
		{
			EngineApplicationInterface.IMetaMesh.ClearMeshesForLowerLods(base.Pointer, lodToClear);
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x00007DC7 File Offset: 0x00005FC7
		public void ClearMeshes()
		{
			EngineApplicationInterface.IMetaMesh.ClearMeshes(base.Pointer);
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x00007DD9 File Offset: 0x00005FD9
		public void SetNumLods(int lodToClear)
		{
			EngineApplicationInterface.IMetaMesh.SetNumLods(base.Pointer, lodToClear);
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x00007DEC File Offset: 0x00005FEC
		public static void CheckMetaMeshExistence(string metaMeshName, int lod_count_check)
		{
			EngineApplicationInterface.IMetaMesh.CheckMetaMeshExistence(metaMeshName, lod_count_check);
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x00007DFA File Offset: 0x00005FFA
		public static MetaMesh GetMorphedCopy(string metaMeshName, float morphTarget, bool showErrors)
		{
			return EngineApplicationInterface.IMetaMesh.GetMorphedCopy(metaMeshName, morphTarget, showErrors);
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x00007E09 File Offset: 0x00006009
		public MetaMesh CreateCopy()
		{
			return EngineApplicationInterface.IMetaMesh.CreateCopy(base.Pointer);
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x00007E1B File Offset: 0x0000601B
		public void AddMesh(Mesh mesh)
		{
			EngineApplicationInterface.IMetaMesh.AddMesh(base.Pointer, mesh.Pointer, 0U);
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x00007E34 File Offset: 0x00006034
		public void AddMesh(Mesh mesh, uint lodLevel)
		{
			EngineApplicationInterface.IMetaMesh.AddMesh(base.Pointer, mesh.Pointer, lodLevel);
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x00007E4D File Offset: 0x0000604D
		public void AddMetaMesh(MetaMesh metaMesh)
		{
			EngineApplicationInterface.IMetaMesh.AddMetaMesh(base.Pointer, metaMesh.Pointer);
		}

		// Token: 0x0600081B RID: 2075 RVA: 0x00007E65 File Offset: 0x00006065
		public void SetCullMode(MBMeshCullingMode cullMode)
		{
			EngineApplicationInterface.IMetaMesh.SetCullMode(base.Pointer, cullMode);
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x00007E78 File Offset: 0x00006078
		public void AddMaterialShaderFlag(string materialShaderFlag)
		{
			for (int i = 0; i < this.MeshCount; i++)
			{
				Mesh meshAtIndex = this.GetMeshAtIndex(i);
				Material material = meshAtIndex.GetMaterial();
				material = material.CreateCopy();
				material.AddMaterialShaderFlag(materialShaderFlag, false);
				meshAtIndex.SetMaterial(material);
			}
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x00007EB9 File Offset: 0x000060B9
		public void MergeMultiMeshes(MetaMesh metaMesh)
		{
			EngineApplicationInterface.IMetaMesh.MergeMultiMeshes(base.Pointer, metaMesh.Pointer);
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x00007ED1 File Offset: 0x000060D1
		public void AssignClothBodyFrom(MetaMesh metaMesh)
		{
			EngineApplicationInterface.IMetaMesh.AssignClothBodyFrom(base.Pointer, metaMesh.Pointer);
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x00007EE9 File Offset: 0x000060E9
		public void BatchMultiMeshes(MetaMesh metaMesh)
		{
			EngineApplicationInterface.IMetaMesh.BatchMultiMeshes(base.Pointer, metaMesh.Pointer);
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x00007F01 File Offset: 0x00006101
		public bool HasClothData()
		{
			return EngineApplicationInterface.IMetaMesh.HasClothData(base.Pointer);
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x00007F14 File Offset: 0x00006114
		public void BatchMultiMeshesMultiple(List<MetaMesh> metaMeshes)
		{
			UIntPtr[] array = new UIntPtr[metaMeshes.Count];
			for (int i = 0; i < metaMeshes.Count; i++)
			{
				array[i] = metaMeshes[i].Pointer;
			}
			EngineApplicationInterface.IMetaMesh.BatchMultiMeshesMultiple(base.Pointer, array, metaMeshes.Count);
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x00007F64 File Offset: 0x00006164
		public void ClearEditData()
		{
			EngineApplicationInterface.IMetaMesh.ClearEditData(base.Pointer);
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000823 RID: 2083 RVA: 0x00007F76 File Offset: 0x00006176
		public int MeshCount
		{
			get
			{
				return EngineApplicationInterface.IMetaMesh.GetMeshCount(base.Pointer);
			}
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x00007F88 File Offset: 0x00006188
		public Mesh GetMeshAtIndex(int meshIndex)
		{
			return EngineApplicationInterface.IMetaMesh.GetMeshAtIndex(base.Pointer, meshIndex);
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x00007F9C File Offset: 0x0000619C
		public Mesh GetFirstMeshWithTag(string tag)
		{
			for (int i = 0; i < this.MeshCount; i++)
			{
				Mesh meshAtIndex = this.GetMeshAtIndex(i);
				if (meshAtIndex.HasTag(tag))
				{
					return meshAtIndex;
				}
			}
			return null;
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x00007FCE File Offset: 0x000061CE
		private void Release()
		{
			EngineApplicationInterface.IMetaMesh.Release(base.Pointer);
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x00007FE0 File Offset: 0x000061E0
		public uint GetFactor1()
		{
			return EngineApplicationInterface.IMetaMesh.GetFactor1(base.Pointer);
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x00007FF2 File Offset: 0x000061F2
		public void SetGlossMultiplier(float value)
		{
			EngineApplicationInterface.IMetaMesh.SetGlossMultiplier(base.Pointer, value);
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x00008005 File Offset: 0x00006205
		public uint GetFactor2()
		{
			return EngineApplicationInterface.IMetaMesh.GetFactor2(base.Pointer);
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x00008017 File Offset: 0x00006217
		public void SetFactor1Linear(uint linearFactorColor1)
		{
			EngineApplicationInterface.IMetaMesh.SetFactor1Linear(base.Pointer, linearFactorColor1);
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0000802A File Offset: 0x0000622A
		public void SetFactor2Linear(uint linearFactorColor2)
		{
			EngineApplicationInterface.IMetaMesh.SetFactor2Linear(base.Pointer, linearFactorColor2);
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x0000803D File Offset: 0x0000623D
		public void SetFactor1(uint factorColor1)
		{
			EngineApplicationInterface.IMetaMesh.SetFactor1(base.Pointer, factorColor1);
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x00008050 File Offset: 0x00006250
		public void SetFactor2(uint factorColor2)
		{
			EngineApplicationInterface.IMetaMesh.SetFactor2(base.Pointer, factorColor2);
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x00008063 File Offset: 0x00006263
		public void SetVectorArgument(float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3)
		{
			EngineApplicationInterface.IMetaMesh.SetVectorArgument(base.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x0000807A File Offset: 0x0000627A
		public void SetVectorArgument2(float vectorArgument0, float vectorArgument1, float vectorArgument2, float vectorArgument3)
		{
			EngineApplicationInterface.IMetaMesh.SetVectorArgument2(base.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x00008091 File Offset: 0x00006291
		public Vec3 GetVectorArgument2()
		{
			return EngineApplicationInterface.IMetaMesh.GetVectorArgument2(base.Pointer);
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x000080A3 File Offset: 0x000062A3
		public void SetMaterial(Material material)
		{
			EngineApplicationInterface.IMetaMesh.SetMaterial(base.Pointer, material.Pointer);
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x000080BB File Offset: 0x000062BB
		public void SetLodBias(int lodBias)
		{
			EngineApplicationInterface.IMetaMesh.SetLodBias(base.Pointer, lodBias);
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x000080CE File Offset: 0x000062CE
		public void SetBillboarding(BillboardType billboard)
		{
			EngineApplicationInterface.IMetaMesh.SetBillboarding(base.Pointer, billboard);
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x000080E1 File Offset: 0x000062E1
		public void UseHeadBoneFaceGenScaling(Skeleton skeleton, sbyte headLookDirectionBoneIndex, MatrixFrame frame)
		{
			EngineApplicationInterface.IMetaMesh.UseHeadBoneFaceGenScaling(base.Pointer, skeleton.Pointer, headLookDirectionBoneIndex, ref frame);
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x000080FC File Offset: 0x000062FC
		public void DrawTextWithDefaultFont(string text, Vec2 textPositionMin, Vec2 textPositionMax, Vec2 size, uint color, TextFlags flags)
		{
			EngineApplicationInterface.IMetaMesh.DrawTextWithDefaultFont(base.Pointer, text, textPositionMin, textPositionMax, size, color, flags);
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000836 RID: 2102 RVA: 0x00008118 File Offset: 0x00006318
		// (set) Token: 0x06000837 RID: 2103 RVA: 0x00008140 File Offset: 0x00006340
		public MatrixFrame Frame
		{
			get
			{
				MatrixFrame matrixFrame = default(MatrixFrame);
				EngineApplicationInterface.IMetaMesh.GetFrame(base.Pointer, ref matrixFrame);
				return matrixFrame;
			}
			set
			{
				EngineApplicationInterface.IMetaMesh.SetFrame(base.Pointer, ref value);
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000838 RID: 2104 RVA: 0x00008154 File Offset: 0x00006354
		// (set) Token: 0x06000839 RID: 2105 RVA: 0x00008166 File Offset: 0x00006366
		public Vec3 VectorUserData
		{
			get
			{
				return EngineApplicationInterface.IMetaMesh.GetVectorUserData(base.Pointer);
			}
			set
			{
				EngineApplicationInterface.IMetaMesh.SetVectorUserData(base.Pointer, ref value);
			}
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x0000817A File Offset: 0x0000637A
		public void PreloadForRendering()
		{
			EngineApplicationInterface.IMetaMesh.PreloadForRendering(base.Pointer);
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x0000818C File Offset: 0x0000638C
		public int CheckResources()
		{
			return EngineApplicationInterface.IMetaMesh.CheckResources(base.Pointer);
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x0000819E File Offset: 0x0000639E
		public void PreloadShaders(bool useTableau, bool useTeamColor)
		{
			EngineApplicationInterface.IMetaMesh.PreloadShaders(base.Pointer, useTableau, useTeamColor);
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x000081B2 File Offset: 0x000063B2
		public void RecomputeBoundingBox(bool recomputeMeshes)
		{
			EngineApplicationInterface.IMetaMesh.RecomputeBoundingBox(base.Pointer, recomputeMeshes);
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x000081C5 File Offset: 0x000063C5
		public void AddEditDataUser()
		{
			EngineApplicationInterface.IMetaMesh.AddEditDataUser(base.Pointer);
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x000081D7 File Offset: 0x000063D7
		public void ReleaseEditDataUser()
		{
			EngineApplicationInterface.IMetaMesh.ReleaseEditDataUser(base.Pointer);
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x000081E9 File Offset: 0x000063E9
		public void SetEditDataPolicy(EditDataPolicy policy)
		{
			EngineApplicationInterface.IMetaMesh.SetEditDataPolicy(base.Pointer, policy);
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x000081FC File Offset: 0x000063FC
		public MatrixFrame Fit()
		{
			MatrixFrame identity = MatrixFrame.Identity;
			Vec3 vec = new Vec3(1000000f, 1000000f, 1000000f, -1f);
			Vec3 vec2 = new Vec3(-1000000f, -1000000f, -1000000f, -1f);
			for (int num = 0; num != this.MeshCount; num++)
			{
				Vec3 boundingBoxMin = this.GetMeshAtIndex(num).GetBoundingBoxMin();
				Vec3 boundingBoxMax = this.GetMeshAtIndex(num).GetBoundingBoxMax();
				vec = Vec3.Vec3Min(vec, boundingBoxMin);
				vec2 = Vec3.Vec3Max(vec2, boundingBoxMax);
			}
			Vec3 vec3 = (vec + vec2) * 0.5f;
			float num2 = MathF.Max(vec2.x - vec.x, vec2.y - vec.y);
			float num3 = 0.95f / num2;
			identity.origin -= vec3 * num3;
			identity.rotation.ApplyScaleLocal(num3);
			return identity;
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x000082F8 File Offset: 0x000064F8
		public BoundingBox GetBoundingBox()
		{
			BoundingBox boundingBox = default(BoundingBox);
			EngineApplicationInterface.IMetaMesh.GetBoundingBox(base.Pointer, ref boundingBox);
			return boundingBox;
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x00008320 File Offset: 0x00006520
		public VisibilityMaskFlags GetVisibilityMask()
		{
			return EngineApplicationInterface.IMetaMesh.GetVisibilityMask(base.Pointer);
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x00008332 File Offset: 0x00006532
		public void SetVisibilityMask(VisibilityMaskFlags visibilityMask)
		{
			EngineApplicationInterface.IMetaMesh.SetVisibilityMask(base.Pointer, visibilityMask);
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x00008345 File Offset: 0x00006545
		public string GetName()
		{
			return EngineApplicationInterface.IMetaMesh.GetName(base.Pointer);
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x00008358 File Offset: 0x00006558
		public static void GetAllMultiMeshes(ref List<MetaMesh> multiMeshList)
		{
			int multiMeshCount = EngineApplicationInterface.IMetaMesh.GetMultiMeshCount();
			UIntPtr[] array = new UIntPtr[multiMeshCount];
			EngineApplicationInterface.IMetaMesh.GetAllMultiMeshes(array);
			for (int i = 0; i < multiMeshCount; i++)
			{
				multiMeshList.Add(new MetaMesh(array[i]));
			}
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x0000839E File Offset: 0x0000659E
		public static MetaMesh GetMultiMesh(string name)
		{
			return EngineApplicationInterface.IMetaMesh.GetMultiMesh(name);
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x000083AB File Offset: 0x000065AB
		public void SetContourState(bool alwaysVisible)
		{
			EngineApplicationInterface.IMetaMesh.SetContourState(base.Pointer, alwaysVisible);
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x000083BE File Offset: 0x000065BE
		public void SetContourColor(uint color)
		{
			EngineApplicationInterface.IMetaMesh.SetContourColor(base.Pointer, color);
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x000083D1 File Offset: 0x000065D1
		public void SetMaterialToSubMeshesWithTag(Material bodyMaterial, string tag)
		{
			EngineApplicationInterface.IMetaMesh.SetMaterialToSubMeshesWithTag(base.Pointer, bodyMaterial.Pointer, tag);
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x000083EA File Offset: 0x000065EA
		public void SetFactorColorToSubMeshesWithTag(uint color, string tag)
		{
			EngineApplicationInterface.IMetaMesh.SetFactorColorToSubMeshesWithTag(base.Pointer, color, tag);
		}
	}
}
