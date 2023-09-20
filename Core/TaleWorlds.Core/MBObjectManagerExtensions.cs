using System;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
	public static class MBObjectManagerExtensions
	{
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
