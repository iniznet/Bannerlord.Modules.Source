using System;
using SandBox.View.Map;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000023 RID: 35
	public class NavigateOnMapTutorialStep1 : TutorialItemBase
	{
		// Token: 0x060000AA RID: 170 RVA: 0x0000346E File Offset: 0x0000166E
		public NavigateOnMapTutorialStep1()
		{
			base.Type = "NavigateOnMapTutorialStep1";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = true;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000034A5 File Offset: 0x000016A5
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 4;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000034A8 File Offset: 0x000016A8
		public override bool IsConditionsMetForActivation()
		{
			return !TutorialHelper.TownMenuIsOpen && !TutorialHelper.VillageMenuIsOpen && TutorialHelper.CurrentContext == 4;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000034C4 File Offset: 0x000016C4
		public override void OnMainMapCameraMove(MainMapCameraMoveEvent obj)
		{
			base.OnMainMapCameraMove(obj);
			this._movedPosition = obj.PositionChanged || this._movedPosition;
			this._movedRotation = obj.RotationChanged || this._movedRotation;
			if (this._movedRotation && this._movedPosition && this._completionTime == DateTime.MinValue)
			{
				this._completionTime = TutorialHelper.CurrentTime;
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00003534 File Offset: 0x00001734
		public override bool IsConditionsMetForCompletion()
		{
			return !(this._completionTime == DateTime.MinValue) && (TutorialHelper.CurrentTime - this._completionTime).TotalSeconds > 2.0;
		}

		// Token: 0x0400002D RID: 45
		private bool _movedPosition;

		// Token: 0x0400002E RID: 46
		private bool _movedRotation;

		// Token: 0x0400002F RID: 47
		private const float _delayInSeconds = 2f;

		// Token: 0x04000030 RID: 48
		private DateTime _completionTime = DateTime.MinValue;
	}
}
