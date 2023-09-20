using System;
using System.Xml;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
	// Token: 0x020003C2 RID: 962
	public class GoldRecoveryEffect : MPPerkEffect
	{
		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x060033B7 RID: 13239 RVA: 0x000D66BE File Offset: 0x000D48BE
		public override bool IsTickRequired
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060033B8 RID: 13240 RVA: 0x000D66C1 File Offset: 0x000D48C1
		protected GoldRecoveryEffect()
		{
		}

		// Token: 0x060033B9 RID: 13241 RVA: 0x000D66CC File Offset: 0x000D48CC
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
					XmlAttribute xmlAttribute = attributes["is_disabled_in_warmup"];
					text = ((xmlAttribute != null) ? xmlAttribute.Value : null);
				}
			}
			string text2 = text;
			base.IsDisabledInWarmup = ((text2 != null) ? text2.ToLower() : null) == "true";
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
					XmlAttribute xmlAttribute2 = attributes2["value"];
					text3 = ((xmlAttribute2 != null) ? xmlAttribute2.Value : null);
				}
			}
			string text4 = text3;
			if (text4 == null || !int.TryParse(text4, out this._value))
			{
				Debug.FailedAssert("provided 'value' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\GoldRecoveryEffect.cs", "Deserialize", 29);
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
					XmlAttribute xmlAttribute3 = attributes3["period"];
					text5 = ((xmlAttribute3 != null) ? xmlAttribute3.Value : null);
				}
			}
			string text6 = text5;
			if (text6 == null || !int.TryParse(text6, out this._period) || this._period < 1)
			{
				Debug.FailedAssert("provided 'period' is invalid", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Network\\Gameplay\\Perks\\Effects\\GoldRecoveryEffect.cs", "Deserialize", 35);
			}
		}

		// Token: 0x060033BA RID: 13242 RVA: 0x000D67CC File Offset: 0x000D49CC
		public override void OnTick(Agent agent, int tickCount)
		{
			agent = ((agent != null && agent.IsMount) ? agent.RiderAgent : agent);
			MissionPeer missionPeer = ((agent != null) ? agent.MissionPeer : null) ?? ((agent != null) ? agent.OwningAgentMissionPeer : null);
			if (tickCount % this._period == 0 && missionPeer != null)
			{
				Mission mission = Mission.Current;
				MissionMultiplayerGameModeBase missionMultiplayerGameModeBase = ((mission != null) ? mission.GetMissionBehavior<MissionMultiplayerGameModeBase>() : null);
				if (missionMultiplayerGameModeBase == null)
				{
					return;
				}
				missionMultiplayerGameModeBase.ChangeCurrentGoldForPeer(missionPeer, missionPeer.Representative.Gold + this._value);
			}
		}

		// Token: 0x040015FE RID: 5630
		protected static string StringType = "GoldRecovery";

		// Token: 0x040015FF RID: 5631
		private int _value;

		// Token: 0x04001600 RID: 5632
		private int _period;
	}
}
