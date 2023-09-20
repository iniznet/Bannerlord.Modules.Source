using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby
{
	// Token: 0x0200005C RID: 92
	public class MPLobbyGameTypeVM : ViewModel
	{
		// Token: 0x060007CC RID: 1996 RVA: 0x0001E5B5 File Offset: 0x0001C7B5
		public MPLobbyGameTypeVM(string gameType, bool isCasual, Action<string> onSelection)
		{
			this.GameTypeID = gameType;
			this.IsCasual = isCasual;
			this._onSelection = onSelection;
			this.RefreshValues();
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x0001E5D8 File Offset: 0x0001C7D8
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Hint = new HintViewModel(GameTexts.FindText("str_multiplayer_game_stats_description", this.GameTypeID), null);
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x0001E5FC File Offset: 0x0001C7FC
		private void OnSelected()
		{
			Action<string> onSelection = this._onSelection;
			if (onSelection == null)
			{
				return;
			}
			onSelection(this.GameTypeID);
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x060007CF RID: 1999 RVA: 0x0001E614 File Offset: 0x0001C814
		// (set) Token: 0x060007D0 RID: 2000 RVA: 0x0001E61C File Offset: 0x0001C81C
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
					if (value)
					{
						this.OnSelected();
					}
				}
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x060007D1 RID: 2001 RVA: 0x0001E643 File Offset: 0x0001C843
		// (set) Token: 0x060007D2 RID: 2002 RVA: 0x0001E64B File Offset: 0x0001C84B
		[DataSourceProperty]
		public string GameTypeID
		{
			get
			{
				return this._gameTypeID;
			}
			set
			{
				if (value != this._gameTypeID)
				{
					this._gameTypeID = value;
					base.OnPropertyChangedWithValue<string>(value, "GameTypeID");
				}
			}
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x060007D3 RID: 2003 RVA: 0x0001E66E File Offset: 0x0001C86E
		// (set) Token: 0x060007D4 RID: 2004 RVA: 0x0001E676 File Offset: 0x0001C876
		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x040003F6 RID: 1014
		private readonly Action<string> _onSelection;

		// Token: 0x040003F7 RID: 1015
		public readonly bool IsCasual;

		// Token: 0x040003F8 RID: 1016
		private bool _isSelected;

		// Token: 0x040003F9 RID: 1017
		private string _gameTypeID;

		// Token: 0x040003FA RID: 1018
		private HintViewModel _hint;
	}
}
