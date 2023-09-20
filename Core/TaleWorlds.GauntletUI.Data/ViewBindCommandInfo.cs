using System;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI.Data
{
	internal class ViewBindCommandInfo
	{
		internal GauntletView Owner { get; private set; }

		internal string Command { get; private set; }

		internal BindingPath Path { get; private set; }

		internal string Parameter { get; private set; }

		internal ViewBindCommandInfo(GauntletView view, string command, BindingPath path, string parameter)
		{
			this.Owner = view;
			this.Command = command;
			this.Path = path;
			this.Parameter = parameter;
		}
	}
}
