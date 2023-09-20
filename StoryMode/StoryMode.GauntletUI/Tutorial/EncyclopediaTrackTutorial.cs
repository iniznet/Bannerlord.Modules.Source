using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200002E RID: 46
	public class EncyclopediaTrackTutorial : TutorialItemBase
	{
		// Token: 0x060000E1 RID: 225 RVA: 0x00003CE3 File Offset: 0x00001EE3
		public EncyclopediaTrackTutorial()
		{
			base.Type = "EncyclopediaTrackTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = "EncyclopediaItemTrackButton";
			base.MouseRequired = false;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00003D0F File Offset: 0x00001F0F
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 9;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00003D14 File Offset: 0x00001F14
		public override bool IsConditionsMetForActivation()
		{
			bool isActive = this._isActive;
			this._isActive = TutorialHelper.CurrentEncyclopediaPage == 9;
			if (!isActive && this._isActive)
			{
				Game.Current.EventManager.RegisterEvent<PlayerToggleTrackSettlementFromEncyclopediaEvent>(new Action<PlayerToggleTrackSettlementFromEncyclopediaEvent>(this.OnTrackToggledFromEncyclopedia));
			}
			else if (!this._isActive && isActive)
			{
				Game.Current.EventManager.UnregisterEvent<PlayerToggleTrackSettlementFromEncyclopediaEvent>(new Action<PlayerToggleTrackSettlementFromEncyclopediaEvent>(this.OnTrackToggledFromEncyclopedia));
			}
			return this._isActive;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00003D90 File Offset: 0x00001F90
		public override bool IsConditionsMetForCompletion()
		{
			if (this._isActive)
			{
				bool flag = false;
				if (this._isActive)
				{
					if (TutorialHelper.CurrentContext != 9)
					{
						flag = true;
					}
					if (TutorialHelper.CurrentEncyclopediaPage != 12 && TutorialHelper.CurrentEncyclopediaPage != 9)
					{
						flag = true;
					}
					if (this._usedTrackFromEncyclopedia)
					{
						flag = true;
					}
				}
				if (flag)
				{
					Game.Current.EventManager.UnregisterEvent<PlayerToggleTrackSettlementFromEncyclopediaEvent>(new Action<PlayerToggleTrackSettlementFromEncyclopediaEvent>(this.OnTrackToggledFromEncyclopedia));
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00003DF9 File Offset: 0x00001FF9
		private void OnTrackToggledFromEncyclopedia(PlayerToggleTrackSettlementFromEncyclopediaEvent callback)
		{
			this._usedTrackFromEncyclopedia = true;
		}

		// Token: 0x04000041 RID: 65
		private bool _isActive;

		// Token: 0x04000042 RID: 66
		private bool _usedTrackFromEncyclopedia;
	}
}
