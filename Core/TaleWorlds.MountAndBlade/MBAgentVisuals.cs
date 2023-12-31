﻿using System;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineClass("Agent_visuals")]
	public sealed class MBAgentVisuals : NativeObject
	{
		internal MBAgentVisuals(UIntPtr pointer)
		{
			base.Construct(pointer);
		}

		private UIntPtr GetPtr()
		{
			return base.Pointer;
		}

		public static MBAgentVisuals CreateAgentVisuals(Scene scene, string ownerName, Vec3 eyeOffset)
		{
			return MBAPI.IMBAgentVisuals.CreateAgentVisuals(scene.Pointer, ownerName, eyeOffset);
		}

		public void Tick(MBAgentVisuals parentAgentVisuals, float dt, bool entityMoving, float speed)
		{
			MBAPI.IMBAgentVisuals.Tick(this.GetPtr(), (parentAgentVisuals != null) ? parentAgentVisuals.GetPtr() : UIntPtr.Zero, dt, entityMoving, speed);
		}

		public MatrixFrame GetGlobalFrame()
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			MBAPI.IMBAgentVisuals.GetGlobalFrame(this.GetPtr(), ref matrixFrame);
			return matrixFrame;
		}

		public MatrixFrame GetFrame()
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			MBAPI.IMBAgentVisuals.GetFrame(this.GetPtr(), ref matrixFrame);
			return matrixFrame;
		}

		public GameEntity GetEntity()
		{
			return MBAPI.IMBAgentVisuals.GetEntity(this.GetPtr());
		}

		public bool IsValid()
		{
			return MBAPI.IMBAgentVisuals.IsValid(this.GetPtr());
		}

		public Vec3 GetGlobalStableEyePoint(bool isHumanoid)
		{
			return MBAPI.IMBAgentVisuals.GetGlobalStableEyePoint(this.GetPtr(), isHumanoid);
		}

		public Vec3 GetGlobalStableNeckPoint(bool isHumanoid)
		{
			return MBAPI.IMBAgentVisuals.GetGlobalStableNeckPoint(this.GetPtr(), isHumanoid);
		}

		public MatrixFrame GetBoneEntitialFrame(sbyte bone, bool useBoneMapping)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			MBAPI.IMBAgentVisuals.GetBoneEntitialFrame(base.Pointer, bone, useBoneMapping, ref matrixFrame);
			return matrixFrame;
		}

		public RagdollState GetCurrentRagdollState()
		{
			return MBAPI.IMBAgentVisuals.GetCurrentRagdollState(base.Pointer);
		}

		public sbyte GetRealBoneIndex(HumanBone boneType)
		{
			return MBAPI.IMBAgentVisuals.GetRealBoneIndex(this.GetPtr(), boneType);
		}

		public CompositeComponent AddPrefabToAgentVisualBoneByBoneType(string prefabName, HumanBone boneType)
		{
			return MBAPI.IMBAgentVisuals.AddPrefabToAgentVisualBoneByBoneType(this.GetPtr(), prefabName, boneType);
		}

		public CompositeComponent AddPrefabToAgentVisualBoneByRealBoneIndex(string prefabName, sbyte realBoneIndex)
		{
			return MBAPI.IMBAgentVisuals.AddPrefabToAgentVisualBoneByRealBoneIndex(this.GetPtr(), prefabName, realBoneIndex);
		}

		public GameEntity GetAttachedWeaponEntity(int attachedWeaponIndex)
		{
			return MBAPI.IMBAgentVisuals.GetAttachedWeaponEntity(this.GetPtr(), attachedWeaponIndex);
		}

		public void SetFrame(ref MatrixFrame frame)
		{
			MBAPI.IMBAgentVisuals.SetFrame(this.GetPtr(), ref frame);
		}

		public void SetEntity(GameEntity value)
		{
			MBAPI.IMBAgentVisuals.SetEntity(this.GetPtr(), value.Pointer);
		}

		public static void FillEntityWithBodyMeshesWithoutAgentVisuals(GameEntity entity, SkinGenerationParams skinParams, BodyProperties bodyProperties, MetaMesh glovesMesh)
		{
			MBAPI.IMBAgentVisuals.FillEntityWithBodyMeshesWithoutAgentVisuals(entity.Pointer, ref skinParams, ref bodyProperties, glovesMesh);
		}

		public BoneBodyTypeData GetBoneTypeData(sbyte boneIndex)
		{
			BoneBodyTypeData boneBodyTypeData = default(BoneBodyTypeData);
			MBAPI.IMBAgentVisuals.GetBoneTypeData(base.Pointer, boneIndex, ref boneBodyTypeData);
			return boneBodyTypeData;
		}

		public Skeleton GetSkeleton()
		{
			return MBAPI.IMBAgentVisuals.GetSkeleton(this.GetPtr());
		}

		public void SetSkeleton(Skeleton newSkeleton)
		{
			MBAPI.IMBAgentVisuals.SetSkeleton(this.GetPtr(), newSkeleton.Pointer);
		}

		public void CreateParticleSystemAttachedToBone(string particleName, sbyte boneIndex, ref MatrixFrame boneLocalParticleFrame)
		{
			int runtimeIdByName = ParticleSystemManager.GetRuntimeIdByName(particleName);
			this.CreateParticleSystemAttachedToBone(runtimeIdByName, boneIndex, ref boneLocalParticleFrame);
		}

		public void CreateParticleSystemAttachedToBone(int runtimeParticleindex, sbyte boneIndex, ref MatrixFrame boneLocalParticleFrame)
		{
			MBAPI.IMBAgentVisuals.CreateParticleSystemAttachedToBone(this.GetPtr(), runtimeParticleindex, boneIndex, ref boneLocalParticleFrame);
		}

		public void SetVisible(bool value)
		{
			MBAPI.IMBAgentVisuals.SetVisible(this.GetPtr(), value);
		}

		public bool GetVisible()
		{
			return MBAPI.IMBAgentVisuals.GetVisible(this.GetPtr());
		}

		public void AddChildEntity(GameEntity entity)
		{
			MBAPI.IMBAgentVisuals.AddChildEntity(this.GetPtr(), entity.Pointer);
		}

		public void SetClothWindToWeaponAtIndex(Vec3 windDirection, bool isLocal, EquipmentIndex weaponIndex)
		{
			MBAPI.IMBAgentVisuals.SetClothWindToWeaponAtIndex(this.GetPtr(), windDirection, isLocal, (int)weaponIndex);
		}

		public void RemoveChildEntity(GameEntity entity, int removeReason)
		{
			MBAPI.IMBAgentVisuals.RemoveChildEntity(this.GetPtr(), entity.Pointer, removeReason);
		}

		public bool CheckResources(bool addToQueue)
		{
			return MBAPI.IMBAgentVisuals.CheckResources(this.GetPtr(), addToQueue);
		}

		public void AddSkinMeshes(SkinGenerationParams skinParams, BodyProperties bodyProperties, bool useGPUMorph, bool useFaceCache)
		{
			MBAPI.IMBAgentVisuals.AddSkinMeshesToAgentEntity(this.GetPtr(), ref skinParams, ref bodyProperties, useGPUMorph, useFaceCache);
		}

		public void SetFaceGenerationParams(FaceGenerationParams faceGenerationParams)
		{
			MBAPI.IMBAgentVisuals.SetFaceGenerationParams(this.GetPtr(), faceGenerationParams);
		}

		public void SetLodAtlasShadingIndex(int index, bool useTeamColor, uint teamColor1, uint teamColor2)
		{
			MBAPI.IMBAgentVisuals.SetLodAtlasShadingIndex(this.GetPtr(), index, useTeamColor, teamColor1, teamColor2);
		}

		public void ClearVisualComponents(bool removeSkeleton)
		{
			MBAPI.IMBAgentVisuals.ClearVisualComponents(this.GetPtr(), removeSkeleton);
		}

		public void LazyUpdateAgentRendererData()
		{
			MBAPI.IMBAgentVisuals.LazyUpdateAgentRendererData(this.GetPtr());
		}

		public void AddMultiMesh(MetaMesh metaMesh, BodyMeshTypes bodyMeshIndex)
		{
			MBAPI.IMBAgentVisuals.AddMultiMesh(this.GetPtr(), metaMesh.Pointer, (int)bodyMeshIndex);
		}

		public void ApplySkeletonScale(Vec3 mountSitBoneScale, float mountRadiusAdder, sbyte[] boneIndices, Vec3[] boneScales)
		{
			MBAPI.IMBAgentVisuals.ApplySkeletonScale(base.Pointer, mountSitBoneScale, mountRadiusAdder, (byte)boneIndices.Length, boneIndices, boneScales);
		}

		public void UpdateSkeletonScale(int bodyDeformType)
		{
			MBAPI.IMBAgentVisuals.UpdateSkeletonScale(base.Pointer, bodyDeformType);
		}

		public void AddHorseReinsClothMesh(MetaMesh reinMesh, MetaMesh ropeMesh)
		{
			MBAPI.IMBAgentVisuals.AddHorseReinsClothMesh(base.Pointer, reinMesh.Pointer, ropeMesh.Pointer);
		}

		public void BatchLastLodMeshes()
		{
			MBAPI.IMBAgentVisuals.BatchLastLodMeshes(this.GetPtr());
		}

		public void AddWeaponToAgentEntity(int slotIndex, in WeaponData weaponData, WeaponStatsData[] weaponStatsData, in WeaponData ammoWeaponData, WeaponStatsData[] ammoWeaponStatsData, GameEntity cachedEntity)
		{
			MBAPI.IMBAgentVisuals.AddWeaponToAgentEntity(this.GetPtr(), slotIndex, weaponData, weaponStatsData, weaponStatsData.Length, ammoWeaponData, ammoWeaponStatsData, ammoWeaponStatsData.Length, cachedEntity);
		}

		public void UpdateQuiverMeshesWithoutAgent(int weaponIndex, int ammoCount)
		{
			MBAPI.IMBAgentVisuals.UpdateQuiverMeshesWithoutAgent(this.GetPtr(), weaponIndex, ammoCount);
		}

		public void SetWieldedWeaponIndices(int slotIndexRightHand, int slotIndexLeftHand)
		{
			MBAPI.IMBAgentVisuals.SetWieldedWeaponIndices(this.GetPtr(), slotIndexRightHand, slotIndexLeftHand);
		}

		public void ClearAllWeaponMeshes()
		{
			MBAPI.IMBAgentVisuals.ClearAllWeaponMeshes(this.GetPtr());
		}

		public void ClearWeaponMeshes(EquipmentIndex index)
		{
			MBAPI.IMBAgentVisuals.ClearWeaponMeshes(this.GetPtr(), (int)index);
		}

		public void MakeVoice(int voiceId, Vec3 position)
		{
			MBAPI.IMBAgentVisuals.MakeVoice(this.GetPtr(), voiceId, ref position);
		}

		public void SetSetupMorphNode(bool value)
		{
			MBAPI.IMBAgentVisuals.SetSetupMorphNode(this.GetPtr(), value);
		}

		public void UseScaledWeapons(bool value)
		{
			MBAPI.IMBAgentVisuals.UseScaledWeapons(this.GetPtr(), value);
		}

		public void SetClothComponentKeepStateOfAllMeshes(bool keepState)
		{
			MBAPI.IMBAgentVisuals.SetClothComponentKeepStateOfAllMeshes(this.GetPtr(), keepState);
		}

		public MatrixFrame GetFacegenScalingMatrix()
		{
			MatrixFrame identity = MatrixFrame.Identity;
			Vec3 currentHelmetScalingFactor = MBAPI.IMBAgentVisuals.GetCurrentHelmetScalingFactor(this.GetPtr());
			identity.rotation.ApplyScaleLocal(currentHelmetScalingFactor);
			return identity;
		}

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

		public void SetAgentActionChannel(int actionChannelNo, int actionIndex, float channelParameter = 0f, float blendPeriodOverride = -0.2f, bool forceFaceMorphRestart = true)
		{
			MBAPI.IMBSkeletonExtensions.SetAgentActionChannel(this.GetSkeleton().Pointer, actionChannelNo, actionIndex, channelParameter, blendPeriodOverride, forceFaceMorphRestart);
		}

		public void SetVoiceDefinitionIndex(int voiceDefinitionIndex, float voicePitch)
		{
			MBAPI.IMBAgentVisuals.SetVoiceDefinitionIndex(this.GetPtr(), voiceDefinitionIndex, voicePitch);
		}

		public void StartRhubarbRecord(string path, int soundId)
		{
			MBAPI.IMBAgentVisuals.StartRhubarbRecord(this.GetPtr(), path, soundId);
		}

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

		public void SetEnableOcclusionCulling(bool enable)
		{
			MBAPI.IMBAgentVisuals.SetEnableOcclusionCulling(this.GetPtr(), enable);
		}

		public void SetAgentLodZeroOrMax(bool makeZero)
		{
			MBAPI.IMBAgentVisuals.SetAgentLodMakeZeroOrMax(this.GetPtr(), makeZero);
		}

		public void SetAgentLocalSpeed(Vec2 speed)
		{
			MBAPI.IMBAgentVisuals.SetAgentLocalSpeed(this.GetPtr(), speed);
		}

		public void SetLookDirection(Vec3 direction)
		{
			MBAPI.IMBAgentVisuals.SetLookDirection(this.GetPtr(), direction);
		}

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

		public void Reset()
		{
			MBAPI.IMBAgentVisuals.Reset(this.GetPtr());
		}

		public void ResetNextFrame()
		{
			MBAPI.IMBAgentVisuals.ResetNextFrame(this.GetPtr());
		}
	}
}
