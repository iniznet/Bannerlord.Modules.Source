using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000214 RID: 532
	public static class GameKeyTextExtensions
	{
		// Token: 0x06001DC3 RID: 7619 RVA: 0x0006B627 File Offset: 0x00069827
		public static TextObject GetHotKeyGameText(this GameTextManager gameTextManager, string categoryName, string hotKeyId)
		{
			return gameTextManager.GetHotKeyGameTextFromKeyID(HotKeyManager.GetHotKeyId(categoryName, hotKeyId));
		}

		// Token: 0x06001DC4 RID: 7620 RVA: 0x0006B636 File Offset: 0x00069836
		public static TextObject GetHotKeyGameText(this GameTextManager gameTextManager, string categoryName, int gameKeyId)
		{
			return gameTextManager.GetHotKeyGameTextFromKeyID(HotKeyManager.GetHotKeyId(categoryName, gameKeyId));
		}

		// Token: 0x06001DC5 RID: 7621 RVA: 0x0006B645 File Offset: 0x00069845
		public static TextObject GetHotKeyGameTextFromKeyID(this GameTextManager gameTextManager, string keyId)
		{
			return gameTextManager.FindText("str_game_key_text", keyId.ToLower());
		}
	}
}
