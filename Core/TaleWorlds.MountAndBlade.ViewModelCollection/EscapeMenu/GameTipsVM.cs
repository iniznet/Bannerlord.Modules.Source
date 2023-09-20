using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.EscapeMenu
{
	public class GameTipsVM : ViewModel
	{
		public GameTipsVM(bool isAutoChangeEnabled, bool navigationButtonsEnabled)
		{
			this._navigationButtonsEnabled = navigationButtonsEnabled;
			this._isAutoChangeEnabled = isAutoChangeEnabled;
			this.RefreshValues();
		}

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

		public void ExecutePreviousTip()
		{
			this._currentTipIndex--;
			if (this._currentTipIndex < 0)
			{
				this._currentTipIndex = this._allTips.Count - 1;
			}
			this.CurrentTip = this._allTips[this._currentTipIndex];
		}

		public void ExecuteNextTip()
		{
			this._currentTipIndex = (this._currentTipIndex + 1) % this._allTips.Count;
			this.CurrentTip = this._allTips[this._currentTipIndex];
		}

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

		private MBList<string> _allTips;

		private readonly float _tipTimeInterval = 5f;

		private readonly bool _isAutoChangeEnabled;

		private int _currentTipIndex;

		private float _totalDt;

		private string _currentTip;

		private string _gameTipTitle;

		private bool _navigationButtonsEnabled;
	}
}
