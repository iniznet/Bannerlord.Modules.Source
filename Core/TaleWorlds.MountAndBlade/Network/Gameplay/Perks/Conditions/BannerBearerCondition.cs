using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	public class BannerBearerCondition : MPPerkCondition
	{
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.AliveBotCountChange | MPPerkCondition.PerkEventFlags.BannerPickUp | MPPerkCondition.PerkEventFlags.BannerDrop | MPPerkCondition.PerkEventFlags.SpawnEnd;
			}
		}

		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		protected BannerBearerCondition()
		{
		}

		protected override void Deserialize(XmlNode node)
		{
		}

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

		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			MissionPeer missionPeer = ((agent != null) ? agent.MissionPeer : null) ?? ((agent != null) ? agent.OwningAgentMissionPeer : null);
			return this.Check(missionPeer);
		}

		protected static string StringType = "BannerBearer";
	}
}
