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
	// Token: 0x0200009F RID: 159
	public class SaveHandler
	{
		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06001159 RID: 4441 RVA: 0x0004FC8D File Offset: 0x0004DE8D
		// (set) Token: 0x0600115A RID: 4442 RVA: 0x0004FC95 File Offset: 0x0004DE95
		public IMainHeroVisualSupplier MainHeroVisualSupplier { get; set; }

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x0600115B RID: 4443 RVA: 0x0004FC9E File Offset: 0x0004DE9E
		public bool IsSaving
		{
			get
			{
				return !this.SaveArgsQueue.IsEmpty<SaveHandler.SaveArgs>();
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x0600115C RID: 4444 RVA: 0x0004FCAE File Offset: 0x0004DEAE
		public string IronmanModSaveName
		{
			get
			{
				return "Ironman" + Campaign.Current.UniqueGameId;
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x0600115D RID: 4445 RVA: 0x0004FCC4 File Offset: 0x0004DEC4
		private bool _isAutoSaveEnabled
		{
			get
			{
				return this.AutoSaveInterval > -1;
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x0600115E RID: 4446 RVA: 0x0004FCCF File Offset: 0x0004DECF
		private double _autoSavePriorityTimeLimit
		{
			get
			{
				return (double)this.AutoSaveInterval * 0.75;
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x0600115F RID: 4447 RVA: 0x0004FCE2 File Offset: 0x0004DEE2
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

		// Token: 0x06001160 RID: 4448 RVA: 0x0004FCFF File Offset: 0x0004DEFF
		public void QuickSaveCurrentGame()
		{
			this.SetSaveArgs(SaveHandler.SaveArgs.SaveMode.QuickSave, null);
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x0004FD09 File Offset: 0x0004DF09
		public void SaveAs(string saveName)
		{
			this.SetSaveArgs(SaveHandler.SaveArgs.SaveMode.SaveAs, saveName);
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x0004FD14 File Offset: 0x0004DF14
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

		// Token: 0x06001163 RID: 4451 RVA: 0x0004FD7A File Offset: 0x0004DF7A
		public void CampaignTick()
		{
			if (Campaign.Current.TimeControlMode != CampaignTimeControlMode.Stop)
			{
				this.TryAutoSave(false);
			}
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x0004FD90 File Offset: 0x0004DF90
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

		// Token: 0x06001165 RID: 4453 RVA: 0x0004FEA3 File Offset: 0x0004E0A3
		private void OnSaveCompleted(ValueTuple<SaveResult, string> result)
		{
			this._saveStep = SaveHandler.SaveSteps.PreSave;
			if (this.SaveArgsQueue.Dequeue().Mode == SaveHandler.SaveArgs.SaveMode.AutoSave)
			{
				this._lastAutoSaveTime = DateTime.Now;
			}
			this.OnSaveEnded(result.Item1 == SaveResult.Success, result.Item2);
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x0004FEDF File Offset: 0x0004E0DF
		public void SignalAutoSave()
		{
			this.TryAutoSave(true);
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x0004FEE8 File Offset: 0x0004E0E8
		private void OnSaveStarted()
		{
			Campaign.Current.WaitAsyncTasks();
			CampaignEventDispatcher.Instance.OnSaveStarted();
			MBInformationManager.HideInformations();
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x0004FF04 File Offset: 0x0004E104
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

		// Token: 0x06001169 RID: 4457 RVA: 0x0004FF53 File Offset: 0x0004E153
		private void SetSaveArgs(SaveHandler.SaveArgs.SaveMode saveType, string saveName = null)
		{
			this.SaveArgsQueue.Enqueue(new SaveHandler.SaveArgs(saveType, saveName));
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x0004FF68 File Offset: 0x0004E168
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

		// Token: 0x04000600 RID: 1536
		private SaveHandler.SaveSteps _saveStep;

		// Token: 0x04000601 RID: 1537
		private static readonly CultureInfo _invariantCulture = CultureInfo.InvariantCulture;

		// Token: 0x04000603 RID: 1539
		private Queue<SaveHandler.SaveArgs> SaveArgsQueue = new Queue<SaveHandler.SaveArgs>();

		// Token: 0x04000604 RID: 1540
		private DateTime _lastAutoSaveTime = DateTime.Now;

		// Token: 0x020004CB RID: 1227
		private readonly struct SaveArgs
		{
			// Token: 0x06004163 RID: 16739 RVA: 0x001336FA File Offset: 0x001318FA
			public SaveArgs(SaveHandler.SaveArgs.SaveMode mode, string name)
			{
				this.Mode = mode;
				this.Name = name;
			}

			// Token: 0x040014AE RID: 5294
			public readonly SaveHandler.SaveArgs.SaveMode Mode;

			// Token: 0x040014AF RID: 5295
			public readonly string Name;

			// Token: 0x02000778 RID: 1912
			public enum SaveMode
			{
				// Token: 0x04001EA0 RID: 7840
				SaveAs,
				// Token: 0x04001EA1 RID: 7841
				QuickSave,
				// Token: 0x04001EA2 RID: 7842
				AutoSave
			}
		}

		// Token: 0x020004CC RID: 1228
		private enum SaveSteps
		{
			// Token: 0x040014B1 RID: 5297
			PreSave,
			// Token: 0x040014B2 RID: 5298
			Saving = 2,
			// Token: 0x040014B3 RID: 5299
			AwaitingCompletion
		}
	}
}
