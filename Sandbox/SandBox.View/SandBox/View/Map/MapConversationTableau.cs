using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Tableaus;

namespace SandBox.View.Map
{
	// Token: 0x02000040 RID: 64
	public class MapConversationTableau : ICampaignMission
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600020B RID: 523 RVA: 0x00013C4C File Offset: 0x00011E4C
		// (set) Token: 0x0600020C RID: 524 RVA: 0x00013C54 File Offset: 0x00011E54
		public Texture Texture { get; private set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600020D RID: 525 RVA: 0x00013C5D File Offset: 0x00011E5D
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

		// Token: 0x0600020E RID: 526 RVA: 0x00013C70 File Offset: 0x00011E70
		public MapConversationTableau()
		{
			this._changeIdleActionTimer = new Timer(Game.Current.ApplicationTime, 8f, true);
			this._agentVisuals = new List<AgentVisuals>();
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(this._isEnabled);
			}
			CampaignMission.Current = this;
			this._dataProvider = SandBoxViewSubModule.MapConversationDataProvider;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00013D54 File Offset: 0x00011F54
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

		// Token: 0x06000210 RID: 528 RVA: 0x00013E14 File Offset: 0x00012014
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

		// Token: 0x06000211 RID: 529 RVA: 0x00013E8C File Offset: 0x0001208C
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
				this.RenderScale = NativeOptions.GetConfig(21) / 100f;
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

		// Token: 0x06000212 RID: 530 RVA: 0x00013F74 File Offset: 0x00012174
		public void OnFinalize()
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
			this.Texture = null;
			IEnumerable<GameEntity> enumerable = this._tableauScene.FindEntitiesWithTag(this._cachedAtmosphereName);
			this._cachedAtmosphereName = "";
			foreach (GameEntity gameEntity in enumerable)
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
			CampaignMission.Current = null;
			TableauCacheManager.Current.ReturnCachedMapConversationTableauScene();
			this._tableauScene = null;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00014094 File Offset: 0x00012294
		public void OnTick(float dt)
		{
			if (this._data != null && !this._initialized)
			{
				this.FirstTimeInit();
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

		// Token: 0x06000214 RID: 532 RVA: 0x000141D0 File Offset: 0x000123D0
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
			TableauView view2 = this.View;
			if (view2 != null)
			{
				view2.SetEnable(true);
			}
			this._initialized = true;
			this.OnConversationPlay(this._initialIdleActionId, this._initialFaceAnimId, this._initialReactionId, this._initialReactionFaceAnimId, this._initialSoundPath);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x000144E0 File Offset: 0x000126E0
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

		// Token: 0x06000216 RID: 534 RVA: 0x000147C8 File Offset: 0x000129C8
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

		// Token: 0x06000217 RID: 535 RVA: 0x00014ACC File Offset: 0x00012CCC
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

		// Token: 0x06000218 RID: 536 RVA: 0x00014C48 File Offset: 0x00012E48
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

		// Token: 0x06000219 RID: 537 RVA: 0x00014D74 File Offset: 0x00012F74
		void ICampaignMission.OnConversationPlay(string idleActionId, string idleFaceAnimId, string reactionId, string reactionFaceAnimId, string soundPath)
		{
			if (this._initialized)
			{
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
					return;
				}
			}
			else
			{
				this._initialIdleActionId = idleActionId;
				this._initialFaceAnimId = idleFaceAnimId;
				this._initialReactionId = reactionId;
				this._initialReactionFaceAnimId = reactionFaceAnimId;
				this._initialSoundPath = soundPath;
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x00014F30 File Offset: 0x00013130
		private void RemovePreviousAgentsSoundEvent()
		{
			if (this._conversationSoundEvent != null)
			{
				this._agentVisuals[0].StartRhubarbRecord("", -1);
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00014F54 File Offset: 0x00013154
		private void PlayConversationSoundEvent(string soundPath)
		{
			Debug.Print("Conversation sound playing: " + soundPath, 5, 12, 17592186044416UL);
			this._conversationSoundEvent = SoundEvent.CreateEventFromExternalFile("event:/Extra/voiceover", soundPath, this._tableauScene);
			this._conversationSoundEvent.Play();
			int soundId = this._conversationSoundEvent.GetSoundId();
			string rhubarbXmlPathFromSoundPath = this.GetRhubarbXmlPathFromSoundPath(soundPath);
			this._agentVisuals[0].StartRhubarbRecord(rhubarbXmlPathFromSoundPath, soundId);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00014FC7 File Offset: 0x000131C7
		private void StopConversationSoundEvent()
		{
			if (this._conversationSoundEvent != null)
			{
				this._conversationSoundEvent.Stop();
				this._conversationSoundEvent = null;
			}
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00014FE4 File Offset: 0x000131E4
		private string GetRhubarbXmlPathFromSoundPath(string soundPath)
		{
			int num = soundPath.LastIndexOf('.');
			return soundPath.Substring(0, num) + ".xml";
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600021E RID: 542 RVA: 0x0001500C File Offset: 0x0001320C
		GameState ICampaignMission.State
		{
			get
			{
				return Game.Current.GameStateManager.ActiveState;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600021F RID: 543 RVA: 0x0001501D File Offset: 0x0001321D
		IMissionTroopSupplier ICampaignMission.AgentSupplier
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000220 RID: 544 RVA: 0x00015020 File Offset: 0x00013220
		// (set) Token: 0x06000221 RID: 545 RVA: 0x00015023 File Offset: 0x00013223
		Location ICampaignMission.Location
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000222 RID: 546 RVA: 0x00015025 File Offset: 0x00013225
		// (set) Token: 0x06000223 RID: 547 RVA: 0x00015028 File Offset: 0x00013228
		Alley ICampaignMission.LastVisitedAlley
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0001502A File Offset: 0x0001322A
		MissionMode ICampaignMission.Mode
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0001502D File Offset: 0x0001322D
		void ICampaignMission.AddAgentFollowing(IAgent agent)
		{
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0001502F File Offset: 0x0001322F
		bool ICampaignMission.AgentLookingAtAgent(IAgent agent1, IAgent agent2)
		{
			return false;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x00015032 File Offset: 0x00013232
		bool ICampaignMission.CheckIfAgentCanFollow(IAgent agent)
		{
			return false;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00015035 File Offset: 0x00013235
		bool ICampaignMission.CheckIfAgentCanUnFollow(IAgent agent)
		{
			return false;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00015038 File Offset: 0x00013238
		void ICampaignMission.OnConversationContinue()
		{
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0001503A File Offset: 0x0001323A
		void ICampaignMission.EndMission()
		{
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0001503C File Offset: 0x0001323C
		void ICampaignMission.OnCharacterLocationChanged(LocationCharacter locationCharacter, Location fromLocation, Location toLocation)
		{
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0001503E File Offset: 0x0001323E
		void ICampaignMission.OnCloseEncounterMenu()
		{
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00015040 File Offset: 0x00013240
		void ICampaignMission.OnConversationEnd(IAgent agent)
		{
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00015042 File Offset: 0x00013242
		void ICampaignMission.OnConversationStart(IAgent agent, bool setActionsInstantly)
		{
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00015044 File Offset: 0x00013244
		void ICampaignMission.OnProcessSentence()
		{
		}

		// Token: 0x06000230 RID: 560 RVA: 0x00015046 File Offset: 0x00013246
		void ICampaignMission.RemoveAgentFollowing(IAgent agent)
		{
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00015048 File Offset: 0x00013248
		void ICampaignMission.SetMissionMode(MissionMode newMode, bool atStart)
		{
		}

		// Token: 0x04000112 RID: 274
		private const float MinimumTimeRequiredToChangeIdleAction = 8f;

		// Token: 0x04000114 RID: 276
		private Scene _tableauScene;

		// Token: 0x04000115 RID: 277
		private float _animationFrequencyThreshold = 2.5f;

		// Token: 0x04000116 RID: 278
		private MatrixFrame _frame;

		// Token: 0x04000117 RID: 279
		private GameEntity _cameraEntity;

		// Token: 0x04000118 RID: 280
		private SoundEvent _conversationSoundEvent;

		// Token: 0x04000119 RID: 281
		private Camera _continuousRenderCamera;

		// Token: 0x0400011A RID: 282
		private MapConversationTableauData _data;

		// Token: 0x0400011B RID: 283
		private float _cameraRatio;

		// Token: 0x0400011C RID: 284
		private IMapConversationDataProvider _dataProvider;

		// Token: 0x0400011D RID: 285
		private bool _initialized;

		// Token: 0x0400011E RID: 286
		private string _initialFaceAnimId;

		// Token: 0x0400011F RID: 287
		private string _initialIdleActionId;

		// Token: 0x04000120 RID: 288
		private string _initialReactionId;

		// Token: 0x04000121 RID: 289
		private string _initialReactionFaceAnimId;

		// Token: 0x04000122 RID: 290
		private string _initialSoundPath;

		// Token: 0x04000123 RID: 291
		private Timer _changeIdleActionTimer;

		// Token: 0x04000124 RID: 292
		private int _tableauSizeX;

		// Token: 0x04000125 RID: 293
		private int _tableauSizeY;

		// Token: 0x04000126 RID: 294
		private uint _clothColor1 = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		// Token: 0x04000127 RID: 295
		private uint _clothColor2 = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		// Token: 0x04000128 RID: 296
		private List<AgentVisuals> _agentVisuals;

		// Token: 0x04000129 RID: 297
		private static readonly string fallbackAnimActName = "act_inventory_idle_start";

		// Token: 0x0400012A RID: 298
		private float _animationGap;

		// Token: 0x0400012B RID: 299
		private bool _isEnabled = true;

		// Token: 0x0400012C RID: 300
		private float RenderScale = 1f;

		// Token: 0x0400012D RID: 301
		private const float _baseCameraRatio = 1.7777778f;

		// Token: 0x0400012E RID: 302
		private float _baseCameraFOV = -1f;

		// Token: 0x0400012F RID: 303
		private string _cachedAtmosphereName = "";

		// Token: 0x04000130 RID: 304
		private string _opponentLeaderEquipmentCache;

		// Token: 0x02000079 RID: 121
		private struct DefaultConversationAnimationData
		{
			// Token: 0x040002C2 RID: 706
			public static readonly MapConversationTableau.DefaultConversationAnimationData Invalid = new MapConversationTableau.DefaultConversationAnimationData
			{
				ActionName = "",
				AnimationDataValid = false
			};

			// Token: 0x040002C3 RID: 707
			public ConversationAnimData AnimationData;

			// Token: 0x040002C4 RID: 708
			public string ActionName;

			// Token: 0x040002C5 RID: 709
			public bool AnimationDataValid;
		}
	}
}
