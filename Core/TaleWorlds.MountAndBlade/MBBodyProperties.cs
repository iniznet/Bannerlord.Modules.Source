using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class MBBodyProperties
	{
		public static int GetNumEditableDeformKeys(int race, bool initialGender, int age)
		{
			return MBAPI.IMBFaceGen.GetNumEditableDeformKeys(race, initialGender, (float)age);
		}

		public static void GetParamsFromKey(ref FaceGenerationParams faceGenerationParams, BodyProperties bodyProperties, bool earsAreHidden, bool mouthHidden)
		{
			MBAPI.IMBFaceGen.GetParamsFromKey(ref faceGenerationParams, ref bodyProperties, earsAreHidden, mouthHidden);
		}

		public static void GetParamsMax(int race, int curGender, int curAge, ref int hairNum, ref int beardNum, ref int faceTextureNum, ref int mouthTextureNum, ref int faceTattooNum, ref int soundNum, ref int eyebrowNum, ref float scale)
		{
			MBAPI.IMBFaceGen.GetParamsMax(race, curGender, (float)curAge, ref hairNum, ref beardNum, ref faceTextureNum, ref mouthTextureNum, ref faceTattooNum, ref soundNum, ref eyebrowNum, ref scale);
		}

		public static void GetZeroProbabilities(int race, int curGender, float curAge, ref float tattooZeroProbability)
		{
			MBAPI.IMBFaceGen.GetZeroProbabilities(race, curGender, curAge, ref tattooZeroProbability);
		}

		public static void ProduceNumericKeyWithParams(FaceGenerationParams faceGenerationParams, bool earsAreHidden, bool mouthIsHidden, ref BodyProperties bodyProperties)
		{
			MBAPI.IMBFaceGen.ProduceNumericKeyWithParams(ref faceGenerationParams, earsAreHidden, mouthIsHidden, ref bodyProperties);
		}

		public static void TransformFaceKeysToDefaultFace(ref FaceGenerationParams faceGenerationParams)
		{
			MBAPI.IMBFaceGen.TransformFaceKeysToDefaultFace(ref faceGenerationParams);
		}

		public static void ProduceNumericKeyWithDefaultValues(ref BodyProperties initialBodyProperties, bool earsAreHidden, bool mouthIsHidden, int race, int gender, int age)
		{
			MBAPI.IMBFaceGen.ProduceNumericKeyWithDefaultValues(ref initialBodyProperties, earsAreHidden, mouthIsHidden, race, gender, (float)age);
		}

		public static BodyProperties GetRandomBodyProperties(int race, bool isFemale, BodyProperties bodyPropertiesMin, BodyProperties bodyPropertiesMax, int hairCoverType, int seed, string hairTags, string beardTags, string tatooTags)
		{
			BodyProperties bodyProperties = default(BodyProperties);
			MBAPI.IMBFaceGen.GetRandomBodyProperties(race, isFemale ? 1 : 0, ref bodyPropertiesMin, ref bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tatooTags, ref bodyProperties);
			return bodyProperties;
		}

		public static DeformKeyData GetDeformKeyData(int keyNo, int race, int gender, int age)
		{
			DeformKeyData deformKeyData = default(DeformKeyData);
			MBAPI.IMBFaceGen.GetDeformKeyData(keyNo, ref deformKeyData, race, gender, (float)age);
			return deformKeyData;
		}

		public static int GetFaceGenInstancesLength(int race, int gender, int age)
		{
			return MBAPI.IMBFaceGen.GetFaceGenInstancesLength(race, gender, (float)age);
		}

		public static bool EnforceConstraints(ref FaceGenerationParams faceGenerationParams)
		{
			return MBAPI.IMBFaceGen.EnforceConstraints(ref faceGenerationParams);
		}

		public static float GetScaleFromKey(int race, int gender, BodyProperties bodyProperties)
		{
			return MBAPI.IMBFaceGen.GetScaleFromKey(race, gender, ref bodyProperties);
		}

		public static int GetHairColorCount(int race, int curGender, int age)
		{
			return MBAPI.IMBFaceGen.GetHairColorCount(race, curGender, (float)age);
		}

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

		public static int GetTatooColorCount(int race, int curGender, int age)
		{
			return MBAPI.IMBFaceGen.GetTatooColorCount(race, curGender, (float)age);
		}

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

		public static int GetSkinColorCount(int race, int curGender, int age)
		{
			return MBAPI.IMBFaceGen.GetSkinColorCount(race, curGender, (float)age);
		}

		public static BodyMeshMaturityType GetMaturityType(float age)
		{
			return (BodyMeshMaturityType)MBAPI.IMBFaceGen.GetMaturityType(age);
		}

		public static string[] GetRaceIds()
		{
			return MBAPI.IMBFaceGen.GetRaceIds().Split(new char[] { ';' });
		}

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

		public static List<bool> GetVoiceTypeUsableForPlayerData(int race, int curGender, float age, int voiceTypeCount)
		{
			bool[] array = new bool[voiceTypeCount];
			MBAPI.IMBFaceGen.GetVoiceTypeUsableForPlayerData(race, curGender, age, array);
			return new List<bool>(array);
		}

		public static void SetHair(ref BodyProperties bodyProperties, int hair, int beard, int tattoo)
		{
			FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false, false);
			if (hair > -1)
			{
				faceGenerationParams.CurrentHair = hair;
			}
			if (beard > -1)
			{
				faceGenerationParams.CurrentBeard = beard;
			}
			if (tattoo > -1)
			{
				faceGenerationParams.CurrentFaceTattoo = tattoo;
			}
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, false, ref bodyProperties);
		}

		public static void SetBody(ref BodyProperties bodyProperties, int build, int weight)
		{
			FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false, false);
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, false, ref bodyProperties);
		}

		public static void SetPigmentation(ref BodyProperties bodyProperties, int skinColor, int hairColor, int eyeColor)
		{
			FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false, false);
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, false, ref bodyProperties);
		}

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
			int faceGenInstancesLength = MBBodyProperties.GetFaceGenInstancesLength(race, faceGenerationParams.CurrentGender, (int)faceGenerationParams.CurrentAge);
			for (int j = 0; j < faceGenInstancesLength; j++)
			{
				DeformKeyData deformKeyData = MBBodyProperties.GetDeformKeyData(j, race, faceGenerationParams.CurrentGender, (int)faceGenerationParams.CurrentAge);
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
			faceGenerationParams2.CurrentAge = faceGenerationParams.CurrentAge + (float)MBRandom.RandomInt(18, 25);
			float num2;
			faceGenerationParams2.SetRandomParamsExceptKeys(race, 0, (int)faceGenerationParams2.CurrentAge, out num2);
			faceGenerationParams2.CurrentFaceTattoo = 0;
			faceGenerationParams3.CurrentAge = faceGenerationParams.CurrentAge + (float)MBRandom.RandomInt(18, 22);
			float num3;
			faceGenerationParams3.SetRandomParamsExceptKeys(race, 1, (int)faceGenerationParams3.CurrentAge, out num3);
			faceGenerationParams3.CurrentFaceTattoo = 0;
			faceGenerationParams3.HeightMultiplier = faceGenerationParams2.HeightMultiplier * MBRandom.RandomFloatRanged(0.7f, 0.9f);
			if (faceGenerationParams3.CurrentHair == 0)
			{
				faceGenerationParams3.CurrentHair = 1;
			}
			float num4 = MBRandom.RandomFloat * MathF.Min(faceGenerationParams.CurrentSkinColorOffset, 1f - faceGenerationParams.CurrentSkinColorOffset);
			if (MBRandom.RandomInt(2) == 1)
			{
				faceGenerationParams2.CurrentSkinColorOffset = faceGenerationParams.CurrentSkinColorOffset + num4;
				faceGenerationParams3.CurrentSkinColorOffset = faceGenerationParams.CurrentSkinColorOffset - num4;
			}
			else
			{
				faceGenerationParams2.CurrentSkinColorOffset = faceGenerationParams.CurrentSkinColorOffset - num4;
				faceGenerationParams3.CurrentSkinColorOffset = faceGenerationParams.CurrentSkinColorOffset + num4;
			}
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams3, false, false, ref motherBodyProperties);
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams2, false, false, ref fatherBodyProperties);
		}

		public static BodyProperties GetBodyPropertiesWithAge(ref BodyProperties bodyProperties, float age)
		{
			FaceGenerationParams faceGenerationParams = default(FaceGenerationParams);
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, false, false);
			faceGenerationParams.CurrentAge = age;
			BodyProperties bodyProperties2 = default(BodyProperties);
			MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, false, false, ref bodyProperties2);
			return bodyProperties2;
		}

		public enum GenerationType
		{
			FromMother,
			FromFather,
			Count
		}
	}
}
