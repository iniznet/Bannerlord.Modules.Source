using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.AfterBattle
{
	public abstract class MPAfterBattleRewardItemVM : ViewModel
	{
		public int Type
		{
			get
			{
				return this._type;
			}
			set
			{
				if (value != this._type)
				{
					this._type = value;
					base.OnPropertyChangedWithValue(value, "Type");
				}
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		private int _type;

		private string _name;

		public enum RewardType
		{
			Loot,
			Badge
		}
	}
}
