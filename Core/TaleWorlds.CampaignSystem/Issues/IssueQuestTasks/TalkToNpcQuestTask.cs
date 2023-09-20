using System;

namespace TaleWorlds.CampaignSystem.Issues.IssueQuestTasks
{
	public class TalkToNpcQuestTask : QuestTaskBase
	{
		public TalkToNpcQuestTask(Hero hero, Action onSucceededAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, null, null)
		{
			this._character = hero.CharacterObject;
		}

		public TalkToNpcQuestTask(CharacterObject character, Action onSucceededAction, DialogFlow dialogFlow = null)
			: base(dialogFlow, onSucceededAction, null, null)
		{
			this._character = character;
		}

		public bool IsTaskCharacter()
		{
			return this._character == CharacterObject.OneToOneConversationCharacter;
		}

		protected override void OnFinished()
		{
			this._character = null;
		}

		public override void SetReferences()
		{
		}

		private CharacterObject _character;
	}
}
