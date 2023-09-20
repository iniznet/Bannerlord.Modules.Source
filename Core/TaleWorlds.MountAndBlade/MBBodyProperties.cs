using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001AA RID: 426
	public static class MBBodyProperties
	{
		// Token: 0x06001734 RID: 5940 RVA: 0x0004F65B File Offset: 0x0004D85B
		public static int GetNumEditableDeformKeys(int race, bool initialGender, int age)
		{
			return MBAPI.IMBFaceGen.GetNumEditableDeformKeys(race, initialGender, (float)age);
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x0004F66B File Offset: 0x0004D86B
		public static void GetParamsFromKey(ref FaceGenerationParams faceGenerationParams, BodyProperties bodyProperties, bool earsAreHidden, bool mouthHidden)
		{
			MBAPI.IMBFaceGen.GetParamsFromKey(ref faceGenerationParams, ref bodyProperties, earsAreHidden, mouthHidden);
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x0004F67C File Offset: 0x0004D87C
		public static void GetParamsMax(int race, int curGender, int curAge, ref int hairNum, ref int beardNum, ref int faceTextureNum, ref int mouthTextureNum, ref int faceTattooNum, ref int soundNum, ref int eyebrowNum, ref float scale)
		{
			MBAPI.IMBFaceGen.GetParamsMax(race, curGender, (float)curAge, ref hairNum, ref beardNum, ref faceTextureNum, ref mouthTextureNum, ref faceTattooNum, ref soundNum, ref eyebrowNum, ref scale);
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x0004F6A6 File Offset: 0x0004D8A6
		public static void GetZeroProbabilities(int race, int curGender, float curAge, ref float tattooZeroProbability)
		{
			MBAPI.IMBFaceGen.GetZeroProbabilities(race, curGender, curAge, ref tattooZeroProbability);
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x0004F6B6 File Offset: 0x0004D8B6
		public static void ProduceNumericKeyWithParams(FaceGenerationParams faceGenerationParams, bool earsAreHidden, bool mouthIsHidden, ref BodyProperties bodyProperties)
		{
			MBAPI.IMBFaceGen.ProduceNumericKeyWithParams(ref faceGenerationParams, earsAreHidden, mouthIsHidden, ref bodyProperties);
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x0004F6C7 File Offset: 0x0004D8C7
		public static void TransformFaceKeysToDefaultFace(ref FaceGenerationParams faceGenerationParams)
		{
			MBAPI.IMBFaceGen.TransformFaceKeysToDefaultFace(ref faceGenerationParams);
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x0004F6D4 File Offset: 0x0004D8D4
		public static void ProduceNumericKeyWithDefaultValues(ref BodyProperties initialBodyProperties, bool earsAreHidden, bool mouthIsHidden, int race, int gender, int age)
		{
			MBAPI.IMBFaceGen.ProduceNumericKeyWithDefaultValues(ref initialBodyProperties, earsAreHidden, mouthIsHidden, race, gender, (float)age);
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x0004F6EC File Offset: 0x0004D8EC
		public static BodyProperties GetRandomBodyProperties(int race, bool isFemale, BodyProperties bodyPropertiesMin, BodyProperties bodyPropertiesMax, int hairCoverType, int seed, string hairTags, string beardTags, string tatooTags)
		{
			BodyProperties bodyProperties = default(BodyProperties);
			MBAPI.IMBFaceGen.GetRandomBodyProperties(race, isFemale ? 1 : 0, ref bodyPropertiesMin, ref bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tatooTags, ref bodyProperties);
			return bodyProperties;
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x0004F724 File Offset: 0x0004D924
		public static DeformKeyData GetDeformKeyData(int keyNo, int race, int gender, int age)
		{
			DeformKeyData deformKeyData = default(DeformKeyData);
			MBAPI.IMBFaceGen.GetDeformKeyData(keyNo, ref deformKeyData, race, gender, (float)age);
			return deformKeyData;
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x0004F74B File Offset: 0x0004D94B
		public static int GetFaceGenInstancesLength(int race, int gender, int age)
		{
			return MBAPI.IMBFaceGen.GetFaceGenInstancesLength(race, gender, (float)age);
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x0004F75B File Offset: 0x0004D95B
		public static bool EnforceConstraints(ref FaceGenerationParams faceGenerationParams)
		{
			return MBAPI.IMBFaceGen.EnforceConstraints(ref faceGenerationParams);
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x0004F768 File Offset: 0x0004D968
		public static float GetScaleFromKey(int race, int gender, BodyProperties bodyProperties)
		{
			return MBAPI.IMBFaceGen.GetScaleFromKey(race, gender, ref bodyProperties);
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x0004F778 File Offset: 0x0004D978
		public static int GetHairColorCount(int race, int curGender, int age)
		{
			return MBAPI.IMBFaceGen.GetHairColorCount(race, curGender, (float)age);
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x0004F788 File Offset: 0x0004D988
		public static List<uint> GetHairColorGradientPoints(int race, int curGender, int age)
		{
			int hairColorCount = MBBodyProperties.GetHairColorCount(race, curGender, age);
			List<uint> list = new List<uint>();
			Vec3[] array = new Vec3[hairColorCount];
			MBAPI.IMBFaceGen.GetHairColorGradientPoints(race, curGender, (float)age, array);
			foreach (Vec3 vec in array)
			{
				list.Add(MBMath.ColorFromRGBA(vec.x, vec.y, vec.z, 1f));
			}
			return list;
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x0004F7F7 File Offset: 0x0004D9F7
		public static int GetTatooColorCount(int race, int curGender, int age)
		{
			return MBAPI.IMBFaceGen.GetTatooColorCount(race, curGender, (float)age);
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x0004F808 File Offset: 0x0004DA08
		public static List<uint> GetTatooColorGradientPoints(int race, int curGender, int age)
		{
			int tatooColorCount = MBBodyProperties.GetTatooColorCount(race, curGender, age);
			List<uint> list = new List<uint>();
			Vec3[] array = new Vec3[tatooColorCount];
			MBAPI.IMBFaceGen.GetTatooColorGradientPoints(race, curGender, (float)age, array);
			foreach (Vec3 vec in array)
			{
				list.Add(MBMath.ColorFromRGBA(vec.x, vec.y, vec.z, 1f));
			}
			return list;
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x0004F877 File Offset: 0x0004DA77
		public static int GetSkinColorCount(int race, int curGender, int age)
		{
			return MBAPI.IMBFaceGen.GetSkinColorCount(race, curGender, (float)age);
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x0004F887 File Offset: 0x0004DA87
		public static BodyMeshMaturityType GetMaturityType(float age)
		{
			return (BodyMeshMaturityType)MBAPI.IMBFaceGen.GetMaturityType(age);
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x0004F894 File Offset: 0x0004DA94
		public static string[] GetRaceIds()
		{
			return MBAPI.IMBFaceGen.GetRaceIds().Split(new char[] { ';' });
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x0004F8B0 File Offset: 0x0004DAB0
		public static List<uint> GetSkinColorGradientPoints(int race, int curGender, int age)
		{
			int skinColorCount = MBBodyProperties.GetSkinColorCount(race, curGender, age);
			List<uint> list = new List<uint>();
			Vec3[] array = new Vec3[skinColorCount];
			MBAPI.IMBFaceGen.GetSkinColorGradientPoints(race, curGender, (float)age, array);
			foreach (Vec3 vec in array)
			{
				list.Add(MBMath.ColorFromRGBA(vec.x, vec.y, vec.z, 1f));
			}
			return list;
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x0004F920 File Offset: 0x0004DB20
		public static List<bool> GetVoiceTypeUsableForPlayerData(int race, int curGender, float age, int voiceTypeCount)
		{
			bool[] array = new bool[voiceTypeCount];
			MBAPI.IMBFaceGen.GetVoiceTypeUsableForPlayerData(race, curGender, age, array);
			return new List<bool>(array);
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x0004F948 File Offset: 0x0004DB48
		public static void SetHair(ref BodyProperties bodyProperties, int hair, int beard, int tattoo)
		{
			FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false, false);
			if (hair > -1)
			{
				faceGenerationParams._currentHair = hair;
			}
			if (beard > -1)
			{
				faceGenerationParams._curBeard = beard;
			}
			if (tattoo > -1)
			{
				faceGenerationParams._curFaceTattoo = tattoo;
			}
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, false, ref bodyProperties);
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x0004F998 File Offset: 0x0004DB98
		public static void SetBody(ref BodyProperties bodyProperties, int build, int weight)
		{
			FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false, false);
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, false, ref bodyProperties);
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x0004F9CC File Offset: 0x0004DBCC
		public static void SetPigmentation(ref BodyProperties bodyProperties, int skinColor, int hairColor, int eyeColor)
		{
			FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false, false);
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, false, ref bodyProperties);
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x0004FA04 File Offset: 0x0004DC04
		public static void GenerateParentKey(BodyProperties childBodyProperties, int race, ref BodyProperties motherBodyProperties, ref BodyProperties fatherBodyProperties)
		{
			FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
			FaceGenerationParams faceGenerationParams2 = FaceGenerationParams.Create();
			FaceGenerationParams faceGenerationParams3 = FaceGenerationParams.Create();
			MBBodyProperties.GenerationType[] array = new MBBodyProperties.GenerationType[4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (MBBodyProperties.GenerationType)MBRandom.RandomInt(2);
			}
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, childBodyProperties, false, false);
			int faceGenInstancesLength = MBBodyProperties.GetFaceGenInstancesLength(race, faceGenerationParams._currentGender, (int)faceGenerationParams._curAge);
			for (int j = 0; j < faceGenInstancesLength; j++)
			{
				DeformKeyData deformKeyData = MBBodyProperties.GetDeformKeyData(j, race, faceGenerationParams._currentGender, (int)faceGenerationParams._curAge);
				if (deformKeyData.GroupId >= 0 && deformKeyData.GroupId != 0 && deformKeyData.GroupId != 5 && deformKeyData.GroupId != 6)
				{
					float num = MBRandom.RandomFloat * MathF.Min(faceGenerationParams.KeyWeights[j], 1f - faceGenerationParams.KeyWeights[j]);
					if (array[deformKeyData.GroupId - 1] == MBBodyProperties.GenerationType.FromMother)
					{
						faceGenerationParams3.KeyWeights[j] = faceGenerationParams.KeyWeights[j];
						faceGenerationParams2.KeyWeights[j] = faceGenerationParams.KeyWeights[j] + num;
					}
					else if (array[deformKeyData.GroupId - 1] == MBBodyProperties.GenerationType.FromFather)
					{
						faceGenerationParams2.KeyWeights[j] = faceGenerationParams.KeyWeights[j];
						faceGenerationParams3.KeyWeights[j] = faceGenerationParams.KeyWeights[j] + num;
					}
					else
					{
						faceGenerationParams3.KeyWeights[j] = faceGenerationParams.KeyWeights[j] + num;
						faceGenerationParams2.KeyWeights[j] = faceGenerationParams.KeyWeights[j] - num;
					}
				}
			}
			faceGenerationParams2._curAge = faceGenerationParams._curAge + (float)MBRandom.RandomInt(18, 25);
			float num2;
			faceGenerationParams2.SetRandomParamsExceptKeys(race, 0, (int)faceGenerationParams2._curAge, out num2);
			faceGenerationParams2._curFaceTattoo = 0;
			faceGenerationParams3._curAge = faceGenerationParams._curAge + (float)MBRandom.RandomInt(18, 22);
			float num3;
			faceGenerationParams3.SetRandomParamsExceptKeys(race, 1, (int)faceGenerationParams3._curAge, out num3);
			faceGenerationParams3._curFaceTattoo = 0;
			faceGenerationParams3._heightMultiplier = faceGenerationParams2._heightMultiplier * MBRandom.RandomFloatRanged(0.7f, 0.9f);
			if (faceGenerationParams3._currentHair == 0)
			{
				faceGenerationParams3._currentHair = 1;
			}
			float num4 = MBRandom.RandomFloat * MathF.Min(faceGenerationParams._curSkinColorOffset, 1f - faceGenerationParams._curSkinColorOffset);
			if (MBRandom.RandomInt(2) == 1)
			{
				faceGenerationParams2._curSkinColorOffset = faceGenerationParams._curSkinColorOffset + num4;
				faceGenerationParams3._curSkinColorOffset = faceGenerationParams._curSkinColorOffset - num4;
			}
			else
			{
				faceGenerationParams2._curSkinColorOffset = faceGenerationParams._curSkinColorOffset - num4;
				faceGenerationParams3._curSkinColorOffset = faceGenerationParams._curSkinColorOffset + num4;
			}
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams3, false, false, ref motherBodyProperties);
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams2, false, false, ref fatherBodyProperties);
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x0004FC90 File Offset: 0x0004DE90
		public static BodyProperties GetBodyPropertiesWithAge(ref BodyProperties bodyProperties, float age)
		{
			FaceGenerationParams faceGenerationParams = default(FaceGenerationParams);
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false, false);
			faceGenerationParams._curAge = age;
			BodyProperties bodyProperties2 = default(BodyProperties);
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, false, ref bodyProperties2);
			return bodyProperties2;
		}

		// Token: 0x020004FD RID: 1277
		public enum GenerationType
		{
			// Token: 0x04001B5A RID: 7002
			FromMother,
			// Token: 0x04001B5B RID: 7003
			FromFather,
			// Token: 0x04001B5C RID: 7004
			Count
		}
	}
}
