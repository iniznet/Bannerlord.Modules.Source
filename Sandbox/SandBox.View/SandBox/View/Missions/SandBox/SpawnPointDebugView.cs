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
	public class SpawnPointDebugView : ScriptComponentBehavior
	{
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			this.DetermineSceneType();
			this.AddSpawnPointsToList(false);
		}

		protected override void OnInit()
		{
			base.OnInit();
			this.DetermineSceneType();
			this.AddSpawnPointsToList(false);
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			if (SpawnPointDebugView.ActivateDebugUI || (MBEditor.IsEditModeOn && this.ActivateDebugUIEditor))
			{
				return 2 | base.GetTickRequirement();
			}
			return base.GetTickRequirement();
		}

		protected override void OnTick(float dt)
		{
			this.ToolMainFunction();
		}

		protected override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.ToolMainFunction();
		}

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

		private void ShowBasicInformationTab()
		{
			this.ImGUITextArea("Tool tried to detect the scene type. If scene type is not correct or not determined", !this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("please select the scene type from toggle buttons below.", this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("Scene Type: " + this._sceneType + " ", !this._separatorNeeded, !this._onSameLineNeeded);
			this.ImGUITextArea("Scene Name: " + this._sceneName + " ", !this._separatorNeeded, !this._onSameLineNeeded);
			this.HandleRadioButtons();
		}

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

		private void ChangeTab(bool basicInformationTab, bool entityInformationTab, bool navigationMeshCheckTab)
		{
			this._basicInformationTab = basicInformationTab;
			this._entityInformationTab = entityInformationTab;
			this._navigationMeshCheckTab = navigationMeshCheckTab;
		}

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

		private bool DetectWhichPassage(PassageUsePoint passageUsePoint, string spName, string locationName)
		{
			string toLocationId = passageUsePoint.ToLocationId;
			if (this._sceneType != SpawnPointUnits.SceneType.Center && this._sceneType != SpawnPointUnits.SceneType.Castle)
			{
				locationName = "center";
			}
			return toLocationId == locationName && spName == "npc_passage_" + locationName;
		}

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

		private int GetCategoryCount(SpawnPointDebugView.CategoryId CategoryId)
		{
			int num = 0;
			if (this._invalidSpawnPointsDictionary.ContainsKey(CategoryId))
			{
				num = this._invalidSpawnPointsDictionary[CategoryId].Count;
			}
			return num;
		}

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

		private void ClearAllLists()
		{
			foreach (KeyValuePair<SpawnPointDebugView.CategoryId, List<SpawnPointDebugView.InvalidPosition>> keyValuePair in this._invalidSpawnPointsDictionary)
			{
				keyValuePair.Value.Clear();
			}
		}

		private bool ImGUIButton(string buttonText, bool smallButton)
		{
			if (smallButton)
			{
				return Imgui.SmallButton(buttonText);
			}
			return Imgui.Button(buttonText);
		}

		private void LeaveSpaceBetweenTabs()
		{
			this.OnSameLine();
			this.ImGUITextArea(" ", !this._separatorNeeded, this._onSameLineNeeded);
		}

		private void EndImGUIWindow()
		{
			Imgui.End();
			Imgui.EndMainThreadScope();
		}

		private void StartImGUIWindow(string str)
		{
			Imgui.BeginMainThreadScope();
			Imgui.Begin(str);
		}

		private void ImGUITextArea(string text, bool separatorNeeded, bool onSameLine)
		{
			Imgui.Text(text);
			this.ImGUISeparatorSameLineHandler(separatorNeeded, onSameLine);
		}

		private void ImGUICheckBox(string text, ref bool is_checked, bool separatorNeeded, bool onSameLine)
		{
			Imgui.Checkbox(text, ref is_checked);
			this.ImGUISeparatorSameLineHandler(separatorNeeded, onSameLine);
		}

		private void ImguiSameLine(float positionX, float spacingWidth)
		{
			Imgui.SameLine(positionX, spacingWidth);
		}

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

		private void OnSameLine()
		{
			Imgui.SameLine(0f, 0f);
		}

		private void Separator()
		{
			Imgui.Separator();
		}

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

		private void DrawDebugLinesForInvalidSpawnPoints(SpawnPointDebugView.CategoryId CategoryId, uint color)
		{
			if (this._invalidSpawnPointsDictionary.ContainsKey(CategoryId))
			{
				foreach (SpawnPointDebugView.InvalidPosition invalidPosition in this._invalidSpawnPointsDictionary[CategoryId])
				{
				}
			}
		}

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

		private const string BattleSetName = "sp_battle_set";

		private const string CenterConversationPoint = "center_conversation_point";

		public static bool ActivateDebugUI;

		public bool ActivateDebugUIEditor;

		private readonly bool _separatorNeeded = true;

		private readonly bool _onSameLineNeeded = true;

		private bool _townCenterRadioButton;

		private bool _tavernRadioButton;

		private bool _arenaRadioButton;

		private bool _villageRadioButton;

		private bool _lordshallRadioButton;

		private bool _castleRadioButton;

		private bool _basicInformationTab;

		private bool _entityInformationTab;

		private bool _navigationMeshCheckTab;

		private bool _relatedEntityWindow;

		private string _relatedPrefabTag;

		private int _cameraFocusIndex;

		private bool _showNPCs;

		private bool _showChairs;

		private bool _showAnimals;

		private bool _showSemiValidPoints;

		private bool _showPassagePoints;

		private bool _showOutOfBoundPoints;

		private bool _showPassagesList;

		private bool _showAnimalsList;

		private bool _showNPCsList;

		private bool _showDontUseList;

		private bool _showOthersList;

		private string _sceneName;

		private SpawnPointUnits.SceneType _sceneType;

		private readonly bool _normalButton;

		private int _currentTownsfolkCount;

		private Vec3 _redColor = new Vec3(200f, 0f, 0f, 255f);

		private Vec3 _greenColor = new Vec3(0f, 200f, 0f, 255f);

		private Vec3 _blueColor = new Vec3(0f, 180f, 180f, 255f);

		private Vec3 _yellowColor = new Vec3(200f, 200f, 0f, 255f);

		private Vec3 _purbleColor = new Vec3(255f, 0f, 255f, 255f);

		private uint _npcDebugLineColor = 4294901760U;

		private uint _chairDebugLineColor = 4278255360U;

		private uint _animalDebugLineColor = 4279356620U;

		private uint _semivalidChairDebugLineColor = 4294963200U;

		private uint _passageDebugLineColor = 4288217241U;

		private uint _missionBoundDebugLineColor = uint.MaxValue;

		private int _totalInvalidPoints;

		private int _currentInvalidPoints;

		private int _disabledFaceId;

		private int _particularfaceID;

		private Dictionary<SpawnPointDebugView.CategoryId, List<SpawnPointDebugView.InvalidPosition>> _invalidSpawnPointsDictionary = new Dictionary<SpawnPointDebugView.CategoryId, List<SpawnPointDebugView.InvalidPosition>>();

		private string allPrefabsWithParticularTag;

		private IList<SpawnPointUnits> _spUnitsList = new List<SpawnPointUnits>();

		private enum CategoryId
		{
			NPC,
			Animal,
			Chair,
			Passage,
			OutOfMissionBound,
			SemivalidChair
		}

		private struct InvalidPosition
		{
			public Vec3 position;

			public GameEntity entity;

			public bool isDisabledNavMesh;

			public bool doNotShowWarning;
		}
	}
}
