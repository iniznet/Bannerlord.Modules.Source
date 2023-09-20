using System;

namespace TaleWorlds.CampaignSystem.Issues.IssueQuestTasks
{
	// Token: 0x02000329 RID: 809
	public class TalkToNpcQuestTask : QuestTaskBase
	{
		// Token: 0x06002DDA RID: 11738 RVA: 0x000BF7F7 File Offset: 0x000BD9F7
		public TalkToNpcQuestTask(Hero hero, Action onSucceededAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, null, null)
		{
			this._character = hero.CharacterObject;
		}

		// Token: 0x06002DDB RID: 11739 RVA: 0x000BF80F File Offset: 0x000BDA0F
		public TalkToNpcQuestTask(CharacterObject character, Action onSucceededAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, null, null)
		{
			this._character = character;
		}

		// Token: 0x06002DDC RID: 11740 RVA: 0x000BF822 File Offset: 0x000BDA22
		public bool IsTaskCharacter()
		{
			return this._character == CharacterObject.OneToOneConversationCharacter;
		}

		// Token: 0x06002DDD RID: 11741 RVA: 0x000BF831 File Offset: 0x000BDA31
		protected override void OnFinished()
		{
			this._character = null;
		}

		// Token: 0x06002DDE RID: 11742 RVA: 0x000BF83A File Offset: 0x000BDA3A
		public override void SetReferences()
		{
		}

		// Token: 0x04000DD3 RID: 3539
		private CharacterObject _character;
	}
}
