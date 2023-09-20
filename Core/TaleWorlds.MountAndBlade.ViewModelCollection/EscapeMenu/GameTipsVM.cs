using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu
{
	// Token: 0x0200010A RID: 266
	public class GameTipsVM : ViewModel
	{
		// Token: 0x060017DD RID: 6109 RVA: 0x0004EE8B File Offset: 0x0004D08B
		public GameTipsVM(bool isAutoChangeEnabled, bool navigationButtonsEnabled)
		{
			this._navigationButtonsEnabled = navigationButtonsEnabled;
			this._isAutoChangeEnabled = isAutoChangeEnabled;
			this.RefreshValues();
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x0004EEB4 File Offset: 0x0004D0B4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this._allTips = new MBList<string>();
			this.GameTipTitle = GameTexts.FindText("str_game_tip_title", null).ToString();
			string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4));
			GameTexts.SetVariable("LEAVE_AREA_KEY", keyHyperlinkText);
			string keyHyperlinkText2 = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 5));
			GameTexts.SetVariable("MISSION_INDICATORS_KEY", keyHyperlinkText2);
			GameTexts.SetVariable("EXTEND_KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("MapHotKeyCategory", "MapFollowModifier")));
			GameTexts.SetVariable("ENCYCLOPEDIA_SHORTCUT", HyperlinkTexts.GetKeyHyperlinkText("RightMouseButton"));
			if (Input.IsMouseActive)
			{
				foreach (TextObject textObject in GameTexts.FindAllTextVariations("str_game_tip_pc"))
				{
					this._allTips.Add(textObject.ToString());
				}
			}
			foreach (TextObject textObject2 in GameTexts.FindAllTextVariations("str_game_tip"))
			{
				this._allTips.Add(textObject2.ToString());
			}
			this.NavigationButtonsEnabled = this._allTips.Count > 1;
			this.CurrentTip = ((this._allTips.Count == 0) ? string.Empty : this._allTips.GetRandomElement<string>());
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x0004F02C File Offset: 0x0004D22C
		public void ExecutePreviousTip()
		{
			this._currentTipIndex--;
			if (this._currentTipIndex < 0)
			{
				this._currentTipIndex = this._allTips.Count - 1;
			}
			this.CurrentTip = this._allTips[this._currentTipIndex];
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x0004F07A File Offset: 0x0004D27A
		public void ExecuteNextTip()
		{
			this._currentTipIndex = (this._currentTipIndex + 1) % this._allTips.Count;
			this.CurrentTip = this._allTips[this._currentTipIndex];
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x0004F0AD File Offset: 0x0004D2AD
		public void OnTick(float dt)
		{
			if (this._isAutoChangeEnabled)
			{
				this._totalDt += dt;
				if (this._totalDt > this._tipTimeInterval)
				{
					this.ExecuteNextTip();
					this._totalDt = 0f;
				}
			}
		}

		// Token: 0x170007CC RID: 1996
		// (get) Token: 0x060017E2 RID: 6114 RVA: 0x0004F0E4 File Offset: 0x0004D2E4
		// (set) Token: 0x060017E3 RID: 6115 RVA: 0x0004F0EC File Offset: 0x0004D2EC
		[DataSourceProperty]
		public string CurrentTip
		{
			get
			{
				return this._currentTip;
			}
			set
			{
				if (value != this._currentTip)
				{
					this._currentTip = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentTip");
				}
			}
		}

		// Token: 0x170007CD RID: 1997
		// (get) Token: 0x060017E4 RID: 6116 RVA: 0x0004F10F File Offset: 0x0004D30F
		// (set) Token: 0x060017E5 RID: 6117 RVA: 0x0004F117 File Offset: 0x0004D317
		[DataSourceProperty]
		public string GameTipTitle
		{
			get
			{
				return this._gameTipTitle;
			}
			set
			{
				if (value != this._gameTipTitle)
				{
					this._gameTipTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "GameTipTitle");
				}
			}
		}

		// Token: 0x170007CE RID: 1998
		// (get) Token: 0x060017E6 RID: 6118 RVA: 0x0004F13A File Offset: 0x0004D33A
		// (set) Token: 0x060017E7 RID: 6119 RVA: 0x0004F142 File Offset: 0x0004D342
		[DataSourceProperty]
		public bool NavigationButtonsEnabled
		{
			get
			{
				return this._navigationButtonsEnabled;
			}
			set
			{
				if (value != this._navigationButtonsEnabled)
				{
					this._navigationButtonsEnabled = value;
					base.OnPropertyChangedWithValue(value, "NavigationButtonsEnabled");
				}
			}
		}

		// Token: 0x04000B6A RID: 2922
		private MBList<string> _allTips;

		// Token: 0x04000B6B RID: 2923
		private readonly float _tipTimeInterval = 5f;

		// Token: 0x04000B6C RID: 2924
		private readonly bool _isAutoChangeEnabled;

		// Token: 0x04000B6D RID: 2925
		private int _currentTipIndex;

		// Token: 0x04000B6E RID: 2926
		private float _totalDt;

		// Token: 0x04000B6F RID: 2927
		private string _currentTip;

		// Token: 0x04000B70 RID: 2928
		private string _gameTipTitle;

		// Token: 0x04000B71 RID: 2929
		private bool _navigationButtonsEnabled;
	}
}
