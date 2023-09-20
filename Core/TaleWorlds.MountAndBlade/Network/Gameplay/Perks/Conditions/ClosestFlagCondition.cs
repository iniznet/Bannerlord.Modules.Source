using System;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003D7 RID: 983
	public class ClosestFlagCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
	{
		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x06003419 RID: 13337 RVA: 0x000D7F7B File Offset: 0x000D617B
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.FlagCapture | MPPerkCondition.PerkEventFlags.FlagRemoval;
			}
		}

		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x0600341A RID: 13338 RVA: 0x000D7F7E File Offset: 0x000D617E
		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600341B RID: 13339 RVA: 0x000D7F81 File Offset: 0x000D6181
		protected ClosestFlagCondition()
		{
		}

		// Token: 0x0600341C RID: 13340 RVA: 0x000D7F8C File Offset: 0x000D618C
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
				Debug.FailedAssert("provided 'owner' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\ClosestFlagCondition.cs", "Deserialize", 40);
			}
		}

		// Token: 0x0600341D RID: 13341 RVA: 0x000D7FF9 File Offset: 0x000D61F9
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x0600341E RID: 13342 RVA: 0x000D8010 File Offset: 0x000D6210
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

		// Token: 0x0400162F RID: 5679
		protected static string StringType = "FlagDominationClosestFlag";

		// Token: 0x04001630 RID: 5680
		private ClosestFlagCondition.FlagOwner _owner;

		// Token: 0x020006C8 RID: 1736
		private enum FlagOwner
		{
			// Token: 0x040022B1 RID: 8881
			Ally,
			// Token: 0x040022B2 RID: 8882
			Enemy,
			// Token: 0x040022B3 RID: 8883
			None,
			// Token: 0x040022B4 RID: 8884
			Any
		}
	}
}
