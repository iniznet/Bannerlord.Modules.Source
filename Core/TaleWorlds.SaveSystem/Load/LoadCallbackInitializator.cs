using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	internal class LoadCallbackInitializator
	{
		public LoadCallbackInitializator(LoadData loadData, ObjectHeaderLoadData[] objectHeaderLoadDatas, int objectCount)
		{
			this._loadData = loadData;
			this._objectHeaderLoadDatas = objectHeaderLoadDatas;
			this._objectCount = objectCount;
		}

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

		private ObjectHeaderLoadData[] _objectHeaderLoadDatas;

		private int _objectCount;

		private LoadData _loadData;
	}
}
