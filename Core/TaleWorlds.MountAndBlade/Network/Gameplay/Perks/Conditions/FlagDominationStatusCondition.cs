using System;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003D9 RID: 985
	public class FlagDominationStatusCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
	{
		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x06003426 RID: 13350 RVA: 0x000D81A2 File Offset: 0x000D63A2
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.FlagCapture | MPPerkCondition.PerkEventFlags.FlagRemoval;
			}
		}

		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x06003427 RID: 13351 RVA: 0x000D81A5 File Offset: 0x000D63A5
		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003428 RID: 13352 RVA: 0x000D81A8 File Offset: 0x000D63A8
		protected FlagDominationStatusCondition()
		{
		}

		// Token: 0x06003429 RID: 13353 RVA: 0x000D81B0 File Offset: 0x000D63B0
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
				Debug.FailedAssert("provided 'status' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\FlagDominationStatusCondition.cs", "Deserialize", 39);
			}
		}

		// Token: 0x0600342A RID: 13354 RVA: 0x000D821D File Offset: 0x000D641D
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x0600342B RID: 13355 RVA: 0x000D8234 File Offset: 0x000D6434
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

		// Token: 0x04001633 RID: 5683
		protected static string StringType = "FlagDominationStatus";

		// Token: 0x04001634 RID: 5684
		private FlagDominationStatusCondition.Status _status;

		// Token: 0x020006C9 RID: 1737
		private enum Status
		{
			// Token: 0x040022B6 RID: 8886
			Winning,
			// Token: 0x040022B7 RID: 8887
			Losing,
			// Token: 0x040022B8 RID: 8888
			Tie
		}
	}
}
