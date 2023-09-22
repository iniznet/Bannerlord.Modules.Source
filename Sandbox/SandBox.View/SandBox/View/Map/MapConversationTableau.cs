using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Tableaus;

namespace SandBox.View.Map
{
	public class MapConversationTableau
	{
		public Texture Texture { get; private set; }

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

		public MapConversationTableau()
		{
			this._changeIdleActionTimer = new Timer(Game.Current.ApplicationTime, 8f, true);
			this._agentVisuals = new List<AgentVisuals>();
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(this._isEnabled);
			}
			this._dataProvider = SandBoxViewSubModule.MapConversationDataProvider;
		}

		public void SetEnabled(bool enabled)
		{
			if (this._isEnabled != enabled)
			{
				if (enabled)
				{
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
					this.Texture = TableauView.AddTableau("MapConvTableau", new RenderTargetComponent.TextureUpdateEventHandler(this.CharacterTableauContinuousRenderFunction), this._tableauScene, this._tableauSizeX, this._tableauSizeY);
					this.Texture.TableauView.SetSceneUsesContour(false);
				}
				else
				{
					TableauView view3 = this.View;
					if (view3 != null)
					{
						view3.SetEnable(false);
					}
					TableauView view4 = this.View;
					if (view4 != null)
					{
						view4.ClearAll(false, false);
					}
				}
				this._isEnabled = enabled;
			}
		}

		public void SetData(object data)
		{
			if (this._data != null)
			{
				this._initialized = false;
				foreach (AgentVisuals agentVisuals in this._agentVisuals)
				{
					agentVisuals.Reset();
				}
				this._agentVisuals.Clear();
			}
			this._data = data as MapConversationTableauData;
		}

		public void SetTargetSize(int width, int height)
		{
			int num;
			int num2;
			if (width <= 0 || height <= 0)
			{
				num = 10;
				num2 = 10;
			}
			else
			{
				this.RenderScale = NativeOptions.GetConfig(25) / 100f;
				num = (int)((float)width * this.RenderScale);
				num2 = (int)((float)height * this.RenderScale);
			}
			if (num != this._tableauSizeX || num2 != this._tableauSizeY)
			{
				this._tableauSizeX = num;
				this._tableauSizeY = num2;
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
				this.Texture = TableauView.AddTableau("MapConvTableau", new RenderTargetComponent.TextureUpdateEventHandler(this.CharacterTableauContinuousRenderFunction), this._tableauScene, this._tableauSizeX, this._tableauSizeY);
			}
		}

