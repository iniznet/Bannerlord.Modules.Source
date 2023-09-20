using System;
using System.Threading;
using psai.net;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MBMusicManager
	{
		public static MBMusicManager Current { get; private set; }

		public MusicMode CurrentMode { get; private set; }

		private MBMusicManager()
		{
			if (!NativeConfig.DisableSound)
			{
				PsaiCore.Instance.LoadSoundtrackFromProjectFile(BasePath.Name + "music/soundtrack.xml");
			}
		}

		public static bool IsCreationCompleted()
		{
			return MBMusicManager._creationCompleted;
		}

		private static void ProcessCreation(object callback)
		{
			MBMusicManager.Current = new MBMusicManager();
			MusicParameters.LoadFromXml();
			MBMusicManager._creationCompleted = true;
		}

		public static void Create()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(MBMusicManager.ProcessCreation));
		}

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

		public void OnCampaignMusicHandlerInit(IMusicHandler campaignMusicHandler)
		{
			this._campaignMusicHandler = campaignMusicHandler;
			this._activeMusicHandler = this._campaignMusicHandler;
		}

		public void OnCampaignMusicHandlerFinalize()
		{
			this._campaignMusicHandler = null;
			this.CheckActiveHandler();
		}

		public void OnBattleMusicHandlerInit(IMusicHandler battleMusicHandler)
		{
			this._battleMusicHandler = battleMusicHandler;
			this._activeMusicHandler = this._battleMusicHandler;
		}

		public void OnBattleMusicHandlerFinalize()
		{
			this._battleMusicHandler = null;
			this.CheckActiveHandler();
		}

		public void OnSilencedMusicHandlerInit(IMusicHandler silencedMusicHandler)
		{
			this._silencedMusicHandler = silencedMusicHandler;
			this._activeMusicHandler = this._silencedMusicHandler;
		}

		public void OnSilencedMusicHandlerFinalize()
		{
			this._silencedMusicHandler = null;
			this.CheckActiveHandler();
		}

		private void CheckActiveHandler()
		{
			IMusicHandler musicHandler;
			if ((musicHandler = this._battleMusicHandler) == null)
			{
				musicHandler = this._silencedMusicHandler ?? this._campaignMusicHandler;
			}
			this._activeMusicHandler = musicHandler;
		}

		private void ActivateMenuMode()
		{
			if (!this._systemPaused)
			{
				this.CurrentMode = MusicMode.Menu;
				PsaiCore.Instance.MenuModeEnter(5, 0.5f);
			}
		}

		private void DeactivateMenuMode()
		{
			PsaiCore.Instance.MenuModeLeave();
			this.CurrentMode = MusicMode.Paused;
		}

		public void ActivateBattleMode()
		{
			if (!this._systemPaused)
			{
				this.CurrentMode = MusicMode.Battle;
			}
		}

		public void DeactivateBattleMode()
		{
			PsaiCore.Instance.StopMusic(true, 3f);
			this.CurrentMode = MusicMode.Paused;
		}

		public void ActivateCampaignMode()
		{
			if (!this._systemPaused)
			{
				this.CurrentMode = MusicMode.Campaign;
			}
		}

		public void DeactivateCampaignMode()
		{
			PsaiCore.Instance.StopMusic(true, 3f);
			this.CurrentMode = MusicMode.Paused;
		}

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

		private bool CheckMenuModeActivationTimer()
		{
			return this._menuModeActivationTimer <= 0f;
		}

		public void UnpauseMusicManagerSystem()
		{
			if (this._systemPaused)
			{
				this._systemPaused = false;
				this._menuModeActivationTimer = 1f;
			}
		}

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

		public void StartTheme(MusicTheme theme, float startIntensity, bool queueEndSegment = false)
		{
			PsaiCore.Instance.TriggerMusicTheme((int)theme, startIntensity);
			if (queueEndSegment)
			{
				PsaiCore.Instance.StopMusic(false, 3f);
			}
		}

		public void StartThemeWithConstantIntensity(MusicTheme theme, bool queueEndSegment = false)
		{
			PsaiCore.Instance.HoldCurrentIntensity(true);
			this.StartTheme(theme, 0f, queueEndSegment);
		}

		public void ForceStopThemeWithFadeOut()
		{
			PsaiCore.Instance.StopMusic(true, 3f);
		}

		public void ChangeCurrentThemeIntensity(float deltaIntensity)
		{
			PsaiCore.Instance.AddToCurrentIntensity(deltaIntensity);
		}

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

		public MusicTheme GetSiegeTheme(CultureCode cultureCode)
		{
			return this._battleMode.GetSiegeTheme(cultureCode);
		}

		public MusicTheme GetBattleTheme(CultureCode cultureCode, int battleSize, out bool isPaganBattle)
		{
			return this._battleMode.GetBattleTheme(cultureCode, battleSize, out isPaganBattle);
		}

		public MusicTheme GetBattleEndTheme(CultureCode cultureCode, bool isVictory)
		{
			return this._battleMode.GetBattleEndTheme(cultureCode, isVictory);
		}

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

		private const float DefaultFadeOutDurationInSeconds = 3f;

		private const float MenuModeActivationTimerInSeconds = 0.5f;

		private MBMusicManager.BattleMusicMode _battleMode;

		private MBMusicManager.CampaignMusicMode _campaignMode;

		private IMusicHandler _campaignMusicHandler;

		private IMusicHandler _battleMusicHandler;

		private IMusicHandler _silencedMusicHandler;

		private IMusicHandler _activeMusicHandler;

		private static bool _initialized;

		private static bool _creationCompleted;

		private float _menuModeActivationTimer;

		private bool _systemPaused;

		private int _latestFrameUpdatedNo = -1;

		private class CampaignMusicMode
		{
			public CampaignMusicMode()
			{
				this._factionSpecificCampaignThemeSelectionFactor = 0.35f;
				this._factionSpecificCampaignDramaticThemeSelectionFactor = 0.35f;
			}

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

			private const float DefaultSelectionFactorForFactionSpecificCampaignTheme = 0.35f;

			private const float SelectionFactorDecayAmountForFactionSpecificCampaignTheme = 0.1f;

			private const float SelectionFactorGrowthAmountForFactionSpecificCampaignTheme = 0.1f;

			private float _factionSpecificCampaignThemeSelectionFactor;

			private float _factionSpecificCampaignDramaticThemeSelectionFactor;
		}

		private class BattleMusicMode
		{
			public BattleMusicMode()
			{
				this._factionSpecificBattleThemeSelectionFactor = 0.35f;
				this._factionSpecificSiegeThemeSelectionFactor = 0.35f;
			}

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

			private const float DefaultSelectionFactorForFactionSpecificBattleTheme = 0.35f;

			private const float SelectionFactorDecayAmountForFactionSpecificBattleTheme = 0.1f;

			private const float SelectionFactorGrowthAmountForFactionSpecificBattleTheme = 0.1f;

			private const float DefaultSelectionFactorForFactionSpecificVictoryTheme = 0.65f;

			private float _factionSpecificBattleThemeSelectionFactor;

			private float _factionSpecificSiegeThemeSelectionFactor;
		}
	}
}
