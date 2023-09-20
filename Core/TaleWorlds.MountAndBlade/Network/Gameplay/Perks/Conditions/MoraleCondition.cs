using System;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003DD RID: 989
	public class MoraleCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
	{
		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x06003440 RID: 13376 RVA: 0x000D86B8 File Offset: 0x000D68B8
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.MoraleChange;
			}
		}

		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x06003441 RID: 13377 RVA: 0x000D86BB File Offset: 0x000D68BB
		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003442 RID: 13378 RVA: 0x000D86BE File Offset: 0x000D68BE
		protected MoraleCondition()
		{
		}

		// Token: 0x06003443 RID: 13379 RVA: 0x000D86C8 File Offset: 0x000D68C8
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
				Debug.FailedAssert("provided 'min' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\MoraleCondition.cs", "Deserialize", 35);
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
				Debug.FailedAssert("provided 'max' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\MoraleCondition.cs", "Deserialize", 45);
			}
		}

		// Token: 0x06003444 RID: 13380 RVA: 0x000D8790 File Offset: 0x000D6990
		public override bool Check(MissionPeer peer)
		{
			return this.Check((peer != null) ? peer.ControlledAgent : null);
		}

		// Token: 0x06003445 RID: 13381 RVA: 0x000D87A4 File Offset: 0x000D69A4
		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			Team team = ((agent != null) ? agent.Team : null);
			if (team != null)
			{
				MissionMultiplayerFlagDomination gameModeInstance = base.GameModeInstance;
				float num = ((team.Side == BattleSideEnum.Attacker) ? gameModeInstance.MoraleRounded : (-gameModeInstance.MoraleRounded));
				return num >= this._min && num <= this._max;
			}
			return false;
		}

		// Token: 0x0400163C RID: 5692
		protected static string StringType = "FlagDominationMorale";

		// Token: 0x0400163D RID: 5693
		private float _min;

		// Token: 0x0400163E RID: 5694
		private float _max;
	}
}
