using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.Missions.MissionLogics;
using SandBox.Objects;
using SandBox.Objects.AnimationPoints;
using SandBox.Objects.AreaMarkers;
using SandBox.Objects.Usables;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Objects;

namespace SandBox.View.Missions.SandBox
{
	// Token: 0x02000026 RID: 38
	public class SpawnPointDebugView : ScriptComponentBehavior
	{
		// Token: 0x06000103 RID: 259 RVA: 0x0000C8EF File Offset: 0x0000AAEF
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			this.DetermineSceneType();
			this.AddSpawnPointsToList(false);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000C904 File Offset: 0x0000AB04
		protected override void OnInit()
		{
			base.OnInit();
			this.DetermineSceneType();
			this.AddSpawnPointsToList(false);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06000105 RID: 261 RVA: 0x0000C925 File Offset: 0x0000AB25
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (SpawnPointDebugView.ActivateDebugUI || (MBEditor.IsEditModeOn && this.ActivateDebugUIEditor))
			{
				return 2 | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000C94C File Offset: 0x0000AB4C
		protected override void OnTick(float dt)
		{
			this.ToolMainFunction();
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000C954 File Offset: 0x0000AB54
		protected override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.ToolMainFunction();
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000C964 File Offset: 0x0000AB64
		private void ToolMainFunction()
		{
			if (SpawnPointDebugView.ActivateDebugUI || (MBEditor.IsEditModeOn && this.ActivateDebugUIEditor))
			{
				this.StartImGUIWindow("Debug Window");
				if (Mission.Current != null)
				{
					this.ImGUITextArea("- Do Not Hide The Mouse Cursor When Debug Window Is Intersecting With The Center Of The Screen!! -", this._separatorNeeded, !this._onSameLineNeeded);
				}
				if (this.ImGUIButton("Scene Basic Information Tab", this._normalButton))
				{
					this.ChangeTab(true, false, false);
				}
				this.LeaveSpaceBetweenTabs();
				if (this.ImGUIButton("Scene Entity Check Tab", this._normalButton))
				{
					this.ChangeTab(false, true, false);
				}
				this.LeaveSpaceBetweenTabs();
				if (this.ImGUIButton("Navigation Mesh Check Tab", this._normalButton))
				{
					this.ChangeTab(false, false, true);
				}
				if (this._entityInformationTab)
				{
					this.ShowEntityInformationTab();
				}
				if (this._basicInformationTab)
				{
					this.ShowBasicInformationTab();
				}
				if (this._navigationMeshCheckTab)
				{
					this.ShowNavigationCheckTab();
				}
				if (this._relatedEntityWindow)
				{
					this.ShowRelatedEntity();
				}
				this.ImGUITextArea("If there are more than one 'SpawnPointDebugView' in the scene, please remove them.", this._separatorNeeded, !this._onSameLineNeeded);
				this.ImGUITextArea("If you have any questions about this tool feel free to ask Campaign team.", this._separatorNeeded, !this._onSameLineNeeded);
				this.EndImGUIWindow();
			}
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000CA8C File Offset: 0x0000AC8C
		private void ShowRelatedEntity()
		{
			this.StartImGUIWindow("Entity Window");
			if (this.ImGUIButton("Close Tab", this._normalButton))
			{
				this._relatedEntityWindow = false;
			}
			this.ImGUITextArea("Please expand the window!", !this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("Prefabs with '" + this._relatedPrefabTag + "' tags are listed.", this._separatorNeeded, !this._onSameLineNeeded);
			this.FindAllPrefabsWithSelectedTag();
			this.EndImGUIWindow();
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000CB14 File Offset: 0x0000AD14
		private void ShowBasicInformationTab()
		{
			this.ImGUITextArea("Tool tried to detect the scene type. If scene type is not correct or not determined", !this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("please select the scene type from toggle buttons below.", this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("Scene Type: " + this._sceneType + " ", !this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("Scene Name: " + this._sceneName + " ", !this._separatorNeeded, !this._onSameLineNeeded);
			this.HandleRadioButtons();
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000CBC0 File Offset: 0x0000ADC0
		private void HandleRadioButtons()
		{
			if (this.ImGUIButton("Town Center", this._townCenterRadioButton))
			{
				this._sceneType = SpawnPointUnits.SceneType.Center;
				this._townCenterRadioButton = false;
				this._tavernRadioButton = false;
				this._villageRadioButton = false;
				this._arenaRadioButton = false;
				this._lordshallRadioButton = false;
				this._castleRadioButton = false;
				this.AddSpawnPointsToList(true);
			}
			if (this.ImGUIButton("Tavern", this._tavernRadioButton))
			{
				this._sceneType = SpawnPointUnits.SceneType.Tavern;
				this._tavernRadioButton = false;
				this._townCenterRadioButton = false;
				this._villageRadioButton = false;
				this._arenaRadioButton = false;
				this._lordshallRadioButton = false;
				this._castleRadioButton = false;
				this.AddSpawnPointsToList(true);
			}
			if (this.ImGUIButton("Village", this._villageRadioButton))
			{
				this._sceneType = SpawnPointUnits.SceneType.VillageCenter;
				this._villageRadioButton = false;
				this._townCenterRadioButton = false;
				this._tavernRadioButton = false;
				this._arenaRadioButton = false;
				this._lordshallRadioButton = false;
				this.AddSpawnPointsToList(true);
			}
			if (this.ImGUIButton("Arena", this._arenaRadioButton))
			{
				this._sceneType = SpawnPointUnits.SceneType.Arena;
				this._arenaRadioButton = false;
				this._townCenterRadioButton = false;
				this._tavernRadioButton = false;
				this._villageRadioButton = false;
				this._lordshallRadioButton = false;
				this._castleRadioButton = false;
				this.AddSpawnPointsToList(true);
			}
			if (this.ImGUIButton("Lords Hall", this._lordshallRadioButton))
			{
				this._sceneType = SpawnPointUnits.SceneType.LordsHall;
				this._lordshallRadioButton = false;
				this._townCenterRadioButton = false;
				this._tavernRadioButton = false;
				this._villageRadioButton = false;
				this._arenaRadioButton = false;
				this._castleRadioButton = false;
				this.AddSpawnPointsToList(true);
			}
			if (this.ImGUIButton("Castle", this._castleRadioButton))
			{
				this._sceneType = SpawnPointUnits.SceneType.Castle;
				this._castleRadioButton = false;
				this._lordshallRadioButton = false;
				this._townCenterRadioButton = false;
				this._tavernRadioButton = false;
				this._villageRadioButton = false;
				this._arenaRadioButton = false;
				this.AddSpawnPointsToList(true);
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000CD88 File Offset: 0x0000AF88
		private void ChangeTab(bool basicInformationTab, bool entityInformationTab, bool navigationMeshCheckTab)
		{
			this._basicInformationTab = basicInformationTab;
			this._entityInformationTab = entityInformationTab;
			this._navigationMeshCheckTab = navigationMeshCheckTab;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000CDA0 File Offset: 0x0000AFA0
		private void DetermineSceneType()
		{
			this._sceneName = base.Scene.GetName();
			if (this._sceneName.Contains("tavern"))
			{
				this._sceneType = SpawnPointUnits.SceneType.Tavern;
				return;
			}
			if (this._sceneName.Contains("lords_hall") || (this._sceneName.Contains("interior") && (this._sceneName.Contains("lords_hall") || this._sceneName.Contains("castle") || this._sceneName.Contains("keep"))))
			{
				this._sceneType = SpawnPointUnits.SceneType.LordsHall;
				return;
			}
			if (this._sceneName.Contains("village"))
			{
				this._sceneType = SpawnPointUnits.SceneType.VillageCenter;
				return;
			}
			if (this._sceneName.Contains("town") || this._sceneName.Contains("city"))
			{
				this._sceneType = SpawnPointUnits.SceneType.Center;
				return;
			}
			if (this._sceneName.Contains("dungeon"))
			{
				this._sceneType = SpawnPointUnits.SceneType.Dungeon;
				return;
			}
			if (this._sceneName.Contains("hippodrome") || this._sceneName.Contains("arena"))
			{
				this._sceneType = SpawnPointUnits.SceneType.Arena;
				return;
			}
			if (this._sceneName.Contains("castle") || this._sceneName.Contains("siege"))
			{
				this._sceneType = SpawnPointUnits.SceneType.Castle;
				return;
			}
			if (this._sceneName.Contains("interior"))
			{
				this._sceneType = SpawnPointUnits.SceneType.EmptyShop;
				return;
			}
			this._sceneType = SpawnPointUnits.SceneType.NotDetermined;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000CF14 File Offset: 0x0000B114
		private void AddSpawnPointsToList(bool alreadyInitialized)
		{
			this._spUnitsList.Clear();
			if (this._sceneType == SpawnPointUnits.SceneType.Center)
			{
				this._spUnitsList.Add(new SpawnPointUnits("spawnpoint_player_outside", SpawnPointUnits.SceneType.Center, "npc", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("alley_1_population", SpawnPointUnits.SceneType.Center, 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("alley_2_population", SpawnPointUnits.SceneType.Center, 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("alley_3_population", SpawnPointUnits.SceneType.Center, 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("center_conversation_point", SpawnPointUnits.SceneType.Center, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation_alley_1", SpawnPointUnits.SceneType.Center, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation_alley_2", SpawnPointUnits.SceneType.Center, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation_alley_3", SpawnPointUnits.SceneType.Center, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("workshop_area_1_population", SpawnPointUnits.SceneType.Center, 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("workshop_area_2_population", SpawnPointUnits.SceneType.Center, 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("workshop_area_3_population", SpawnPointUnits.SceneType.Center, 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation_workshop_1", SpawnPointUnits.SceneType.Center, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation_workshop_2", SpawnPointUnits.SceneType.Center, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation_workshop_3", SpawnPointUnits.SceneType.Center, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("workshop_1_notable_parent", SpawnPointUnits.SceneType.Center, 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("workshop_2_notable_parent", SpawnPointUnits.SceneType.Center, 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("workshop_3_notable_parent", SpawnPointUnits.SceneType.Center, 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("navigation_mesh_deactivator", SpawnPointUnits.SceneType.Center, 0, 1));
				this._spUnitsList.Add(new SpawnPointUnits("alley_marker", SpawnPointUnits.SceneType.Center, 3, 3));
				this._spUnitsList.Add(new SpawnPointUnits("workshop_area_marker", SpawnPointUnits.SceneType.Center, 3, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_outside_near_town_main_gate", SpawnPointUnits.SceneType.Center, "npc", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("npc_dancer", SpawnPointUnits.SceneType.Center, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("spawnpoint_cleaner", SpawnPointUnits.SceneType.Center, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("npc_beggar", SpawnPointUnits.SceneType.Center, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_notable_artisan", SpawnPointUnits.SceneType.Center, "npc", 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_notable_gangleader", SpawnPointUnits.SceneType.Center, "npc", 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_notable_merchant", SpawnPointUnits.SceneType.Center, "npc", 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_notable_preacher", SpawnPointUnits.SceneType.Center, "npc", 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_merchant", SpawnPointUnits.SceneType.Center, "npc", 1, 25));
				this._spUnitsList.Add(new SpawnPointUnits("sp_armorer", SpawnPointUnits.SceneType.Center, "npc", 1, 25));
				this._spUnitsList.Add(new SpawnPointUnits("sp_blacksmith", SpawnPointUnits.SceneType.Center, "npc", 1, 25));
				this._spUnitsList.Add(new SpawnPointUnits("sp_weaponsmith", SpawnPointUnits.SceneType.Center, "npc", 1, 25));
				this._spUnitsList.Add(new SpawnPointUnits("sp_horse_merchant", SpawnPointUnits.SceneType.Center, "npc", 1, 25));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard", SpawnPointUnits.SceneType.Center, "npc", 2, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard_castle", SpawnPointUnits.SceneType.Center, "npc", 1, 2));
				this._spUnitsList.Add(new SpawnPointUnits("sp_prison_guard", SpawnPointUnits.SceneType.Center, "npc", 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard_patrol", SpawnPointUnits.SceneType.Center, "npc", 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard_unarmed", SpawnPointUnits.SceneType.Center, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_tavern_wench", SpawnPointUnits.SceneType.Center, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("spawnpoint_tavernkeeper", SpawnPointUnits.SceneType.Center, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_barber", SpawnPointUnits.SceneType.Center, "npc", 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation", SpawnPointUnits.SceneType.Center, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_tavern", SpawnPointUnits.SceneType.Center, "passage", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_arena", SpawnPointUnits.SceneType.Center, "passage", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_lordshall", SpawnPointUnits.SceneType.Center, "passage", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_prison", SpawnPointUnits.SceneType.Center, "passage", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_house_1", SpawnPointUnits.SceneType.Center, "passage", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_house_2", SpawnPointUnits.SceneType.Center, "passage", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_house_3", SpawnPointUnits.SceneType.Center, "passage", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("desert_war_horse", SpawnPointUnits.SceneType.Center, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("steppe_charger", SpawnPointUnits.SceneType.Center, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("war_horse", SpawnPointUnits.SceneType.Center, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("charger", SpawnPointUnits.SceneType.Center, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("desert_horse", SpawnPointUnits.SceneType.Center, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("hunter", SpawnPointUnits.SceneType.Center, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_sheep", SpawnPointUnits.SceneType.Center, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_cow", SpawnPointUnits.SceneType.Center, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_hog", SpawnPointUnits.SceneType.Center, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_goose", SpawnPointUnits.SceneType.Center, "animal", 0, int.MaxValue));
			}
			else if (this._sceneType == SpawnPointUnits.SceneType.Tavern)
			{
				this._spUnitsList.Add(new SpawnPointUnits("musician", SpawnPointUnits.SceneType.Tavern, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_tavern_wench", SpawnPointUnits.SceneType.Tavern, "npc", 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("spawnpoint_tavernkeeper", SpawnPointUnits.SceneType.Tavern, "npc", 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("spawnpoint_mercenary", SpawnPointUnits.SceneType.Tavern, "npc", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("gambler_npc", SpawnPointUnits.SceneType.Tavern, "npc", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_center", SpawnPointUnits.SceneType.Tavern, "passage", 1, 1));
			}
			else if (this._sceneType == SpawnPointUnits.SceneType.VillageCenter)
			{
				this._spUnitsList.Add(new SpawnPointUnits("sp_notable", SpawnPointUnits.SceneType.VillageCenter, "notable", 6, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_notable_rural_notable", SpawnPointUnits.SceneType.VillageCenter, "npc", 6, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation", SpawnPointUnits.SceneType.VillageCenter, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("alley_1_population", SpawnPointUnits.SceneType.VillageCenter, 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("alley_2_population", SpawnPointUnits.SceneType.VillageCenter, 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("alley_3_population", SpawnPointUnits.SceneType.VillageCenter, 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("center_conversation_point", SpawnPointUnits.SceneType.VillageCenter, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation_alley_1", SpawnPointUnits.SceneType.VillageCenter, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation_alley_2", SpawnPointUnits.SceneType.VillageCenter, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation_alley_3", SpawnPointUnits.SceneType.VillageCenter, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("alley_marker", SpawnPointUnits.SceneType.VillageCenter, 3, 3));
				this._spUnitsList.Add(new SpawnPointUnits("battle_set", SpawnPointUnits.SceneType.VillageCenter, 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("navigation_mesh_deactivator", SpawnPointUnits.SceneType.VillageCenter, 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("desert_war_horse", SpawnPointUnits.SceneType.VillageCenter, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("steppe_charger", SpawnPointUnits.SceneType.VillageCenter, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("war_horse", SpawnPointUnits.SceneType.VillageCenter, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("charger", SpawnPointUnits.SceneType.VillageCenter, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("desert_horse", SpawnPointUnits.SceneType.VillageCenter, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("hunter", SpawnPointUnits.SceneType.VillageCenter, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_sheep", SpawnPointUnits.SceneType.VillageCenter, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_cow", SpawnPointUnits.SceneType.VillageCenter, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_hog", SpawnPointUnits.SceneType.VillageCenter, "animal", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_goose", SpawnPointUnits.SceneType.VillageCenter, "animal", 0, int.MaxValue));
			}
			else if (this._sceneType == SpawnPointUnits.SceneType.Arena)
			{
				this._spUnitsList.Add(new SpawnPointUnits("spawnpoint_tournamentmaster", SpawnPointUnits.SceneType.Arena, "npc", 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_near_arena_master", SpawnPointUnits.SceneType.Arena, "npc", 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("tournament_archery", SpawnPointUnits.SceneType.Arena, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("tournament_fight", SpawnPointUnits.SceneType.Arena, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("tournament_jousting", SpawnPointUnits.SceneType.Arena, 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_center", SpawnPointUnits.SceneType.Arena, "passage", 1, 1));
			}
			else if (this._sceneType == SpawnPointUnits.SceneType.LordsHall)
			{
				this._spUnitsList.Add(new SpawnPointUnits("battle_set", SpawnPointUnits.SceneType.LordsHall, 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard", SpawnPointUnits.SceneType.LordsHall, "npc", 2, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_notable", SpawnPointUnits.SceneType.LordsHall, "npc", 10, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_king", SpawnPointUnits.SceneType.LordsHall, "npc", 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_throne", SpawnPointUnits.SceneType.LordsHall, "npc", 1, 2));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_center", SpawnPointUnits.SceneType.LordsHall, "passage", 1, 1));
			}
			else if (this._sceneType == SpawnPointUnits.SceneType.Castle)
			{
				this._spUnitsList.Add(new SpawnPointUnits("sp_prison_guard", SpawnPointUnits.SceneType.Castle, "npc", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard", SpawnPointUnits.SceneType.Castle, "npc", 2, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard_castle", SpawnPointUnits.SceneType.Castle, "npc", 1, 2));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard_patrol", SpawnPointUnits.SceneType.Castle, "npc", 1, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard_unarmed", SpawnPointUnits.SceneType.Castle, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_lordshall", SpawnPointUnits.SceneType.Castle, "passage", 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("sp_player_conversation", SpawnPointUnits.SceneType.Castle, 1, int.MaxValue));
			}
			else if (this._sceneType == SpawnPointUnits.SceneType.Dungeon)
			{
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard", SpawnPointUnits.SceneType.Dungeon, "npc", 2, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard_patrol", SpawnPointUnits.SceneType.Dungeon, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_guard_unarmed", SpawnPointUnits.SceneType.Dungeon, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("npc_passage_center", SpawnPointUnits.SceneType.Castle, "passage", 1, 1));
			}
			if (!alreadyInitialized)
			{
				this._spUnitsList.Add(new SpawnPointUnits("spawnpoint_player", SpawnPointUnits.SceneType.All, 1, 1));
				this._spUnitsList.Add(new SpawnPointUnits("npc_common", SpawnPointUnits.SceneType.All, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("npc_common_limited", SpawnPointUnits.SceneType.All, "npc", 0, int.MaxValue));
				this._spUnitsList.Add(new SpawnPointUnits("sp_npc", SpawnPointUnits.SceneType.All, "DONTUSE", 0, 0));
				this._spUnitsList.Add(new SpawnPointUnits("spawnpoint_elder", SpawnPointUnits.SceneType.VillageCenter, "DONTUSE", 0, 0));
			}
			this._invalidSpawnPointsDictionary.Clear();
			this._invalidSpawnPointsDictionary.Add(SpawnPointDebugView.CategoryId.NPC, new List<SpawnPointDebugView.InvalidPosition>());
			this._invalidSpawnPointsDictionary.Add(SpawnPointDebugView.CategoryId.Animal, new List<SpawnPointDebugView.InvalidPosition>());
			this._invalidSpawnPointsDictionary.Add(SpawnPointDebugView.CategoryId.Chair, new List<SpawnPointDebugView.InvalidPosition>());
			this._invalidSpawnPointsDictionary.Add(SpawnPointDebugView.CategoryId.Passage, new List<SpawnPointDebugView.InvalidPosition>());
			this._invalidSpawnPointsDictionary.Add(SpawnPointDebugView.CategoryId.SemivalidChair, new List<SpawnPointDebugView.InvalidPosition>());
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000DDC0 File Offset: 0x0000BFC0
		private List<List<string>> GetLevelCombinationsToCheck()
		{
			base.GameEntity.Scene.GetName();
			bool flag = base.GameEntity.Scene.GetUpgradeLevelMaskOfLevelName("siege") > 0U;
			List<List<string>> list = new List<List<string>>();
			if (flag)
			{
				list.Add(new List<string>());
				list[0].Add("level_1");
				list[0].Add("civilian");
				list.Add(new List<string>());
				list[1].Add("level_2");
				list[1].Add("civilian");
				list.Add(new List<string>());
				list[2].Add("level_3");
				list[2].Add("civilian");
			}
			else
			{
				list.Add(new List<string>());
				list[0].Add("base");
			}
			return list;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000DEA8 File Offset: 0x0000C0A8
		protected override bool OnCheckForProblems()
		{
			base.OnCheckForProblems();
			bool flag = false;
			if (this._sceneType == SpawnPointUnits.SceneType.NotDetermined)
			{
				flag = true;
				MBEditor.AddEntityWarning(base.GameEntity, "Scene type could not be determined");
			}
			uint upgradeLevelMask = base.GameEntity.Scene.GetUpgradeLevelMask();
			foreach (List<string> list in this.GetLevelCombinationsToCheck())
			{
				string text = "";
				for (int i = 0; i < list.Count - 1; i++)
				{
					text = text + list[i] + "|";
				}
				text += list[list.Count - 1];
				base.GameEntity.Scene.SetUpgradeLevelVisibility(list);
				this.CountEntities();
				foreach (SpawnPointUnits spawnPointUnits in this._spUnitsList)
				{
					if (spawnPointUnits.Place == SpawnPointUnits.SceneType.All || this._sceneType == spawnPointUnits.Place)
					{
						bool flag2 = spawnPointUnits.CurrentCount <= spawnPointUnits.MaxCount && spawnPointUnits.CurrentCount >= spawnPointUnits.MinCount;
						flag |= !flag2;
						if (!flag2)
						{
							string text2 = "Spawnpoint (" + spawnPointUnits.SpName + ") has some issues. ";
							if (spawnPointUnits.MaxCount < spawnPointUnits.CurrentCount)
							{
								text2 = string.Concat(new object[] { text2, "It is placed too much. Placed count(", spawnPointUnits.CurrentCount, "). Max count(", spawnPointUnits.MaxCount, "). Level: ", text });
							}
							else
							{
								text2 = string.Concat(new object[] { text2, "It is placed too less. Placed count(", spawnPointUnits.CurrentCount, "). Min count(", spawnPointUnits.MinCount, "). Level: ", text });
							}
							MBEditor.AddEntityWarning(base.GameEntity, text2);
						}
					}
				}
			}
			base.GameEntity.Scene.SetUpgradeLevelVisibility(upgradeLevelMask);
			this.CheckForNavigationMesh();
			foreach (List<SpawnPointDebugView.InvalidPosition> list2 in this._invalidSpawnPointsDictionary.Values)
			{
				foreach (SpawnPointDebugView.InvalidPosition invalidPosition in list2)
				{
					if (!invalidPosition.doNotShowWarning)
					{
						string text3;
						if (invalidPosition.isDisabledNavMesh)
						{
							text3 = string.Concat(new object[]
							{
								"Special entity with name (",
								invalidPosition.entity.Name,
								") has a navigation mesh below which is deactivated by the deactivater script. Position ",
								invalidPosition.position.x,
								" , ",
								invalidPosition.position.y,
								" , ",
								invalidPosition.position.z,
								"."
							});
						}
						else
						{
							text3 = string.Concat(new object[]
							{
								"Special entity with name (",
								invalidPosition.entity.Name,
								") has no navigation mesh below. Position ",
								invalidPosition.position.x,
								" , ",
								invalidPosition.position.y,
								" , ",
								invalidPosition.position.z,
								"."
							});
						}
						MBEditor.AddEntityWarning(invalidPosition.entity, text3);
						flag = true;
					}
				}
			}
			return flag;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000E2F8 File Offset: 0x0000C4F8
		private void ShowEntityInformationTab()
		{
			this.ImGUITextArea("This tab calculates the spawnpoint counts and warns you if", !this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("counts are not in the given criteria.", this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("Click 'Count Entities' button to calculate and toggle categories.", this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("You can use the list button to list all the prefabs with tag.", this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("Current Townsfolk count: " + this._currentTownsfolkCount, this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUICheckBox("NPCs ", ref this._showNPCsList, !this._separatorNeeded, this._onSameLineNeeded);
			this.ImGUICheckBox("Animals ", ref this._showAnimalsList, !this._separatorNeeded, this._onSameLineNeeded);
			this.ImGUICheckBox("Passages ", ref this._showPassagesList, !this._separatorNeeded, this._onSameLineNeeded);
			this.ImGUICheckBox("Others ", ref this._showOthersList, !this._separatorNeeded, this._onSameLineNeeded);
			this.ImGUICheckBox("DONT USE ", ref this._showDontUseList, this._separatorNeeded, !this._onSameLineNeeded);
			this.WriteTableHeaders();
			foreach (SpawnPointUnits spawnPointUnits in this._spUnitsList)
			{
				if (spawnPointUnits.Place == SpawnPointUnits.SceneType.All)
				{
					if (spawnPointUnits.CurrentCount > spawnPointUnits.MaxCount || spawnPointUnits.CurrentCount < spawnPointUnits.MinCount)
					{
						this.WriteLineOfTableDebug(spawnPointUnits, this._redColor, spawnPointUnits.Type);
					}
					else
					{
						this.WriteLineOfTableDebug(spawnPointUnits, this._greenColor, spawnPointUnits.Type);
					}
				}
				else if (this._sceneType == spawnPointUnits.Place)
				{
					if (spawnPointUnits.CurrentCount > spawnPointUnits.MaxCount || spawnPointUnits.CurrentCount < spawnPointUnits.MinCount)
					{
						this.WriteLineOfTableDebug(spawnPointUnits, this._redColor, spawnPointUnits.Type);
					}
					else
					{
						this.WriteLineOfTableDebug(spawnPointUnits, this._greenColor, spawnPointUnits.Type);
					}
				}
			}
			if (this.ImGUIButton("COUNT ENTITIES", this._normalButton))
			{
				this.CountEntities();
			}
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000E53C File Offset: 0x0000C73C
		private void CalculateSpawnedAgentCount(SpawnPointUnits spUnit)
		{
			if (spUnit.SpName == "npc_common")
			{
				spUnit.SpawnedAgentCount = (int)((float)spUnit.CurrentCount * 0.2f + 0.15f);
			}
			else if (spUnit.SpName == "npc_common_limited")
			{
				spUnit.SpawnedAgentCount = (int)((float)spUnit.CurrentCount * 0.15f + 0.1f);
			}
			else if (spUnit.SpName == "npc_beggar")
			{
				spUnit.SpawnedAgentCount = (int)((float)spUnit.CurrentCount * 0.33f);
			}
			else if (spUnit.SpName == "spawnpoint_cleaner" || spUnit.SpName == "npc_dancer" || spUnit.SpName == "sp_guard_patrol" || spUnit.SpName == "sp_guard")
			{
				spUnit.SpawnedAgentCount = spUnit.CurrentCount;
			}
			else if (spUnit.CurrentCount != 0)
			{
				spUnit.SpawnedAgentCount = 1;
			}
			this._currentTownsfolkCount += spUnit.SpawnedAgentCount;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000E64C File Offset: 0x0000C84C
		private void CountEntities()
		{
			this._currentTownsfolkCount = 0;
			foreach (SpawnPointUnits spawnPointUnits in this._spUnitsList.ToList<SpawnPointUnits>())
			{
				List<GameEntity> list = base.Scene.FindEntitiesWithTag(spawnPointUnits.SpName).ToList<GameEntity>();
				int num = 0;
				using (List<GameEntity>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.GetEditModeLevelVisibility())
						{
							num++;
						}
					}
				}
				spawnPointUnits.CurrentCount = num;
				this.CalculateSpawnedAgentCount(spawnPointUnits);
				this.CountPassages(spawnPointUnits);
				using (List<GameEntity>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.IsGhostObject())
						{
							spawnPointUnits.CurrentCount--;
						}
					}
				}
				if (spawnPointUnits.SpName == "navigation_mesh_deactivator")
				{
					this._disabledFaceId = -1;
					using (List<GameEntity>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							GameEntity gameEntity = enumerator2.Current;
							NavigationMeshDeactivator firstScriptOfType = gameEntity.GetFirstScriptOfType<NavigationMeshDeactivator>();
							if (firstScriptOfType != null && firstScriptOfType.GameEntity.GetEditModeLevelVisibility())
							{
								this._disabledFaceId = firstScriptOfType.DisableFaceWithId;
							}
						}
						continue;
					}
				}
				if (spawnPointUnits.SpName == "alley_marker")
				{
					this.CheckForCommonAreas(list, spawnPointUnits);
				}
				else if (spawnPointUnits.SpName == "workshop_area_marker")
				{
					this.CheckForWorkshops(list, spawnPointUnits);
				}
				else if (spawnPointUnits.SpName == "center_conversation_point")
				{
					List<GameEntity> list2 = base.Scene.FindEntitiesWithTag("sp_player_conversation").ToList<GameEntity>();
					List<GameEntity> list3 = base.Scene.FindEntitiesWithTag("alley_marker").ToList<GameEntity>();
					foreach (GameEntity gameEntity2 in list2)
					{
						bool flag = false;
						using (List<GameEntity>.Enumerator enumerator3 = list3.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								if (enumerator3.Current.GetFirstScriptOfType<CommonAreaMarker>().IsPositionInRange(gameEntity2.GlobalPosition))
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							SpawnPointUnits spawnPointUnits2 = this._spUnitsList.FirstOrDefault((SpawnPointUnits x) => x.SpName == "center_conversation_point" && x.Place == this._sceneType);
							if (spawnPointUnits2 != null)
							{
								spawnPointUnits2.CurrentCount++;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000E960 File Offset: 0x0000CB60
		private void CheckForCommonAreas(IEnumerable<GameEntity> allGameEntitiesWithGivenTag, SpawnPointUnits spUnit)
		{
			foreach (GameEntity gameEntity in allGameEntitiesWithGivenTag)
			{
				CommonAreaMarker alleyMarker = gameEntity.GetFirstScriptOfType<CommonAreaMarker>();
				if (alleyMarker != null && !gameEntity.IsGhostObject())
				{
					float areaRadius = alleyMarker.AreaRadius;
					List<GameEntity> list = base.Scene.FindEntitiesWithTag("npc_common").ToList<GameEntity>();
					foreach (GameEntity gameEntity2 in list.ToList<GameEntity>())
					{
						float num = areaRadius * areaRadius;
						if (gameEntity2.HasScriptOfType<Passage>() || !gameEntity2.IsVisibleIncludeParents() || gameEntity2.GlobalPosition.DistanceSquared(gameEntity.GlobalPosition) > num)
						{
							list.Remove(gameEntity2);
						}
					}
					List<GameEntity> list2 = base.Scene.FindEntitiesWithTag("sp_player_conversation").ToList<GameEntity>();
					int num2 = 0;
					Func<SpawnPointUnits, bool> <>9__0;
					foreach (GameEntity gameEntity3 in list2)
					{
						if (alleyMarker.IsPositionInRange(gameEntity3.GlobalPosition))
						{
							IEnumerable<SpawnPointUnits> spUnitsList = this._spUnitsList;
							Func<SpawnPointUnits, bool> func;
							if ((func = <>9__0) == null)
							{
								func = (<>9__0 = (SpawnPointUnits x) => x.SpName == "sp_player_conversation_alley_" + alleyMarker.AreaIndex && x.Place == this._sceneType);
							}
							SpawnPointUnits spawnPointUnits = spUnitsList.FirstOrDefault(func);
							if (spawnPointUnits != null)
							{
								num2 = (spawnPointUnits.CurrentCount = num2 + 1);
							}
						}
					}
					if (alleyMarker.AreaIndex == 1)
					{
						SpawnPointUnits spawnPointUnits2 = this._spUnitsList.FirstOrDefault((SpawnPointUnits x) => x.SpName == "alley_1_population" && x.Place == this._sceneType);
						if (spawnPointUnits2 != null)
						{
							spawnPointUnits2.CurrentCount = this.FindValidSpawnPointCountOfUsableMachine(list);
						}
					}
					else if (alleyMarker.AreaIndex == 2)
					{
						SpawnPointUnits spawnPointUnits3 = this._spUnitsList.FirstOrDefault((SpawnPointUnits x) => x.SpName == "alley_2_population" && x.Place == this._sceneType);
						if (spawnPointUnits3 != null)
						{
							spawnPointUnits3.CurrentCount = this.FindValidSpawnPointCountOfUsableMachine(list);
						}
					}
					else if (alleyMarker.AreaIndex == 3)
					{
						SpawnPointUnits spawnPointUnits4 = this._spUnitsList.FirstOrDefault((SpawnPointUnits x) => x.SpName == "alley_3_population" && x.Place == this._sceneType);
						if (spawnPointUnits4 != null)
						{
							spawnPointUnits4.CurrentCount = this.FindValidSpawnPointCountOfUsableMachine(list);
						}
					}
				}
			}
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000EBF0 File Offset: 0x0000CDF0
		private void CheckForWorkshops(IEnumerable<GameEntity> allGameEntitiesWithGivenTag, SpawnPointUnits spUnit)
		{
			foreach (GameEntity gameEntity in allGameEntitiesWithGivenTag)
			{
				WorkshopAreaMarker workshopAreaMarker = gameEntity.GetFirstScriptOfType<WorkshopAreaMarker>();
				if (workshopAreaMarker != null && !gameEntity.IsGhostObject())
				{
					float areaRadius = workshopAreaMarker.AreaRadius;
					List<GameEntity> list = new List<GameEntity>();
					base.Scene.GetEntities(ref list);
					float num = areaRadius * areaRadius;
					foreach (GameEntity gameEntity2 in list.ToList<GameEntity>())
					{
						if (!gameEntity2.HasScriptOfType<UsableMachine>() || gameEntity2.HasScriptOfType<Passage>() || gameEntity2.GlobalPosition.DistanceSquared(gameEntity.GlobalPosition) > num)
						{
							list.Remove(gameEntity2);
						}
					}
					using (List<GameEntity>.Enumerator enumerator2 = base.Scene.FindEntitiesWithTag("sp_notables_parent").ToList<GameEntity>().GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.GlobalPosition.DistanceSquared(gameEntity.GlobalPosition) < num)
							{
								if (workshopAreaMarker.AreaIndex == 1)
								{
									SpawnPointUnits spawnPointUnits = this._spUnitsList.FirstOrDefault((SpawnPointUnits x) => x.SpName == "workshop_1_notable_parent" && x.Place == this._sceneType);
									if (spawnPointUnits != null)
									{
										spawnPointUnits.CurrentCount = 1;
									}
								}
								else if (workshopAreaMarker.AreaIndex == 2)
								{
									SpawnPointUnits spawnPointUnits2 = this._spUnitsList.FirstOrDefault((SpawnPointUnits x) => x.SpName == "workshop_2_notable_parent" && x.Place == this._sceneType);
									if (spawnPointUnits2 != null)
									{
										spawnPointUnits2.CurrentCount = 1;
									}
								}
								else if (workshopAreaMarker.AreaIndex == 3)
								{
									SpawnPointUnits spawnPointUnits3 = this._spUnitsList.FirstOrDefault((SpawnPointUnits x) => x.SpName == "workshop_3_notable_parent" && x.Place == this._sceneType);
									if (spawnPointUnits3 != null)
									{
										spawnPointUnits3.CurrentCount = 1;
									}
								}
							}
						}
					}
					List<GameEntity> list2 = base.Scene.FindEntitiesWithTag("sp_player_conversation").ToList<GameEntity>();
					int num2 = 0;
					Func<SpawnPointUnits, bool> <>9__3;
					foreach (GameEntity gameEntity3 in list2)
					{
						if (workshopAreaMarker.IsPositionInRange(gameEntity3.GlobalPosition))
						{
							IEnumerable<SpawnPointUnits> spUnitsList = this._spUnitsList;
							Func<SpawnPointUnits, bool> func;
							if ((func = <>9__3) == null)
							{
								func = (<>9__3 = (SpawnPointUnits x) => x.SpName == "sp_player_conversation_workshop_" + workshopAreaMarker.AreaIndex && x.Place == this._sceneType);
							}
							SpawnPointUnits spawnPointUnits4 = spUnitsList.FirstOrDefault(func);
							if (spawnPointUnits4 != null)
							{
								num2 = (spawnPointUnits4.CurrentCount = num2 + 1);
							}
						}
					}
					if (workshopAreaMarker.AreaIndex == 1)
					{
						SpawnPointUnits spawnPointUnits5 = this._spUnitsList.FirstOrDefault((SpawnPointUnits x) => x.SpName == "workshop_area_1_population" && x.Place == this._sceneType);
						if (spawnPointUnits5 != null)
						{
							spawnPointUnits5.CurrentCount += this.FindValidSpawnPointCountOfUsableMachine(list);
						}
					}
					else if (workshopAreaMarker.AreaIndex == 2)
					{
						SpawnPointUnits spawnPointUnits6 = this._spUnitsList.FirstOrDefault((SpawnPointUnits x) => x.SpName == "workshop_area_2_population" && x.Place == this._sceneType);
						if (spawnPointUnits6 != null)
						{
							spawnPointUnits6.CurrentCount += this.FindValidSpawnPointCountOfUsableMachine(list);
						}
					}
					else if (workshopAreaMarker.AreaIndex == 3)
					{
						SpawnPointUnits spawnPointUnits7 = this._spUnitsList.FirstOrDefault((SpawnPointUnits x) => x.SpName == "workshop_area_3_population" && x.Place == this._sceneType);
						if (spawnPointUnits7 != null)
						{
							spawnPointUnits7.CurrentCount += this.FindValidSpawnPointCountOfUsableMachine(list);
						}
					}
				}
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000EFA0 File Offset: 0x0000D1A0
		private int FindValidSpawnPointCountOfUsableMachine(List<GameEntity> gameEntities)
		{
			int num = 0;
			foreach (GameEntity gameEntity in gameEntities)
			{
				UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
				if (firstScriptOfType != null)
				{
					num += MissionAgentHandler.GetPointCountOfUsableMachine(firstScriptOfType, false);
				}
			}
			return num;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000EFFC File Offset: 0x0000D1FC
		private void CountPassages(SpawnPointUnits spUnit)
		{
			if (spUnit.SpName.Contains("npc_passage"))
			{
				foreach (GameEntity gameEntity in base.Scene.FindEntitiesWithTag("npc_passage"))
				{
					foreach (GameEntity gameEntity2 in gameEntity.GetChildren())
					{
						PassageUsePoint firstScriptOfType = gameEntity2.GetFirstScriptOfType<PassageUsePoint>();
						if (firstScriptOfType != null && !gameEntity2.IsGhostObject() && gameEntity2.GetEditModeLevelVisibility() && (this.DetectWhichPassage(firstScriptOfType, spUnit.SpName, "tavern") || this.DetectWhichPassage(firstScriptOfType, spUnit.SpName, "arena") || this.DetectWhichPassage(firstScriptOfType, spUnit.SpName, "prison") || this.DetectWhichPassage(firstScriptOfType, spUnit.SpName, "lordshall") || this.DetectWhichPassage(firstScriptOfType, spUnit.SpName, "house_1") || this.DetectWhichPassage(firstScriptOfType, spUnit.SpName, "house_2") || this.DetectWhichPassage(firstScriptOfType, spUnit.SpName, "house_3")))
						{
							spUnit.CurrentCount++;
						}
					}
				}
			}
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000F178 File Offset: 0x0000D378
		private void CalculateCurrentInvalidPointsCount()
		{
			this._currentInvalidPoints = 0;
			if (this._showAnimals)
			{
				this._currentInvalidPoints += this.GetCategoryCount(SpawnPointDebugView.CategoryId.Animal);
			}
			if (this._showChairs)
			{
				this._currentInvalidPoints += this.GetCategoryCount(SpawnPointDebugView.CategoryId.Chair);
			}
			if (this._showNPCs)
			{
				this._currentInvalidPoints += this.GetCategoryCount(SpawnPointDebugView.CategoryId.NPC);
			}
			if (this._showSemiValidPoints)
			{
				this._currentInvalidPoints += this.GetCategoryCount(SpawnPointDebugView.CategoryId.SemivalidChair);
			}
			if (this._showPassagePoints)
			{
				this._currentInvalidPoints += this.GetCategoryCount(SpawnPointDebugView.CategoryId.Passage);
			}
			if (this._showOutOfBoundPoints)
			{
				this._currentInvalidPoints += this.GetCategoryCount(SpawnPointDebugView.CategoryId.OutOfMissionBound);
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000F234 File Offset: 0x0000D434
		private bool DetectWhichPassage(PassageUsePoint passageUsePoint, string spName, string locationName)
		{
			string toLocationId = passageUsePoint.ToLocationId;
			if (this._sceneType != SpawnPointUnits.SceneType.Center && this._sceneType != SpawnPointUnits.SceneType.Castle)
			{
				locationName = "center";
			}
			return toLocationId == locationName && spName == "npc_passage_" + locationName;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000F26F File Offset: 0x0000D46F
		private void ShowNavigationCheckTab()
		{
			this.WriteNavigationMeshTabTexts();
			this.ToggleButtons();
			this.CalculateCurrentInvalidPointsCount();
			if (this.ImGUIButton("CHECK", this._normalButton))
			{
				this.CheckForNavigationMesh();
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000F29C File Offset: 0x0000D49C
		private void CheckForNavigationMesh()
		{
			this.ClearAllLists();
			this.CountEntities();
			foreach (SpawnPointUnits spawnPointUnits in this._spUnitsList)
			{
				if (!(spawnPointUnits.SpName == "alley_marker") && !(spawnPointUnits.SpName == "navigation_mesh_deactivator"))
				{
					this.CheckIfPassage(spawnPointUnits);
					this.CheckIfChairOrAnimal(spawnPointUnits);
				}
			}
			this.RemoveDuplicateValuesInLists();
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000F328 File Offset: 0x0000D528
		private void CheckNavigationMeshForParticularEntity(GameEntity gameEntity, SpawnPointDebugView.CategoryId categoryId)
		{
			if (gameEntity.Name == "workshop_1" || gameEntity.Name == "workshop_2" || gameEntity.Name == "workshop_3")
			{
				return;
			}
			Vec3 origin = gameEntity.GetGlobalFrame().origin;
			if (!gameEntity.HasScriptOfType<NavigationMeshDeactivator>() && MBEditor.IsEditModeOn && gameEntity.GetEditModeLevelVisibility() && gameEntity.HasScriptOfType<StandingPoint>())
			{
				if (Mission.Current != null && !Mission.Current.IsPositionInsideBoundaries(origin.AsVec2))
				{
					this.AddPositionToInvalidList(categoryId, origin, gameEntity, false, false);
				}
				this._particularfaceID = -1;
				if (base.Scene.GetNavigationMeshForPosition(ref origin, ref this._particularfaceID, 1.5f))
				{
					if (!gameEntity.Name.Contains("player") && this._particularfaceID == this._disabledFaceId && (this._sceneType == SpawnPointUnits.SceneType.Center || this._sceneType == SpawnPointUnits.SceneType.VillageCenter) && categoryId != SpawnPointDebugView.CategoryId.Chair && categoryId != SpawnPointDebugView.CategoryId.Animal)
					{
						if (!(gameEntity.Parent != null) || !(gameEntity.Parent.Name == "sp_battle_set"))
						{
							this.AddPositionToInvalidList(categoryId, origin, gameEntity, true, false);
							return;
						}
					}
					else if (gameEntity.Parent != null)
					{
						this.CheckSemiValidsOfChair(gameEntity);
						return;
					}
				}
				else
				{
					if (categoryId == SpawnPointDebugView.CategoryId.Chair && gameEntity.Parent != null)
					{
						this.CheckSemiValidsOfChair(gameEntity);
						return;
					}
					this.AddPositionToInvalidList(categoryId, origin, gameEntity, false, false);
				}
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000F494 File Offset: 0x0000D694
		private void CheckSemiValidsOfChair(GameEntity gameEntity)
		{
			AnimationPoint firstScriptOfType = gameEntity.GetFirstScriptOfType<AnimationPoint>();
			if (firstScriptOfType != null)
			{
				bool flag = false;
				bool flag2 = false;
				List<AnimationPoint> alternatives = firstScriptOfType.GetAlternatives();
				if (alternatives != null && !Extensions.IsEmpty<AnimationPoint>(alternatives))
				{
					foreach (AnimationPoint animationPoint in alternatives)
					{
						Vec3 origin = animationPoint.GameEntity.GetGlobalFrame().origin;
						if ((base.Scene.GetNavigationMeshForPosition(ref origin, ref this._particularfaceID, 1.5f) || animationPoint.GameEntity.IsGhostObject()) && this._particularfaceID != this._disabledFaceId)
						{
							flag = true;
							if (animationPoint == firstScriptOfType)
							{
								flag2 = true;
							}
						}
					}
					if (!flag2)
					{
						if (flag)
						{
							Vec3 origin2 = firstScriptOfType.GameEntity.GetGlobalFrame().origin;
							this.AddPositionToInvalidList(SpawnPointDebugView.CategoryId.SemivalidChair, origin2, gameEntity, false, true);
							return;
						}
						Vec3 origin3 = firstScriptOfType.GameEntity.GetGlobalFrame().origin;
						this.AddPositionToInvalidList(SpawnPointDebugView.CategoryId.Chair, origin3, gameEntity, false, false);
						return;
					}
				}
				else
				{
					Vec3 origin4 = firstScriptOfType.GameEntity.GetGlobalFrame().origin;
					if (!base.Scene.GetNavigationMeshForPosition(ref origin4) && !firstScriptOfType.GameEntity.IsGhostObject())
					{
						this.AddPositionToInvalidList(SpawnPointDebugView.CategoryId.Chair, origin4, gameEntity, false, false);
					}
				}
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000F5D8 File Offset: 0x0000D7D8
		private void CheckIfChairOrAnimal(SpawnPointUnits spUnit)
		{
			foreach (GameEntity gameEntity in base.Scene.FindEntitiesWithTag(spUnit.SpName))
			{
				IEnumerable<GameEntity> children = gameEntity.GetChildren();
				if (children.Count<GameEntity>() != 0)
				{
					using (IEnumerator<GameEntity> enumerator2 = children.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							GameEntity gameEntity2 = enumerator2.Current;
							if (gameEntity2.Name.Contains("chair") && !gameEntity2.IsGhostObject())
							{
								this.CheckNavigationMeshForParticularEntity(gameEntity2, SpawnPointDebugView.CategoryId.Chair);
							}
							else if (!gameEntity2.IsGhostObject() && !gameEntity2.IsGhostObject())
							{
								this.CheckNavigationMeshForParticularEntity(gameEntity2, SpawnPointDebugView.CategoryId.NPC);
							}
						}
						continue;
					}
				}
				if (spUnit.Type == "animal" && !gameEntity.IsGhostObject())
				{
					this.CheckNavigationMeshForParticularEntity(gameEntity, SpawnPointDebugView.CategoryId.Animal);
				}
				else if (!gameEntity.IsGhostObject())
				{
					this.CheckNavigationMeshForParticularEntity(gameEntity, SpawnPointDebugView.CategoryId.NPC);
				}
			}
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000F6E8 File Offset: 0x0000D8E8
		private void CheckIfPassage(SpawnPointUnits spUnit)
		{
			if (spUnit.SpName.Contains("passage"))
			{
				foreach (GameEntity gameEntity in base.Scene.FindEntitiesWithTag("npc_passage"))
				{
					foreach (GameEntity gameEntity2 in gameEntity.GetChildren())
					{
						if (gameEntity2.Name.Contains("passage") && !gameEntity2.IsGhostObject())
						{
							this.CheckNavigationMeshForParticularEntity(gameEntity2, SpawnPointDebugView.CategoryId.Passage);
							break;
						}
					}
				}
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000F7A4 File Offset: 0x0000D9A4
		private void RemoveDuplicateValuesInLists()
		{
			this._invalidSpawnPointsDictionary = this._invalidSpawnPointsDictionary.ToDictionary((KeyValuePair<SpawnPointDebugView.CategoryId, List<SpawnPointDebugView.InvalidPosition>> c) => c.Key, (KeyValuePair<SpawnPointDebugView.CategoryId, List<SpawnPointDebugView.InvalidPosition>> c) => c.Value.Distinct<SpawnPointDebugView.InvalidPosition>().ToList<SpawnPointDebugView.InvalidPosition>());
			if (this._invalidSpawnPointsDictionary.ContainsKey(SpawnPointDebugView.CategoryId.SemivalidChair))
			{
				foreach (SpawnPointDebugView.InvalidPosition invalidPosition in this._invalidSpawnPointsDictionary[SpawnPointDebugView.CategoryId.SemivalidChair])
				{
					if (this._invalidSpawnPointsDictionary[SpawnPointDebugView.CategoryId.Chair].Contains(invalidPosition))
					{
						this._invalidSpawnPointsDictionary[SpawnPointDebugView.CategoryId.Chair].Remove(invalidPosition);
					}
				}
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000F87C File Offset: 0x0000DA7C
		private void AddPositionToInvalidList(SpawnPointDebugView.CategoryId categoryId, Vec3 globalPosition, GameEntity entity, bool isDisabledNavMesh, bool doNotShowWarning = false)
		{
			if (!entity.IsGhostObject() && entity.IsVisibleIncludeParents() && this._invalidSpawnPointsDictionary.ContainsKey(categoryId))
			{
				SpawnPointDebugView.InvalidPosition invalidPosition;
				invalidPosition.position = globalPosition;
				invalidPosition.entity = entity;
				invalidPosition.isDisabledNavMesh = isDisabledNavMesh;
				invalidPosition.doNotShowWarning = doNotShowWarning;
				if (this._invalidSpawnPointsDictionary[categoryId].All((SpawnPointDebugView.InvalidPosition x) => x.position != globalPosition))
				{
					this._invalidSpawnPointsDictionary[categoryId].Add(invalidPosition);
				}
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000F90C File Offset: 0x0000DB0C
		private void ToggleButtons()
		{
			if (this._showNPCs)
			{
				this.DrawDebugLinesForInvalidSpawnPoints(SpawnPointDebugView.CategoryId.NPC, this._npcDebugLineColor);
			}
			if (this._showChairs)
			{
				this.DrawDebugLinesForInvalidSpawnPoints(SpawnPointDebugView.CategoryId.Chair, this._chairDebugLineColor);
			}
			if (this._showAnimals)
			{
				this.DrawDebugLinesForInvalidSpawnPoints(SpawnPointDebugView.CategoryId.Animal, this._animalDebugLineColor);
			}
			if (this._showSemiValidPoints)
			{
				this.DrawDebugLinesForInvalidSpawnPoints(SpawnPointDebugView.CategoryId.SemivalidChair, this._semivalidChairDebugLineColor);
			}
			if (this._showPassagePoints)
			{
				this.DrawDebugLinesForInvalidSpawnPoints(SpawnPointDebugView.CategoryId.Passage, this._passageDebugLineColor);
			}
			if (this._showOutOfBoundPoints)
			{
				this.DrawDebugLinesForInvalidSpawnPoints(SpawnPointDebugView.CategoryId.OutOfMissionBound, this._missionBoundDebugLineColor);
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000F998 File Offset: 0x0000DB98
		private void FindAllPrefabsWithSelectedTag()
		{
			if (this.allPrefabsWithParticularTag != null)
			{
				string[] array = this.allPrefabsWithParticularTag.Split(new char[] { '/' });
				for (int i = 0; i < array.Length; i++)
				{
					this.ImGUITextArea(array[i], !this._separatorNeeded, !this._onSameLineNeeded);
				}
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000F9F0 File Offset: 0x0000DBF0
		private void FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId CategoryId)
		{
			List<SpawnPointDebugView.InvalidPosition> list;
			this._invalidSpawnPointsDictionary.TryGetValue(CategoryId, out list);
			if (list.Count == 0 || this._cameraFocusIndex < 0 || this._cameraFocusIndex >= list.Count)
			{
				this._cameraFocusIndex = 0;
				return;
			}
			MBEditor.ZoomToPosition(list[this._cameraFocusIndex].position);
			int num2;
			if (this._cameraFocusIndex < list.Count - 1)
			{
				int num = this._cameraFocusIndex + 1;
				this._cameraFocusIndex = num;
				num2 = num;
			}
			else
			{
				num2 = (this._cameraFocusIndex = 0);
			}
			this._cameraFocusIndex = num2;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000FA7C File Offset: 0x0000DC7C
		private int GetCategoryCount(SpawnPointDebugView.CategoryId CategoryId)
		{
			int num = 0;
			if (this._invalidSpawnPointsDictionary.ContainsKey(CategoryId))
			{
				num = this._invalidSpawnPointsDictionary[CategoryId].Count;
			}
			return num;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000FAAC File Offset: 0x0000DCAC
		[CommandLineFunctionality.CommandLineArgumentFunction("mission_spawnpoint_count_and_mesh_checker_ui", "debug")]
		public static string OpenToolFromConsole(List<string> strings)
		{
			if (strings.Count != 1 || (strings[0] != "0" && strings[0] != "1"))
			{
				return "Input is incorrect.";
			}
			if (strings[0] == "0")
			{
				SpawnPointDebugView.ActivateDebugUI = false;
				GameEntity firstEntityWithScriptComponent = Mission.Current.Scene.GetFirstEntityWithScriptComponent<SpawnPointDebugView>();
				if (firstEntityWithScriptComponent != null)
				{
					SpawnPointDebugView spawnPointDebugView = firstEntityWithScriptComponent.GetScriptComponents<SpawnPointDebugView>().First<SpawnPointDebugView>();
					spawnPointDebugView.SetScriptComponentToTick(spawnPointDebugView.GetTickRequirement());
				}
				return "Debug Tool: OFF";
			}
			if (strings[0] == "1")
			{
				SpawnPointDebugView.ActivateDebugUI = true;
				GameEntity firstEntityWithScriptComponent2 = Mission.Current.Scene.GetFirstEntityWithScriptComponent<SpawnPointDebugView>();
				if (firstEntityWithScriptComponent2 != null)
				{
					SpawnPointDebugView spawnPointDebugView2 = firstEntityWithScriptComponent2.GetScriptComponents<SpawnPointDebugView>().First<SpawnPointDebugView>();
					spawnPointDebugView2.SetScriptComponentToTick(spawnPointDebugView2.GetTickRequirement());
				}
				return "Debug Tool: ON";
			}
			return " ";
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000FB90 File Offset: 0x0000DD90
		private void ClearAllLists()
		{
			foreach (KeyValuePair<SpawnPointDebugView.CategoryId, List<SpawnPointDebugView.InvalidPosition>> keyValuePair in this._invalidSpawnPointsDictionary)
			{
				keyValuePair.Value.Clear();
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000FBE8 File Offset: 0x0000DDE8
		private bool ImGUIButton(string buttonText, bool smallButton)
		{
			if (smallButton)
			{
				return Imgui.SmallButton(buttonText);
			}
			return Imgui.Button(buttonText);
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000FBFA File Offset: 0x0000DDFA
		private void LeaveSpaceBetweenTabs()
		{
			this.OnSameLine();
			this.ImGUITextArea(" ", !this._separatorNeeded, this._onSameLineNeeded);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000FC1C File Offset: 0x0000DE1C
		private void EndImGUIWindow()
		{
			Imgui.End();
			Imgui.EndMainThreadScope();
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000FC28 File Offset: 0x0000DE28
		private void StartImGUIWindow(string str)
		{
			Imgui.BeginMainThreadScope();
			Imgui.Begin(str);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000FC35 File Offset: 0x0000DE35
		private void ImGUITextArea(string text, bool separatorNeeded, bool onSameLine)
		{
			Imgui.Text(text);
			this.ImGUISeparatorSameLineHandler(separatorNeeded, onSameLine);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000FC45 File Offset: 0x0000DE45
		private void ImGUICheckBox(string text, ref bool is_checked, bool separatorNeeded, bool onSameLine)
		{
			Imgui.Checkbox(text, ref is_checked);
			this.ImGUISeparatorSameLineHandler(separatorNeeded, onSameLine);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000FC58 File Offset: 0x0000DE58
		private void ImguiSameLine(float positionX, float spacingWidth)
		{
			Imgui.SameLine(positionX, spacingWidth);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000FC61 File Offset: 0x0000DE61
		private void ImGUISeparatorSameLineHandler(bool separatorNeeded, bool onSameLine)
		{
			if (separatorNeeded)
			{
				this.Separator();
			}
			if (onSameLine)
			{
				this.OnSameLine();
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000FC75 File Offset: 0x0000DE75
		private void OnSameLine()
		{
			Imgui.SameLine(0f, 0f);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000FC86 File Offset: 0x0000DE86
		private void Separator()
		{
			Imgui.Separator();
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000FC90 File Offset: 0x0000DE90
		private void WriteLineOfTableDebug(SpawnPointUnits spUnit, Vec3 Color, string type)
		{
			if ((type == "animal" && this._showAnimalsList) || (type == "npc" && this._showNPCsList) || (type == "passage" && this._showPassagesList) || (type == "DONTUSE" && this._showDontUseList) || (type == "other" && this._showOthersList))
			{
				Imgui.PushStyleColor(0, ref Color);
				this.ImguiSameLine(0f, 0f);
				this.ImGUITextArea(spUnit.SpName, !this._separatorNeeded, this._onSameLineNeeded);
				this.ImguiSameLine(305f, 10f);
				this.ImGUITextArea(spUnit.MinCount.ToString(), !this._separatorNeeded, this._onSameLineNeeded);
				this.ImguiSameLine(345f, 10f);
				this.ImGUITextArea((spUnit.MaxCount == int.MaxValue) ? "-" : spUnit.MaxCount.ToString(), !this._separatorNeeded, this._onSameLineNeeded);
				this.ImguiSameLine(405f, 10f);
				this.ImGUITextArea(spUnit.CurrentCount.ToString(), !this._separatorNeeded, this._onSameLineNeeded);
				this.ImguiSameLine(500f, 10f);
				this.ImGUITextArea(spUnit.SpawnedAgentCount.ToString(), !this._separatorNeeded, this._onSameLineNeeded);
				Imgui.PopStyleColor();
				this.ImguiSameLine(575f, 10f);
				if (this.ImGUIButton(spUnit.SpName, this._normalButton))
				{
					this._relatedEntityWindow = true;
					this._relatedPrefabTag = spUnit.SpName;
					this.allPrefabsWithParticularTag = MBEditor.GetAllPrefabsAndChildWithTag(this._relatedPrefabTag);
				}
				this.ImGUITextArea(" ", !this._separatorNeeded, !this._onSameLineNeeded);
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000FE84 File Offset: 0x0000E084
		private void WriteNavigationMeshTabTexts()
		{
			this.ImGUITextArea("This tool will mark the spawn points which are not on the navigation mesh", !this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("or on the navigation mesh that will be deactivated by 'Navigation Mesh Deactivator'", !this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("Deactivation Face Id: " + this._disabledFaceId, !this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("Click 'CHECK' button to calculate.", this._separatorNeeded, !this._onSameLineNeeded);
			Imgui.PushStyleColor(0, ref this._redColor);
			this.ImGUICheckBox("Show NPCs ", ref this._showNPCs, !this._separatorNeeded, this._onSameLineNeeded);
			this.ImGUITextArea("(" + this.GetCategoryCount(SpawnPointDebugView.CategoryId.NPC) + ")", !this._separatorNeeded, !this._onSameLineNeeded);
			if (this._showNPCs)
			{
				if (this.ImGUIButton("<Previous NPC", this._normalButton))
				{
					this._cameraFocusIndex -= 2;
					this.FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId.NPC);
				}
				this.ImguiSameLine(120f, 20f);
				if (this.ImGUIButton("Next NPC>", this._normalButton))
				{
					this.FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId.NPC);
				}
				this.ImGUITextArea(string.Concat(new object[]
				{
					this._cameraFocusIndex + 1,
					" (",
					this.GetCategoryCount(SpawnPointDebugView.CategoryId.NPC),
					")"
				}), this._separatorNeeded, !this._onSameLineNeeded);
			}
			Imgui.PopStyleColor();
			Imgui.PushStyleColor(0, ref this._blueColor);
			this.ImGUICheckBox("Show Animals ", ref this._showAnimals, !this._separatorNeeded, this._onSameLineNeeded);
			this.ImGUITextArea("(" + this.GetCategoryCount(SpawnPointDebugView.CategoryId.Animal) + ")", !this._separatorNeeded, !this._onSameLineNeeded);
			if (this._showAnimals)
			{
				if (this.ImGUIButton("<Previous Animal", this._normalButton))
				{
					this._cameraFocusIndex -= 2;
					this.FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId.Animal);
				}
				this.ImguiSameLine(120f, 20f);
				if (this.ImGUIButton("Next Animal>", this._normalButton))
				{
					this.FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId.Animal);
				}
				this.ImGUITextArea(string.Concat(new object[]
				{
					this._cameraFocusIndex + 1,
					" (",
					this.GetCategoryCount(SpawnPointDebugView.CategoryId.Animal),
					")"
				}), !this._separatorNeeded, !this._onSameLineNeeded);
			}
			Imgui.PopStyleColor();
			Imgui.PushStyleColor(0, ref this._purbleColor);
			this.ImGUICheckBox("Show Passages ", ref this._showPassagePoints, !this._separatorNeeded, this._onSameLineNeeded);
			this.ImGUITextArea("(" + this.GetCategoryCount(SpawnPointDebugView.CategoryId.Passage) + ")", !this._separatorNeeded, !this._onSameLineNeeded);
			if (this._showPassagePoints)
			{
				if (this.ImGUIButton("<Previous Passage", this._normalButton))
				{
					this._cameraFocusIndex -= 2;
					this.FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId.Passage);
				}
				this.ImguiSameLine(120f, 20f);
				if (this.ImGUIButton("Next Passage>", this._normalButton))
				{
					this.FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId.Passage);
				}
				this.ImGUITextArea(string.Concat(new object[]
				{
					this._cameraFocusIndex + 1,
					" (",
					this.GetCategoryCount(SpawnPointDebugView.CategoryId.Passage),
					")"
				}), !this._separatorNeeded, !this._onSameLineNeeded);
			}
			Imgui.PopStyleColor();
			Imgui.PushStyleColor(0, ref this._greenColor);
			this.ImGUICheckBox("Show Chairs ", ref this._showChairs, !this._separatorNeeded, this._onSameLineNeeded);
			this.ImGUITextArea("(" + this.GetCategoryCount(SpawnPointDebugView.CategoryId.Chair) + ")", !this._separatorNeeded, !this._onSameLineNeeded);
			if (this._showChairs)
			{
				if (this.ImGUIButton("<Previous Chair", this._normalButton))
				{
					this._cameraFocusIndex -= 2;
					this.FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId.Chair);
				}
				this.ImguiSameLine(120f, 20f);
				if (this.ImGUIButton("Next Chair>", this._normalButton))
				{
					this.FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId.Chair);
				}
				this.ImGUITextArea(string.Concat(new object[]
				{
					this._cameraFocusIndex + 1,
					" (",
					this.GetCategoryCount(SpawnPointDebugView.CategoryId.Chair),
					")"
				}), !this._separatorNeeded, !this._onSameLineNeeded);
			}
			Imgui.PopStyleColor();
			Imgui.PushStyleColor(0, ref this._yellowColor);
			this.ImGUICheckBox("Show semi-valid Chairs* ", ref this._showSemiValidPoints, !this._separatorNeeded, this._onSameLineNeeded);
			this.ImGUITextArea("(" + this.GetCategoryCount(SpawnPointDebugView.CategoryId.SemivalidChair) + ")", !this._separatorNeeded, !this._onSameLineNeeded);
			if (this._showSemiValidPoints)
			{
				if (this.ImGUIButton("<Previous S-Chair", this._normalButton))
				{
					this._cameraFocusIndex -= 2;
					this.FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId.SemivalidChair);
				}
				this.ImguiSameLine(120f, 20f);
				if (this.ImGUIButton("Next S-Chair>", this._normalButton))
				{
					this.FocusCameraToMisplacedObjects(SpawnPointDebugView.CategoryId.SemivalidChair);
				}
				this.ImGUITextArea(string.Concat(new object[]
				{
					this._cameraFocusIndex + 1,
					" (",
					this.GetCategoryCount(SpawnPointDebugView.CategoryId.SemivalidChair),
					")"
				}), !this._separatorNeeded, !this._onSameLineNeeded);
			}
			Imgui.PopStyleColor();
			this.ImGUICheckBox("Show out of Mission Bound Points**", ref this._showOutOfBoundPoints, !this._separatorNeeded, this._onSameLineNeeded);
			this.ImGUITextArea(" (" + this.GetCategoryCount(SpawnPointDebugView.CategoryId.OutOfMissionBound) + ")", !this._separatorNeeded, !this._onSameLineNeeded);
			this._totalInvalidPoints = this.GetCategoryCount(SpawnPointDebugView.CategoryId.NPC) + this.GetCategoryCount(SpawnPointDebugView.CategoryId.Chair) + this.GetCategoryCount(SpawnPointDebugView.CategoryId.Animal) + this.GetCategoryCount(SpawnPointDebugView.CategoryId.SemivalidChair) + this.GetCategoryCount(SpawnPointDebugView.CategoryId.Passage) + this.GetCategoryCount(SpawnPointDebugView.CategoryId.OutOfMissionBound);
			this.ImGUITextArea(string.Concat(new object[] { "(", this._currentInvalidPoints, " / ", this._totalInvalidPoints, " ) are being shown." }), !this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("Found " + this._totalInvalidPoints + " invalid spawnpoints.", this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("* Points that have at least one valid point as alternative", this._separatorNeeded, !this._onSameLineNeeded);
			if (Mission.Current == null)
			{
				this.ImGUITextArea("** Mission bound checking feature is not working in editor. Open mission to check it.", this._separatorNeeded, !this._onSameLineNeeded);
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000105B8 File Offset: 0x0000E7B8
		private void DrawDebugLinesForInvalidSpawnPoints(SpawnPointDebugView.CategoryId CategoryId, uint color)
		{
			if (this._invalidSpawnPointsDictionary.ContainsKey(CategoryId))
			{
				foreach (SpawnPointDebugView.InvalidPosition invalidPosition in this._invalidSpawnPointsDictionary[CategoryId])
				{
				}
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00010618 File Offset: 0x0000E818
		private void WriteTableHeaders()
		{
			this.ImguiSameLine(0f, 0f);
			this.ImGUITextArea("Tag Name", !this._separatorNeeded, this._onSameLineNeeded);
			this.ImguiSameLine(295f, 10f);
			this.ImGUITextArea("Min", !this._separatorNeeded, this._onSameLineNeeded);
			this.ImguiSameLine(340f, 10f);
			this.ImGUITextArea("Max", !this._separatorNeeded, this._onSameLineNeeded);
			this.ImguiSameLine(390f, 10f);
			this.ImGUITextArea("Current", !this._separatorNeeded, this._onSameLineNeeded);
			this.ImguiSameLine(465f, 10f);
			this.ImGUITextArea("Agent Count", !this._separatorNeeded, this._onSameLineNeeded);
			this.ImguiSameLine(575f, 10f);
			this.ImGUITextArea("List all prefabs with tag:", this._separatorNeeded, !this._onSameLineNeeded);
		}

		// Token: 0x04000087 RID: 135
		private const string BattleSetName = "sp_battle_set";

		// Token: 0x04000088 RID: 136
		private const string CenterConversationPoint = "center_conversation_point";

		// Token: 0x04000089 RID: 137
		public static bool ActivateDebugUI;

		// Token: 0x0400008A RID: 138
		public bool ActivateDebugUIEditor;

		// Token: 0x0400008B RID: 139
		private readonly bool _separatorNeeded = true;

		// Token: 0x0400008C RID: 140
		private readonly bool _onSameLineNeeded = true;

		// Token: 0x0400008D RID: 141
		private bool _townCenterRadioButton;

		// Token: 0x0400008E RID: 142
		private bool _tavernRadioButton;

		// Token: 0x0400008F RID: 143
		private bool _arenaRadioButton;

		// Token: 0x04000090 RID: 144
		private bool _villageRadioButton;

		// Token: 0x04000091 RID: 145
		private bool _lordshallRadioButton;

		// Token: 0x04000092 RID: 146
		private bool _castleRadioButton;

		// Token: 0x04000093 RID: 147
		private bool _basicInformationTab;

		// Token: 0x04000094 RID: 148
		private bool _entityInformationTab;

		// Token: 0x04000095 RID: 149
		private bool _navigationMeshCheckTab;

		// Token: 0x04000096 RID: 150
		private bool _relatedEntityWindow;

		// Token: 0x04000097 RID: 151
		private string _relatedPrefabTag;

		// Token: 0x04000098 RID: 152
		private int _cameraFocusIndex;

		// Token: 0x04000099 RID: 153
		private bool _showNPCs;

		// Token: 0x0400009A RID: 154
		private bool _showChairs;

		// Token: 0x0400009B RID: 155
		private bool _showAnimals;

		// Token: 0x0400009C RID: 156
		private bool _showSemiValidPoints;

		// Token: 0x0400009D RID: 157
		private bool _showPassagePoints;

		// Token: 0x0400009E RID: 158
		private bool _showOutOfBoundPoints;

		// Token: 0x0400009F RID: 159
		private bool _showPassagesList;

		// Token: 0x040000A0 RID: 160
		private bool _showAnimalsList;

		// Token: 0x040000A1 RID: 161
		private bool _showNPCsList;

		// Token: 0x040000A2 RID: 162
		private bool _showDontUseList;

		// Token: 0x040000A3 RID: 163
		private bool _showOthersList;

		// Token: 0x040000A4 RID: 164
		private string _sceneName;

		// Token: 0x040000A5 RID: 165
		private SpawnPointUnits.SceneType _sceneType;

		// Token: 0x040000A6 RID: 166
		private readonly bool _normalButton;

		// Token: 0x040000A7 RID: 167
		private int _currentTownsfolkCount;

		// Token: 0x040000A8 RID: 168
		private Vec3 _redColor = new Vec3(200f, 0f, 0f, 255f);

		// Token: 0x040000A9 RID: 169
		private Vec3 _greenColor = new Vec3(0f, 200f, 0f, 255f);

		// Token: 0x040000AA RID: 170
		private Vec3 _blueColor = new Vec3(0f, 180f, 180f, 255f);

		// Token: 0x040000AB RID: 171
		private Vec3 _yellowColor = new Vec3(200f, 200f, 0f, 255f);

		// Token: 0x040000AC RID: 172
		private Vec3 _purbleColor = new Vec3(255f, 0f, 255f, 255f);

		// Token: 0x040000AD RID: 173
		private uint _npcDebugLineColor = 4294901760U;

		// Token: 0x040000AE RID: 174
		private uint _chairDebugLineColor = 4278255360U;

		// Token: 0x040000AF RID: 175
		private uint _animalDebugLineColor = 4279356620U;

		// Token: 0x040000B0 RID: 176
		private uint _semivalidChairDebugLineColor = 4294963200U;

		// Token: 0x040000B1 RID: 177
		private uint _passageDebugLineColor = 4288217241U;

		// Token: 0x040000B2 RID: 178
		private uint _missionBoundDebugLineColor = uint.MaxValue;

		// Token: 0x040000B3 RID: 179
		private int _totalInvalidPoints;

		// Token: 0x040000B4 RID: 180
		private int _currentInvalidPoints;

		// Token: 0x040000B5 RID: 181
		private int _disabledFaceId;

		// Token: 0x040000B6 RID: 182
		private int _particularfaceID;

		// Token: 0x040000B7 RID: 183
		private Dictionary<SpawnPointDebugView.CategoryId, List<SpawnPointDebugView.InvalidPosition>> _invalidSpawnPointsDictionary = new Dictionary<SpawnPointDebugView.CategoryId, List<SpawnPointDebugView.InvalidPosition>>();

		// Token: 0x040000B8 RID: 184
		private string allPrefabsWithParticularTag;

		// Token: 0x040000B9 RID: 185
		private IList<SpawnPointUnits> _spUnitsList = new List<SpawnPointUnits>();

		// Token: 0x0200006D RID: 109
		private enum CategoryId
		{
			// Token: 0x04000276 RID: 630
			NPC,
			// Token: 0x04000277 RID: 631
			Animal,
			// Token: 0x04000278 RID: 632
			Chair,
			// Token: 0x04000279 RID: 633
			Passage,
			// Token: 0x0400027A RID: 634
			OutOfMissionBound,
			// Token: 0x0400027B RID: 635
			SemivalidChair
		}

		// Token: 0x0200006E RID: 110
		private struct InvalidPosition
		{
			// Token: 0x0400027C RID: 636
			public Vec3 position;

			// Token: 0x0400027D RID: 637
			public GameEntity entity;

			// Token: 0x0400027E RID: 638
			public bool isDisabledNavMesh;

			// Token: 0x0400027F RID: 639
			public bool doNotShowWarning;
		}
	}
}
