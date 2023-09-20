using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000086 RID: 134
	[EngineClass("rglSkeleton")]
	public sealed class Skeleton : NativeObject
	{
		// Token: 0x06000A0C RID: 2572 RVA: 0x0000AEB4 File Offset: 0x000090B4
		internal Skeleton(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x0000AEC3 File Offset: 0x000090C3
		public static Skeleton CreateFromModel(string modelName)
		{
			return EngineApplicationInterface.ISkeleton.CreateFromModel(modelName);
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x0000AED0 File Offset: 0x000090D0
		public static Skeleton CreateFromModelWithNullAnimTree(GameEntity entity, string modelName, float boneScale = 1f)
		{
			return EngineApplicationInterface.ISkeleton.CreateFromModelWithNullAnimTree(entity.Pointer, modelName, boneScale);
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000A0F RID: 2575 RVA: 0x0000AEE4 File Offset: 0x000090E4
		public bool IsValid
		{
			get
			{
				return base.Pointer != UIntPtr.Zero;
			}
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x0000AEF6 File Offset: 0x000090F6
		public string GetName()
		{
			return EngineApplicationInterface.ISkeleton.GetName(this);
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x0000AF03 File Offset: 0x00009103
		public string GetBoneName(sbyte boneIndex)
		{
			return EngineApplicationInterface.ISkeleton.GetBoneName(this, boneIndex);
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x0000AF11 File Offset: 0x00009111
		public sbyte GetBoneChildAtIndex(sbyte boneIndex, sbyte childIndex)
		{
			return EngineApplicationInterface.ISkeleton.GetBoneChildAtIndex(this, boneIndex, childIndex);
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x0000AF20 File Offset: 0x00009120
		public sbyte GetBoneChildCount(sbyte boneIndex)
		{
			return EngineApplicationInterface.ISkeleton.GetBoneChildCount(this, boneIndex);
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x0000AF2E File Offset: 0x0000912E
		public sbyte GetParentBoneIndex(sbyte boneIndex)
		{
			return EngineApplicationInterface.ISkeleton.GetParentBoneIndex(this, boneIndex);
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x0000AF3C File Offset: 0x0000913C
		public void AddMeshToBone(UIntPtr mesh, sbyte boneIndex)
		{
			EngineApplicationInterface.ISkeleton.AddMeshToBone(base.Pointer, mesh, boneIndex);
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x0000AF50 File Offset: 0x00009150
		public void Freeze(bool p)
		{
			EngineApplicationInterface.ISkeleton.Freeze(base.Pointer, p);
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x0000AF63 File Offset: 0x00009163
		public bool IsFrozen()
		{
			return EngineApplicationInterface.ISkeleton.IsFrozen(base.Pointer);
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x0000AF75 File Offset: 0x00009175
		public void SetBoneLocalFrame(sbyte boneIndex, MatrixFrame localFrame)
		{
			EngineApplicationInterface.ISkeleton.SetBoneLocalFrame(base.Pointer, boneIndex, ref localFrame);
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x0000AF8A File Offset: 0x0000918A
		public sbyte GetBoneCount()
		{
			return EngineApplicationInterface.ISkeleton.GetBoneCount(base.Pointer);
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x0000AF9C File Offset: 0x0000919C
		public void GetBoneBody(sbyte boneIndex, ref CapsuleData data)
		{
			EngineApplicationInterface.ISkeleton.GetBoneBody(base.Pointer, boneIndex, ref data);
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x0000AFB0 File Offset: 0x000091B0
		public static bool SkeletonModelExist(string skeletonModelName)
		{
			return EngineApplicationInterface.ISkeleton.SkeletonModelExist(skeletonModelName);
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x0000AFBD File Offset: 0x000091BD
		public void ForceUpdateBoneFrames()
		{
			EngineApplicationInterface.ISkeleton.ForceUpdateBoneFrames(base.Pointer);
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x0000AFD0 File Offset: 0x000091D0
		public MatrixFrame GetBoneEntitialFrameWithIndex(sbyte boneIndex)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.ISkeleton.GetBoneEntitialFrameWithIndex(base.Pointer, boneIndex, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x0000AFFC File Offset: 0x000091FC
		public MatrixFrame GetBoneEntitialFrameWithName(string boneName)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.ISkeleton.GetBoneEntitialFrameWithName(base.Pointer, boneName, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x0000B025 File Offset: 0x00009225
		public RagdollState GetCurrentRagdollState()
		{
			return EngineApplicationInterface.ISkeleton.GetCurrentRagdollState(base.Pointer);
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x0000B037 File Offset: 0x00009237
		public void ActivateRagdoll()
		{
			EngineApplicationInterface.ISkeleton.ActivateRagdoll(base.Pointer);
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x0000B049 File Offset: 0x00009249
		public sbyte GetSkeletonBoneMapping(sbyte boneIndex)
		{
			return EngineApplicationInterface.ISkeleton.GetSkeletonBoneMapping(base.Pointer, boneIndex);
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x0000B05C File Offset: 0x0000925C
		public void AddMesh(Mesh mesh)
		{
			EngineApplicationInterface.ISkeleton.AddMesh(base.Pointer, mesh.Pointer);
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x0000B074 File Offset: 0x00009274
		public void ClearComponents()
		{
			EngineApplicationInterface.ISkeleton.ClearComponents(base.Pointer);
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x0000B086 File Offset: 0x00009286
		public void AddComponent(GameEntityComponent component)
		{
			EngineApplicationInterface.ISkeleton.AddComponent(base.Pointer, component.Pointer);
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x0000B09E File Offset: 0x0000929E
		public bool HasComponent(GameEntityComponent component)
		{
			return EngineApplicationInterface.ISkeleton.HasComponent(base.Pointer, component.Pointer);
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x0000B0B6 File Offset: 0x000092B6
		public void RemoveComponent(GameEntityComponent component)
		{
			EngineApplicationInterface.ISkeleton.RemoveComponent(base.Pointer, component.Pointer);
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x0000B0CE File Offset: 0x000092CE
		public void ClearMeshes(bool clearBoneComponents = true)
		{
			EngineApplicationInterface.ISkeleton.ClearMeshes(base.Pointer, clearBoneComponents);
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x0000B0E1 File Offset: 0x000092E1
		public int GetComponentCount(GameEntity.ComponentType componentType)
		{
			return EngineApplicationInterface.ISkeleton.GetComponentCount(base.Pointer, componentType);
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x0000B0F4 File Offset: 0x000092F4
		public void UpdateEntitialFramesFromLocalFrames()
		{
			EngineApplicationInterface.ISkeleton.UpdateEntitialFramesFromLocalFrames(base.Pointer);
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x0000B106 File Offset: 0x00009306
		public void ResetFrames()
		{
			EngineApplicationInterface.ISkeleton.ResetFrames(base.Pointer);
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x0000B118 File Offset: 0x00009318
		public GameEntityComponent GetComponentAtIndex(GameEntity.ComponentType componentType, int index)
		{
			return EngineApplicationInterface.ISkeleton.GetComponentAtIndex(base.Pointer, componentType, index);
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x0000B12C File Offset: 0x0000932C
		public void SetUsePreciseBoundingVolume(bool value)
		{
			EngineApplicationInterface.ISkeleton.SetUsePreciseBoundingVolume(base.Pointer, value);
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x0000B140 File Offset: 0x00009340
		public MatrixFrame GetBoneEntitialRestFrame(sbyte boneIndex, bool useBoneMapping)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.ISkeleton.GetBoneEntitialRestFrame(base.Pointer, boneIndex, useBoneMapping, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x0000B16C File Offset: 0x0000936C
		public MatrixFrame GetBoneLocalRestFrame(sbyte boneIndex, bool useBoneMapping = true)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.ISkeleton.GetBoneLocalRestFrame(base.Pointer, boneIndex, useBoneMapping, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x0000B198 File Offset: 0x00009398
		public MatrixFrame GetBoneEntitialRestFrame(sbyte boneIndex)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.ISkeleton.GetBoneEntitialRestFrame(base.Pointer, boneIndex, true, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x0000B1C4 File Offset: 0x000093C4
		public MatrixFrame GetBoneEntitialFrameAtChannel(int channelNo, sbyte boneIndex)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.ISkeleton.GetBoneEntitialFrameAtChannel(base.Pointer, channelNo, boneIndex, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x0000B1F0 File Offset: 0x000093F0
		public MatrixFrame GetBoneEntitialFrame(sbyte boneIndex)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			EngineApplicationInterface.ISkeleton.GetBoneEntitialFrame(base.Pointer, boneIndex, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x0000B219 File Offset: 0x00009419
		public int GetBoneComponentCount(sbyte boneIndex)
		{
			return EngineApplicationInterface.ISkeleton.GetBoneComponentCount(base.Pointer, boneIndex);
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x0000B22C File Offset: 0x0000942C
		public GameEntityComponent GetBoneComponentAtIndex(sbyte boneIndex, int componentIndex)
		{
			return EngineApplicationInterface.ISkeleton.GetBoneComponentAtIndex(base.Pointer, boneIndex, componentIndex);
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x0000B240 File Offset: 0x00009440
		public bool HasBoneComponent(sbyte boneIndex, GameEntityComponent component)
		{
			return EngineApplicationInterface.ISkeleton.HasBoneComponent(base.Pointer, boneIndex, component);
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x0000B254 File Offset: 0x00009454
		public void AddComponentToBone(sbyte boneIndex, GameEntityComponent component)
		{
			EngineApplicationInterface.ISkeleton.AddComponentToBone(base.Pointer, boneIndex, component);
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x0000B268 File Offset: 0x00009468
		public void RemoveBoneComponent(sbyte boneIndex, GameEntityComponent component)
		{
			EngineApplicationInterface.ISkeleton.RemoveBoneComponent(base.Pointer, boneIndex, component);
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x0000B27C File Offset: 0x0000947C
		public void ClearMeshesAtBone(sbyte boneIndex)
		{
			EngineApplicationInterface.ISkeleton.ClearMeshesAtBone(base.Pointer, boneIndex);
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x0000B28F File Offset: 0x0000948F
		public void TickAnimations(float dt, MatrixFrame globalFrame, bool tickAnimsForChildren)
		{
			EngineApplicationInterface.ISkeleton.TickAnimations(base.Pointer, ref globalFrame, dt, tickAnimsForChildren);
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x0000B2A5 File Offset: 0x000094A5
		public void TickAnimationsAndForceUpdate(float dt, MatrixFrame globalFrame, bool tickAnimsForChildren)
		{
			EngineApplicationInterface.ISkeleton.TickAnimationsAndForceUpdate(base.Pointer, ref globalFrame, dt, tickAnimsForChildren);
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x0000B2BB File Offset: 0x000094BB
		public float GetAnimationParameterAtChannel(int channelNo)
		{
			return EngineApplicationInterface.ISkeleton.GetSkeletonAnimationParameterAtChannel(base.Pointer, channelNo);
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x0000B2CE File Offset: 0x000094CE
		public void SetAnimationParameterAtChannel(int channelNo, float parameter)
		{
			EngineApplicationInterface.ISkeleton.SetSkeletonAnimationParameterAtChannel(base.Pointer, channelNo, parameter);
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x0000B2E2 File Offset: 0x000094E2
		public float GetAnimationSpeedAtChannel(int channelNo)
		{
			return EngineApplicationInterface.ISkeleton.GetSkeletonAnimationSpeedAtChannel(base.Pointer, channelNo);
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x0000B2F5 File Offset: 0x000094F5
		public void SetAnimationSpeedAtChannel(int channelNo, float speed)
		{
			EngineApplicationInterface.ISkeleton.SetSkeletonAnimationSpeedAtChannel(base.Pointer, channelNo, speed);
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x0000B309 File Offset: 0x00009509
		public void SetUptoDate(bool value)
		{
			EngineApplicationInterface.ISkeleton.SetSkeletonUptoDate(base.Pointer, value);
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x0000B31C File Offset: 0x0000951C
		public string GetAnimationAtChannel(int channelNo)
		{
			return EngineApplicationInterface.ISkeleton.GetAnimationAtChannel(base.Pointer, channelNo);
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x0000B32F File Offset: 0x0000952F
		public int GetAnimationIndexAtChannel(int channelNo)
		{
			return EngineApplicationInterface.ISkeleton.GetAnimationIndexAtChannel(base.Pointer, channelNo);
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x0000B342 File Offset: 0x00009542
		public void ResetCloths()
		{
			EngineApplicationInterface.ISkeleton.ResetCloths(base.Pointer);
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x0000B354 File Offset: 0x00009554
		public IEnumerable<Mesh> GetAllMeshes()
		{
			NativeObjectArray nativeObjectArray = NativeObjectArray.Create();
			EngineApplicationInterface.ISkeleton.GetAllMeshes(this, nativeObjectArray);
			foreach (NativeObject nativeObject in ((IEnumerable<NativeObject>)nativeObjectArray))
			{
				Mesh mesh = (Mesh)nativeObject;
				yield return mesh;
			}
			IEnumerator<NativeObject> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x0000B364 File Offset: 0x00009564
		public static sbyte GetBoneIndexFromName(string skeletonModelName, string boneName)
		{
			return EngineApplicationInterface.ISkeleton.GetBoneIndexFromName(skeletonModelName, boneName);
		}

		// Token: 0x040001A3 RID: 419
		public const sbyte MaxBoneCount = 64;
	}
}
