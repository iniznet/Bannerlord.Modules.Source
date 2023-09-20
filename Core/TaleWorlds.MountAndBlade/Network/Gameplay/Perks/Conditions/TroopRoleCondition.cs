using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003E1 RID: 993
	public class TroopRoleCondition : MPPerkCondition
	{
		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x0600345B RID: 13403 RVA: 0x000D8D5E File Offset: 0x000D6F5E
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.PeerControlledAgentChange;
			}
		}

		// Token: 0x0600345C RID: 13404 RVA: 0x000D8D62 File Offset: 0x000D6F62
		protected TroopRoleCondition()
		{
		}

		// Token: 0x0600345D RID: 13405 RVA: 0x000D8D6C File Offset: 0x000D6F6C
		protected override void Deserialize(XmlNode node)
		{
			string text;
			if (node == null)
			{
				text = null;
			}
			else
			{
				XmlAttributeCollection attributes = node.Attributes;
				if (attributes == null)
				{
					text = null;
				}
				else
				{
					XmlAttribute xmlAttribute = attributes["role"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			this._role = TroopRoleCondition.Role.Sergeant;
			if (text2 != null && !Enum.TryParse<TroopRoleCondition.Role>(text2, true, out this._role))
			{
				this._role = TroopRoleCondition.Role.Sergeant;
				Debug.FailedAssert("provided 'role' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\TroopRoleCondition.cs", "Deserialize", 35);
			}
		}

		// Token: 0x0600345E RID: 13406 RVA: 0x000D8DD9 File Offset: 0x000D6FD9
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x0600345F RID: 13407 RVA: 0x000D8DF0 File Offset: 0x000D6FF0
		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			if (agent != null && MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) > 0)
			{
				switch (this._role)
				{
				case TroopRoleCondition.Role.Sergeant:
					return this.IsAgentSergeant(agent);
				case TroopRoleCondition.Role.Troop:
					return !this.IsAgentBannerBearer(agent) && !this.IsAgentSergeant(agent);
				case TroopRoleCondition.Role.BannerBearer:
					return this.IsAgentBannerBearer(agent) && !this.IsAgentSergeant(agent);
				}
			}
			return false;
		}

		// Token: 0x06003460 RID: 13408 RVA: 0x000D8E71 File Offset: 0x000D7071
		private bool IsAgentSergeant(Agent agent)
		{
			return agent.Character == MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character).HeroCharacter;
		}

		// Token: 0x06003461 RID: 13409 RVA: 0x000D8E8C File Offset: 0x000D708C
		private bool IsAgentBannerBearer(Agent agent)
		{
			MissionPeer missionPeer = ((agent != null) ? agent.MissionPeer : null) ?? ((agent != null) ? agent.OwningAgentMissionPeer : null);
			Formation formation = ((missionPeer != null) ? missionPeer.ControlledFormation : null);
			if (formation != null)
			{
				MissionWeapon missionWeapon = agent.Equipment[EquipmentIndex.ExtraWeaponSlot];
				if (!missionWeapon.IsEmpty && missionWeapon.Item.ItemType == ItemObject.ItemTypeEnum.Banner && new Banner(formation.BannerCode, missionPeer.Team.Color, missionPeer.Team.Color2).Serialize() == missionWeapon.Banner.Serialize())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0400164A RID: 5706
		protected static string StringType = "TroopRole";

		// Token: 0x0400164B RID: 5707
		private TroopRoleCondition.Role _role;

		// Token: 0x020006CB RID: 1739
		private enum Role
		{
			// Token: 0x040022BF RID: 8895
			Sergeant,
			// Token: 0x040022C0 RID: 8896
			Troop,
			// Token: 0x040022C1 RID: 8897
			BannerBearer
		}
	}
}
