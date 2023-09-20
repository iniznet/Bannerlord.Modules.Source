using System;
using StoryMode.ViewModelCollection.Tutorial;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;

namespace StoryMode.GauntletUI.Tutorial
{
	// Token: 0x0200003A RID: 58
	public class AssignRolesTutorial : TutorialItemBase
	{
		// Token: 0x06000123 RID: 291 RVA: 0x0000454B File Offset: 0x0000274B
		public AssignRolesTutorial()
		{
			base.Type = "AssignRolesTutorial";
			base.Placement = TutorialItemVM.ItemPlacements.Top;
			base.HighlightedVisualElementID = "RoleAssignmentWidget";
			base.MouseRequired = false;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00004577 File Offset: 0x00002777
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 6;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000457A File Offset: 0x0000277A
		public override void OnClanRoleAssignedThroughClanScreen(ClanRoleAssignedThroughClanScreenEvent obj)
		{
			this._playerAssignedRoleToClanMember = true;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00004583 File Offset: 0x00002783
		public override bool IsConditionsMetForActivation()
		{
			return TutorialHelper.PlayerHasUnassignedRolesAndMember;
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000458A File Offset: 0x0000278A
		public override bool IsConditionsMetForCompletion()
		{
			return this._playerAssignedRoleToClanMember;
		}

		// Token: 0x04000059 RID: 89
		private bool _playerAssignedRoleToClanMember;
	}
}
