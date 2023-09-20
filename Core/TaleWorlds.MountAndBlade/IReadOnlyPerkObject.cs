using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000319 RID: 793
	public interface IReadOnlyPerkObject
	{
		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x06002AB1 RID: 10929
		TextObject Name { get; }

		// Token: 0x17000797 RID: 1943
		// (get) Token: 0x06002AB2 RID: 10930
		TextObject Description { get; }

		// Token: 0x17000798 RID: 1944
		// (get) Token: 0x06002AB3 RID: 10931
		List<string> GameModes { get; }

		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06002AB4 RID: 10932
		int PerkListIndex { get; }

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x06002AB5 RID: 10933
		string IconId { get; }

		// Token: 0x1700079B RID: 1947
		// (get) Token: 0x06002AB6 RID: 10934
		string HeroIdleAnimOverride { get; }

		// Token: 0x1700079C RID: 1948
		// (get) Token: 0x06002AB7 RID: 10935
		string HeroMountIdleAnimOverride { get; }

		// Token: 0x1700079D RID: 1949
		// (get) Token: 0x06002AB8 RID: 10936
		string TroopIdleAnimOverride { get; }

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x06002AB9 RID: 10937
		string TroopMountIdleAnimOverride { get; }

		// Token: 0x06002ABA RID: 10938
		float GetTroopCountMultiplier(bool isWarmup);

		// Token: 0x06002ABB RID: 10939
		float GetExtraTroopCount(bool isWarmup);

		// Token: 0x06002ABC RID: 10940
		List<ValueTuple<EquipmentIndex, EquipmentElement>> GetAlternativeEquipments(bool isWarmup, bool isPlayer, List<ValueTuple<EquipmentIndex, EquipmentElement>> alternativeEquipments, bool getAllEquipments = false);

		// Token: 0x06002ABD RID: 10941
		float GetDrivenPropertyBonusOnSpawn(bool isWarmup, bool isPlayer, DrivenProperty drivenProperty, float baseValue);

		// Token: 0x06002ABE RID: 10942
		float GetHitpoints(bool isWarmup, bool isPlayer);

		// Token: 0x06002ABF RID: 10943
		MPPerkObject Clone(MissionPeer peer);
	}
}
