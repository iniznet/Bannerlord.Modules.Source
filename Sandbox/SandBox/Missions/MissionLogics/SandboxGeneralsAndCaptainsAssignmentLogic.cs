using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class SandboxGeneralsAndCaptainsAssignmentLogic : GeneralsAndCaptainsAssignmentLogic
	{
		public SandboxGeneralsAndCaptainsAssignmentLogic(TextObject attackerGeneralName, TextObject defenderGeneralName, TextObject attackerAllyGeneralName = null, TextObject defenderAllyGeneralName = null, bool createBodyguard = true)
			: base(attackerGeneralName, defenderGeneralName, attackerAllyGeneralName, defenderAllyGeneralName, createBodyguard)
		{
		}

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
