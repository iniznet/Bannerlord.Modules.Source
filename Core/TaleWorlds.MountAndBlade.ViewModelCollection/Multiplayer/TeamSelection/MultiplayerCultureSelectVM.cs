using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.TeamSelection
{
	// Token: 0x0200004A RID: 74
	public class MultiplayerCultureSelectVM : ViewModel
	{
		// Token: 0x06000614 RID: 1556 RVA: 0x00019454 File Offset: 0x00017654
		public MultiplayerCultureSelectVM(Action<BasicCultureObject> onCultureSelected, Action onClose)
		{
			this._onCultureSelected = onCultureSelected;
			this._onClose = onClose;
			this._firstCulture = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			this._secondCulture = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
			this.FirstCultureCode = this._firstCulture.StringId;
			this.SecondCultureCode = this._secondCulture.StringId;
			this.RefreshValues();
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x000194D0 File Offset: 0x000176D0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.GameModeText = GameTexts.FindText("str_multiplayer_official_game_type_name", MultiplayerOptions.OptionType.GameType.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)).ToString();
			this.CultureSelectionText = new TextObject("{=yQ0p8Glo}Select Culture", null).ToString();
			this.FirstCultureName = this._firstCulture.Name.ToString();
			this.SecondCultureName = this._secondCulture.Name.ToString();
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00019544 File Offset: 0x00017744
		public void ExecuteSelectCulture(int cultureIndex)
		{
			if (cultureIndex == 0)
			{
				Action<BasicCultureObject> onCultureSelected = this._onCultureSelected;
				if (onCultureSelected == null)
				{
					return;
				}
				onCultureSelected(this._firstCulture);
				return;
			}
			else
			{
				if (cultureIndex != 1)
				{
					Debug.FailedAssert("Invalid Culture Index!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\TeamSelection\\MultiplayerCultureSelectVM.cs", "ExecuteSelectCulture", 56);
					return;
				}
				Action<BasicCultureObject> onCultureSelected2 = this._onCultureSelected;
				if (onCultureSelected2 == null)
				{
					return;
				}
				onCultureSelected2(this._secondCulture);
				return;
			}
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x0001959C File Offset: 0x0001779C
		public void ExecuteClose()
		{
			Action onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			onClose();
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000618 RID: 1560 RVA: 0x000195AE File Offset: 0x000177AE
		// (set) Token: 0x06000619 RID: 1561 RVA: 0x000195B6 File Offset: 0x000177B6
		[DataSourceProperty]
		public string GameModeText
		{
			get
			{
				return this._gameModeText;
			}
			set
			{
				if (value != this._gameModeText)
				{
					this._gameModeText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameModeText");
				}
			}
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x0600061A RID: 1562 RVA: 0x000195D9 File Offset: 0x000177D9
		// (set) Token: 0x0600061B RID: 1563 RVA: 0x000195E1 File Offset: 0x000177E1
		[DataSourceProperty]
		public string CultureSelectionText
		{
			get
			{
				return this._cultureSelectionText;
			}
			set
			{
				if (value != this._cultureSelectionText)
				{
					this._cultureSelectionText = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureSelectionText");
				}
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x0600061C RID: 1564 RVA: 0x00019604 File Offset: 0x00017804
		// (set) Token: 0x0600061D RID: 1565 RVA: 0x0001960C File Offset: 0x0001780C
		[DataSourceProperty]
		public string FirstCultureName
		{
			get
			{
				return this._firstCultureName;
			}
			set
			{
				if (value != this._firstCultureName)
				{
					this._firstCultureName = value;
					base.OnPropertyChangedWithValue<string>(value, "FirstCultureName");
				}
			}
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x0600061E RID: 1566 RVA: 0x0001962F File Offset: 0x0001782F
		// (set) Token: 0x0600061F RID: 1567 RVA: 0x00019637 File Offset: 0x00017837
		[DataSourceProperty]
		public string SecondCultureName
		{
			get
			{
				return this._secondCultureName;
			}
			set
			{
				if (value != this._secondCultureName)
				{
					this._secondCultureName = value;
					base.OnPropertyChangedWithValue<string>(value, "SecondCultureName");
				}
			}
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000620 RID: 1568 RVA: 0x0001965A File Offset: 0x0001785A
		// (set) Token: 0x06000621 RID: 1569 RVA: 0x00019662 File Offset: 0x00017862
		[DataSourceProperty]
		public string FirstCultureCode
		{
			get
			{
				return this._firstCultureCode;
			}
			set
			{
				if (value != this._firstCultureCode)
				{
					this._firstCultureCode = value;
					base.OnPropertyChangedWithValue<string>(value, "FirstCultureCode");
				}
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000622 RID: 1570 RVA: 0x00019685 File Offset: 0x00017885
		// (set) Token: 0x06000623 RID: 1571 RVA: 0x0001968D File Offset: 0x0001788D
		[DataSourceProperty]
		public string SecondCultureCode
		{
			get
			{
				return this._secondCultureCode;
			}
			set
			{
				if (value != this._secondCultureCode)
				{
					this._secondCultureCode = value;
					base.OnPropertyChangedWithValue<string>(value, "SecondCultureCode");
				}
			}
		}

		// Token: 0x04000311 RID: 785
		private BasicCultureObject _firstCulture;

		// Token: 0x04000312 RID: 786
		private BasicCultureObject _secondCulture;

		// Token: 0x04000313 RID: 787
		private Action<BasicCultureObject> _onCultureSelected;

		// Token: 0x04000314 RID: 788
		private Action _onClose;

		// Token: 0x04000315 RID: 789
		private string _gameModeText;

		// Token: 0x04000316 RID: 790
		private string _cultureSelectionText;

		// Token: 0x04000317 RID: 791
		private string _firstCultureName;

		// Token: 0x04000318 RID: 792
		private string _secondCultureName;

		// Token: 0x04000319 RID: 793
		private string _firstCultureCode;

		// Token: 0x0400031A RID: 794
		private string _secondCultureCode;
	}
}
