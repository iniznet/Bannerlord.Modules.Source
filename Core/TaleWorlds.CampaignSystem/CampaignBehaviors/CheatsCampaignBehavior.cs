using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x02000382 RID: 898
	public class CheatsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06003493 RID: 13459 RVA: 0x000DFA10 File Offset: 0x000DDC10
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
		}

		// Token: 0x06003494 RID: 13460 RVA: 0x000DFA40 File Offset: 0x000DDC40
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06003495 RID: 13461 RVA: 0x000DFA44 File Offset: 0x000DDC44
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

		// Token: 0x06003496 RID: 13462 RVA: 0x000DFA90 File Offset: 0x000DDC90
		public void DailyTick()
		{
			if (CheatsCampaignBehavior._partyCheatEnabled)
			{
				this.LogMobilePartyNumbers();
			}
		}

		// Token: 0x06003497 RID: 13463 RVA: 0x000DFAA0 File Offset: 0x000DDCA0
		private void LogMobilePartyNumbers()
		{
			this._partyInfo.Clear();
			this._partyInfo.Add(new CheatsCampaignBehavior.PartyInfo(CampaignTime.Now, "All", MobileParty.All.Count));
			this._partyInfo.Add(new CheatsCampaignBehavior.PartyInfo(CampaignTime.Now, "Bandit", MobileParty.All.Count((MobileParty x) => x.IsBandit)));
			this._partyInfo.Add(new CheatsCampaignBehavior.PartyInfo(CampaignTime.Now, "MinorFaction", MobileParty.All.Count((MobileParty x) => x.MapFaction.IsMinorFaction)));
			this._partyInfo.Add(new CheatsCampaignBehavior.PartyInfo(CampaignTime.Now, "Lord", MobileParty.All.Count((MobileParty x) => x.LeaderHero != null && x.LeaderHero.IsLord)));
		}

		// Token: 0x06003498 RID: 13464 RVA: 0x000DFBA8 File Offset: 0x000DDDA8
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

		// Token: 0x06003499 RID: 13465 RVA: 0x000DFC28 File Offset: 0x000DDE28
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

		// Token: 0x0600349A RID: 13466 RVA: 0x000DFD08 File Offset: 0x000DDF08
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

		// Token: 0x04001119 RID: 4377
		private static int MovingTimeForward;

		// Token: 0x0400111A RID: 4378
		private static bool MovingTimeForwardCheat;

		// Token: 0x0400111B RID: 4379
		private List<CheatsCampaignBehavior.PartyInfo> _partyInfo = new List<CheatsCampaignBehavior.PartyInfo>();

		// Token: 0x0400111C RID: 4380
		private static bool _partyCheatEnabled;

		// Token: 0x020006C1 RID: 1729
		private class PartyInfo
		{
			// Token: 0x1700135D RID: 4957
			// (get) Token: 0x06005435 RID: 21557 RVA: 0x0016A91B File Offset: 0x00168B1B
			// (set) Token: 0x06005436 RID: 21558 RVA: 0x0016A923 File Offset: 0x00168B23
			public CampaignTime Time { get; private set; }

			// Token: 0x1700135E RID: 4958
			// (get) Token: 0x06005437 RID: 21559 RVA: 0x0016A92C File Offset: 0x00168B2C
			// (set) Token: 0x06005438 RID: 21560 RVA: 0x0016A934 File Offset: 0x00168B34
			public string PartyType { get; private set; }

			// Token: 0x1700135F RID: 4959
			// (get) Token: 0x06005439 RID: 21561 RVA: 0x0016A93D File Offset: 0x00168B3D
			// (set) Token: 0x0600543A RID: 21562 RVA: 0x0016A945 File Offset: 0x00168B45
			public int PartyNum { get; private set; }

			// Token: 0x0600543B RID: 21563 RVA: 0x0016A94E File Offset: 0x00168B4E
			public PartyInfo(CampaignTime time, string partyType, int partyNum)
			{
				this.Time = time;
				this.PartyType = partyType;
				this.PartyNum = partyNum;
			}
		}
	}
}
