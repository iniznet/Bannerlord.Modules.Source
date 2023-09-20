﻿using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
	public class WeaponSpawner : ScriptComponentBehavior
	{
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
