using System;

namespace JetBrains.Annotations
{
	// Token: 0x020000CD RID: 205
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class StringFormatMethodAttribute : Attribute
	{
		// Token: 0x0600085F RID: 2143 RVA: 0x0000F1DB File Offset: 0x0000D3DB
		public StringFormatMethodAttribute(string formatParameterName)
		{
			this.FormatParameterName = formatParameterName;
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000860 RID: 2144 RVA: 0x0000F1EA File Offset: 0x0000D3EA
		// (set) Token: 0x06000861 RID: 2145 RVA: 0x0000F1F2 File Offset: 0x0000D3F2
		[UsedImplicitly]
		public string FormatParameterName { get; private set; }
	}
}
