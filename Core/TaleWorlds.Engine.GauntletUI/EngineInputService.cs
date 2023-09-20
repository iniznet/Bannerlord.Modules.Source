using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.InputSystem;

namespace TaleWorlds.Engine.GauntletUI
{
	public class EngineInputService : IInputService
	{
		public EngineInputService(IInputContext inputContext)
		{
			this._inputContext = inputContext;
		}

		bool IInputService.MouseEnabled
		{
			get
			{
				return this._mouseEnabled;
			}
		}

		bool IInputService.KeyboardEnabled
		{
			get
			{
				return this._keyboardEnabled;
			}
		}

		bool IInputService.GamepadEnabled
		{
			get
			{
				return this._gamepadEnabled;
			}
		}

		public void UpdateInputDevices(bool keyboardEnabled, bool mouseEnabled, bool gamepadEnabled)
		{
			this._mouseEnabled = mouseEnabled;
			this._keyboardEnabled = keyboardEnabled;
			this._gamepadEnabled = gamepadEnabled;
		}

		private IInputContext _inputContext;

		private bool _mouseEnabled;

		private bool _keyboardEnabled;

		private bool _gamepadEnabled;
	}
}
