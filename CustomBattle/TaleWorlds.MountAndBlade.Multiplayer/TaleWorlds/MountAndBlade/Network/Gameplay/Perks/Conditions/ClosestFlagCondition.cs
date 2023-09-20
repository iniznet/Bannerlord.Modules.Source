using System;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	public class ClosestFlagCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
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

		protected ClosestFlagCondition()
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
			this._owner = ClosestFlagCondition.FlagOwner.Any;
			if (text2 != null && !Enum.TryParse<ClosestFlagCondition.FlagOwner>(text2, true, out this._owner))
			{
				this._owner = ClosestFlagCondition.FlagOwner.Any;
				Debug.FailedAssert("provided 'owner' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.Multiplayer\\Perks\\Conditions\\ClosestFlagCondition.cs", "Deserialize", 40);
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
				ClosestFlagCondition.FlagOwner flagOwner = ClosestFlagCondition.FlagOwner.None;
				float num = float.MaxValue;
				foreach (FlagCapturePoint flagCapturePoint in gameModeInstance.AllCapturePoints)
				{
					if (!flagCapturePoint.IsDeactivated)
					{
						float num2 = agent.Position.DistanceSquared(flagCapturePoint.Position);
						if (num2 < num)
						{
							num = num2;
							Team flagOwnerTeam = gameModeInstance.GetFlagOwnerTeam(flagCapturePoint);
							if (flagOwnerTeam == null)
							{
								flagOwner = ClosestFlagCondition.FlagOwner.None;
							}
							else if (flagOwnerTeam == agent.Team)
							{
								flagOwner = ClosestFlagCondition.FlagOwner.Ally;
							}
							else
							{
								flagOwner = ClosestFlagCondition.FlagOwner.Enemy;
							}
						}
					}
				}
				return this._owner == ClosestFlagCondition.FlagOwner.Any || this._owner == flagOwner;
			}
			return false;
		}

		protected static string StringType = "FlagDominationClosestFlag";

		private ClosestFlagCondition.FlagOwner _owner;

		private enum FlagOwner
		{
			Ally,
			Enemy,
			None,
			Any
		}
	}
}
