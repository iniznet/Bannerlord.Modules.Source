using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	public class SaveHandler
	{
		public IMainHeroVisualSupplier MainHeroVisualSupplier { get; set; }

		public bool IsSaving
		{
			get
			{
				return !this.SaveArgsQueue.IsEmpty<SaveHandler.SaveArgs>();
			}
		}

		public string IronmanModSaveName
		{
			get
			{
				return "Ironman" + Campaign.Current.UniqueGameId;
			}
		}

		private bool _isAutoSaveEnabled
		{
			get
			{
				return this.AutoSaveInterval > -1;
			}
		}

		private double _autoSavePriorityTimeLimit
		{
			get
			{
				return (double)this.AutoSaveInterval * 0.75;
			}
		}

		public int AutoSaveInterval
		{
			get
			{
				ISaveManager sandBoxSaveManager = Campaign.Current.SandBoxManager.SandBoxSaveManager;
				if (sandBoxSaveManager == null)
				{
					return 15;
				}
				return sandBoxSaveManager.GetAutoSaveInterval();
			}
		}

		public void QuickSaveCurrentGame()
		{
			this.SetSaveArgs(SaveHandler.SaveArgs.SaveMode.QuickSave, null);
		}

		public void SaveAs(string saveName)
		{
			this.SetSaveArgs(SaveHandler.SaveArgs.SaveMode.SaveAs, saveName);
		}

		private void TryAutoSave(bool isPriority)
		{
			MapState mapState;
			if (this._isAutoSaveEnabled && (mapState = GameStateManager.Current.ActiveState as MapState) != null && !mapState.MapConversationActive)
			{
				double totalMinutes = (DateTime.Now - this._lastAutoSaveTime).TotalMinutes;
				double num = (isPriority ? this._autoSavePriorityTimeLimit : ((double)this.AutoSaveInterval));
				if (totalMinutes > num)
				{
					this.SetSaveArgs(SaveHandler.SaveArgs.SaveMode.AutoSave, null);
				}
			}
		}

		public void CampaignTick()
		{
			if (Campaign.Current.TimeControlMode != CampaignTimeControlMode.Stop)
			{
				this.TryAutoSave(false);
			}
		}

		internal void SaveTick()
		{
			if (!this.SaveArgsQueue.IsEmpty<SaveHandler.SaveArgs>())
			{
				switch (this._saveStep)
				{
				case SaveHandler.SaveSteps.PreSave:
					this._saveStep++;
					this.OnSaveStarted();
					return;
				case SaveHandler.SaveSteps.Saving:
				{
					this._saveStep++;
					CampaignEventDispatcher.Instance.OnBeforeSave();
					if (CampaignOptions.IsIronmanMode)
					{
						MBSaveLoad.SaveAsCurrentGame(this.GetSaveMetaData(), this.IronmanModSaveName, new Action<ValueTuple<SaveResult, string>>(this.OnSaveCompleted));
						return;
					}
					SaveHandler.SaveArgs saveArgs = this.SaveArgsQueue.Peek();
					switch (saveArgs.Mode)
					{
					case SaveHandler.SaveArgs.SaveMode.SaveAs:
						MBSaveLoad.SaveAsCurrentGame(this.GetSaveMetaData(), saveArgs.Name, new Action<ValueTuple<SaveResult, string>>(this.OnSaveCompleted));
						return;
					case SaveHandler.SaveArgs.SaveMode.QuickSave:
						MBSaveLoad.QuickSaveCurrentGame(this.GetSaveMetaData(), new Action<ValueTuple<SaveResult, string>>(this.OnSaveCompleted));
						return;
					case SaveHandler.SaveArgs.SaveMode.AutoSave:
						MBSaveLoad.AutoSaveCurrentGame(this.GetSaveMetaData(), new Action<ValueTuple<SaveResult, string>>(this.OnSaveCompleted));
						return;
					default:
						return;
					}
					break;
				}
				case SaveHandler.SaveSteps.AwaitingCompletion:
					return;
				}
				this._saveStep++;
			}
		}

		private void OnSaveCompleted(ValueTuple<SaveResult, string> result)
		{
			this._saveStep = SaveHandler.SaveSteps.PreSave;
			if (this.SaveArgsQueue.Dequeue().Mode == SaveHandler.SaveArgs.SaveMode.AutoSave)
			{
				this._lastAutoSaveTime = DateTime.Now;
			}
			this.OnSaveEnded(result.Item1 == SaveResult.Success, result.Item2);
		}

		public void SignalAutoSave()
		{
			this.TryAutoSave(true);
		}

		private void OnSaveStarted()
		{
			Campaign.Current.WaitAsyncTasks();
			CampaignEventDispatcher.Instance.OnSaveStarted();
			MBInformationManager.HideInformations();
		}

		private void OnSaveEnded(bool isSaveSuccessful, string newSaveGameName)
		{
			ISaveManager sandBoxSaveManager = Campaign.Current.SandBoxManager.SandBoxSaveManager;
			if (sandBoxSaveManager != null)
			{
				sandBoxSaveManager.OnSaveOver(isSaveSuccessful, newSaveGameName);
			}
			CampaignEventDispatcher.Instance.OnSaveOver(isSaveSuccessful, newSaveGameName);
			if (!isSaveSuccessful)
			{
				MBInformationManager.AddQuickInformation(new TextObject("{=u9PPxTNL}Save Error!", null), 0, null, "");
			}
		}

		private void SetSaveArgs(SaveHandler.SaveArgs.SaveMode saveType, string saveName = null)
		{
			this.SaveArgsQueue.Enqueue(new SaveHandler.SaveArgs(saveType, saveName));
		}

		public CampaignSaveMetaDataArgs GetSaveMetaData()
		{
			string[] array = SandBoxManager.Instance.ModuleManager.ModuleNames.ToArray<string>();
			KeyValuePair<string, string>[] array2 = new KeyValuePair<string, string>[15];
			array2[0] = new KeyValuePair<string, string>("UniqueGameId", Campaign.Current.UniqueGameId ?? "");
			array2[1] = new KeyValuePair<string, string>("MainHeroLevel", Hero.MainHero.Level.ToString(SaveHandler._invariantCulture));
			array2[2] = new KeyValuePair<string, string>("MainPartyFood", Campaign.Current.MainParty.Food.ToString(SaveHandler._invariantCulture));
			array2[3] = new KeyValuePair<string, string>("MainHeroGold", Hero.MainHero.Gold.ToString(SaveHandler._invariantCulture));
			array2[4] = new KeyValuePair<string, string>("ClanInfluence", Clan.PlayerClan.Influence.ToString(SaveHandler._invariantCulture));
			array2[5] = new KeyValuePair<string, string>("ClanFiefs", Clan.PlayerClan.Settlements.Count.ToString(SaveHandler._invariantCulture));
			array2[6] = new KeyValuePair<string, string>("MainPartyHealthyMemberCount", Campaign.Current.MainParty.MemberRoster.TotalHealthyCount.ToString(SaveHandler._invariantCulture));
			array2[7] = new KeyValuePair<string, string>("MainPartyPrisonerMemberCount", Campaign.Current.MainParty.PrisonRoster.TotalManCount.ToString(SaveHandler._invariantCulture));
			array2[8] = new KeyValuePair<string, string>("MainPartyWoundedMemberCount", Campaign.Current.MainParty.MemberRoster.TotalWounded.ToString(SaveHandler._invariantCulture));
			int num = 9;
			string text = "CharacterName";
			TextObject name = Hero.MainHero.Name;
			array2[num] = new KeyValuePair<string, string>(text, (name != null) ? name.ToString() : null);
			array2[10] = new KeyValuePair<string, string>("DayLong", Campaign.Current.CampaignStartTime.ElapsedDaysUntilNow.ToString(SaveHandler._invariantCulture));
			array2[11] = new KeyValuePair<string, string>("ClanBannerCode", Clan.PlayerClan.Banner.Serialize());
			int num2 = 12;
			string text2 = "MainHeroVisual";
			IMainHeroVisualSupplier mainHeroVisualSupplier = this.MainHeroVisualSupplier;
			array2[num2] = new KeyValuePair<string, string>(text2, ((mainHeroVisualSupplier != null) ? mainHeroVisualSupplier.GetMainHeroVisualCode() : null) ?? string.Empty);
			array2[13] = new KeyValuePair<string, string>("IronmanMode", (CampaignOptions.IsIronmanMode ? 1 : 0).ToString());
			array2[14] = new KeyValuePair<string, string>("HealthPercentage", MBMath.ClampInt(Hero.MainHero.HitPoints * 100 / Hero.MainHero.MaxHitPoints, 1, 100).ToString());
			return new CampaignSaveMetaDataArgs(array, array2);
		}

		private SaveHandler.SaveSteps _saveStep;

		private static readonly CultureInfo _invariantCulture = CultureInfo.InvariantCulture;

		private Queue<SaveHandler.SaveArgs> SaveArgsQueue = new Queue<SaveHandler.SaveArgs>();

		private DateTime _lastAutoSaveTime = DateTime.Now;

		private readonly struct SaveArgs
		{
			public SaveArgs(SaveHandler.SaveArgs.SaveMode mode, string name)
			{
				this.Mode = mode;
				this.Name = name;
			}

			public readonly SaveHandler.SaveArgs.SaveMode Mode;

			public readonly string Name;

			public enum SaveMode
			{
				SaveAs,
				QuickSave,
				AutoSave
			}
		}

		private enum SaveSteps
		{
			PreSave,
			Saving = 2,
			AwaitingCompletion
		}
	}
}
