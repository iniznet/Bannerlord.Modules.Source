using System;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	public class LastRemainingFlagCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
	{
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return 6;
			}
		}

		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		protected LastRemainingFlagCondition()
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
					XmlAttribute xmlAttribute = attributes["owner"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			this._owner = LastRemainingFlagCondition.FlagOwner.Any;
			if (text2 != null && !Enum.TryParse<LastRemainingFlagCondition.FlagOwner>(text2, true, out this._owner))
			{
				this._owner = LastRemainingFlagCondition.FlagOwner.Any;
				Debug.FailedAssert("provided 'owner' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Conditions\\LastRemainingFlagCondition.cs", "Deserialize", 40);
			}
		}

		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			if (agent != null)
			{
				MissionMultiplayerFlagDomination gameModeInstance = base.GameModeInstance;
				LastRemainingFlagCondition.FlagOwner flagOwner = LastRemainingFlagCondition.FlagOwner.None;
				int num = 0;
				foreach (FlagCapturePoint flagCapturePoint in gameModeInstance.AllCapturePoints)
				{
					if (!flagCapturePoint.IsDeactivated)
					{
						num++;
						Team flagOwnerTeam = gameModeInstance.GetFlagOwnerTeam(flagCapturePoint);
						if (flagOwnerTeam == null)
						{
							flagOwner = LastRemainingFlagCondition.FlagOwner.None;
						}
						else if (flagOwnerTeam == agent.Team)
						{
							flagOwner = LastRemainingFlagCondition.FlagOwner.Ally;
						}
						else
						{
							flagOwner = LastRemainingFlagCondition.FlagOwner.Enemy;
						}
					}
				}
				return num == 1 && (this._owner == LastRemainingFlagCondition.FlagOwner.Any || this._owner == flagOwner);
			}
			return false;
		}

		protected static string StringType = "FlagDominationLastRemainingFlag";

		private LastRemainingFlagCondition.FlagOwner _owner;

		private enum FlagOwner
		{
			Ally,
			Enemy,
			None,
			Any
		}
	}
}
