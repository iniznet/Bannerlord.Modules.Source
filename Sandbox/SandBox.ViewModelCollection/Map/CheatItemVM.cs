using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace SandBox.ViewModelCollection.Map
{
	public class CheatItemVM : StringItemWithActionVM
	{
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

		private readonly string[] _cheats;

		private bool _closeOnExecute;

		private Action _executeClose;
	}
}
