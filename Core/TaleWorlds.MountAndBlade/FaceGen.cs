using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class FaceGen : IFaceGen
	{
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

		public static void CreateInstance()
		{
			FaceGen.SetInstance(new FaceGen());
		}

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

		public Monster GetMonsterWithSuffix(int race, string suffix)
		{
			return this.GetMonster(this._raceNamesArray[race] + suffix);
		}

		public Monster GetBaseMonsterFromRace(int race)
		{
			if (race >= 0 && race < this._monstersArray.Length)
			{
				Monster monster = this._monstersArray[race];
				if (monster == null)
				{
					monster = Game.Current.ObjectManager.GetObject<Monster>(this._raceNamesArray[race]);
					this._monstersArray[race] = monster;
				}
				return monster;
			}
			Debug.FailedAssert("Monster race index is out of bounds: " + race, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\FaceGen.cs", "GetBaseMonsterFromRace", 64);
			return null;
		}

		public BodyProperties GetRandomBodyProperties(int race, bool isFemale, BodyProperties bodyPropertiesMin, BodyProperties bodyPropertiesMax, int hairCoverType, int seed, string hairTags, string beardTags, string tattooTags)
		{
			return MBBodyProperties.GetRandomBodyProperties(race, isFemale, bodyPropertiesMin, bodyPropertiesMax, hairCoverType, seed, hairTags, beardTags, tattooTags);
		}

		void IFaceGen.GenerateParentBody(BodyProperties childBodyProperties, int race, ref BodyProperties motherBodyProperties, ref BodyProperties fatherBodyProperties)
		{
			MBBodyProperties.GenerateParentKey(childBodyProperties, race, ref motherBodyProperties, ref fatherBodyProperties);
		}

		void IFaceGen.SetHair(ref BodyProperties bodyProperties, int hair, int beard, int tattoo)
		{
			MBBodyProperties.SetHair(ref bodyProperties, hair, beard, tattoo);
		}

		void IFaceGen.SetBody(ref BodyProperties bodyProperties, int build, int weight)
		{
			MBBodyProperties.SetBody(ref bodyProperties, build, weight);
		}

		void IFaceGen.SetPigmentation(ref BodyProperties bodyProperties, int skinColor, int hairColor, int eyeColor)
		{
			MBBodyProperties.SetPigmentation(ref bodyProperties, skinColor, hairColor, eyeColor);
		}

		public BodyProperties GetBodyPropertiesWithAge(ref BodyProperties bodyProperties, float age)
		{
			return MBBodyProperties.GetBodyPropertiesWithAge(ref bodyProperties, age);
		}

		public void GetParamsFromBody(ref FaceGenerationParams faceGenerationParams, BodyProperties bodyProperties, bool earsAreHidden, bool mouthIsHidden)
		{
			MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, bodyProperties, earsAreHidden, mouthIsHidden);
		}

		public BodyMeshMaturityType GetMaturityTypeWithAge(float age)
		{
			return MBBodyProperties.GetMaturityType(age);
		}

		public int GetRaceCount()
		{
			return this._raceNamesArray.Length;
		}

		public int GetRaceOrDefault(string raceId)
		{
			return this._raceNamesDictionary[raceId];
		}

		public string GetBaseMonsterNameFromRace(int race)
		{
			return this._raceNamesArray[race];
		}

		public string[] GetRaceNames()
		{
			return (string[])this._raceNamesArray.Clone();
		}

		private readonly Dictionary<string, int> _raceNamesDictionary;

		private readonly string[] _raceNamesArray;

		private readonly Dictionary<string, Monster> _monstersDictionary;

		private readonly Monster[] _monstersArray;
	}
}
