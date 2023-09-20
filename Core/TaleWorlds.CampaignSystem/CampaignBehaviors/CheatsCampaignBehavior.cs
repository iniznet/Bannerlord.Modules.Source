using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class CheatsCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		public void HourlyTick()
		{
			if (CheatsCampaignBehavior.MovingTimeForwardCheat)
			{
				CheatsCampaignBehavior.MovingTimeForward--;
			}
			if (CheatsCampaignBehavior.MovingTimeForwardCheat && (CheatsCampaignBehavior.MovingTimeForward == 0 || Campaign.Current.TimeControlMode != CampaignTimeControlMode.UnstoppableFastForward))
			{
				CheatsCampaignBehavior.MovingTimeForwardCheat = false;
				Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
			}
		}

		public void DailyTick()
		{
			if (CheatsCampaignBehavior._partyCheatEnabled)
			{
				this.LogMobilePartyNumbers();
			}
		}

		private void LogMobilePartyNumbers()
		{
			this._partyInfo.Clear();
			this._partyInfo.Add(new CheatsCampaignBehavior.PartyInfo(CampaignTime.Now, "All", MobileParty.All.Count));
			this._partyInfo.Add(new CheatsCampaignBehavior.PartyInfo(CampaignTime.Now, "Bandit", MobileParty.All.Count((MobileParty x) => x.IsBandit)));
			this._partyInfo.Add(new CheatsCampaignBehavior.PartyInfo(CampaignTime.Now, "MinorFaction", MobileParty.All.Count((MobileParty x) => x.MapFaction.IsMinorFaction)));
			this._partyInfo.Add(new CheatsCampaignBehavior.PartyInfo(CampaignTime.Now, "Lord", MobileParty.All.Count((MobileParty x) => x.LeaderHero != null && x.LeaderHero.IsLord)));
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("enable_party_count", "campaign")]
		public static string EnablePartyCount(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 1))
			{
				return "Format is \"campaign.enable_party_count 0/1\".";
			}
			if (strings[0] == "0")
			{
				CheatsCampaignBehavior._partyCheatEnabled = false;
				return "success";
			}
			if (strings[0] == "1")
			{
				CheatsCampaignBehavior._partyCheatEnabled = true;
				Campaign.Current.GetCampaignBehavior<CheatsCampaignBehavior>().LogMobilePartyNumbers();
				return "success";
			}
			return "invalid input";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("print_party_count_statistics", "campaign")]
		public static string PrintPartyCountStatistics(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (!CampaignCheats.CheckParameters(strings, 0))
			{
				return "Format is \"campaign.print_party_count_statistics\".";
			}
			string text = "";
			CheatsCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<CheatsCampaignBehavior>();
			if (!campaignBehavior._partyInfo.IsEmpty<CheatsCampaignBehavior.PartyInfo>())
			{
				foreach (CheatsCampaignBehavior.PartyInfo partyInfo in campaignBehavior._partyInfo)
				{
					text = string.Concat(new object[] { text, partyInfo.Time, " ", partyInfo.PartyType, " ", partyInfo.PartyNum, "\n" });
				}
				return text;
			}
			return "no statistics yet";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("move_time_forward", "campaign")]
		public static string MoveTimeForward(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.move_time_forward [Hours]\".";
			}
			int num = 0;
			if (!int.TryParse(strings[0], out num))
			{
				return "Enter a number.";
			}
			CheatsCampaignBehavior.MovingTimeForward = num;
			CheatsCampaignBehavior.MovingTimeForwardCheat = true;
			Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppableFastForward;
			return "Moving " + num + " hours in time.";
		}

		private static int MovingTimeForward;

		private static bool MovingTimeForwardCheat;

		private List<CheatsCampaignBehavior.PartyInfo> _partyInfo = new List<CheatsCampaignBehavior.PartyInfo>();

		private static bool _partyCheatEnabled;

		private class PartyInfo
		{
			public CampaignTime Time { get; private set; }

			public string PartyType { get; private set; }

			public int PartyNum { get; private set; }

			public PartyInfo(CampaignTime time, string partyType, int partyNum)
			{
				this.Time = time;
				this.PartyType = partyType;
				this.PartyNum = partyNum;
			}
		}
	}
}
