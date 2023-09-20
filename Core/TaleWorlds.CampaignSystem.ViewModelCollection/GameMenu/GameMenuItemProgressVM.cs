using System;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu
{
	// Token: 0x02000085 RID: 133
	public class GameMenuItemProgressVM : ViewModel
	{
		// Token: 0x06000D29 RID: 3369 RVA: 0x00035664 File Offset: 0x00033864
		public GameMenuItemProgressVM(MenuContext context, int virtualIndex)
		{
			this._context = context;
			this._gameMenuManager = Campaign.Current.GameMenuManager;
			this._virtualIndex = virtualIndex;
			this._text1 = Campaign.Current.GameMenuManager.GetVirtualMenuOptionText(this._context, this._virtualIndex).ToString();
			this._text2 = Campaign.Current.GameMenuManager.GetVirtualMenuOptionText2(this._context, this._virtualIndex).ToString();
			this.RefreshValues();
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x000356FD File Offset: 0x000338FD
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Refresh();
		}

		// Token: 0x06000D2B RID: 3371 RVA: 0x0003570C File Offset: 0x0003390C
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

		// Token: 0x06000D2C RID: 3372 RVA: 0x00035832 File Offset: 0x00033A32
		public void OnTick()
		{
			this.Refresh();
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x06000D2D RID: 3373 RVA: 0x0003583A File Offset: 0x00033A3A
		// (set) Token: 0x06000D2E RID: 3374 RVA: 0x00035842 File Offset: 0x00033A42
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

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x06000D2F RID: 3375 RVA: 0x00035865 File Offset: 0x00033A65
		// (set) Token: 0x06000D30 RID: 3376 RVA: 0x0003586D File Offset: 0x00033A6D
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

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x06000D31 RID: 3377 RVA: 0x00035890 File Offset: 0x00033A90
		// (set) Token: 0x06000D32 RID: 3378 RVA: 0x00035898 File Offset: 0x00033A98
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

		// Token: 0x0400060B RID: 1547
		private readonly MenuContext _context;

		// Token: 0x0400060C RID: 1548
		private readonly GameMenuManager _gameMenuManager;

		// Token: 0x0400060D RID: 1549
		private readonly int _virtualIndex;

		// Token: 0x0400060E RID: 1550
		private string _text1 = "";

		// Token: 0x0400060F RID: 1551
		private string _text2 = "";

		// Token: 0x04000610 RID: 1552
		private string _text;

		// Token: 0x04000611 RID: 1553
		private string _progressText;

		// Token: 0x04000612 RID: 1554
		private float _progress;
	}
}
