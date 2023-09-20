using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Nameplate.NameplateNotifications
{
	public class SettlementNotificationItemBaseVM : ViewModel
	{
		public int CreatedTick { get; set; }

		public SettlementNotificationItemBaseVM(Action<SettlementNotificationItemBaseVM> onRemove, int createdTick)
		{
			this._onRemove = onRemove;
			this.RelationType = 0;
			this.CreatedTick = createdTick;
		}

		public void ExecuteRemove()
		{
			this._onRemove(this);
		}

		public string CharacterName
		{
			get
			{
				return this._characterName;
			}
			set
			{
				if (value != this._characterName)
				{
					this._characterName = value;
					base.OnPropertyChangedWithValue<string>(value, "CharacterName");
				}
			}
		}

		public int RelationType
		{
			get
			{
				return this._relationType;
			}
			set
			{
				if (value != this._relationType)
				{
					this._relationType = value;
					base.OnPropertyChangedWithValue(value, "RelationType");
				}
			}
		}

		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		public ImageIdentifierVM CharacterVisual
		{
			get
			{
				return this._characterVisual;
			}
			set
			{
				if (value != this._characterVisual)
				{
					this._characterVisual = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "CharacterVisual");
				}
			}
		}

		private readonly Action<SettlementNotificationItemBaseVM> _onRemove;

		private ImageIdentifierVM _characterVisual;

		private string _text;

		private string _characterName;

		private int _relationType;
	}
}
