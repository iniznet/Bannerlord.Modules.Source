using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x0200003C RID: 60
	public class ObjectLoadData
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000222 RID: 546 RVA: 0x00009A9A File Offset: 0x00007C9A
		// (set) Token: 0x06000223 RID: 547 RVA: 0x00009AA2 File Offset: 0x00007CA2
		public int Id { get; private set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000224 RID: 548 RVA: 0x00009AAB File Offset: 0x00007CAB
		// (set) Token: 0x06000225 RID: 549 RVA: 0x00009AB3 File Offset: 0x00007CB3
		public object Target { get; private set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000226 RID: 550 RVA: 0x00009ABC File Offset: 0x00007CBC
		// (set) Token: 0x06000227 RID: 551 RVA: 0x00009AC4 File Offset: 0x00007CC4
		public LoadContext Context { get; private set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000228 RID: 552 RVA: 0x00009ACD File Offset: 0x00007CCD
		// (set) Token: 0x06000229 RID: 553 RVA: 0x00009AD5 File Offset: 0x00007CD5
		public TypeDefinition TypeDefinition { get; private set; }

		// Token: 0x0600022A RID: 554 RVA: 0x00009AE0 File Offset: 0x00007CE0
		public object GetDataBySaveId(int localSaveId)
		{
			MemberLoadData memberLoadData = this._memberValues.SingleOrDefault((MemberLoadData value) => (int)value.MemberSaveId.LocalSaveId == localSaveId);
			if (memberLoadData != null)
			{
				return memberLoadData.GetDataToUse();
			}
			return null;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x00009B20 File Offset: 0x00007D20
		public object GetMemberValueBySaveId(int localSaveId)
		{
			MemberLoadData memberLoadData = this._memberValues.SingleOrDefault((MemberLoadData value) => (int)value.MemberSaveId.LocalSaveId == localSaveId);
			if (memberLoadData == null)
			{
				return null;
			}
			return memberLoadData.GetDataToUse();
		}

		// Token: 0x0600022C RID: 556 RVA: 0x00009B5C File Offset: 0x00007D5C
		public object GetFieldValueBySaveId(int localSaveId)
		{
			FieldLoadData fieldLoadData = this._fieldValues.SingleOrDefault((FieldLoadData value) => (int)value.MemberSaveId.LocalSaveId == localSaveId);
			if (fieldLoadData == null)
			{
				return null;
			}
			return fieldLoadData.GetDataToUse();
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00009B98 File Offset: 0x00007D98
		public object GetPropertyValueBySaveId(int localSaveId)
		{
			PropertyLoadData propertyLoadData = this._propertyValues.SingleOrDefault((PropertyLoadData value) => (int)value.MemberSaveId.LocalSaveId == localSaveId);
			if (propertyLoadData == null)
			{
				return null;
			}
			return propertyLoadData.GetDataToUse();
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00009BD4 File Offset: 0x00007DD4
		public bool HasMember(int localSaveId)
		{
			return this._memberValues.Any((MemberLoadData x) => (int)x.MemberSaveId.LocalSaveId == localSaveId);
		}

		// Token: 0x0600022F RID: 559 RVA: 0x00009C08 File Offset: 0x00007E08
		public ObjectLoadData(LoadContext context, int id)
		{
			this.Context = context;
			this.Id = id;
			this._propertyValues = new List<PropertyLoadData>();
			this._fieldValues = new List<FieldLoadData>();
			this._memberValues = new List<MemberLoadData>();
			this._childStructs = new List<ObjectLoadData>();
		}

		// Token: 0x06000230 RID: 560 RVA: 0x00009C58 File Offset: 0x00007E58
		public ObjectLoadData(ObjectHeaderLoadData headerLoadData)
		{
			this.Id = headerLoadData.Id;
			this.Target = headerLoadData.Target;
			this.Context = headerLoadData.Context;
			this.TypeDefinition = headerLoadData.TypeDefinition;
			this._propertyValues = new List<PropertyLoadData>();
			this._fieldValues = new List<FieldLoadData>();
			this._memberValues = new List<MemberLoadData>();
			this._childStructs = new List<ObjectLoadData>();
		}

		// Token: 0x06000231 RID: 561 RVA: 0x00009CC8 File Offset: 0x00007EC8
		public void InitializeReaders(SaveEntryFolder saveEntryFolder)
		{
			BinaryReader binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Basics)).GetBinaryReader();
			this._saveId = SaveId.ReadSaveIdFrom(binaryReader);
			this._propertyCount = binaryReader.ReadShort();
			this._childStructCount = binaryReader.ReadShort();
			for (int i = 0; i < (int)this._childStructCount; i++)
			{
				ObjectLoadData objectLoadData = new ObjectLoadData(this.Context, i);
				this._childStructs.Add(objectLoadData);
			}
			foreach (SaveEntry saveEntry in saveEntryFolder.ChildEntries)
			{
				if (saveEntry.Id.Extension == SaveEntryExtension.Property)
				{
					BinaryReader binaryReader2 = saveEntry.GetBinaryReader();
					PropertyLoadData propertyLoadData = new PropertyLoadData(this, binaryReader2);
					this._propertyValues.Add(propertyLoadData);
					this._memberValues.Add(propertyLoadData);
				}
				else if (saveEntry.Id.Extension == SaveEntryExtension.Field)
				{
					BinaryReader binaryReader3 = saveEntry.GetBinaryReader();
					FieldLoadData fieldLoadData = new FieldLoadData(this, binaryReader3);
					this._fieldValues.Add(fieldLoadData);
					this._memberValues.Add(fieldLoadData);
				}
			}
			for (int j = 0; j < (int)this._childStructCount; j++)
			{
				ObjectLoadData objectLoadData2 = this._childStructs[j];
				SaveEntryFolder childFolder = saveEntryFolder.GetChildFolder(new FolderId(j, SaveFolderExtension.Struct));
				objectLoadData2.InitializeReaders(childFolder);
			}
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00009E38 File Offset: 0x00008038
		public void CreateStruct()
		{
			this.TypeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this._saveId) as TypeDefinition;
			if (this.TypeDefinition != null)
			{
				Type type = this.TypeDefinition.Type;
				this.Target = FormatterServices.GetUninitializedObject(type);
			}
			foreach (ObjectLoadData objectLoadData in this._childStructs)
			{
				objectLoadData.CreateStruct();
			}
		}

		// Token: 0x06000233 RID: 563 RVA: 0x00009ECC File Offset: 0x000080CC
		public void FillCreatedObject()
		{
			foreach (ObjectLoadData objectLoadData in this._childStructs)
			{
				objectLoadData.CreateStruct();
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00009F1C File Offset: 0x0000811C
		public void Read()
		{
			foreach (ObjectLoadData objectLoadData in this._childStructs)
			{
				objectLoadData.Read();
			}
			foreach (MemberLoadData memberLoadData in this._memberValues)
			{
				memberLoadData.Read();
				if (memberLoadData.SavedMemberType == SavedMemberType.CustomStruct)
				{
					int num = (int)memberLoadData.Data;
					object target = this._childStructs[num].Target;
					memberLoadData.SetCustomStructData(target);
				}
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00009FE0 File Offset: 0x000081E0
		public void FillObject()
		{
			foreach (ObjectLoadData objectLoadData in this._childStructs)
			{
				objectLoadData.FillObject();
			}
			foreach (FieldLoadData fieldLoadData in this._fieldValues)
			{
				fieldLoadData.FillObject();
			}
			foreach (PropertyLoadData propertyLoadData in this._propertyValues)
			{
				propertyLoadData.FillObject();
			}
		}

		// Token: 0x040000B3 RID: 179
		private short _propertyCount;

		// Token: 0x040000B4 RID: 180
		private List<PropertyLoadData> _propertyValues;

		// Token: 0x040000B5 RID: 181
		private List<FieldLoadData> _fieldValues;

		// Token: 0x040000B6 RID: 182
		private List<MemberLoadData> _memberValues;

		// Token: 0x040000B7 RID: 183
		private SaveId _saveId;

		// Token: 0x040000B8 RID: 184
		private List<ObjectLoadData> _childStructs;

		// Token: 0x040000B9 RID: 185
		private short _childStructCount;
	}
}
