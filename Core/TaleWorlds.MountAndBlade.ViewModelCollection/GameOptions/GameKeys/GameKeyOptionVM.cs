using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys
{
	public class GameKeyOptionVM : KeyOptionVM
	{
		public GameKey CurrentGameKey { get; private set; }

		public GameKeyOptionVM(GameKey gameKey, Action<KeyOptionVM> onKeybindRequest, Action<GameKeyOptionVM, InputKey> onKeySet)
			: base(gameKey.GroupId, ((GameKeyDefinition)gameKey.Id).ToString(), onKeybindRequest)
		{
			this._onKeySet = onKeySet;
			this.CurrentGameKey = gameKey;
			base.Key = (Input.IsGamepadActive ? this.CurrentGameKey.ControllerKey : this.CurrentGameKey.KeyboardKey);
			if (base.Key == null)
			{
				base.Key = new Key(InputKey.Invalid);
			}
			base.CurrentKey = new Key(base.Key.InputKey);
			this._initalKey = base.Key.InputKey;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			base.Name = Module.CurrentModule.GlobalTextManager.FindText("str_key_name", this._groupId + "_" + this._id).ToString();
			base.Description = Module.CurrentModule.GlobalTextManager.FindText("str_key_description", this._groupId + "_" + this._id).ToString();
			base.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", base.CurrentKey.ToString().ToLower()).ToString();
		}

		private void ExecuteKeybindRequest()
		{
			this._onKeybindRequest(this);
		}

		public override void Set(InputKey newKey)
		{
			this._onKeySet(this, newKey);
		}

		public override void Update()
		{
			base.Key = (Input.IsGamepadActive ? this.CurrentGameKey.ControllerKey : this.CurrentGameKey.KeyboardKey);
			if (base.Key == null)
			{
				base.Key = new Key(InputKey.Invalid);
			}
			base.CurrentKey = new Key(base.Key.InputKey);
			base.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", base.CurrentKey.ToString().ToLower()).ToString();
		}

		public override void OnDone()
		{
			Key key = base.Key;
			if (key != null)
			{
				key.ChangeKey(base.CurrentKey.InputKey);
			}
			this._initalKey = base.CurrentKey.InputKey;
		}

		internal override bool IsChanged()
		{
			return base.CurrentKey.InputKey != this._initalKey;
		}

		public void Revert()
		{
			this.Set(this._initalKey);
			this.Update();
		}

		public void Apply()
		{
			this.OnDone();
			base.CurrentKey = base.Key;
		}

		private InputKey _initalKey;

		private readonly Action<GameKeyOptionVM, InputKey> _onKeySet;
	}
}
