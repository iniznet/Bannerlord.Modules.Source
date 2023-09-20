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
	public class ItemTableau
	{
		public Texture Texture { get; private set; }

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

		private bool _isSizeValid
		{
			get
			{
				return this._tableauSizeX > 0 && this._tableauSizeY > 0;
			}
		}

		public ItemTableau()
		{
			this.SetEnabled(true);
		}

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

		public void SetStringId(string stringId)
		{
			this._stringId = stringId;
			this.Recalculate();
		}

		public void SetAmmo(int ammo)
		{
			this._ammo = ammo;
			this.Recalculate();
		}

		public void SetAverageUnitCost(int averageUnitCost)
		{
			this._averageUnitCost = averageUnitCost;
			this.Recalculate();
		}

		public void SetItemModifierId(string itemModifierId)
		{
			this._itemModifierId = itemModifierId;
			this.Recalculate();
		}

		public void SetBannerCode(string bannerCode)
		{
			this._bannerCode = bannerCode;
			this.Recalculate();
		}

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

		private void TranslateCamera(bool value)
		{
			this.TranslateCameraAux(value);
		}

		private void TranslateCameraAux(bool value)
		{
			this._isRotatingByDefault = !value && this._isRotatingByDefault;
			this._isTranslating = value;
			this.UpdateMouseLock(false);
		}

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

		public void RotateItem(bool value)
		{
			this._isRotatingByDefault = !value && this._isRotatingByDefault;
			this._isRotating = value;
			this.UpdateMouseLock(false);
		}

		public void RotateItemVerticalWithAmount(float value)
		{
			this.UpdateRotation(0f, value / -2f);
		}

		public void RotateItemHorizontalWithAmount(float value)
		{
			this.UpdateRotation(value / 2f, 0f);
		}

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

		public void SetInitialTiltRotation(float amount)
		{
			this._hasInitialTiltRotation = true;
			this._initialTiltRotation = amount;
		}

		public void SetInitialPanRotation(float amount)
		{
			this._hasInitialPanRotation = true;
			this._initialPanRotation = amount;
		}

		public void Zoom(double value)
		{
			this._curZoomSpeed -= (float)(value / 1000.0);
		}

		public void SetItem(ItemRosterElement itemRosterElement)
		{
			this._itemRosterElement = itemRosterElement;
			this.RefreshItemTableau();
		}

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

		private void MakeCameraLookMidPoint()
		{
			Vec3 vec = this._camera.Frame.rotation.TransformToParent(this._curCamDisplacement);
			Vec3 vec2 = this._midPoint + vec;
			float num = this._midPoint.Length * 0.5263158f;
			Vec3 vec3 = vec2 - this._camera.Direction * num;
			this._camera.Position = vec3;
		}

		private void SetCamFovHorizontal(float camFov)
		{
			this._camera.SetFovHorizontal(camFov, 1f, 0.1f, 50f);
		}

		private void UpdateMouseLock(bool forceUnlock = false)
		{
			this._lockMouse = (this._isRotating || this._isTranslating) && !forceUnlock;
			MouseManager.LockCursorAtCurrentPosition(this._lockMouse);
			MouseManager.ShowCursor(!this._lockMouse);
		}

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

		private Scene _tableauScene;

		private GameEntity _itemTableauEntity;

		private MatrixFrame _itemTableauFrame = MatrixFrame.Identity;

		private bool _isRotating;

		private bool _isTranslating;

		private bool _isRotatingByDefault;

		private bool _initialized;

		private int _tableauSizeX;

		private int _tableauSizeY;

		private float _cameraRatio;

		private Camera _camera;

		private Vec3 _midPoint;

		private const float InitialCamFov = 1f;

		private float _curZoomSpeed;

		private Vec3 _curCamDisplacement = Vec3.Zero;

		private bool _isEnabled;

		private float _panRotation;

		private float _tiltRotation;

		private bool _hasInitialTiltRotation;

		private float _initialTiltRotation;

		private bool _hasInitialPanRotation;

		private float _initialPanRotation;

		private float RenderScale = 1f;

		private string _stringId = "";

		private int _ammo;

		private int _averageUnitCost;

		private string _itemModifierId = "";

		private string _bannerCode = "";

		private ItemRosterElement _itemRosterElement;

		private MatrixFrame _initialFrame;

		private bool _lockMouse;
	}
}
