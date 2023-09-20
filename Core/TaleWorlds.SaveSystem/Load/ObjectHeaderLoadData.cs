using System;
using System.Runtime.Serialization;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x0200003B RID: 59
	public class ObjectHeaderLoadData
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600020F RID: 527 RVA: 0x0000993A File Offset: 0x00007B3A
		// (set) Token: 0x06000210 RID: 528 RVA: 0x00009942 File Offset: 0x00007B42
		public int Id { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000211 RID: 529 RVA: 0x0000994B File Offset: 0x00007B4B
		// (set) Token: 0x06000212 RID: 530 RVA: 0x00009953 File Offset: 0x00007B53
		public object LoadedObject { get; private set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000213 RID: 531 RVA: 0x0000995C File Offset: 0x00007B5C
		// (set) Token: 0x06000214 RID: 532 RVA: 0x00009964 File Offset: 0x00007B64
		public object Target { get; private set; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000215 RID: 533 RVA: 0x0000996D File Offset: 0x00007B6D
		// (set) Token: 0x06000216 RID: 534 RVA: 0x00009975 File Offset: 0x00007B75
		public int PropertyCount { get; private set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000217 RID: 535 RVA: 0x0000997E File Offset: 0x00007B7E
		// (set) Token: 0x06000218 RID: 536 RVA: 0x00009986 File Offset: 0x00007B86
		public int ChildStructCount { get; private set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000219 RID: 537 RVA: 0x0000998F File Offset: 0x00007B8F
		// (set) Token: 0x0600021A RID: 538 RVA: 0x00009997 File Offset: 0x00007B97
		public TypeDefinition TypeDefinition { get; private set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600021B RID: 539 RVA: 0x000099A0 File Offset: 0x00007BA0
		// (set) Token: 0x0600021C RID: 540 RVA: 0x000099A8 File Offset: 0x00007BA8
		public LoadContext Context { get; private set; }

		// Token: 0x0600021D RID: 541 RVA: 0x000099B1 File Offset: 0x00007BB1
		public ObjectHeaderLoadData(LoadContext context, int id)
		{
			this.Context = context;
			this.Id = id;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x000099C8 File Offset: 0x00007BC8
		public void InitialieReaders(SaveEntryFolder saveEntryFolder)
		{
			BinaryReader binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Basics)).GetBinaryReader();
			this._saveId = SaveId.ReadSaveIdFrom(binaryReader);
			this.PropertyCount = (int)binaryReader.ReadShort();
			this.ChildStructCount = (int)binaryReader.ReadShort();
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00009A0C File Offset: 0x00007C0C
		public void CreateObject()
		{
			this.TypeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this._saveId) as TypeDefinition;
			if (this.TypeDefinition != null)
			{
				Type type = this.TypeDefinition.Type;
				this.LoadedObject = FormatterServices.GetUninitializedObject(type);
				this.Target = this.LoadedObject;
			}
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00009A66 File Offset: 0x00007C66
		public void AdvancedResolveObject(MetaData metaData, ObjectLoadData objectLoadData)
		{
			this.Target = this.TypeDefinition.AdvancedResolveObject(this.LoadedObject, metaData, objectLoadData);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x00009A81 File Offset: 0x00007C81
		public void ResolveObject()
		{
			this.Target = this.TypeDefinition.ResolveObject(this.LoadedObject);
		}

		// Token: 0x040000AE RID: 174
		private SaveId _saveId;
	}
}
