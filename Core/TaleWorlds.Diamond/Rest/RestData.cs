using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace TaleWorlds.Diamond.Rest
{
	// Token: 0x0200002F RID: 47
	[DataContract]
	[Serializable]
	public abstract class RestData
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000EA RID: 234 RVA: 0x0000373D File Offset: 0x0000193D
		// (set) Token: 0x060000EB RID: 235 RVA: 0x00003745 File Offset: 0x00001945
		[DataMember]
		public string TypeName { get; set; }

		// Token: 0x060000EC RID: 236 RVA: 0x0000374E File Offset: 0x0000194E
		protected RestData()
		{
			this.TypeName = base.GetType().FullName;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00003767 File Offset: 0x00001967
		public string SerializeAsJson()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
