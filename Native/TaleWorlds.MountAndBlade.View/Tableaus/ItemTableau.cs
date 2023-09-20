using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	// Token: 0x02000022 RID: 34
	public class ItemTableau
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000141 RID: 321 RVA: 0x0000AC00 File Offset: 0x00008E00
		// (set) Token: 0x06000142 RID: 322 RVA: 0x0000AC08 File Offset: 0x00008E08
		public Texture Texture { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000143 RID: 323 RVA: 0x0000AC11 File Offset: 0x00008E11
		private TableauView View
		{
			get
			{
				if (this.Texture != null)
				{
					return this.Texture.TableauView;
				}
				return null;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000144 RID: 324 RVA: 0x0000AC2E File Offset: 0x00008E2E
		private bool _isSizeValid
		{
			get
			{
				return this._tableauSizeX > 0 && this._tableauSizeY > 0;
			}
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000AC44 File Offset: 0x00008E44
		public ItemTableau()
		{
			this.SetEnabled(true);
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000ACA0 File Offset: 0x00008EA0
		public void SetTargetSize(int width, int height)
		{
			bool isSizeValid = this._isSizeValid;
			this._isRotating = false;
			if (width <= 0 || height <= 0)
			{
				this._tableauSizeX = 10;
				this._tableauSizeY = 10;
			}
			else
			{
				this.RenderScale = NativeOptions.GetConfig(21) / 100f;
				this._tableauSizeX = (int)((float)width * this.RenderScale);
				this._tableauSizeY = (int)((float)height * this.RenderScale);
			}
			this._cameraRatio = (float)this._tableauSizeX / (float)this._tableauSizeY;
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(false);
			}
			TableauView view2 = this.View;
			if (view2 != null)
			{
				view2.AddClearTask(true);
			}
			Texture texture = this.Texture;
			if (texture != null)
			{
				texture.ReleaseNextFrame();
			}
			if (!isSizeValid && this._isSizeValid)
			{
				this.Recalculate();
			}
			this.Texture = TableauView.AddTableau("ItemTableau", new RenderTargetComponent.TextureUpdateEventHandler(this.TableauMaterialTabInventoryItemTooltipOnRender), this._tableauScene, this._tableauSizeX, this._tableauSizeY);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000AD90 File Offset: 0x00008F90
		public void OnFinalize()
		{
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(false);
			}
			Camera camera = this._camera;
			if (camera != null)
			{
				camera.ReleaseCameraEntity();
			}
			this._camera = null;
			TableauView view2 = this.View;
			if (view2 != null)
			{
				view2.AddClearTask(false);
			}
			this._tableauScene = null;
			this.Texture = null;
			this._initialized = false;
			if (this._lockMouse)
			{
				this.UpdateMouseLock(true);
			}
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000AE00 File Offset: 0x00009000
		protected void SetEnabled(bool enabled)
		{
			this._isRotatingByDefault = true;
			this._isRotating = false;
			this.ResetCamera();
			this._isEnabled = enabled;
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(this._isEnabled);
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000AE44 File Offset: 0x00009044
		public void SetStringId(string stringId)
		{
			this._stringId = stringId;
			this.Recalculate();
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000AE53 File Offset: 0x00009053
		public void SetAmmo(int ammo)
		{
			this._ammo = ammo;
			this.Recalculate();
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000AE62 File Offset: 0x00009062
		public void SetAverageUnitCost(int averageUnitCost)
		{
			this._averageUnitCost = averageUnitCost;
			this.Recalculate();
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000AE71 File Offset: 0x00009071
		public void SetItemModifierId(string itemModifierId)
		{
			this._itemModifierId = itemModifierId;
			this.Recalculate();
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000AE80 File Offset: 0x00009080
		public void SetBannerCode(string bannerCode)
		{
			this._bannerCode = bannerCode;
			this.Recalculate();
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000AE90 File Offset: 0x00009090
		public void Recalculate()
		{
			if (UiStringHelper.IsStringNoneOrEmptyForUi(this._stringId) || !this._isSizeValid)
			{
				return;
			}
			ItemModifier itemModifier = null;
			ItemObject itemObject = MBObjectManager.Instance.GetObject<ItemObject>(this._stringId);
			if (itemObject == null)
			{
				itemObject = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>().FirstOrDefault((ItemObject item) => item.IsCraftedWeapon && item.WeaponDesign.HashedCode == this._stringId);
			}
			if (!string.IsNullOrEmpty(this._itemModifierId))
			{
				itemModifier = MBObjectManager.Instance.GetObject<ItemModifier>(this._itemModifierId);
			}
			if (itemObject == null)
			{
				return;
			}
			this._itemRosterElement = new ItemRosterElement(itemObject, this._ammo, itemModifier);
			this.RefreshItemTableau();
			if (this._itemTableauEntity != null)
			{
				float num = Screen.RealScreenResolutionWidth / (float)this._tableauSizeX;
				float num2 = Screen.RealScreenResolutionHeight / (float)this._tableauSizeY;
				float num3 = ((num > num2) ? num : num2);
				if (num3 < 1f)
				{
					Vec3 globalBoxMax = this._itemTableauEntity.GlobalBoxMax;
					Vec3 globalBoxMin = this._itemTableauEntity.GlobalBoxMin;
					this._itemTableauFrame = this._itemTableauEntity.GetFrame();
					float length = this._itemTableauFrame.rotation.f.Length;
					this._itemTableauFrame.rotation.Orthonormalize();
					this._itemTableauFrame.rotation.ApplyScaleLocal(length * num3);
					this._itemTableauEntity.SetFrame(ref this._itemTableauFrame);
					if (globalBoxMax.NearlyEquals(this._itemTableauEntity.GlobalBoxMax, 1E-05f) && globalBoxMin.NearlyEquals(this._itemTableauEntity.GlobalBoxMin, 1E-05f))
					{
						this._itemTableauEntity.SetBoundingboxDirty();
						this._itemTableauEntity.RecomputeBoundingBox();
					}
					this._itemTableauFrame.origin = this._itemTableauFrame.origin + (globalBoxMax + globalBoxMin - this._itemTableauEntity.GlobalBoxMax - this._itemTableauEntity.GlobalBoxMin) * 0.5f;
					this._itemTableauEntity.SetFrame(ref this._itemTableauFrame);
					this._midPoint = (this._itemTableauEntity.GlobalBoxMax + this._itemTableauEntity.GlobalBoxMin) * 0.5f + (globalBoxMax + globalBoxMin - this._itemTableauEntity.GlobalBoxMax - this._itemTableauEntity.GlobalBoxMin) * 0.5f;
				}
				else
				{
					this._midPoint = (this._itemTableauEntity.GlobalBoxMax + this._itemTableauEntity.GlobalBoxMin) * 0.5f;
				}
				if (this._itemRosterElement.EquipmentElement.Item.ItemType == 15 || this._itemRosterElement.EquipmentElement.Item.ItemType == 7)
				{
					this._midPoint *= 1.2f;
				}
				this.ResetCamera();
			}
			this._isRotatingByDefault = true;
			this._isRotating = false;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000B17C File Offset: 0x0000937C
		public void Initialize()
		{
			this._isRotatingByDefault = true;
			this._isRotating = false;
			this._isTranslating = false;
			this._tableauScene = Scene.CreateNewScene(true, true, 0, "mono_renderscene");
			this._tableauScene.SetName("ItemTableau");
			this._tableauScene.DisableStaticShadows(true);
			this._tableauScene.SetAtmosphereWithName("character_menu_a");
			Vec3 vec;
			vec..ctor(1f, -1f, -1f, -1f);
			this._tableauScene.SetSunDirection(ref vec);
			this._tableauScene.SetClothSimulationState(false);
			this.ResetCamera();
			this._initialized = true;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000B21E File Offset: 0x0000941E
		private void TranslateCamera(bool value)
		{
			this.TranslateCameraAux(value);
		}

		// Token: 0x06000151 RID: 337 RVA: 0x0000B227 File Offset: 0x00009427
		private void TranslateCameraAux(bool value)
		{
			this._isRotatingByDefault = !value && this._isRotatingByDefault;
			this._isTranslating = value;
			this.UpdateMouseLock(false);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x0000B24C File Offset: 0x0000944C
		private void ResetCamera()
		{
			this._curCamDisplacement = Vec3.Zero;
			this._curZoomSpeed = 0f;
			if (this._camera != null)
			{
				this._camera.Frame = MatrixFrame.Identity;
				this.SetCamFovHorizontal(1f);
				this.MakeCameraLookMidPoint();
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000B29E File Offset: 0x0000949E
		public void RotateItem(bool value)
		{
			this._isRotatingByDefault = !value && this._isRotatingByDefault;
			this._isRotating = value;
			this.UpdateMouseLock(false);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000B2C0 File Offset: 0x000094C0
		public void RotateItemVerticalWithAmount(float value)
		{
			this.UpdateRotation(0f, value / -2f);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000B2D4 File Offset: 0x000094D4
		public void RotateItemHorizontalWithAmount(float value)
		{
			this.UpdateRotation(value / 2f, 0f);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000B2E8 File Offset: 0x000094E8
		public void OnTick(float dt)
		{
			float num = Input.MouseMoveX + Input.GetKeyState(222).X * 1000f * dt;
			float num2 = Input.MouseMoveY + Input.GetKeyState(222).Y * -1000f * dt;
			if (this._isEnabled && (this._isRotating || this._isTranslating) && (!MBMath.ApproximatelyEqualsTo(num, 0f, 1E-05f) || !MBMath.ApproximatelyEqualsTo(num2, 0f, 1E-05f)))
			{
				if (this._isRotating)
				{
					this.UpdateRotation(num, num2);
				}
				if (this._isTranslating)
				{
					this.UpdatePosition(num, num2);
				}
			}
			this.TickCameraZoom(dt);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000B39C File Offset: 0x0000959C
		private void UpdatePosition(float mouseMoveX, float mouseMoveY)
		{
			if (this._initialized)
			{
				Vec3 vec;
				vec..ctor(mouseMoveX / (float)(-(float)this._tableauSizeX), mouseMoveY / (float)this._tableauSizeY, 0f, -1f);
				vec *= 2.2f * this._camera.HorizontalFov;
				this._curCamDisplacement += vec;
				this.MakeCameraLookMidPoint();
			}
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000B408 File Offset: 0x00009608
		private void UpdateRotation(float mouseMoveX, float mouseMoveY)
		{
			if (this._initialized)
			{
				this._panRotation += mouseMoveX * 0.004363323f;
				this._tiltRotation += mouseMoveY * 0.004363323f;
				this._tiltRotation = MathF.Clamp(this._tiltRotation, -2.984513f, -0.15707964f);
				MatrixFrame matrixFrame = this._itemTableauEntity.GetFrame();
				Vec3 vec = (this._itemTableauEntity.GetBoundingBoxMax() + this._itemTableauEntity.GetBoundingBoxMin()) * 0.5f;
				MatrixFrame identity = MatrixFrame.Identity;
				identity.origin = vec;
				MatrixFrame identity2 = MatrixFrame.Identity;
				identity2.origin = -vec;
				matrixFrame *= identity;
				matrixFrame.rotation = Mat3.Identity;
				matrixFrame.rotation.ApplyScaleLocal(this._initialFrame.rotation.GetScaleVector());
				matrixFrame.rotation.RotateAboutSide(this._tiltRotation);
				matrixFrame.rotation.RotateAboutUp(this._panRotation);
				matrixFrame *= identity2;
				this._itemTableauEntity.SetFrame(ref matrixFrame);
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000B51F File Offset: 0x0000971F
		public void SetInitialTiltRotation(float amount)
		{
			this._hasInitialTiltRotation = true;
			this._initialTiltRotation = amount;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000B52F File Offset: 0x0000972F
		public void SetInitialPanRotation(float amount)
		{
			this._hasInitialPanRotation = true;
			this._initialPanRotation = amount;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000B53F File Offset: 0x0000973F
		public void Zoom(double value)
		{
			this._curZoomSpeed -= (float)(value / 1000.0);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x0000B55A File Offset: 0x0000975A
		public void SetItem(ItemRosterElement itemRosterElement)
		{
			this._itemRosterElement = itemRosterElement;
			this.RefreshItemTableau();
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000B56C File Offset: 0x0000976C
		private void RefreshItemTableau()
		{
			if (!this._initialized)
			{
				this.Initialize();
			}
			if (this._itemTableauEntity != null)
			{
				this._itemTableauEntity.Remove(102);
				this._itemTableauEntity = null;
			}
			if (this._itemRosterElement.EquipmentElement.Item != null)
			{
				ItemObject.ItemTypeEnum itemType = this._itemRosterElement.EquipmentElement.Item.ItemType;
				if (this._itemTableauEntity == null)
				{
					MatrixFrame itemFrameForItemTooltip = this._itemRosterElement.GetItemFrameForItemTooltip();
					itemFrameForItemTooltip.origin.z = itemFrameForItemTooltip.origin.z + 2.5f;
					MetaMesh itemMeshForInventory = this._itemRosterElement.GetItemMeshForInventory(false);
					Banner banner = new Banner(this._bannerCode);
					uint num = 0U;
					uint num2 = 0U;
					if (!string.IsNullOrEmpty(this._bannerCode))
					{
						num = banner.GetPrimaryColor();
						num2 = BannerManager.ColorPalette[banner.BannerDataList[1].ColorId].Color;
					}
					if (itemMeshForInventory != null)
					{
						if (itemType == 15)
						{
							this._itemTableauEntity = GameEntity.CreateEmpty(this._tableauScene, true);
							AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(Game.Current.DefaultMonster, MBActionSet.GetActionSet(Game.Current.DefaultMonster.ActionSetCode), 1f, false);
							GameEntityExtensions.CreateSkeletonWithActionSet(this._itemTableauEntity, ref animationSystemData);
							this._itemTableauEntity.SetFrame(ref itemFrameForItemTooltip);
							MBSkeletonExtensions.SetAgentActionChannel(this._itemTableauEntity.Skeleton, 0, ActionIndexCache.Create("act_tableau_hand_armor_pose"), 0f, -0.2f, true);
							this._itemTableauEntity.AddMultiMeshToSkeleton(itemMeshForInventory);
							MBSkeletonExtensions.TickActionChannels(this._itemTableauEntity.Skeleton);
							this._itemTableauEntity.Skeleton.TickAnimationsAndForceUpdate(0.01f, itemFrameForItemTooltip, true);
						}
						else if (itemType == 1 || itemType == 19)
						{
							HorseComponent horseComponent = this._itemRosterElement.EquipmentElement.Item.HorseComponent;
							Monster monster = horseComponent.Monster;
							this._itemTableauEntity = GameEntity.CreateEmpty(this._tableauScene, true);
							AnimationSystemData animationSystemData2 = MonsterExtensions.FillAnimationSystemData(monster, MBGlobals.GetActionSet(horseComponent.Monster.ActionSetCode), 1f, false);
							GameEntityExtensions.CreateSkeletonWithActionSet(this._itemTableauEntity, ref animationSystemData2);
							MBSkeletonExtensions.SetAgentActionChannel(this._itemTableauEntity.Skeleton, 0, ActionIndexCache.Create("act_inventory_idle_start"), 0f, -0.2f, true);
							this._itemTableauEntity.SetFrame(ref itemFrameForItemTooltip);
							this._itemTableauEntity.AddMultiMeshToSkeleton(itemMeshForInventory);
						}
						else if (itemType == 23 && this._itemRosterElement.EquipmentElement.Item.ArmorComponent != null)
						{
							this._itemTableauEntity = this._tableauScene.AddItemEntity(ref itemFrameForItemTooltip, itemMeshForInventory);
							MetaMesh copy = MetaMesh.GetCopy(this._itemRosterElement.EquipmentElement.Item.ArmorComponent.ReinsMesh, true, true);
							if (copy != null)
							{
								this._itemTableauEntity.AddMultiMesh(copy, true);
							}
						}
						else if (itemType == 7)
						{
							if (this._itemRosterElement.EquipmentElement.Item.IsUsingTableau && !Extensions.IsEmpty<BannerData>(banner.BannerDataList))
							{
								itemMeshForInventory.SetMaterial(this._itemRosterElement.EquipmentElement.Item.GetTableauMaterial(banner));
							}
							this._itemTableauEntity = this._tableauScene.AddItemEntity(ref itemFrameForItemTooltip, itemMeshForInventory);
						}
						else if (itemType == 24)
						{
							if (this._itemRosterElement.EquipmentElement.Item.IsUsingTableau && !Extensions.IsEmpty<BannerData>(banner.BannerDataList))
							{
								itemMeshForInventory.SetMaterial(this._itemRosterElement.EquipmentElement.Item.GetTableauMaterial(banner));
							}
							if (!string.IsNullOrEmpty(this._bannerCode))
							{
								for (int i = 0; i < itemMeshForInventory.MeshCount; i++)
								{
									itemMeshForInventory.GetMeshAtIndex(i).Color = num;
									itemMeshForInventory.GetMeshAtIndex(i).Color2 = num2;
								}
							}
							this._itemTableauEntity = this._tableauScene.AddItemEntity(ref itemFrameForItemTooltip, itemMeshForInventory);
						}
						else
						{
							this._itemTableauEntity = this._tableauScene.AddItemEntity(ref itemFrameForItemTooltip, itemMeshForInventory);
						}
					}
					else
					{
						MBDebug.ShowWarning("[DEBUG]Item with " + this._itemRosterElement.EquipmentElement.Item.StringId + "[DEBUG] string id cannot be found");
					}
				}
				SkinMask skinMask = 481;
				if (this._itemRosterElement.EquipmentElement.Item.HasArmorComponent)
				{
					skinMask = this._itemRosterElement.EquipmentElement.Item.ArmorComponent.MeshesMask;
				}
				string text = "";
				bool flag = Extensions.HasAnyFlag<ItemFlags>(this._itemRosterElement.EquipmentElement.Item.ItemFlags, 2048);
				bool flag2 = false;
				if (12 == itemType || 22 == itemType)
				{
					text = "base_head";
					flag2 = true;
				}
				else if (13 == itemType)
				{
					if (Extensions.HasAnyFlag<SkinMask>(skinMask, 32))
					{
						text = "base_body";
						flag2 = true;
					}
				}
				else if (14 == itemType)
				{
					if (Extensions.HasAnyFlag<SkinMask>(skinMask, 256))
					{
						text = "base_foot";
						flag2 = true;
					}
				}
				else if (15 == itemType)
				{
					if (Extensions.HasAnyFlag<SkinMask>(skinMask, 128))
					{
						MetaMesh copy2 = MetaMesh.GetCopy(flag ? "base_hand_female" : "base_hand", false, false);
						this._itemTableauEntity.AddMultiMeshToSkeleton(copy2);
					}
				}
				else if (23 == itemType)
				{
					text = "horse_base_mesh";
					flag2 = false;
				}
				if (text.Length > 0)
				{
					if (flag2 && flag)
					{
						text += "_female";
					}
					MetaMesh copy3 = MetaMesh.GetCopy(text, false, false);
					this._itemTableauEntity.AddMultiMesh(copy3, true);
				}
				TableauView view = this.View;
				if (view != null)
				{
					float num3 = (this._itemTableauEntity.GetBoundingBoxMax() - this._itemTableauEntity.GetBoundingBoxMin()).Length * 2f;
					Vec3 origin = this._itemTableauEntity.GetGlobalFrame().origin;
					view.SetFocusedShadowmap(true, ref origin, num3);
				}
				if (this._itemTableauEntity != null)
				{
					this._initialFrame = this._itemTableauEntity.GetFrame();
					Vec3 eulerAngles = this._initialFrame.rotation.GetEulerAngles();
					this._panRotation = eulerAngles.x;
					this._tiltRotation = eulerAngles.z;
					if (this._hasInitialPanRotation)
					{
						this._panRotation = this._initialPanRotation;
					}
					else if (itemType == 7)
					{
						this._panRotation = -3.1415927f;
					}
					if (this._hasInitialTiltRotation)
					{
						this._tiltRotation = this._initialTiltRotation;
						return;
					}
					if (itemType == 7)
					{
						this._tiltRotation = 0f;
						return;
					}
					this._tiltRotation = -1.5707964f;
				}
			}
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000BBF4 File Offset: 0x00009DF4
		private void TableauMaterialTabInventoryItemTooltipOnRender(Texture sender, EventArgs e)
		{
			if (this._initialized)
			{
				TableauView tableauView = this.View;
				if (tableauView == null)
				{
					tableauView = sender.TableauView;
					tableauView.SetEnable(this._isEnabled);
				}
				if (this._itemRosterElement.EquipmentElement.Item == null)
				{
					tableauView.SetContinuousRendering(false);
					tableauView.SetDeleteAfterRendering(true);
					return;
				}
				tableauView.SetRenderWithPostfx(true);
				tableauView.SetClearColor(0U);
				tableauView.SetScene(this._tableauScene);
				if (this._camera == null)
				{
					this._camera = Camera.CreateCamera();
					this._camera.SetViewVolume(true, -0.5f, 0.5f, -0.5f, 0.5f, 0.01f, 100f);
					this.ResetCamera();
					tableauView.SetSceneUsesSkybox(false);
				}
				tableauView.SetCamera(this._camera);
				if (this._isRotatingByDefault)
				{
					this.UpdateRotation(1f, 0f);
				}
				tableauView.SetDeleteAfterRendering(false);
				tableauView.SetContinuousRendering(true);
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000BCF0 File Offset: 0x00009EF0
		private void MakeCameraLookMidPoint()
		{
			Vec3 vec = this._camera.Frame.rotation.TransformToParent(this._curCamDisplacement);
			Vec3 vec2 = this._midPoint + vec;
			float num = this._midPoint.Length * 0.5263158f;
			Vec3 vec3 = vec2 - this._camera.Direction * num;
			this._camera.Position = vec3;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000BD5D File Offset: 0x00009F5D
		private void SetCamFovHorizontal(float camFov)
		{
			this._camera.SetFovHorizontal(camFov, 1f, 0.1f, 50f);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000BD7A File Offset: 0x00009F7A
		private void UpdateMouseLock(bool forceUnlock = false)
		{
			this._lockMouse = (this._isRotating || this._isTranslating) && !forceUnlock;
			MouseManager.LockCursorAtCurrentPosition(this._lockMouse);
			MouseManager.ShowCursor(!this._lockMouse);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000BDB4 File Offset: 0x00009FB4
		private void TickCameraZoom(float dt)
		{
			if (this._camera != null)
			{
				float num = this._camera.HorizontalFov;
				num += this._curZoomSpeed;
				num = MBMath.ClampFloat(num, 0.1f, 2f);
				this.SetCamFovHorizontal(num);
				if (dt > 0f)
				{
					this._curZoomSpeed = MBMath.Lerp(this._curZoomSpeed, 0f, MBMath.ClampFloat(dt * 25.9f, 0f, 1f), 1E-05f);
				}
			}
		}

		// Token: 0x040000C3 RID: 195
		private Scene _tableauScene;

		// Token: 0x040000C4 RID: 196
		private GameEntity _itemTableauEntity;

		// Token: 0x040000C5 RID: 197
		private MatrixFrame _itemTableauFrame = MatrixFrame.Identity;

		// Token: 0x040000C6 RID: 198
		private bool _isRotating;

		// Token: 0x040000C7 RID: 199
		private bool _isTranslating;

		// Token: 0x040000C8 RID: 200
		private bool _isRotatingByDefault;

		// Token: 0x040000C9 RID: 201
		private bool _initialized;

		// Token: 0x040000CA RID: 202
		private int _tableauSizeX;

		// Token: 0x040000CB RID: 203
		private int _tableauSizeY;

		// Token: 0x040000CC RID: 204
		private float _cameraRatio;

		// Token: 0x040000CD RID: 205
		private Camera _camera;

		// Token: 0x040000CE RID: 206
		private Vec3 _midPoint;

		// Token: 0x040000CF RID: 207
		private const float InitialCamFov = 1f;

		// Token: 0x040000D0 RID: 208
		private float _curZoomSpeed;

		// Token: 0x040000D1 RID: 209
		private Vec3 _curCamDisplacement = Vec3.Zero;

		// Token: 0x040000D2 RID: 210
		private bool _isEnabled;

		// Token: 0x040000D3 RID: 211
		private float _panRotation;

		// Token: 0x040000D4 RID: 212
		private float _tiltRotation;

		// Token: 0x040000D5 RID: 213
		private bool _hasInitialTiltRotation;

		// Token: 0x040000D6 RID: 214
		private float _initialTiltRotation;

		// Token: 0x040000D7 RID: 215
		private bool _hasInitialPanRotation;

		// Token: 0x040000D8 RID: 216
		private float _initialPanRotation;

		// Token: 0x040000D9 RID: 217
		private float RenderScale = 1f;

		// Token: 0x040000DA RID: 218
		private string _stringId = "";

		// Token: 0x040000DB RID: 219
		private int _ammo;

		// Token: 0x040000DC RID: 220
		private int _averageUnitCost;

		// Token: 0x040000DD RID: 221
		private string _itemModifierId = "";

		// Token: 0x040000DE RID: 222
		private string _bannerCode = "";

		// Token: 0x040000DF RID: 223
		private ItemRosterElement _itemRosterElement;

		// Token: 0x040000E0 RID: 224
		private MatrixFrame _initialFrame;

		// Token: 0x040000E1 RID: 225
		private bool _lockMouse;
	}
}
