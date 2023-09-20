using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	public class MoraleCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
	{
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return 1;
			}
		}

		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		protected MoraleCondition()
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
					XmlAttribute xmlAttribute = attributes["min"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			if (text2 == null)
			{
				this._min = -1f;
			}
			else if (!float.TryParse(text2, out this._min))
			{
				Debug.FailedAssert("provided 'min' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Conditions\\MoraleCondition.cs", "Deserialize", 35);
			}
			string text3;
			if (node == null)
			{
				text3 = null;
			}
			else
			{
				XmlAttributeCollection attributes2 = node.Attributes;
				if (attributes2 == null)
				{
					text3 = null;
				}
				else
				{
					XmlAttribute xmlAttribute2 = attributes2["max"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			if (text4 == null)
			{
				this._max = 1f;
				return;
			}
			if (!float.TryParse(text4, out this._max))
			{
				Debug.FailedAssert("provided 'max' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Conditions\\MoraleCondition.cs", "Deserialize", 45);
			}
		}

		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			Team team = ((agent != null) ? agent.Team : null);
			if (team != null)
			{
				MissionMultiplayerFlagDomination gameModeInstance = base.GameModeInstance;
				float num = ((team.Side == 1) ? gameModeInstance.MoraleRounded : (-gameModeInstance.MoraleRounded));
				return num >= this._min && num <= this._max;
			}
			return false;
		}

		protected static string StringType = "FlagDominationMorale";

		private float _min;

		private float _max;
	}
}
