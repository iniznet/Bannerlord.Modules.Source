using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GameMenu
{
	public class SettlementMenuPartyCharacterListsButtonWidget : ButtonWidget
	{
		public Brush PartyListButtonBrush { get; set; }

		public Brush CharacterListButtonBrush { get; set; }

		public ContainerPageControlWidget CharactersList { get; set; }

		public ContainerPageControlWidget PartiesList { get; set; }

		public int MaxNumOfVisuals { get; set; } = 5;

		public SettlementMenuPartyCharacterListsButtonWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			base.Brush = (this.ChildPartiesList.IsVisible ? this.PartyListButtonBrush : (this.ChildCharactersList.IsVisible ? this.CharacterListButtonBrush : null));
			if (!this._initialized)
			{
				if (this.CharactersList.IsVisible)
				{
					this.SetCharacterListVisible();
				}
				else if (this.PartiesList.IsVisible)
				{
					this.SetPartyListVisible();
				}
				this._initialized = true;
			}
		}

		protected override void OnClick()
		{
			base.OnClick();
			if (!this.PartiesList.IsVisible && this.CharactersList.IsVisible)
			{
				this.SetPartyListVisible();
				return;
			}
			if (this.PartiesList.IsVisible && !this.CharactersList.IsVisible)
			{
				this.SetCharacterListVisible();
			}
		}

		private void SetCharacterListVisible()
		{
			this.CharactersList.IsVisible = true;
			this.PartiesList.IsVisible = false;
			this.ChildPartiesList.IsVisible = true;
			this.ChildCharactersList.IsVisible = false;
		}

		private void SetPartyListVisible()
		{
			this.CharactersList.IsVisible = false;
			this.PartiesList.IsVisible = true;
			this.ChildPartiesList.IsVisible = false;
			this.ChildCharactersList.IsVisible = true;
		}

		public ListPanel ChildCharactersList
		{
			get
			{
				return this._childCharactersList;
			}
			set
			{
				if (value != this._childCharactersList)
				{
					this._childCharactersList = value;
					this._childCharactersList.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnListItemAdded));
				}
			}
		}

		public ListPanel ChildPartiesList
		{
			get
			{
				return this._childPartiesList;
			}
			set
			{
				if (value != this._childPartiesList)
				{
					this._childPartiesList = value;
					this._childPartiesList.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnListItemAdded));
				}
			}
		}

		private void OnListItemAdded(Widget parent, Widget child)
		{
			if (parent.ChildCount > this.MaxNumOfVisuals)
			{
				child.IsVisible = false;
			}
		}

		private bool _initialized;

		private ListPanel _childCharactersList;

		private ListPanel _childPartiesList;
	}
}
