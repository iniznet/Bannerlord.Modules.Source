using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class MBExtensions
	{
		private static Vec2 GetGlobalOrganicDirectionAux(ColumnFormation columnFormation, int depthCount = -1)
		{
			IEnumerable<Agent> unitsAtVanguardFile = columnFormation.GetUnitsAtVanguardFile<Agent>();
			Vec2 vec = Vec2.Zero;
			int num = 0;
			Agent agent = null;
			foreach (Agent agent2 in unitsAtVanguardFile)
			{
				if (agent != null)
				{
					Vec2 vec2 = (agent.Position - agent2.Position).AsVec2.Normalized();
					vec += vec2;
					num++;
				}
				agent = agent2;
				if (depthCount > 0 && num >= depthCount)
				{
					break;
				}
			}
			if (num == 0)
			{
				return Vec2.Invalid;
			}
			return vec * (1f / (float)num);
		}

		public static Vec2 GetGlobalOrganicDirection(this ColumnFormation columnFormation)
		{
			return MBExtensions.GetGlobalOrganicDirectionAux(columnFormation, -1);
		}

		public static Vec2 GetGlobalHeadDirection(this ColumnFormation columnFormation)
		{
			return MBExtensions.GetGlobalOrganicDirectionAux(columnFormation, 3);
		}

		public static IEnumerable<T> FindAllWithType<T>(this IEnumerable<GameEntity> entities) where T : ScriptComponentBehavior
		{
			return entities.SelectMany((GameEntity e) => e.GetScriptComponents<T>());
		}

		public static IEnumerable<T> FindAllWithType<T>(this IEnumerable<MissionObject> missionObjects) where T : MissionObject
		{
			return from e in missionObjects
				where e != null && e is T
				select e as T;
		}

		public static List<GameEntity> FindAllWithCompatibleType(this IEnumerable<GameEntity> sceneProps, params Type[] types)
		{
			List<GameEntity> list = new List<GameEntity>();
			foreach (GameEntity gameEntity in sceneProps)
			{
				foreach (ScriptComponentBehavior scriptComponentBehavior in gameEntity.GetScriptComponents())
				{
					Type type = scriptComponentBehavior.GetType();
					for (int i = 0; i < types.Length; i++)
					{
						if (types[i].IsAssignableFrom(type))
						{
							list.Add(gameEntity);
						}
					}
				}
			}
			return list;
		}

		public static List<MissionObject> FindAllWithCompatibleType(this IEnumerable<MissionObject> missionObjects, params Type[] types)
		{
			List<MissionObject> list = new List<MissionObject>();
			foreach (MissionObject missionObject in missionObjects)
			{
				if (missionObject != null)
				{
					Type type = missionObject.GetType();
					for (int i = 0; i < types.Length; i++)
					{
						if (types[i].IsAssignableFrom(type))
						{
							list.Add(missionObject);
						}
					}
				}
			}
			return list;
		}

		private static void CollectObjectsAux<T>(GameEntity entity, MBList<T> list) where T : ScriptComponentBehavior
		{
			IEnumerable<T> scriptComponents = entity.GetScriptComponents<T>();
			list.AddRange(scriptComponents);
			foreach (GameEntity gameEntity in entity.GetChildren())
			{
				MBExtensions.CollectObjectsAux<T>(gameEntity, list);
			}
		}

		public static MBList<T> CollectObjects<T>(this GameEntity entity) where T : ScriptComponentBehavior
		{
			MBList<T> mblist = new MBList<T>();
			MBExtensions.CollectObjectsAux<T>(entity, mblist);
			return mblist;
		}

		public static List<T> CollectObjectsWithTag<T>(this GameEntity entity, string tag) where T : ScriptComponentBehavior
		{
			List<T> list = new List<T>();
			foreach (GameEntity gameEntity in entity.GetChildren())
			{
				if (gameEntity.HasTag(tag))
				{
					IEnumerable<T> scriptComponents = gameEntity.GetScriptComponents<T>();
					list.AddRange(scriptComponents);
				}
				if (gameEntity.ChildCount > 0)
				{
					list.AddRange(gameEntity.CollectObjectsWithTag(tag));
				}
			}
			return list;
		}

		public static List<GameEntity> CollectChildrenEntitiesWithTag(this GameEntity entity, string tag)
		{
			List<GameEntity> list = new List<GameEntity>();
			foreach (GameEntity gameEntity in entity.GetChildren())
			{
				if (gameEntity.HasTag(tag))
				{
					list.Add(gameEntity);
				}
				if (gameEntity.ChildCount > 0)
				{
					list.AddRange(gameEntity.CollectChildrenEntitiesWithTag(tag));
				}
			}
			return list;
		}

		public static GameEntity GetFirstChildEntityWithTag(this GameEntity entity, string tag)
		{
			foreach (GameEntity gameEntity in entity.GetChildren())
			{
				if (gameEntity.HasTag(tag))
				{
					return gameEntity;
				}
			}
			return null;
		}

		public static T GetFirstScriptInFamilyDescending<T>(this GameEntity entity) where T : ScriptComponentBehavior
		{
			T t = entity.GetFirstScriptOfType<T>();
			if (t != null)
			{
				return t;
			}
			foreach (GameEntity gameEntity in entity.GetChildren())
			{
				t = gameEntity.GetFirstScriptInFamilyDescending<T>();
				if (t != null)
				{
					return t;
				}
			}
			return default(T);
		}

		public static bool HasParentOfType(this GameEntity e, Type t)
		{
			for (;;)
			{
				e = e.Parent;
				if (e.GetScriptComponents().Any((ScriptComponentBehavior sc) => sc.GetType() == t))
				{
					break;
				}
				if (!(e != null))
				{
					return false;
				}
			}
			return true;
		}

		public static TSource ElementAtOrValue<TSource>(this IEnumerable<TSource> source, int index, TSource value)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (index >= 0)
			{
				IList<TSource> list = source as IList<TSource>;
				if (list == null)
				{
					using (IEnumerator<TSource> enumerator = source.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (index == 0)
							{
								return enumerator.Current;
							}
							index--;
						}
					}
					return value;
				}
				if (index < list.Count)
				{
					return list[index];
				}
			}
			return value;
		}

		public static bool IsOpponentOf(this BattleSideEnum s, BattleSideEnum side)
		{
			return (s == BattleSideEnum.Attacker && side == BattleSideEnum.Defender) || (s == BattleSideEnum.Defender && side == BattleSideEnum.Attacker);
		}
	}
}
