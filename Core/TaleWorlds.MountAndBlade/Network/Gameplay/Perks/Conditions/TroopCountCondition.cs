using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
	// Token: 0x020003E0 RID: 992
	public class TroopCountCondition : MPPerkCondition
	{
		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x06003454 RID: 13396 RVA: 0x000D8B60 File Offset: 0x000D6D60
		public override MPPerkCondition.PerkEventFlags EventFlags
		{
			get
			{
				return MPPerkCondition.PerkEventFlags.AliveBotCountChange | MPPerkCondition.PerkEventFlags.SpawnEnd;
			}
		}

		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x06003455 RID: 13397 RVA: 0x000D8B67 File Offset: 0x000D6D67
		public override bool IsPeerCondition
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003456 RID: 13398 RVA: 0x000D8B6A File Offset: 0x000D6D6A
		protected TroopCountCondition()
		{
		}

		// Token: 0x06003457 RID: 13399 RVA: 0x000D8B74 File Offset: 0x000D6D74
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
					XmlAttribute xmlAttribute = attributes["is_ratio"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			this._isRatio = ((text2 != null) ? text2.ToLower() : null) == "true";
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
					XmlAttribute xmlAttribute2 = attributes2["min"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			if (text4 == null)
			{
				this._min = 0f;
			}
			else if (!float.TryParse(text4, out this._min))
			{
				Debug.FailedAssert("provided 'min' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\TroopCountCondition.cs", "Deserialize", 39);
			}
			string text5;
			if (node == null)
			{
				text5 = null;
			}
			else
			{
				XmlAttributeCollection attributes3 = node.Attributes;
				if (attributes3 == null)
				{
					text5 = null;
				}
				else
				{
					XmlAttribute xmlAttribute3 = attributes3["max"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text6 = text5;
			if (text6 == null)
			{
				this._max = (this._isRatio ? 1f : float.MaxValue);
				return;
			}
			if (!float.TryParse(text6, out this._max))
			{
				Debug.FailedAssert("provided 'max' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Conditions\\TroopCountCondition.cs", "Deserialize", 49);
			}
		}

		// Token: 0x06003458 RID: 13400 RVA: 0x000D8C94 File Offset: 0x000D6E94
		public override bool Check(MissionPeer peer)
		{
			if (peer == null || MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) <= 0 || peer.ControlledFormation == null)
			{
				return false;
			}
			int num = (peer.IsControlledAgentActive ? (peer.BotsUnderControlAlive + 1) : peer.BotsUnderControlAlive);
			if (this._isRatio)
			{
				float num2 = (float)num / (float)(peer.BotsUnderControlTotal + 1);
				return num2 >= this._min && num2 <= this._max;
			}
			return (float)num >= this._min && (float)num <= this._max;
		}

		// Token: 0x06003459 RID: 13401 RVA: 0x000D8D18 File Offset: 0x000D6F18
		public override bool Check(Agent agent)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			return this.Check(((agent != null) ? agent.MissionPeer : null) ?? ((agent != null) ? agent.OwningAgentMissionPeer : null));
		}

		// Token: 0x04001646 RID: 5702
		protected static string StringType = "TroopCount";

		// Token: 0x04001647 RID: 5703
		private bool _isRatio;

		// Token: 0x04001648 RID: 5704
		private float _min;

		// Token: 0x04001649 RID: 5705
		private float _max;
	}
}
