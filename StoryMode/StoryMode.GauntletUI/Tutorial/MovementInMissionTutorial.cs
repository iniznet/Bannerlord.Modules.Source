using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x02000016 RID: 22
	public class MovementInMissionTutorial : TutorialItemBase
	{
		// Token: 0x06000066 RID: 102 RVA: 0x00002B4E File Offset: 0x00000D4E
		public MovementInMissionTutorial()
		{
			base.Type = "MovementInMissionTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Right;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00002B7A File Offset: 0x00000D7A
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerMovedBackward && this._playerMovedLeft && this._playerMovedRight && this._playerMovedForward;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00002B9C File Offset: 0x00000D9C
		public override void OnPlayerMovementFlagChanged(MissionPlayerMovementFlagsChangeEvent obj)
		{
			base.OnPlayerMovementFlagChanged(obj);
			this._playerMovedRight = this._playerMovedRight || (obj.MovementFlag & 4) == 4;
			this._playerMovedLeft = this._playerMovedLeft || (obj.MovementFlag & 8) == 8;
			this._playerMovedForward = this._playerMovedForward || (obj.MovementFlag & 1) == 1;
			this._playerMovedBackward = this._playerMovedBackward || (obj.MovementFlag & 2) == 2;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00002C20 File Offset: 0x00000E20
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00002C23 File Offset: 0x00000E23
		public override bool IsConditionsMetForActivation()
		{
			return Mission.Current != null && Mission.Current.Mode != 6 && !TutorialHelper.PlayerIsInAConversation && TutorialHelper.CurrentContext == 8;
		}

		// Token: 0x04000017 RID: 23
		private bool _playerMovedForward;

		// Token: 0x04000018 RID: 24
		private bool _playerMovedBackward;

		// Token: 0x04000019 RID: 25
		private bool _playerMovedLeft;

		// Token: 0x0400001A RID: 26
		private bool _playerMovedRight;
	}
}
