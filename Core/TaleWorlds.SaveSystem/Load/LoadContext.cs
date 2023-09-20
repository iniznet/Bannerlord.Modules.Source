using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x02000037 RID: 55
	public class LoadContext
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060001EB RID: 491 RVA: 0x00009150 File Offset: 0x00007350
		// (set) Token: 0x060001EC RID: 492 RVA: 0x00009158 File Offset: 0x00007358
		public object RootObject { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060001ED RID: 493 RVA: 0x00009161 File Offset: 0x00007361
		// (set) Token: 0x060001EE RID: 494 RVA: 0x00009169 File Offset: 0x00007369
		public DefinitionContext DefinitionContext { get; private set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060001EF RID: 495 RVA: 0x00009172 File Offset: 0x00007372
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x0000917A File Offset: 0x0000737A
		public ISaveDriver Driver { get; private set; }

		// Token: 0x060001F1 RID: 497 RVA: 0x00009183 File Offset: 0x00007383
		public LoadContext(DefinitionContext definitionContext, ISaveDriver driver)
		{
			this.DefinitionContext = definitionContext;
			this._objectHeaderLoadDatas = null;
			this._containerHeaderLoadDatas = null;
			this._strings = null;
			this.Driver = driver;
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x000091B0 File Offset: 0x000073B0
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

		// Token: 0x060001F3 RID: 499 RVA: 0x00009208 File Offset: 0x00007408
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
						TWParallel.For(0, this._objectCount, delegate(int startInclusive, int endExclusive)
						{
							for (int l = startInclusive; l < endExclusive; l++)
							{
								ObjectHeaderLoadData objectHeaderLoadData3 = new ObjectHeaderLoadData(this, l);
								SaveEntryFolder childFolder = headerRootFolder.GetChildFolder(new FolderId(l, SaveFolderExtension.Object));
								objectHeaderLoadData3.InitialieReaders(childFolder);
								this._objectHeaderLoadDatas[l] = objectHeaderLoadData3;
							}
						}, 16);
						TWParallel.For(0, this._containerCount, delegate(int startInclusive, int endExclusive)
						{
							for (int m = startInclusive; m < endExclusive; m++)
							{
								ContainerHeaderLoadData containerHeaderLoadData2 = new ContainerHeaderLoadData(this, m);
								SaveEntryFolder childFolder2 = headerRootFolder.GetChildFolder(new FolderId(m, SaveFolderExtension.Container));
								containerHeaderLoadData2.InitialieReaders(childFolder2);
								this._containerHeaderLoadDatas[m] = containerHeaderLoadData2;
							}
						}, 16);
					}
					using (new PerformanceTestBlock("LoadContext::Create Objects"))
					{
						foreach (ObjectHeaderLoadData objectHeaderLoadData in this._objectHeaderLoadDatas)
						{
							objectHeaderLoadData.CreateObject();
							if (objectHeaderLoadData.Id == 0)
							{
								this.RootObject = objectHeaderLoadData.Target;
							}
						}
						foreach (ContainerHeaderLoadData containerHeaderLoadData in this._containerHeaderLoadDatas)
						{
							if (containerHeaderLoadData.GetObjectTypeDefinition())
							{
								containerHeaderLoadData.CreateObject();
							}
						}
					}
				}
				GC.Collect();
				using (new PerformanceTestBlock("LoadContext::Load Strings"))
				{
					ArchiveDeserializer archiveDeserializer2 = new ArchiveDeserializer();
					archiveDeserializer2.LoadFrom(loadData.GameData.Strings);
					for (int j = 0; j < this._stringCount; j++)
					{
						string text = LoadContext.LoadString(archiveDeserializer2, j);
						this._strings[j] = text;
					}
				}
				GC.Collect();
				using (new PerformanceTestBlock("LoadContext::Resolve Objects"))
				{
					for (int k = 0; k < this._objectHeaderLoadDatas.Length; k++)
					{
						ObjectHeaderLoadData objectHeaderLoadData2 = this._objectHeaderLoadDatas[k];
						TypeDefinition typeDefinition = objectHeaderLoadData2.TypeDefinition;
						if (typeDefinition != null)
						{
							object loadedObject = objectHeaderLoadData2.LoadedObject;
							if (typeDefinition.CheckIfRequiresAdvancedResolving(loadedObject))
							{
								ObjectLoadData objectLoadData = LoadContext.CreateLoadData(loadData, k, objectHeaderLoadData2);
								objectHeaderLoadData2.AdvancedResolveObject(loadData.MetaData, objectLoadData);
							}
							else
							{
								objectHeaderLoadData2.ResolveObject();
							}
						}
					}
				}
				GC.Collect();
				using (new PerformanceTestBlock("LoadContext::Load Object Datas"))
				{
					TWParallel.For(0, this._objectCount, delegate(int startInclusive, int endExclusive)
					{
						for (int n = startInclusive; n < endExclusive; n++)
						{
							ObjectHeaderLoadData objectHeaderLoadData4 = this._objectHeaderLoadDatas[n];
							if (objectHeaderLoadData4.Target == objectHeaderLoadData4.LoadedObject)
							{
								LoadContext.CreateLoadData(loadData, n, objectHeaderLoadData4);
							}
						}
					}, 16);
				}
				using (new PerformanceTestBlock("LoadContext::Load Container Datas"))
				{
					TWParallel.For(0, this._containerCount, delegate(int startInclusive, int endExclusive)
					{
						for (int num = startInclusive; num < endExclusive; num++)
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
					}, 16);
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

		// Token: 0x060001F4 RID: 500 RVA: 0x00009630 File Offset: 0x00007830
		internal LoadCallbackInitializator CreateLoadCallbackInitializator(LoadData loadData)
		{
			return new LoadCallbackInitializator(loadData, this._objectHeaderLoadDatas, this._objectCount);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00009644 File Offset: 0x00007844
		private static string LoadString(ArchiveDeserializer saveArchive, int id)
		{
			return saveArchive.RootFolder.GetChildFolder(new FolderId(-1, SaveFolderExtension.Strings)).GetEntry(new EntryId(id, SaveEntryExtension.Txt)).GetBinaryReader()
				.ReadString();
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00009670 File Offset: 0x00007870
		public static bool TryConvertType(Type sourceType, Type targetType, ref object data)
		{
			if (LoadContext.<TryConvertType>g__isNum|23_2(sourceType) && LoadContext.<TryConvertType>g__isNum|23_2(targetType))
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
			if (LoadContext.<TryConvertType>g__isNum|23_2(sourceType) && targetType == typeof(string))
			{
				if (LoadContext.<TryConvertType>g__isInt|23_0(sourceType))
				{
					data = Convert.ToInt64(data).ToString();
				}
				else if (LoadContext.<TryConvertType>g__isFloat|23_1(sourceType))
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

		// Token: 0x060001F7 RID: 503 RVA: 0x0000974C File Offset: 0x0000794C
		public ObjectHeaderLoadData GetObjectWithId(int id)
		{
			ObjectHeaderLoadData objectHeaderLoadData = null;
			if (id != -1)
			{
				objectHeaderLoadData = this._objectHeaderLoadDatas[id];
			}
			return objectHeaderLoadData;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000976C File Offset: 0x0000796C
		public ContainerHeaderLoadData GetContainerWithId(int id)
		{
			ContainerHeaderLoadData containerHeaderLoadData = null;
			if (id != -1)
			{
				containerHeaderLoadData = this._containerHeaderLoadDatas[id];
			}
			return containerHeaderLoadData;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000978C File Offset: 0x0000798C
		public string GetStringWithId(int id)
		{
			string text = null;
			if (id != -1)
			{
				text = this._strings[id];
			}
			return text;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x000097AC File Offset: 0x000079AC
		[CompilerGenerated]
		internal static bool <TryConvertType>g__isInt|23_0(Type type)
		{
			return type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(ulong) || type == typeof(uint) || type == typeof(ushort);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x00009825 File Offset: 0x00007A25
		[CompilerGenerated]
		internal static bool <TryConvertType>g__isFloat|23_1(Type type)
		{
			return type == typeof(double) || type == typeof(float);
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000984B File Offset: 0x00007A4B
		[CompilerGenerated]
		internal static bool <TryConvertType>g__isNum|23_2(Type type)
		{
			return LoadContext.<TryConvertType>g__isInt|23_0(type) || LoadContext.<TryConvertType>g__isFloat|23_1(type);
		}

		// Token: 0x04000098 RID: 152
		private int _objectCount;

		// Token: 0x04000099 RID: 153
		private int _stringCount;

		// Token: 0x0400009A RID: 154
		private int _containerCount;

		// Token: 0x0400009B RID: 155
		private ObjectHeaderLoadData[] _objectHeaderLoadDatas;

		// Token: 0x0400009C RID: 156
		private ContainerHeaderLoadData[] _containerHeaderLoadDatas;

		// Token: 0x0400009D RID: 157
		private string[] _strings;
	}
}
