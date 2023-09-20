using System;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003DF RID: 991
	public class OwnedFlagCountCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
	{
		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x0600344D RID: 13389 RVA: 0x000D89C9 File Offset: 0x000D6BC9
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.FlagCapture | MPPerkCondition.PerkEventFlags.FlagRemoval;
			}
		}

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x0600344E RID: 13390 RVA: 0x000D89CC File Offset: 0x000D6BCC
		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600344F RID: 13391 RVA: 0x000D89CF File Offset: 0x000D6BCF
		protected OwnedFlagCountCondition()
		{
		}

		// Token: 0x06003450 RID: 13392 RVA: 0x000D89D8 File Offset: 0x000D6BD8
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
				this._min = 0;
			}
			else if (!int.TryParse(text2, out this._min))
			{
				Debug.FailedAssert("provided 'min' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\OwnedFlagCountCondition.cs", "Deserialize", 35);
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
				this._max = int.MaxValue;
				return;
			}
			if (!int.TryParse(text4, out this._max))
			{
				Debug.FailedAssert("provided 'max' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\OwnedFlagCountCondition.cs", "Deserialize", 45);
			}
		}

		// Token: 0x06003451 RID: 13393 RVA: 0x000D8A9C File Offset: 0x000D6C9C
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x06003452 RID: 13394 RVA: 0x000D8AB0 File Offset: 0x000D6CB0
		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			if (agent != null)
			{
				MissionMultiplayerFlagDomination gameModeInstance = base.GameModeInstance;
				int num = 0;
				foreach (FlagCapturePoint flagCapturePoint in gameModeInstance.AllCapturePoints)
				{
					if (!flagCapturePoint.IsDeactivated && gameModeInstance.GetFlagOwnerTeam(flagCapturePoint) == agent.Team)
					{
						num++;
					}
				}
				return num >= this._min && num <= this._max;
			}
			return false;
		}

		// Token: 0x04001643 RID: 5699
		protected static string StringType = "FlagDominationOwnedFlagCount";

		// Token: 0x04001644 RID: 5700
		private int _min;

		// Token: 0x04001645 RID: 5701
		private int _max;
	}
}
