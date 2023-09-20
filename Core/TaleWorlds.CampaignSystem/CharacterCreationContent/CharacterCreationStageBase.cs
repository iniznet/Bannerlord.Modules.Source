using System;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	public abstract class CharacterCreationStageBase
	{
		public CharacterCreationState CharacterCreationState { get; }

		public ICharacterCreationStageListener Listener { get; set; }

		protected CharacterCreationStageBase(CharacterCreationState state)
		{
			this.CharacterCreationState = state;
		}

		protected internal virtual void OnFinalize()
		{
			ICharacterCreationStageListener listener = this.Listener;
			if (listener == null)
			{
				return;
			}
			listener.OnStageFinalize();
		}
	}
}
