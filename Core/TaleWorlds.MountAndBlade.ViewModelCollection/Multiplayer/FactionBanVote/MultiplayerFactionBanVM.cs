using System;
using System.Linq;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FactionBanVote
{
	// Token: 0x020000C3 RID: 195
	public class MultiplayerFactionBanVM : ViewModel
	{
		// Token: 0x06001269 RID: 4713 RVA: 0x0003C944 File Offset: 0x0003AB44
		public MultiplayerFactionBanVM()
		{
			this.SelectTitle = "SELECT FACTION";
			this.BanTitle = "BAN FACTION";
			this._banList = new MBBindingList<MultiplayerFactionBanVoteVM>();
			foreach (BasicCultureObject basicCultureObject in MultiplayerClassDivisions.AvailableCultures)
			{
				this._banList.Add(new MultiplayerFactionBanVoteVM(basicCultureObject, new Action<MultiplayerFactionBanVoteVM>(this.OnBanFaction)));
			}
			this._selectList = new MBBindingList<MultiplayerFactionBanVoteVM>();
			foreach (BasicCultureObject basicCultureObject2 in MultiplayerClassDivisions.AvailableCultures)
			{
				this._selectList.Add(new MultiplayerFactionBanVoteVM(basicCultureObject2, new Action<MultiplayerFactionBanVoteVM>(this.OnSelectFaction)));
			}
			foreach (MultiplayerFactionBanVoteVM multiplayerFactionBanVoteVM in this._selectList)
			{
				if (multiplayerFactionBanVoteVM.IsEnabled)
				{
					multiplayerFactionBanVoteVM.IsSelected = true;
					break;
				}
			}
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x0003CA74 File Offset: 0x0003AC74
		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		// Token: 0x0600126B RID: 4715 RVA: 0x0003CA7C File Offset: 0x0003AC7C
		public override void OnFinalize()
		{
			base.OnFinalize();
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x0003CA84 File Offset: 0x0003AC84
		private void OnSelectFaction(MultiplayerFactionBanVoteVM vote)
		{
			MultiplayerFactionBanVM.VoteForCulture(CultureVoteTypes.Select, vote.Culture);
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x0003CA92 File Offset: 0x0003AC92
		private void OnBanFaction(MultiplayerFactionBanVoteVM vote)
		{
			MultiplayerFactionBanVM.VoteForCulture(CultureVoteTypes.Ban, vote.Culture);
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x0003CAA0 File Offset: 0x0003ACA0
		private void Refresh()
		{
			foreach (MultiplayerFactionBanVoteVM multiplayerFactionBanVoteVM in this._banList)
			{
				multiplayerFactionBanVoteVM.IsSelected = false;
				multiplayerFactionBanVoteVM.IsEnabled = false;
			}
			MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
			bool flag = false;
			foreach (MultiplayerFactionBanVoteVM multiplayerFactionBanVoteVM2 in this._selectList)
			{
				if (flag)
				{
					multiplayerFactionBanVoteVM2.IsSelected = true;
					flag = false;
					break;
				}
				if (component.VotedForBan == multiplayerFactionBanVoteVM2.Culture)
				{
					multiplayerFactionBanVoteVM2.IsEnabled = false;
					if (multiplayerFactionBanVoteVM2.IsSelected)
					{
						multiplayerFactionBanVoteVM2.IsSelected = false;
						flag = true;
					}
				}
			}
			if (flag)
			{
				MultiplayerFactionBanVoteVM multiplayerFactionBanVoteVM3 = this._selectList.FirstOrDefault((MultiplayerFactionBanVoteVM s) => s.IsEnabled);
				if (multiplayerFactionBanVoteVM3 != null)
				{
					multiplayerFactionBanVoteVM3.IsSelected = true;
				}
			}
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x0003CBA4 File Offset: 0x0003ADA4
		private static void VoteForCulture(CultureVoteTypes voteType, BasicCultureObject culture)
		{
			MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
			if (GameNetwork.IsServer)
			{
				component.HandleVoteChange(voteType, culture);
				return;
			}
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new CultureVoteClient(voteType, culture));
				GameNetwork.EndModuleEventAsClient();
			}
		}

		// Token: 0x17000602 RID: 1538
		// (get) Token: 0x06001270 RID: 4720 RVA: 0x0003CBE9 File Offset: 0x0003ADE9
		// (set) Token: 0x06001271 RID: 4721 RVA: 0x0003CBF1 File Offset: 0x0003ADF1
		[DataSourceProperty]
		public MBBindingList<MultiplayerFactionBanVoteVM> SelectList
		{
			get
			{
				return this._selectList;
			}
			set
			{
				if (value != this._selectList)
				{
					this._selectList = value;
					base.OnPropertyChangedWithValue<MBBindingList<MultiplayerFactionBanVoteVM>>(value, "SelectList");
				}
			}
		}

		// Token: 0x17000603 RID: 1539
		// (get) Token: 0x06001272 RID: 4722 RVA: 0x0003CC0F File Offset: 0x0003AE0F
		// (set) Token: 0x06001273 RID: 4723 RVA: 0x0003CC17 File Offset: 0x0003AE17
		[DataSourceProperty]
		public MBBindingList<MultiplayerFactionBanVoteVM> BanList
		{
			get
			{
				return this._banList;
			}
			set
			{
				if (value != this._banList)
				{
					this._banList = value;
					base.OnPropertyChangedWithValue<MBBindingList<MultiplayerFactionBanVoteVM>>(value, "BanList");
				}
			}
		}

		// Token: 0x17000604 RID: 1540
		// (get) Token: 0x06001274 RID: 4724 RVA: 0x0003CC35 File Offset: 0x0003AE35
		// (set) Token: 0x06001275 RID: 4725 RVA: 0x0003CC3D File Offset: 0x0003AE3D
		[DataSourceProperty]
		public string SelectTitle
		{
			get
			{
				return this._selectTitle;
			}
			set
			{
				if (value != this._selectTitle)
				{
					this._selectTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectTitle");
				}
			}
		}

		// Token: 0x17000605 RID: 1541
		// (get) Token: 0x06001276 RID: 4726 RVA: 0x0003CC60 File Offset: 0x0003AE60
		// (set) Token: 0x06001277 RID: 4727 RVA: 0x0003CC68 File Offset: 0x0003AE68
		[DataSourceProperty]
		public string BanTitle
		{
			get
			{
				return this._banTitle;
			}
			set
			{
				if (value != this._banTitle)
				{
					this._banTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "BanTitle");
				}
			}
		}

		// Token: 0x040008CC RID: 2252
		private MBBindingList<MultiplayerFactionBanVoteVM> _banList;

		// Token: 0x040008CD RID: 2253
		private MBBindingList<MultiplayerFactionBanVoteVM> _selectList;

		// Token: 0x040008CE RID: 2254
		private string _selectTitle;

		// Token: 0x040008CF RID: 2255
		private string _banTitle;
	}
}
