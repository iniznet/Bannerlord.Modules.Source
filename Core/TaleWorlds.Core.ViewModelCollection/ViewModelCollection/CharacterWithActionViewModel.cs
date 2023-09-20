using System;

namespace TaleWorlds.Core.ViewModelCollection
{
	public class CharacterWithActionViewModel : CharacterViewModel
	{
		public CharacterWithActionViewModel(Action onAction)
		{
			this._onAction = onAction;
		}

		private void ExecuteAction()
		{
			Action onAction = this._onAction;
			if (onAction == null)
			{
				return;
			}
			onAction();
		}

		private Action _onAction;
	}
}
