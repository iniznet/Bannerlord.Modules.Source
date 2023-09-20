using System;

namespace TaleWorlds.Core.ViewModelCollection
{
	// Token: 0x0200000A RID: 10
	public class CharacterWithActionViewModel : CharacterViewModel
	{
		// Token: 0x06000053 RID: 83 RVA: 0x0000281C File Offset: 0x00000A1C
		public CharacterWithActionViewModel(Action onAction)
		{
			this._onAction = onAction;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x0000282B File Offset: 0x00000A2B
		private void ExecuteAction()
		{
			Action onAction = this._onAction;
			if (onAction == null)
			{
				return;
			}
			onAction();
		}

		// Token: 0x04000018 RID: 24
		private Action _onAction;
	}
}
