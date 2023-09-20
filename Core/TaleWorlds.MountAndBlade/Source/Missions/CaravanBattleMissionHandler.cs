using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	public class CaravanBattleMissionHandler : MissionLogic
	{
		public CaravanBattleMissionHandler(int unitCount, bool isCamelCulture, bool isCaravan)
		{
			this._unitCount = unitCount;
			this._isCamelCulture = isCamelCulture;
			this._isCaravan = isCaravan;
		}

		public override void AfterStart()
		{
			base.AfterStart();
			float battleSizeOffset = Mission.GetBattleSizeOffset((int)((float)this._unitCount * 1.5f), base.Mission.GetInitialSpawnPath());
			WorldFrame battleSideInitialSpawnPathFrame = base.Mission.GetBattleSideInitialSpawnPathFrame(BattleSideEnum.Defender, battleSizeOffset);
			this._entity = GameEntity.Instantiate(Mission.Current.Scene, this._isCaravan ? "caravan_scattered_goods_prop" : "villager_scattered_goods_prop", new MatrixFrame(battleSideInitialSpawnPathFrame.Rotation, battleSideInitialSpawnPathFrame.Origin.GetGroundVec3()));
			this._entity.SetMobility(GameEntity.Mobility.dynamic);
			foreach (GameEntity gameEntity in this._entity.GetChildren())
			{
				float num;
				Vec3 vec;
				Mission.Current.Scene.GetTerrainHeightAndNormal(gameEntity.GlobalPosition.AsVec2, out num, out vec);
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				globalFrame.origin.z = num;
				globalFrame.rotation.u = vec;
				globalFrame.rotation.Orthonormalize();
				gameEntity.SetGlobalFrame(globalFrame);
			}
			IEnumerable<GameEntity> enumerable = from c in this._entity.GetChildren()
				where c.HasTag("caravan_animal_spawn")
				select c;
			int num2 = (int)((float)enumerable.Count<GameEntity>() * 0.4f);
			foreach (GameEntity gameEntity2 in enumerable)
			{
				MatrixFrame globalFrame2 = gameEntity2.GetGlobalFrame();
				string text;
				if (this._isCamelCulture)
				{
					if (num2 > 0)
					{
						int num3 = MBRandom.RandomInt(this._camelMountableHarnesses.Length);
						text = this._camelMountableHarnesses[num3];
					}
					else
					{
						int num4 = MBRandom.RandomInt(this._camelLoadHarnesses.Length);
						text = this._camelLoadHarnesses[num4];
					}
				}
				else if (num2 > 0)
				{
					int num5 = MBRandom.RandomInt(this._muleMountableHarnesses.Length);
					text = this._muleMountableHarnesses[num5];
				}
				else
				{
					int num6 = MBRandom.RandomInt(this._muleLoadHarnesses.Length);
					text = this._muleLoadHarnesses[num6];
				}
				ItemRosterElement itemRosterElement = new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>(text), 0, null);
				ItemRosterElement itemRosterElement2 = (this._isCamelCulture ? ((num2-- > 0) ? new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>("pack_camel"), 0, null) : new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>("pack_camel_unmountable"), 0, null)) : ((num2-- > 0) ? new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>("mule"), 0, null) : new ItemRosterElement(Game.Current.ObjectManager.GetObject<ItemObject>("mule_unmountable"), 0, null)));
				Mission mission = Mission.Current;
				ItemRosterElement itemRosterElement3 = itemRosterElement2;
				ItemRosterElement itemRosterElement4 = itemRosterElement;
				Vec2 vec2 = globalFrame2.rotation.f.AsVec2;
				vec2 = vec2.Normalized();
				Agent agent = mission.SpawnMonster(itemRosterElement3, itemRosterElement4, globalFrame2.origin, vec2, -1);
				agent.SetAgentFlags(agent.GetAgentFlags() & ~AgentFlag.CanWander);
			}
			TacticalPosition firstScriptInFamilyDescending = this._entity.GetFirstScriptInFamilyDescending<TacticalPosition>();
			if (firstScriptInFamilyDescending != null)
			{
				foreach (Team team in Mission.Current.Teams)
				{
					team.TeamAI.TacticalPositions.Add(firstScriptInFamilyDescending);
				}
			}
		}

		private GameEntity _entity;

		private int _unitCount;

		private bool _isCamelCulture;

		private bool _isCaravan;

		private readonly string[] _camelLoadHarnesses = new string[] { "camel_saddle_a", "camel_saddle_b" };

		private readonly string[] _camelMountableHarnesses = new string[] { "camel_saddle" };

		private readonly string[] _muleLoadHarnesses = new string[] { "mule_load_a", "mule_load_b", "mule_load_c" };

		private readonly string[] _muleMountableHarnesses = new string[] { "aseran_village_harness", "steppe_fur_harness", "steppe_harness" };

		private const string CaravanPrefabName = "caravan_scattered_goods_prop";

		private const string VillagerGoodsPrefabName = "villager_scattered_goods_prop";
	}
}