		public void OnFinalize(bool clearNextFrame)
		{
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(false);
			}
			this.RemovePreviousAgentsSoundEvent();
			this.StopConversationSoundEvent();
			Camera continuousRenderCamera = this._continuousRenderCamera;
			if (continuousRenderCamera != null)
			{
				continuousRenderCamera.ReleaseCameraEntity();
			}
			this._continuousRenderCamera = null;
			foreach (AgentVisuals agentVisuals in this._agentVisuals)
			{
				agentVisuals.ResetNextFrame();
			}
			this._agentVisuals = null;
			if (clearNextFrame)
			{
				this.View.AddClearTask(true);
				this.Texture.ReleaseNextFrame();
			}
			else
			{
				this.View.ClearAll(false, false);
				this.Texture.Release();
			}
			this.Texture = null;
			IEnumerable<GameEntity> enumerable = this._tableauScene.FindEntitiesWithTag(this._cachedAtmosphereName);
			this._cachedAtmosphereName = "";
			foreach (GameEntity gameEntity in enumerable)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
			TableauCacheManager.Current.ReturnCachedMapConversationTableauScene();
			this._tableauScene = null;
		}

		public void OnTick(float dt)
		{
			if (this._data != null && !this._initialized)
			{
				this.FirstTimeInit();
				MapScreen instance = MapScreen.Instance;
				((instance != null) ? instance.GetMapView<MapConversationView>() : null).ConversationMission.SetConversationTableau(this);
			}
			if (this._conversationSoundEvent != null && !this._conversationSoundEvent.IsPlaying())
			{
				this.RemovePreviousAgentsSoundEvent();
				this._conversationSoundEvent.Stop();
				this._conversationSoundEvent = null;
			}
			if (this._animationFrequencyThreshold > this._animationGap)
			{
				this._animationGap += dt;
			}
			TableauView view = this.View;
			if (view != null)
			{
				if (this._continuousRenderCamera == null)
				{
					this._continuousRenderCamera = Camera.CreateCamera();
				}
				view.SetDoNotRenderThisFrame(false);
			}
			if (this._agentVisuals != null && this._agentVisuals.Count > 0)
			{
				this._agentVisuals[0].TickVisuals();
			}
			if (this._agentVisuals[0].GetEquipment().CalculateEquipmentCode() != this._opponentLeaderEquipmentCache)
			{
				this._initialized = false;
				foreach (AgentVisuals agentVisuals in this._agentVisuals)
				{
					agentVisuals.Reset();
				}
				this._agentVisuals.Clear();
			}
		}

		private void FirstTimeInit()
		{
			if (this._tableauScene == null)
			{
				this._tableauScene = TableauCacheManager.Current.GetCachedMapConversationTableauScene();
			}
			string atmosphereNameFromData = this._dataProvider.GetAtmosphereNameFromData(this._data);
			this._tableauScene.SetAtmosphereWithName(atmosphereNameFromData);
			IEnumerable<GameEntity> enumerable = this._tableauScene.FindEntitiesWithTag(atmosphereNameFromData);
			this._cachedAtmosphereName = atmosphereNameFromData;
			foreach (GameEntity gameEntity in enumerable)
			{
				gameEntity.SetVisibilityExcludeParents(true);
			}
			if (this._continuousRenderCamera == null)
			{
				this._continuousRenderCamera = Camera.CreateCamera();
				this._cameraEntity = this._tableauScene.FindEntityWithTag("player_infantry_to_infantry");
				Vec3 vec = default(Vec3);
				this._cameraEntity.GetCameraParamsFromCameraScript(this._continuousRenderCamera, ref vec);
				this._baseCameraFOV = this._continuousRenderCamera.HorizontalFov;
			}
			this.SpawnOpponentLeader();
			PartyBase party = this._data.ConversationPartnerData.Party;
			bool flag;
			if (party == null)
			{
				flag = false;
			}
			else
			{
				TroopRoster memberRoster = party.MemberRoster;
				int? num = ((memberRoster != null) ? new int?(memberRoster.TotalManCount) : null);
				int num2 = 1;
				flag = (num.GetValueOrDefault() > num2) & (num != null);
			}
			if (flag)
			{
				int num3 = MathF.Min(2, this._data.ConversationPartnerData.Party.MemberRoster.ToFlattenedRoster().Count<FlattenedTroopRosterElement>() - 1);
				IOrderedEnumerable<TroopRosterElement> orderedEnumerable = from t in this._data.ConversationPartnerData.Party.MemberRoster.GetTroopRoster()
					orderby t.Character.Level descending
					select t;
				foreach (TroopRosterElement troopRosterElement in orderedEnumerable)
				{
					CharacterObject character = troopRosterElement.Character;
					if (character != this._data.ConversationPartnerData.Character && !character.IsPlayerCharacter)
					{
						num3--;
						this.SpawnOpponentBodyguardCharacter(character, num3);
					}
					if (num3 == 0)
					{
						break;
					}
				}
				if (num3 == 1)
				{
					num3--;
					this.SpawnOpponentBodyguardCharacter(orderedEnumerable.First((TroopRosterElement troop) => !troop.Character.IsHero).Character, num3);
				}
			}
			this._agentVisuals.ForEach(delegate(AgentVisuals a)
			{
				a.SetAgentLodZeroOrMaxExternal(true);
			});
			this._tableauScene.ForceLoadResources();
			this._cameraRatio = Screen.RealScreenResolutionWidth / Screen.RealScreenResolutionHeight;
			this.SetTargetSize((int)Screen.RealScreenResolutionWidth, (int)Screen.RealScreenResolutionHeight);
			uint num4 = uint.MaxValue;
			num4 &= 4294966271U;
			TableauView view = this.View;
			if (view != null)
			{
				view.SetPostfxConfigParams((int)num4);
			}
			this._tableauScene.FindEntityWithTag(this.RainingEntityTag).SetVisibilityExcludeParents(this._data.IsRaining);
			this._tableauScene.FindEntityWithTag(this.SnowingEntityTag).SetVisibilityExcludeParents(this._data.IsSnowing);
			this._tableauScene.Tick(3f);
			TableauView view2 = this.View;
			if (view2 != null)
			{
				view2.SetEnable(true);
			}
			this._initialized = true;
		}

		private void SpawnOpponentLeader()
		{
			CharacterObject character = this._data.ConversationPartnerData.Character;
			if (character != null)
			{
				GameEntity gameEntity = this._tableauScene.FindEntityWithTag("player_infantry_spawn");
				MapConversationTableau.DefaultConversationAnimationData defaultAnimForCharacter = this.GetDefaultAnimForCharacter(character, false);
				this._opponentLeaderEquipmentCache = null;
				Equipment equipment;
				if (this._data.ConversationPartnerData.IsCivilianEquipmentRequiredForLeader)
				{
					equipment = (this._data.ConversationPartnerData.Character.IsHero ? character.FirstCivilianEquipment : character.CivilianEquipments.ElementAt(this._data.ConversationPartnerData.Character.GetDefaultFaceSeed(0) % character.CivilianEquipments.Count<Equipment>()));
				}
				else
				{
					equipment = (this._data.ConversationPartnerData.Character.IsHero ? character.FirstBattleEquipment : character.BattleEquipments.ElementAt(this._data.ConversationPartnerData.Character.GetDefaultFaceSeed(0) % character.BattleEquipments.Count<Equipment>()));
				}
				equipment = equipment.Clone(false);
				for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 12; equipmentIndex++)
				{
					if (!equipment[equipmentIndex].IsEmpty && equipment[equipmentIndex].Item.Type == 24)
					{
						equipment[equipmentIndex] = EquipmentElement.Invalid;
						break;
					}
				}
				int num = -1;
				if (this._data.ConversationPartnerData.Party != null)
				{
					num = CharacterHelper.GetPartyMemberFaceSeed(this._data.ConversationPartnerData.Party, character, 0);
				}
				ValueTuple<uint, uint> deterministicColorsForCharacter = CharacterHelper.GetDeterministicColorsForCharacter(character);
				Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(character.Race);
				AgentVisualsData agentVisualsData = new AgentVisualsData();
				Hero heroObject = character.HeroObject;
				AgentVisualsData agentVisualsData2 = agentVisualsData.Banner((heroObject != null) ? heroObject.ClanBanner : null).Equipment(equipment).Race(character.Race);
				Hero heroObject2 = character.HeroObject;
				AgentVisuals agentVisuals = AgentVisuals.Create(agentVisualsData2.BodyProperties((heroObject2 != null) ? heroObject2.BodyProperties : character.GetBodyProperties(equipment, num)).Frame(gameEntity.GetGlobalFrame()).UseMorphAnims(true)
					.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, character.IsFemale, "_warrior"))
					.ActionCode(ActionIndexCache.Create(defaultAnimForCharacter.ActionName))
					.Scene(this._tableauScene)
					.Monster(baseMonsterFromRace)
					.PrepareImmediately(true)
					.SkeletonType(character.IsFemale ? 1 : 0)
					.ClothColor1(deterministicColorsForCharacter.Item1)
					.ClothColor2(deterministicColorsForCharacter.Item2), "MapConversationTableau", true, false, false);
				agentVisuals.GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.1f, this._frame, true);
				Vec3 globalStableEyePoint = agentVisuals.GetVisuals().GetGlobalStableEyePoint(true);
				agentVisuals.SetLookDirection(this._cameraEntity.GetGlobalFrame().origin - globalStableEyePoint);
				string defaultFaceIdle = CharacterHelper.GetDefaultFaceIdle(character);
				MBSkeletonExtensions.SetFacialAnimation(agentVisuals.GetVisuals().GetSkeleton(), 1, defaultFaceIdle, false, true);
				this._agentVisuals.Add(agentVisuals);
				this._opponentLeaderEquipmentCache = ((equipment != null) ? equipment.CalculateEquipmentCode() : null);
			}
		}

		private void SpawnOpponentBodyguardCharacter(CharacterObject character, int indexOfBodyguard)
		{
			if (indexOfBodyguard >= 0 && indexOfBodyguard <= 1)
			{
				GameEntity gameEntity = this._tableauScene.FindEntitiesWithTag("player_bodyguard_infantry_spawn").ElementAt(indexOfBodyguard);
				MapConversationTableau.DefaultConversationAnimationData defaultAnimForCharacter = this.GetDefaultAnimForCharacter(character, true);
				int num = (indexOfBodyguard + 10) * 5;
				Equipment equipment;
				if (this._data.ConversationPartnerData.IsCivilianEquipmentRequiredForBodyGuardCharacters)
				{
					equipment = (this._data.ConversationPartnerData.Character.IsHero ? character.FirstCivilianEquipment : character.CivilianEquipments.ElementAt(num % character.CivilianEquipments.Count<Equipment>()));
				}
				else
				{
					equipment = (this._data.ConversationPartnerData.Character.IsHero ? character.FirstBattleEquipment : character.BattleEquipments.ElementAt(num % character.BattleEquipments.Count<Equipment>()));
				}
				int num2 = -1;
				if (this._data.ConversationPartnerData.Party != null)
				{
					num2 = CharacterHelper.GetPartyMemberFaceSeed(this._data.ConversationPartnerData.Party, this._data.ConversationPartnerData.Character, num);
				}
				Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(character.Race);
				AgentVisualsData agentVisualsData = new AgentVisualsData();
				PartyBase party = this._data.ConversationPartnerData.Party;
				Banner banner;
				if (party == null)
				{
					banner = null;
				}
				else
				{
					Hero leaderHero = party.LeaderHero;
					banner = ((leaderHero != null) ? leaderHero.ClanBanner : null);
				}
				AgentVisualsData agentVisualsData2 = agentVisualsData.Banner(banner).Equipment(equipment).Race(character.Race)
					.BodyProperties(character.GetBodyProperties(equipment, num2))
					.Frame(gameEntity.GetGlobalFrame())
					.UseMorphAnims(true)
					.ActionSet(MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, character.IsFemale, "_warrior"))
					.ActionCode(ActionIndexCache.Create(defaultAnimForCharacter.ActionName))
					.Scene(this._tableauScene)
					.Monster(baseMonsterFromRace)
					.PrepareImmediately(true)
					.SkeletonType(character.IsFemale ? 1 : 0);
				PartyBase party2 = this._data.ConversationPartnerData.Party;
				uint? num3;
				if (party2 == null)
				{
					num3 = null;
				}
				else
				{
					Hero leaderHero2 = party2.LeaderHero;
					num3 = ((leaderHero2 != null) ? new uint?(leaderHero2.MapFaction.Color) : null);
				}
				AgentVisualsData agentVisualsData3 = agentVisualsData2.ClothColor1(num3 ?? uint.MaxValue);
				PartyBase party3 = this._data.ConversationPartnerData.Party;
				uint? num4;
				if (party3 == null)
				{
					num4 = null;
				}
				else
				{
					Hero leaderHero3 = party3.LeaderHero;
					num4 = ((leaderHero3 != null) ? new uint?(leaderHero3.MapFaction.Color2) : null);
				}
				AgentVisuals agentVisuals = AgentVisuals.Create(agentVisualsData3.ClothColor2(num4 ?? uint.MaxValue), "MapConversationTableau", true, false, false);
				agentVisuals.GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.1f, this._frame, true);
				Vec3 globalStableEyePoint = agentVisuals.GetVisuals().GetGlobalStableEyePoint(true);
				agentVisuals.SetLookDirection(this._cameraEntity.GetGlobalFrame().origin - globalStableEyePoint);
				string defaultFaceIdle = CharacterHelper.GetDefaultFaceIdle(character);
				MBSkeletonExtensions.SetFacialAnimation(agentVisuals.GetVisuals().GetSkeleton(), 1, defaultFaceIdle, false, true);
				this._agentVisuals.Add(agentVisuals);
			}
		}

		internal void CharacterTableauContinuousRenderFunction(Texture sender, EventArgs e)
		{
			Scene scene = (Scene)sender.UserData;
			this.Texture = sender;
			TableauView tableauView = sender.TableauView;
			if (scene == null)
			{
				tableauView.SetContinuousRendering(false);
				tableauView.SetDeleteAfterRendering(true);
				return;
			}
			scene.EnsurePostfxSystem();
			scene.SetDofMode(true);
			scene.SetMotionBlurMode(false);
			scene.SetBloom(true);
			scene.SetDynamicShadowmapCascadesRadiusMultiplier(0.31f);
			tableauView.SetRenderWithPostfx(true);
			uint num = uint.MaxValue;
			num &= 4294966271U;
			if (tableauView != null)
			{
				tableauView.SetPostfxConfigParams((int)num);
			}
			if (this._continuousRenderCamera != null)
			{
				float num2 = this._cameraRatio / 1.7777778f;
				this._continuousRenderCamera.SetFovHorizontal(num2 * this._baseCameraFOV, this._cameraRatio, 0.2f, 200f);
				tableauView.SetCamera(this._continuousRenderCamera);
				tableauView.SetScene(scene);
				tableauView.SetSceneUsesSkybox(true);
				tableauView.SetDeleteAfterRendering(false);
				tableauView.SetContinuousRendering(true);
				tableauView.SetClearColor(0U);
				tableauView.SetClearGbuffer(true);
				tableauView.DoNotClear(false);
				tableauView.SetFocusedShadowmap(true, ref this._frame.origin, 1.55f);
				scene.ForceLoadResources();
				bool flag = true;
				do
				{
					flag = true;
					foreach (AgentVisuals agentVisuals in this._agentVisuals)
					{
						flag = flag && agentVisuals.GetVisuals().CheckResources(true);
					}
				}
				while (!flag);
			}
		}

		private MapConversationTableau.DefaultConversationAnimationData GetDefaultAnimForCharacter(CharacterObject character, bool preferLoopAnimationIfAvailable)
		{
			MapConversationTableau.DefaultConversationAnimationData invalid = MapConversationTableau.DefaultConversationAnimationData.Invalid;
			CultureObject culture = character.Culture;
			if (culture != null && culture.IsBandit)
			{
				invalid.ActionName = "aggressive";
			}
			else
			{
				Hero heroObject = character.HeroObject;
				if (heroObject != null && heroObject.IsWounded)
				{
					PlayerEncounter playerEncounter = PlayerEncounter.Current;
					if (playerEncounter != null && playerEncounter.EncounterState == 6)
					{
						invalid.ActionName = "weary";
						goto IL_6D;
					}
				}
				invalid.ActionName = CharacterHelper.GetStandingBodyIdle(character);
			}
			IL_6D:
			ConversationAnimData conversationAnimData;
			if (Campaign.Current.ConversationManager.ConversationAnimationManager.ConversationAnims.TryGetValue(invalid.ActionName, out conversationAnimData))
			{
				bool flag = !string.IsNullOrEmpty(conversationAnimData.IdleAnimStart);
				bool flag2 = !string.IsNullOrEmpty(conversationAnimData.IdleAnimLoop);
				invalid.ActionName = (((preferLoopAnimationIfAvailable && flag2) || !flag) ? conversationAnimData.IdleAnimLoop : conversationAnimData.IdleAnimStart);
				invalid.AnimationData = conversationAnimData;
				invalid.AnimationDataValid = true;
			}
			else
			{
				invalid.ActionName = MapConversationTableau.fallbackAnimActName;
				if (Campaign.Current.ConversationManager.ConversationAnimationManager.ConversationAnims.TryGetValue(invalid.ActionName, out conversationAnimData))
				{
					invalid.AnimationData = conversationAnimData;
					invalid.AnimationDataValid = true;
				}
			}
			return invalid;
		}

		public void OnConversationPlay(string idleActionId, string idleFaceAnimId, string reactionId, string reactionFaceAnimId, string soundPath)
		{
			if (!this._initialized)
			{
				Debug.FailedAssert("Conversation Tableau shouldn't play before initialization", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox.View\\Map\\MapConversationTableau.cs", "OnConversationPlay", 586);
				return;
			}
			if (!Campaign.Current.ConversationManager.SpeakerAgent.Character.IsPlayerCharacter)
			{
				bool flag = false;
				bool flag2 = string.IsNullOrEmpty(idleActionId);
				ConversationAnimData animationData;
				if (flag2)
				{
					MapConversationTableau.DefaultConversationAnimationData defaultAnimForCharacter = this.GetDefaultAnimForCharacter(this._data.ConversationPartnerData.Character, false);
					animationData = defaultAnimForCharacter.AnimationData;
					flag = defaultAnimForCharacter.AnimationDataValid;
				}
				else if (Campaign.Current.ConversationManager.ConversationAnimationManager.ConversationAnims.TryGetValue(idleActionId, out animationData))
				{
					flag = true;
				}
				if (flag)
				{
					if (!string.IsNullOrEmpty(reactionId))
					{
						this._agentVisuals[0].SetAction(ActionIndexCache.Create(animationData.Reactions[reactionId]), 0f, false);
					}
					else if (!flag2 || this._changeIdleActionTimer.Check(Game.Current.ApplicationTime))
					{
						ActionIndexCache actionIndexCache = ActionIndexCache.Create(animationData.IdleAnimStart);
						if (!this._agentVisuals[0].DoesActionContinueWithCurrentAction(actionIndexCache))
						{
							this._changeIdleActionTimer.Reset(Game.Current.ApplicationTime);
							this._agentVisuals[0].SetAction(actionIndexCache, 0f, false);
						}
					}
				}
				if (!string.IsNullOrEmpty(reactionFaceAnimId))
				{
					MBSkeletonExtensions.SetFacialAnimation(this._agentVisuals[0].GetVisuals().GetSkeleton(), 1, reactionFaceAnimId, false, false);
				}
				else if (!string.IsNullOrEmpty(idleFaceAnimId))
				{
					MBSkeletonExtensions.SetFacialAnimation(this._agentVisuals[0].GetVisuals().GetSkeleton(), 1, idleFaceAnimId, false, true);
				}
			}
			this.RemovePreviousAgentsSoundEvent();
			this.StopConversationSoundEvent();
			if (!string.IsNullOrEmpty(soundPath))
			{
				this.PlayConversationSoundEvent(soundPath);
			}
		}

		private void RemovePreviousAgentsSoundEvent()
		{
			if (this._conversationSoundEvent != null)
			{
				this._agentVisuals[0].StartRhubarbRecord("", -1);
			}
		}

		private void PlayConversationSoundEvent(string soundPath)
		{
			Debug.Print("Conversation sound playing: " + soundPath, 5, 12, 17592186044416UL);
			this._conversationSoundEvent = SoundEvent.CreateEventFromExternalFile("event:/Extra/voiceover", soundPath, this._tableauScene);
			this._conversationSoundEvent.Play();
			int soundId = this._conversationSoundEvent.GetSoundId();
			string rhubarbXmlPathFromSoundPath = this.GetRhubarbXmlPathFromSoundPath(soundPath);
			this._agentVisuals[0].StartRhubarbRecord(rhubarbXmlPathFromSoundPath, soundId);
		}

		private void StopConversationSoundEvent()
		{
			if (this._conversationSoundEvent != null)
			{
				this._conversationSoundEvent.Stop();
				this._conversationSoundEvent = null;
			}
		}

		private string GetRhubarbXmlPathFromSoundPath(string soundPath)
		{
			int num = soundPath.LastIndexOf('.');
			return soundPath.Substring(0, num) + ".xml";
		}

		private const float MinimumTimeRequiredToChangeIdleAction = 8f;

		private Scene _tableauScene;

		private float _animationFrequencyThreshold = 2.5f;

		private MatrixFrame _frame;

		private GameEntity _cameraEntity;

		private SoundEvent _conversationSoundEvent;

		private Camera _continuousRenderCamera;

		private MapConversationTableauData _data;

		private float _cameraRatio;

		private IMapConversationDataProvider _dataProvider;

		private bool _initialized;

		private Timer _changeIdleActionTimer;

		private int _tableauSizeX;

		private int _tableauSizeY;

		private uint _clothColor1 = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		private uint _clothColor2 = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		private List<AgentVisuals> _agentVisuals;

		private static readonly string fallbackAnimActName = "act_inventory_idle_start";

		private readonly string RainingEntityTag = "raining_entity";

		private readonly string SnowingEntityTag = "snowing_entity";

		private float _animationGap;

		private bool _isEnabled = true;

		private float RenderScale = 1f;

		private const float _baseCameraRatio = 1.7777778f;

		private float _baseCameraFOV = -1f;

		private string _cachedAtmosphereName = "";

		private string _opponentLeaderEquipmentCache;

		private struct DefaultConversationAnimationData
		{
			public static readonly MapConversationTableau.DefaultConversationAnimationData Invalid = new MapConversationTableau.DefaultConversationAnimationData
			{
				ActionName = "",
				AnimationDataValid = false
			};

			public ConversationAnimData AnimationData;

			public string ActionName;

			public bool AnimationDataValid;
		}
	}
}
