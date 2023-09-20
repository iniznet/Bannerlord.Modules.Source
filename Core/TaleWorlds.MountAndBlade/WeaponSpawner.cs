using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000394 RID: 916
	public class WeaponSpawner : ScriptComponentBehavior
	{
		// Token: 0x06003251 RID: 12881 RVA: 0x000D0F18 File Offset: 0x000CF118
		public void SpawnWeapon()
		{
			base.OnPreInit();
			base.GameEntity.RemoveAllChildren();
			MissionWeapon missionWeapon = new MissionWeapon(MBObjectManager.Instance.GetObject<ItemObject>(base.GameEntity.Name), null, null);
			GameEntity gameEntity = Mission.Current.SpawnWeaponWithNewEntity(ref missionWeapon, Mission.WeaponSpawnFlags.WithPhysics, base.GameEntity.GetGlobalFrame());
			List<string> list = new List<string>();
			foreach (string text in base.GameEntity.Tags)
			{
				gameEntity.AddTag(text);
				list.Add(text);
			}
			for (int j = 0; j < list.Count; j++)
			{
				base.GameEntity.RemoveTag(list[j]);
			}
		}
	}
}
