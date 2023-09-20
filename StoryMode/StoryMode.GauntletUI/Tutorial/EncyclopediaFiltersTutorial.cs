using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000030 RID: 48
	public class EncyclopediaFiltersTutorial : TutorialItemBase
	{
		// Token: 0x060000EB RID: 235 RVA: 0x00003EE6 File Offset: 0x000020E6
		public EncyclopediaFiltersTutorial()
		{
			base.Type = "EncyclopediaFiltersTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "EncyclopediaFiltersContainer";
			base.MouseRequired = false;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00003F12 File Offset: 0x00002112
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00003F18 File Offset: 0x00002118
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
				Game.Current.EventManager.RegisterEvent<OnEncyclopediaFilterActivatedEvent>(new Action<OnEncyclopediaFilterActivatedEvent>(this.OnFilterClicked));
			}
			else if (!this._isActive && isActive)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaFilterActivatedEvent>(new Action<OnEncyclopediaFilterActivatedEvent>(this.OnFilterClicked));
			}
			return this._isActive;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00003F9E File Offset: 0x0000219E
		private void OnFilterClicked(OnEncyclopediaFilterActivatedEvent evnt)
		{
			this._isAnyFilterSelected = true;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00003FA7 File Offset: 0x000021A7
		public override bool IsConditionsMetForCompletion()
		{
			if (this._isActive && this._isAnyFilterSelected)
			{
				Game.Current.EventManager.UnregisterEvent<OnEncyclopediaFilterActivatedEvent>(new Action<OnEncyclopediaFilterActivatedEvent>(this.OnFilterClicked));
				return true;
			}
			return false;
		}

		// Token: 0x04000045 RID: 69
		private bool _isActive;

		// Token: 0x04000046 RID: 70
		private bool _isAnyFilterSelected;
	}
}
