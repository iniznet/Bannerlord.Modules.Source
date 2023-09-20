using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	public abstract class GameKeyContext
	{
		public string GameKeyCategoryId { get; private set; }

		public GameKeyContext.GameKeyContextType Type { get; private set; }

		public MBReadOnlyList<GameKey> RegisteredGameKeys
		{
			get
			{
				return this._registeredGameKeys;
			}
		}

		public Dictionary<string, HotKey>.ValueCollection RegisteredHotKeys
		{
			get
			{
				return this._registeredHotKeys.Values;
			}
		}

		public Dictionary<string, GameAxisKey>.ValueCollection RegisteredGameAxisKeys
		{
			get
			{
				return this._registeredAxisKeys.Values;
			}
		}

		protected GameKeyContext(string id, int gameKeysCount, GameKeyContext.GameKeyContextType type = GameKeyContext.GameKeyContextType.Default)
		{
			this.GameKeyCategoryId = id;
			this.Type = type;
			this._registeredHotKeys = new Dictionary<string, HotKey>();
			this._registeredAxisKeys = new Dictionary<string, GameAxisKey>();
			this._registeredGameKeys = new MBList<GameKey>(gameKeysCount);
			for (int i = 0; i < gameKeysCount; i++)
			{
				this._registeredGameKeys.Add(null);
			}
		}

		protected internal void RegisterHotKey(HotKey gameKey, bool addIfMissing = true)
		{
			if (GameKeyContext._isRDownSwappedWithRRight)
			{
				for (int i = 0; i < gameKey.Keys.Count; i++)
				{
					Key key = gameKey.Keys[i];
					if (key != null && key.InputKey == InputKey.ControllerRDown)
					{
						key.ChangeKey(InputKey.ControllerRRight);
					}
					else if (key != null && key.InputKey == InputKey.ControllerRRight)
					{
						key.ChangeKey(InputKey.ControllerRDown);
					}
				}
			}
			if (this._registeredHotKeys.ContainsKey(gameKey.Id))
			{
				this._registeredHotKeys[gameKey.Id] = gameKey;
				return;
			}
			if (addIfMissing)
			{
				this._registeredHotKeys.Add(gameKey.Id, gameKey);
			}
		}

		protected internal void RegisterGameKey(GameKey gameKey, bool addIfMissing = true)
		{
			if (GameKeyContext._isRDownSwappedWithRRight)
			{
				Key controllerKey = gameKey.ControllerKey;
				if (controllerKey != null && controllerKey.InputKey == InputKey.ControllerRDown)
				{
					controllerKey.ChangeKey(InputKey.ControllerRRight);
				}
				else if (controllerKey != null && controllerKey.InputKey == InputKey.ControllerRRight)
				{
					controllerKey.ChangeKey(InputKey.ControllerRDown);
				}
			}
			this._registeredGameKeys[gameKey.Id] = gameKey;
		}

		protected internal void RegisterGameAxisKey(GameAxisKey gameKey, bool addIfMissing = true)
		{
			if (this._registeredAxisKeys.ContainsKey(gameKey.Id))
			{
				this._registeredAxisKeys[gameKey.Id] = gameKey;
				return;
			}
			if (addIfMissing)
			{
				this._registeredAxisKeys.Add(gameKey.Id, gameKey);
			}
		}

		internal static void SetIsRDownSwappedWithRRight(bool value)
		{
			GameKeyContext._isRDownSwappedWithRRight = value;
		}

		public HotKey GetHotKey(string hotKeyId)
		{
			HotKey hotKey = null;
			this._registeredHotKeys.TryGetValue(hotKeyId, out hotKey);
			return hotKey;
		}

		public GameKey GetGameKey(int gameKeyId)
		{
			for (int i = 0; i < this._registeredGameKeys.Count; i++)
			{
				GameKey gameKey = this._registeredGameKeys[i];
				if (gameKey != null && gameKey.Id == gameKeyId)
				{
					return gameKey;
				}
			}
			Debug.FailedAssert(string.Format("Couldn't find {0} in {1}", gameKeyId, this.GameKeyCategoryId), "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.InputSystem\\GameKeyContext.cs", "GetGameKey", 125);
			return null;
		}

		public GameKey GetGameKey(string gameKeyId)
		{
			for (int i = 0; i < this._registeredGameKeys.Count; i++)
			{
				GameKey gameKey = this._registeredGameKeys[i];
				if (gameKey != null && gameKey.StringId == gameKeyId)
				{
					return gameKey;
				}
			}
			Debug.FailedAssert("Couldn't find " + gameKeyId + " in " + this.GameKeyCategoryId, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.InputSystem\\GameKeyContext.cs", "GetGameKey", 140);
			return null;
		}

		internal GameAxisKey GetGameAxisKey(string axisKeyId)
		{
			GameAxisKey gameAxisKey;
			this._registeredAxisKeys.TryGetValue(axisKeyId, out gameAxisKey);
			return gameAxisKey;
		}

		public string GetHotKeyId(string hotKeyId)
		{
			HotKey hotKey;
			if (this._registeredHotKeys.TryGetValue(hotKeyId, out hotKey))
			{
				return hotKey.ToString();
			}
			GameAxisKey gameAxisKey;
			if (this._registeredAxisKeys.TryGetValue(hotKeyId, out gameAxisKey))
			{
				return gameAxisKey.ToString();
			}
			Debug.FailedAssert("HotKey with id: " + hotKeyId + " is not registered.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.InputSystem\\GameKeyContext.cs", "GetHotKeyId", 163);
			return "";
		}

		public string GetHotKeyId(int gameKeyId)
		{
			GameKey gameKey = this._registeredGameKeys[gameKeyId];
			if (gameKey != null)
			{
				return gameKey.ToString();
			}
			Debug.FailedAssert("GameKey with id: " + gameKeyId + " is not registered.", "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\GauntletUI\\TaleWorlds.InputSystem\\GameKeyContext.cs", "GetHotKeyId", 175);
			return "";
		}

		private readonly Dictionary<string, HotKey> _registeredHotKeys;

		private readonly MBList<GameKey> _registeredGameKeys;

		private readonly Dictionary<string, GameAxisKey> _registeredAxisKeys;

		private static bool _isRDownSwappedWithRRight = true;

		public enum GameKeyContextType
		{
			Default,
			AuxiliaryNotSerialized,
			AuxiliarySerialized,
			AuxiliarySerializedAndShownInOptions
		}
	}
}
