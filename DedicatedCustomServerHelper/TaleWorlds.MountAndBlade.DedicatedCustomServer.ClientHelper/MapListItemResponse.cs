using System;
using Newtonsoft.Json;

namespace TaleWorlds.MountAndBlade.DedicatedCustomServer.ClientHelper
{
	[Serializable]
	public class MapListItemResponse
	{
		public string Name { get; private set; }

		public string UniqueToken { get; private set; }

		public string Revision { get; private set; }

		[JsonConstructor]
		public MapListItemResponse(string name, string uniqueToken, string revision)
		{
			this.Name = name;
			this.UniqueToken = uniqueToken;
			this.Revision = revision;
		}
	}
}
