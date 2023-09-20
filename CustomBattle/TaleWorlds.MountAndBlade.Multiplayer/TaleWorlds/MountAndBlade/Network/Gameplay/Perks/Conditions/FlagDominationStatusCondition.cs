using System;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	public class FlagDominationStatusCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
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

		protected FlagDominationStatusCondition()
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
					XmlAttribute xmlAttribute = attributes["status"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			this._status = FlagDominationStatusCondition.Status.Tie;
			if (text2 != null && !Enum.TryParse<FlagDominationStatusCondition.Status>(text2, true, out this._status))
			{
				this._status = FlagDominationStatusCondition.Status.Tie;
				Debug.FailedAssert("provided 'status' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Conditions\\FlagDominationStatusCondition.cs", "Deserialize", 39);
			}
		}

		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			if (agent == null)
			{
				return false;
			}
			MissionMultiplayerFlagDomination gameModeInstance = base.GameModeInstance;
			int num = 0;
			int num2 = 0;
			foreach (FlagCapturePoint flagCapturePoint in gameModeInstance.AllCapturePoints)
			{
				if (!flagCapturePoint.IsDeactivated)
				{
					Team flagOwnerTeam = gameModeInstance.GetFlagOwnerTeam(flagCapturePoint);
					if (flagOwnerTeam == agent.Team)
					{
						num++;
					}
					else if (flagOwnerTeam != null)
					{
						num2++;
					}
				}
			}
			if (this._status == FlagDominationStatusCondition.Status.Winning)
			{
				return num > num2;
			}
			if (this._status != FlagDominationStatusCondition.Status.Losing)
			{
				return num == num2;
			}
			return num2 > num;
		}

		protected static string StringType = "FlagDominationStatus";

		private FlagDominationStatusCondition.Status _status;

		private enum Status
		{
			Winning,
			Losing,
			Tie
		}
	}
}
