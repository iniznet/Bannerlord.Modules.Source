using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	public class CodeBlock
	{
		public List<string> Lines
		{
			get
			{
				return this._lines;
			}
		}

		public CodeBlock()
		{
			this._lines = new List<string>();
		}

		public void AddLine(string line)
		{
			this._lines.Add(line);
		}

		public void AddLines(IEnumerable<string> lines)
		{
			foreach (string text in lines)
			{
				this._lines.Add(text);
			}
		}

		private List<string> _lines;
	}
}
