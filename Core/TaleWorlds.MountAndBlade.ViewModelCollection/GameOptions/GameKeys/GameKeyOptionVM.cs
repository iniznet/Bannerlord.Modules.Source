using System;
using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys
{
	// Token: 0x02000102 RID: 258
	public class GameKeyOptionVM : KeyOptionVM
	{
		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x060016D1 RID: 5841 RVA: 0x0004A26C File Offset: 0x0004846C
		// (set) Token: 0x060016D2 RID: 5842 RVA: 0x0004A274 File Offset: 0x00048474
		public GameKey CurrentGameKey { get; private set; }

		// Token: 0x060016D3 RID: 5843 RVA: 0x0004A280 File Offset: 0x00048480
		public GameKeyOptionVM(GameKey gameKey, Action<KeyOptionVM> onKeybindRequest, Action<GameKeyOptionVM, InputKey> onKeySet)
			: base(gameKey.GroupId, ((GameKeyDefinition)gameKey.Id).ToString(), onKeybindRequest)
		{
			this._onKeySet = onKeySet;
			this.CurrentGameKey = gameKey;
			base.Key = (Input.IsGamepadActive ? this.CurrentGameKey.ControllerKey : this.CurrentGameKey.KeyboardKey);
			base.CurrentKey = new Key(base.Key.InputKey);
			this._initalKey = base.Key.InputKey;
			this.RefreshValues();
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x0004A310 File Offset: 0x00048510
		public override void RefreshValues()
		{
			base.RefreshValues();
			base.Name = Module.CurrentModule.GlobalTextManager.FindText("str_key_name", this._groupId + "_" + this._id).ToString();
			base.Description = Module.CurrentModule.GlobalTextManager.FindText("str_key_description", this._groupId + "_" + this._id).ToString();
			base.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", base.CurrentKey.ToString().ToLower()).ToString();
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x0004A3BC File Offset: 0x000485BC
		private void ExecuteKeybindRequest()
		{
			this._onKeybindRequest(this);
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x0004A3CA File Offset: 0x000485CA
		public override void Set(InputKey newKey)
		{
			this._onKeySet(this, newKey);
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x0004A3DC File Offset: 0x000485DC
		public override void Update()
		{
			base.Key = (Input.IsGamepadActive ? this.CurrentGameKey.ControllerKey : this.CurrentGameKey.KeyboardKey);
			Key key = base.Key;
			base.CurrentKey = new Key((key != null) ? key.InputKey : InputKey.Invalid);
			base.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", base.CurrentKey.ToString().ToLower()).ToString();
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x0004A45A File Offset: 0x0004865A
		public override void OnDone()
		{
			Key key = base.Key;
			if (key != null)
			{
				key.ChangeKey(base.CurrentKey.InputKey);
			}
			this._initalKey = base.CurrentKey.InputKey;
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x0004A489 File Offset: 0x00048689
		internal override bool IsChanged()
		{
			return base.CurrentKey.InputKey != this._initalKey;
		}

		// Token: 0x060016DA RID: 5850 RVA: 0x0004A4A4 File Offset: 0x000486A4
		internal override void OnGamepadActiveStateChanged(GamepadActiveStateChangedEvent obj)
		{
			base.Key = (Input.IsGamepadActive ? this.CurrentGameKey.ControllerKey : this.CurrentGameKey.KeyboardKey);
			base.CurrentKey = new Key(base.Key.InputKey);
			this._initalKey = base.Key.InputKey;
			this.RefreshValues();
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x0004A503 File Offset: 0x00048703
		public void Revert()
		{
			this.Set(this._initalKey);
			this.Update();
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x0004A517 File Offset: 0x00048717
		public void Apply()
		{
			this.OnDone();
			base.CurrentKey = base.Key;
		}

		// Token: 0x04000AD3 RID: 2771
		private InputKey _initalKey;

		// Token: 0x04000AD5 RID: 2773
		private readonly Action<GameKeyOptionVM, InputKey> _onKeySet;
	}
}
