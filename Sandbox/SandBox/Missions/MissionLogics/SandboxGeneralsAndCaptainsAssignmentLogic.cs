using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x0200004F RID: 79
	public class SandboxGeneralsAndCaptainsAssignmentLogic : GeneralsAndCaptainsAssignmentLogic
	{
		// Token: 0x060003AE RID: 942 RVA: 0x0001B251 File Offset: 0x00019451
		public SandboxGeneralsAndCaptainsAssignmentLogic(TextObject attackerGeneralName, TextObject defenderGeneralName, TextObject attackerAllyGeneralName = null, TextObject defenderAllyGeneralName = null, bool createBodyguard = true)
			: base(attackerGeneralName, defenderGeneralName, attackerAllyGeneralName, defenderAllyGeneralName, createBodyguard)
		{
		}

		// Token: 0x060003AF RID: 943 RVA: 0x0001B260 File Offset: 0x00019460
		protected override void SortCaptainsByPriority(Team team, ref List<Agent> captains)
		{
			EncounterModel encounterModel = Campaign.Current.Models.EncounterModel;
			if (encounterModel != null)
			{
				captains = captains.OrderByDescending(delegate(Agent captain)
				{
					if (captain != team.GeneralAgent)
					{
						CharacterObject characterObject;
						return (float)(((characterObject = captain.Character as CharacterObject) != null && characterObject.HeroObject != null) ? encounterModel.GetCharacterSergeantScore(characterObject.HeroObject) : 0);
					}
					return float.MaxValue;
				}).ToList<Agent>();
				return;
			}
			base.SortCaptainsByPriority(team, ref captains);
		}
	}
}
