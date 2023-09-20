using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000065 RID: 101
	public abstract class MessageManagerBase : DotNetObject
	{
		// Token: 0x060007FE RID: 2046
		[EngineCallback]
		protected internal abstract void PostWarningLine(string text);

		// Token: 0x060007FF RID: 2047
		[EngineCallback]
		protected internal abstract void PostSuccessLine(string text);

		// Token: 0x06000800 RID: 2048
		[EngineCallback]
		protected internal abstract void PostMessageLineFormatted(string text, uint color);

		// Token: 0x06000801 RID: 2049
		[EngineCallback]
		protected internal abstract void PostMessageLine(string text, uint color);
	}
}
