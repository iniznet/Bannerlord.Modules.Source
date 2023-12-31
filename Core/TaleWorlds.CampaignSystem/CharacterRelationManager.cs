﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem
{
	public class CharacterRelationManager
	{
		internal static void AutoGeneratedStaticCollectObjectsCharacterRelationManager(object o, List<object> collectedObjects)
		{
			((CharacterRelationManager)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			collectedObjects.Add(this._heroRelations);
		}

		internal static object AutoGeneratedGetMemberValue_heroRelations(object o)
		{
			return ((CharacterRelationManager)o)._heroRelations;
		}

		public static CharacterRelationManager Instance
		{
			get
			{
				return Campaign.Current.CharacterRelationManager;
			}
		}

		public CharacterRelationManager()
		{
			this._heroRelations = new CharacterRelationManager.HeroRelations();
		}

		public static int GetHeroRelation(Hero hero1, Hero hero2)
		{
			return CharacterRelationManager.Instance._heroRelations.GetRelation(hero1, hero2);
		}

		public static void SetHeroRelation(Hero hero1, Hero hero2, int value)
		{
			if (hero1 != hero2)
			{
				CharacterRelationManager.Instance._heroRelations.SetRelation(hero1, hero2, value);
				return;
			}
			Debug.FailedAssert("hero1 != hero2", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\CharacterRelationManager.cs", "SetHeroRelation", 262);
		}

		public void AfterLoad()
		{
			if (MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.1.0", 27066))
			{
				this._heroRelations.ClearOldData();
			}
		}

		public void RemoveHero(Hero deadHero)
		{
			this._heroRelations.Remove(deadHero);
		}

		[SaveableField(1)]
		private readonly CharacterRelationManager.HeroRelations _heroRelations;

		internal class HeroRelations
		{
			internal static void AutoGeneratedStaticCollectObjectsHeroRelations(object o, List<object> collectedObjects)
			{
				((CharacterRelationManager.HeroRelations)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this._relations);
			}

			internal static object AutoGeneratedGetMemberValue_relations(object o)
			{
				return ((CharacterRelationManager.HeroRelations)o)._relations;
			}

			public int GetRelation(Hero hero1, Hero hero2)
			{
				ValueTuple<long, long> hashCodes = this.GetHashCodes(hero1, hero2);
				Dictionary<long, int> dictionary;
				int num;
				if (this._relations.TryGetValue(hashCodes.Item1, out dictionary) && dictionary.TryGetValue(hashCodes.Item2, out num))
				{
					return num;
				}
				return 0;
			}

			public void SetRelation(Hero hero1, Hero hero2, int value)
			{
				ValueTuple<long, long> hashCodes = this.GetHashCodes(hero1, hero2);
				if (value != 0)
				{
					Dictionary<long, int> dictionary;
					if (!this._relations.TryGetValue(hashCodes.Item1, out dictionary))
					{
						dictionary = new Dictionary<long, int>();
						this._relations.Add(hashCodes.Item1, dictionary);
					}
					dictionary[hashCodes.Item2] = value;
					return;
				}
				Dictionary<long, int> dictionary2;
				if (this._relations.TryGetValue(hashCodes.Item1, out dictionary2) && dictionary2.ContainsKey(hashCodes.Item2))
				{
					dictionary2.Remove(hashCodes.Item2);
					if (!dictionary2.Any<KeyValuePair<long, int>>())
					{
						this._relations.Remove(hashCodes.Item1);
					}
				}
			}

			public void Remove(Hero hero)
			{
				int hashCode = hero.Id.GetHashCode();
				this._relations.Remove((long)hashCode);
				foreach (Dictionary<long, int> dictionary in this._relations.Values)
				{
					dictionary.Remove((long)hashCode);
				}
			}

			public void ClearOldData()
			{
				this.ClearOldData<Dictionary<long, int>>(this._relations);
				foreach (Dictionary<long, int> dictionary in this._relations.Values)
				{
					this.ClearOldData<int>(dictionary);
				}
			}

			private void ClearOldData<T>(Dictionary<long, T> obj)
			{
				HashSet<long> hashSet = new HashSet<long>(obj.Keys);
				foreach (Hero hero in Campaign.Current.CampaignObjectManager.AliveHeroes)
				{
					if (hashSet.Contains((long)hero.Id.GetHashCode()))
					{
						hashSet.Remove((long)hero.Id.GetHashCode());
					}
				}
				foreach (long num in hashSet)
				{
					obj.Remove(num);
				}
			}

			private ValueTuple<long, long> GetHashCodes(Hero hero1, Hero hero2)
			{
				if (hero1.Id > hero2.Id)
				{
					return new ValueTuple<long, long>((long)hero1.Id.GetHashCode(), (long)hero2.Id.GetHashCode());
				}
				return new ValueTuple<long, long>((long)hero2.Id.GetHashCode(), (long)hero1.Id.GetHashCode());
			}

			[SaveableField(1)]
			private Dictionary<long, Dictionary<long, int>> _relations = new Dictionary<long, Dictionary<long, int>>();
		}
	}
}
