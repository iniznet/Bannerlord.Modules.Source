using System;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000186 RID: 390
	[EngineClass("Agent_visuals")]
	public sealed class MBAgentVisuals : NativeObject
	{
		// Token: 0x06001448 RID: 5192 RVA: 0x0004E937 File Offset: 0x0004CB37
		internal MBAgentVisuals(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x0004E946 File Offset: 0x0004CB46
		private UIntPtr GetPtr()
		{
			return base.Pointer;
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x0004E94E File Offset: 0x0004CB4E
		public static MBAgentVisuals CreateAgentVisuals(Scene scene, string ownerName, Vec3 eyeOffset)
		{
			return MBAPI.IMBAgentVisuals.CreateAgentVisuals(scene.Pointer, ownerName, eyeOffset);
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x0004E962 File Offset: 0x0004CB62
		public void Tick(MBAgentVisuals parentAgentVisuals, float dt, bool entityMoving, float speed)
		{
			MBAPI.IMBAgentVisuals.Tick(this.GetPtr(), (parentAgentVisuals != null) ? parentAgentVisuals.GetPtr() : UIntPtr.Zero, dt, entityMoving, speed);
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x0004E988 File Offset: 0x0004CB88
		public MatrixFrame GetGlobalFrame()
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			MBAPI.IMBAgentVisuals.GetGlobalFrame(this.GetPtr(), ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x0600144D RID: 5197 RVA: 0x0004E9B0 File Offset: 0x0004CBB0
		public MatrixFrame GetFrame()
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			MBAPI.IMBAgentVisuals.GetFrame(this.GetPtr(), ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x0004E9D8 File Offset: 0x0004CBD8
		public GameEntity GetEntity()
		{
			return MBAPI.IMBAgentVisuals.GetEntity(this.GetPtr());
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x0004E9EA File Offset: 0x0004CBEA
		public bool IsValid()
		{
			return MBAPI.IMBAgentVisuals.IsValid(this.GetPtr());
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x0004E9FC File Offset: 0x0004CBFC
		public Vec3 GetGlobalStableEyePoint(bool isHumanoid)
		{
			return MBAPI.IMBAgentVisuals.GetGlobalStableEyePoint(this.GetPtr(), isHumanoid);
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x0004EA0F File Offset: 0x0004CC0F
		public Vec3 GetGlobalStableNeckPoint(bool isHumanoid)
		{
			return MBAPI.IMBAgentVisuals.GetGlobalStableNeckPoint(this.GetPtr(), isHumanoid);
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0004EA24 File Offset: 0x0004CC24
		public MatrixFrame GetBoneEntitialFrame(sbyte bone, bool useBoneMapping)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			MBAPI.IMBAgentVisuals.GetBoneEntitialFrame(base.Pointer, bone, useBoneMapping, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0004EA4E File Offset: 0x0004CC4E
		public RagdollState GetCurrentRagdollState()
		{
			return MBAPI.IMBAgentVisuals.GetCurrentRagdollState(base.Pointer);
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x0004EA60 File Offset: 0x0004CC60
		public sbyte GetRealBoneIndex(HumanBone boneType)
		{
			return MBAPI.IMBAgentVisuals.GetRealBoneIndex(this.GetPtr(), boneType);
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x0004EA73 File Offset: 0x0004CC73
		public CompositeComponent AddPrefabToAgentVisualBoneByBoneType(string prefabName, HumanBone boneType)
		{
			return MBAPI.IMBAgentVisuals.AddPrefabToAgentVisualBoneByBoneType(this.GetPtr(), prefabName, boneType);
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x0004EA87 File Offset: 0x0004CC87
		public CompositeComponent AddPrefabToAgentVisualBoneByRealBoneIndex(string prefabName, sbyte realBoneIndex)
		{
			return MBAPI.IMBAgentVisuals.AddPrefabToAgentVisualBoneByRealBoneIndex(this.GetPtr(), prefabName, realBoneIndex);
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x0004EA9B File Offset: 0x0004CC9B
		public GameEntity GetAttachedWeaponEntity(int attachedWeaponIndex)
		{
			return MBAPI.IMBAgentVisuals.GetAttachedWeaponEntity(this.GetPtr(), attachedWeaponIndex);
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x0004EAAE File Offset: 0x0004CCAE
		public void SetFrame(ref MatrixFrame frame)
		{
			MBAPI.IMBAgentVisuals.SetFrame(this.GetPtr(), ref frame);
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x0004EAC1 File Offset: 0x0004CCC1
		public void SetEntity(GameEntity value)
		{
			MBAPI.IMBAgentVisuals.SetEntity(this.GetPtr(), value.Pointer);
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x0004EAD9 File Offset: 0x0004CCD9
		public static void FillEntityWithBodyMeshesWithoutAgentVisuals(GameEntity entity, SkinGenerationParams skinParams, BodyProperties bodyProperties, MetaMesh glovesMesh)
		{
			MBAPI.IMBAgentVisuals.FillEntityWithBodyMeshesWithoutAgentVisuals(entity.Pointer, ref skinParams, ref bodyProperties, glovesMesh);
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x0004EAF0 File Offset: 0x0004CCF0
		public BoneBodyTypeData GetBoneTypeData(sbyte boneIndex)
		{
			BoneBodyTypeData boneBodyTypeData = default(BoneBodyTypeData);
			MBAPI.IMBAgentVisuals.GetBoneTypeData(base.Pointer, boneIndex, ref boneBodyTypeData);
			return boneBodyTypeData;
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x0004EB19 File Offset: 0x0004CD19
		public Skeleton GetSkeleton()
		{
			return MBAPI.IMBAgentVisuals.GetSkeleton(this.GetPtr());
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x0004EB2B File Offset: 0x0004CD2B
		public void SetSkeleton(Skeleton newSkeleton)
		{
			MBAPI.IMBAgentVisuals.SetSkeleton(this.GetPtr(), newSkeleton.Pointer);
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x0004EB44 File Offset: 0x0004CD44
		public void CreateParticleSystemAttachedToBone(string particleName, sbyte boneIndex, ref MatrixFrame boneLocalParticleFrame)
		{
			int runtimeIdByName = ParticleSystemManager.GetRuntimeIdByName(particleName);
			this.CreateParticleSystemAttachedToBone(runtimeIdByName, boneIndex, ref boneLocalParticleFrame);
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x0004EB61 File Offset: 0x0004CD61
		public void CreateParticleSystemAttachedToBone(int runtimeParticleindex, sbyte boneIndex, ref MatrixFrame boneLocalParticleFrame)
		{
			MBAPI.IMBAgentVisuals.CreateParticleSystemAttachedToBone(this.GetPtr(), runtimeParticleindex, boneIndex, ref boneLocalParticleFrame);
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x0004EB76 File Offset: 0x0004CD76
		public void SetVisible(bool value)
		{
			MBAPI.IMBAgentVisuals.SetVisible(this.GetPtr(), value);
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x0004EB89 File Offset: 0x0004CD89
		public bool GetVisible()
		{
			return MBAPI.IMBAgentVisuals.GetVisible(this.GetPtr());
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x0004EB9B File Offset: 0x0004CD9B
		public void AddChildEntity(GameEntity entity)
		{
			MBAPI.IMBAgentVisuals.AddChildEntity(this.GetPtr(), entity.Pointer);
		}

		// Token: 0x06001463 RID: 5219 RVA: 0x0004EBB4 File Offset: 0x0004CDB4
		public void SetClothWindToWeaponAtIndex(Vec3 windDirection, bool isLocal, EquipmentIndex weaponIndex)
		{
			MBAPI.IMBAgentVisuals.SetClothWindToWeaponAtIndex(this.GetPtr(), windDirection, isLocal, (int)weaponIndex);
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x0004EBC9 File Offset: 0x0004CDC9
		public void RemoveChildEntity(GameEntity entity, int removeReason)
		{
			MBAPI.IMBAgentVisuals.RemoveChildEntity(this.GetPtr(), entity.Pointer, removeReason);
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x0004EBE2 File Offset: 0x0004CDE2
		public bool CheckResources(bool addToQueue)
		{
			return MBAPI.IMBAgentVisuals.CheckResources(this.GetPtr(), addToQueue);
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x0004EBF5 File Offset: 0x0004CDF5
		public void AddSkinMeshes(SkinGenerationParams skinParams, BodyProperties bodyProperties, bool useGPUMorph, bool useFaceCache)
		{
			MBAPI.IMBAgentVisuals.AddSkinMeshesToAgentEntity(this.GetPtr(), ref skinParams, ref bodyProperties, useGPUMorph, useFaceCache);
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x0004EC0E File Offset: 0x0004CE0E
		public void SetFaceGenerationParams(FaceGenerationParams faceGenerationParams)
		{
			MBAPI.IMBAgentVisuals.SetFaceGenerationParams(this.GetPtr(), faceGenerationParams);
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x0004EC21 File Offset: 0x0004CE21
		public void SetLodAtlasShadingIndex(int index, bool useTeamColor, uint teamColor1, uint teamColor2)
		{
			MBAPI.IMBAgentVisuals.SetLodAtlasShadingIndex(this.GetPtr(), index, useTeamColor, teamColor1, teamColor2);
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x0004EC38 File Offset: 0x0004CE38
		public void ClearVisualComponents(bool removeSkeleton)
		{
			MBAPI.IMBAgentVisuals.ClearVisualComponents(this.GetPtr(), removeSkeleton);
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x0004EC4B File Offset: 0x0004CE4B
		public void LazyUpdateAgentRendererData()
		{
			MBAPI.IMBAgentVisuals.LazyUpdateAgentRendererData(this.GetPtr());
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x0004EC5D File Offset: 0x0004CE5D
		public void AddMultiMesh(MetaMesh metaMesh, BodyMeshTypes bodyMeshIndex)
		{
			MBAPI.IMBAgentVisuals.AddMultiMesh(this.GetPtr(), metaMesh.Pointer, (int)bodyMeshIndex);
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x0004EC76 File Offset: 0x0004CE76
		public void ApplySkeletonScale(Vec3 mountSitBoneScale, float mountRadiusAdder, sbyte[] boneIndices, Vec3[] boneScales)
		{
			MBAPI.IMBAgentVisuals.ApplySkeletonScale(base.Pointer, mountSitBoneScale, mountRadiusAdder, (byte)boneIndices.Length, boneIndices, boneScales);
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x0004EC91 File Offset: 0x0004CE91
		public void UpdateSkeletonScale(int bodyDeformType)
		{
			MBAPI.IMBAgentVisuals.UpdateSkeletonScale(base.Pointer, bodyDeformType);
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x0004ECA4 File Offset: 0x0004CEA4
		public void AddHorseReinsClothMesh(MetaMesh reinMesh, MetaMesh ropeMesh)
		{
			MBAPI.IMBAgentVisuals.AddHorseReinsClothMesh(base.Pointer, reinMesh.Pointer, ropeMesh.Pointer);
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x0004ECC2 File Offset: 0x0004CEC2
		public void BatchLastLodMeshes()
		{
			MBAPI.IMBAgentVisuals.BatchLastLodMeshes(this.GetPtr());
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x0004ECD4 File Offset: 0x0004CED4
		public void AddWeaponToAgentEntity(int slotIndex, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, in WeaponData ammoWeaponData, WeaponStatsData[] ammoWeaponStatsData, GameEntity cachedEntity)
		{
			MBAPI.IMBAgentVisuals.AddWeaponToAgentEntity(this.GetPtr(), slotIndex, weaponData, weaponStatsData, weaponStatsData.Length, ammoWeaponData, ammoWeaponStatsData, ammoWeaponStatsData.Length, cachedEntity);
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x0004ED01 File Offset: 0x0004CF01
		public void UpdateQuiverMeshesWithoutAgent(int weaponIndex, int ammoCount)
		{
			MBAPI.IMBAgentVisuals.UpdateQuiverMeshesWithoutAgent(this.GetPtr(), weaponIndex, ammoCount);
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x0004ED15 File Offset: 0x0004CF15
		public void SetWieldedWeaponIndices(int slotIndexRightHand, int slotIndexLeftHand)
		{
			MBAPI.IMBAgentVisuals.SetWieldedWeaponIndices(this.GetPtr(), slotIndexRightHand, slotIndexLeftHand);
		}

		// Token: 0x06001473 RID: 5235 RVA: 0x0004ED29 File Offset: 0x0004CF29
		public void ClearAllWeaponMeshes()
		{
			MBAPI.IMBAgentVisuals.ClearAllWeaponMeshes(this.GetPtr());
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x0004ED3B File Offset: 0x0004CF3B
		public void ClearWeaponMeshes(EquipmentIndex index)
		{
			MBAPI.IMBAgentVisuals.ClearWeaponMeshes(this.GetPtr(), (int)index);
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x0004ED4E File Offset: 0x0004CF4E
		public void MakeVoice(int voiceId, Vec3 position)
		{
			MBAPI.IMBAgentVisuals.MakeVoice(this.GetPtr(), voiceId, ref position);
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x0004ED63 File Offset: 0x0004CF63
		public void SetSetupMorphNode(bool value)
		{
			MBAPI.IMBAgentVisuals.SetSetupMorphNode(this.GetPtr(), value);
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x0004ED76 File Offset: 0x0004CF76
		public void UseScaledWeapons(bool value)
		{
			MBAPI.IMBAgentVisuals.UseScaledWeapons(this.GetPtr(), value);
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x0004ED89 File Offset: 0x0004CF89
		public void SetClothComponentKeepStateOfAllMeshes(bool keepState)
		{
			MBAPI.IMBAgentVisuals.SetClothComponentKeepStateOfAllMeshes(this.GetPtr(), keepState);
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x0004ED9C File Offset: 0x0004CF9C
		public MatrixFrame GetFacegenScalingMatrix()
		{
			MatrixFrame identity = MatrixFrame.Identity;
			Vec3 currentHelmetScalingFactor = MBAPI.IMBAgentVisuals.GetCurrentHelmetScalingFactor(this.GetPtr());
			identity.rotation.ApplyScaleLocal(currentHelmetScalingFactor);
			return identity;
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x0004EDD0 File Offset: 0x0004CFD0
		public void ReplaceMeshWithMesh(MetaMesh oldMetaMesh, MetaMesh newMetaMesh, BodyMeshTypes bodyMeshIndex)
		{
			if (oldMetaMesh != null)
			{
				MBAPI.IMBAgentVisuals.RemoveMultiMesh(this.GetPtr(), oldMetaMesh.Pointer, (int)bodyMeshIndex);
			}
			if (newMetaMesh != null)
			{
				MBAPI.IMBAgentVisuals.AddMultiMesh(this.GetPtr(), newMetaMesh.Pointer, (int)bodyMeshIndex);
			}
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x0004EE1D File Offset: 0x0004D01D
		public void SetAgentActionChannel(int actionChannelNo, int actionIndex, float channelParameter = 0f, float blendPeriodOverride = -0.2f, bool forceFaceMorphRestart = true)
		{
			MBAPI.IMBSkeletonExtensions.SetAgentActionChannel(this.GetSkeleton().Pointer, actionChannelNo, actionIndex, channelParameter, blendPeriodOverride, forceFaceMorphRestart);
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x0004EE3B File Offset: 0x0004D03B
		public void SetVoiceDefinitionIndex(int voiceDefinitionIndex, float voicePitch)
		{
			MBAPI.IMBAgentVisuals.SetVoiceDefinitionIndex(this.GetPtr(), voiceDefinitionIndex, voicePitch);
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x0004EE4F File Offset: 0x0004D04F
		public void StartRhubarbRecord(string path, int soundId)
		{
			MBAPI.IMBAgentVisuals.StartRhubarbRecord(this.GetPtr(), path, soundId);
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x0004EE64 File Offset: 0x0004D064
		public void SetContourColor(uint? color, bool alwaysVisible = true)
		{
			if (color != null)
			{
				MBAPI.IMBAgentVisuals.SetAsContourEntity(this.GetPtr(), color.Value);
				MBAPI.IMBAgentVisuals.SetContourState(this.GetPtr(), alwaysVisible);
				return;
			}
			MBAPI.IMBAgentVisuals.DisableContour(this.GetPtr());
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x0004EEB3 File Offset: 0x0004D0B3
		public void SetEnableOcclusionCulling(bool enable)
		{
			MBAPI.IMBAgentVisuals.SetEnableOcclusionCulling(this.GetPtr(), enable);
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x0004EEC6 File Offset: 0x0004D0C6
		public void SetAgentLodZeroOrMax(bool makeZero)
		{
			MBAPI.IMBAgentVisuals.SetAgentLodMakeZeroOrMax(this.GetPtr(), makeZero);
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x0004EED9 File Offset: 0x0004D0D9
		public void SetAgentLocalSpeed(Vec2 speed)
		{
			MBAPI.IMBAgentVisuals.SetAgentLocalSpeed(this.GetPtr(), speed);
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x0004EEEC File Offset: 0x0004D0EC
		public void SetLookDirection(Vec3 direction)
		{
			MBAPI.IMBAgentVisuals.SetLookDirection(this.GetPtr(), direction);
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x0004EF00 File Offset: 0x0004D100
		public static BodyMeshTypes GetBodyMeshIndex(EquipmentIndex equipmentIndex)
		{
			switch (equipmentIndex)
			{
			case EquipmentIndex.NumAllWeaponSlots:
				return BodyMeshTypes.Cap;
			case EquipmentIndex.Body:
				return BodyMeshTypes.Chestpiece;
			case EquipmentIndex.Leg:
				return BodyMeshTypes.Footwear;
			case EquipmentIndex.Gloves:
				return BodyMeshTypes.Gloves;
			case EquipmentIndex.Cape:
				return BodyMeshTypes.Shoulderpiece;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Base\\MBAgentVisuals.cs", "GetBodyMeshIndex", 393);
				return BodyMeshTypes.Invalid;
			}
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x0004EF52 File Offset: 0x0004D152
		public void Reset()
		{
			MBAPI.IMBAgentVisuals.Reset(this.GetPtr());
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x0004EF64 File Offset: 0x0004D164
		public void ResetNextFrame()
		{
			MBAPI.IMBAgentVisuals.ResetNextFrame(this.GetPtr());
		}
	}
}
