using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.InputSystem;

namespace TaleWorlds.Engine.GauntletUI
{
	// Token: 0x02000002 RID: 2
	public class EngineInputService : IInputService
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public EngineInputService(IInputContext inputContext)
		{
			this._inputContext = inputContext;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		bool IInputService.MouseEnabled
		{
			get
			{
				return this._mouseEnabled;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000205F File Offset: 0x0000025F
		bool IInputService.KeyboardEnabled
		{
			get
			{
				return this._keyboardEnabled;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002067 File Offset: 0x00000267
		bool IInputService.GamepadEnabled
		{
			get
			{
				return this._gamepadEnabled;
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000206F File Offset: 0x0000026F
		public void UpdateInputDevices(bool keyboardEnabled, bool mouseEnabled, bool gamepadEnabled)
		{
			this._mouseEnabled = mouseEnabled;
			this._keyboardEnabled = keyboardEnabled;
			this._gamepadEnabled = gamepadEnabled;
		}

		// Token: 0x04000001 RID: 1
		private IInputContext _inputContext;

		// Token: 0x04000002 RID: 2
		private bool _mouseEnabled;

		// Token: 0x04000003 RID: 3
		private bool _keyboardEnabled;

		// Token: 0x04000004 RID: 4
		private bool _gamepadEnabled;
	}
}
