using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	public class LoadContext
	{
		public static bool EnableLoadStatistics
		{
			get
			{
				return false;
			}
		}

		public object RootObject { get; private set; }

		public DefinitionContext DefinitionContext { get; private set; }

		public ISaveDriver Driver { get; private set; }

		public LoadContext(DefinitionContext definitionContext, ISaveDriver driver)
		{
			this.DefinitionContext = definitionContext;
			this._objectHeaderLoadDatas = null;
			this._containerHeaderLoadDatas = null;
			this._strings = null;
			this.Driver = driver;
		}

		internal static ObjectLoadData CreateLoadData(LoadData loadData, int i, ObjectHeaderLoadData header)
		{
			ArchiveDeserializer archiveDeserializer = new ArchiveDeserializer();
			archiveDeserializer.LoadFrom(loadData.GameData.ObjectData[i]);
			SaveEntryFolder rootFolder = archiveDeserializer.RootFolder;
			ObjectLoadData objectLoadData = new ObjectLoadData(header);
			SaveEntryFolder childFolder = rootFolder.GetChildFolder(new FolderId(i, SaveFolderExtension.Object));
			objectLoadData.InitializeReaders(childFolder);
			objectLoadData.FillCreatedObject();
			objectLoadData.Read();
			objectLoadData.FillObject();
			return objectLoadData;
		}

		public bool Load(LoadData loadData, bool loadAsLateInitialize)
		{
			bool flag = false;
			try
			{
				using (new PerformanceTestBlock("LoadContext::Load Headers"))
				{
					using (new PerformanceTestBlock("LoadContext::Load And Create Header"))
					{
						ArchiveDeserializer archiveDeserializer = new ArchiveDeserializer();
						archiveDeserializer.LoadFrom(loadData.GameData.Header);
						SaveEntryFolder headerRootFolder = archiveDeserializer.RootFolder;
						BinaryReader binaryReader = headerRootFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Config)).GetBinaryReader();
						this._objectCount = binaryReader.ReadInt();
						this._stringCount = binaryReader.ReadInt();
						this._containerCount = binaryReader.ReadInt();
						this._objectHeaderLoadDatas = new ObjectHeaderLoadData[this._objectCount];
						this._containerHeaderLoadDatas = new ContainerHeaderLoadData[this._containerCount];
						this._strings = new string[this._stringCount];
						if (LoadContext.EnableLoadStatistics)
						{
							for (int i = 0; i < this._objectCount; i++)
							{
								ObjectHeaderLoadData objectHeaderLoadData = new ObjectHeaderLoadData(this, i);
								SaveEntryFolder childFolder = headerRootFolder.GetChildFolder(new FolderId(i, SaveFolderExtension.Object));
								objectHeaderLoadData.InitialieReaders(childFolder);
								this._objectHeaderLoadDatas[i] = objectHeaderLoadData;
							}
							for (int j = 0; j < this._containerCount; j++)
							{
								ContainerHeaderLoadData containerHeaderLoadData = new ContainerHeaderLoadData(this, j);
								SaveEntryFolder childFolder2 = headerRootFolder.GetChildFolder(new FolderId(j, SaveFolderExtension.Container));
								containerHeaderLoadData.InitialieReaders(childFolder2);
								this._containerHeaderLoadDatas[j] = containerHeaderLoadData;
							}
						}
						else
						{
							TWParallel.For(0, this._objectCount, delegate(int startInclusive, int endExclusive)
							{
								for (int num2 = startInclusive; num2 < endExclusive; num2++)
								{
									ObjectHeaderLoadData objectHeaderLoadData5 = new ObjectHeaderLoadData(this, num2);
									SaveEntryFolder childFolder4 = headerRootFolder.GetChildFolder(new FolderId(num2, SaveFolderExtension.Object));
									objectHeaderLoadData5.InitialieReaders(childFolder4);
									this._objectHeaderLoadDatas[num2] = objectHeaderLoadData5;
								}
							}, 16);
							TWParallel.For(0, this._containerCount, delegate(int startInclusive, int endExclusive)
							{
								for (int num3 = startInclusive; num3 < endExclusive; num3++)
								{
									ContainerHeaderLoadData containerHeaderLoadData3 = new ContainerHeaderLoadData(this, num3);
									SaveEntryFolder childFolder5 = headerRootFolder.GetChildFolder(new FolderId(num3, SaveFolderExtension.Container));
									containerHeaderLoadData3.InitialieReaders(childFolder5);
									this._containerHeaderLoadDatas[num3] = containerHeaderLoadData3;
								}
							}, 16);
						}
					}
					using (new PerformanceTestBlock("LoadContext::Create Objects"))
					{
						foreach (ObjectHeaderLoadData objectHeaderLoadData2 in this._objectHeaderLoadDatas)
						{
							objectHeaderLoadData2.CreateObject();
							if (objectHeaderLoadData2.Id == 0)
							{
								this.RootObject = objectHeaderLoadData2.Target;
							}
						}
						foreach (ContainerHeaderLoadData containerHeaderLoadData2 in this._containerHeaderLoadDatas)
						{
							if (containerHeaderLoadData2.GetObjectTypeDefinition())
							{
								containerHeaderLoadData2.CreateObject();
							}
						}
					}
				}
				GC.Collect();
				using (new PerformanceTestBlock("LoadContext::Load Strings"))
				{
					ArchiveDeserializer archiveDeserializer2 = new ArchiveDeserializer();
					archiveDeserializer2.LoadFrom(loadData.GameData.Strings);
					for (int l = 0; l < this._stringCount; l++)
					{
						string text = LoadContext.LoadString(archiveDeserializer2, l);
						this._strings[l] = text;
					}
				}
				GC.Collect();
				using (new PerformanceTestBlock("LoadContext::Resolve Objects"))
				{
					for (int m = 0; m < this._objectHeaderLoadDatas.Length; m++)
					{
						ObjectHeaderLoadData objectHeaderLoadData3 = this._objectHeaderLoadDatas[m];
						TypeDefinition typeDefinition = objectHeaderLoadData3.TypeDefinition;
						if (typeDefinition != null)
						{
							object loadedObject = objectHeaderLoadData3.LoadedObject;
							if (typeDefinition.CheckIfRequiresAdvancedResolving(loadedObject))
							{
								ObjectLoadData objectLoadData = LoadContext.CreateLoadData(loadData, m, objectHeaderLoadData3);
								objectHeaderLoadData3.AdvancedResolveObject(loadData.MetaData, objectLoadData);
							}
							else
							{
								objectHeaderLoadData3.ResolveObject();
							}
						}
					}
				}
				GC.Collect();
				using (new PerformanceTestBlock("LoadContext::Load Object Datas"))
				{
					if (LoadContext.EnableLoadStatistics)
					{
						for (int n = 0; n < this._objectCount; n++)
						{
							ObjectHeaderLoadData objectHeaderLoadData4 = this._objectHeaderLoadDatas[n];
							if (objectHeaderLoadData4.Target == objectHeaderLoadData4.LoadedObject)
							{
								LoadContext.CreateLoadData(loadData, n, objectHeaderLoadData4);
							}
						}
					}
					else
					{
						TWParallel.For(0, this._objectCount, delegate(int startInclusive, int endExclusive)
						{
							for (int num4 = startInclusive; num4 < endExclusive; num4++)
							{
								ObjectHeaderLoadData objectHeaderLoadData6 = this._objectHeaderLoadDatas[num4];
								if (objectHeaderLoadData6.Target == objectHeaderLoadData6.LoadedObject)
								{
									LoadContext.CreateLoadData(loadData, num4, objectHeaderLoadData6);
								}
							}
						}, 16);
					}
				}
				using (new PerformanceTestBlock("LoadContext::Load Container Datas"))
				{
					if (LoadContext.EnableLoadStatistics)
					{
						for (int num = 0; num < this._containerCount; num++)
						{
							byte[] array = loadData.GameData.ContainerData[num];
							ArchiveDeserializer archiveDeserializer3 = new ArchiveDeserializer();
							archiveDeserializer3.LoadFrom(array);
							SaveEntryFolder rootFolder = archiveDeserializer3.RootFolder;
							ContainerLoadData containerLoadData = new ContainerLoadData(this._containerHeaderLoadDatas[num]);
							SaveEntryFolder childFolder3 = rootFolder.GetChildFolder(new FolderId(num, SaveFolderExtension.Container));
							containerLoadData.InitializeReaders(childFolder3);
							containerLoadData.FillCreatedObject();
							containerLoadData.Read();
							containerLoadData.FillObject();
						}
					}
					else
					{
						TWParallel.For(0, this._containerCount, delegate(int startInclusive, int endExclusive)
						{
							for (int num5 = startInclusive; num5 < endExclusive; num5++)
							{
								byte[] array2 = loadData.GameData.ContainerData[num5];
								ArchiveDeserializer archiveDeserializer4 = new ArchiveDeserializer();
								archiveDeserializer4.LoadFrom(array2);
								SaveEntryFolder rootFolder2 = archiveDeserializer4.RootFolder;
								ContainerLoadData containerLoadData2 = new ContainerLoadData(this._containerHeaderLoadDatas[num5]);
								SaveEntryFolder childFolder6 = rootFolder2.GetChildFolder(new FolderId(num5, SaveFolderExtension.Container));
								containerLoadData2.InitializeReaders(childFolder6);
								containerLoadData2.FillCreatedObject();
								containerLoadData2.Read();
								containerLoadData2.FillObject();
							}
						}, 16);
					}
				}
				GC.Collect();
				if (!loadAsLateInitialize)
				{
					this.CreateLoadCallbackInitializator(loadData).InitializeObjects();
				}
				flag = true;
			}
			catch (Exception ex)
			{
				Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
				flag = false;
			}
			return flag;
		}

		internal LoadCallbackInitializator CreateLoadCallbackInitializator(LoadData loadData)
		{
			return new LoadCallbackInitializator(loadData, this._objectHeaderLoadDatas, this._objectCount);
		}

		private static string LoadString(ArchiveDeserializer saveArchive, int id)
		{
			return saveArchive.RootFolder.GetChildFolder(new FolderId(-1, SaveFolderExtension.Strings)).GetEntry(new EntryId(id, SaveEntryExtension.Txt)).GetBinaryReader()
				.ReadString();
		}

		public static bool TryConvertType(Type sourceType, Type targetType, ref object data)
		{
			if (LoadContext.<TryConvertType>g__isNum|25_2(sourceType) && LoadContext.<TryConvertType>g__isNum|25_2(targetType))
			{
				try
				{
					data = Convert.ChangeType(data, targetType);
					return true;
				}
				catch
				{
					return false;
				}
			}
			if (LoadContext.<TryConvertType>g__isNum|25_2(sourceType) && targetType == typeof(string))
			{
				if (LoadContext.<TryConvertType>g__isInt|25_0(sourceType))
				{
					data = Convert.ToInt64(data).ToString();
				}
				else if (LoadContext.<TryConvertType>g__isFloat|25_1(sourceType))
				{
					data = Convert.ToDouble(data).ToString(CultureInfo.InvariantCulture);
				}
				return true;
			}
			if (sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == typeof(List<>) && targetType.IsGenericType)
			{
				targetType.GetGenericTypeDefinition() == typeof(MBList<>);
			}
			return false;
		}

		public ObjectHeaderLoadData GetObjectWithId(int id)
		{
			ObjectHeaderLoadData objectHeaderLoadData = null;
			if (id != -1)
			{
				objectHeaderLoadData = this._objectHeaderLoadDatas[id];
			}
			return objectHeaderLoadData;
		}

		public ContainerHeaderLoadData GetContainerWithId(int id)
		{
			ContainerHeaderLoadData containerHeaderLoadData = null;
			if (id != -1)
			{
				containerHeaderLoadData = this._containerHeaderLoadDatas[id];
			}
			return containerHeaderLoadData;
		}

		public string GetStringWithId(int id)
		{
			string text = null;
			if (id != -1)
			{
				text = this._strings[id];
			}
			return text;
		}

		[CompilerGenerated]
		internal static bool <TryConvertType>g__isInt|25_0(Type type)
		{
			return type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(ulong) || type == typeof(uint) || type == typeof(ushort);
		}

		[CompilerGenerated]
		internal static bool <TryConvertType>g__isFloat|25_1(Type type)
		{
			return type == typeof(double) || type == typeof(float);
		}

		[CompilerGenerated]
		internal static bool <TryConvertType>g__isNum|25_2(Type type)
		{
			return LoadContext.<TryConvertType>g__isInt|25_0(type) || LoadContext.<TryConvertType>g__isFloat|25_1(type);
		}

		private int _objectCount;

		private int _stringCount;

		private int _containerCount;

		private ObjectHeaderLoadData[] _objectHeaderLoadDatas;

		private ContainerHeaderLoadData[] _containerHeaderLoadDatas;

		private string[] _strings;
	}
}
