using System;

namespace SandBox.ViewModelCollection.Map.Cheat
{
	public class CheatActionItemVM : CheatItemBaseVM
	{
		public CheatActionItemVM(GameplayCheatItem cheat, Action<CheatActionItemVM> onCheatExecuted)
		{
			this._onCheatExecuted = onCheatExecuted;
			this.Cheat = cheat;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			GameplayCheatItem cheat = this.Cheat;
			base.Name = ((cheat != null) ? cheat.GetName().ToString() : null);
		}

		public override void ExecuteAction()
		{
			GameplayCheatItem cheat = this.Cheat;
			if (cheat != null)
			{
				cheat.ExecuteCheat();
			}
			Action<CheatActionItemVM> onCheatExecuted = this._onCheatExecuted;
			if (onCheatExecuted == null)
			{
				return;
			}
			onCheatExecuted(this);
		}

		public readonly GameplayCheatItem Cheat;

		private readonly Action<CheatActionItemVM> _onCheatExecuted;
	}
}
