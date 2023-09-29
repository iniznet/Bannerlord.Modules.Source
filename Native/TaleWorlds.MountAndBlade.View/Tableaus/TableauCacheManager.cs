using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Scripts;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlayerServices.Avatar;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	public class TableauCacheManager
	{
		public static TableauCacheManager Current { get; private set; }

		public MatrixFrame InventorySceneCameraFrame { get; private set; }

		private void InitializeThumbnailCreator()
		{
			this._thumbnailCreatorView = ThumbnailCreatorView.CreateThumbnailCreatorView();
			ThumbnailCreatorView.renderCallback = (ThumbnailCreatorView.OnThumbnailRenderCompleteDelegate)Delegate.Combine(ThumbnailCreatorView.renderCallback, new ThumbnailCreatorView.OnThumbnailRenderCompleteDelegate(this.OnThumbnailRenderComplete));
			foreach (Scene scene in BannerlordTableauManager.TableauCharacterScenes)
			{
				this._thumbnailCreatorView.RegisterScene(scene, true);
			}
			this._bannerScene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
			this._bannerScene.DisableStaticShadows(true);
			this._bannerScene.SetName("TableauCacheManager.BannerScene");
			this._bannerScene.SetDefaultLighting();
			SceneInitializationData sceneInitializationData;
			sceneInitializationData..ctor(true);
			sceneInitializationData.InitPhysicsWorld = false;
			sceneInitializationData.DoNotUseLoadingScreen = true;
			this._inventoryScene = Scene.CreateNewScene(true, false, 2, "mono_renderscene");
			this._inventoryScene.Read("inventory_character_scene", ref sceneInitializationData, "");
			this._inventoryScene.SetShadow(true);
			this._inventoryScene.DisableStaticShadows(true);
			this.InventorySceneCameraFrame = this._inventoryScene.FindEntityWithTag("camera_instance").GetGlobalFrame();
			this._inventorySceneAgentRenderer = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(this._inventoryScene, 32);
			this._thumbnailCreatorView.RegisterScene(this._bannerScene, false);
			this.bannerTableauGPUAllocationIndex = Utilities.RegisterGPUAllocationGroup("BannerTableauCache");
			this.itemTableauGPUAllocationIndex = Utilities.RegisterGPUAllocationGroup("ItemTableauCache");
			this.characterTableauGPUAllocationIndex = Utilities.RegisterGPUAllocationGroup("CharacterTableauCache");
		}

		public bool IsCachedInventoryTableauSceneUsed()
		{
			return this._inventorySceneBeingUsed;
		}

		public Scene GetCachedInventoryTableauScene()
		{
			this._inventorySceneBeingUsed = true;
			return this._inventoryScene;
		}

		public void ReturnCachedInventoryTableauScene()
		{
			this._inventorySceneBeingUsed = false;
		}

		public bool IsCachedMapConversationTableauSceneUsed()
		{
			return this._mapConversationSceneBeingUsed;
		}

		public Scene GetCachedMapConversationTableauScene()
		{
			this._mapConversationSceneBeingUsed = true;
			return this._mapConversationScene;
		}

		public void ReturnCachedMapConversationTableauScene()
		{
			this._mapConversationSceneBeingUsed = false;
		}

		public static int GetNumberOfPendingRequests()
		{
			if (TableauCacheManager.Current != null)
			{
				return TableauCacheManager.Current._thumbnailCreatorView.GetNumberOfPendingRequests();
			}
			return 0;
		}

		public static bool IsNativeMemoryCleared()
		{
			return TableauCacheManager.Current != null && TableauCacheManager.Current._thumbnailCreatorView.IsMemoryCleared();
		}

		public static void InitializeManager()
		{
			TableauCacheManager.Current = new TableauCacheManager();
			TableauCacheManager.Current._renderCallbacks = new Dictionary<string, TableauCacheManager.RenderDetails>();
			TableauCacheManager.Current.InitializeThumbnailCreator();
			TableauCacheManager.Current._avatarVisuals = new ThumbnailCache(100, TableauCacheManager.Current._thumbnailCreatorView);
			TableauCacheManager.Current._itemVisuals = new ThumbnailCache(100, TableauCacheManager.Current._thumbnailCreatorView);
			TableauCacheManager.Current._craftingPieceVisuals = new ThumbnailCache(100, TableauCacheManager.Current._thumbnailCreatorView);
			TableauCacheManager.Current._characterVisuals = new ThumbnailCache(100, TableauCacheManager.Current._thumbnailCreatorView);
			TableauCacheManager.Current._bannerVisuals = new ThumbnailCache(100, TableauCacheManager.Current._thumbnailCreatorView);
			TableauCacheManager.Current._bannerCamera = TableauCacheManager.CreateDefaultBannerCamera();
			TableauCacheManager.Current._nineGridBannerCamera = TableauCacheManager.CreateNineGridBannerCamera();
			TableauCacheManager.Current._heroSilhouetteTexture = Texture.GetFromResource("hero_silhouette");
		}

		public static void InitializeSandboxValues()
		{
			SceneInitializationData sceneInitializationData;
			sceneInitializationData..ctor(true);
			sceneInitializationData.InitPhysicsWorld = false;
			TableauCacheManager.Current._mapConversationScene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
			TableauCacheManager.Current._mapConversationScene.SetName("MapConversationTableau");
			TableauCacheManager.Current._mapConversationScene.DisableStaticShadows(true);
			TableauCacheManager.Current._mapConversationScene.Read("scn_conversation_tableau", ref sceneInitializationData, "");
			TableauCacheManager.Current._mapConversationScene.SetShadow(true);
			TableauCacheManager.Current._mapConversationSceneAgentRenderer = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(TableauCacheManager.Current._mapConversationScene, 32);
			Utilities.LoadVirtualTextureTileset("WorldMap");
		}

		public static void ReleaseSandboxValues()
		{
			MBAgentRendererSceneController.DestructAgentRendererSceneController(TableauCacheManager.Current._mapConversationScene, TableauCacheManager.Current._mapConversationSceneAgentRenderer, false);
			TableauCacheManager.Current._mapConversationSceneAgentRenderer = null;
			TableauCacheManager.Current._mapConversationScene.ClearAll();
			TableauCacheManager.Current._mapConversationScene.ManualInvalidate();
			TableauCacheManager.Current._mapConversationScene = null;
		}

		public static void ClearManager()
		{
			Debug.Print("TableauCacheManager::ClearManager", 0, 12, 17592186044416UL);
			if (TableauCacheManager.Current != null)
			{
				TableauCacheManager.Current._renderCallbacks = null;
				TableauCacheManager.Current._avatarVisuals = null;
				TableauCacheManager.Current._itemVisuals = null;
				TableauCacheManager.Current._craftingPieceVisuals = null;
				TableauCacheManager.Current._characterVisuals = null;
				TableauCacheManager.Current._bannerVisuals = null;
				Camera bannerCamera = TableauCacheManager.Current._bannerCamera;
				if (bannerCamera != null)
				{
					bannerCamera.ReleaseCamera();
				}
				TableauCacheManager.Current._bannerCamera = null;
				Camera nineGridBannerCamera = TableauCacheManager.Current._nineGridBannerCamera;
				if (nineGridBannerCamera != null)
				{
					nineGridBannerCamera.ReleaseCamera();
				}
				TableauCacheManager.Current._nineGridBannerCamera = null;
				MBAgentRendererSceneController.DestructAgentRendererSceneController(TableauCacheManager.Current._inventoryScene, TableauCacheManager.Current._inventorySceneAgentRenderer, true);
				Scene inventoryScene = TableauCacheManager.Current._inventoryScene;
				if (inventoryScene != null)
				{
					inventoryScene.ManualInvalidate();
				}
				TableauCacheManager.Current._inventoryScene = null;
				Scene bannerScene = TableauCacheManager.Current._bannerScene;
				if (bannerScene != null)
				{
					bannerScene.ClearDecals();
				}
				Scene bannerScene2 = TableauCacheManager.Current._bannerScene;
				if (bannerScene2 != null)
				{
					bannerScene2.ClearAll();
				}
				Scene bannerScene3 = TableauCacheManager.Current._bannerScene;
				if (bannerScene3 != null)
				{
					bannerScene3.ManualInvalidate();
				}
				TableauCacheManager.Current._bannerScene = null;
				ThumbnailCreatorView.renderCallback = (ThumbnailCreatorView.OnThumbnailRenderCompleteDelegate)Delegate.Remove(ThumbnailCreatorView.renderCallback, new ThumbnailCreatorView.OnThumbnailRenderCompleteDelegate(TableauCacheManager.Current.OnThumbnailRenderComplete));
				TableauCacheManager.Current._thumbnailCreatorView.ClearRequests();
				TableauCacheManager.Current._thumbnailCreatorView = null;
				TableauCacheManager.Current = null;
			}
		}

		private string ByteWidthToString(int bytes)
		{
			double num = Math.Log((double)bytes);
			if (bytes == 0)
			{
				num = 0.0;
			}
			int num2 = (int)(num / Math.Log(1024.0));
			char c = " KMGTPE"[num2];
			return ((double)bytes / Math.Pow(1024.0, (double)num2)).ToString("0.00") + " " + c.ToString() + "      ";
		}

		private void PrintTextureToImgui(string name, ThumbnailCache cache)
		{
			int totalMemorySize = cache.GetTotalMemorySize();
			Imgui.Text(name);
			Imgui.NextColumn();
			Imgui.Text(cache.Count.ToString());
			Imgui.NextColumn();
			Imgui.Text(this.ByteWidthToString(totalMemorySize));
			Imgui.NextColumn();
		}

		public void OnImguiProfilerTick()
		{
			Imgui.BeginMainThreadScope();
			Imgui.Begin("Tableau Cache Manager");
			Imgui.Columns(3, "", true);
			Imgui.Text("Name");
			Imgui.NextColumn();
			Imgui.Text("Count");
			Imgui.NextColumn();
			Imgui.Text("Memory");
			Imgui.NextColumn();
			Imgui.Separator();
			this.PrintTextureToImgui("Items", this._itemVisuals);
			this.PrintTextureToImgui("Banners", this._bannerVisuals);
			this.PrintTextureToImgui("Characters", this._characterVisuals);
			this.PrintTextureToImgui("Avatars", this._avatarVisuals);
			this.PrintTextureToImgui("Crafting Pieces", this._craftingPieceVisuals);
			Imgui.Text("Render Callbacks");
			Imgui.NextColumn();
			Imgui.Text(this._renderCallbacks.Count<KeyValuePair<string, TableauCacheManager.RenderDetails>>().ToString());
			Imgui.NextColumn();
			Imgui.Text("-");
			Imgui.NextColumn();
			Imgui.End();
			Imgui.EndMainThreadScope();
		}

		private void OnThumbnailRenderComplete(string renderId, Texture renderTarget)
		{
			Texture texture = null;
			if (this._itemVisuals.GetValue(renderId, out texture))
			{
				this._itemVisuals.Add(renderId, renderTarget);
			}
			else if (this._craftingPieceVisuals.GetValue(renderId, out texture))
			{
				this._craftingPieceVisuals.Add(renderId, renderTarget);
			}
			else if (this._characterVisuals.GetValue(renderId, out texture))
			{
				this._characterVisuals.Add(renderId, renderTarget);
			}
			else if (!this._avatarVisuals.GetValue(renderId, out texture) && !this._bannerVisuals.GetValue(renderId, out texture))
			{
				renderTarget.Release();
			}
			if (this._renderCallbacks.ContainsKey(renderId))
			{
				foreach (Action<Texture> action in this._renderCallbacks[renderId].Actions)
				{
					if (action != null)
					{
						action(renderTarget);
					}
				}
				this._renderCallbacks.Remove(renderId);
			}
		}

		public Texture CreateAvatarTexture(string avatarID, byte[] avatarBytes, uint width, uint height, AvatarData.ImageType imageType)
		{
			Texture texture;
			this._avatarVisuals.GetValue(avatarID, out texture);
			if (texture == null)
			{
				if (imageType == null)
				{
					texture = Texture.CreateFromMemory(avatarBytes);
				}
				else if (imageType == 1)
				{
					texture = Texture.CreateFromByteArray(avatarBytes, (int)width, (int)height);
				}
				this._avatarVisuals.Add(avatarID, texture);
			}
			this._avatarVisuals.AddReference(avatarID);
			return texture;
		}

		public void BeginCreateItemTexture(ItemObject itemObject, string additionalArgs, Action<Texture> setAction)
		{
			string text = itemObject.StringId;
			if (itemObject.Type == 7)
			{
				text = text + "_" + additionalArgs;
			}
			Texture texture;
			if (this._itemVisuals.GetValue(text, out texture))
			{
				if (this._renderCallbacks.ContainsKey(text))
				{
					this._renderCallbacks[text].Actions.Add(setAction);
				}
				else if (setAction != null)
				{
					setAction(texture);
				}
				this._itemVisuals.AddReference(text);
				return;
			}
			Camera camera = null;
			int num = 2;
			int num2 = 256;
			int num3 = 120;
			GameEntity gameEntity = this.CreateItemBaseEntity(itemObject, BannerlordTableauManager.TableauCharacterScenes[num], ref camera);
			this._thumbnailCreatorView.RegisterEntityWithoutTexture(BannerlordTableauManager.TableauCharacterScenes[num], camera, gameEntity, num2, num3, this.itemTableauGPUAllocationIndex, text, "item_tableau_" + text);
			gameEntity.ManualInvalidate();
			this._itemVisuals.Add(text, null);
			this._itemVisuals.AddReference(text);
			if (!this._renderCallbacks.ContainsKey(text))
			{
				this._renderCallbacks.Add(text, new TableauCacheManager.RenderDetails(new List<Action<Texture>>()));
			}
			this._renderCallbacks[text].Actions.Add(setAction);
		}

		public void BeginCreateCraftingPieceTexture(CraftingPiece craftingPiece, string type, Action<Texture> setAction)
		{
			string text = craftingPiece.StringId + "$" + type;
			Texture texture;
			if (this._craftingPieceVisuals.GetValue(text, out texture))
			{
				if (this._renderCallbacks.ContainsKey(text))
				{
					this._renderCallbacks[text].Actions.Add(setAction);
				}
				else if (setAction != null)
				{
					setAction(texture);
				}
				this._craftingPieceVisuals.AddReference(text);
				return;
			}
			Camera camera = null;
			int num = 2;
			int num2 = 256;
			int num3 = 180;
			GameEntity gameEntity = this.CreateCraftingPieceBaseEntity(craftingPiece, type, BannerlordTableauManager.TableauCharacterScenes[num], ref camera);
			this._thumbnailCreatorView.RegisterEntityWithoutTexture(BannerlordTableauManager.TableauCharacterScenes[num], camera, gameEntity, num2, num3, this.itemTableauGPUAllocationIndex, text, "craft_tableau");
			gameEntity.ManualInvalidate();
			this._craftingPieceVisuals.Add(text, null);
			this._craftingPieceVisuals.AddReference(text);
			if (!this._renderCallbacks.ContainsKey(text))
			{
				this._renderCallbacks.Add(text, new TableauCacheManager.RenderDetails(new List<Action<Texture>>()));
			}
			this._renderCallbacks[text].Actions.Add(setAction);
		}

		public void BeginCreateCharacterTexture(CharacterCode characterCode, Action<Texture> setAction, bool isBig)
		{
			if (MBObjectManager.Instance == null)
			{
				return;
			}
			characterCode.BodyProperties = new BodyProperties(new DynamicBodyProperties((float)((int)characterCode.BodyProperties.Age), (float)((int)characterCode.BodyProperties.Weight), (float)((int)characterCode.BodyProperties.Build)), characterCode.BodyProperties.StaticProperties);
			string text = characterCode.CreateNewCodeString();
			text += (isBig ? "1" : "0");
			Texture texture;
			if (this._characterVisuals.GetValue(text, out texture))
			{
				if (this._renderCallbacks.ContainsKey(text))
				{
					this._renderCallbacks[text].Actions.Add(setAction);
				}
				else if (setAction != null)
				{
					setAction(texture);
				}
				this._characterVisuals.AddReference(text);
				return;
			}
			Camera camera = null;
			int num = (isBig ? 0 : 4);
			GameEntity gameEntity = this.CreateCharacterBaseEntity(characterCode, BannerlordTableauManager.TableauCharacterScenes[num], ref camera, isBig);
			gameEntity = this.FillEntityWithPose(characterCode, gameEntity, BannerlordTableauManager.TableauCharacterScenes[num]);
			int num2 = 256;
			int num3 = (isBig ? 120 : 184);
			this._thumbnailCreatorView.RegisterEntityWithoutTexture(BannerlordTableauManager.TableauCharacterScenes[num], camera, gameEntity, num2, num3, this.characterTableauGPUAllocationIndex, text, "character_tableau_" + this._characterCount.ToString());
			gameEntity.ManualInvalidate();
			this._characterCount++;
			this._characterVisuals.Add(text, null);
			this._characterVisuals.AddReference(text);
			if (!this._renderCallbacks.ContainsKey(text))
			{
				this._renderCallbacks.Add(text, new TableauCacheManager.RenderDetails(new List<Action<Texture>>()));
			}
			this._renderCallbacks[text].Actions.Add(setAction);
		}

		public Texture GetCachedHeroSilhouetteTexture()
		{
			return this._heroSilhouetteTexture;
		}

		public Texture BeginCreateBannerTexture(BannerCode bannerCode, Action<Texture> setAction, bool isTableauOrNineGrid = false, bool isLarge = false)
		{
			int num = 512;
			int num2 = 512;
			Camera camera = this._bannerCamera;
			string text = "BannerThumbnail";
			if (isTableauOrNineGrid)
			{
				camera = this._nineGridBannerCamera;
				if (isLarge)
				{
					num = 1024;
					num2 = 1024;
					text = "BannerTableauLarge";
				}
				else
				{
					text = "BannerTableauSmall";
				}
			}
			string text2 = text + ":" + bannerCode.Code;
			Texture texture;
			if (this._bannerVisuals.GetValue(text2, out texture))
			{
				if (this._renderCallbacks.ContainsKey(text2))
				{
					this._renderCallbacks[text2].Actions.Add(setAction);
				}
				else if (setAction != null)
				{
					setAction(texture);
				}
				this._bannerVisuals.AddReference(text2);
				return texture;
			}
			MatrixFrame identity = MatrixFrame.Identity;
			Banner banner = bannerCode.CalculateBanner();
			if (Game.Current == null)
			{
				banner.SetBannerVisual(new BannerVisualCreator().CreateBannerVisual(banner));
			}
			MetaMesh metaMesh = banner.ConvertToMultiMesh();
			GameEntity gameEntity = this._bannerScene.AddItemEntity(ref identity, metaMesh);
			metaMesh.ManualInvalidate();
			gameEntity.SetVisibilityExcludeParents(false);
			Texture texture2 = Texture.CreateRenderTarget(text + this._bannerCount.ToString(), num, num2, true, false, true, true);
			this._thumbnailCreatorView.RegisterEntity(this._bannerScene, camera, texture2, gameEntity, this.bannerTableauGPUAllocationIndex, text2);
			this._bannerVisuals.Add(text2, texture2);
			this._bannerVisuals.AddReference(text2);
			this._bannerCount++;
			if (!this._renderCallbacks.ContainsKey(text2))
			{
				this._renderCallbacks.Add(text2, new TableauCacheManager.RenderDetails(new List<Action<Texture>>()));
			}
			this._renderCallbacks[text2].Actions.Add(setAction);
			return texture2;
		}

		public void Tick()
		{
			ThumbnailCache avatarVisuals = this._avatarVisuals;
			if (avatarVisuals != null)
			{
				avatarVisuals.Tick();
			}
			ThumbnailCache itemVisuals = this._itemVisuals;
			if (itemVisuals != null)
			{
				itemVisuals.Tick();
			}
			ThumbnailCache craftingPieceVisuals = this._craftingPieceVisuals;
			if (craftingPieceVisuals != null)
			{
				craftingPieceVisuals.Tick();
			}
			ThumbnailCache characterVisuals = this._characterVisuals;
			if (characterVisuals != null)
			{
				characterVisuals.Tick();
			}
			ThumbnailCache bannerVisuals = this._bannerVisuals;
			if (bannerVisuals == null)
			{
				return;
			}
			bannerVisuals.Tick();
		}

		public void ReleaseTextureWithId(CraftingPiece craftingPiece, string type)
		{
			string text = craftingPiece.StringId + "$" + type;
			this._craftingPieceVisuals.MarkForDeletion(text);
		}

		public void ReleaseTextureWithId(CharacterCode characterCode, bool isBig)
		{
			characterCode.BodyProperties = new BodyProperties(new DynamicBodyProperties((float)((int)characterCode.BodyProperties.Age), (float)((int)characterCode.BodyProperties.Weight), (float)((int)characterCode.BodyProperties.Build)), characterCode.BodyProperties.StaticProperties);
			string text = characterCode.CreateNewCodeString();
			text += (isBig ? "1" : "0");
			this._characterVisuals.MarkForDeletion(text);
		}

		public void ReleaseTextureWithId(ItemObject itemObject)
		{
			string stringId = itemObject.StringId;
			this._itemVisuals.MarkForDeletion(stringId);
		}

		public void ReleaseTextureWithId(BannerCode bannerCode, bool isTableau = false, bool isLarge = false)
		{
			string text = "BannerThumbnail";
			if (isTableau)
			{
				if (isLarge)
				{
					text = "BannerTableauLarge";
				}
				else
				{
					text = "BannerTableauSmall";
				}
			}
			string text2 = text + ":" + bannerCode.Code;
			this._bannerVisuals.MarkForDeletion(text2);
		}

		public void ForceReleaseBanner(BannerCode bannerCode, bool isTableau = false, bool isLarge = false)
		{
			string text = "BannerThumbnail";
			if (isTableau)
			{
				if (isLarge)
				{
					text = "BannerTableauLarge";
				}
				else
				{
					text = "BannerTableauSmall";
				}
			}
			string text2 = text + ":" + bannerCode.Code;
			this._bannerVisuals.ForceDelete(text2);
		}

		private void GetItemPoseAndCameraForCraftedItem(ItemObject item, Scene scene, ref Camera camera, ref MatrixFrame itemFrame, ref MatrixFrame itemFrame1, ref MatrixFrame itemFrame2)
		{
			if (camera == null)
			{
				camera = Camera.CreateCamera();
			}
			itemFrame = MatrixFrame.Identity;
			WeaponClass weaponClass = item.WeaponDesign.Template.WeaponDescriptions[0].WeaponClass;
			Vec3 u = itemFrame.rotation.u;
			Vec3 vec = itemFrame.origin - u * (item.WeaponDesign.CraftedWeaponLength * 0.5f);
			Vec3 vec2 = vec + u * item.WeaponDesign.CraftedWeaponLength;
			Vec3 vec3 = vec - u * item.WeaponDesign.BottomPivotOffset;
			int num = 0;
			Vec3 vec4 = default(Vec3);
			foreach (float num2 in item.WeaponDesign.TopPivotOffsets)
			{
				if (num2 > MathF.Abs(1E-05f))
				{
					Vec3 vec5 = vec + u * num2;
					if (num == 1)
					{
						vec4 = vec5;
					}
					num++;
				}
			}
			if (weaponClass == 2 || weaponClass == 3)
			{
				GameEntity gameEntity = scene.FindEntityWithTag("sword_camera");
				Vec3 vec6 = default(Vec3);
				gameEntity.GetCameraParamsFromCameraScript(camera, ref vec6);
				gameEntity.SetVisibilityExcludeParents(false);
				Vec3 vec7 = itemFrame.TransformToLocal(vec3);
				MatrixFrame identity = MatrixFrame.Identity;
				identity.origin = -vec7;
				GameEntity gameEntity2 = scene.FindEntityWithTag("sword");
				gameEntity2.SetVisibilityExcludeParents(false);
				itemFrame = gameEntity2.GetGlobalFrame();
				itemFrame = itemFrame.TransformToParent(identity);
			}
			if (weaponClass == 4 || weaponClass == 5)
			{
				GameEntity gameEntity3 = scene.FindEntityWithTag("axe_camera");
				Vec3 vec8 = default(Vec3);
				gameEntity3.GetCameraParamsFromCameraScript(camera, ref vec8);
				gameEntity3.SetVisibilityExcludeParents(false);
				Vec3 vec9 = itemFrame.TransformToLocal(vec4);
				MatrixFrame identity2 = MatrixFrame.Identity;
				identity2.origin = -vec9;
				GameEntity gameEntity4 = scene.FindEntityWithTag("axe");
				gameEntity4.SetVisibilityExcludeParents(false);
				itemFrame = gameEntity4.GetGlobalFrame();
				itemFrame = itemFrame.TransformToParent(identity2);
			}
			if (weaponClass == 1)
			{
				GameEntity gameEntity5 = scene.FindEntityWithTag("sword_camera");
				Vec3 vec10 = default(Vec3);
				gameEntity5.GetCameraParamsFromCameraScript(camera, ref vec10);
				gameEntity5.SetVisibilityExcludeParents(false);
				Vec3 vec11 = itemFrame.TransformToLocal(vec3);
				MatrixFrame identity3 = MatrixFrame.Identity;
				identity3.origin = -vec11;
				GameEntity gameEntity6 = scene.FindEntityWithTag("sword");
				gameEntity6.SetVisibilityExcludeParents(false);
				itemFrame = gameEntity6.GetGlobalFrame();
				itemFrame = itemFrame.TransformToParent(identity3);
			}
			if (weaponClass == 19)
			{
				GameEntity gameEntity7 = scene.FindEntityWithTag("throwing_axe_camera");
				Vec3 vec12 = default(Vec3);
				gameEntity7.GetCameraParamsFromCameraScript(camera, ref vec12);
				gameEntity7.SetVisibilityExcludeParents(false);
				Vec3 vec13 = vec + u * item.PrimaryWeapon.CenterOfMass;
				Vec3 vec14 = itemFrame.TransformToLocal(vec13);
				MatrixFrame identity4 = MatrixFrame.Identity;
				identity4.origin = -vec14 * 2.5f;
				GameEntity gameEntity8 = scene.FindEntityWithTag("throwing_axe");
				gameEntity8.SetVisibilityExcludeParents(false);
				itemFrame = gameEntity8.GetGlobalFrame();
				itemFrame = itemFrame.TransformToParent(identity4);
				gameEntity8 = scene.FindEntityWithTag("throwing_axe_1");
				gameEntity8.SetVisibilityExcludeParents(false);
				itemFrame1 = gameEntity8.GetGlobalFrame();
				itemFrame1 = itemFrame1.TransformToParent(identity4);
				gameEntity8 = scene.FindEntityWithTag("throwing_axe_2");
				gameEntity8.SetVisibilityExcludeParents(false);
				itemFrame2 = gameEntity8.GetGlobalFrame();
				itemFrame2 = itemFrame2.TransformToParent(identity4);
			}
			if (weaponClass == 21)
			{
				GameEntity gameEntity9 = scene.FindEntityWithTag("javelin_camera");
				Vec3 vec15 = default(Vec3);
				gameEntity9.GetCameraParamsFromCameraScript(camera, ref vec15);
				gameEntity9.SetVisibilityExcludeParents(false);
				Vec3 vec16 = itemFrame.TransformToLocal(vec4);
				MatrixFrame identity5 = MatrixFrame.Identity;
				identity5.origin = -vec16 * 2.2f;
				GameEntity gameEntity10 = scene.FindEntityWithTag("javelin");
				gameEntity10.SetVisibilityExcludeParents(false);
				itemFrame = gameEntity10.GetGlobalFrame();
				itemFrame = itemFrame.TransformToParent(identity5);
				gameEntity10 = scene.FindEntityWithTag("javelin_1");
				gameEntity10.SetVisibilityExcludeParents(false);
				itemFrame1 = gameEntity10.GetGlobalFrame();
				itemFrame1 = itemFrame1.TransformToParent(identity5);
				gameEntity10 = scene.FindEntityWithTag("javelin_2");
				gameEntity10.SetVisibilityExcludeParents(false);
				itemFrame2 = gameEntity10.GetGlobalFrame();
				itemFrame2 = itemFrame2.TransformToParent(identity5);
			}
			if (weaponClass == 20)
			{
				GameEntity gameEntity11 = scene.FindEntityWithTag("javelin_camera");
				Vec3 vec17 = default(Vec3);
				gameEntity11.GetCameraParamsFromCameraScript(camera, ref vec17);
				gameEntity11.SetVisibilityExcludeParents(false);
				Vec3 vec18 = itemFrame.TransformToLocal(vec2);
				MatrixFrame identity6 = MatrixFrame.Identity;
				identity6.origin = -vec18 * 1.4f;
				GameEntity gameEntity12 = scene.FindEntityWithTag("javelin");
				gameEntity12.SetVisibilityExcludeParents(false);
				itemFrame = gameEntity12.GetGlobalFrame();
				itemFrame = itemFrame.TransformToParent(identity6);
				gameEntity12 = scene.FindEntityWithTag("javelin_1");
				gameEntity12.SetVisibilityExcludeParents(false);
				itemFrame1 = gameEntity12.GetGlobalFrame();
				itemFrame1 = itemFrame1.TransformToParent(identity6);
				gameEntity12 = scene.FindEntityWithTag("javelin_2");
				gameEntity12.SetVisibilityExcludeParents(false);
				itemFrame2 = gameEntity12.GetGlobalFrame();
				itemFrame2 = itemFrame2.TransformToParent(identity6);
			}
			if (weaponClass == 10 || weaponClass == 9 || weaponClass == 11 || weaponClass == 6 || weaponClass == 8)
			{
				GameEntity gameEntity13 = scene.FindEntityWithTag("spear_camera");
				Vec3 vec19 = default(Vec3);
				gameEntity13.GetCameraParamsFromCameraScript(camera, ref vec19);
				gameEntity13.SetVisibilityExcludeParents(false);
				Vec3 vec20 = itemFrame.TransformToLocal(vec4);
				MatrixFrame identity7 = MatrixFrame.Identity;
				identity7.origin = -vec20;
				GameEntity gameEntity14 = scene.FindEntityWithTag("spear");
				gameEntity14.SetVisibilityExcludeParents(false);
				itemFrame = gameEntity14.GetGlobalFrame();
				itemFrame = itemFrame.TransformToParent(identity7);
			}
		}

		private void GetItemPoseAndCamera(ItemObject item, Scene scene, ref Camera camera, ref MatrixFrame itemFrame, ref MatrixFrame itemFrame1, ref MatrixFrame itemFrame2)
		{
			if (item.IsCraftedWeapon)
			{
				this.GetItemPoseAndCameraForCraftedItem(item, scene, ref camera, ref itemFrame, ref itemFrame1, ref itemFrame2);
				return;
			}
			string text = "";
			TableauCacheManager.CustomPoseParameters customPoseParameters = new TableauCacheManager.CustomPoseParameters
			{
				CameraTag = "goods_cam",
				DistanceModifier = 6f,
				FrameTag = "goods_frame"
			};
			if (item.WeaponComponent != null)
			{
				WeaponClass weaponClass = item.WeaponComponent.PrimaryWeapon.WeaponClass;
				if (weaponClass - 2 <= 1)
				{
					text = "sword";
				}
			}
			else
			{
				ItemObject.ItemTypeEnum type = item.Type;
				if (type != 12)
				{
					if (type == 13)
					{
						text = "armor";
					}
				}
				else
				{
					text = "helmet";
				}
			}
			if (item.Type == 7)
			{
				text = "shield";
			}
			if (item.Type == 9)
			{
				text = "crossbow";
			}
			if (item.Type == 8)
			{
				text = "bow";
			}
			if (item.Type == 14)
			{
				text = "boot";
			}
			if (item.Type == 1)
			{
				text = ((HorseComponent)item.ItemComponent).Monster.MonsterUsage;
			}
			if (item.Type == 23)
			{
				text = "horse";
			}
			if (item.Type == 22)
			{
				text = "cape";
			}
			if (item.Type == 15)
			{
				text = "glove";
			}
			if (item.Type == 5)
			{
				text = "arrow";
			}
			if (item.Type == 6)
			{
				text = "bolt";
			}
			if (item.Type == 24)
			{
				customPoseParameters = new TableauCacheManager.CustomPoseParameters
				{
					CameraTag = "banner_cam",
					DistanceModifier = 1.5f,
					FrameTag = "banner_frame",
					FocusAlignment = TableauCacheManager.CustomPoseParameters.Alignment.Top
				};
			}
			if (item.Type == 19)
			{
				customPoseParameters = new TableauCacheManager.CustomPoseParameters
				{
					CameraTag = customPoseParameters.CameraTag,
					DistanceModifier = 3f,
					FrameTag = customPoseParameters.FrameTag
				};
			}
			if (item.StringId == "iron" || item.StringId == "hardwood" || item.StringId == "charcoal" || item.StringId == "ironIngot1" || item.StringId == "ironIngot2" || item.StringId == "ironIngot3" || item.StringId == "ironIngot4" || item.StringId == "ironIngot5" || item.StringId == "ironIngot6" || item.ItemCategory == DefaultItemCategories.Silver)
			{
				text = "craftmat";
			}
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = text + "_cam";
				string text3 = text + "_frame";
				GameEntity gameEntity = scene.FindEntityWithTag(text2);
				if (gameEntity != null)
				{
					camera = Camera.CreateCamera();
					Vec3 vec = default(Vec3);
					gameEntity.GetCameraParamsFromCameraScript(camera, ref vec);
				}
				GameEntity gameEntity2 = scene.FindEntityWithTag(text3);
				if (gameEntity2 != null)
				{
					itemFrame = gameEntity2.GetGlobalFrame();
					gameEntity2.SetVisibilityExcludeParents(false);
				}
			}
			else
			{
				GameEntity gameEntity3 = scene.FindEntityWithTag(customPoseParameters.CameraTag);
				if (gameEntity3 != null)
				{
					camera = Camera.CreateCamera();
					Vec3 vec2 = default(Vec3);
					gameEntity3.GetCameraParamsFromCameraScript(camera, ref vec2);
				}
				GameEntity gameEntity4 = scene.FindEntityWithTag(customPoseParameters.FrameTag);
				if (gameEntity4 != null)
				{
					itemFrame = gameEntity4.GetGlobalFrame();
					gameEntity4.SetVisibilityExcludeParents(false);
					gameEntity4.UpdateGlobalBounds();
					MatrixFrame globalFrame = gameEntity4.GetGlobalFrame();
					MetaMesh itemMeshForInventory = new ItemRosterElement(item, 0, null).GetItemMeshForInventory(false);
					Vec3 vec3;
					vec3..ctor(1000000f, 1000000f, 1000000f, -1f);
					Vec3 vec4;
					vec4..ctor(-1000000f, -1000000f, -1000000f, -1f);
					if (itemMeshForInventory != null)
					{
						MatrixFrame identity = MatrixFrame.Identity;
						for (int num = 0; num != itemMeshForInventory.MeshCount; num++)
						{
							Vec3 boundingBoxMin = itemMeshForInventory.GetMeshAtIndex(num).GetBoundingBoxMin();
							Vec3 boundingBoxMax = itemMeshForInventory.GetMeshAtIndex(num).GetBoundingBoxMax();
							Vec3[] array = new Vec3[]
							{
								globalFrame.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMin.y, boundingBoxMin.z, -1f)),
								globalFrame.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMin.y, boundingBoxMax.z, -1f)),
								globalFrame.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMax.y, boundingBoxMin.z, -1f)),
								globalFrame.TransformToParent(new Vec3(boundingBoxMin.x, boundingBoxMax.y, boundingBoxMax.z, -1f)),
								globalFrame.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMin.y, boundingBoxMin.z, -1f)),
								globalFrame.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMin.y, boundingBoxMax.z, -1f)),
								globalFrame.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMax.y, boundingBoxMin.z, -1f)),
								globalFrame.TransformToParent(new Vec3(boundingBoxMax.x, boundingBoxMax.y, boundingBoxMax.z, -1f))
							};
							for (int i = 0; i < 8; i++)
							{
								vec3 = Vec3.Vec3Min(vec3, array[i]);
								vec4 = Vec3.Vec3Max(vec4, array[i]);
							}
						}
					}
					Vec3 vec5 = (vec3 + vec4) * 0.5f;
					Vec3 vec6 = gameEntity4.GetGlobalFrame().TransformToLocal(vec5);
					MatrixFrame globalFrame2 = gameEntity4.GetGlobalFrame();
					globalFrame2.origin -= vec6;
					itemFrame = globalFrame2;
					MatrixFrame frame = camera.Frame;
					float num2 = (vec4 - vec3).Length * customPoseParameters.DistanceModifier;
					frame.origin += frame.rotation.u * num2;
					if (customPoseParameters.FocusAlignment == TableauCacheManager.CustomPoseParameters.Alignment.Top)
					{
						frame.origin += new Vec3(0f, 0f, (vec4 - vec3).Z * 0.3f, -1f);
					}
					else if (customPoseParameters.FocusAlignment == TableauCacheManager.CustomPoseParameters.Alignment.Bottom)
					{
						frame.origin -= new Vec3(0f, 0f, (vec4 - vec3).Z * 0.3f, -1f);
					}
					camera.Frame = frame;
				}
			}
			if (camera == null)
			{
				camera = Camera.CreateCamera();
				camera.SetViewVolume(false, -1f, 1f, -0.5f, 0.5f, 0.01f, 100f);
				MatrixFrame identity2 = MatrixFrame.Identity;
				identity2.origin -= identity2.rotation.u * 7f;
				identity2.rotation.u = identity2.rotation.u * -1f;
				camera.Frame = identity2;
			}
			if (item.Type == 7)
			{
				GameEntity gameEntity5 = scene.FindEntityWithTag("shield_cam");
				MatrixFrame holsterFrameByIndex = MBItem.GetHolsterFrameByIndex(MBItem.GetItemHolsterIndex(item.ItemHolsters[0]));
				itemFrame.rotation = holsterFrameByIndex.rotation;
				MatrixFrame matrixFrame = itemFrame.TransformToParent(gameEntity5.GetFrame());
				camera.Frame = matrixFrame;
			}
		}

		private GameEntity AddItem(Scene scene, ItemObject item, MatrixFrame itemFrame, MatrixFrame itemFrame1, MatrixFrame itemFrame2)
		{
			ItemRosterElement itemRosterElement;
			itemRosterElement..ctor(item, 0, null);
			MetaMesh itemMeshForInventory = itemRosterElement.GetItemMeshForInventory(false);
			if (item.IsCraftedWeapon)
			{
				MatrixFrame frame = itemMeshForInventory.Frame;
				frame.Elevate(-item.WeaponDesign.CraftedWeaponLength / 2f);
				itemMeshForInventory.Frame = frame;
			}
			GameEntity gameEntity = null;
			if (itemMeshForInventory != null && itemRosterElement.EquipmentElement.Item.ItemType == 15)
			{
				gameEntity = GameEntity.CreateEmpty(scene, true);
				AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(Game.Current.DefaultMonster, MBActionSet.GetActionSet(Game.Current.DefaultMonster.ActionSetCode), 1f, false);
				GameEntityExtensions.CreateSkeletonWithActionSet(gameEntity, ref animationSystemData);
				gameEntity.SetFrame(ref itemFrame);
				MBSkeletonExtensions.SetAgentActionChannel(gameEntity.Skeleton, 0, this.act_tableau_hand_armor_pose, 0f, -0.2f, true);
				gameEntity.AddMultiMeshToSkeleton(itemMeshForInventory);
				gameEntity.Skeleton.TickAnimationsAndForceUpdate(0.01f, itemFrame, true);
			}
			else if (itemMeshForInventory != null)
			{
				if (item.WeaponComponent != null)
				{
					WeaponClass weaponClass = item.WeaponComponent.PrimaryWeapon.WeaponClass;
					if (weaponClass == 19 || weaponClass == 20 || weaponClass == 21 || weaponClass == 13)
					{
						gameEntity = GameEntity.CreateEmpty(scene, true);
						MetaMesh metaMesh = itemMeshForInventory.CreateCopy();
						metaMesh.Frame = itemFrame;
						gameEntity.AddMultiMesh(metaMesh, true);
						MetaMesh metaMesh2 = itemMeshForInventory.CreateCopy();
						metaMesh2.Frame = itemFrame1;
						gameEntity.AddMultiMesh(metaMesh2, true);
						MetaMesh metaMesh3 = itemMeshForInventory.CreateCopy();
						metaMesh3.Frame = itemFrame2;
						gameEntity.AddMultiMesh(metaMesh3, true);
					}
					else
					{
						gameEntity = scene.AddItemEntity(ref itemFrame, itemMeshForInventory);
					}
				}
				else
				{
					gameEntity = scene.AddItemEntity(ref itemFrame, itemMeshForInventory);
					if (item.Type == 23 && item.ArmorComponent != null)
					{
						MetaMesh copy = MetaMesh.GetCopy(item.ArmorComponent.ReinsMesh, true, true);
						if (copy != null)
						{
							gameEntity.AddMultiMesh(copy, true);
						}
					}
				}
			}
			else
			{
				MBDebug.ShowWarning("[DEBUG]Item with " + itemRosterElement.EquipmentElement.Item.StringId + "[DEBUG] string id cannot be found");
			}
			gameEntity.SetVisibilityExcludeParents(false);
			return gameEntity;
		}

		private void GetPoseParamsFromCharacterCode(CharacterCode characterCode, out string poseName, out bool hasHorse)
		{
			hasHorse = false;
			if (characterCode.IsHero)
			{
				int num = MBRandom.NondeterministicRandomInt % 8;
				poseName = "lord_" + num;
				return;
			}
			poseName = "troop_villager";
			int num2 = -1;
			int num3 = -1;
			Equipment equipment = characterCode.CalculateEquipment();
			switch (characterCode.FormationClass)
			{
			case 0:
			case 2:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			{
				for (int i = 0; i < 4; i++)
				{
					ItemObject item = equipment[i].Item;
					if (((item != null) ? item.PrimaryWeapon : null) != null)
					{
						if (num3 == -1 && Extensions.HasAnyFlag<ItemFlags>(equipment[i].Item.ItemFlags, 524288))
						{
							num3 = i;
						}
						if (num2 == -1 && Extensions.HasAnyFlag<WeaponFlags>(equipment[i].Item.PrimaryWeapon.WeaponFlags, 1L))
						{
							num2 = i;
						}
					}
				}
				break;
			}
			case 1:
			case 3:
			{
				for (int j = 0; j < 4; j++)
				{
					ItemObject item2 = equipment[j].Item;
					if (((item2 != null) ? item2.PrimaryWeapon : null) != null)
					{
						if (num3 == -1 && Extensions.HasAnyFlag<ItemFlags>(equipment[j].Item.ItemFlags, 524288))
						{
							num3 = j;
						}
						if (num2 == -1 && Extensions.HasAnyFlag<WeaponFlags>(equipment[j].Item.PrimaryWeapon.WeaponFlags, 2L))
						{
							num2 = j;
						}
					}
				}
				break;
			}
			}
			if (num2 != -1)
			{
				switch (equipment[num2].Item.PrimaryWeapon.WeaponClass)
				{
				case 2:
				case 4:
					if (num3 == -1)
					{
						poseName = "troop_infantry_sword1h";
					}
					else if (equipment[num3].Item.PrimaryWeapon.IsShield)
					{
						poseName = "troop_infantry_sword1h";
					}
					break;
				case 3:
				case 5:
				case 8:
					poseName = "troop_infantry_sword2h";
					break;
				case 9:
				case 10:
					poseName = "troop_spear";
					break;
				case 11:
				case 21:
					poseName = "troop_spear";
					break;
				case 15:
					poseName = "troop_bow";
					break;
				case 16:
					poseName = "troop_crossbow";
					break;
				}
			}
			if (!equipment[10].IsEmpty)
			{
				if (num2 != -1)
				{
					HorseComponent horseComponent = equipment[10].Item.HorseComponent;
					bool flag;
					if (horseComponent == null)
					{
						flag = false;
					}
					else
					{
						Monster monster = horseComponent.Monster;
						int? num4 = ((monster != null) ? new int?(monster.FamilyType) : null);
						int num5 = 2;
						flag = (num4.GetValueOrDefault() == num5) & (num4 != null);
					}
					bool flag2 = flag;
					ItemObject.ItemTypeEnum type = equipment[num2].Item.Type;
					if (type != 2)
					{
						if (type == 8)
						{
							poseName = "troop_cavalry_archer";
						}
						else
						{
							poseName = "troop_cavalry_lance";
						}
					}
					else if (num3 == -1)
					{
						poseName = "troop_cavalry_sword";
					}
					else if (equipment[num3].Item.PrimaryWeapon.IsShield)
					{
						poseName = "troop_cavalry_sword";
					}
					if (flag2)
					{
						poseName = "camel_" + poseName;
					}
				}
				hasHorse = true;
			}
		}

		private GameEntity CreateCraftingPieceBaseEntity(CraftingPiece craftingPiece, string ItemType, Scene scene, ref Camera camera)
		{
			MatrixFrame matrixFrame = MatrixFrame.Identity;
			bool flag = false;
			string text = "craftingPiece_cam";
			string text2 = "craftingPiece_frame";
			if (craftingPiece.PieceType == null)
			{
				if (ItemType == "OneHandedAxe" || ItemType == "ThrowingAxe")
				{
					text = "craft_axe_camera";
					text2 = "craft_axe";
				}
				else if (ItemType == "TwoHandedAxe")
				{
					text = "craft_big_axe_camera";
					text2 = "craft_big_axe";
				}
				else if (ItemType == "Dagger" || ItemType == "ThrowingKnife" || ItemType == "TwoHandedPolearm" || ItemType == "Pike" || ItemType == "Javelin")
				{
					text = "craft_spear_blade_camera";
					text2 = "craft_spear_blade";
				}
				else if (ItemType == "Mace" || ItemType == "TwoHandedMace")
				{
					text = "craft_mace_camera";
					text2 = "craft_mace";
				}
				else
				{
					text = "craft_blade_camera";
					text2 = "craft_blade";
				}
				flag = true;
			}
			else if (craftingPiece.PieceType == 3)
			{
				text = "craft_pommel_camera";
				text2 = "craft_pommel";
				flag = true;
			}
			else if (craftingPiece.PieceType == 1)
			{
				text = "craft_guard_camera";
				text2 = "craft_guard";
				flag = true;
			}
			else if (craftingPiece.PieceType == 2)
			{
				text = "craft_handle_camera";
				text2 = "craft_handle";
				flag = true;
			}
			bool flag2 = false;
			if (flag)
			{
				GameEntity gameEntity = scene.FindEntityWithTag(text);
				if (gameEntity != null)
				{
					camera = Camera.CreateCamera();
					Vec3 vec = default(Vec3);
					gameEntity.GetCameraParamsFromCameraScript(camera, ref vec);
				}
				GameEntity gameEntity2 = scene.FindEntityWithTag(text2);
				if (gameEntity2 != null)
				{
					matrixFrame = gameEntity2.GetGlobalFrame();
					gameEntity2.SetVisibilityExcludeParents(false);
					flag2 = true;
				}
			}
			else
			{
				GameEntity gameEntity3 = scene.FindEntityWithTag("old_system_item_frame");
				if (gameEntity3 != null)
				{
					matrixFrame = gameEntity3.GetGlobalFrame();
					gameEntity3.SetVisibilityExcludeParents(false);
				}
			}
			if (camera == null)
			{
				camera = Camera.CreateCamera();
				camera.SetViewVolume(false, -1f, 1f, -0.5f, 0.5f, 0.01f, 100f);
				MatrixFrame identity = MatrixFrame.Identity;
				identity.origin -= identity.rotation.u * 7f;
				identity.rotation.u = identity.rotation.u * -1f;
				camera.Frame = identity;
			}
			if (!flag2)
			{
				matrixFrame = craftingPiece.GetCraftingPieceFrameForInventory();
			}
			MetaMesh copy = MetaMesh.GetCopy(craftingPiece.MeshName, true, false);
			GameEntity gameEntity4 = null;
			if (copy != null)
			{
				gameEntity4 = scene.AddItemEntity(ref matrixFrame, copy);
			}
			else
			{
				MBDebug.ShowWarning("[DEBUG]craftingPiece with " + craftingPiece.StringId + "[DEBUG] string id cannot be found");
			}
			gameEntity4.SetVisibilityExcludeParents(false);
			return gameEntity4;
		}

		private GameEntity CreateItemBaseEntity(ItemObject item, Scene scene, ref Camera camera)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			MatrixFrame identity2 = MatrixFrame.Identity;
			MatrixFrame identity3 = MatrixFrame.Identity;
			this.GetItemPoseAndCamera(item, scene, ref camera, ref identity, ref identity2, ref identity3);
			return this.AddItem(scene, item, identity, identity2, identity3);
		}

		private GameEntity CreateCharacterBaseEntity(CharacterCode characterCode, Scene scene, ref Camera camera, bool isBig)
		{
			string text;
			bool flag;
			this.GetPoseParamsFromCharacterCode(characterCode, out text, out flag);
			string text2 = text + "_pose";
			string text3 = (isBig ? (text + "_cam") : (text + "_cam_small"));
			GameEntity gameEntity = scene.FindEntityWithTag(text2);
			if (gameEntity == null)
			{
				return null;
			}
			gameEntity.SetVisibilityExcludeParents(true);
			GameEntity gameEntity2 = GameEntity.CopyFromPrefab(gameEntity);
			gameEntity2.Name = gameEntity.Name + "Instance";
			gameEntity2.RemoveTag(text2);
			scene.AttachEntity(gameEntity2, false);
			gameEntity2.SetVisibilityExcludeParents(true);
			gameEntity.SetVisibilityExcludeParents(false);
			GameEntity gameEntity3 = scene.FindEntityWithTag(text3);
			Vec3 vec = default(Vec3);
			camera = Camera.CreateCamera();
			if (gameEntity3 != null)
			{
				gameEntity3.GetCameraParamsFromCameraScript(camera, ref vec);
				camera.Frame = gameEntity3.GetGlobalFrame();
			}
			return gameEntity2;
		}

		private GameEntity FillEntityWithPose(CharacterCode characterCode, GameEntity poseEntity, Scene scene)
		{
			if (characterCode.IsEmpty)
			{
				Debug.FailedAssert("Trying to fill entity with empty character code", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\Tableaus\\TableauCacheManager.cs", "FillEntityWithPose", 1536);
				return poseEntity;
			}
			if (string.IsNullOrEmpty(characterCode.EquipmentCode))
			{
				Debug.FailedAssert("Trying to fill entity with invalid equipment code", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\Tableaus\\TableauCacheManager.cs", "FillEntityWithPose", 1542);
				return poseEntity;
			}
			if (FaceGen.GetBaseMonsterFromRace(characterCode.Race) == null)
			{
				Debug.FailedAssert("There are no monster data for the race: " + characterCode.Race, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\Tableaus\\TableauCacheManager.cs", "FillEntityWithPose", 1549);
				return poseEntity;
			}
			if (TableauCacheManager.Current != null && poseEntity != null)
			{
				string text;
				bool flag;
				this.GetPoseParamsFromCharacterCode(characterCode, out text, out flag);
				CharacterSpawner characterSpawner = poseEntity.GetScriptComponents<CharacterSpawner>().First<CharacterSpawner>();
				characterSpawner.SetCreateFaceImmediately(false);
				characterSpawner.InitWithCharacter(characterCode, false);
			}
			return poseEntity;
		}

		public static Camera CreateDefaultBannerCamera()
		{
			return TableauCacheManager.CreateCamera(0.33333334f, 0.6666667f, -0.6666667f, -0.33333334f, 0.001f, 510f);
		}

		public static Camera CreateNineGridBannerCamera()
		{
			return TableauCacheManager.CreateCamera(0f, 1f, -1f, 0f, 0.001f, 510f);
		}

		private static Camera CreateCamera(float left, float right, float bottom, float top, float near, float far)
		{
			Camera camera = Camera.CreateCamera();
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin.z = 400f;
			camera.Frame = identity;
			camera.LookAt(new Vec3(0f, 0f, 400f, -1f), new Vec3(0f, 0f, 0f, -1f), new Vec3(0f, 1f, 0f, -1f));
			camera.SetViewVolume(false, left, right, bottom, top, near, far);
			return camera;
		}

		private ThumbnailCreatorView _thumbnailCreatorView;

		private Scene _bannerScene;

		private Scene _inventoryScene;

		private bool _inventorySceneBeingUsed;

		private MBAgentRendererSceneController _inventorySceneAgentRenderer;

		private Scene _mapConversationScene;

		private bool _mapConversationSceneBeingUsed;

		private MBAgentRendererSceneController _mapConversationSceneAgentRenderer;

		private Camera _bannerCamera;

		private Camera _nineGridBannerCamera;

		private readonly ActionIndexCache act_tableau_hand_armor_pose = ActionIndexCache.Create("act_tableau_hand_armor_pose");

		private int _characterCount;

		private int _bannerCount;

		private Dictionary<string, TableauCacheManager.RenderDetails> _renderCallbacks;

		private ThumbnailCache _avatarVisuals;

		private ThumbnailCache _itemVisuals;

		private ThumbnailCache _craftingPieceVisuals;

		private ThumbnailCache _characterVisuals;

		private ThumbnailCache _bannerVisuals;

		private int bannerTableauGPUAllocationIndex;

		private int itemTableauGPUAllocationIndex;

		private int characterTableauGPUAllocationIndex;

		private Texture _heroSilhouetteTexture;

		private struct RenderDetails
		{
			public List<Action<Texture>> Actions { get; private set; }

			public RenderDetails(List<Action<Texture>> setActionList)
			{
				this.Actions = setActionList;
			}
		}

		private struct CustomPoseParameters
		{
			public string CameraTag;

			public string FrameTag;

			public float DistanceModifier;

			public TableauCacheManager.CustomPoseParameters.Alignment FocusAlignment;

			public enum Alignment
			{
				Center,
				Top,
				Bottom
			}
		}
	}
}
