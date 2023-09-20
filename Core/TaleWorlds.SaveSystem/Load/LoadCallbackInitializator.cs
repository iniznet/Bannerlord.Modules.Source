using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x02000036 RID: 54
	internal class LoadCallbackInitializator
	{
		// Token: 0x060001E8 RID: 488 RVA: 0x00008E4E File Offset: 0x0000704E
		public LoadCallbackInitializator(LoadData loadData, ObjectHeaderLoadData[] objectHeaderLoadDatas, int objectCount)
		{
			this._loadData = loadData;
			this._objectHeaderLoadDatas = objectHeaderLoadDatas;
			this._objectCount = objectCount;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00008E6C File Offset: 0x0000706C
		public void InitializeObjects()
		{
			using (new PerformanceTestBlock("LoadContext::Callbacks"))
			{
				for (int i = 0; i < this._objectCount; i++)
				{
					ObjectHeaderLoadData objectHeaderLoadData = this._objectHeaderLoadDatas[i];
					if (objectHeaderLoadData.Target != null)
					{
						TypeDefinition typeDefinition = objectHeaderLoadData.TypeDefinition;
						IEnumerable<MethodInfo> enumerable = ((typeDefinition != null) ? typeDefinition.InitializationCallbacks : null);
						if (enumerable != null)
						{
							foreach (MethodInfo methodInfo in enumerable)
							{
								ParameterInfo[] parameters = methodInfo.GetParameters();
								if (parameters.Length > 1 && parameters[1].ParameterType == typeof(ObjectLoadData))
								{
									ObjectLoadData objectLoadData = LoadContext.CreateLoadData(this._loadData, i, objectHeaderLoadData);
									methodInfo.Invoke(objectHeaderLoadData.Target, new object[]
									{
										this._loadData.MetaData,
										objectLoadData
									});
								}
								else if (parameters.Length == 1)
								{
									methodInfo.Invoke(objectHeaderLoadData.Target, new object[] { this._loadData.MetaData });
								}
								else
								{
									methodInfo.Invoke(objectHeaderLoadData.Target, null);
								}
							}
						}
					}
				}
			}
			GC.Collect();
			this.AfterInitializeObjects();
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00008FE0 File Offset: 0x000071E0
		public void AfterInitializeObjects()
		{
			using (new PerformanceTestBlock("LoadContext::AfterCallbacks"))
			{
				for (int i = 0; i < this._objectCount; i++)
				{
					ObjectHeaderLoadData objectHeaderLoadData = this._objectHeaderLoadDatas[i];
					if (objectHeaderLoadData.Target != null)
					{
						TypeDefinition typeDefinition = objectHeaderLoadData.TypeDefinition;
						IEnumerable<MethodInfo> enumerable = ((typeDefinition != null) ? typeDefinition.LateInitializationCallbacks : null);
						if (enumerable != null)
						{
							foreach (MethodInfo methodInfo in enumerable)
							{
								ParameterInfo[] parameters = methodInfo.GetParameters();
								if (parameters.Length > 1 && parameters[1].ParameterType == typeof(ObjectLoadData))
								{
									ObjectLoadData objectLoadData = LoadContext.CreateLoadData(this._loadData, i, objectHeaderLoadData);
									methodInfo.Invoke(objectHeaderLoadData.Target, new object[]
									{
										this._loadData.MetaData,
										objectLoadData
									});
								}
								else if (parameters.Length == 1)
								{
									methodInfo.Invoke(objectHeaderLoadData.Target, new object[] { this._loadData.MetaData });
								}
								else
								{
									methodInfo.Invoke(objectHeaderLoadData.Target, null);
								}
							}
						}
					}
				}
			}
			GC.Collect();
		}

		// Token: 0x04000094 RID: 148
		private ObjectHeaderLoadData[] _objectHeaderLoadDatas;

		// Token: 0x04000095 RID: 149
		private int _objectCount;

		// Token: 0x04000096 RID: 150
		private LoadData _loadData;
	}
}
