using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000058 RID: 88
	public static class FaceGen
	{
		// Token: 0x06000648 RID: 1608 RVA: 0x00016F48 File Offset: 0x00015148
		public static void SetInstance(IFaceGen faceGen)
		{
			FaceGen._instance = faceGen;
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00016F50 File Offset: 0x00015150
		public static BodyProperties GetRandomBodyProperties(int race, bool isFemale, BodyProperties bodyPropertiesMin, BodyProperties bodyPropertiesMax, int hairCoverType, int seed, string hairTags, string beardTags, string tatooTags)
		{
			if (FaceGen._instance != null)
			{
				return FaceGen._instance.GetRandomBodyProperties(race, isFemale, bodyPropertiesMin, bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tatooTags);
			}
			return bodyPropertiesMin;
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00016F7E File Offset: 0x0001517E
		public static int GetRaceCount()
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return 0;
			}
			return instance.GetRaceCount();
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00016F90 File Offset: 0x00015190
		public static int GetRaceOrDefault(string raceId)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return 0;
			}
			return instance.GetRaceOrDefault(raceId);
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00016FA3 File Offset: 0x000151A3
		public static string GetBaseMonsterNameFromRace(int race)
		{
			IFaceGen instance = FaceGen._instance;
			return ((instance != null) ? instance.GetBaseMonsterNameFromRace(race) : null) ?? null;
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00016FBC File Offset: 0x000151BC
		public static string[] GetRaceNames()
		{
			IFaceGen instance = FaceGen._instance;
			return ((instance != null) ? instance.GetRaceNames() : null) ?? null;
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00016FD4 File Offset: 0x000151D4
		public static Monster GetMonster(string monsterID)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return null;
			}
			return instance.GetMonster(monsterID);
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00016FE7 File Offset: 0x000151E7
		public static Monster GetMonsterWithSuffix(int race, string suffix)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return null;
			}
			return instance.GetMonsterWithSuffix(race, suffix);
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00016FFB File Offset: 0x000151FB
		public static Monster GetBaseMonsterFromRace(int race)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return null;
			}
			return instance.GetBaseMonsterFromRace(race);
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x0001700E File Offset: 0x0001520E
		public static void GenerateParentKey(BodyProperties childBodyProperties, int race, ref BodyProperties motherBodyProperties, ref BodyProperties fatherBodyProperties)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return;
			}
			instance.GenerateParentBody(childBodyProperties, race, ref motherBodyProperties, ref fatherBodyProperties);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00017023 File Offset: 0x00015223
		public static void SetHair(ref BodyProperties bodyProperties, int hair, int beard, int tattoo)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return;
			}
			instance.SetHair(ref bodyProperties, hair, beard, tattoo);
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00017038 File Offset: 0x00015238
		public static void SetBody(ref BodyProperties bodyProperties, int build, int weight)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return;
			}
			instance.SetBody(ref bodyProperties, build, weight);
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x0001704C File Offset: 0x0001524C
		public static void SetPigmentation(ref BodyProperties bodyProperties, int skinColor, int hairColor, int eyeColor)
		{
			IFaceGen instance = FaceGen._instance;
			if (instance == null)
			{
				return;
			}
			instance.SetPigmentation(ref bodyProperties, skinColor, hairColor, eyeColor);
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00017061 File Offset: 0x00015261
		public static BodyProperties GetBodyPropertiesWithAge(ref BodyProperties originalBodyProperties, float age)
		{
			if (FaceGen._instance != null)
			{
				return FaceGen._instance.GetBodyPropertiesWithAge(ref originalBodyProperties, age);
			}
			return originalBodyProperties;
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x0001707D File Offset: 0x0001527D
		public static BodyMeshMaturityType GetMaturityTypeWithAge(float age)
		{
			if (FaceGen._instance != null)
			{
				return FaceGen._instance.GetMaturityTypeWithAge(age);
			}
			return BodyMeshMaturityType.Child;
		}

		// Token: 0x04000332 RID: 818
		public const string MonsterSuffixSettlement = "_settlement";

		// Token: 0x04000333 RID: 819
		public const string MonsterSuffixSettlementSlow = "_settlement_slow";

		// Token: 0x04000334 RID: 820
		public const string MonsterSuffixSettlementFast = "_settlement_fast";

		// Token: 0x04000335 RID: 821
		public const string MonsterSuffixChild = "_child";

		// Token: 0x04000336 RID: 822
		public static bool ShowDebugValues;

		// Token: 0x04000337 RID: 823
		public static bool UpdateDeformKeys;

		// Token: 0x04000338 RID: 824
		private static IFaceGen _instance;
	}
}
