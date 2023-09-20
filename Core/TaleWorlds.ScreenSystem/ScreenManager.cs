using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.ScreenSystem
{
	public static class ScreenManager
	{
		public static IScreenManagerEngineConnection EngineInterface
		{
			get
			{
				return ScreenManager._engineInterface;
			}
		}

		public static float Scale { get; private set; } = 1f;

		public static Vec2 UsableArea
		{
			get
			{
				return ScreenManager._usableArea;
			}
			private set
			{
				if (value != ScreenManager._usableArea)
				{
					ScreenManager._usableArea = value;
					ScreenManager.OnUsableAreaChanged(ScreenManager._usableArea);
				}
			}
		}

		public static bool IsEnterButtonRDown
		{
			get
			{
				return ScreenManager._engineInterface.GetIsEnterButtonRDown();
			}
		}

		public static event ScreenManager.OnPushScreenEvent OnPushScreen;

		public static event ScreenManager.OnPopScreenEvent OnPopScreen;

		public static event ScreenManager.OnControllerDisconnectedEvent OnControllerDisconnected;

		public static List<ScreenLayer> SortedLayers
		{
			get
			{
				if (!ScreenManager._isSortedActiveLayersDirty)
				{
					int count = ScreenManager._sortedLayers.Count;
					ScreenBase topScreen = ScreenManager.TopScreen;
					int? num = ((topScreen != null) ? new int?(topScreen.Layers.Count) : null);
					ObservableCollection<GlobalLayer> globalLayers = ScreenManager._globalLayers;
					int? num2 = num + ((globalLayers != null) ? new int?(globalLayers.Count) : null);
					if ((count == num2.GetValueOrDefault()) & (num2 != null))
					{
						goto IL_145;
					}
				}
				ScreenManager._isMouseInputActiveLastFrame = false;
				ScreenManager._sortedLayers.Clear();
				if (ScreenManager.TopScreen != null)
				{
					for (int i = 0; i < ScreenManager.TopScreen.Layers.Count; i++)
					{
						ScreenLayer screenLayer = ScreenManager.TopScreen.Layers[i];
						if (screenLayer != null)
						{
							ScreenManager._sortedLayers.Add(screenLayer);
						}
					}
				}
				foreach (GlobalLayer globalLayer in ScreenManager._globalLayers)
				{
					ScreenManager._sortedLayers.Add(globalLayer.Layer);
				}
				ScreenManager._sortedLayers.Sort();
				ScreenManager._isSortedActiveLayersDirty = false;
				IL_145:
				return ScreenManager._sortedLayers;
			}
		}

		public static ScreenBase TopScreen { get; private set; }

		public static ScreenLayer FocusedLayer { get; private set; }

		public static ScreenLayer FirstHitLayer { get; private set; }

		static ScreenManager()
		{
			ScreenManager._screenList.CollectionChanged += ScreenManager.OnScreenListChanged;
			ScreenManager._globalLayers.CollectionChanged += ScreenManager.OnGlobalListChanged;
			ScreenManager.FocusedLayer = null;
			ScreenManager.FirstHitLayer = null;
		}

		public static void Initialize(IScreenManagerEngineConnection engineInterface)
		{
			ScreenManager._engineInterface = engineInterface;
		}

		internal static void RefreshGlobalOrder()
		{
			if (!ScreenManager._isRefreshActive)
			{
				ScreenManager._isRefreshActive = true;
				int num = -2000;
				int num2 = 10000;
				for (int i = 0; i < ScreenManager.SortedLayers.Count; i++)
				{
					if (ScreenManager.SortedLayers[i] != null)
					{
						if (!ScreenManager.SortedLayers[i].Finalized)
						{
							ScreenLayer screenLayer = ScreenManager.SortedLayers[i];
							if (screenLayer != null && screenLayer.IsActive)
							{
								ScreenLayer screenLayer2 = ScreenManager.SortedLayers[i];
								if (screenLayer2 != null)
								{
									screenLayer2.RefreshGlobalOrder(ref num);
								}
							}
							else
							{
								ScreenLayer screenLayer3 = ScreenManager.SortedLayers[i];
								if (screenLayer3 != null)
								{
									screenLayer3.RefreshGlobalOrder(ref num2);
								}
							}
						}
						ScreenManager._globalOrderDirty = false;
					}
				}
				ScreenManager._isRefreshActive = false;
			}
		}

		public static void RemoveGlobalLayer(GlobalLayer layer)
		{
			Debug.Print("RemoveGlobalLayer", 0, Debug.DebugColor.White, 17592186044416UL);
			ScreenManager._globalLayers.Remove(layer);
			layer.Layer.HandleDeactivate();
			ScreenManager._globalOrderDirty = true;
		}

		public static void AddGlobalLayer(GlobalLayer layer, bool isFocusable)
		{
			Debug.Print("AddGlobalLayer", 0, Debug.DebugColor.White, 17592186044416UL);
			int num = ScreenManager._globalLayers.Count;
			for (int i = 0; i < ScreenManager._globalLayers.Count; i++)
			{
				if (ScreenManager._globalLayers[i].Layer.InputRestrictions.Order >= layer.Layer.InputRestrictions.Order)
				{
					num = i;
					break;
				}
			}
			ScreenManager._globalLayers.Insert(num, layer);
			layer.Layer.HandleActivate();
			ScreenManager._globalOrderDirty = true;
		}

		public static void OnConstrainStateChanged(bool isConstrained)
		{
			Debug.Print("OnConstrainStateChanged: " + isConstrained.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
			ScreenManager.OnGameWindowFocusChange(!isConstrained);
		}

		public static bool ScreenTypeExistsAtList(ScreenBase screen)
		{
			Type type = screen.GetType();
			using (IEnumerator<ScreenBase> enumerator = ScreenManager._screenList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetType() == type)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void UpdateLayout()
		{
			foreach (GlobalLayer globalLayer in ScreenManager._globalLayers)
			{
				globalLayer.UpdateLayout();
			}
			foreach (ScreenBase screenBase in ScreenManager._screenList)
			{
				screenBase.UpdateLayout();
			}
		}

		public static void SetSuspendLayer(ScreenLayer layer, bool isSuspended)
		{
			if (isSuspended)
			{
				layer.HandleDeactivate();
			}
			else
			{
				layer.HandleActivate();
			}
			layer.LastActiveState = !isSuspended;
		}

		public static void OnFinalize()
		{
			ScreenManager.DeactivateAndFinalizeAllScreens();
			ScreenManager._screenList.CollectionChanged -= ScreenManager.OnScreenListChanged;
			ScreenManager._globalLayers.CollectionChanged -= ScreenManager.OnGlobalListChanged;
			ScreenManager._screenList = null;
			ScreenManager._globalLayers = null;
			ScreenManager.FocusedLayer = null;
		}

		private static void DeactivateAndFinalizeAllScreens()
		{
			Debug.Print("DeactivateAndFinalizeAllScreens", 0, Debug.DebugColor.White, 17592186044416UL);
			for (int i = ScreenManager._screenList.Count - 1; i >= 0; i--)
			{
				ScreenManager._screenList[i].HandlePause();
			}
			for (int j = ScreenManager._screenList.Count - 1; j >= 0; j--)
			{
				ScreenManager._screenList[j].HandleDeactivate();
			}
			for (int k = ScreenManager._screenList.Count - 1; k >= 0; k--)
			{
				ScreenManager._screenList[k].HandleFinalize();
			}
			ScreenManager._screenList.Clear();
			Common.MemoryCleanupGC(false);
		}

		internal static void UpdateLateTickLayers(List<ScreenLayer> layers)
		{
			ScreenManager._lateTickLayers = layers;
		}

		public static void Tick(float dt, bool activeMouseVisible)
		{
			for (int i = 0; i < ScreenManager._globalLayers.Count; i++)
			{
				GlobalLayer globalLayer = ScreenManager._globalLayers[i];
				if (globalLayer != null)
				{
					globalLayer.EarlyTick(dt);
				}
			}
			ScreenManager.Update();
			ScreenManager._lateTickLayers = null;
			if (ScreenManager.TopScreen != null)
			{
				ScreenManager.TopScreen.FrameTick(dt);
				ScreenBase screenBase = ScreenManager.FindPredecessor(ScreenManager.TopScreen);
				if (screenBase != null)
				{
					screenBase.IdleTick(dt);
				}
			}
			for (int j = 0; j < ScreenManager._globalLayers.Count; j++)
			{
				GlobalLayer globalLayer2 = ScreenManager._globalLayers[j];
				if (globalLayer2 != null)
				{
					globalLayer2.Tick(dt);
				}
			}
			ScreenManager.LateUpdate(dt, activeMouseVisible);
			ScreenManager.ShowScreenDebugInformation();
		}

		public static void LateTick(float dt)
		{
			if (ScreenManager._lateTickLayers != null)
			{
				for (int i = 0; i < ScreenManager._lateTickLayers.Count; i++)
				{
					if (!ScreenManager._lateTickLayers[i].Finalized)
					{
						ScreenManager._lateTickLayers[i].LateTick(dt);
					}
				}
				ScreenManager._lateTickLayers.Clear();
			}
			for (int j = 0; j < ScreenManager._globalLayers.Count; j++)
			{
				ScreenManager._globalLayers[j].LateTick(dt);
			}
		}

		public static void OnPlatformScreenKeyboardRequested(string initialText, string descriptionText, int maxLength, int keyboardTypeEnum)
		{
			Action<string, string, int, int> platformTextRequested = ScreenManager.PlatformTextRequested;
			if (platformTextRequested == null)
			{
				return;
			}
			platformTextRequested(initialText, descriptionText, maxLength, keyboardTypeEnum);
		}

		public static void OnOnscreenKeyboardDone(string inputText)
		{
			ScreenLayer focusedLayer = ScreenManager.FocusedLayer;
			if (focusedLayer == null)
			{
				return;
			}
			focusedLayer.OnOnScreenKeyboardDone(inputText);
		}

		public static void OnOnscreenKeyboardCanceled()
		{
			ScreenLayer focusedLayer = ScreenManager.FocusedLayer;
			if (focusedLayer == null)
			{
				return;
			}
			focusedLayer.OnOnScreenKeyboardCanceled();
		}

		public static void OnGameWindowFocusChange(bool focusGained)
		{
			Debug.Print("OnGameWindowFocusChange: " + focusGained.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
			string text = "TopScreen: ";
			ScreenBase topScreen = ScreenManager.TopScreen;
			string text2;
			if (topScreen == null)
			{
				text2 = null;
			}
			else
			{
				Type type = topScreen.GetType();
				text2 = ((type != null) ? type.Name : null);
			}
			Debug.Print(text + text2, 0, Debug.DebugColor.White, 17592186044416UL);
			bool flag = false;
			if (!Debugger.IsAttached && !flag)
			{
				ScreenBase topScreen2 = ScreenManager.TopScreen;
				if (topScreen2 != null)
				{
					topScreen2.OnFocusChangeOnGameWindow(focusGained);
				}
			}
			if (focusGained)
			{
				Action focusGained2 = ScreenManager.FocusGained;
				if (focusGained2 == null)
				{
					return;
				}
				focusGained2();
			}
		}

		public static event Action FocusGained;

		public static event Action<string, string, int, int> PlatformTextRequested;

		public static void ReplaceTopScreen(ScreenBase screen)
		{
			Debug.Print("ReplaceToTopScreen", 0, Debug.DebugColor.White, 17592186044416UL);
			if (ScreenManager._screenList.Count > 0)
			{
				ScreenManager.TopScreen.HandlePause();
				ScreenManager.TopScreen.HandleDeactivate();
				ScreenManager.TopScreen.HandleFinalize();
				ScreenManager.OnPopScreenEvent onPopScreen = ScreenManager.OnPopScreen;
				if (onPopScreen != null)
				{
					onPopScreen(ScreenManager.TopScreen);
				}
				ScreenManager._screenList.Remove(ScreenManager.TopScreen);
			}
			ScreenManager._screenList.Add(screen);
			screen.HandleInitialize();
			screen.HandleActivate();
			screen.HandleResume();
			ScreenManager._globalOrderDirty = true;
			ScreenManager.OnPushScreenEvent onPushScreen = ScreenManager.OnPushScreen;
			if (onPushScreen == null)
			{
				return;
			}
			onPushScreen(screen);
		}

		public static List<ScreenLayer> GetPersistentInputRestrictions()
		{
			List<ScreenLayer> list = new List<ScreenLayer>();
			foreach (GlobalLayer globalLayer in ScreenManager._globalLayers)
			{
				list.Add(globalLayer.Layer);
			}
			return list;
		}

		public static void SetAndActivateRootScreen(ScreenBase screen)
		{
			Debug.Print("SetAndActivateRootScreen", 0, Debug.DebugColor.White, 17592186044416UL);
			if (ScreenManager.TopScreen != null)
			{
				throw new Exception("TopScreen is not null.");
			}
			ScreenManager._screenList.Add(screen);
			screen.HandleInitialize();
			screen.HandleActivate();
			screen.HandleResume();
			ScreenManager._globalOrderDirty = true;
			ScreenManager.OnPushScreenEvent onPushScreen = ScreenManager.OnPushScreen;
			if (onPushScreen == null)
			{
				return;
			}
			onPushScreen(screen);
		}

		public static void CleanAndPushScreen(ScreenBase screen)
		{
			Debug.Print("CleanAndPushScreen", 0, Debug.DebugColor.White, 17592186044416UL);
			ScreenManager.DeactivateAndFinalizeAllScreens();
			ScreenManager._screenList.Add(screen);
			screen.HandleInitialize();
			screen.HandleActivate();
			screen.HandleResume();
			ScreenManager._globalOrderDirty = true;
			ScreenManager.OnPushScreenEvent onPushScreen = ScreenManager.OnPushScreen;
			if (onPushScreen == null)
			{
				return;
			}
			onPushScreen(screen);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("cb_clear_siege_machine_selection", "ui")]
		public static string ClearSiegeMachineSelection(List<string> args)
		{
			ScreenBase screenBase = ScreenManager._screenList.FirstOrDefault((ScreenBase x) => x.GetType().GetMethod("ClearSiegeMachineSelections") != null);
			if (screenBase != null)
			{
				screenBase.GetType().GetMethod("ClearSiegeMachineSelections").Invoke(screenBase, null);
			}
			return "Siege machine selections have been cleared.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("cb_copy_battle_layout_to_clipboard", "ui")]
		public static string CopyCustomBattle(List<string> args)
		{
			ScreenBase screenBase = ScreenManager._screenList.FirstOrDefault((ScreenBase x) => x.GetType().GetMethod("CopyBattleLayoutToClipboard") != null);
			if (screenBase != null)
			{
				screenBase.GetType().GetMethod("CopyBattleLayoutToClipboard").Invoke(screenBase, null);
				return "Custom battle layout has been copied to clipboard as text.";
			}
			return "Something went wrong";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("cb_apply_battle_layout_from_string", "ui")]
		public static string ApplyCustomBattleLayout(List<string> args)
		{
			ScreenBase screenBase = ScreenManager._screenList.FirstOrDefault((ScreenBase x) => x.GetType().GetMethod("ApplyCustomBattleLayout") != null);
			if (screenBase == null || args.Count <= 0)
			{
				return "Something went wrong.";
			}
			string text = args.Aggregate((string i, string j) => i + " " + j);
			if (text.Count<char>() > 5)
			{
				screenBase.GetType().GetMethod("ApplyCustomBattleLayout").Invoke(screenBase, new object[] { text });
				return "Applied new layout from text.";
			}
			return "Argument is not right.";
		}

		public static void PushScreen(ScreenBase screen)
		{
			Debug.Print("PushScreen", 0, Debug.DebugColor.White, 17592186044416UL);
			if (ScreenManager._screenList.Count > 0)
			{
				ScreenManager.TopScreen.HandlePause();
				if (ScreenManager.TopScreen.IsActive)
				{
					ScreenManager.TopScreen.HandleDeactivate();
				}
			}
			ScreenManager._screenList.Add(screen);
			screen.HandleInitialize();
			screen.HandleActivate();
			screen.HandleResume();
			ScreenManager._globalOrderDirty = true;
			ScreenManager.OnPushScreenEvent onPushScreen = ScreenManager.OnPushScreen;
			if (onPushScreen == null)
			{
				return;
			}
			onPushScreen(screen);
		}

		public static void PopScreen()
		{
			Debug.Print("PopScreen", 0, Debug.DebugColor.White, 17592186044416UL);
			if (ScreenManager._screenList.Count > 0)
			{
				ScreenManager.TopScreen.HandlePause();
				ScreenManager.TopScreen.HandleDeactivate();
				ScreenManager.TopScreen.HandleFinalize();
				Debug.Print("PopScreen - " + ScreenManager.TopScreen.GetType().ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
				ScreenManager.OnPopScreenEvent onPopScreen = ScreenManager.OnPopScreen;
				if (onPopScreen != null)
				{
					onPopScreen(ScreenManager.TopScreen);
				}
				ScreenManager._screenList.Remove(ScreenManager.TopScreen);
			}
			if (ScreenManager._screenList.Count > 0)
			{
				ScreenBase topScreen = ScreenManager.TopScreen;
				ScreenManager.TopScreen.HandleActivate();
				if (topScreen == ScreenManager.TopScreen)
				{
					ScreenManager.TopScreen.HandleResume();
				}
			}
			ScreenManager._globalOrderDirty = true;
		}

		public static void CleanScreens()
		{
			Debug.Print("CleanScreens", 0, Debug.DebugColor.White, 17592186044416UL);
			while (ScreenManager._screenList.Count > 0)
			{
				ScreenManager.TopScreen.HandlePause();
				ScreenManager.TopScreen.HandleDeactivate();
				ScreenManager.TopScreen.HandleFinalize();
				ScreenManager.OnPopScreenEvent onPopScreen = ScreenManager.OnPopScreen;
				if (onPopScreen != null)
				{
					onPopScreen(ScreenManager.TopScreen);
				}
				ScreenManager._screenList.Remove(ScreenManager.TopScreen);
			}
			ScreenManager._globalOrderDirty = true;
		}

		private static ScreenBase FindPredecessor(ScreenBase screen)
		{
			ScreenBase screenBase = null;
			int num = ScreenManager._screenList.IndexOf(screen);
			if (num > 0)
			{
				screenBase = ScreenManager._screenList[num - 1];
			}
			return screenBase;
		}

		public static void Update(IReadOnlyList<int> lastKeysPressed)
		{
			ScreenManager._lastPressedKeys = lastKeysPressed;
			ScreenBase topScreen = ScreenManager.TopScreen;
			if (topScreen != null && topScreen.IsActive)
			{
				ScreenManager.TopScreen.Update(ScreenManager._lastPressedKeys);
			}
			for (int i = 0; i < ScreenManager._globalLayers.Count; i++)
			{
				GlobalLayer globalLayer = ScreenManager._globalLayers[i];
				if (globalLayer.Layer.IsActive)
				{
					globalLayer.Update(ScreenManager._lastPressedKeys);
				}
			}
		}

		private static bool? GetMouseInput()
		{
			bool flag = false;
			if (Input.IsKeyDown(InputKey.LeftMouseButton) || Input.IsKeyDown(InputKey.RightMouseButton) || Input.IsKeyDown(InputKey.MiddleMouseButton) || Input.IsKeyDown(InputKey.X1MouseButton) || Input.IsKeyDown(InputKey.X2MouseButton) || Input.IsKeyDown(ScreenManager.IsEnterButtonRDown ? InputKey.ControllerRDown : InputKey.ControllerRRight))
			{
				flag = true;
			}
			if (!ScreenManager._isMouseInputActiveLastFrame && flag)
			{
				flag = true;
			}
			else
			{
				if (!ScreenManager._isMouseInputActiveLastFrame || flag)
				{
					return null;
				}
				flag = false;
			}
			ScreenManager._isMouseInputActiveLastFrame = flag;
			return new bool?(flag);
		}

		public static void EarlyUpdate(Vec2 usableArea)
		{
			ScreenManager.UsableArea = usableArea;
			ScreenManager.RefreshGlobalOrder();
			InputType inputType = InputType.None;
			for (int i = 0; i < ScreenManager.SortedLayers.Count; i++)
			{
				ScreenLayer screenLayer = ScreenManager.SortedLayers[i];
				if (screenLayer != null && screenLayer.IsActive)
				{
					ScreenManager.SortedLayers[i].MouseEnabled = true;
				}
			}
			bool? mouseInput = ScreenManager.GetMouseInput();
			for (int j = ScreenManager.SortedLayers.Count - 1; j >= 0; j--)
			{
				ScreenLayer screenLayer2 = ScreenManager.SortedLayers[j];
				if (screenLayer2 != null && screenLayer2.IsActive && !screenLayer2.Finalized)
				{
					bool? flag = null;
					bool? flag2 = mouseInput;
					bool flag3 = false;
					if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
					{
						flag = new bool?(false);
					}
					InputType inputType2 = InputType.None;
					InputUsageMask inputUsageMask = screenLayer2.InputUsageMask;
					screenLayer2.ScreenOrderInLastFrame = j;
					screenLayer2.IsHitThisFrame = false;
					if (screenLayer2.HitTest())
					{
						if (ScreenManager.FirstHitLayer == null)
						{
							ScreenManager.FirstHitLayer = screenLayer2;
							ScreenManager._engineInterface.ActivateMouseCursor(screenLayer2.ActiveCursor);
						}
						if (!inputType.HasAnyFlag(InputType.MouseButton) && inputUsageMask.HasAnyFlag(InputUsageMask.MouseButtons))
						{
							flag = mouseInput;
							inputType2 |= InputType.MouseButton;
							inputType |= InputType.MouseButton;
							screenLayer2.IsHitThisFrame = true;
						}
						if (!inputType.HasAnyFlag(InputType.MouseWheel) && inputUsageMask.HasAnyFlag(InputUsageMask.MouseWheels))
						{
							inputType2 |= InputType.MouseWheel;
							inputType |= InputType.MouseWheel;
							screenLayer2.IsHitThisFrame = true;
						}
					}
					if (!inputType.HasAnyFlag(InputType.Key) && ScreenManager.FocusTest(screenLayer2))
					{
						inputType2 |= InputType.Key;
						inputType |= InputType.Key;
					}
					screenLayer2.EarlyProcessEvents(inputType2, flag);
				}
			}
		}

		private static void Update()
		{
			int num = 0;
			for (int i = 0; i < ScreenManager.SortedLayers.Count; i++)
			{
				if (ScreenManager.SortedLayers[i].IsActive)
				{
					num++;
				}
			}
			if (ScreenManager._sortedActiveLayersCopyForUpdate.Length < num)
			{
				ScreenManager._sortedActiveLayersCopyForUpdate = new ScreenLayer[num];
			}
			int num2 = 0;
			for (int j = 0; j < ScreenManager.SortedLayers.Count; j++)
			{
				ScreenLayer screenLayer = ScreenManager.SortedLayers[j];
				if (screenLayer.IsActive)
				{
					ScreenManager._sortedActiveLayersCopyForUpdate[num2] = screenLayer;
					num2++;
				}
			}
			for (int k = num2 - 1; k >= 0; k--)
			{
				ScreenLayer screenLayer2 = ScreenManager._sortedActiveLayersCopyForUpdate[k];
				if (!screenLayer2.Finalized)
				{
					screenLayer2.ProcessEvents();
				}
			}
			for (int l = 0; l < ScreenManager._sortedActiveLayersCopyForUpdate.Length; l++)
			{
				ScreenManager._sortedActiveLayersCopyForUpdate[l] = null;
			}
		}

		private static void LateUpdate(float dt, bool activeMouseVisible)
		{
			for (int i = 0; i < ScreenManager.SortedLayers.Count; i++)
			{
				ScreenLayer screenLayer = ScreenManager.SortedLayers[i];
				if (screenLayer != null && screenLayer.IsActive)
				{
					screenLayer.LateProcessEvents();
				}
			}
			for (int j = 0; j < ScreenManager.SortedLayers.Count; j++)
			{
				ScreenLayer screenLayer2 = ScreenManager.SortedLayers[j];
				if (screenLayer2 != null && screenLayer2.IsActive)
				{
					screenLayer2.OnLateUpdate(dt);
					if (screenLayer2 != ScreenManager.FocusedLayer || ScreenManager._focusedLayerChangedThisFrame)
					{
						screenLayer2.Input.ResetLastDownKeys();
					}
				}
			}
			if (!ScreenManager._focusedLayerChangedThisFrame)
			{
				ScreenLayer focusedLayer = ScreenManager.FocusedLayer;
				if (focusedLayer != null)
				{
					InputContext input = focusedLayer.Input;
					if (input != null)
					{
						input.UpdateLastDownKeys();
					}
				}
			}
			ScreenManager._focusedLayerChangedThisFrame = false;
			ScreenManager.FirstHitLayer = null;
			ScreenManager.UpdateMouseVisibility(activeMouseVisible);
			if (ScreenManager._globalOrderDirty)
			{
				ScreenManager.RefreshGlobalOrder();
			}
		}

		internal static void UpdateMouseVisibility(bool activeMouseVisible)
		{
			for (int i = 0; i < ScreenManager.SortedLayers.Count; i++)
			{
				ScreenLayer screenLayer = ScreenManager.SortedLayers[i];
				if (screenLayer.IsActive && screenLayer.InputRestrictions.MouseVisibility)
				{
					if (!ScreenManager._activeMouseVisible)
					{
						ScreenManager.SetMouseVisible(true);
					}
					return;
				}
			}
			if (ScreenManager._activeMouseVisible)
			{
				ScreenManager.SetMouseVisible(false);
			}
		}

		public static bool IsControllerActive()
		{
			return Input.IsControllerConnected && Input.IsGamepadActive && !Input.IsMouseActive && ScreenManager._engineInterface.GetMouseVisible();
		}

		public static bool IsMouseCursorHidden()
		{
			return !Input.IsMouseActive && ScreenManager._engineInterface.GetMouseVisible();
		}

		public static bool IsMouseCursorActive()
		{
			return Input.IsMouseActive && ScreenManager._engineInterface.GetMouseVisible();
		}

		public static bool IsLayerBlockedAtPosition(ScreenLayer layer, Vector2 position)
		{
			for (int i = ScreenManager.SortedLayers.Count - 1; i >= 0; i--)
			{
				ScreenLayer screenLayer = ScreenManager.SortedLayers[i];
				if (layer == screenLayer)
				{
					return false;
				}
				if (screenLayer != null && screenLayer.IsActive && !screenLayer.Finalized && screenLayer.HitTest(position))
				{
					if (screenLayer.InputUsageMask.HasAnyFlag(InputUsageMask.MouseButtons))
					{
						return layer != ScreenManager.SortedLayers[i];
					}
					if (screenLayer.InputUsageMask.HasAnyFlag(InputUsageMask.MouseWheels))
					{
						return layer != ScreenManager.SortedLayers[i];
					}
				}
			}
			return false;
		}

		private static void SetMouseVisible(bool value)
		{
			ScreenManager._activeMouseVisible = value;
			ScreenManager._engineInterface.SetMouseVisible(value);
		}

		public static bool GetMouseVisibility()
		{
			return ScreenManager._activeMouseVisible;
		}

		public static void TrySetFocus(ScreenLayer layer)
		{
			if (ScreenManager.FocusedLayer != null && ScreenManager.FocusedLayer.InputRestrictions.Order > layer.InputRestrictions.Order && layer.IsActive)
			{
				return;
			}
			if (!layer.IsFocusLayer && !layer.FocusTest())
			{
				return;
			}
			if (ScreenManager.FocusedLayer != layer)
			{
				ScreenManager._focusedLayerChangedThisFrame = true;
				if (ScreenManager.FocusedLayer != null)
				{
					ScreenManager.FocusedLayer.OnLoseFocus();
				}
			}
			ScreenManager.FocusedLayer = layer;
		}

		public static void TryLoseFocus(ScreenLayer layer)
		{
			if (ScreenManager.FocusedLayer != layer)
			{
				return;
			}
			ScreenLayer focusedLayer = ScreenManager.FocusedLayer;
			if (focusedLayer != null)
			{
				focusedLayer.OnLoseFocus();
			}
			for (int i = ScreenManager.SortedLayers.Count - 1; i >= 0; i--)
			{
				ScreenLayer screenLayer = ScreenManager.SortedLayers[i];
				if (screenLayer.IsActive && screenLayer.IsFocusLayer && layer != screenLayer)
				{
					ScreenManager.FocusedLayer = screenLayer;
					ScreenManager._focusedLayerChangedThisFrame = true;
					return;
				}
			}
			ScreenManager.FocusedLayer = null;
		}

		private static bool FocusTest(ScreenLayer layer)
		{
			if (Input.IsGamepadActive && layer.InputRestrictions.CanOverrideFocusOnHit)
			{
				return layer.IsHitThisFrame;
			}
			return ScreenManager.FocusedLayer == layer;
		}

		public static void OnScaleChange(float newScale)
		{
			ScreenManager.Scale = newScale;
			foreach (GlobalLayer globalLayer in ScreenManager._globalLayers)
			{
				globalLayer.UpdateLayout();
			}
			foreach (ScreenBase screenBase in ScreenManager._screenList)
			{
				screenBase.UpdateLayout();
			}
		}

		public static void OnControllerDisconnect()
		{
			ScreenManager.OnControllerDisconnectedEvent onControllerDisconnected = ScreenManager.OnControllerDisconnected;
			if (onControllerDisconnected == null)
			{
				return;
			}
			onControllerDisconnected();
		}

		private static void OnScreenListChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Debug.Print("OnScreenListChanged", 0, Debug.DebugColor.White, 17592186044416UL);
			ScreenManager._isSortedActiveLayersDirty = true;
			ObservableCollection<ScreenBase> screenList = ScreenManager._screenList;
			if (screenList != null && screenList.Count > 0)
			{
				if (ScreenManager.TopScreen != null)
				{
					ScreenManager.TopScreen.OnAddLayer -= ScreenManager.OnLayerAddedToTopLayer;
					ScreenManager.TopScreen.OnRemoveLayer -= ScreenManager.OnLayerRemovedFromTopLayer;
				}
				ScreenManager.TopScreen = ScreenManager._screenList[ScreenManager._screenList.Count - 1];
				if (ScreenManager.TopScreen != null)
				{
					ScreenManager.TopScreen.OnAddLayer += ScreenManager.OnLayerAddedToTopLayer;
					ScreenManager.TopScreen.OnRemoveLayer += ScreenManager.OnLayerRemovedFromTopLayer;
				}
			}
			else
			{
				if (ScreenManager.TopScreen != null)
				{
					ScreenManager.TopScreen.OnAddLayer -= ScreenManager.OnLayerAddedToTopLayer;
					ScreenManager.TopScreen.OnRemoveLayer -= ScreenManager.OnLayerRemovedFromTopLayer;
				}
				ScreenManager.TopScreen = null;
			}
			ScreenManager._isSortedActiveLayersDirty = true;
		}

		private static void OnLayerAddedToTopLayer(ScreenLayer layer)
		{
			ScreenManager._isSortedActiveLayersDirty = true;
		}

		private static void OnLayerRemovedFromTopLayer(ScreenLayer layer)
		{
			ScreenManager._isSortedActiveLayersDirty = true;
		}

		private static void OnGlobalListChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ScreenManager._isSortedActiveLayersDirty = true;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("set_screen_debug_information_enabled", "ui")]
		public static string SetScreenDebugInformationEnabled(List<string> args)
		{
			string text = "Usage: ui.set_screen_debug_information_enabled [True/False]";
			if (args.Count != 1)
			{
				return text;
			}
			bool flag;
			if (bool.TryParse(args[0], out flag))
			{
				ScreenManager.SetScreenDebugInformationEnabled(flag);
				return "Success.";
			}
			return text;
		}

		public static void SetScreenDebugInformationEnabled(bool isEnabled)
		{
			ScreenManager._isScreenDebugInformationEnabled = isEnabled;
		}

		private static void ShowScreenDebugInformation()
		{
			if (ScreenManager._isScreenDebugInformationEnabled)
			{
				ScreenManager._engineInterface.BeginDebugPanel("Screen Debug Information");
				for (int i = 0; i < ScreenManager.SortedLayers.Count; i++)
				{
					ScreenLayer screenLayer = ScreenManager.SortedLayers[i];
					if (ScreenManager._engineInterface.DrawDebugTreeNode(string.Format("{0}###{1}.{2}.{3}", new object[]
					{
						screenLayer.GetType().Name,
						screenLayer.Name,
						i,
						screenLayer.Name.GetDeterministicHashCode()
					})))
					{
						screenLayer.DrawDebugInfo();
						ScreenManager._engineInterface.PopDebugTreeNode();
					}
				}
				ScreenManager._engineInterface.EndDebugPanel();
			}
		}

		private static void OnUsableAreaChanged(Vec2 newUsableArea)
		{
			ScreenManager.UpdateLayout();
		}

		private static IScreenManagerEngineConnection _engineInterface;

		private static Vec2 _usableArea = new Vec2(1f, 1f);

		private static List<ScreenLayer> _lateTickLayers;

		private static ObservableCollection<ScreenBase> _screenList = new ObservableCollection<ScreenBase>();

		private static ObservableCollection<GlobalLayer> _globalLayers = new ObservableCollection<GlobalLayer>();

		private static List<ScreenLayer> _sortedLayers = new List<ScreenLayer>(16);

		private static ScreenLayer[] _sortedActiveLayersCopyForUpdate = new ScreenLayer[16];

		private static bool _isSortedActiveLayersDirty = true;

		private static bool _focusedLayerChangedThisFrame;

		private static bool _isMouseInputActiveLastFrame;

		private static bool _isScreenDebugInformationEnabled;

		private static bool _activeMouseVisible = true;

		private static IReadOnlyList<int> _lastPressedKeys;

		private static bool _globalOrderDirty;

		private static bool _isRefreshActive = false;

		public delegate void OnPushScreenEvent(ScreenBase pushedScreen);

		public delegate void OnPopScreenEvent(ScreenBase poppedScreen);

		public delegate void OnControllerDisconnectedEvent();
	}
}
