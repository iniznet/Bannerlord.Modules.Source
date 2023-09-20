using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TaleWorlds.Diamond.Rest
{
	[DataContract]
	[Serializable]
	public abstract class RestData
	{
		[DataMember]
		public string TypeName { get; set; }

		protected RestData()
		{
			this.TypeName = base.GetType().FullName;
		}

		public string SerializeAsJson()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
