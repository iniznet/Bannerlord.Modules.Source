using System;
using System.Collections.Generic;

namespace TaleWorlds.ObjectSystem
{
	public struct MbObjectXmlInformation
	{
		public MbObjectXmlInformation(string id, string name, string moduleName, List<string> gameTypesIncluded)
		{
			this.Id = id;
			this.Name = name;
			this.ModuleName = moduleName;
			this.GameTypesIncluded = gameTypesIncluded;
		}

		public string Id;

		public string Name;

		public string ModuleName;

		public List<string> GameTypesIncluded;
	}
}
