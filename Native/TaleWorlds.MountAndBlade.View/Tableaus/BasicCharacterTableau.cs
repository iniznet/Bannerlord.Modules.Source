using System;
using System.Collections.Generic;
using System.Globalization;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View.Tableaus
{
	// Token: 0x0200001F RID: 31
	public class BasicCharacterTableau
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00007F16 File Offset: 0x00006116
		// (set) Token: 0x060000FA RID: 250 RVA: 0x00007F1E File Offset: 0x0000611E
		public Texture Texture { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00007F27 File Offset: 0x00006127
		public bool IsVersionCompatible
		{
			get
			{
				return this._isVersionCompatible;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00007F2F File Offset: 0x0000612F
		private TableauView View
		{
			get
			{
				Texture texture = this.Texture;
				if (texture == null)
				{
					return null;
				}
				return texture.TableauView;
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00007F44 File Offset: 0x00006144
		public BasicCharacterTableau()
		{
			this._isFirstFrame = true;
			this._isVisualsDirty = false;
			this._bodyProperties = BodyProperties.Default;
			this._currentCharacters = new GameEntity[2];
			this._currentCharacters[0] = null;
			this._currentCharacters[1] = null;
			this._currentMounts = new GameEntity[2];
			this._currentMounts[0] = null;
			this._currentMounts[1] = null;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00007FC4 File Offset: 0x000061C4
		public void OnTick(float dt)
		{
			if (this._isEnabled && this._isRotatingCharacter)
			{
				this.UpdateCharacterRotation((int)Input.MouseMoveX);
			}
			TableauView view = this.View;
			if (view != null)
			{
				view.SetDoNotRenderThisFrame(false);
			}
			if (this._isFirstFrame)
			{
				this.FirstTimeInit();
				this._isFirstFrame = false;
			}
			if (this._isVisualsDirty)
			{
				this.RefreshCharacterTableau();
				this._isVisualsDirty = false;
			}
			if (this._checkWhetherEntitiesAreReady)
			{
				int num = (this._currentEntityToShowIndex + 1) % 2;
				bool flag = true;
				if (!this._currentCharacters[this._currentEntityToShowIndex].CheckResources(true, true))
				{
					flag = false;
				}
				if (!this._currentMounts[this._currentEntityToShowIndex].CheckResources(true, true))
				{
					flag = false;
				}
				if (!flag)
				{
					this._currentCharacters[this._currentEntityToShowIndex].SetVisibilityExcludeParents(false);
					this._currentMounts[this._currentEntityToShowIndex].SetVisibilityExcludeParents(false);
					this._currentCharacters[num].SetVisibilityExcludeParents(true);
					this._currentMounts[num].SetVisibilityExcludeParents(true);
					return;
				}
				this._currentCharacters[this._currentEntityToShowIndex].SetVisibilityExcludeParents(true);
				this._currentMounts[this._currentEntityToShowIndex].SetVisibilityExcludeParents(true);
				this._currentCharacters[num].SetVisibilityExcludeParents(false);
				this._currentMounts[num].SetVisibilityExcludeParents(false);
				this._checkWhetherEntitiesAreReady = false;
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00008100 File Offset: 0x00006300
		private void UpdateCharacterRotation(int mouseMoveX)
		{
			if (this._initialized && this._skeletonName != null)
			{
				this._mainCharacterRotation += (float)mouseMoveX * 0.005f;
				MatrixFrame initialSpawnFrame = this._initialSpawnFrame;
				initialSpawnFrame.rotation.RotateAboutUp(this._mainCharacterRotation);
				this._currentCharacters[0].SetFrame(ref initialSpawnFrame);
				this._currentCharacters[1].SetFrame(ref initialSpawnFrame);
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00008169 File Offset: 0x00006369
		private void SetEnabled(bool enabled)
		{
			this._isEnabled = enabled;
			TableauView view = this.View;
			if (view == null)
			{
				return;
			}
			view.SetEnable(this._isEnabled);
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00008188 File Offset: 0x00006388
		public void SetTargetSize(int width, int height)
		{
			this._isRotatingCharacter = false;
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
			this.Texture = TableauView.AddTableau("BasicCharacterTableau", new RenderTargetComponent.TextureUpdateEventHandler(this.CharacterTableauContinuousRenderFunction), this._tableauScene, this._tableauSizeX, this._tableauSizeY);
			this.View.SetScene(this._tableauScene);
			this.View.SetSceneUsesSkybox(false);
			this.View.SetDeleteAfterRendering(false);
			this.View.SetContinuousRendering(true);
			this.View.SetDoNotRenderThisFrame(true);
			this.View.SetClearColor(0U);
			this.View.SetFocusedShadowmap(true, ref this._initialSpawnFrame.origin, 1.55f);
			this.View.SetRenderWithPostfx(true);
			this.SetCamera();
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000082DC File Offset: 0x000064DC
		public void OnFinalize()
		{
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(false);
			}
			TableauView view2 = this.View;
			if (view2 != null)
			{
				view2.AddClearTask(false);
			}
			this.Texture = null;
			this._bannerCode = null;
			if (this._tableauScene != null)
			{
				this._tableauScene = null;
			}
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00008330 File Offset: 0x00006530
		public void DeserializeCharacterCode(string code)
		{
			if (code != this._characterCode)
			{
				if (this._initialized)
				{
					this.ResetProperties();
				}
				this._characterCode = code;
				string[] array = code.Split(new char[] { '|' });
				int num;
				if (int.TryParse(array[0], out num) && 4 == num)
				{
					this._isVersionCompatible = true;
					int num2 = 0;
					try
					{
						num2++;
						this._skeletonName = array[num2];
						num2++;
						Enum.TryParse<SkinMask>(array[num2], false, out this._skinMeshesMask);
						num2++;
						bool.TryParse(array[num2], out this._isFemale);
						num2++;
						this._race = int.Parse(array[num2]);
						num2++;
						Enum.TryParse<Equipment.UnderwearTypes>(array[num2], false, out this._underwearType);
						num2++;
						Enum.TryParse<ArmorComponent.BodyMeshTypes>(array[num2], false, out this._bodyMeshType);
						num2++;
						Enum.TryParse<ArmorComponent.HairCoverTypes>(array[num2], false, out this._hairCoverType);
						num2++;
						Enum.TryParse<ArmorComponent.BeardCoverTypes>(array[num2], false, out this._beardCoverType);
						num2++;
						Enum.TryParse<ArmorComponent.BodyDeformTypes>(array[num2], false, out this._bodyDeformType);
						num2++;
						float.TryParse(array[num2], NumberStyles.Any, CultureInfo.InvariantCulture, out this._faceDirtAmount);
						num2++;
						BodyProperties.FromString(array[num2], ref this._bodyProperties);
						num2++;
						uint.TryParse(array[num2], out this._clothColor1);
						num2++;
						uint.TryParse(array[num2], out this._clothColor2);
						this._equipmentMeshes = new List<string>();
						this._equipmentHasColors = new List<bool>();
						this._equipmentHasGenderVariations = new List<bool>();
						this._equipmentHasTableau = new List<bool>();
						for (EquipmentIndex equipmentIndex = 5; equipmentIndex < 10; equipmentIndex++)
						{
							num2++;
							this._equipmentMeshes.Add(array[num2]);
							num2++;
							bool flag;
							bool.TryParse(array[num2], out flag);
							this._equipmentHasColors.Add(flag);
							num2++;
							bool flag2;
							bool.TryParse(array[num2], out flag2);
							this._equipmentHasGenderVariations.Add(flag2);
							num2++;
							bool flag3;
							bool.TryParse(array[num2], out flag3);
							this._equipmentHasTableau.Add(flag3);
						}
						num2++;
						this._mountSkeletonName = array[num2];
						num2++;
						this._mountMeshName = array[num2];
						num2++;
						this._mountCreationKey = array[num2];
						num2++;
						this._mountMaterialName = array[num2];
						num2++;
						if (array[num2].Length > 0)
						{
							uint.TryParse(array[num2], out this._mountManeMeshMultiplier);
						}
						else
						{
							this._mountManeMeshMultiplier = 0U;
						}
						num2++;
						this._mountIdleAnimationName = array[num2];
						num2++;
						this._mountHarnessMeshName = array[num2];
						num2++;
						bool.TryParse(array[num2], out this._mountHarnessHasColors);
						num2++;
						this._mountReinsMeshName = array[num2];
						num2++;
						int num3 = int.Parse(array[num2]);
						this._maneMeshNames = new string[num3];
						for (int i = 0; i < num3; i++)
						{
							num2++;
							this._maneMeshNames[i] = array[num2];
						}
					}
					catch (Exception ex)
					{
						this.ResetProperties();
						Debug.FailedAssert("Exception: " + ex.Message, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\Tableaus\\BasicCharacterTableau.cs", "DeserializeCharacterCode", 348);
						Debug.FailedAssert("Couldn't parse character code: " + code, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.View\\Tableaus\\BasicCharacterTableau.cs", "DeserializeCharacterCode", 349);
					}
				}
				this._isVisualsDirty = true;
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00008678 File Offset: 0x00006878
		private void ResetProperties()
		{
			this._skeletonName = string.Empty;
			this._skinMeshesMask = 0;
			this._isFemale = false;
			this._underwearType = 0;
			this._bodyMeshType = 0;
			this._hairCoverType = 0;
			this._beardCoverType = 0;
			this._bodyDeformType = 0;
			this._faceDirtAmount = 0f;
			this._bodyProperties = BodyProperties.Default;
			this._clothColor1 = 0U;
			this._clothColor2 = 0U;
			List<string> equipmentMeshes = this._equipmentMeshes;
			if (equipmentMeshes != null)
			{
				equipmentMeshes.Clear();
			}
			List<bool> equipmentHasColors = this._equipmentHasColors;
			if (equipmentHasColors != null)
			{
				equipmentHasColors.Clear();
			}
			this._mountSkeletonName = string.Empty;
			this._mountMeshName = string.Empty;
			this._mountCreationKey = string.Empty;
			this._mountMaterialName = string.Empty;
			this._mountManeMeshMultiplier = uint.MaxValue;
			this._mountIdleAnimationName = string.Empty;
			this._mountHarnessMeshName = string.Empty;
			this._mountReinsMeshName = string.Empty;
			this._maneMeshNames = null;
			this._mountHarnessHasColors = false;
			this._race = 0;
			this._isVersionCompatible = false;
			this._characterCode = string.Empty;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00008784 File Offset: 0x00006984
		private void FirstTimeInit()
		{
			if (this._tableauScene == null)
			{
				this._tableauScene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
				this._tableauScene.SetName("BasicCharacterTableau");
				this._tableauScene.DisableStaticShadows(true);
				SceneInitializationData sceneInitializationData;
				sceneInitializationData..ctor(true);
				sceneInitializationData.InitPhysicsWorld = false;
				sceneInitializationData.DoNotUseLoadingScreen = true;
				SceneInitializationData sceneInitializationData2 = sceneInitializationData;
				this._tableauScene.Read("inventory_character_scene", ref sceneInitializationData2, "");
				this._tableauScene.SetShadow(true);
				this._mountSpawnPoint = this._tableauScene.FindEntityWithTag("horse_inv").GetGlobalFrame();
				this._initialSpawnFrame = this._tableauScene.FindEntityWithTag("agent_inv").GetGlobalFrame();
				this._tableauScene.EnsurePostfxSystem();
				this._tableauScene.SetDofMode(false);
				this._tableauScene.SetMotionBlurMode(false);
				this._tableauScene.SetBloom(true);
				this._tableauScene.SetDynamicShadowmapCascadesRadiusMultiplier(0.31f);
				this._tableauScene.RemoveEntity(this._tableauScene.FindEntityWithTag("agent_inv"), 99);
				this._tableauScene.RemoveEntity(this._tableauScene.FindEntityWithTag("horse_inv"), 100);
				this._currentCharacters[0] = GameEntity.CreateEmpty(this._tableauScene, false);
				this._currentCharacters[1] = GameEntity.CreateEmpty(this._tableauScene, false);
				this._currentMounts[0] = GameEntity.CreateEmpty(this._tableauScene, false);
				this._currentMounts[1] = GameEntity.CreateEmpty(this._tableauScene, false);
			}
			this.SetEnabled(true);
			this._initialized = true;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000891C File Offset: 0x00006B1C
		private static void ApplyBannerTextureToMesh(Mesh armorMesh, Texture bannerTexture)
		{
			if (armorMesh != null)
			{
				Material material = armorMesh.GetMaterial().CreateCopy();
				material.SetTexture(1, bannerTexture);
				uint num = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
				ulong shaderFlags = material.GetShaderFlags();
				material.SetShaderFlags(shaderFlags | (ulong)num);
				armorMesh.SetMaterial(material);
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00008974 File Offset: 0x00006B74
		private void RefreshCharacterTableau()
		{
			if (!this._initialized)
			{
				return;
			}
			this._currentEntityToShowIndex = (this._currentEntityToShowIndex + 1) % 2;
			GameEntity gameEntity = this._currentCharacters[this._currentEntityToShowIndex];
			gameEntity.ClearEntityComponents(true, true, true);
			GameEntity gameEntity2 = this._currentMounts[this._currentEntityToShowIndex];
			gameEntity2.ClearEntityComponents(true, true, true);
			this._mainCharacterRotation = 0f;
			if (!string.IsNullOrEmpty(this._skeletonName))
			{
				AnimationSystemData hardcodedAnimationSystemDataForHumanSkeleton = AnimationSystemData.GetHardcodedAnimationSystemDataForHumanSkeleton();
				bool flag = this._bodyProperties.Age >= 14f && this._isFemale;
				gameEntity.Skeleton = MBSkeletonExtensions.CreateWithActionSet(ref hardcodedAnimationSystemDataForHumanSkeleton);
				MetaMesh metaMesh = null;
				bool flag2 = this._equipmentMeshes[3].Length > 0;
				for (int i = 0; i < 5; i++)
				{
					string text = this._equipmentMeshes[i];
					if (text.Length > 0)
					{
						bool flag3 = flag && this._equipmentHasGenderVariations[i];
						MetaMesh metaMesh2 = MetaMesh.GetCopy(flag3 ? (text + "_female") : (text + "_male"), false, true);
						if (metaMesh2 == null)
						{
							string text2 = text;
							if (flag3)
							{
								text2 += (flag2 ? "_converted_slim" : "_converted");
							}
							else
							{
								text2 += (flag2 ? "_slim" : "");
							}
							metaMesh2 = MetaMesh.GetCopy(text2, false, true) ?? MetaMesh.GetCopy(text, false, true);
						}
						if (metaMesh2 != null)
						{
							if (i == 3)
							{
								metaMesh = metaMesh2;
							}
							gameEntity.AddMultiMeshToSkeleton(metaMesh2);
							if (this._equipmentHasTableau[i])
							{
								for (int j = 0; j < metaMesh2.MeshCount; j++)
								{
									Mesh currentMesh = metaMesh2.GetMeshAtIndex(j);
									Mesh currentMesh3 = currentMesh;
									if (currentMesh3 != null && !currentMesh3.HasTag("dont_use_tableau"))
									{
										Mesh currentMesh2 = currentMesh;
										if (currentMesh2 != null && currentMesh2.HasTag("banner_replacement_mesh") && this._bannerCode != null)
										{
											TableauCacheManager.Current.BeginCreateBannerTexture(this._bannerCode, delegate(Texture t)
											{
												BasicCharacterTableau.ApplyBannerTextureToMesh(currentMesh, t);
											}, true, false);
											break;
										}
									}
								}
							}
							else if (this._equipmentHasColors[i])
							{
								for (int k = 0; k < metaMesh2.MeshCount; k++)
								{
									Mesh meshAtIndex = metaMesh2.GetMeshAtIndex(k);
									if (!meshAtIndex.HasTag("no_team_color"))
									{
										meshAtIndex.Color = this._clothColor1;
										meshAtIndex.Color2 = this._clothColor2;
										Material material = meshAtIndex.GetMaterial().CreateCopy();
										material.AddMaterialShaderFlag("use_double_colormap_with_mask_texture", false);
										meshAtIndex.SetMaterial(material);
									}
								}
							}
							metaMesh2.ManualInvalidate();
						}
					}
				}
				gameEntity.SetGlobalFrame(ref this._initialSpawnFrame);
				SkinGenerationParams skinGenerationParams;
				skinGenerationParams..ctor(this._skinMeshesMask, this._underwearType, this._bodyMeshType, this._hairCoverType, this._beardCoverType, this._bodyDeformType, true, this._faceDirtAmount, flag ? 1 : 0, this._race, false, false);
				MBAgentVisuals.FillEntityWithBodyMeshesWithoutAgentVisuals(gameEntity, skinGenerationParams, this._bodyProperties, metaMesh);
				MBSkeletonExtensions.SetAgentActionChannel(gameEntity.Skeleton, 0, ActionIndexCache.Create("act_inventory_idle"), 0f, -0.2f, true);
				gameEntity.SetEnforcedMaximumLodLevel(0);
				gameEntity.CheckResources(true, true);
				if (this._mountMeshName.Length > 0)
				{
					gameEntity2.Skeleton = Skeleton.CreateFromModel(this._mountSkeletonName);
					MetaMesh copy = MetaMesh.GetCopy(this._mountMeshName, true, false);
					if (copy != null)
					{
						MountCreationKey mountCreationKey = MountCreationKey.FromString(this._mountCreationKey);
						MountVisualCreator.SetHorseColors(copy, mountCreationKey);
						if (!string.IsNullOrEmpty(this._mountMaterialName))
						{
							Material fromResource = Material.GetFromResource(this._mountMaterialName);
							copy.SetMaterialToSubMeshesWithTag(fromResource, "horse_body");
							copy.SetFactorColorToSubMeshesWithTag(this._mountManeMeshMultiplier, "horse_tail");
						}
						gameEntity2.AddMultiMeshToSkeleton(copy);
						copy.ManualInvalidate();
						if (this._mountHarnessMeshName.Length > 0)
						{
							MetaMesh copy2 = MetaMesh.GetCopy(this._mountHarnessMeshName, false, true);
							if (copy2 != null)
							{
								if (this._mountReinsMeshName.Length > 0)
								{
									MetaMesh copy3 = MetaMesh.GetCopy(this._mountReinsMeshName, false, true);
									if (copy3 != null)
									{
										gameEntity2.AddMultiMeshToSkeleton(copy3);
										copy3.ManualInvalidate();
									}
								}
								gameEntity2.AddMultiMeshToSkeleton(copy2);
								if (this._mountHarnessHasColors)
								{
									for (int l = 0; l < copy2.MeshCount; l++)
									{
										Mesh meshAtIndex2 = copy2.GetMeshAtIndex(l);
										if (!meshAtIndex2.HasTag("no_team_color"))
										{
											meshAtIndex2.Color = this._clothColor1;
											meshAtIndex2.Color2 = this._clothColor2;
											Material material2 = meshAtIndex2.GetMaterial().CreateCopy();
											material2.AddMaterialShaderFlag("use_double_colormap_with_mask_texture", false);
											meshAtIndex2.SetMaterial(material2);
										}
									}
								}
								copy2.ManualInvalidate();
							}
						}
					}
					string[] maneMeshNames = this._maneMeshNames;
					for (int m = 0; m < maneMeshNames.Length; m++)
					{
						MetaMesh copy4 = MetaMesh.GetCopy(maneMeshNames[m], false, true);
						if (this._mountManeMeshMultiplier != 4294967295U)
						{
							copy4.SetFactor1Linear(this._mountManeMeshMultiplier);
						}
						gameEntity2.AddMultiMeshToSkeleton(copy4);
						copy4.ManualInvalidate();
					}
					gameEntity2.SetGlobalFrame(ref this._mountSpawnPoint);
					MBSkeletonExtensions.SetAnimationAtChannel(gameEntity2.Skeleton, this._mountIdleAnimationName, 0, 1f, 0f, 0f);
					gameEntity2.SetEnforcedMaximumLodLevel(0);
					gameEntity2.CheckResources(true, true);
				}
			}
			this._currentCharacters[this._currentEntityToShowIndex].SetVisibilityExcludeParents(false);
			this._currentMounts[this._currentEntityToShowIndex].SetVisibilityExcludeParents(false);
			int num = (this._currentEntityToShowIndex + 1) % 2;
			this._currentCharacters[num].SetVisibilityExcludeParents(true);
			this._currentMounts[num].SetVisibilityExcludeParents(true);
			this._checkWhetherEntitiesAreReady = true;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00008F40 File Offset: 0x00007140
		internal void SetCamera()
		{
			Camera camera = Camera.CreateCamera();
			camera.Frame = this._tableauScene.FindEntityWithTag("camera_instance").GetGlobalFrame();
			camera.SetFovVertical(0.7853982f, this._cameraRatio, 0.2f, 200f);
			this.View.SetCamera(camera);
			camera.ManualInvalidate();
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00008F9B File Offset: 0x0000719B
		public void RotateCharacter(bool value)
		{
			this._isRotatingCharacter = value;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00008FA4 File Offset: 0x000071A4
		public void SetBannerCode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this._bannerCode = null;
				return;
			}
			this._bannerCode = BannerCode.CreateFrom(value);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00008FC4 File Offset: 0x000071C4
		internal void CharacterTableauContinuousRenderFunction(Texture sender, EventArgs e)
		{
			NativeObject nativeObject = (Scene)sender.UserData;
			TableauView tableauView = sender.TableauView;
			if (nativeObject == null)
			{
				tableauView.SetContinuousRendering(false);
				tableauView.SetDeleteAfterRendering(true);
			}
		}

		// Token: 0x0400004D RID: 77
		private bool _isVersionCompatible;

		// Token: 0x0400004E RID: 78
		private const int _expectedCharacterCodeVersion = 4;

		// Token: 0x0400004F RID: 79
		private bool _initialized;

		// Token: 0x04000050 RID: 80
		private int _tableauSizeX;

		// Token: 0x04000051 RID: 81
		private int _tableauSizeY;

		// Token: 0x04000052 RID: 82
		private float RenderScale = 1f;

		// Token: 0x04000053 RID: 83
		private float _cameraRatio;

		// Token: 0x04000054 RID: 84
		private List<string> _equipmentMeshes;

		// Token: 0x04000055 RID: 85
		private List<bool> _equipmentHasColors;

		// Token: 0x04000056 RID: 86
		private List<bool> _equipmentHasGenderVariations;

		// Token: 0x04000057 RID: 87
		private List<bool> _equipmentHasTableau;

		// Token: 0x04000058 RID: 88
		private uint _clothColor1;

		// Token: 0x04000059 RID: 89
		private uint _clothColor2;

		// Token: 0x0400005A RID: 90
		private MatrixFrame _mountSpawnPoint;

		// Token: 0x0400005B RID: 91
		private MatrixFrame _initialSpawnFrame;

		// Token: 0x0400005C RID: 92
		private Scene _tableauScene;

		// Token: 0x0400005D RID: 93
		private SkinMask _skinMeshesMask;

		// Token: 0x0400005E RID: 94
		private bool _isFemale;

		// Token: 0x0400005F RID: 95
		private string _skeletonName;

		// Token: 0x04000060 RID: 96
		private string _characterCode;

		// Token: 0x04000061 RID: 97
		private Equipment.UnderwearTypes _underwearType;

		// Token: 0x04000062 RID: 98
		private string _mountMeshName;

		// Token: 0x04000063 RID: 99
		private string _mountCreationKey;

		// Token: 0x04000064 RID: 100
		private string _mountMaterialName;

		// Token: 0x04000065 RID: 101
		private uint _mountManeMeshMultiplier;

		// Token: 0x04000066 RID: 102
		private ArmorComponent.BodyMeshTypes _bodyMeshType;

		// Token: 0x04000067 RID: 103
		private ArmorComponent.HairCoverTypes _hairCoverType;

		// Token: 0x04000068 RID: 104
		private ArmorComponent.BeardCoverTypes _beardCoverType;

		// Token: 0x04000069 RID: 105
		private ArmorComponent.BodyDeformTypes _bodyDeformType;

		// Token: 0x0400006A RID: 106
		private string _mountSkeletonName;

		// Token: 0x0400006B RID: 107
		private string _mountIdleAnimationName;

		// Token: 0x0400006C RID: 108
		private string _mountHarnessMeshName;

		// Token: 0x0400006D RID: 109
		private string _mountReinsMeshName;

		// Token: 0x0400006E RID: 110
		private string[] _maneMeshNames;

		// Token: 0x0400006F RID: 111
		private bool _mountHarnessHasColors;

		// Token: 0x04000070 RID: 112
		private bool _isFirstFrame;

		// Token: 0x04000071 RID: 113
		private float _faceDirtAmount;

		// Token: 0x04000072 RID: 114
		private float _mainCharacterRotation;

		// Token: 0x04000073 RID: 115
		private bool _isVisualsDirty;

		// Token: 0x04000074 RID: 116
		private bool _isRotatingCharacter;

		// Token: 0x04000075 RID: 117
		private bool _isEnabled;

		// Token: 0x04000076 RID: 118
		private int _race;

		// Token: 0x04000077 RID: 119
		private readonly GameEntity[] _currentCharacters;

		// Token: 0x04000078 RID: 120
		private readonly GameEntity[] _currentMounts;

		// Token: 0x04000079 RID: 121
		private int _currentEntityToShowIndex;

		// Token: 0x0400007A RID: 122
		private bool _checkWhetherEntitiesAreReady;

		// Token: 0x0400007B RID: 123
		private BodyProperties _bodyProperties = BodyProperties.Default;

		// Token: 0x0400007C RID: 124
		private BannerCode _bannerCode;
	}
}
