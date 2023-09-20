using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Test
{
	// Token: 0x02000061 RID: 97
	public class TestMissionController : MissionLogic
	{
		// Token: 0x06000432 RID: 1074 RVA: 0x0001EEF7 File Offset: 0x0001D0F7
		public TestMissionController()
		{
			this._campaign = Campaign.Current;
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x0001EF0C File Offset: 0x0001D10C
		public override void AfterStart()
		{
			base.Mission.IsInventoryAccessible = true;
			base.Mission.IsQuestScreenAccessible = true;
			base.AfterStart();
			if (!this._campaign.IsInitializedSinglePlayerReferences)
			{
				this._campaign.InitializeSinglePlayerReferences();
			}
			if (!Extensions.IsEmpty<Team>(base.Mission.Teams))
			{
				throw new MBIllegalValueException("Number of teams is not 0.");
			}
			base.Mission.IsInventoryAccessible = true;
			base.Mission.Teams.Add(0, uint.MaxValue, uint.MaxValue, null, true, false, true);
			base.Mission.Teams.Add(1, 4284776512U, uint.MaxValue, null, true, false, true);
			base.Mission.PlayerTeam = base.Mission.AttackerTeam;
			MatrixFrame matrixFrame;
			matrixFrame..ctor(Mat3.Identity, new Vec3(10f, 10f, 1f, -1f));
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("spawnpoint_player");
			if (gameEntity != null)
			{
				matrixFrame = gameEntity.GetGlobalFrame();
				matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			}
			else
			{
				gameEntity = base.Mission.Scene.FindEntityWithTag("spawnpoint");
				if (gameEntity != null)
				{
					matrixFrame = gameEntity.GetGlobalFrame();
					matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				}
			}
			CharacterObject characterObject = CharacterObject.PlayerCharacter ?? Game.Current.ObjectManager.GetObject<CharacterObject>("main_hero");
			Mission mission = base.Mission;
			AgentBuildData agentBuildData = new AgentBuildData(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.AttackerTeam).InitialPosition(ref matrixFrame.origin);
			Vec2 asVec = matrixFrame.rotation.f.AsVec2;
			mission.SpawnAgent(agentBuildData.InitialDirection(ref asVec).Controller(2), false).WieldInitialWeapons(2);
			GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, 20000, true);
			this.SpawnSheeps();
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x0001F0E8 File Offset: 0x0001D2E8
		private void SpawnSheeps()
		{
			foreach (GameEntity gameEntity in base.Mission.Scene.FindEntitiesWithTag("sp_sheep"))
			{
				MatrixFrame matrixFrame = ((gameEntity != null) ? gameEntity.GetFrame() : MatrixFrame.Identity);
				ItemRosterElement itemRosterElement;
				itemRosterElement..ctor(Game.Current.ObjectManager.GetObject<ItemObject>("sheep"), 0, null);
				Mission mission = base.Mission;
				ItemRosterElement itemRosterElement2 = itemRosterElement;
				ItemRosterElement itemRosterElement3 = default(ItemRosterElement);
				Vec2 vec = matrixFrame.rotation.f.AsVec2;
				vec = vec.Normalized();
				mission.SpawnMonster(itemRosterElement2, itemRosterElement3, ref matrixFrame.origin, ref vec, -1);
			}
		}

		// Token: 0x040001FB RID: 507
		private Campaign _campaign;
	}
}
