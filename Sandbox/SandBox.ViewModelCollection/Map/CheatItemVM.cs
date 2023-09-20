using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Map
{
	// Token: 0x0200002A RID: 42
	public class CheatItemVM : StringItemWithActionVM
	{
		// Token: 0x06000333 RID: 819 RVA: 0x0000F9BC File Offset: 0x0000DBBC
		public CheatItemVM(string name, string cheatCode, bool closeOnExecute, Action executeClose)
			: base(null, name, null)
		{
			this._closeOnExecute = closeOnExecute;
			this._executeClose = executeClose;
			this._cheats = cheatCode.Split(new char[] { ';' });
			this._onExecute = delegate(object o)
			{
				for (int i = 0; i < this._cheats.Length; i++)
				{
					string[] array = this._cheats[i].Split(new char[] { '.' });
					string[] array2 = array[1].Split(new char[] { ' ' });
					string text = array[0] + "." + array2[0];
					string[] array3 = new string[16];
					Array.Copy(array2, 1, array3, 0, array2.Length - 1);
					bool flag;
					CommandLineFunctionality.CallFunction(text, string.Join(" ", array3).Trim(), ref flag);
					if (!flag)
					{
						Utilities.ExecuteCommandLineCommand(text + " " + string.Join(" ", array3).Trim());
					}
				}
				if (this._closeOnExecute)
				{
					this._executeClose();
				}
			};
		}

		// Token: 0x040001A5 RID: 421
		private readonly string[] _cheats;

		// Token: 0x040001A6 RID: 422
		private bool _closeOnExecute;

		// Token: 0x040001A7 RID: 423
		private Action _executeClose;
	}
}
