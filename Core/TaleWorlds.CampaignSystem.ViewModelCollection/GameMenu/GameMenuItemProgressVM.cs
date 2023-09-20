using System;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu
{
	public class GameMenuItemProgressVM : ViewModel
	{
		public GameMenuItemProgressVM(MenuContext context, int virtualIndex)
		{
			this._context = context;
			this._gameMenuManager = Campaign.Current.GameMenuManager;
			this._virtualIndex = virtualIndex;
			this._text1 = Campaign.Current.GameMenuManager.GetVirtualMenuOptionText(this._context, this._virtualIndex).ToString();
			this._text2 = Campaign.Current.GameMenuManager.GetVirtualMenuOptionText2(this._context, this._virtualIndex).ToString();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Refresh();
		}

		private void Refresh()
		{
			switch (this._gameMenuManager.GetVirtualMenuAndOptionType(this._context))
			{
			case GameMenu.MenuAndOptionType.WaitMenuShowProgressAndHoursOption:
			{
				float virtualMenuTargetWaitHours = Campaign.Current.GameMenuManager.GetVirtualMenuTargetWaitHours(this._context);
				if (virtualMenuTargetWaitHours > 1f)
				{
					GameTexts.SetVariable("PLURAL_HOURS", 1);
				}
				else
				{
					GameTexts.SetVariable("PLURAL_HOURS", 0);
				}
				GameTexts.SetVariable("HOUR", MathF.Round(virtualMenuTargetWaitHours).ToString("0.0"));
				this.ProgressText = GameTexts.FindText("str_hours", null).ToString();
				goto IL_C3;
			}
			case GameMenu.MenuAndOptionType.WaitMenuShowOnlyProgressOption:
				this.ProgressText = "";
				goto IL_C3;
			}
			Debug.FailedAssert("Shouldn't create game menu progress for normal options", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\GameMenu\\GameMenuItemProgressVM.cs", "Refresh", 62);
			return;
			IL_C3:
			this.Text = (Campaign.Current.GameMenuManager.GetVirtualMenuIsWaitActive(this._context) ? this._text2 : this._text1);
			float virtualMenuProgress = Campaign.Current.GameMenuManager.GetVirtualMenuProgress(this._context);
			this.Progress = (float)MathF.Round(virtualMenuProgress * 100f);
		}

		public void OnTick()
		{
			this.Refresh();
		}

		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		[DataSourceProperty]
		public string ProgressText
		{
			get
			{
				return this._progressText;
			}
			set
			{
				if (value != this._progressText)
				{
					this._progressText = value;
					base.OnPropertyChangedWithValue<string>(value, "ProgressText");
				}
			}
		}

		[DataSourceProperty]
		public float Progress
		{
			get
			{
				return this._progress;
			}
			set
			{
				if (value != this._progress)
				{
					this._progress = value;
					base.OnPropertyChangedWithValue(value, "Progress");
				}
			}
		}

		private readonly MenuContext _context;

		private readonly GameMenuManager _gameMenuManager;

		private readonly int _virtualIndex;

		private string _text1 = "";

		private string _text2 = "";

		private string _text;

		private string _progressText;

		private float _progress;
	}
}
