using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ObjectSystem;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	// Token: 0x02000007 RID: 7
	[GameStateScreen(typeof(CraftingState))]
	public class GauntletCraftingScreen : ScreenBase, ICraftingStateHandler, IGameStateListener
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002B80 File Offset: 0x00000D80
		private SceneView SceneView
		{
			get
			{
				return this._sceneLayer.SceneView;
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002B8D File Offset: 0x00000D8D
		public GauntletCraftingScreen(CraftingState craftingState)
		{
			this._craftingState = craftingState;
			this._craftingState.Handler = this;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002BB4 File Offset: 0x00000DB4
		private void ReloadPieces()
		{
			string key = GauntletCraftingScreen._reloadXmlPath.Key;
			string text = GauntletCraftingScreen._reloadXmlPath.Value;
			if (!text.EndsWith(".xml"))
			{
				text += ".xml";
			}
			GauntletCraftingScreen._reloadXmlPath = new KeyValuePair<string, string>(null, null);
			XmlDocument xmlDocument = Game.Current.ObjectManager.LoadXMLFromFileSkipValidation(ModuleHelper.GetModuleFullPath(key) + "ModuleData/" + text, "");
			if (xmlDocument != null)
			{
				foreach (object obj in xmlDocument.ChildNodes[1].ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlAttributeCollection attributes = xmlNode.Attributes;
					if (attributes != null)
					{
						string innerText = attributes["id"].InnerText;
						CraftingPiece @object = Game.Current.ObjectManager.GetObject<CraftingPiece>(innerText);
						if (@object != null)
						{
							@object.Deserialize(Game.Current.ObjectManager, xmlNode);
						}
					}
				}
				this._craftingState.CraftingLogic.ReIndex(true);
				this.RefreshItemEntity(this._dataSource.IsInCraftingMode);
				this._dataSource.WeaponDesign.RefreshItem();
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002CF8 File Offset: 0x00000EF8
		public void Initialize()
		{
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._craftingCategory = spriteData.SpriteCategories["ui_crafting"];
			this._craftingCategory.Load(resourceContext, uiresourceDepot);
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", false);
			this._gauntletMovie = this._gauntletLayer.LoadMovie("Crafting", this._dataSource);
			this._gauntletLayer.IsFocusLayer = true;
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.InputRestrictions.SetCanOverrideFocusOnHit(true);
			base.AddLayer(this._gauntletLayer);
			this.OpenScene();
			this.RefreshItemEntity(true);
			this._isInitialized = true;
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			game.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(14));
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002DD4 File Offset: 0x00000FD4
		protected override void OnInitialize()
		{
			this.Initialize();
			this._sceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			this._sceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._sceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("CraftingHotkeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("CraftingHotkeyCategory"));
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002E84 File Offset: 0x00001084
		protected override void OnFinalize()
		{
			base.OnFinalize();
			Game game = Game.Current;
			if (game != null)
			{
				game.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
			}
			this.SceneView.ClearAll(true, true);
			this._craftingCategory.Unload();
			CraftingVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnFinalize();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002EDC File Offset: 0x000010DC
		protected override void OnFrameTick(float dt)
		{
			LoadingWindow.DisableGlobalLoadingWindow();
			base.OnFrameTick(dt);
			this._dataSource.CanSwitchTabs = !Input.IsGamepadActive || !InformationManager.GetIsAnyTooltipActiveAndExtended();
			if (this._dataSource.IsInCraftingMode)
			{
				this._dataSource.WeaponDesign.WeaponControlsEnabled = this._sceneLayer.IsHitThisFrame;
			}
			if (this._sceneLayer.Input.IsControlDown() || this._gauntletLayer.Input.IsControlDown())
			{
				if (this._sceneLayer.Input.IsHotKeyPressed("Copy") || this._gauntletLayer.Input.IsHotKeyPressed("Copy"))
				{
					this.CopyXmlCode();
				}
				else if (this._sceneLayer.Input.IsHotKeyPressed("Paste") || this._gauntletLayer.Input.IsHotKeyPressed("Paste"))
				{
					this.PasteXmlCode();
				}
			}
			if (this._craftingState.CraftingLogic.CurrentCraftingTemplate == null)
			{
				return;
			}
			if (!this._sceneLayer.Input.IsHotKeyDown("Rotate") && !this._sceneLayer.Input.IsHotKeyDown("Zoom"))
			{
				this._sceneLayer.InputRestrictions.SetMouseVisibility(true);
			}
			this._craftingScene.Tick(dt);
			if (Input.IsGamepadActive || (!this._gauntletLayer.IsFocusedOnInput() && !this._sceneLayer.IsFocusedOnInput()))
			{
				if (this.IsHotKeyReleasedInAnyLayer("Exit"))
				{
					this._dataSource.ExecuteCancel();
				}
				else if (this.IsHotKeyReleasedInAnyLayer("Confirm"))
				{
					this._dataSource.ExecuteConfirm();
				}
				else if (this._dataSource.CanSwitchTabs)
				{
					if (this.IsHotKeyReleasedInAnyLayer("SwitchToPreviousTab"))
					{
						if (this._dataSource.IsInSmeltingMode)
						{
							this._dataSource.ExecuteSwitchToRefinement();
						}
						else if (this._dataSource.IsInCraftingMode)
						{
							this._dataSource.ExecuteSwitchToSmelting();
						}
						else if (this._dataSource.IsInRefinementMode)
						{
							this._dataSource.ExecuteSwitchToCrafting();
						}
					}
					else if (this.IsHotKeyReleasedInAnyLayer("SwitchToNextTab"))
					{
						if (this._dataSource.IsInSmeltingMode)
						{
							this._dataSource.ExecuteSwitchToCrafting();
						}
						else if (this._dataSource.IsInCraftingMode)
						{
							this._dataSource.ExecuteSwitchToRefinement();
						}
						else if (this._dataSource.IsInRefinementMode)
						{
							this._dataSource.ExecuteSwitchToSmelting();
						}
					}
				}
			}
			bool flag = false;
			if (GauntletCraftingScreen._reloadXmlPath.Key != null && GauntletCraftingScreen._reloadXmlPath.Value != null)
			{
				this.ReloadPieces();
				flag = true;
			}
			if (!flag)
			{
				if (base.DebugInput.IsHotKeyPressed("Reset"))
				{
					this.OnResetCamera();
				}
				if (this._dataSource.IsInCraftingMode)
				{
					float num = 0f;
					float num2 = 0f;
					if (Input.IsGamepadActive)
					{
						num = this._sceneLayer.Input.GetGameKeyAxis("CameraAxisX");
						num2 = this._sceneLayer.Input.GetGameKeyAxis("CameraAxisY");
					}
					else if (this._sceneLayer.Input.IsHotKeyDown("Rotate") || this._sceneLayer.Input.IsHotKeyDown("Zoom"))
					{
						num = this._sceneLayer.Input.GetMouseMoveX();
						num2 = this._sceneLayer.Input.GetMouseMoveY();
					}
					if (num != 0f || num2 != 0f)
					{
						this.OnMouseMove(num, num2, dt);
					}
					this.ZoomTick(dt);
				}
				this._craftingScene.SetDepthOfFieldParameters(this._dofParams.x, this._dofParams.z, false);
				this._craftingScene.SetDepthOfFieldFocus(this._initialEntityFrame.origin.Distance(this._cameraFrame.origin));
				if (this._dataSource.IsInCraftingMode)
				{
					this._craftingEntity.SetFrame(ref this._craftingEntityFrame);
				}
				this.SceneView.SetCamera(this._camera);
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000032C6 File Offset: 0x000014C6
		private void OnClose()
		{
			ICampaignMission campaignMission = CampaignMission.Current;
			if (campaignMission != null)
			{
				campaignMission.EndMission();
			}
			Game.Current.GameStateManager.PopState(0);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000032E8 File Offset: 0x000014E8
		private void OnResetCamera()
		{
			this._sceneLayer.InputRestrictions.SetMouseVisibility(true);
			this.ResetEntityAndCamera();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003301 File Offset: 0x00001501
		private void OnWeaponCrafted()
		{
			WeaponDesignResultPopupVM craftingResultPopup = this._dataSource.WeaponDesign.CraftingResultPopup;
			if (craftingResultPopup == null)
			{
				return;
			}
			craftingResultPopup.SetDoneInputKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetHotKey("Confirm"));
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003334 File Offset: 0x00001534
		public void OnCraftingLogicInitialized()
		{
			CraftingVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = new CraftingVM(this._craftingState.CraftingLogic, new Action(this.OnClose), new Action(this.OnResetCamera), new Action(this.OnWeaponCrafted), new Func<WeaponComponentData, ItemObject.ItemUsageSetFlags>(this.GetItemUsageSetFlag))
			{
				OnItemRefreshed = new CraftingVM.OnItemRefreshedDelegate(this.RefreshItemEntity)
			};
			this._dataSource.WeaponDesign.CraftingHistory.SetDoneKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetHotKey("Confirm"));
			this._dataSource.WeaponDesign.CraftingHistory.SetCancelKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetConfirmInputKey(HotKeyManager.GetCategory("CraftingHotkeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetExitInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetPreviousTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this._dataSource.SetNextTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003478 File Offset: 0x00001678
		public void OnCraftingLogicRefreshed()
		{
			this._dataSource.OnCraftingLogicRefreshed(this._craftingState.CraftingLogic);
			if (this._isInitialized)
			{
				this.RefreshItemEntity(true);
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000034A0 File Offset: 0x000016A0
		private void OpenScene()
		{
			this._craftingScene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
			this._craftingScene.SetName("GauntletCraftingScreen");
			SceneInitializationData sceneInitializationData = default(SceneInitializationData);
			sceneInitializationData.InitPhysicsWorld = false;
			this._craftingScene.Read("crafting_menu_outdoor", ref sceneInitializationData, "");
			this._craftingScene.DisableStaticShadows(true);
			this._craftingScene.SetShadow(true);
			this._craftingScene.SetClothSimulationState(true);
			this.InitializeEntityAndCamera();
			this._sceneLayer = new SceneLayer("SceneLayer", true, true);
			this._sceneLayer.IsFocusLayer = true;
			this._sceneLayer.InputRestrictions.SetCanOverrideFocusOnHit(true);
			base.AddLayer(this._sceneLayer);
			this.SceneView.SetScene(this._craftingScene);
			this.SceneView.SetCamera(this._camera);
			this.SceneView.SetSceneUsesShadows(true);
			this.SceneView.SetAcceptGlobalDebugRenderObjects(true);
			this.SceneView.SetRenderWithPostfx(true);
			this.SceneView.SetResolutionScaling(true);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000035B0 File Offset: 0x000017B0
		private void InitializeEntityAndCamera()
		{
			GameEntity gameEntity = this._craftingScene.FindEntityWithTag("weapon_point");
			MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
			this._craftingScene.RemoveEntity(gameEntity, 114);
			globalFrame.Elevate(1.6f);
			this._craftingEntityFrame = globalFrame;
			this._initialEntityFrame = this._craftingEntityFrame;
			this._craftingEntity = GameEntity.CreateEmpty(this._craftingScene, true);
			this._craftingEntity.SetFrame(ref this._craftingEntityFrame);
			this._camera = Camera.CreateCamera();
			this._dofParams = default(Vec3);
			this._curCamSpeed = new Vec2(0f, 0f);
			GameEntity gameEntity2 = this._craftingScene.FindEntityWithTag("camera_point");
			gameEntity2.GetCameraParamsFromCameraScript(this._camera, ref this._dofParams);
			float fovVertical = this._camera.GetFovVertical();
			float aspectRatio = Screen.AspectRatio;
			float near = this._camera.Near;
			float far = this._camera.Far;
			this._camera.SetFovVertical(fovVertical, aspectRatio, near, far);
			this._craftingScene.SetDepthOfFieldParameters(this._dofParams.x, this._dofParams.z, false);
			this._craftingScene.SetDepthOfFieldFocus(this._dofParams.y);
			this._cameraFrame = gameEntity2.GetFrame();
			this._initialCameraFrame = this._cameraFrame;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003704 File Offset: 0x00001904
		private void RefreshItemEntity(bool isItemVisible)
		{
			this._dataSource.WeaponDesign.CurrentWeaponHasScabbard = false;
			if (this._craftingEntity != null)
			{
				this._craftingEntityFrame = this._craftingEntity.GetFrame();
				this._craftingEntity.Remove(115);
				this._craftingEntity = null;
			}
			if (isItemVisible)
			{
				this._craftingEntity = GameEntity.CreateEmpty(this._craftingScene, true);
				this._craftingEntity.SetFrame(ref this._craftingEntityFrame);
				this._craftedData = this._craftingState.CraftingLogic.CurrentWeaponDesign;
				if (this._craftedData != null)
				{
					this._craftingEntityFrame = this._craftingEntity.GetFrame();
					float num = this._craftedData.CraftedWeaponLength / 2f;
					this._craftingEntity.SetFrame(ref this._craftingEntityFrame);
					BladeData bladeData = this._craftedData.UsedPieces[0].CraftingPiece.BladeData;
					this._dataSource.WeaponDesign.CurrentWeaponHasScabbard = !string.IsNullOrEmpty(bladeData.HolsterMeshName);
					MetaMesh metaMesh;
					if (!this._dataSource.WeaponDesign.IsScabbardVisible)
					{
						metaMesh = CraftedDataView.BuildWeaponMesh(this._craftedData, -num, false, false);
					}
					else
					{
						metaMesh = CraftedDataView.BuildHolsterMeshWithWeapon(this._craftedData, -num, false);
						if (metaMesh == null)
						{
							metaMesh = CraftedDataView.BuildWeaponMesh(this._craftedData, -num, false, false);
						}
					}
					this._craftingEntity = this._craftingScene.AddItemEntity(ref this._craftingEntityFrame, metaMesh);
				}
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003874 File Offset: 0x00001A74
		private void OnMouseMove(float deltaX, float deltaY, float dT)
		{
			if (!base.DebugInput.IsControlDown() && !base.DebugInput.IsAltDown())
			{
				if (Input.IsGamepadActive)
				{
					if (Mathf.Abs(deltaX) > 0.1f)
					{
						deltaX = (deltaX - Mathf.Sign(deltaX) * 0.1f) / 0.9f;
						this._craftingEntityFrame.rotation.RotateAboutUp(2f * deltaX * 3.1415927f / 180f);
					}
					if (Mathf.Abs(deltaY) > 0.1f)
					{
						deltaY = (deltaY - Mathf.Sign(deltaY) * 0.1f) / 0.9f;
						this._craftingEntityFrame.rotation.RotateAboutSide(2f * deltaY * 3.1415927f / 180f);
					}
				}
				else if (this._sceneLayer.Input.IsHotKeyDown("Rotate"))
				{
					Vec2 vec;
					vec..ctor(0.02f, 0.02f);
					Vec2 vec2;
					vec2..ctor(deltaX, -deltaY);
					Vec2 vec3;
					vec3..ctor(vec2.x / vec.x, vec2.y / vec.y);
					Vec2 vec4;
					vec4..ctor(dT * vec3.x, dT * vec3.y);
					float num = 0.95f;
					this._curCamSpeed = this._curCamSpeed * num + vec4;
					Vec2 vec5;
					vec5..ctor(this._curCamSpeed.x * dT, this._curCamSpeed.y * dT);
					this._craftingEntityFrame.rotation.RotateAboutAnArbitraryVector(Vec3.Side, vec5.y * 3.1415927f / 180f);
					this._craftingEntityFrame.rotation.RotateAboutAnArbitraryVector(Vec3.Up, vec5.x * 3.1415927f / 180f);
					MBWindowManager.DontChangeCursorPos();
					this._sceneLayer.InputRestrictions.SetMouseVisibility(false);
					this._gauntletLayer.InputRestrictions.SetMouseVisibility(false);
				}
				else if (this._sceneLayer.Input.IsHotKeyDown("Zoom"))
				{
					float num2 = ((MathF.Abs(deltaX) >= MathF.Abs(deltaY)) ? deltaX : deltaY);
					this._craftingEntityFrame.rotation.RotateAboutUp(num2 * 3.1415927f / 180f * 0.15f);
					MBWindowManager.DontChangeCursorPos();
					this._sceneLayer.InputRestrictions.SetMouseVisibility(false);
					this._gauntletLayer.InputRestrictions.SetMouseVisibility(false);
				}
				if (this._sceneLayer.Input.IsHotKeyDown("Rotate") && this._sceneLayer.Input.IsHotKeyDown("Zoom"))
				{
					this.ResetEntityAndCamera();
				}
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003B10 File Offset: 0x00001D10
		private float GetActiveZoomAmount()
		{
			if (Input.IsGamepadActive)
			{
				float gameKeyState = this._sceneLayer.Input.GetGameKeyState(55);
				float gameKeyState2 = this._sceneLayer.Input.GetGameKeyState(56);
				return gameKeyState - gameKeyState2;
			}
			return MBMath.ClampFloat(this._zoomAmount - (float)MathF.Sign(this._sceneLayer.Input.GetDeltaMouseScroll()) * 0.05f, -0.6f, 0.5f);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003B80 File Offset: 0x00001D80
		private void ZoomTick(float dt)
		{
			this._zoomAmount = this.GetActiveZoomAmount();
			if (MathF.Abs(this._zoomAmount) < 1E-05f)
			{
				this._zoomAmount = 0f;
				return;
			}
			int num = MathF.Sign(this._zoomAmount);
			Vec3 vec = (float)(-(float)num) * (this._initialEntityFrame.origin - this._cameraFrame.origin);
			vec.Normalize();
			float num2 = (Input.IsGamepadActive ? 2f : 5f);
			float num3 = dt * num2;
			this._cameraFrame.origin = this._cameraFrame.origin + vec * num3;
			this._zoomAmount += (float)(-(float)num) * num3;
			float num4 = this._initialEntityFrame.origin.Distance(this._cameraFrame.origin);
			if (num4 > 3.3f)
			{
				this._cameraFrame.origin = this._cameraFrame.origin + (float)(-(float)num) * vec * (num4 - 3.3f);
				num4 = this._initialEntityFrame.origin.Distance(this._cameraFrame.origin);
				this._zoomAmount = 0f;
			}
			else if (num4 < 0.55f)
			{
				this._cameraFrame.origin = this._cameraFrame.origin + (float)(-(float)num) * vec * (num4 - 0.55f);
				num4 = this._initialEntityFrame.origin.Distance(this._cameraFrame.origin);
				this._zoomAmount = 0f;
			}
			else if (num != MathF.Sign(this._zoomAmount))
			{
				this._zoomAmount = 0f;
			}
			this._camera.Frame = this._cameraFrame;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003D4A File Offset: 0x00001F4A
		private void ResetEntityAndCamera()
		{
			this._zoomAmount = 0f;
			this._craftingEntityFrame = this._initialEntityFrame;
			this._cameraFrame = this._initialCameraFrame;
			this._camera.Frame = this._cameraFrame;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003D80 File Offset: 0x00001F80
		private void CopyXmlCode()
		{
			Input.SetClipboardText(this._craftingState.CraftingLogic.GetXmlCodeForCurrentItem(this._craftingState.CraftingLogic.GetCurrentCraftedItemObject(false, null)));
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003DAC File Offset: 0x00001FAC
		private void PasteXmlCode()
		{
			string clipboardText = Input.GetClipboardText();
			if (!string.IsNullOrEmpty(clipboardText))
			{
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(clipboardText);
				if (@object != null)
				{
					this.SwithToCraftedItem(@object);
					return;
				}
				CraftingTemplate craftingTemplate;
				ValueTuple<CraftingPiece, int>[] array;
				if (this._craftingState.CraftingLogic.TryGetWeaponPropertiesFromXmlCode(clipboardText, ref craftingTemplate, ref array))
				{
					this._dataSource.SetCurrentDesignManually(craftingTemplate, array);
				}
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003E04 File Offset: 0x00002004
		private void SwithToCraftedItem(ItemObject itemObject)
		{
			if (itemObject != null && itemObject.IsCraftedWeapon)
			{
				if (!this._dataSource.IsInCraftingMode)
				{
					this._dataSource.ExecuteSwitchToCrafting();
				}
				WeaponDesign weaponDesign = itemObject.WeaponDesign;
				if (this._craftingState.CraftingLogic.CurrentCraftingTemplate != weaponDesign.Template)
				{
					this._dataSource.WeaponDesign.SelectPrimaryWeaponClass(weaponDesign.Template);
				}
				foreach (WeaponDesignElement weaponDesignElement in weaponDesign.UsedPieces)
				{
					if (weaponDesignElement.IsValid)
					{
						this._dataSource.WeaponDesign.SwitchToPiece(weaponDesignElement);
					}
				}
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003E9E File Offset: 0x0000209E
		private ItemObject.ItemUsageSetFlags GetItemUsageSetFlag(WeaponComponentData item)
		{
			if (!string.IsNullOrEmpty(item.ItemUsage))
			{
				return MBItem.GetItemUsageSetFlags(item.ItemUsage);
			}
			return 0;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003EBA File Offset: 0x000020BA
		private bool IsHotKeyReleasedInAnyLayer(string hotKeyId)
		{
			return this._sceneLayer.Input.IsHotKeyReleased(hotKeyId) || this._gauntletLayer.Input.IsHotKeyReleased(hotKeyId);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003EE2 File Offset: 0x000020E2
		[CommandLineFunctionality.CommandLineArgumentFunction("reload_pieces", "crafting")]
		public static string ReloadCraftingPieces(List<string> strings)
		{
			if (strings.Count != 2)
			{
				return "Usage: crafting.reload_pieces {MODULE_NAME} {XML_NAME}";
			}
			GauntletCraftingScreen._reloadXmlPath = new KeyValuePair<string, string>(strings[0], strings[1]);
			return "Reloading crafting pieces...";
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003F10 File Offset: 0x00002110
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003F12 File Offset: 0x00002112
		void IGameStateListener.OnFinalize()
		{
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003F14 File Offset: 0x00002114
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003F16 File Offset: 0x00002116
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x0400000D RID: 13
		private const float _controllerRotationSensitivity = 2f;

		// Token: 0x0400000E RID: 14
		private Scene _craftingScene;

		// Token: 0x0400000F RID: 15
		private SceneLayer _sceneLayer;

		// Token: 0x04000010 RID: 16
		private readonly CraftingState _craftingState;

		// Token: 0x04000011 RID: 17
		private CraftingVM _dataSource;

		// Token: 0x04000012 RID: 18
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000013 RID: 19
		private IGauntletMovie _gauntletMovie;

		// Token: 0x04000014 RID: 20
		private SpriteCategory _craftingCategory;

		// Token: 0x04000015 RID: 21
		private Camera _camera;

		// Token: 0x04000016 RID: 22
		private MatrixFrame _cameraFrame;

		// Token: 0x04000017 RID: 23
		private MatrixFrame _initialCameraFrame;

		// Token: 0x04000018 RID: 24
		private Vec3 _dofParams;

		// Token: 0x04000019 RID: 25
		private Vec2 _curCamSpeed;

		// Token: 0x0400001A RID: 26
		private float _zoomAmount;

		// Token: 0x0400001B RID: 27
		private GameEntity _craftingEntity;

		// Token: 0x0400001C RID: 28
		private MatrixFrame _craftingEntityFrame = MatrixFrame.Identity;

		// Token: 0x0400001D RID: 29
		private MatrixFrame _initialEntityFrame;

		// Token: 0x0400001E RID: 30
		private WeaponDesign _craftedData;

		// Token: 0x0400001F RID: 31
		private bool _isInitialized;

		// Token: 0x04000020 RID: 32
		private static KeyValuePair<string, string> _reloadXmlPath;
	}
}
