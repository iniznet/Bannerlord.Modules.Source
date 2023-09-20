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
	// Token: 0x02000041 RID: 65
	public class LocationItemSpawnHandler : MissionLogic
	{
		// Token: 0x06000317 RID: 791 RVA: 0x0001476C File Offset: 0x0001296C
		public override void AfterStart()
		{
			if (CampaignMission.Current.Location != null && CampaignMission.Current.Location.SpecialItems.Count != 0)
			{
				this.SpawnSpecialItems();
			}
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00014798 File Offset: 0x00012998
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

		// Token: 0x06000319 RID: 793 RVA: 0x0001485C File Offset: 0x00012A5C
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

		// Token: 0x04000198 RID: 408
		private Dictionary<ItemObject, GameEntity> _spawnedEntities;
	}
}
