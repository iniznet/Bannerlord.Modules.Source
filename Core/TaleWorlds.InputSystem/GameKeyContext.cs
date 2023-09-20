using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.InputSystem
{
	// Token: 0x02000005 RID: 5
	public abstract class GameKeyContext
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000050 RID: 80 RVA: 0x000028AE File Offset: 0x00000AAE
		// (set) Token: 0x06000051 RID: 81 RVA: 0x000028B6 File Offset: 0x00000AB6
		public string GameKeyCategoryId { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000052 RID: 82 RVA: 0x000028BF File Offset: 0x00000ABF
		// (set) Token: 0x06000053 RID: 83 RVA: 0x000028C7 File Offset: 0x00000AC7
		public GameKeyContext.GameKeyContextType Type { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000054 RID: 84 RVA: 0x000028D0 File Offset: 0x00000AD0
		public MBReadOnlyList<GameKey> RegisteredGameKeys
		{
			get
			{
				return this._registeredGameKeys;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000028D8 File Offset: 0x00000AD8
		public Dictionary<string, HotKey>.ValueCollection RegisteredHotKeys
		{
			get
			{
				return this._registeredHotKeys.Values;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000056 RID: 86 RVA: 0x000028E5 File Offset: 0x00000AE5
		public Dictionary<string, GameAxisKey>.ValueCollection RegisteredGameAxisKeys
		{
			get
			{
				return this._registeredAxisKeys.Values;
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000028F4 File Offset: 0x00000AF4
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

		// Token: 0x06000058 RID: 88 RVA: 0x00002950 File Offset: 0x00000B50
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

		// Token: 0x06000059 RID: 89 RVA: 0x000029FC File Offset: 0x00000BFC
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

		// Token: 0x0600005A RID: 90 RVA: 0x00002A61 File Offset: 0x00000C61
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

		// Token: 0x0600005B RID: 91 RVA: 0x00002A9E File Offset: 0x00000C9E
		internal static void SetIsRDownSwappedWithRRight(bool value)
		{
			GameKeyContext._isRDownSwappedWithRRight = value;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002AA8 File Offset: 0x00000CA8
		public HotKey GetHotKey(string hotKeyId)
		{
			HotKey hotKey = null;
			this._registeredHotKeys.TryGetValue(hotKeyId, out hotKey);
			return hotKey;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00002AC8 File Offset: 0x00000CC8
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

		// Token: 0x0600005E RID: 94 RVA: 0x00002B30 File Offset: 0x00000D30
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

		// Token: 0x0600005F RID: 95 RVA: 0x00002BA0 File Offset: 0x00000DA0
		internal GameAxisKey GetGameAxisKey(string axisKeyId)
		{
			GameAxisKey gameAxisKey;
			this._registeredAxisKeys.TryGetValue(axisKeyId, out gameAxisKey);
			return gameAxisKey;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00002BC0 File Offset: 0x00000DC0
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

		// Token: 0x06000061 RID: 97 RVA: 0x00002C24 File Offset: 0x00000E24
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

		// Token: 0x04000012 RID: 18
		private readonly Dictionary<string, HotKey> _registeredHotKeys;

		// Token: 0x04000013 RID: 19
		private readonly MBList<GameKey> _registeredGameKeys;

		// Token: 0x04000014 RID: 20
		private readonly Dictionary<string, GameAxisKey> _registeredAxisKeys;

		// Token: 0x04000015 RID: 21
		private static bool _isRDownSwappedWithRRight = true;

		// Token: 0x02000012 RID: 18
		public enum GameKeyContextType
		{
			// Token: 0x04000151 RID: 337
			Default,
			// Token: 0x04000152 RID: 338
			AuxiliaryNotSerialized,
			// Token: 0x04000153 RID: 339
			AuxiliarySerialized,
			// Token: 0x04000154 RID: 340
			AuxiliarySerializedAndShownInOptions
		}
	}
}
