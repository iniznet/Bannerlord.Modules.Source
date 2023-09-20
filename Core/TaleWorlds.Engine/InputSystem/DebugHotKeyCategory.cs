using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.Engine.InputSystem
{
	public class DebugHotKeyCategory : GameKeyContext
	{
		public DebugHotKeyCategory()
			: base("Debug", 0, GameKeyContext.GameKeyContextType.AuxiliaryNotSerialized)
		{
			this.RegisterDebugHotkey("CraftingScreenResetVariable", InputKey.Home, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("LeftMouseButton", InputKey.LeftMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("RightMouseButton", InputKey.RightMouseButton, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Score", InputKey.O, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Copy", InputKey.C, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Numpad0", InputKey.Numpad0, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Numpad1", InputKey.Numpad1, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("NumpadArrowBackward", InputKey.Numpad2, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Numpad3", InputKey.Numpad3, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("NumpadArrowLeft", InputKey.Numpad4, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Numpad5", InputKey.Numpad5, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("NumpadArrowRight", InputKey.Numpad6, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Numpad7", InputKey.Numpad7, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("NumpadArrowForward", InputKey.Numpad8, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Numpad9", InputKey.Numpad9, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Reset", InputKey.R, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("SiegeDeploymentViewHotkeyFinishDeployment", InputKey.P, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MapScreenHotkeyShowPos", InputKey.T, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Y", InputKey.Y, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("E", InputKey.E, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("J", InputKey.J, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("H", InputKey.H, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F1", InputKey.F1, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F2", InputKey.F2, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F3", InputKey.F3, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F4", InputKey.F4, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F5", InputKey.F5, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F6", InputKey.F6, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F7", InputKey.F7, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F8", InputKey.F8, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F9", InputKey.F9, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F10", InputKey.F10, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F11", InputKey.F11, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenHotkeyShowDebug", InputKey.F12, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("ToggleUI", InputKey.F10, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenHotkeyExtendedDebugKey", InputKey.K, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugAgentTeleportMissionControllerHotkeyDisableScriptedMovement", InputKey.Home, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("EngineInterfaceHotkeyTakeScreenShot", InputKey.Insert, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenIncreaseTotalUploadLimit", InputKey.PageDown, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenHotkeyIncreaseTotalUploadLimit", InputKey.PageUp, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenHotkeyDecreaseRulerDistanceFromPivot", InputKey.PageDown, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenHotkeyIncreaseRulerDistanceFromPivot", InputKey.PageUp, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionHotkeySetDebugPathEndPos", InputKey.X, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionHotkeyRenderCombatCollisionCapsules", InputKey.V, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("T", InputKey.T, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("K", InputKey.K, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("U", InputKey.U, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionHotkeySetDebugPathStartPos", InputKey.Z, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("A", InputKey.A, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("B", InputKey.B, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("C", InputKey.C, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("AiTestMissionControllerHotkeySpawnFormation", InputKey.M, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("N", InputKey.N, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Q", InputKey.Q, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Space", InputKey.Space, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("F", InputKey.F, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("W", InputKey.W, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("S", InputKey.S, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D", InputKey.D, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("UpArrow", InputKey.Up, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("LeftArrow", InputKey.Left, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("RightArrow", InputKey.Right, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DownArrow", InputKey.Down, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D0", InputKey.D0, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D1", InputKey.D1, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D2", InputKey.D2, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D3", InputKey.D3, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D4", InputKey.D4, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D5", InputKey.D5, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D6", InputKey.D6, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D7", InputKey.D7, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D8", InputKey.D8, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("D9", InputKey.D9, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("ChangeEnemyTeam", InputKey.NumpadSlash, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("AnimationTestControllerHotkeyUseWeaponTesting", InputKey.L, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("SwapToEnemy", InputKey.E, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("BaseBattleMissionControllerHotkeyBecomePlayer", InputKey.P, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("BaseBattleMissionControllerHotkeyDrawNavMeshLines", InputKey.N, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("ModuleHotkeyOpenDebug", InputKey.D, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("TabbedPanelHotkeyDecreaseSelectedIndex", InputKey.Tab, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("TabbedPanelHotkeyIncreaseSelectedIndex", InputKey.Tab, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Paste", InputKey.V, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Cut", InputKey.X, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionSingleplayerUiHandlerHotkeyUpdateItems", InputKey.U, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionSingleplayerUiHandlerHotkeyJoinEnemyTeam", InputKey.Numpad9, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MapScreenHotkeySwitchCampaignTrueSight", InputKey.T, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("SiegeDeploymentViewHotkeyTeleportMainAgent", InputKey.LeftMouseButton, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("SoftwareOcclusionCheckerHotkeySaveOcclusionImage", InputKey.NumpadSlash, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("UIExtendedDebugKey", InputKey.G, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MapScreenHotkeyOpenEncyclopedia", InputKey.E, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyCamDebugAndAdjustEnabled", InputKey.L, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FaceGeneratorExtendedDebugKey", InputKey.F, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyNumpad0", InputKey.Numpad0, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyNumpad1", InputKey.Numpad1, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyNumpad2", InputKey.Numpad2, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyNumpad3", InputKey.Numpad3, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyNumpad4", InputKey.Numpad4, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyNumpad5", InputKey.Numpad5, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyNumpad6", InputKey.Numpad6, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyNumpad7", InputKey.Numpad7, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyNumpad8", InputKey.Numpad8, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyNumpad9", InputKey.Numpad9, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeyResetFaceToDefault", InputKey.D0, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionExtendedDebugKey", InputKey.Period, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyChargeSide", InputKey.Numpad9, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyToggleSide", InputKey.NumpadPeriod, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyToggleFactionBackward", InputKey.Numpad1, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyToggleFactionForward", InputKey.Numpad2, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyToggleTroopForward", InputKey.Numpad5, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyToggleTroopBackward", InputKey.Numpad4, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyIncreaseSpawnCount", InputKey.Numpad8, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyDecreaseSpawnCount", InputKey.Numpad7, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeySpawnCustom", InputKey.Numpad0, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyOrderLooseAndInfantryFormation", InputKey.Numpad1, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyOrderScatterAndRangedFormation", InputKey.Numpad2, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyOrderSkeinAndCavalryFormation", InputKey.Numpad3, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyOrderLineAndHorseArcherFormation", InputKey.Numpad4, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionExtendedDebugKey2", InputKey.S, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyOrderCircle", InputKey.Numpad5, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyOrderColumn", InputKey.Numpad6, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyOrderShieldWall", InputKey.Numpad7, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FormationTestMissionControllerHotkeyOrderSquare", InputKey.Numpad8, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CustomCameraMissionViewHotkeyIncreaseCustomCameraIndex", InputKey.Y, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeyEnableRuler", InputKey.R, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeyEnableRulerPoint1", InputKey.D1, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeyEnableRulerPoint2", InputKey.D2, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeySwitchSelectedPieceMovement", InputKey.M, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeySetSelectedVariableIndexZero", InputKey.Numpad1, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeySetSelectedVariableIndexOne", InputKey.Numpad2, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeySetSelectedVariableIndexTwo", InputKey.Numpad3, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeySelectPieceZero", InputKey.D1, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeySelectPieceOne", InputKey.D2, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeySelectPieceTwo", InputKey.D3, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("CraftingScreenHotkeySelectPieceThree", InputKey.D4, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeySetFaceKeyMin", InputKey.D1, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeySetFaceKeyMax", InputKey.D2, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("Refresh", InputKey.R, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeySetCurFaceKeyToMin", InputKey.D1, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MbFaceGeneratorScreenHotkeySetCurFaceKeyToMax", InputKey.D2, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenHotkeyFixCamera", InputKey.Home, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("ReplayCaptureLogicHotkeyRenderWithScreenshot", InputKey.Numpad1, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("EnterEditMode", InputKey.E, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenHotkeyIncrementArtificialLag", InputKey.L, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenHotkeyIncrementArtificialLoss", InputKey.J, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenHotkeyResetDebugVariables", InputKey.X, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("FixSkeletons", InputKey.K, HotKey.Modifiers.Shift | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionScreenHotkeySwitchCameraSmooth", InputKey.S, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugAgentTeleportMissionControllerHotkeyTeleportMainAgent", InputKey.MiddleMouseButton, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyKillAI", InputKey.Numpad1, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyOpenMissionDebug", InputKey.PageUp, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyKillDefender", InputKey.Numpad2, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyKillAttacker", InputKey.Numpad3, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyKillMainAgent", InputKey.Numpad9, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyAttackingAiAgent", InputKey.LeftMouseButton, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyDefendingAiAgent", InputKey.RightMouseButton, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyNormalAiAgent", InputKey.MiddleMouseButton, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyAiAgentSideZero", InputKey.Left, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyAiAgentSideOne", InputKey.Right, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyAiAgentSideTwo", InputKey.Up, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyAiAgentSideThree", InputKey.Down, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("UsableMachineAiBaseHotkeyShowMachineUsers", InputKey.Up, HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("UsableMachineAiBaseHotkeyRetreatScriptActive", InputKey.Numpad7, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("UsableMachineAiBaseHotkeyRetreatScriptPassive", InputKey.Numpad7, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MissionDebugHandlerHotkeyColorEnemyTeam", InputKey.NumpadMultiply, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyAimAtBallistas", InputKey.Numpad3, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyAimAtBallistas2", InputKey.Numpad3, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyAimAtMangonels", InputKey.Numpad4, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyAimAtMangonels2", InputKey.Numpad4, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyAimAtBattlements", InputKey.Numpad5, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyTargetDebugActive", InputKey.Numpad8, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyTargetDebugDisactive", InputKey.Numpad8, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyAimAtNone", InputKey.Numpad0, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyAimAtNone2", InputKey.Numpad0, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyAimAtRam", InputKey.Numpad1, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugSiegeBehaviorHotkeyAimAtSt", InputKey.Numpad2, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugNetworkEventStatisticsHotkeyToggleActive", InputKey.F6, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugNetworkEventStatisticsHotkeyClear", InputKey.M, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugNetworkEventStatisticsHotkeyDumpDataAndClear", InputKey.N, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugNetworkEventStatisticsHotkeyDumpData", InputKey.J, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugNetworkEventStatisticsHotkeyClearReplicationData", InputKey.V, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugNetworkEventStatisticsHotkeyDumpReplicationData", InputKey.C, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugNetworkEventStatisticsHotkeyDumpAndClearReplicationData", InputKey.B, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DebugCustomBattlePredefinedSettings1", InputKey.F1, HotKey.Modifiers.Control, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("DisableParallelSettlementPositionUpdate", InputKey.M, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("OpenUIEditor", InputKey.U, HotKey.Modifiers.Alt, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("MapScreenPrintMultiLineText", InputKey.Minus, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("LeaveWhileInConversation", InputKey.Tab, HotKey.Modifiers.None, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("ShowHighlightsSummary", InputKey.H, HotKey.Modifiers.Shift, HotKey.Modifiers.None);
			this.RegisterDebugHotkey("ResetMusicParameters", InputKey.R, HotKey.Modifiers.Shift | HotKey.Modifiers.Alt | HotKey.Modifiers.Control, HotKey.Modifiers.None);
		}

		private void RegisterDebugHotkey(string id, InputKey hotkeyKey, HotKey.Modifiers modifiers, HotKey.Modifiers negativeModifiers = HotKey.Modifiers.None)
		{
			base.RegisterHotKey(new HotKey(id, "Debug", hotkeyKey, modifiers, negativeModifiers), true);
		}

		public const string CategoryId = "Debug";

		public const string LeftMouseButton = "LeftMouseButton";

		public const string RightMouseButton = "RightMouseButton";

		public const string SelectAll = "SelectAll";

		public const string Redo = "Redo";

		public const string Undo = "Undo";

		public const string Copy = "Copy";

		public const string Score = "Score";

		public const string SetOriginToZero = "SetOriginToZero";

		public const string TestEngineCrash = "TestEngineCrash";

		public const string AgentHotkeySwitchRender = "AgentHotkeySwitchRender";

		public const string AgentHotkeySwitchFaceAnimationDebug = "AgentHotkeySwitchFaceAnimationDebug";

		public const string AgentHotkeyCheckCollisionCapsule = "AgentHotkeyCheckCollisionCapsule";

		public const string EngineInterfaceHotkeyWireframe = "EngineInterfaceHotkeyWireframe";

		public const string EngineInterfaceHotkeyWireframe2 = "EngineInterfaceHotkeyWireframe2";

		public const string EngineInterfaceHotkeyTakeScreenShot = "EngineInterfaceHotkeyTakeScreenShot";

		public const string EditingManagerHotkeyCrashReporting = "EditingManagerHotkeyCrashReporting";

		public const string EditingManagerHotkeyEmergencySceneSaving = "EditingManagerHotkeyEmergencySceneSaving";

		public const string EditingManagerHotkeyAssertTestEntityOperations = "EditingManagerHotkeyAssertTestEntityOperations";

		public const string EditingManagerHotkeyUpdateSceneDialog = "EditingManagerHotkeyUpdateSceneDialog";

		public const string EditingManagerHotkeySetTriadToWorld = "EditingManagerHotkeySetTriadToWorld";

		public const string EditingManagerHotkeySetTriadToLocal = "EditingManagerHotkeySetTriadToLocal";

		public const string EditingManagerHotkeySetTriadToScreen = "EditingManagerHotkeySetTriadToScreen";

		public const string EditingManagerHotkeyCameraSmoothMode = "EditingManagerHotkeyCameraSmoothMode";

		public const string EditingManagerHotkeyDisplayNormalsOfSelectedEntities = "EditingManagerHotkeyDisplayNormalsOfSelectedEntities";

		public const string EditingManagerHotkeyChoosePhysicsMaterial = "EditingManagerHotkeyChoosePhysicsMaterial";

		public const string EditingManagerHotkeySwitchObjectsLockedForSelection = "EditingManagerHotkeySwitchObjectsLockedForSelection";

		public const string ApplicationHotkeyAnimationReload = "ApplicationHotkeyAnimationReload";

		public const string ApplicationHotkeyIncreasePingDelay = "ApplicationHotkeyIncreasePingDelay";

		public const string ApplicationHotkeyIncreaseLossRatio = "ApplicationHotkeyIncreaseLossRatio";

		public const string ApplicationHotkeySaveAllContentFilesWithType = "ApplicationHotkeySaveAllContentFilesWithType";

		public const string MissionHotkeySwitchAnimationDebugSystem = "MissionHotkeySwitchAnimationDebugSystem";

		public const string MissionHotkeyAssignMainAgentToDebugAgent = "MissionHotkeyAssignMainAgentToDebugAgent";

		public const string MissionHotkeyUseProgrammerSound = "MissionHotkeyUseProgrammerSound";

		public const string MissionHotkeySetDebugPathStartPos = "MissionHotkeySetDebugPathStartPos";

		public const string MissionHotkeySetDebugPathEndPos = "MissionHotkeySetDebugPathEndPos";

		public const string MissionHotkeyRenderCombatCollisionCapsules = "MissionHotkeyRenderCombatCollisionCapsules";

		public const string ModelviewerHotkeyApplyUpwardsForce = "ModelviewerHotkeyApplyUpwardsForce";

		public const string ModelviewerHotkeyApplyDownwardsForce = "ModelviewerHotkeyApplyDownwardsForce";

		public const string NavigationMeshBuilderHotkeyMakeFourLastVerticesFace = "NavigationMeshBuilderHotkeyMakeFourLastVerticesFace";

		public const string CameraControllerHotkeyMoveForward = "CameraControllerHotkeyMoveForward";

		public const string CameraControllerHotkeyMoveBackward = "CameraControllerHotkeyMoveBackward";

		public const string CameraControllerHotkeyMoveLeft = "CameraControllerHotkeyMoveLeft";

		public const string CameraControllerHotkeyMoveRight = "CameraControllerHotkeyMoveRight";

		public const string CameraControllerHotkeyMoveUpward = "CameraControllerHotkeyMoveUpward";

		public const string CameraControllerHotkeyMoveDownward = "CameraControllerHotkeyMoveDownward";

		public const string CameraControllerHotkeyPenCamera = "CameraControllerHotkeyPenCamera";

		public const string ClothSimulationHotkeyResetAllMeshes = "ClothSimulationHotkeyResetAllMeshes";

		public const string EngineInterfaceHotkeySwitchForwardPhysicxDebugMode = "EngineInterfaceHotkeySwitchForwardPhysicxDebugMode";

		public const string EngineInterfaceHotkeySwitchBackwardPhysicxDebugMode = "EngineInterfaceHotkeySwitchBackwardPhysicxDebugMode";

		public const string EngineInterfaceHotkeyShowPhysicsDebugInfo = "EngineInterfaceHotkeyShowPhysicsDebugInfo";

		public const string EngineInterfaceHotkeyShowProfileModes = "EngineInterfaceHotkeyShowProfileModes";

		public const string EngineInterfaceHotkeyShowDebugInfo = "EngineInterfaceHotkeyShowDebugInfo";

		public const string EngineInterfaceHotkeyDecreaseByTenDrawOneByOneIndex = "EngineInterfaceHotkeyDecreaseByTenDrawOneByOneIndex";

		public const string EngineInterfaceHotkeyIncreaseByTenDrawOneByOneIndex = "EngineInterfaceHotkeyIncreaseByTenDrawOneByOneIndex";

		public const string EngineInterfaceHotkeyDecreaseDrawOneByOneIndex = "EngineInterfaceHotkeyDecreaseDrawOneByOneIndex";

		public const string EngineInterfaceHotkeyIncreaseDrawOneByOneIndex = "EngineInterfaceHotkeyIncreaseDrawOneByOneIndex";

		public const string EngineInterfaceHotkeyForceSetDrawOneByOneIndexMinusone = "EngineInterfaceHotkeyForceSetDrawOneByOneIndexMinusone";

		public const string EngineInterfaceHotkeySetDrawOneByOneIndexMinusone = "EngineInterfaceHotkeySetDrawOneByOneIndexMinusone";

		public const string EngineInterfaceHotkeyReleaseUnusedMemory = "EngineInterfaceHotkeyReleaseUnusedMemory";

		public const string EngineInterfaceHotkeyChangeShaderVisualizationMode = "EngineInterfaceHotkeyChangeShaderVisualizationMode";

		public const string EngineInterfaceHotkeyOnlyRenderDeferredQuad = "EngineInterfaceHotkeyOnlyRenderDeferredQuad";

		public const string EngineInterfaceHotkeyOnlyRenderNonDeferredMeshes = "EngineInterfaceHotkeyOnlyRenderNonDeferredMeshes";

		public const string EngineInterfaceHotkeyChangeAnimationDebugMode = "EngineInterfaceHotkeyChangeAnimationDebugMode";

		public const string EngineInterfaceHotkeyTestAssertReport = "EngineInterfaceHotkeyTestAssertReport";

		public const string EngineInterfaceHotkeyTestCreateBugReportTask = "EngineInterfaceHotkeyTestCreateBugReportTask";

		public const string EngineInterfaceHotkeySlowmotion = "EngineInterfaceHotkeySlowmotion";

		public const string EngineInterfaceHotkeyRecompileShader = "EngineInterfaceHotkeyRecompileShader";

		public const string EngineInterfaceHotkeyToggleConsole = "EngineInterfaceHotkeyToggleConsole";

		public const string EngineInterfaceHotkeyShowConsoleManager = "EngineInterfaceHotkeyShowConsoleManager";

		public const string EngineInterfaceHotkeyShowDebugTools = "EngineInterfaceHotkeyShowDebugTools";

		public const string SceneHotkeyIncreaseEnforcedSkyboxIndex = "SceneHotkeyIncreaseEnforcedSkyboxIndex";

		public const string SceneHotkeyDecreaseEnforcedSkyboxIndex = "SceneHotkeyDecreaseEnforcedSkyboxIndex";

		public const string SceneHotkeyCheckBoundingBoxCorrectness = "SceneHotkeyCheckBoundingBoxCorrectness";

		public const string SceneHotkeyShowNavigationMeshIds = "SceneHotkeyShowNavigationMeshIds";

		public const string SceneHotkeyShowNavigationMeshIdsXray = "SceneHotkeyShowNavigationMeshIdsXray";

		public const string SceneHotkeyShowNavigationMeshIslands = "SceneHotkeyShowNavigationMeshIslands";

		public const string SceneHotkeySetNewCharacterDetailModifier = "SceneHotkeySetNewCharacterDetailModifier";

		public const string SceneHotkeyShowTerrainMaterials = "SceneHotkeyShowTerrainMaterials";

		public const string SceneViewHotkeyTakeHighQualityScreenshot = "SceneViewHotkeyTakeHighQualityScreenshot";

		public const string SoundManagerHotkeyReloadSounds = "SoundManagerHotkeyReloadSounds";

		public const string ReplayEditorHotkeyRenderSounds = "ReplayEditorHotkeyRenderSounds";

		public const string FrameMoveTaskHotkeyUseTelemetryProfiler = "FrameMoveTaskHotkeyUseTelemetryProfiler";

		public const string SkeletonHotkeyActivateDisableAnimationFpsOptimization = "SkeletonHotkeyActivateDisableAnimationFpsOptimization";

		public const string SkeletonHotkeyDisactiveDisableAnimationFpsOptimization = "SkeletonHotkeyDisactiveDisableAnimationFpsOptimization";

		public const string LibraryHotkeyDisableCommitChanges = "LibraryHotkeyDisableCommitChanges";

		public const string Numpad0 = "Numpad0";

		public const string Numpad1 = "Numpad1";

		public const string Numpad3 = "Numpad3";

		public const string Numpad5 = "Numpad5";

		public const string Numpad7 = "Numpad7";

		public const string Numpad9 = "Numpad9";

		public const string D0 = "D0";

		public const string D1 = "D1";

		public const string D2 = "D2";

		public const string D3 = "D3";

		public const string D4 = "D4";

		public const string D5 = "D5";

		public const string D6 = "D6";

		public const string D7 = "D7";

		public const string D8 = "D8";

		public const string D9 = "D9";

		public const string F1 = "F1";

		public const string F2 = "F2";

		public const string F3 = "F3";

		public const string F4 = "F4";

		public const string F5 = "F5";

		public const string F6 = "F6";

		public const string F7 = "F7";

		public const string F8 = "F8";

		public const string F9 = "F9";

		public const string F10 = "F10";

		public const string F11 = "F11";

		public const string Y = "Y";

		public const string A = "A";

		public const string F = "F";

		public const string B = "B";

		public const string N = "N";

		public const string C = "C";

		public const string E = "E";

		public const string J = "J";

		public const string Q = "Q";

		public const string H = "H";

		public const string W = "W";

		public const string S = "S";

		public const string U = "U";

		public const string T = "T";

		public const string K = "K";

		public const string M = "M";

		public const string G = "G";

		public const string D = "D";

		public const string Space = "Space";

		public const string UpArrow = "UpArrow";

		public const string LeftArrow = "LeftArrow";

		public const string DownArrow = "DownArrow";

		public const string RightArrow = "RightArrow";

		public const string NumpadArrowForward = "NumpadArrowForward";

		public const string NumpadArrowBackward = "NumpadArrowBackward";

		public const string NumpadArrowLeft = "NumpadArrowLeft";

		public const string NumpadArrowRight = "NumpadArrowRight";

		public const string SwapToEnemy = "SwapToEnemy";

		public const string ChangeEnemyTeam = "ChangeEnemyTeam";

		public const string Paste = "Paste";

		public const string Cut = "Cut";

		public const string Refresh = "Refresh";

		public const string EnterEditMode = "EnterEditMode";

		public const string FixSkeletons = "FixSkeletons";

		public const string Reset = "Reset";

		public const string AnimationTestControllerHotkeyUseWeaponTesting = "AnimationTestControllerHotkeyUseWeaponTesting";

		public const string BaseBattleMissionControllerHotkeyBecomePlayer = "BaseBattleMissionControllerHotkeyBecomePlayer";

		public const string BaseBattleMissionControllerHotkeyDrawNavMeshLines = "BaseBattleMissionControllerHotkeyDrawNavMeshLines";

		public const string ModuleHotkeyOpenDebug = "ModuleHotkeyOpenDebug";

		public const string FormationTestMissionControllerHotkeyChargeSide = "FormationTestMissionControllerHotkeyChargeSide";

		public const string FormationTestMissionControllerHotkeyToggleSide = "FormationTestMissionControllerHotkeyToggleSide";

		public const string FormationTestMissionControllerHotkeyToggleFactionBackward = "FormationTestMissionControllerHotkeyToggleFactionBackward";

		public const string FormationTestMissionControllerHotkeyToggleFactionForward = "FormationTestMissionControllerHotkeyToggleFactionForward";

		public const string FormationTestMissionControllerHotkeyToggleTroopForward = "FormationTestMissionControllerHotkeyToggleTroopForward";

		public const string FormationTestMissionControllerHotkeyToggleTroopBackward = "FormationTestMissionControllerHotkeyToggleTroopBackward";

		public const string FormationTestMissionControllerHotkeyIncreaseSpawnCount = "FormationTestMissionControllerHotkeyIncreaseSpawnCount";

		public const string FormationTestMissionControllerHotkeyDecreaseSpawnCount = "FormationTestMissionControllerHotkeyDecreaseSpawnCount";

		public const string FormationTestMissionControllerHotkeySpawnCustom = "FormationTestMissionControllerHotkeySpawnCustom";

		public const string FormationTestMissionControllerHotkeyOrderLooseAndInfantryFormation = "FormationTestMissionControllerHotkeyOrderLooseAndInfantryFormation";

		public const string FormationTestMissionControllerHotkeyOrderScatterAndRangedFormation = "FormationTestMissionControllerHotkeyOrderScatterAndRangedFormation";

		public const string FormationTestMissionControllerHotkeyOrderSkeinAndCavalryFormation = "FormationTestMissionControllerHotkeyOrderSkeinAndCavalryFormation";

		public const string FormationTestMissionControllerHotkeyOrderLineAndHorseArcherFormation = "FormationTestMissionControllerHotkeyOrderLineAndHorseArcherFormation";

		public const string FormationTestMissionControllerHotkeyOrderCircle = "FormationTestMissionControllerHotkeyOrderCircle";

		public const string FormationTestMissionControllerHotkeyOrderColumn = "FormationTestMissionControllerHotkeyOrderColumn";

		public const string FormationTestMissionControllerHotkeyOrderShieldWall = "FormationTestMissionControllerHotkeyOrderShieldWall";

		public const string FormationTestMissionControllerHotkeyOrderSquare = "FormationTestMissionControllerHotkeyOrderSquare";

		public const string AiTestMissionControllerHotkeySpawnFormation = "AiTestMissionControllerHotkeySpawnFormation";

		public const string TabbedPanelHotkeyDecreaseSelectedIndex = "TabbedPanelHotkeyDecreaseSelectedIndex";

		public const string TabbedPanelHotkeyIncreaseSelectedIndex = "TabbedPanelHotkeyIncreaseSelectedIndex";

		public const string MissionSingleplayerUiHandlerHotkeyUpdateItems = "MissionSingleplayerUiHandlerHotkeyUpdateItems";

		public const string MissionSingleplayerUiHandlerHotkeyJoinEnemyTeam = "MissionSingleplayerUiHandlerHotkeyJoinEnemyTeam";

		public const string SiegeDeploymentViewHotkeyTeleportMainAgent = "SiegeDeploymentViewHotkeyTeleportMainAgent";

		public const string SiegeDeploymentViewHotkeyFinishDeployment = "SiegeDeploymentViewHotkeyFinishDeployment";

		public const string CraftingScreenHotkeyEnableRuler = "CraftingScreenHotkeyEnableRuler";

		public const string CraftingScreenHotkeyEnableRulerPoint1 = "CraftingScreenHotkeyEnableRulerPoint1";

		public const string CraftingScreenHotkeyEnableRulerPoint2 = "CraftingScreenHotkeyEnableRulerPoint2";

		public const string CraftingScreenHotkeySwitchSelectedPieceMovement = "CraftingScreenHotkeySwitchSelectedPieceMovement";

		public const string CraftingScreenHotkeySetSelectedVariableIndexZero = "CraftingScreenHotkeySetSelectedVariableIndexZero";

		public const string CraftingScreenHotkeySetSelectedVariableIndexOne = "CraftingScreenHotkeySetSelectedVariableIndexOne";

		public const string CraftingScreenHotkeySetSelectedVariableIndexTwo = "CraftingScreenHotkeySetSelectedVariableIndexTwo";

		public const string CraftingScreenHotkeySelectPieceZero = "CraftingScreenHotkeySelectPieceZero";

		public const string CraftingScreenHotkeySelectPieceOne = "CraftingScreenHotkeySelectPieceOne";

		public const string CraftingScreenHotkeySelectPieceTwo = "CraftingScreenHotkeySelectPieceTwo";

		public const string CraftingScreenHotkeySelectPieceThree = "CraftingScreenHotkeySelectPieceThree";

		public const string MbFaceGeneratorScreenHotkeyCamDebugAndAdjustEnabled = "MbFaceGeneratorScreenHotkeyCamDebugAndAdjustEnabled";

		public const string MbFaceGeneratorScreenHotkeyNumpad0 = "MbFaceGeneratorScreenHotkeyNumpad0";

		public const string MbFaceGeneratorScreenHotkeyNumpad1 = "MbFaceGeneratorScreenHotkeyNumpad1";

		public const string MbFaceGeneratorScreenHotkeyNumpad2 = "MbFaceGeneratorScreenHotkeyNumpad2";

		public const string MbFaceGeneratorScreenHotkeyNumpad3 = "MbFaceGeneratorScreenHotkeyNumpad3";

		public const string MbFaceGeneratorScreenHotkeyNumpad4 = "MbFaceGeneratorScreenHotkeyNumpad4";

		public const string MbFaceGeneratorScreenHotkeyNumpad5 = "MbFaceGeneratorScreenHotkeyNumpad5";

		public const string MbFaceGeneratorScreenHotkeyNumpad6 = "MbFaceGeneratorScreenHotkeyNumpad6";

		public const string MbFaceGeneratorScreenHotkeyNumpad7 = "MbFaceGeneratorScreenHotkeyNumpad7";

		public const string MbFaceGeneratorScreenHotkeyNumpad8 = "MbFaceGeneratorScreenHotkeyNumpad8";

		public const string MbFaceGeneratorScreenHotkeyNumpad9 = "MbFaceGeneratorScreenHotkeyNumpad9";

		public const string MbFaceGeneratorScreenHotkeyResetFaceToDefault = "MbFaceGeneratorScreenHotkeyResetFaceToDefault";

		public const string MbFaceGeneratorScreenHotkeySetFaceKeyMax = "MbFaceGeneratorScreenHotkeySetFaceKeyMax";

		public const string MbFaceGeneratorScreenHotkeySetFaceKeyMin = "MbFaceGeneratorScreenHotkeySetFaceKeyMin";

		public const string MbFaceGeneratorScreenHotkeySetCurFaceKeyToMax = "MbFaceGeneratorScreenHotkeySetCurFaceKeyToMax";

		public const string MbFaceGeneratorScreenHotkeySetCurFaceKeyToMin = "MbFaceGeneratorScreenHotkeySetCurFaceKeyToMin";

		public const string SoftwareOcclusionCheckerHotkeySaveOcclusionImage = "SoftwareOcclusionCheckerHotkeySaveOcclusionImage";

		public const string MapScreenHotkeySwitchCampaignTrueSight = "MapScreenHotkeySwitchCampaignTrueSight";

		public const string MapScreenPrintMultiLineText = "MapScreenPrintMultiLineText";

		public const string MapScreenHotkeyShowPos = "MapScreenHotkeyShowPos";

		public const string MapScreenHotkeyOpenEncyclopedia = "MapScreenHotkeyOpenEncyclopedia";

		public const string ReplayCaptureLogicHotkeyRenderWithScreenshot = "ReplayCaptureLogicHotkeyRenderWithScreenshot";

		public const string MissionScreenHotkeyFixCamera = "MissionScreenHotkeyFixCamera";

		public const string MissionScreenHotkeyIncrementArtificialLag = "MissionScreenHotkeyIncrementArtificialLag";

		public const string MissionScreenHotkeyIncrementArtificialLoss = "MissionScreenHotkeyIncrementArtificialLoss";

		public const string MissionScreenHotkeyResetDebugVariables = "MissionScreenHotkeyResetDebugVariables";

		public const string MissionScreenHotkeySwitchCameraSmooth = "MissionScreenHotkeySwitchCameraSmooth";

		public const string MissionScreenHotkeyIncreaseFirstFormationWidth = "MissionScreenHotkeyIncreaseFirstFormationWidth";

		public const string MissionScreenHotkeyDecreaseFirstFormationWidth = "MissionScreenHotkeyDecreaseFirstFormationWidth";

		public const string MissionScreenHotkeyExtendedDebugKey = "MissionScreenHotkeyExtendedDebugKey";

		public const string MissionScreenHotkeyShowDebug = "MissionScreenHotkeyShowDebug";

		public const string MissionScreenHotkeyIncreaseTotalUploadLimit = "MissionScreenHotkeyIncreaseTotalUploadLimit";

		public const string MissionScreenIncreaseTotalUploadLimit = "MissionScreenIncreaseTotalUploadLimit";

		public const string MissionScreenHotkeyDecreaseRulerDistanceFromPivot = "MissionScreenHotkeyDecreaseRulerDistanceFromPivot";

		public const string MissionScreenHotkeyIncreaseRulerDistanceFromPivot = "MissionScreenHotkeyIncreaseRulerDistanceFromPivot";

		public const string DebugAgentTeleportMissionControllerHotkeyTeleportMainAgent = "DebugAgentTeleportMissionControllerHotkeyTeleportMainAgent";

		public const string DebugAgentTeleportMissionControllerHotkeyDisableScriptedMovement = "DebugAgentTeleportMissionControllerHotkeyDisableScriptedMovement";

		public const string MissionDebugHandlerHotkeyKillAI = "MissionDebugHandlerHotkeyKillAI";

		public const string MissionDebugHandlerHotkeyKillAttacker = "MissionDebugHandlerHotkeyKillAttacker";

		public const string MissionDebugHandlerHotkeyKillDefender = "MissionDebugHandlerHotkeyKillDefender";

		public const string MissionDebugHandlerHotkeyKillMainAgent = "MissionDebugHandlerHotkeyKillMainAgent";

		public const string MissionDebugHandlerHotkeyAttackingAiAgent = "MissionDebugHandlerHotkeyAttackingAiAgent";

		public const string MissionDebugHandlerHotkeyDefendingAiAgent = "MissionDebugHandlerHotkeyDefendingAiAgent";

		public const string MissionDebugHandlerHotkeyNormalAiAgent = "MissionDebugHandlerHotkeyNormalAiAgent";

		public const string MissionDebugHandlerHotkeyAiAgentSideZero = "MissionDebugHandlerHotkeyAiAgentSideZero";

		public const string MissionDebugHandlerHotkeyAiAgentSideOne = "MissionDebugHandlerHotkeyAiAgentSideOne";

		public const string MissionDebugHandlerHotkeyAiAgentSideTwo = "MissionDebugHandlerHotkeyAiAgentSideTwo";

		public const string MissionDebugHandlerHotkeyAiAgentSideThree = "MissionDebugHandlerHotkeyAiAgentSideThree";

		public const string MissionDebugHandlerHotkeyColorEnemyTeam = "MissionDebugHandlerHotkeyColorEnemyTeam";

		public const string MissionDebugHandlerHotkeyOpenMissionDebug = "MissionDebugHandlerHotkeyOpenMissionDebug";

		public const string UsableMachineAiBaseHotkeyShowMachineUsers = "UsableMachineAiBaseHotkeyShowMachineUsers";

		public const string UsableMachineAiBaseHotkeyRetreatScriptActive = "UsableMachineAiBaseHotkeyRetreatScriptActive";

		public const string UsableMachineAiBaseHotkeyRetreatScriptPassive = "UsableMachineAiBaseHotkeyRetreatScriptPassive";

		public const string CustomCameraMissionViewHotkeyIncreaseCustomCameraIndex = "CustomCameraMissionViewHotkeyIncreaseCustomCameraIndex";

		public const string DebugSiegeBehaviorHotkeyAimAtBallistas = "DebugSiegeBehaviorHotkeyAimAtBallistas";

		public const string DebugSiegeBehaviorHotkeyAimAtMangonels = "DebugSiegeBehaviorHotkeyAimAtMangonels";

		public const string DebugSiegeBehaviorHotkeyAimAtBattlements = "DebugSiegeBehaviorHotkeyAimAtBattlements";

		public const string DebugSiegeBehaviorHotkeyAimAtNone = "DebugSiegeBehaviorHotkeyAimAtNone";

		public const string DebugSiegeBehaviorHotkeyAimAtNone2 = "DebugSiegeBehaviorHotkeyAimAtNone2";

		public const string DebugSiegeBehaviorHotkeyTargetDebugActive = "DebugSiegeBehaviorHotkeyTargetDebugActive";

		public const string DebugSiegeBehaviorHotkeyTargetDebugDisactive = "DebugSiegeBehaviorHotkeyTargetDebugDisactive";

		public const string DebugSiegeBehaviorHotkeyAimAtRam = "DebugSiegeBehaviorHotkeyAimAtRam";

		public const string DebugSiegeBehaviorHotkeyAimAtSt = "DebugSiegeBehaviorHotkeyAimAtSt";

		public const string DebugSiegeBehaviorHotkeyAimAtBallistas2 = "DebugSiegeBehaviorHotkeyAimAtBallistas2";

		public const string DebugSiegeBehaviorHotkeyAimAtMangonels2 = "DebugSiegeBehaviorHotkeyAimAtMangonels2";

		public const string DebugNetworkEventStatisticsHotkeyClear = "DebugNetworkEventStatisticsHotkeyClear";

		public const string DebugNetworkEventStatisticsHotkeyDumpDataAndClear = "DebugNetworkEventStatisticsHotkeyDumpDataAndClear";

		public const string DebugNetworkEventStatisticsHotkeyDumpData = "DebugNetworkEventStatisticsHotkeyDumpData";

		public const string DebugNetworkEventStatisticsHotkeyClearReplicationData = "DebugNetworkEventStatisticsHotkeyClearReplicationData";

		public const string DebugNetworkEventStatisticsHotkeyDumpReplicationData = "DebugNetworkEventStatisticsHotkeyDumpReplicationData";

		public const string DebugNetworkEventStatisticsHotkeyDumpAndClearReplicationData = "DebugNetworkEventStatisticsHotkeyDumpAndClearReplicationData";

		public const string DebugNetworkEventStatisticsHotkeyToggleActive = "DebugNetworkEventStatisticsHotkeyToggleActive";

		public const string AiSelectDebugAgent1 = "AiSelectDebugAgent1";

		public const string AiSelectDebugAgent2 = "AiSelectDebugAgent2";

		public const string AiClearDebugAgents = "AiClearDebugAgents";

		public const string DebugCustomBattlePredefinedSettings1 = "DebugCustomBattlePredefinedSettings1";

		public const string CraftingScreenResetVariable = "CraftingScreenResetVariable";

		public const string DisableParallelSettlementPositionUpdate = "DisableParallelSettlementPositionUpdate";

		public const string OpenUIEditor = "OpenUIEditor";

		public const string ToggleUI = "ToggleUI";

		public const string LeaveWhileInConversation = "LeaveWhileInConversation";

		public const string ShowHighlightsSummary = "ShowHighlightsSummary";

		public const string ResetMusicParameters = "ResetMusicParameters";

		public const string UIExtendedDebugKey = "UIExtendedDebugKey";

		public const string FaceGeneratorExtendedDebugKey = "FaceGeneratorExtendedDebugKey";

		public const string FormationTestMissionExtendedDebugKey = "FormationTestMissionExtendedDebugKey";

		public const string FormationTestMissionExtendedDebugKey2 = "FormationTestMissionExtendedDebugKey2";
	}
}
