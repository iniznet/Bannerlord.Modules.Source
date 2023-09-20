using System;
using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
	public class CommentSection
	{
		public CommentSection()
		{
			this._lines = new List<string>();
		}

		public void AddCommentLine(string line)
		{
			this._lines.Add(line);
		}

		public void GenerateInto(CodeGenerationFile codeGenerationFile)
		{
			foreach (string text in this._lines)
			{
				codeGenerationFile.AddLine("//" + text);
			}
		}

		private List<string> _lines;
	}
}
