using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public sealed class Monster : MBObjectBase
	{
		public string BaseMonster { get; private set; }

		public float BodyCapsuleRadius { get; private set; }

		public Vec3 BodyCapsulePoint1 { get; private set; }

		public Vec3 BodyCapsulePoint2 { get; private set; }

		public float CrouchedBodyCapsuleRadius { get; private set; }

		public Vec3 CrouchedBodyCapsulePoint1 { get; private set; }

		public Vec3 CrouchedBodyCapsulePoint2 { get; private set; }

		public AgentFlag Flags { get; private set; }

		public int Weight { get; private set; }

		public int HitPoints { get; private set; }

		public string ActionSetCode { get; private set; }

		public string FemaleActionSetCode { get; private set; }

		public int NumPaces { get; private set; }

		public string MonsterUsage { get; private set; }

		public float WalkingSpeedLimit { get; private set; }

		public float CrouchWalkingSpeedLimit { get; private set; }

		public float JumpAcceleration { get; private set; }

		public float AbsorbedDamageRatio { get; private set; }

		public string SoundAndCollisionInfoClassName { get; private set; }

		public float RiderCameraHeightAdder { get; private set; }

		public float RiderBodyCapsuleHeightAdder { get; private set; }

		public float RiderBodyCapsuleForwardAdder { get; private set; }

		public float StandingEyeHeight { get; private set; }

		public float CrouchEyeHeight { get; private set; }

		public float MountedEyeHeight { get; private set; }

		public float RiderEyeHeightAdder { get; private set; }

		public Vec3 EyeOffsetWrtHead { get; private set; }

		public Vec3 FirstPersonCameraOffsetWrtHead { get; private set; }

		public float ArmLength { get; private set; }

		public float ArmWeight { get; private set; }

		public float JumpSpeedLimit { get; private set; }

		public float RelativeSpeedLimitForCharge { get; private set; }

		public int FamilyType { get; private set; }

		public sbyte[] IndicesOfRagdollBonesToCheckForCorpses { get; private set; }

		public sbyte[] RagdollFallSoundBoneIndices { get; private set; }

		public sbyte HeadLookDirectionBoneIndex { get; private set; }

		public sbyte SpineLowerBoneIndex { get; private set; }

		public sbyte SpineUpperBoneIndex { get; private set; }

		public sbyte ThoraxLookDirectionBoneIndex { get; private set; }

		public sbyte NeckRootBoneIndex { get; private set; }

		public sbyte PelvisBoneIndex { get; private set; }

		public sbyte RightUpperArmBoneIndex { get; private set; }

		public sbyte LeftUpperArmBoneIndex { get; private set; }

		public sbyte FallBlowDamageBoneIndex { get; private set; }

		public sbyte TerrainDecalBone0Index { get; private set; }

		public sbyte TerrainDecalBone1Index { get; private set; }

		public sbyte[] RagdollStationaryCheckBoneIndices { get; private set; }

		public sbyte[] MoveAdderBoneIndices { get; private set; }

		public sbyte[] SplashDecalBoneIndices { get; private set; }

		public sbyte[] BloodBurstBoneIndices { get; private set; }

		public sbyte MainHandBoneIndex { get; private set; }

		public sbyte OffHandBoneIndex { get; private set; }

		public sbyte MainHandItemBoneIndex { get; private set; }

		public sbyte OffHandItemBoneIndex { get; private set; }

		public sbyte MainHandItemSecondaryBoneIndex { get; private set; }

		public sbyte OffHandItemSecondaryBoneIndex { get; private set; }

		public sbyte OffHandShoulderBoneIndex { get; private set; }

		public sbyte HandNumBonesForIk { get; private set; }

		public sbyte PrimaryFootBoneIndex { get; private set; }

		public sbyte SecondaryFootBoneIndex { get; private set; }

		public sbyte RightFootIkEndEffectorBoneIndex { get; private set; }

		public sbyte LeftFootIkEndEffectorBoneIndex { get; private set; }

		public sbyte RightFootIkTipBoneIndex { get; private set; }

		public sbyte LeftFootIkTipBoneIndex { get; private set; }

		public sbyte FootNumBonesForIk { get; private set; }

		public Vec3 ReinHandleLeftLocalPosition { get; private set; }

		public Vec3 ReinHandleRightLocalPosition { get; private set; }

		public string ReinSkeleton { get; private set; }

		public string ReinCollisionBody { get; private set; }

		public sbyte FrontBoneToDetectGroundSlopeIndex { get; private set; }

		public sbyte BackBoneToDetectGroundSlopeIndex { get; private set; }

		public sbyte[] BoneIndicesToModifyOnSlopingGround { get; private set; }

		public sbyte BodyRotationReferenceBoneIndex { get; private set; }

		public sbyte RiderSitBoneIndex { get; private set; }

		public sbyte ReinHandleBoneIndex { get; private set; }

		public sbyte ReinCollision1BoneIndex { get; private set; }

		public sbyte ReinCollision2BoneIndex { get; private set; }

		public sbyte ReinHeadBoneIndex { get; private set; }

		public sbyte ReinHeadRightAttachmentBoneIndex { get; private set; }

		public sbyte ReinHeadLeftAttachmentBoneIndex { get; private set; }

		public sbyte ReinRightHandBoneIndex { get; private set; }

		public sbyte ReinLeftHandBoneIndex { get; private set; }

		[CachedData]
		public IMonsterMissionData MonsterMissionData
		{
			get
			{
				IMonsterMissionData monsterMissionData;
				if ((monsterMissionData = this._monsterMissionData) == null)
				{
					monsterMissionData = (this._monsterMissionData = Game.Current.MonsterMissionDataCreator.CreateMonsterMissionData(this));
				}
				return monsterMissionData;
			}
		}

		public override void Deserialize(MBObjectManager objectManager, XmlNode node)
		{
			base.Deserialize(objectManager, node);
			bool flag = false;
			XmlAttribute xmlAttribute = node.Attributes["base_monster"];
			List<sbyte> list;
			List<sbyte> list2;
			List<sbyte> list3;
			List<sbyte> list4;
			List<sbyte> list5;
			List<sbyte> list6;
			List<sbyte> list7;
			if (xmlAttribute != null && !string.IsNullOrEmpty(xmlAttribute.Value))
			{
				flag = true;
				this.BaseMonster = xmlAttribute.Value;
				Monster @object = objectManager.GetObject<Monster>(this.BaseMonster);
				if (!string.IsNullOrEmpty(@object.BaseMonster))
				{
					this.BaseMonster = @object.BaseMonster;
				}
				this.BodyCapsuleRadius = @object.BodyCapsuleRadius;
				this.BodyCapsulePoint1 = @object.BodyCapsulePoint1;
				this.BodyCapsulePoint2 = @object.BodyCapsulePoint2;
				this.CrouchedBodyCapsuleRadius = @object.CrouchedBodyCapsuleRadius;
				this.CrouchedBodyCapsulePoint1 = @object.CrouchedBodyCapsulePoint1;
				this.CrouchedBodyCapsulePoint2 = @object.CrouchedBodyCapsulePoint2;
				this.Flags = @object.Flags;
				this.Weight = @object.Weight;
				this.HitPoints = @object.HitPoints;
				this.ActionSetCode = @object.ActionSetCode;
				this.FemaleActionSetCode = @object.FemaleActionSetCode;
				this.MonsterUsage = @object.MonsterUsage;
				this.NumPaces = @object.NumPaces;
				this.WalkingSpeedLimit = @object.WalkingSpeedLimit;
				this.CrouchWalkingSpeedLimit = @object.CrouchWalkingSpeedLimit;
				this.JumpAcceleration = @object.JumpAcceleration;
				this.AbsorbedDamageRatio = @object.AbsorbedDamageRatio;
				this.SoundAndCollisionInfoClassName = @object.SoundAndCollisionInfoClassName;
				this.RiderCameraHeightAdder = @object.RiderCameraHeightAdder;
				this.RiderBodyCapsuleHeightAdder = @object.RiderBodyCapsuleHeightAdder;
				this.RiderBodyCapsuleForwardAdder = @object.RiderBodyCapsuleForwardAdder;
				this.StandingEyeHeight = @object.StandingEyeHeight;
				this.CrouchEyeHeight = @object.CrouchEyeHeight;
				this.MountedEyeHeight = @object.MountedEyeHeight;
				this.RiderEyeHeightAdder = @object.RiderEyeHeightAdder;
				this.EyeOffsetWrtHead = @object.EyeOffsetWrtHead;
				this.FirstPersonCameraOffsetWrtHead = @object.FirstPersonCameraOffsetWrtHead;
				this.ArmLength = @object.ArmLength;
				this.ArmWeight = @object.ArmWeight;
				this.JumpSpeedLimit = @object.JumpSpeedLimit;
				this.RelativeSpeedLimitForCharge = @object.RelativeSpeedLimitForCharge;
				this.FamilyType = @object.FamilyType;
				list = new List<sbyte>(@object.IndicesOfRagdollBonesToCheckForCorpses);
				list2 = new List<sbyte>(@object.RagdollFallSoundBoneIndices);
				this.HeadLookDirectionBoneIndex = @object.HeadLookDirectionBoneIndex;
				this.SpineLowerBoneIndex = @object.SpineLowerBoneIndex;
				this.SpineUpperBoneIndex = @object.SpineUpperBoneIndex;
				this.ThoraxLookDirectionBoneIndex = @object.ThoraxLookDirectionBoneIndex;
				this.NeckRootBoneIndex = @object.NeckRootBoneIndex;
				this.PelvisBoneIndex = @object.PelvisBoneIndex;
				this.RightUpperArmBoneIndex = @object.RightUpperArmBoneIndex;
				this.LeftUpperArmBoneIndex = @object.LeftUpperArmBoneIndex;
				this.FallBlowDamageBoneIndex = @object.FallBlowDamageBoneIndex;
				this.TerrainDecalBone0Index = @object.TerrainDecalBone0Index;
				this.TerrainDecalBone1Index = @object.TerrainDecalBone1Index;
				list3 = new List<sbyte>(@object.RagdollStationaryCheckBoneIndices);
				list4 = new List<sbyte>(@object.MoveAdderBoneIndices);
				list5 = new List<sbyte>(@object.SplashDecalBoneIndices);
				list6 = new List<sbyte>(@object.BloodBurstBoneIndices);
				this.MainHandBoneIndex = @object.MainHandBoneIndex;
				this.OffHandBoneIndex = @object.OffHandBoneIndex;
				this.MainHandItemBoneIndex = @object.MainHandItemBoneIndex;
				this.OffHandItemBoneIndex = @object.OffHandItemBoneIndex;
				this.MainHandItemSecondaryBoneIndex = @object.MainHandItemSecondaryBoneIndex;
				this.OffHandItemSecondaryBoneIndex = @object.OffHandItemSecondaryBoneIndex;
				this.OffHandShoulderBoneIndex = @object.OffHandShoulderBoneIndex;
				this.HandNumBonesForIk = @object.HandNumBonesForIk;
				this.PrimaryFootBoneIndex = @object.PrimaryFootBoneIndex;
				this.SecondaryFootBoneIndex = @object.SecondaryFootBoneIndex;
				this.RightFootIkEndEffectorBoneIndex = @object.RightFootIkEndEffectorBoneIndex;
				this.LeftFootIkEndEffectorBoneIndex = @object.LeftFootIkEndEffectorBoneIndex;
				this.RightFootIkTipBoneIndex = @object.RightFootIkTipBoneIndex;
				this.LeftFootIkTipBoneIndex = @object.LeftFootIkTipBoneIndex;
				this.FootNumBonesForIk = @object.FootNumBonesForIk;
				this.ReinHandleLeftLocalPosition = @object.ReinHandleLeftLocalPosition;
				this.ReinHandleRightLocalPosition = @object.ReinHandleRightLocalPosition;
				this.ReinSkeleton = @object.ReinSkeleton;
				this.ReinCollisionBody = @object.ReinCollisionBody;
				this.FrontBoneToDetectGroundSlopeIndex = @object.FrontBoneToDetectGroundSlopeIndex;
				this.BackBoneToDetectGroundSlopeIndex = @object.BackBoneToDetectGroundSlopeIndex;
				list7 = new List<sbyte>(@object.BoneIndicesToModifyOnSlopingGround);
				this.BodyRotationReferenceBoneIndex = @object.BodyRotationReferenceBoneIndex;
				this.RiderSitBoneIndex = @object.RiderSitBoneIndex;
				this.ReinHandleBoneIndex = @object.ReinHandleBoneIndex;
				this.ReinCollision1BoneIndex = @object.ReinCollision1BoneIndex;
				this.ReinCollision2BoneIndex = @object.ReinCollision2BoneIndex;
				this.ReinHeadBoneIndex = @object.ReinHeadBoneIndex;
				this.ReinHeadRightAttachmentBoneIndex = @object.ReinHeadRightAttachmentBoneIndex;
				this.ReinHeadLeftAttachmentBoneIndex = @object.ReinHeadLeftAttachmentBoneIndex;
				this.ReinRightHandBoneIndex = @object.ReinRightHandBoneIndex;
				this.ReinLeftHandBoneIndex = @object.ReinLeftHandBoneIndex;
			}
			else
			{
				list = new List<sbyte>(12);
				list2 = new List<sbyte>(4);
				list3 = new List<sbyte>(8);
				list4 = new List<sbyte>(8);
				list5 = new List<sbyte>(8);
				list6 = new List<sbyte>(8);
				list7 = new List<sbyte>(8);
			}
			XmlAttribute xmlAttribute2 = node.Attributes["action_set"];
			if (xmlAttribute2 != null && !string.IsNullOrEmpty(xmlAttribute2.Value))
			{
				this.ActionSetCode = xmlAttribute2.Value;
			}
			XmlAttribute xmlAttribute3 = node.Attributes["female_action_set"];
			if (xmlAttribute3 != null && !string.IsNullOrEmpty(xmlAttribute3.Value))
			{
				this.FemaleActionSetCode = xmlAttribute3.Value;
			}
			XmlAttribute xmlAttribute4 = node.Attributes["monster_usage"];
			if (xmlAttribute4 != null && !string.IsNullOrEmpty(xmlAttribute4.Value))
			{
				this.MonsterUsage = xmlAttribute4.Value;
			}
			else if (!flag)
			{
				this.MonsterUsage = "";
			}
			if (!flag)
			{
				this.Weight = 1;
			}
			XmlAttribute xmlAttribute5 = node.Attributes["weight"];
			int num;
			if (xmlAttribute5 != null && !string.IsNullOrEmpty(xmlAttribute5.Value) && int.TryParse(xmlAttribute5.Value, out num))
			{
				this.Weight = num;
			}
			if (!flag)
			{
				this.HitPoints = 1;
			}
			XmlAttribute xmlAttribute6 = node.Attributes["hit_points"];
			int num2;
			if (xmlAttribute6 != null && !string.IsNullOrEmpty(xmlAttribute6.Value) && int.TryParse(xmlAttribute6.Value, out num2))
			{
				this.HitPoints = num2;
			}
			XmlAttribute xmlAttribute7 = node.Attributes["num_paces"];
			int num3;
			if (xmlAttribute7 != null && !string.IsNullOrEmpty(xmlAttribute7.Value) && int.TryParse(xmlAttribute7.Value, out num3))
			{
				this.NumPaces = num3;
			}
			XmlAttribute xmlAttribute8 = node.Attributes["walking_speed_limit"];
			float num4;
			if (xmlAttribute8 != null && !string.IsNullOrEmpty(xmlAttribute8.Value) && float.TryParse(xmlAttribute8.Value, out num4))
			{
				this.WalkingSpeedLimit = num4;
			}
			XmlAttribute xmlAttribute9 = node.Attributes["crouch_walking_speed_limit"];
			if (xmlAttribute9 != null && !string.IsNullOrEmpty(xmlAttribute9.Value))
			{
				float num5;
				if (float.TryParse(xmlAttribute9.Value, out num5))
				{
					this.CrouchWalkingSpeedLimit = num5;
				}
			}
			else if (!flag)
			{
				this.CrouchWalkingSpeedLimit = this.WalkingSpeedLimit;
			}
			XmlAttribute xmlAttribute10 = node.Attributes["jump_acceleration"];
			float num6;
			if (xmlAttribute10 != null && !string.IsNullOrEmpty(xmlAttribute10.Value) && float.TryParse(xmlAttribute10.Value, out num6))
			{
				this.JumpAcceleration = num6;
			}
			XmlAttribute xmlAttribute11 = node.Attributes["absorbed_damage_ratio"];
			if (xmlAttribute11 != null && !string.IsNullOrEmpty(xmlAttribute11.Value))
			{
				float num7;
				if (float.TryParse(xmlAttribute11.Value, out num7))
				{
					if (num7 < 0f)
					{
						num7 = 0f;
					}
					this.AbsorbedDamageRatio = num7;
				}
			}
			else if (!flag)
			{
				this.AbsorbedDamageRatio = 1f;
			}
			XmlAttribute xmlAttribute12 = node.Attributes["sound_and_collision_info_class"];
			if (xmlAttribute12 != null && !string.IsNullOrEmpty(xmlAttribute12.Value))
			{
				this.SoundAndCollisionInfoClassName = xmlAttribute12.Value;
			}
			XmlAttribute xmlAttribute13 = node.Attributes["rider_camera_height_adder"];
			float num8;
			if (xmlAttribute13 != null && !string.IsNullOrEmpty(xmlAttribute13.Value) && float.TryParse(xmlAttribute13.Value, out num8))
			{
				this.RiderCameraHeightAdder = num8;
			}
			XmlAttribute xmlAttribute14 = node.Attributes["rider_body_capsule_height_adder"];
			float num9;
			if (xmlAttribute14 != null && !string.IsNullOrEmpty(xmlAttribute14.Value) && float.TryParse(xmlAttribute14.Value, out num9))
			{
				this.RiderBodyCapsuleHeightAdder = num9;
			}
			XmlAttribute xmlAttribute15 = node.Attributes["rider_body_capsule_forward_adder"];
			float num10;
			if (xmlAttribute15 != null && !string.IsNullOrEmpty(xmlAttribute15.Value) && float.TryParse(xmlAttribute15.Value, out num10))
			{
				this.RiderBodyCapsuleForwardAdder = num10;
			}
			XmlAttribute xmlAttribute16 = node.Attributes["preliminary_collision_capsule_radius_multiplier"];
			if (!flag && xmlAttribute16 != null && !string.IsNullOrEmpty(xmlAttribute16.Value))
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Monster.cs", "Deserialize", 425);
			}
			XmlAttribute xmlAttribute17 = node.Attributes["rider_preliminary_collision_capsule_height_multiplier"];
			if (!flag && xmlAttribute17 != null && !string.IsNullOrEmpty(xmlAttribute17.Value))
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Monster.cs", "Deserialize", 434);
			}
			XmlAttribute xmlAttribute18 = node.Attributes["rider_preliminary_collision_capsule_height_adder"];
			if (!flag && xmlAttribute18 != null && !string.IsNullOrEmpty(xmlAttribute18.Value))
			{
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Monster.cs", "Deserialize", 443);
			}
			XmlAttribute xmlAttribute19 = node.Attributes["standing_eye_height"];
			float num11;
			if (xmlAttribute19 != null && !string.IsNullOrEmpty(xmlAttribute19.Value) && float.TryParse(xmlAttribute19.Value, out num11))
			{
				this.StandingEyeHeight = num11;
			}
			XmlAttribute xmlAttribute20 = node.Attributes["crouch_eye_height"];
			float num12;
			if (xmlAttribute20 != null && !string.IsNullOrEmpty(xmlAttribute20.Value) && float.TryParse(xmlAttribute20.Value, out num12))
			{
				this.CrouchEyeHeight = num12;
			}
			XmlAttribute xmlAttribute21 = node.Attributes["mounted_eye_height"];
			float num13;
			if (xmlAttribute21 != null && !string.IsNullOrEmpty(xmlAttribute21.Value) && float.TryParse(xmlAttribute21.Value, out num13))
			{
				this.MountedEyeHeight = num13;
			}
			XmlAttribute xmlAttribute22 = node.Attributes["rider_eye_height_adder"];
			float num14;
			if (xmlAttribute22 != null && !string.IsNullOrEmpty(xmlAttribute22.Value) && float.TryParse(xmlAttribute22.Value, out num14))
			{
				this.RiderEyeHeightAdder = num14;
			}
			if (!flag)
			{
				this.EyeOffsetWrtHead = new Vec3(0.01f, 0.01f, 0.01f, -1f);
			}
			XmlAttribute xmlAttribute23 = node.Attributes["eye_offset_wrt_head"];
			Vec3 vec;
			if (xmlAttribute23 != null && !string.IsNullOrEmpty(xmlAttribute23.Value) && Monster.ReadVec3(xmlAttribute23.Value, out vec))
			{
				this.EyeOffsetWrtHead = vec;
			}
			if (!flag)
			{
				this.FirstPersonCameraOffsetWrtHead = new Vec3(0.01f, 0.01f, 0.01f, -1f);
			}
			XmlAttribute xmlAttribute24 = node.Attributes["first_person_camera_offset_wrt_head"];
			Vec3 vec2;
			if (xmlAttribute24 != null && !string.IsNullOrEmpty(xmlAttribute24.Value) && Monster.ReadVec3(xmlAttribute24.Value, out vec2))
			{
				this.FirstPersonCameraOffsetWrtHead = vec2;
			}
			XmlAttribute xmlAttribute25 = node.Attributes["arm_length"];
			float num15;
			if (xmlAttribute25 != null && !string.IsNullOrEmpty(xmlAttribute25.Value) && float.TryParse(xmlAttribute25.Value, out num15))
			{
				this.ArmLength = num15;
			}
			XmlAttribute xmlAttribute26 = node.Attributes["arm_weight"];
			float num16;
			if (xmlAttribute26 != null && !string.IsNullOrEmpty(xmlAttribute26.Value) && float.TryParse(xmlAttribute26.Value, out num16))
			{
				this.ArmWeight = num16;
			}
			XmlAttribute xmlAttribute27 = node.Attributes["jump_speed_limit"];
			float num17;
			if (xmlAttribute27 != null && !string.IsNullOrEmpty(xmlAttribute27.Value) && float.TryParse(xmlAttribute27.Value, out num17))
			{
				this.JumpSpeedLimit = num17;
			}
			if (!flag)
			{
				this.RelativeSpeedLimitForCharge = float.MaxValue;
			}
			XmlAttribute xmlAttribute28 = node.Attributes["relative_speed_limit_for_charge"];
			float num18;
			if (xmlAttribute28 != null && !string.IsNullOrEmpty(xmlAttribute28.Value) && float.TryParse(xmlAttribute28.Value, out num18))
			{
				this.RelativeSpeedLimitForCharge = num18;
			}
			XmlAttribute xmlAttribute29 = node.Attributes["family_type"];
			int num19;
			if (xmlAttribute29 != null && !string.IsNullOrEmpty(xmlAttribute29.Value) && int.TryParse(xmlAttribute29.Value, out num19))
			{
				this.FamilyType = num19;
			}
			sbyte b = -1;
			this.DeserializeBoneIndexArray(list, node, flag, "ragdoll_bone_to_check_for_corpses_", b, false);
			this.DeserializeBoneIndexArray(list2, node, flag, "ragdoll_fall_sound_bone_", b, false);
			this.HeadLookDirectionBoneIndex = this.DeserializeBoneIndex(node, "head_look_direction_bone", flag ? this.HeadLookDirectionBoneIndex : b, b, true);
			this.SpineLowerBoneIndex = this.DeserializeBoneIndex(node, "spine_lower_bone", flag ? this.SpineLowerBoneIndex : b, b, false);
			this.SpineUpperBoneIndex = this.DeserializeBoneIndex(node, "spine_upper_bone", flag ? this.SpineUpperBoneIndex : b, b, false);
			this.ThoraxLookDirectionBoneIndex = this.DeserializeBoneIndex(node, "thorax_look_direction_bone", flag ? this.ThoraxLookDirectionBoneIndex : b, b, true);
			this.NeckRootBoneIndex = this.DeserializeBoneIndex(node, "neck_root_bone", flag ? this.NeckRootBoneIndex : b, b, true);
			this.PelvisBoneIndex = this.DeserializeBoneIndex(node, "pelvis_bone", flag ? this.PelvisBoneIndex : b, b, false);
			this.RightUpperArmBoneIndex = this.DeserializeBoneIndex(node, "right_upper_arm_bone", flag ? this.RightUpperArmBoneIndex : b, b, false);
			this.LeftUpperArmBoneIndex = this.DeserializeBoneIndex(node, "left_upper_arm_bone", flag ? this.LeftUpperArmBoneIndex : b, b, false);
			this.FallBlowDamageBoneIndex = this.DeserializeBoneIndex(node, "fall_blow_damage_bone", flag ? this.FallBlowDamageBoneIndex : b, b, false);
			this.TerrainDecalBone0Index = this.DeserializeBoneIndex(node, "terrain_decal_bone_0", flag ? this.TerrainDecalBone0Index : b, b, false);
			this.TerrainDecalBone1Index = this.DeserializeBoneIndex(node, "terrain_decal_bone_1", flag ? this.TerrainDecalBone1Index : b, b, false);
			this.DeserializeBoneIndexArray(list3, node, flag, "ragdoll_stationary_check_bone_", b, false);
			this.DeserializeBoneIndexArray(list4, node, flag, "move_adder_bone_", b, false);
			this.DeserializeBoneIndexArray(list5, node, flag, "splash_decal_bone_", b, false);
			this.DeserializeBoneIndexArray(list6, node, flag, "blood_burst_bone_", b, false);
			this.MainHandBoneIndex = this.DeserializeBoneIndex(node, "main_hand_bone", flag ? this.MainHandBoneIndex : b, b, true);
			this.OffHandBoneIndex = this.DeserializeBoneIndex(node, "off_hand_bone", flag ? this.OffHandBoneIndex : b, b, true);
			this.MainHandItemBoneIndex = this.DeserializeBoneIndex(node, "main_hand_item_bone", flag ? this.MainHandItemBoneIndex : b, b, true);
			this.OffHandItemBoneIndex = this.DeserializeBoneIndex(node, "off_hand_item_bone", flag ? this.OffHandItemBoneIndex : b, b, true);
			this.MainHandItemSecondaryBoneIndex = this.DeserializeBoneIndex(node, "main_hand_item_secondary_bone", flag ? this.MainHandItemSecondaryBoneIndex : b, b, false);
			this.OffHandItemSecondaryBoneIndex = this.DeserializeBoneIndex(node, "off_hand_item_secondary_bone", flag ? this.OffHandItemSecondaryBoneIndex : b, b, false);
			this.OffHandShoulderBoneIndex = this.DeserializeBoneIndex(node, "off_hand_shoulder_bone", flag ? this.OffHandShoulderBoneIndex : b, b, false);
			XmlAttribute xmlAttribute30 = node.Attributes["hand_num_bones_for_ik"];
			this.HandNumBonesForIk = ((xmlAttribute30 != null) ? sbyte.Parse(xmlAttribute30.Value) : (flag ? this.HandNumBonesForIk : 0));
			this.PrimaryFootBoneIndex = this.DeserializeBoneIndex(node, "primary_foot_bone", flag ? this.PrimaryFootBoneIndex : b, b, false);
			this.SecondaryFootBoneIndex = this.DeserializeBoneIndex(node, "secondary_foot_bone", flag ? this.SecondaryFootBoneIndex : b, b, false);
			this.RightFootIkEndEffectorBoneIndex = this.DeserializeBoneIndex(node, "right_foot_ik_end_effector_bone", flag ? this.RightFootIkEndEffectorBoneIndex : b, b, true);
			this.LeftFootIkEndEffectorBoneIndex = this.DeserializeBoneIndex(node, "left_foot_ik_end_effector_bone", flag ? this.LeftFootIkEndEffectorBoneIndex : b, b, true);
			this.RightFootIkTipBoneIndex = this.DeserializeBoneIndex(node, "right_foot_ik_tip_bone", flag ? this.RightFootIkTipBoneIndex : b, b, true);
			this.LeftFootIkTipBoneIndex = this.DeserializeBoneIndex(node, "left_foot_ik_tip_bone", flag ? this.LeftFootIkTipBoneIndex : b, b, true);
			XmlAttribute xmlAttribute31 = node.Attributes["foot_num_bones_for_ik"];
			this.FootNumBonesForIk = ((xmlAttribute31 != null) ? sbyte.Parse(xmlAttribute31.Value) : (flag ? this.FootNumBonesForIk : 0));
			XmlNode xmlNode = node.Attributes["rein_handle_left_local_pos"];
			Vec3 vec3;
			if (xmlNode != null && Monster.ReadVec3(xmlNode.Value, out vec3))
			{
				this.ReinHandleLeftLocalPosition = vec3;
			}
			XmlNode xmlNode2 = node.Attributes["rein_handle_right_local_pos"];
			Vec3 vec4;
			if (xmlNode2 != null && Monster.ReadVec3(xmlNode2.Value, out vec4))
			{
				this.ReinHandleRightLocalPosition = vec4;
			}
			XmlAttribute xmlAttribute32 = node.Attributes["rein_skeleton"];
			this.ReinSkeleton = ((xmlAttribute32 != null) ? xmlAttribute32.Value : this.ReinSkeleton);
			XmlAttribute xmlAttribute33 = node.Attributes["rein_collision_body"];
			this.ReinCollisionBody = ((xmlAttribute33 != null) ? xmlAttribute33.Value : this.ReinCollisionBody);
			this.DeserializeBoneIndexArray(list7, node, flag, "bones_to_modify_on_sloping_ground_", b, true);
			XmlAttribute xmlAttribute34 = node.Attributes["front_bone_to_detect_ground_slope_index"];
			this.FrontBoneToDetectGroundSlopeIndex = ((xmlAttribute34 != null) ? sbyte.Parse(xmlAttribute34.Value) : (flag ? this.FrontBoneToDetectGroundSlopeIndex : -1));
			XmlAttribute xmlAttribute35 = node.Attributes["back_bone_to_detect_ground_slope_index"];
			this.BackBoneToDetectGroundSlopeIndex = ((xmlAttribute35 != null) ? sbyte.Parse(xmlAttribute35.Value) : (flag ? this.BackBoneToDetectGroundSlopeIndex : -1));
			this.BodyRotationReferenceBoneIndex = this.DeserializeBoneIndex(node, "body_rotation_reference_bone", flag ? this.BodyRotationReferenceBoneIndex : b, b, true);
			this.RiderSitBoneIndex = this.DeserializeBoneIndex(node, "rider_sit_bone", flag ? this.RiderSitBoneIndex : b, b, false);
			this.ReinHandleBoneIndex = this.DeserializeBoneIndex(node, "rein_handle_bone", flag ? this.ReinHandleBoneIndex : b, b, false);
			this.ReinCollision1BoneIndex = this.DeserializeBoneIndex(node, "rein_collision_1_bone", flag ? this.ReinCollision1BoneIndex : b, b, false);
			this.ReinCollision2BoneIndex = this.DeserializeBoneIndex(node, "rein_collision_2_bone", flag ? this.ReinCollision2BoneIndex : b, b, false);
			this.ReinHeadBoneIndex = this.DeserializeBoneIndex(node, "rein_head_bone", flag ? this.ReinHeadBoneIndex : b, b, false);
			this.ReinHeadRightAttachmentBoneIndex = this.DeserializeBoneIndex(node, "rein_head_right_attachment_bone", flag ? this.ReinHeadRightAttachmentBoneIndex : b, b, false);
			this.ReinHeadLeftAttachmentBoneIndex = this.DeserializeBoneIndex(node, "rein_head_left_attachment_bone", flag ? this.ReinHeadLeftAttachmentBoneIndex : b, b, false);
			this.ReinRightHandBoneIndex = this.DeserializeBoneIndex(node, "rein_right_hand_bone", flag ? this.ReinRightHandBoneIndex : b, b, false);
			this.ReinLeftHandBoneIndex = this.DeserializeBoneIndex(node, "rein_left_hand_bone", flag ? this.ReinLeftHandBoneIndex : b, b, false);
			this.IndicesOfRagdollBonesToCheckForCorpses = list.ToArray();
			this.RagdollFallSoundBoneIndices = list2.ToArray();
			this.RagdollStationaryCheckBoneIndices = list3.ToArray();
			this.MoveAdderBoneIndices = list4.ToArray();
			this.SplashDecalBoneIndices = list5.ToArray();
			this.BloodBurstBoneIndices = list6.ToArray();
			this.BoneIndicesToModifyOnSlopingGround = list7.ToArray();
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode3 = (XmlNode)obj;
				if (xmlNode3.Name == "Flags")
				{
					this.Flags = AgentFlag.None;
					using (IEnumerator enumerator2 = Enum.GetValues(typeof(AgentFlag)).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							AgentFlag agentFlag = (AgentFlag)obj2;
							XmlAttribute xmlAttribute36 = xmlNode3.Attributes[agentFlag.ToString()];
							if (xmlAttribute36 != null && !xmlAttribute36.Value.Equals("false", StringComparison.InvariantCultureIgnoreCase))
							{
								this.Flags |= agentFlag;
							}
						}
						continue;
					}
				}
				if (xmlNode3.Name == "Capsules")
				{
					foreach (object obj3 in xmlNode3.ChildNodes)
					{
						XmlNode xmlNode4 = (XmlNode)obj3;
						if (xmlNode4.Attributes != null && (xmlNode4.Name == "preliminary_collision_capsule" || xmlNode4.Name == "body_capsule" || xmlNode4.Name == "crouched_body_capsule"))
						{
							bool flag2 = true;
							Vec3 vec5 = new Vec3(0f, 0f, 0.01f, -1f);
							Vec3 vec6 = Vec3.Zero;
							float num20 = 0.01f;
							if (xmlNode4.Attributes["pos1"] != null)
							{
								Vec3 vec7;
								flag2 = Monster.ReadVec3(xmlNode4.Attributes["pos1"].Value, out vec7) && flag2;
								if (flag2)
								{
									vec5 = vec7;
								}
							}
							if (xmlNode4.Attributes["pos2"] != null)
							{
								Vec3 vec8;
								flag2 = Monster.ReadVec3(xmlNode4.Attributes["pos2"].Value, out vec8) && flag2;
								if (flag2)
								{
									vec6 = vec8;
								}
							}
							if (xmlNode4.Attributes["radius"] != null)
							{
								string text = xmlNode4.Attributes["radius"].Value;
								text = text.Trim();
								flag2 = flag2 && float.TryParse(text, out num20);
							}
							if (flag2)
							{
								if (xmlNode4.Name.StartsWith("p"))
								{
									Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core\\Monster.cs", "Deserialize", 705);
								}
								else if (xmlNode4.Name.StartsWith("c"))
								{
									this.CrouchedBodyCapsuleRadius = num20;
									this.CrouchedBodyCapsulePoint1 = vec5;
									this.CrouchedBodyCapsulePoint2 = vec6;
								}
								else
								{
									this.BodyCapsuleRadius = num20;
									this.BodyCapsulePoint1 = vec5;
									this.BodyCapsulePoint2 = vec6;
								}
							}
						}
					}
				}
			}
		}

		private sbyte DeserializeBoneIndex(XmlNode node, string attributeName, sbyte baseValue, sbyte invalidBoneIndex, bool validateHasParentBone)
		{
			XmlAttribute xmlAttribute = node.Attributes[attributeName];
			sbyte b = ((Monster.GetBoneIndexWithId != null && xmlAttribute != null) ? Monster.GetBoneIndexWithId(this.ActionSetCode, xmlAttribute.Value) : baseValue);
			if (validateHasParentBone && b != invalidBoneIndex)
			{
				Func<string, sbyte, bool> getBoneHasParentBone = Monster.GetBoneHasParentBone;
			}
			return b;
		}

		private void DeserializeBoneIndexArray(List<sbyte> boneIndices, XmlNode node, bool hasBaseMonster, string attributeNamePrefix, sbyte invalidBoneIndex, bool validateHasParentBone)
		{
			int num = 0;
			for (;;)
			{
				bool flag = hasBaseMonster && num < boneIndices.Count;
				sbyte b = this.DeserializeBoneIndex(node, attributeNamePrefix + num, flag ? boneIndices[num] : invalidBoneIndex, invalidBoneIndex, validateHasParentBone);
				if (b == invalidBoneIndex)
				{
					break;
				}
				if (flag)
				{
					boneIndices[num] = b;
				}
				else
				{
					boneIndices.Add(b);
				}
				num++;
			}
		}

		private static bool ReadVec3(string str, out Vec3 v)
		{
			str = str.Trim();
			string[] array = str.Split(",".ToCharArray());
			v = new Vec3(0f, 0f, 0f, -1f);
			return float.TryParse(array[0], out v.x) && float.TryParse(array[1], out v.y) && float.TryParse(array[2], out v.z);
		}

		public sbyte GetBoneToAttachForItemFlags(ItemFlags itemFlags)
		{
			ItemFlags itemFlags2 = itemFlags & ItemFlags.AttachmentMask;
			if (itemFlags2 <= (ItemFlags)0U)
			{
				return this.MainHandItemBoneIndex;
			}
			if (itemFlags2 == ItemFlags.ForceAttachOffHandPrimaryItemBone)
			{
				return this.OffHandItemBoneIndex;
			}
			if (itemFlags2 != ItemFlags.ForceAttachOffHandSecondaryItemBone)
			{
				return this.MainHandItemBoneIndex;
			}
			return this.OffHandItemSecondaryBoneIndex;
		}

		public static Func<string, string, sbyte> GetBoneIndexWithId;

		public static Func<string, sbyte, bool> GetBoneHasParentBone;

		[CachedData]
		private IMonsterMissionData _monsterMissionData;
	}
}
