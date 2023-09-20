using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	// Token: 0x020003F0 RID: 1008
	public class CaravanBattleMissionHandler : MissionLogic
	{
		// Token: 0x060034C5 RID: 13509 RVA: 0x000DB2D8 File Offset: 0x000D94D8
		public CaravanBattleMissionHandler(int unitCount, bool isCamelCulture, bool isCaravan)
		{
			this._unitCount = unitCount;
			this._isCamelCulture = isCamelCulture;
			this._isCaravan = isCaravan;
		}

		// Token: 0x060034C6 RID: 13510 RVA: 0x000DB378 File Offset: 0x000D9578
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

		// Token: 0x04001697 RID: 5783
		private GameEntity _entity;

		// Token: 0x04001698 RID: 5784
		private int _unitCount;

		// Token: 0x04001699 RID: 5785
		private bool _isCamelCulture;

		// Token: 0x0400169A RID: 5786
		private bool _isCaravan;

		// Token: 0x0400169B RID: 5787
		private readonly string[] _camelLoadHarnesses = new string[] { "camel_saddle_a", "camel_saddle_b" };

		// Token: 0x0400169C RID: 5788
		private readonly string[] _camelMountableHarnesses = new string[] { "camel_saddle" };

		// Token: 0x0400169D RID: 5789
		private readonly string[] _muleLoadHarnesses = new string[] { "mule_load_a", "mule_load_b", "mule_load_c" };

		// Token: 0x0400169E RID: 5790
		private readonly string[] _muleMountableHarnesses = new string[] { "aseran_village_harness", "steppe_fur_harness", "steppe_harness" };

		// Token: 0x0400169F RID: 5791
		private const string CaravanPrefabName = "caravan_scattered_goods_prop";

		// Token: 0x040016A0 RID: 5792
		private const string VillagerGoodsPrefabName = "villager_scattered_goods_prop";
	}
}
