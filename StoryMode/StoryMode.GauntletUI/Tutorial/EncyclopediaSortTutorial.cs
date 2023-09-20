using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000031 RID: 49
	public class EncyclopediaSortTutorial : TutorialItemBase
	{
		// Token: 0x060000F0 RID: 240 RVA: 0x00003FD7 File Offset: 0x000021D7
		public EncyclopediaSortTutorial()
		{
			base.Type = "EncyclopediaSortTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "EncyclopediaSortButton";
			base.MouseRequired = false;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00004003 File Offset: 0x00002203
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00004008 File Offset: 0x00002208
		public override bool IsConditionsMetForActivation()
		{
			bool isActive = this._isActive;
			EncyclopediaPages currentEncyclopediaPage = TutorialHelper.CurrentEncyclopediaPage;
			if (currentEncyclopediaPage - 2 <= 5)
			{
				this._isActive = true;
			}
			else
			{
				this._isActive = false;
			}
			if (!isActive && this._isActive)
			{
				Game.Current.EventManager.RegisterEvent<OnEncyclopediaListSortedEvent>(new Action<OnEncyclopediaListSortedEvent>(this.OnSortClicked));
			}
			else if (!this._isActive && isActive)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaListSortedEvent>(new Action<OnEncyclopediaListSortedEvent>(this.OnSortClicked));
			}
			return this._isActive;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000408E File Offset: 0x0000228E
		private void OnSortClicked(OnEncyclopediaListSortedEvent evnt)
		{
			this._isSortClicked = true;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00004097 File Offset: 0x00002297
		public override bool IsConditionsMetForCompletion()
		{
			if (this._isActive && this._isSortClicked)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaListSortedEvent>(new Action<OnEncyclopediaListSortedEvent>(this.OnSortClicked));
				return true;
			}
			return false;
		}

		// Token: 0x04000047 RID: 71
		private bool _isActive;

		// Token: 0x04000048 RID: 72
		private bool _isSortClicked;
	}
}
