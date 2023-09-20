using System;

namespace TaleWorlds.Core
{
	public static class FaceGen
	{
		public static void SetInstance(IFaceGen faceGen)
		{
			FaceGen._instance = faceGen;
		}

		public static BodyProperties GetRandomBodyProperties(int race, bool isFemale, BodyProperties bodyPropertiesMin, BodyProperties bodyPropertiesMax, int hairCoverType, int seed, string hairTags, string beardTags, string tatooTags)
		{
			if (FaceGen._instance != null)
			{
				return FaceGen._instance.GetRandomBodyProperties(race, isFemale, bodyPropertiesMin, bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tatooTags);
			}
			return bodyPropertiesMin;
		}

		public static int GetRaceCount()
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return 0;
			}
			return instance.GetRaceCount();
		}

		public static int GetRaceOrDefault(string raceId)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return 0;
			}
			return instance.GetRaceOrDefault(raceId);
		}

		public static string GetBaseMonsterNameFromRace(int race)
		{
			IFaceGen instance = FaceGen._instance;
			return ((instance != null) ? instance.GetBaseMonsterNameFromRace(race) : null) ?? null;
		}

		public static string[] GetRaceNames()
		{
			IFaceGen instance = FaceGen._instance;
			return ((instance != null) ? instance.GetRaceNames() : null) ?? null;
		}

		public static Monster GetMonster(string monsterID)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return null;
			}
			return instance.GetMonster(monsterID);
		}

		public static Monster GetMonsterWithSuffix(int race, string suffix)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return null;
			}
			return instance.GetMonsterWithSuffix(race, suffix);
		}

		public static Monster GetBaseMonsterFromRace(int race)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return null;
			}
			return instance.GetBaseMonsterFromRace(race);
		}

		public static void GenerateParentKey(BodyProperties childBodyProperties, int race, ref BodyProperties motherBodyProperties, ref BodyProperties fatherBodyProperties)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return;
			}
			instance.GenerateParentBody(childBodyProperties, race, ref motherBodyProperties, ref fatherBodyProperties);
		}

		public static void SetHair(ref BodyProperties bodyProperties, int hair, int beard, int tattoo)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return;
			}
			instance.SetHair(ref bodyProperties, hair, beard, tattoo);
		}

		public static void SetBody(ref BodyProperties bodyProperties, int build, int weight)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return;
			}
			instance.SetBody(ref bodyProperties, build, weight);
		}

		public static void SetPigmentation(ref BodyProperties bodyProperties, int skinColor, int hairColor, int eyeColor)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return;
			}
			instance.SetPigmentation(ref bodyProperties, skinColor, hairColor, eyeColor);
		}

		public static BodyProperties GetBodyPropertiesWithAge(ref BodyProperties originalBodyProperties, float age)
		{
			if (FaceGen._instance != null)
			{
				return FaceGen._instance.GetBodyPropertiesWithAge(ref originalBodyProperties, age);
			}
			return originalBodyProperties;
		}

		public static BodyMeshMaturityType GetMaturityTypeWithAge(float age)
		{
			if (FaceGen._instance != null)
			{
				return FaceGen._instance.GetMaturityTypeWithAge(age);
			}
			return BodyMeshMaturityType.Child;
		}

		public const string MonsterSuffixSettlement = "_settlement";

		public const string MonsterSuffixSettlementSlow = "_settlement_slow";

		public const string MonsterSuffixSettlementFast = "_settlement_fast";

		public const string MonsterSuffixChild = "_child";

		public static bool ShowDebugValues;

		public static bool UpdateDeformKeys;

		private static IFaceGen _instance;
	}
}
