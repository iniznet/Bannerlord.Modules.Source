using System;

namespace TaleWorlds.DotNet
{
	public class EditableScriptComponentVariable : Attribute
	{
		public bool Visible { get; set; }

		public EditableScriptComponentVariable(bool visible)
		{
			this.Visible = visible;
		}
	}
}
