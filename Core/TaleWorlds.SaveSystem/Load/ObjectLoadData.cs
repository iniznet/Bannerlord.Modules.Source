using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	public class ObjectLoadData
	{
		public int Id { get; private set; }

		public object Target { get; private set; }

		public LoadContext Context { get; private set; }

		public TypeDefinition TypeDefinition { get; private set; }

		public object GetDataBySaveId(int localSaveId)
		{
			MemberLoadData memberLoadData = this._memberValues.SingleOrDefault((MemberLoadData value) => (int)value.MemberSaveId.LocalSaveId == localSaveId);
			if (memberLoadData != null)
			{
				return memberLoadData.GetDataToUse();
			}
			return null;
		}

		public object GetMemberValueBySaveId(int localSaveId)
		{
			MemberLoadData memberLoadData = this._memberValues.SingleOrDefault((MemberLoadData value) => (int)value.MemberSaveId.LocalSaveId == localSaveId);
			if (memberLoadData == null)
			{
				return null;
			}
			return memberLoadData.GetDataToUse();
		}

		public object GetFieldValueBySaveId(int localSaveId)
		{
			FieldLoadData fieldLoadData = this._fieldValues.SingleOrDefault((FieldLoadData value) => (int)value.MemberSaveId.LocalSaveId == localSaveId);
			if (fieldLoadData == null)
			{
				return null;
			}
			return fieldLoadData.GetDataToUse();
		}

		public object GetPropertyValueBySaveId(int localSaveId)
		{
			PropertyLoadData propertyLoadData = this._propertyValues.SingleOrDefault((PropertyLoadData value) => (int)value.MemberSaveId.LocalSaveId == localSaveId);
			if (propertyLoadData == null)
			{
				return null;
			}
			return propertyLoadData.GetDataToUse();
		}

		public bool HasMember(int localSaveId)
		{
			return this._memberValues.Any((MemberLoadData x) => (int)x.MemberSaveId.LocalSaveId == localSaveId);
		}

		public ObjectLoadData(LoadContext context, int id)
		{
			this.Context = context;
			this.Id = id;
			this._propertyValues = new List<PropertyLoadData>();
			this._fieldValues = new List<FieldLoadData>();
			this._memberValues = new List<MemberLoadData>();
			this._childStructs = new List<ObjectLoadData>();
		}

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

		public void FillCreatedObject()
		{
			foreach (ObjectLoadData objectLoadData in this._childStructs)
			{
				objectLoadData.CreateStruct();
			}
		}

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

		private short _propertyCount;

		private List<PropertyLoadData> _propertyValues;

		private List<FieldLoadData> _fieldValues;

		private List<MemberLoadData> _memberValues;

		private SaveId _saveId;

		private List<ObjectLoadData> _childStructs;

		private short _childStructCount;
	}
}
