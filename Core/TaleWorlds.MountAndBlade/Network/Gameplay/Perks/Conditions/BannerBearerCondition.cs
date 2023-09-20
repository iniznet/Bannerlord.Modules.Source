using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003D6 RID: 982
	public class BannerBearerCondition : MPPerkCondition
	{
		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x06003412 RID: 13330 RVA: 0x000D7E24 File Offset: 0x000D6024
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.AliveBotCountChange | MPPerkCondition.PerkEventFlags.BannerPickUp | MPPerkCondition.PerkEventFlags.BannerDrop | MPPerkCondition.PerkEventFlags.SpawnEnd;
			}
		}

		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x06003413 RID: 13331 RVA: 0x000D7E2B File Offset: 0x000D602B
		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003414 RID: 13332 RVA: 0x000D7E2E File Offset: 0x000D602E
		protected BannerBearerCondition()
		{
		}

		// Token: 0x06003415 RID: 13333 RVA: 0x000D7E36 File Offset: 0x000D6036
		protected override void Deserialize(XmlNode node)
		{
		}

		// Token: 0x06003416 RID: 13334 RVA: 0x000D7E38 File Offset: 0x000D6038
		public override bool Check(MissionPeer peer)
		{
			Formation formation = ((peer != null) ? peer.ControlledFormation : null);
			if (formation != null && MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
			{
				using (List<IFormationUnit>.Enumerator enumerator = formation.Arrangement.GetAllUnits().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Agent agent;
						if ((agent = enumerator.Current as Agent) != null && agent.IsActive())
						{
							MissionWeapon missionWeapon = agent.Equipment[EquipmentIndex.ExtraWeaponSlot];
							if (!missionWeapon.IsEmpty && missionWeapon.Item.ItemType == ItemObject.ItemTypeEnum.Banner && new Banner(formation.BannerCode, peer.Team.Color, peer.Team.Color2).Serialize() == missionWeapon.Banner.Serialize())
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06003417 RID: 13335 RVA: 0x000D7F28 File Offset: 0x000D6128
		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			MissionPeer missionPeer = ((agent != null) ? agent.MissionPeer : null) ?? ((agent != null) ? agent.OwningAgentMissionPeer : null);
			return this.Check(missionPeer);
		}

		// Token: 0x0400162E RID: 5678
		protected static string StringType = "BannerBearer";
	}
}
