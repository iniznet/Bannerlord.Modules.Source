using System;
using System.Linq;
using NetworkMessages.FromClient;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.FactionBanVote
{
	public class MultiplayerFactionBanVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
		}

		private void OnSelectFaction(MultiplayerFactionBanVoteVM vote)
		{
			MultiplayerFactionBanVM.VoteForCulture(CultureVoteTypes.Select, vote.Culture);
		}

		private void OnBanFaction(MultiplayerFactionBanVoteVM vote)
		{
			MultiplayerFactionBanVM.VoteForCulture(CultureVoteTypes.Ban, vote.Culture);
		}

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

		private MBBindingList<MultiplayerFactionBanVoteVM> _banList;

		private MBBindingList<MultiplayerFactionBanVoteVM> _selectList;

		private string _selectTitle;

		private string _banTitle;
	}
}
