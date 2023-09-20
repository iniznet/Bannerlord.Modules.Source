using System;
using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000204 RID: 516
	public class FaceGen : IFaceGen
	{
		// Token: 0x06001C8A RID: 7306 RVA: 0x00065938 File Offset: 0x00063B38
		private FaceGen()
		{
			this._raceNamesDictionary = new Dictionary<string, int>();
			this._raceNamesArray = MBAPI.IMBFaceGen.GetRaceIds().Split(new char[] { ';' });
			for (int i = 0; i < this._raceNamesArray.Length; i++)
			{
				this._raceNamesDictionary[this._raceNamesArray[i]] = i;
			}
			this._monstersDictionary = new Dictionary<string, Monster>();
			this._monstersArray = new Monster[this._raceNamesArray.Length];
		}

		// Token: 0x06001C8B RID: 7307 RVA: 0x000659BB File Offset: 0x00063BBB
		public static void CreateInstance()
		{
			FaceGen.SetInstance(new FaceGen());
		}

		// Token: 0x06001C8C RID: 7308 RVA: 0x000659C8 File Offset: 0x00063BC8
		public Monster GetMonster(string monsterID)
		{
			Monster @object;
			if (!this._monstersDictionary.TryGetValue(monsterID, out @object))
			{
				@object = Game.Current.ObjectManager.GetObject<Monster>(monsterID);
				this._monstersDictionary[monsterID] = @object;
			}
			return @object;
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x00065A04 File Offset: 0x00063C04
		public Monster GetMonsterWithSuffix(int race, string suffix)
		{
			return this.GetMonster(this._raceNamesArray[race] + suffix);
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x00065A1C File Offset: 0x00063C1C
		public Monster GetBaseMonsterFromRace(int race)
		{
			Monster monster = this._monstersArray[race];
			if (monster == null)
			{
				monster = Game.Current.ObjectManager.GetObject<Monster>(this._raceNamesArray[race]);
				this._monstersArray[race] = monster;
			}
			return monster;
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x00065A58 File Offset: 0x00063C58
		public BodyProperties GetRandomBodyProperties(int race, bool isFemale, BodyProperties bodyPropertiesMin, BodyProperties bodyPropertiesMax, int hairCoverType, int seed, string hairTags, string beardTags, string tattooTags)
		{
			return MBBodyProperties.GetRandomBodyProperties(race, isFemale, bodyPropertiesMin, bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tattooTags);
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x00065A79 File Offset: 0x00063C79
		void IFaceGen.GenerateParentBody(BodyProperties childBodyProperties, int race, ref BodyProperties motherBodyProperties, ref BodyProperties fatherBodyProperties)
		{
			MBBodyProperties.GenerateParentKey(childBodyProperties, race, ref motherBodyProperties, ref fatherBodyProperties);
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x00065A85 File Offset: 0x00063C85
		void IFaceGen.SetHair(ref BodyProperties bodyProperties, int hair, int beard, int tattoo)
		{
			MBBodyProperties.SetHair(ref bodyProperties, hair, beard, tattoo);
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x00065A91 File Offset: 0x00063C91
		void IFaceGen.SetBody(ref BodyProperties bodyProperties, int build, int weight)
		{
			MBBodyProperties.SetBody(ref bodyProperties, build, weight);
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x00065A9B File Offset: 0x00063C9B
		void IFaceGen.SetPigmentation(ref BodyProperties bodyProperties, int skinColor, int hairColor, int eyeColor)
		{
			MBBodyProperties.SetPigmentation(ref bodyProperties, skinColor, hairColor, eyeColor);
		}

		// Token: 0x06001C94 RID: 7316 RVA: 0x00065AA7 File Offset: 0x00063CA7
		public BodyProperties GetBodyPropertiesWithAge(ref BodyProperties bodyProperties, float age)
		{
			return MBBodyProperties.GetBodyPropertiesWithAge(ref bodyProperties, age);
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x00065AB0 File Offset: 0x00063CB0
		public void GetParamsFromBody(ref FaceGenerationParams faceGenerationParams, BodyProperties bodyProperties, bool earsAreHidden, bool mouthIsHidden)
		{
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, earsAreHidden, mouthIsHidden);
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x00065ABC File Offset: 0x00063CBC
		public BodyMeshMaturityType GetMaturityTypeWithAge(float age)
		{
			return MBBodyProperties.GetMaturityType(age);
		}

		// Token: 0x06001C97 RID: 7319 RVA: 0x00065AC4 File Offset: 0x00063CC4
		public int GetRaceCount()
		{
			return this._raceNamesArray.Length;
		}

		// Token: 0x06001C98 RID: 7320 RVA: 0x00065ACE File Offset: 0x00063CCE
		public int GetRaceOrDefault(string raceId)
		{
			return this._raceNamesDictionary[raceId];
		}

		// Token: 0x06001C99 RID: 7321 RVA: 0x00065ADC File Offset: 0x00063CDC
		public string GetBaseMonsterNameFromRace(int race)
		{
			return this._raceNamesArray[race];
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x00065AE6 File Offset: 0x00063CE6
		public string[] GetRaceNames()
		{
			return (string[])this._raceNamesArray.Clone();
		}

		// Token: 0x04000947 RID: 2375
		private readonly Dictionary<string, int> _raceNamesDictionary;

		// Token: 0x04000948 RID: 2376
		private readonly string[] _raceNamesArray;

		// Token: 0x04000949 RID: 2377
		private readonly Dictionary<string, Monster> _monstersDictionary;

		// Token: 0x0400094A RID: 2378
		private readonly Monster[] _monstersArray;
	}
}
