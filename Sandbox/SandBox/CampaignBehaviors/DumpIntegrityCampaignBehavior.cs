using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SandBox.CampaignBehaviors
{
	public class DumpIntegrityCampaignBehavior : CampaignBehaviorBase
	{
		public override void SyncData(IDataStore dataStore)
		{
			TextObject textObject;
			DumpIntegrityCampaignBehavior.IsGameIntegrityAchieved(out textObject);
			this.UpdateDumpInfo();
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnConfigChangedEvent.AddNonSerializedListener(this, new Action(this.OnConfigChanged));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
		}

		private void OnConfigChanged()
		{
			TextObject textObject;
			DumpIntegrityCampaignBehavior.IsGameIntegrityAchieved(out textObject);
			this.UpdateDumpInfo();
		}

		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter campaignGameStarter)
		{
			TextObject textObject;
			DumpIntegrityCampaignBehavior.IsGameIntegrityAchieved(out textObject);
			this.UpdateDumpInfo();
		}

		private void UpdateDumpInfo()
		{
			this._saveIntegrityDumpInfo.Clear();
			this._usedModulesDumpInfo.Clear();
			this._usedVersionsDumpInfo.Clear();
			Campaign campaign = Campaign.Current;
			if (((campaign != null) ? campaign.PreviouslyUsedModules : null) != null && Campaign.Current.UsedGameVersions != null && Campaign.Current.NewGameVersion != null)
			{
				this._saveIntegrityDumpInfo.Add(new KeyValuePair<string, string>("New Game Version", Campaign.Current.NewGameVersion));
				this._saveIntegrityDumpInfo.Add(new KeyValuePair<string, string>("Has Used Cheats", (!DumpIntegrityCampaignBehavior.CheckCheatUsage()).ToString()));
				this._saveIntegrityDumpInfo.Add(new KeyValuePair<string, string>("Has Installed Unofficial Modules", (!DumpIntegrityCampaignBehavior.CheckIfModulesAreDefault()).ToString()));
				this._saveIntegrityDumpInfo.Add(new KeyValuePair<string, string>("Has Reverted to Older Versions", (!DumpIntegrityCampaignBehavior.CheckIfVersionIntegrityIsAchieved()).ToString()));
				TextObject textObject;
				this._saveIntegrityDumpInfo.Add(new KeyValuePair<string, string>("Game Integrity is Achieved", DumpIntegrityCampaignBehavior.IsGameIntegrityAchieved(out textObject).ToString()));
			}
			Campaign campaign2 = Campaign.Current;
			if (((campaign2 != null) ? campaign2.PreviouslyUsedModules : null) != null)
			{
				string[] moduleNames = SandBoxManager.Instance.ModuleManager.ModuleNames;
				using (List<string>.Enumerator enumerator = Campaign.Current.PreviouslyUsedModules.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string module = enumerator.Current;
						bool flag = Extensions.FindIndex<string>(moduleNames, (string x) => x == module) != -1;
						this._usedModulesDumpInfo.Add(new KeyValuePair<string, string>(module, flag.ToString()));
					}
				}
			}
			Campaign campaign3 = Campaign.Current;
			if (((campaign3 != null) ? campaign3.UsedGameVersions : null) != null && Campaign.Current.UsedGameVersions.Count > 0)
			{
				foreach (string text in Campaign.Current.UsedGameVersions)
				{
					this._usedVersionsDumpInfo.Add(new KeyValuePair<string, string>(text, ""));
				}
			}
			this.SendDataToWatchdog();
		}

		private void SendDataToWatchdog()
		{
			foreach (KeyValuePair<string, string> keyValuePair in this._saveIntegrityDumpInfo)
			{
				Utilities.SetWatchdogValue("crash_tags.txt", "Campaign Dump Integrity", keyValuePair.Key, keyValuePair.Value);
			}
			foreach (KeyValuePair<string, string> keyValuePair2 in this._usedModulesDumpInfo)
			{
				Utilities.SetWatchdogValue("crash_tags.txt", "Used Mods", keyValuePair2.Key, keyValuePair2.Value);
			}
			foreach (KeyValuePair<string, string> keyValuePair3 in this._usedVersionsDumpInfo)
			{
				Utilities.SetWatchdogValue("crash_tags.txt", "Used Game Versions", keyValuePair3.Key, keyValuePair3.Value);
			}
		}

		public static bool IsGameIntegrityAchieved(out TextObject reason)
		{
			reason = TextObject.Empty;
			bool flag = true;
			if (!DumpIntegrityCampaignBehavior.CheckCheatUsage())
			{
				reason = new TextObject("{=sO8Zh3ZH}Achievements are disabled due to cheat usage.", null);
				flag = false;
			}
			else if (!DumpIntegrityCampaignBehavior.CheckIfModulesAreDefault())
			{
				reason = new TextObject("{=R0AbAxqX}Achievements are disabled due to unofficial modules.", null);
				flag = false;
			}
			else if (!DumpIntegrityCampaignBehavior.CheckIfVersionIntegrityIsAchieved())
			{
				reason = new TextObject("{=dt00CQCM}Achievements are disabled due to version downgrade.", null);
				flag = false;
			}
			return flag;
		}

		private static bool CheckIfVersionIntegrityIsAchieved()
		{
			for (int i = 0; i < Campaign.Current.UsedGameVersions.Count; i++)
			{
				if (i < Campaign.Current.UsedGameVersions.Count - 1 && ApplicationVersion.FromString(Campaign.Current.UsedGameVersions[i], 24202) > ApplicationVersion.FromString(Campaign.Current.UsedGameVersions[i + 1], 24202))
				{
					Debug.Print("Dump integrity is compromised due to version downgrade", 0, 0, 17592186044416UL);
					return false;
				}
			}
			return true;
		}

		private static bool CheckIfModulesAreDefault()
		{
			bool flag = Campaign.Current.PreviouslyUsedModules.All((string x) => x.Equals("Native", StringComparison.OrdinalIgnoreCase) || x.Equals("SandBoxCore", StringComparison.OrdinalIgnoreCase) || x.Equals("CustomBattle", StringComparison.OrdinalIgnoreCase) || x.Equals("SandBox", StringComparison.OrdinalIgnoreCase) || x.Equals("Multiplayer", StringComparison.OrdinalIgnoreCase) || x.Equals("BirthAndDeath", StringComparison.OrdinalIgnoreCase) || x.Equals("StoryMode", StringComparison.OrdinalIgnoreCase));
			if (!flag)
			{
				Debug.Print("Dump integrity is compromised due to non-default modules being used", 0, 0, 17592186044416UL);
			}
			return flag;
		}

		private static bool CheckCheatUsage()
		{
			if (!Campaign.Current.EnabledCheatsBefore && Game.Current.CheatMode)
			{
				Campaign.Current.EnabledCheatsBefore = Game.Current.CheatMode;
			}
			if (Campaign.Current.EnabledCheatsBefore)
			{
				Debug.Print("Dump integrity is compromised due to cheat usage", 0, 0, 17592186044416UL);
			}
			return !Campaign.Current.EnabledCheatsBefore;
		}

		private readonly List<KeyValuePair<string, string>> _saveIntegrityDumpInfo = new List<KeyValuePair<string, string>>();

		private readonly List<KeyValuePair<string, string>> _usedModulesDumpInfo = new List<KeyValuePair<string, string>>();

		private readonly List<KeyValuePair<string, string>> _usedVersionsDumpInfo = new List<KeyValuePair<string, string>>();
	}
}
