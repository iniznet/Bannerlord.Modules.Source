using System;
using System.Threading;
using psai.net;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001BF RID: 447
	public class MBMusicManager
	{
		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x060019B0 RID: 6576 RVA: 0x0005BCD0 File Offset: 0x00059ED0
		// (set) Token: 0x060019B1 RID: 6577 RVA: 0x0005BCD7 File Offset: 0x00059ED7
		public static MBMusicManager Current { get; private set; }

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x060019B2 RID: 6578 RVA: 0x0005BCDF File Offset: 0x00059EDF
		// (set) Token: 0x060019B3 RID: 6579 RVA: 0x0005BCE7 File Offset: 0x00059EE7
		public MusicMode CurrentMode { get; private set; }

		// Token: 0x060019B4 RID: 6580 RVA: 0x0005BCF0 File Offset: 0x00059EF0
		private MBMusicManager()
		{
			if (!NativeConfig.DisableSound)
			{
				PsaiCore.Instance.LoadSoundtrackFromProjectFile(BasePath.Name + "music/soundtrack.xml");
			}
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x0005BD20 File Offset: 0x00059F20
		public static bool IsCreationCompleted()
		{
			return MBMusicManager._creationCompleted;
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x0005BD27 File Offset: 0x00059F27
		private static void ProcessCreation(object callback)
		{
			MBMusicManager.Current = new MBMusicManager();
			MusicParameters.LoadFromXml();
			MBMusicManager._creationCompleted = true;
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x0005BD3E File Offset: 0x00059F3E
		public static void Create()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(MBMusicManager.ProcessCreation));
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x0005BD54 File Offset: 0x00059F54
		public static void Initialize()
		{
			if (!MBMusicManager._initialized)
			{
				MBMusicManager.Current._battleMode = new MBMusicManager.BattleMusicMode();
				MBMusicManager.Current._campaignMode = new MBMusicManager.CampaignMusicMode();
				MBMusicManager.Current.CurrentMode = MusicMode.Paused;
				MBMusicManager.Current._menuModeActivationTimer = 0.5f;
				MBMusicManager._initialized = true;
				Debug.Print("MusicManager Initialize completed.", 0, Debug.DebugColor.Green, 281474976710656UL);
			}
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x0005BDBB File Offset: 0x00059FBB
		public void OnCampaignMusicHandlerInit(IMusicHandler campaignMusicHandler)
		{
			this._campaignMusicHandler = campaignMusicHandler;
			this._activeMusicHandler = this._campaignMusicHandler;
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x0005BDD0 File Offset: 0x00059FD0
		public void OnCampaignMusicHandlerFinalize()
		{
			this._campaignMusicHandler = null;
			this.CheckActiveHandler();
		}

		// Token: 0x060019BB RID: 6587 RVA: 0x0005BDDF File Offset: 0x00059FDF
		public void OnBattleMusicHandlerInit(IMusicHandler battleMusicHandler)
		{
			this._battleMusicHandler = battleMusicHandler;
			this._activeMusicHandler = this._battleMusicHandler;
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x0005BDF4 File Offset: 0x00059FF4
		public void OnBattleMusicHandlerFinalize()
		{
			this._battleMusicHandler = null;
			this.CheckActiveHandler();
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x0005BE03 File Offset: 0x0005A003
		public void OnSilencedMusicHandlerInit(IMusicHandler silencedMusicHandler)
		{
			this._silencedMusicHandler = silencedMusicHandler;
			this._activeMusicHandler = this._silencedMusicHandler;
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x0005BE18 File Offset: 0x0005A018
		public void OnSilencedMusicHandlerFinalize()
		{
			this._silencedMusicHandler = null;
			this.CheckActiveHandler();
		}

		// Token: 0x060019BF RID: 6591 RVA: 0x0005BE27 File Offset: 0x0005A027
		private void CheckActiveHandler()
		{
			IMusicHandler musicHandler;
			if ((musicHandler = this._battleMusicHandler) == null)
			{
				musicHandler = this._silencedMusicHandler ?? this._campaignMusicHandler;
			}
			this._activeMusicHandler = musicHandler;
		}

		// Token: 0x060019C0 RID: 6592 RVA: 0x0005BE49 File Offset: 0x0005A049
		private void ActivateMenuMode()
		{
			if (!this._systemPaused)
			{
				this.CurrentMode = MusicMode.Menu;
				PsaiCore.Instance.MenuModeEnter(5, 0.5f);
			}
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x0005BE6B File Offset: 0x0005A06B
		private void DeactivateMenuMode()
		{
			PsaiCore.Instance.MenuModeLeave();
			this.CurrentMode = MusicMode.Paused;
		}

		// Token: 0x060019C2 RID: 6594 RVA: 0x0005BE7F File Offset: 0x0005A07F
		public void ActivateBattleMode()
		{
			if (!this._systemPaused)
			{
				this.CurrentMode = MusicMode.Battle;
			}
		}

		// Token: 0x060019C3 RID: 6595 RVA: 0x0005BE90 File Offset: 0x0005A090
		public void DeactivateBattleMode()
		{
			PsaiCore.Instance.StopMusic(true, 3f);
			this.CurrentMode = MusicMode.Paused;
		}

		// Token: 0x060019C4 RID: 6596 RVA: 0x0005BEAA File Offset: 0x0005A0AA
		public void ActivateCampaignMode()
		{
			if (!this._systemPaused)
			{
				this.CurrentMode = MusicMode.Campaign;
			}
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x0005BEBB File Offset: 0x0005A0BB
		public void DeactivateCampaignMode()
		{
			PsaiCore.Instance.StopMusic(true, 3f);
			this.CurrentMode = MusicMode.Paused;
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x0005BED8 File Offset: 0x0005A0D8
		public void DeactivateCurrentMode()
		{
			switch (this.CurrentMode)
			{
			case MusicMode.Menu:
				break;
			case MusicMode.Campaign:
				this.DeactivateCampaignMode();
				return;
			case MusicMode.Battle:
				this.DeactivateBattleMode();
				break;
			default:
				return;
			}
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x0005BF0E File Offset: 0x0005A10E
		private bool CheckMenuModeActivationTimer()
		{
			return this._menuModeActivationTimer <= 0f;
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x0005BF20 File Offset: 0x0005A120
		public void UnpauseMusicManagerSystem()
		{
			if (this._systemPaused)
			{
				this._systemPaused = false;
				this._menuModeActivationTimer = 1f;
			}
		}

		// Token: 0x060019C9 RID: 6601 RVA: 0x0005BF3C File Offset: 0x0005A13C
		public void PauseMusicManagerSystem()
		{
			if (!this._systemPaused)
			{
				if (this.CurrentMode == MusicMode.Menu)
				{
					this.DeactivateMenuMode();
				}
				this._systemPaused = true;
			}
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x0005BF5C File Offset: 0x0005A15C
		public void StartTheme(MusicTheme theme, float startIntensity, bool queueEndSegment = false)
		{
			PsaiCore.Instance.TriggerMusicTheme((int)theme, startIntensity);
			if (queueEndSegment)
			{
				PsaiCore.Instance.StopMusic(false, 3f);
			}
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x0005BF7F File Offset: 0x0005A17F
		public void StartThemeWithConstantIntensity(MusicTheme theme, bool queueEndSegment = false)
		{
			PsaiCore.Instance.HoldCurrentIntensity(true);
			this.StartTheme(theme, 0f, queueEndSegment);
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x0005BF9A File Offset: 0x0005A19A
		public void ForceStopThemeWithFadeOut()
		{
			PsaiCore.Instance.StopMusic(true, 3f);
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x0005BFAD File Offset: 0x0005A1AD
		public void ChangeCurrentThemeIntensity(float deltaIntensity)
		{
			PsaiCore.Instance.AddToCurrentIntensity(deltaIntensity);
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x0005BFBC File Offset: 0x0005A1BC
		public void Update(float dt)
		{
			if (Utilities.EngineFrameNo == this._latestFrameUpdatedNo)
			{
				return;
			}
			this._latestFrameUpdatedNo = Utilities.EngineFrameNo;
			if (this._menuModeActivationTimer > 0f)
			{
				this._menuModeActivationTimer -= dt;
			}
			if (!this._systemPaused)
			{
				if (GameStateManager.Current != null && GameStateManager.Current.ActiveState != null)
				{
					GameState activeState = GameStateManager.Current.ActiveState;
					MusicMode currentMode = this.CurrentMode;
					if (currentMode != MusicMode.Paused)
					{
						if (currentMode == MusicMode.Menu)
						{
							if (!activeState.IsMusicMenuState)
							{
								this.DeactivateMenuMode();
							}
						}
					}
					else if (activeState.IsMusicMenuState && this.CheckMenuModeActivationTimer())
					{
						this.ActivateMenuMode();
					}
				}
				if (this._activeMusicHandler != null)
				{
					this._activeMusicHandler.OnUpdated(dt);
				}
			}
			PsaiCore.Instance.Update();
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x0005C078 File Offset: 0x0005A278
		public MusicTheme GetSiegeTheme(CultureCode cultureCode)
		{
			return this._battleMode.GetSiegeTheme(cultureCode);
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x0005C086 File Offset: 0x0005A286
		public MusicTheme GetBattleTheme(CultureCode cultureCode, int battleSize, out bool isPaganBattle)
		{
			return this._battleMode.GetBattleTheme(cultureCode, battleSize, out isPaganBattle);
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x0005C096 File Offset: 0x0005A296
		public MusicTheme GetBattleEndTheme(CultureCode cultureCode, bool isVictory)
		{
			return this._battleMode.GetBattleEndTheme(cultureCode, isVictory);
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x0005C0A5 File Offset: 0x0005A2A5
		public MusicTheme GetBattleTurnsOneSideTheme(CultureCode cultureCode, bool isPositive, bool isPaganBattle)
		{
			if (isPaganBattle)
			{
				if (!isPositive)
				{
					return MusicTheme.PaganTurnsNegative;
				}
				return MusicTheme.PaganTurnsPositive;
			}
			else
			{
				if (!isPositive)
				{
					return MusicTheme.BattleTurnsNegative;
				}
				return MusicTheme.BattleTurnsPositive;
			}
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x0005C0BC File Offset: 0x0005A2BC
		public MusicTheme GetCampaignMusicTheme(CultureCode cultureCode, bool isDark, bool isWarMode)
		{
			MusicTheme musicTheme = MusicTheme.None;
			if (!isDark && isWarMode)
			{
				musicTheme = this._campaignMode.GetCampaignDramaticThemeWithCulture(cultureCode);
			}
			if (musicTheme != MusicTheme.None)
			{
				return musicTheme;
			}
			return this._campaignMode.GetCampaignTheme(cultureCode, isDark);
		}

		// Token: 0x04000806 RID: 2054
		private const float DefaultFadeOutDurationInSeconds = 3f;

		// Token: 0x04000807 RID: 2055
		private const float MenuModeActivationTimerInSeconds = 0.5f;

		// Token: 0x0400080A RID: 2058
		private MBMusicManager.BattleMusicMode _battleMode;

		// Token: 0x0400080B RID: 2059
		private MBMusicManager.CampaignMusicMode _campaignMode;

		// Token: 0x0400080C RID: 2060
		private IMusicHandler _campaignMusicHandler;

		// Token: 0x0400080D RID: 2061
		private IMusicHandler _battleMusicHandler;

		// Token: 0x0400080E RID: 2062
		private IMusicHandler _silencedMusicHandler;

		// Token: 0x0400080F RID: 2063
		private IMusicHandler _activeMusicHandler;

		// Token: 0x04000810 RID: 2064
		private static bool _initialized;

		// Token: 0x04000811 RID: 2065
		private static bool _creationCompleted;

		// Token: 0x04000812 RID: 2066
		private float _menuModeActivationTimer;

		// Token: 0x04000813 RID: 2067
		private bool _systemPaused;

		// Token: 0x04000814 RID: 2068
		private int _latestFrameUpdatedNo = -1;

		// Token: 0x02000519 RID: 1305
		private class CampaignMusicMode
		{
			// Token: 0x0600396B RID: 14699 RVA: 0x000E839A File Offset: 0x000E659A
			public CampaignMusicMode()
			{
				this._factionSpecificCampaignThemeSelectionFactor = 0.35f;
				this._factionSpecificCampaignDramaticThemeSelectionFactor = 0.35f;
			}

			// Token: 0x0600396C RID: 14700 RVA: 0x000E83B8 File Offset: 0x000E65B8
			public MusicTheme GetCampaignTheme(CultureCode cultureCode, bool isDark)
			{
				if (isDark)
				{
					return MusicTheme.CampaignDark;
				}
				MusicTheme campaignThemeWithCulture = this.GetCampaignThemeWithCulture(cultureCode);
				MusicTheme musicTheme;
				if (campaignThemeWithCulture == MusicTheme.None)
				{
					musicTheme = MusicTheme.CampaignStandard;
					this._factionSpecificCampaignThemeSelectionFactor += 0.1f;
					MBMath.ClampUnit(ref this._factionSpecificCampaignThemeSelectionFactor);
				}
				else
				{
					musicTheme = campaignThemeWithCulture;
					this._factionSpecificCampaignThemeSelectionFactor -= 0.1f;
					MBMath.ClampUnit(ref this._factionSpecificCampaignThemeSelectionFactor);
				}
				return musicTheme;
			}

			// Token: 0x0600396D RID: 14701 RVA: 0x000E8418 File Offset: 0x000E6618
			private MusicTheme GetCampaignThemeWithCulture(CultureCode cultureCode)
			{
				if (MBRandom.NondeterministicRandomFloat <= this._factionSpecificCampaignThemeSelectionFactor)
				{
					this._factionSpecificCampaignThemeSelectionFactor -= 0.1f;
					MBMath.ClampUnit(ref this._factionSpecificCampaignThemeSelectionFactor);
					switch (cultureCode)
					{
					case CultureCode.Empire:
						if (MBRandom.NondeterministicRandomFloat >= 0.5f)
						{
							return MusicTheme.EmpireCampaignB;
						}
						return MusicTheme.EmpireCampaignA;
					case CultureCode.Sturgia:
						return MusicTheme.SturgiaCampaignA;
					case CultureCode.Aserai:
						return MusicTheme.AseraiCampaignA;
					case CultureCode.Vlandia:
						return MusicTheme.VlandiaCampaignA;
					case CultureCode.Khuzait:
						return MusicTheme.KhuzaitCampaignA;
					case CultureCode.Battania:
						return MusicTheme.BattaniaCampaignA;
					}
				}
				return MusicTheme.None;
			}

			// Token: 0x0600396E RID: 14702 RVA: 0x000E8494 File Offset: 0x000E6694
			public MusicTheme GetCampaignDramaticThemeWithCulture(CultureCode cultureCode)
			{
				if (MBRandom.NondeterministicRandomFloat <= this._factionSpecificCampaignDramaticThemeSelectionFactor)
				{
					this._factionSpecificCampaignDramaticThemeSelectionFactor -= 0.1f;
					MBMath.ClampUnit(ref this._factionSpecificCampaignDramaticThemeSelectionFactor);
					switch (cultureCode)
					{
					case CultureCode.Empire:
						return MusicTheme.EmpireCampaignDramatic;
					case CultureCode.Sturgia:
						return MusicTheme.SturgiaCampaignDramatic;
					case CultureCode.Aserai:
						return MusicTheme.AseraiCampaignDramatic;
					case CultureCode.Vlandia:
						return MusicTheme.VlandiaCampaignDramatic;
					case CultureCode.Khuzait:
						return MusicTheme.KhuzaitCampaignDramatic;
					case CultureCode.Battania:
						return MusicTheme.BattaniaCampaignDramatic;
					}
				}
				this._factionSpecificCampaignDramaticThemeSelectionFactor += 0.1f;
				MBMath.ClampUnit(ref this._factionSpecificCampaignDramaticThemeSelectionFactor);
				return MusicTheme.None;
			}

			// Token: 0x04001BD7 RID: 7127
			private const float DefaultSelectionFactorForFactionSpecificCampaignTheme = 0.35f;

			// Token: 0x04001BD8 RID: 7128
			private const float SelectionFactorDecayAmountForFactionSpecificCampaignTheme = 0.1f;

			// Token: 0x04001BD9 RID: 7129
			private const float SelectionFactorGrowthAmountForFactionSpecificCampaignTheme = 0.1f;

			// Token: 0x04001BDA RID: 7130
			private float _factionSpecificCampaignThemeSelectionFactor;

			// Token: 0x04001BDB RID: 7131
			private float _factionSpecificCampaignDramaticThemeSelectionFactor;
		}

		// Token: 0x0200051A RID: 1306
		private class BattleMusicMode
		{
			// Token: 0x0600396F RID: 14703 RVA: 0x000E851A File Offset: 0x000E671A
			public BattleMusicMode()
			{
				this._factionSpecificBattleThemeSelectionFactor = 0.35f;
				this._factionSpecificSiegeThemeSelectionFactor = 0.35f;
			}

			// Token: 0x06003970 RID: 14704 RVA: 0x000E8538 File Offset: 0x000E6738
			private MusicTheme GetBattleThemeWithCulture(CultureCode cultureCode, out bool isPaganBattle)
			{
				isPaganBattle = false;
				MusicTheme musicTheme = MusicTheme.None;
				if (MBRandom.NondeterministicRandomFloat <= this._factionSpecificBattleThemeSelectionFactor)
				{
					this._factionSpecificBattleThemeSelectionFactor -= 0.1f;
					MBMath.ClampUnit(ref this._factionSpecificBattleThemeSelectionFactor);
					if (cultureCode - CultureCode.Sturgia <= 1 || cultureCode - CultureCode.Khuzait <= 1)
					{
						isPaganBattle = true;
						musicTheme = ((MBRandom.NondeterministicRandomFloat < 0.5f) ? MusicTheme.BattlePaganA : MusicTheme.BattlePaganB);
					}
					else
					{
						musicTheme = ((MBRandom.NondeterministicRandomFloat < 0.5f) ? MusicTheme.CombatA : MusicTheme.CombatB);
					}
				}
				return musicTheme;
			}

			// Token: 0x06003971 RID: 14705 RVA: 0x000E85AC File Offset: 0x000E67AC
			private MusicTheme GetSiegeThemeWithCulture(CultureCode cultureCode)
			{
				MusicTheme musicTheme = MusicTheme.None;
				if (MBRandom.NondeterministicRandomFloat <= this._factionSpecificSiegeThemeSelectionFactor)
				{
					this._factionSpecificSiegeThemeSelectionFactor -= 0.1f;
					MBMath.ClampUnit(ref this._factionSpecificSiegeThemeSelectionFactor);
					if (cultureCode - CultureCode.Sturgia <= 1 || cultureCode - CultureCode.Khuzait <= 1)
					{
						musicTheme = MusicTheme.PaganSiege;
					}
				}
				return musicTheme;
			}

			// Token: 0x06003972 RID: 14706 RVA: 0x000E85F5 File Offset: 0x000E67F5
			private MusicTheme GetVictoryThemeForCulture(CultureCode cultureCode)
			{
				if (MBRandom.NondeterministicRandomFloat <= 0.65f)
				{
					switch (cultureCode)
					{
					case CultureCode.Empire:
						return MusicTheme.EmpireVictory;
					case CultureCode.Sturgia:
						return MusicTheme.SturgiaVictory;
					case CultureCode.Aserai:
						return MusicTheme.AseraiVictory;
					case CultureCode.Vlandia:
						return MusicTheme.VlandiaVictory;
					case CultureCode.Khuzait:
						return MusicTheme.KhuzaitVictory;
					case CultureCode.Battania:
						return MusicTheme.BattaniaVictory;
					}
				}
				return MusicTheme.None;
			}

			// Token: 0x06003973 RID: 14707 RVA: 0x000E8638 File Offset: 0x000E6838
			public MusicTheme GetBattleTheme(CultureCode culture, int battleSize, out bool isPaganBattle)
			{
				MusicTheme battleThemeWithCulture = this.GetBattleThemeWithCulture(culture, out isPaganBattle);
				MusicTheme musicTheme;
				if (battleThemeWithCulture == MusicTheme.None)
				{
					musicTheme = (((float)battleSize < (float)MusicParameters.SmallBattleTreshold - (float)MusicParameters.SmallBattleTreshold * 0.2f * MBRandom.NondeterministicRandomFloat) ? MusicTheme.BattleSmall : MusicTheme.BattleMedium);
					this._factionSpecificBattleThemeSelectionFactor += 0.1f;
					MBMath.ClampUnit(ref this._factionSpecificBattleThemeSelectionFactor);
				}
				else
				{
					musicTheme = battleThemeWithCulture;
					this._factionSpecificBattleThemeSelectionFactor -= 0.1f;
					MBMath.ClampUnit(ref this._factionSpecificBattleThemeSelectionFactor);
				}
				return musicTheme;
			}

			// Token: 0x06003974 RID: 14708 RVA: 0x000E86B8 File Offset: 0x000E68B8
			public MusicTheme GetSiegeTheme(CultureCode culture)
			{
				MusicTheme siegeThemeWithCulture = this.GetSiegeThemeWithCulture(culture);
				MusicTheme musicTheme;
				if (siegeThemeWithCulture == MusicTheme.None)
				{
					musicTheme = MusicTheme.BattleSiege;
					this._factionSpecificSiegeThemeSelectionFactor += 0.1f;
					MBMath.ClampUnit(ref this._factionSpecificSiegeThemeSelectionFactor);
				}
				else
				{
					musicTheme = siegeThemeWithCulture;
					this._factionSpecificSiegeThemeSelectionFactor -= 0.1f;
					MBMath.ClampUnit(ref this._factionSpecificSiegeThemeSelectionFactor);
				}
				return musicTheme;
			}

			// Token: 0x06003975 RID: 14709 RVA: 0x000E8714 File Offset: 0x000E6914
			public MusicTheme GetBattleEndTheme(CultureCode culture, bool isVictorious)
			{
				MusicTheme musicTheme;
				if (isVictorious)
				{
					MusicTheme victoryThemeForCulture = this.GetVictoryThemeForCulture(culture);
					if (victoryThemeForCulture == MusicTheme.None)
					{
						musicTheme = MusicTheme.BattleVictory;
					}
					else
					{
						musicTheme = victoryThemeForCulture;
					}
				}
				else
				{
					musicTheme = MusicTheme.BattleDefeat;
				}
				return musicTheme;
			}

			// Token: 0x04001BDC RID: 7132
			private const float DefaultSelectionFactorForFactionSpecificBattleTheme = 0.35f;

			// Token: 0x04001BDD RID: 7133
			private const float SelectionFactorDecayAmountForFactionSpecificBattleTheme = 0.1f;

			// Token: 0x04001BDE RID: 7134
			private const float SelectionFactorGrowthAmountForFactionSpecificBattleTheme = 0.1f;

			// Token: 0x04001BDF RID: 7135
			private const float DefaultSelectionFactorForFactionSpecificVictoryTheme = 0.65f;

			// Token: 0x04001BE0 RID: 7136
			private float _factionSpecificBattleThemeSelectionFactor;

			// Token: 0x04001BE1 RID: 7137
			private float _factionSpecificSiegeThemeSelectionFactor;
		}
	}
}
