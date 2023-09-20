using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x02000039 RID: 57
	public class LoadResult
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000200 RID: 512 RVA: 0x0000987D File Offset: 0x00007A7D
		// (set) Token: 0x06000201 RID: 513 RVA: 0x00009885 File Offset: 0x00007A85
		public object Root { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000202 RID: 514 RVA: 0x0000988E File Offset: 0x00007A8E
		// (set) Token: 0x06000203 RID: 515 RVA: 0x00009896 File Offset: 0x00007A96
		public bool Successful { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000204 RID: 516 RVA: 0x0000989F File Offset: 0x00007A9F
		// (set) Token: 0x06000205 RID: 517 RVA: 0x000098A7 File Offset: 0x00007AA7
		public LoadError[] Errors { get; private set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000206 RID: 518 RVA: 0x000098B0 File Offset: 0x00007AB0
		// (set) Token: 0x06000207 RID: 519 RVA: 0x000098B8 File Offset: 0x00007AB8
		public MetaData MetaData { get; private set; }

		// Token: 0x06000208 RID: 520 RVA: 0x000098C1 File Offset: 0x00007AC1
		private LoadResult()
		{
		}

		// Token: 0x06000209 RID: 521 RVA: 0x000098C9 File Offset: 0x00007AC9
		internal static LoadResult CreateSuccessful(object root, MetaData metaData, LoadCallbackInitializator loadCallbackInitializator)
		{
			return new LoadResult
			{
				Root = root,
				Successful = true,
				MetaData = metaData,
				_loadCallbackInitializator = loadCallbackInitializator
			};
		}

		// Token: 0x0600020A RID: 522 RVA: 0x000098EC File Offset: 0x00007AEC
		internal static LoadResult CreateFailed(IEnumerable<LoadError> errors)
		{
			return new LoadResult
			{
				Successful = false,
				Errors = errors.ToArray<LoadError>()
			};
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00009906 File Offset: 0x00007B06
		public void InitializeObjects()
		{
			this._loadCallbackInitializator.InitializeObjects();
		}

		// Token: 0x040000A5 RID: 165
		private LoadCallbackInitializator _loadCallbackInitializator;
	}
}
