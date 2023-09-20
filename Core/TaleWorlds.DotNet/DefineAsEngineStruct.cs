using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200000A RID: 10
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class DefineAsEngineStruct : Attribute
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000027A8 File Offset: 0x000009A8
		// (set) Token: 0x06000022 RID: 34 RVA: 0x000027B0 File Offset: 0x000009B0
		public Type Type { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000023 RID: 35 RVA: 0x000027B9 File Offset: 0x000009B9
		// (set) Token: 0x06000024 RID: 36 RVA: 0x000027C1 File Offset: 0x000009C1
		public string EngineType { get; set; }

		// Token: 0x06000025 RID: 37 RVA: 0x000027CA File Offset: 0x000009CA
		public DefineAsEngineStruct(Type type, string engineType)
		{
			this.Type = type;
			this.EngineType = engineType;
		}
	}
}
