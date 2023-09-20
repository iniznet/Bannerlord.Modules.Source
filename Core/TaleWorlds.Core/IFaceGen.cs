using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200007F RID: 127
	public interface IFaceGen
	{
		// Token: 0x06000797 RID: 1943
		BodyProperties GetRandomBodyProperties(int race, bool isFemale, BodyProperties bodyPropertiesMin, BodyProperties bodyPropertiesMax, int hairCoverType, int seed, string hairTags, string beardTags, string tatooTags);

		// Token: 0x06000798 RID: 1944
		void GenerateParentBody(BodyProperties childBodyProperties, int race, ref BodyProperties motherBodyProperties, ref BodyProperties fatherBodyProperties);

		// Token: 0x06000799 RID: 1945
		void SetBody(ref BodyProperties bodyProperties, int build, int weight);

		// Token: 0x0600079A RID: 1946
		void SetHair(ref BodyProperties bodyProperties, int hair, int beard, int tattoo);

		// Token: 0x0600079B RID: 1947
		void SetPigmentation(ref BodyProperties bodyProperties, int skinColor, int hairColor, int eyeColor);

		// Token: 0x0600079C RID: 1948
		BodyProperties GetBodyPropertiesWithAge(ref BodyProperties bodyProperties, float age);

		// Token: 0x0600079D RID: 1949
		BodyMeshMaturityType GetMaturityTypeWithAge(float age);

		// Token: 0x0600079E RID: 1950
		int GetRaceCount();

		// Token: 0x0600079F RID: 1951
		int GetRaceOrDefault(string raceId);

		// Token: 0x060007A0 RID: 1952
		string GetBaseMonsterNameFromRace(int race);

		// Token: 0x060007A1 RID: 1953
		string[] GetRaceNames();

		// Token: 0x060007A2 RID: 1954
		Monster GetMonster(string monsterID);

		// Token: 0x060007A3 RID: 1955
		Monster GetMonsterWithSuffix(int race, string suffix);

		// Token: 0x060007A4 RID: 1956
		Monster GetBaseMonsterFromRace(int race);
	}
}
