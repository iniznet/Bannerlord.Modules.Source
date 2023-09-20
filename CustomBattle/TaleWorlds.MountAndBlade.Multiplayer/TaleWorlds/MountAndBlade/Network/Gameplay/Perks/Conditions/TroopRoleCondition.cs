using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	public class TroopRoleCondition : MPPerkCondition
	{
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return 32;
			}
		}

		protected TroopRoleCondition()
		{
		}

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
				Debug.FailedAssert("provided 'role' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Conditions\\TroopRoleCondition.cs", "Deserialize", 35);
			}
		}

		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			if (agent != null && MultiplayerOptionsExtensions.GetIntValue(20, 0) > 0)
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

		private bool IsAgentSergeant(Agent agent)
		{
			return agent.Character == MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character).HeroCharacter;
		}

		private bool IsAgentBannerBearer(Agent agent)
		{
			MissionPeer missionPeer = ((agent != null) ? agent.MissionPeer : null) ?? ((agent != null) ? agent.OwningAgentMissionPeer : null);
			Formation formation = ((missionPeer != null) ? missionPeer.ControlledFormation : null);
			if (formation != null)
			{
				MissionWeapon missionWeapon = agent.Equipment[4];
				if (!missionWeapon.IsEmpty && missionWeapon.Item.ItemType == 24 && new Banner(formation.BannerCode, missionPeer.Team.Color, missionPeer.Team.Color2).Serialize() == missionWeapon.Banner.Serialize())
				{
					return true;
				}
			}
			return false;
		}

		protected static string StringType = "TroopRole";

		private TroopRoleCondition.Role _role;

		private enum Role
		{
			Sergeant,
			Troop,
			BannerBearer
		}
	}
}
