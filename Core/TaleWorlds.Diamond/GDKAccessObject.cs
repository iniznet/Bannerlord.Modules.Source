using System;
using Newtonsoft.Json;

namespace TaleWorlds.Diamond
{
	[Serializable]
	public class GDKAccessObject : AccessObject
	{
		public GDKAccessObject()
		{
			base.Type = "GDK";
		}

		[JsonProperty]
		public string Id;

		[JsonProperty]
		public string Token;
	}
}
