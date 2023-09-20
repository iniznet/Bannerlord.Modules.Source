using System;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003DC RID: 988
	public class LastRemainingFlagCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
	{
		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x06003439 RID: 13369 RVA: 0x000D855A File Offset: 0x000D675A
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.FlagCapture | MPPerkCondition.PerkEventFlags.FlagRemoval;
			}
		}

		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x0600343A RID: 13370 RVA: 0x000D855D File Offset: 0x000D675D
		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600343B RID: 13371 RVA: 0x000D8560 File Offset: 0x000D6760
		protected LastRemainingFlagCondition()
		{
		}

		// Token: 0x0600343C RID: 13372 RVA: 0x000D8568 File Offset: 0x000D6768
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
				Debug.FailedAssert("provided 'owner' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\LastRemainingFlagCondition.cs", "Deserialize", 40);
			}
		}

		// Token: 0x0600343D RID: 13373 RVA: 0x000D85D5 File Offset: 0x000D67D5
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x0600343E RID: 13374 RVA: 0x000D85EC File Offset: 0x000D67EC
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

		// Token: 0x0400163A RID: 5690
		protected static string StringType = "FlagDominationLastRemainingFlag";

		// Token: 0x0400163B RID: 5691
		private LastRemainingFlagCondition.FlagOwner _owner;

		// Token: 0x020006CA RID: 1738
		private enum FlagOwner
		{
			// Token: 0x040022BA RID: 8890
			Ally,
			// Token: 0x040022BB RID: 8891
			Enemy,
			// Token: 0x040022BC RID: 8892
			None,
			// Token: 0x040022BD RID: 8893
			Any
		}
	}
}
