﻿using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x0200004C RID: 76
	public class MountAgentLogic : MissionLogic
	{
		// Token: 0x060003A6 RID: 934 RVA: 0x0001AEC0 File Offset: 0x000190C0
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.IsMainAgent && agent.HasMount)
			{
				this._mainHeroMountAgent = agent.MountAgent;
			}
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x0001AEE0 File Offset: 0x000190E0
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (affectedAgent.IsMount && agentState == 4 && this._mainHeroMountAgent == affectedAgent)
			{
				Equipment equipment = Hero.MainHero.BattleEquipment;
				if (Mission.Current.DoesMissionRequireCivilianEquipment)
				{
					equipment = Hero.MainHero.CivilianEquipment;
				}
				float randomFloat = MBRandom.RandomFloat;
				float num = 0.05f;
				float num2 = 0.2f;
				if (Hero.MainHero.GetPerkValue(DefaultPerks.Riding.WellStraped))
				{
					float primaryBonus = DefaultPerks.Riding.WellStraped.PrimaryBonus;
					num += num * primaryBonus;
					num2 += num * primaryBonus;
				}
				bool flag = randomFloat < num2;
				bool flag2;
				if (randomFloat >= num)
				{
					ItemModifier itemModifier = equipment[10].ItemModifier;
					flag2 = ((itemModifier != null) ? itemModifier.StringId : null) == "lame_horse";
				}
				else
				{
					flag2 = true;
				}
				if (flag2)
				{
					equipment[10] = EquipmentElement.Invalid;
					EquipmentElement equipmentElement = equipment[11];
					equipment[11] = EquipmentElement.Invalid;
					if (!equipmentElement.IsInvalid() && !equipmentElement.IsEmpty)
					{
						MobileParty.MainParty.ItemRoster.AddToCounts(equipmentElement, 1);
					}
					MBInformationManager.AddQuickInformation(new TextObject("{=nZhPS83J}You lost your horse.", null), 0, null, "");
					return;
				}
				if (flag)
				{
					ItemModifier @object = MBObjectManager.Instance.GetObject<ItemModifier>("lame_horse");
					equipment[10] = new EquipmentElement(equipment[10].Item, @object, null, false);
					TextObject textObject = new TextObject("{=a6hwSEAK}Your horse is turned into a {MODIFIED_NAME}, since it got seriously injured.", null);
					textObject.SetTextVariable("MODIFIED_NAME", equipment[10].GetModifiedItemName());
					MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				}
			}
		}

		// Token: 0x040001BD RID: 445
		private Agent _mainHeroMountAgent;
	}
}
