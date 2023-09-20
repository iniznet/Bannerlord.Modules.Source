using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class LocationItemSpawnHandler : MissionLogic
	{
		public override void AfterStart()
		{
			if (CampaignMission.Current.Location != null && CampaignMission.Current.Location.SpecialItems.Count != 0)
			{
				this.SpawnSpecialItems();
			}
		}

		private void SpawnSpecialItems()
		{
			this._spawnedEntities = new Dictionary<ItemObject, GameEntity>();
			List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("sp_special_item").ToList<GameEntity>();
			foreach (ItemObject itemObject in CampaignMission.Current.Location.SpecialItems)
			{
				if (list.Count != 0)
				{
					MatrixFrame globalFrame = list[0].GetGlobalFrame();
					MissionWeapon missionWeapon;
					missionWeapon..ctor(itemObject, null, null);
					GameEntity gameEntity = base.Mission.SpawnWeaponWithNewEntity(ref missionWeapon, 16, globalFrame);
					this._spawnedEntities.Add(itemObject, gameEntity);
					list.RemoveAt(0);
				}
			}
		}

		public override void OnEntityRemoved(GameEntity entity)
		{
			if (this._spawnedEntities != null)
			{
				foreach (KeyValuePair<ItemObject, GameEntity> keyValuePair in this._spawnedEntities)
				{
					if (keyValuePair.Value == entity)
					{
						CampaignMission.Current.Location.SpecialItems.Remove(keyValuePair.Key);
					}
				}
			}
		}

		private Dictionary<ItemObject, GameEntity> _spawnedEntities;
	}
}
