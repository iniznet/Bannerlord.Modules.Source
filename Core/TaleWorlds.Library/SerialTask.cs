using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000082 RID: 130
	public class SerialTask : ITask
	{
		// Token: 0x06000476 RID: 1142 RVA: 0x0000EA77 File Offset: 0x0000CC77
		public SerialTask(SerialTask.DelegateDefinition function)
		{
			this._instance = function;
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0000EA86 File Offset: 0x0000CC86
		void ITask.Invoke()
		{
			this._instance();
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x0000EA93 File Offset: 0x0000CC93
		void ITask.Wait()
		{
		}

		// Token: 0x0400015A RID: 346
		private SerialTask.DelegateDefinition _instance;

		// Token: 0x020000CF RID: 207
		// (Invoke) Token: 0x060006E7 RID: 1767
		public delegate void DelegateDefinition();
	}
}
