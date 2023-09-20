using System;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	// Token: 0x020000A8 RID: 168
	public static class MBObjectManagerExtensions
	{
		// Token: 0x06000837 RID: 2103 RVA: 0x0001BFAC File Offset: 0x0001A1AC
		public static void LoadXML(this MBObjectManager objectManager, string id, bool skipXmlFilterForEditor = false)
		{
			Game game = Game.Current;
			bool flag = false;
			string text = "";
			if (game != null)
			{
				flag = game.GameType.IsDevelopment;
				text = game.GameType.GetType().Name;
			}
			objectManager.LoadXML(id, flag, text, skipXmlFilterForEditor);
		}
	}
}
