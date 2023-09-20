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
	public class BasicCharacterTableau
	{
		public Texture Texture { get; private set; }

		public bool IsVersionCompatible
		{
			get
			{
				return this._isVersionCompatible;
			}
		}

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

		internal void SetCamera()
		{
			Camera camera = Camera.CreateCamera();
			camera.Frame = this._tableauScene.FindEntityWithTag("camera_instance").GetGlobalFrame();
			camera.SetFovVertical(0.7853982f, this._cameraRatio, 0.2f, 200f);
			this.View.SetCamera(camera);
			camera.ManualInvalidate();
		}

		public void RotateCharacter(bool value)
		{
			this._isRotatingCharacter = value;
		}

		public void SetBannerCode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this._bannerCode = null;
				return;
			}
			this._bannerCode = BannerCode.CreateFrom(value);
		}

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

		private bool _isVersionCompatible;

		private const int _expectedCharacterCodeVersion = 4;

		private bool _initialized;

		private int _tableauSizeX;

		private int _tableauSizeY;

		private float RenderScale = 1f;

		private float _cameraRatio;

		private List<string> _equipmentMeshes;

		private List<bool> _equipmentHasColors;

		private List<bool> _equipmentHasGenderVariations;

		private List<bool> _equipmentHasTableau;

		private uint _clothColor1;

		private uint _clothColor2;

		private MatrixFrame _mountSpawnPoint;

		private MatrixFrame _initialSpawnFrame;

		private Scene _tableauScene;

		private SkinMask _skinMeshesMask;

		private bool _isFemale;

		private string _skeletonName;

		private string _characterCode;

		private Equipment.UnderwearTypes _underwearType;

		private string _mountMeshName;

		private string _mountCreationKey;

		private string _mountMaterialName;

		private uint _mountManeMeshMultiplier;

		private ArmorComponent.BodyMeshTypes _bodyMeshType;

		private ArmorComponent.HairCoverTypes _hairCoverType;

		private ArmorComponent.BeardCoverTypes _beardCoverType;

		private ArmorComponent.BodyDeformTypes _bodyDeformType;

		private string _mountSkeletonName;

		private string _mountIdleAnimationName;

		private string _mountHarnessMeshName;

		private string _mountReinsMeshName;

		private string[] _maneMeshNames;

		private bool _mountHarnessHasColors;

		private bool _isFirstFrame;

		private float _faceDirtAmount;

		private float _mainCharacterRotation;

		private bool _isVisualsDirty;

		private bool _isRotatingCharacter;

		private bool _isEnabled;

		private int _race;

		private readonly GameEntity[] _currentCharacters;

		private readonly GameEntity[] _currentMounts;

		private int _currentEntityToShowIndex;

		private bool _checkWhetherEntitiesAreReady;

		private BodyProperties _bodyProperties = BodyProperties.Default;

		private BannerCode _bannerCode;
	}
}
