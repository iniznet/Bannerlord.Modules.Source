using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	internal class ObjectSaveData
	{
		public int ObjectId { get; private set; }

		public ISaveContext Context { get; private set; }

		public object Target { get; private set; }

		public Type Type { get; private set; }

		public bool IsClass { get; private set; }

		internal int PropertyCount
		{
			get
			{
				return this._propertyValues.Count;
			}
		}

		internal int FieldCount
		{
			get
			{
				return this._fieldValues.Count;
			}
		}

		public ObjectSaveData(ISaveContext context, int objectId, object target, bool isClass)
		{
			this.ObjectId = objectId;
			this.Context = context;
			this.Target = target;
			this.IsClass = isClass;
			this._stringMembers = new List<MemberSaveData>();
			this.Type = target.GetType();
			if (this.IsClass)
			{
				this._typeDefinition = context.DefinitionContext.GetClassDefinition(this.Type);
			}
			else
			{
				this._typeDefinition = context.DefinitionContext.GetStructDefinition(this.Type);
			}
			this._childStructs = new Dictionary<MemberDefinition, ObjectSaveData>(3);
			this._propertyValues = new Dictionary<PropertyInfo, PropertySaveData>(this._typeDefinition.PropertyDefinitions.Count);
			this._fieldValues = new Dictionary<FieldInfo, FieldSaveData>(this._typeDefinition.FieldDefinitions.Count);
			if (this._typeDefinition == null)
			{
				throw new Exception("Could not find type definition of type: " + this.Type);
			}
		}

		public void CollectMembers()
		{
			for (int i = 0; i < this._typeDefinition.MemberDefinitions.Count; i++)
			{
				MemberDefinition memberDefinition = this._typeDefinition.MemberDefinitions[i];
				MemberSaveData memberSaveData = null;
				if (memberDefinition is PropertyDefinition)
				{
					PropertyDefinition propertyDefinition = (PropertyDefinition)memberDefinition;
					PropertyInfo propertyInfo = propertyDefinition.PropertyInfo;
					MemberTypeId id = propertyDefinition.Id;
					PropertySaveData propertySaveData = new PropertySaveData(this, propertyDefinition, id);
					this._propertyValues.Add(propertyInfo, propertySaveData);
					memberSaveData = propertySaveData;
				}
				else if (memberDefinition is FieldDefinition)
				{
					FieldDefinition fieldDefinition = (FieldDefinition)memberDefinition;
					FieldInfo fieldInfo = fieldDefinition.FieldInfo;
					MemberTypeId id2 = fieldDefinition.Id;
					FieldSaveData fieldSaveData = new FieldSaveData(this, fieldDefinition, id2);
					this._fieldValues.Add(fieldInfo, fieldSaveData);
					memberSaveData = fieldSaveData;
				}
				Type memberType = memberDefinition.GetMemberType();
				TypeDefinitionBase typeDefinition = this.Context.DefinitionContext.GetTypeDefinition(memberType);
				TypeDefinition typeDefinition2 = typeDefinition as TypeDefinition;
				if (typeDefinition2 != null && !typeDefinition2.IsClassDefinition)
				{
					ObjectSaveData objectSaveData = this._childStructs[memberDefinition];
					memberSaveData.InitializeAsCustomStruct(objectSaveData.ObjectId);
				}
				else
				{
					memberSaveData.Initialize(typeDefinition);
				}
				if (memberSaveData.MemberType == SavedMemberType.String)
				{
					this._stringMembers.Add(memberSaveData);
				}
			}
			foreach (ObjectSaveData objectSaveData2 in this._childStructs.Values)
			{
				objectSaveData2.CollectMembers();
			}
		}

		public void CollectStringsInto(List<string> collection)
		{
			for (int i = 0; i < this._stringMembers.Count; i++)
			{
				string text = (string)this._stringMembers[i].Value;
				collection.Add(text);
			}
			foreach (ObjectSaveData objectSaveData in this._childStructs.Values)
			{
				objectSaveData.CollectStringsInto(collection);
			}
		}

		public void CollectStrings()
		{
			for (int i = 0; i < this._stringMembers.Count; i++)
			{
				string text = (string)this._stringMembers[i].Value;
				this.Context.AddOrGetStringId(text);
			}
			foreach (ObjectSaveData objectSaveData in this._childStructs.Values)
			{
				objectSaveData.CollectStrings();
			}
		}

		public void CollectStructs()
		{
			int num = 0;
			for (int i = 0; i < this._typeDefinition.MemberDefinitions.Count; i++)
			{
				MemberDefinition memberDefinition = this._typeDefinition.MemberDefinitions[i];
				Type memberType = memberDefinition.GetMemberType();
				if (this.Context.DefinitionContext.GetStructDefinition(memberType) != null)
				{
					object value = memberDefinition.GetValue(this.Target);
					ObjectSaveData objectSaveData = new ObjectSaveData(this.Context, num, value, false);
					this._childStructs.Add(memberDefinition, objectSaveData);
					num++;
				}
			}
			foreach (ObjectSaveData objectSaveData2 in this._childStructs.Values)
			{
				objectSaveData2.CollectStructs();
			}
		}

		public void SaveHeaderTo(SaveEntryFolder parentFolder, IArchiveContext archiveContext)
		{
			SaveFolderExtension saveFolderExtension = (this.IsClass ? SaveFolderExtension.Object : SaveFolderExtension.Struct);
			SaveEntryFolder saveEntryFolder = archiveContext.CreateFolder(parentFolder, new FolderId(this.ObjectId, saveFolderExtension), 1);
			BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
			this._typeDefinition.SaveId.WriteTo(binaryWriter);
			binaryWriter.WriteShort((short)this._propertyValues.Count);
			binaryWriter.WriteShort((short)this._childStructs.Count);
			saveEntryFolder.CreateEntry(new EntryId(-1, SaveEntryExtension.Basics)).FillFrom(binaryWriter);
			BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
		}

		public void SaveTo(SaveEntryFolder parentFolder, IArchiveContext archiveContext)
		{
			SaveFolderExtension saveFolderExtension = (this.IsClass ? SaveFolderExtension.Object : SaveFolderExtension.Struct);
			int num = 1 + this._fieldValues.Values.Count + this._propertyValues.Values.Count;
			SaveEntryFolder saveEntryFolder = archiveContext.CreateFolder(parentFolder, new FolderId(this.ObjectId, saveFolderExtension), num);
			BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
			this._typeDefinition.SaveId.WriteTo(binaryWriter);
			binaryWriter.WriteShort((short)this._propertyValues.Count);
			binaryWriter.WriteShort((short)this._childStructs.Count);
			saveEntryFolder.CreateEntry(new EntryId(-1, SaveEntryExtension.Basics)).FillFrom(binaryWriter);
			BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
			int num2 = 0;
			foreach (VariableSaveData variableSaveData in this._fieldValues.Values)
			{
				BinaryWriter binaryWriter2 = BinaryWriterFactory.GetBinaryWriter();
				variableSaveData.SaveTo(binaryWriter2);
				saveEntryFolder.CreateEntry(new EntryId(num2, SaveEntryExtension.Field)).FillFrom(binaryWriter2);
				BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter2);
				num2++;
			}
			int num3 = 0;
			foreach (VariableSaveData variableSaveData2 in this._propertyValues.Values)
			{
				BinaryWriter binaryWriter3 = BinaryWriterFactory.GetBinaryWriter();
				variableSaveData2.SaveTo(binaryWriter3);
				saveEntryFolder.CreateEntry(new EntryId(num3, SaveEntryExtension.Property)).FillFrom(binaryWriter3);
				BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter3);
				num3++;
			}
			foreach (ObjectSaveData objectSaveData in this._childStructs.Values)
			{
				objectSaveData.SaveTo(saveEntryFolder, archiveContext);
			}
		}

		internal static void GetChildObjectFrom(ISaveContext context, object target, MemberDefinition memberDefinition, List<object> collectedObjects)
		{
			Type memberType = memberDefinition.GetMemberType();
			if (memberType.IsClass || memberType.IsInterface)
			{
				if (memberType != typeof(string))
				{
					object value = memberDefinition.GetValue(target);
					if (value != null)
					{
						collectedObjects.Add(value);
						return;
					}
				}
			}
			else
			{
				TypeDefinition structDefinition = context.DefinitionContext.GetStructDefinition(memberType);
				if (structDefinition != null)
				{
					object value2 = memberDefinition.GetValue(target);
					for (int i = 0; i < structDefinition.MemberDefinitions.Count; i++)
					{
						MemberDefinition memberDefinition2 = structDefinition.MemberDefinitions[i];
						ObjectSaveData.GetChildObjectFrom(context, value2, memberDefinition2, collectedObjects);
					}
				}
			}
		}

		public IEnumerable<object> GetChildObjects()
		{
			List<object> list = new List<object>();
			ObjectSaveData.GetChildObjects(this.Context, this._typeDefinition, this.Target, list);
			return list;
		}

		public static void GetChildObjects(ISaveContext context, TypeDefinition typeDefinition, object target, List<object> collectedObjects)
		{
			if (typeDefinition.CollectObjectsMethod != null)
			{
				typeDefinition.CollectObjectsMethod(target, collectedObjects);
				return;
			}
			for (int i = 0; i < typeDefinition.MemberDefinitions.Count; i++)
			{
				MemberDefinition memberDefinition = typeDefinition.MemberDefinitions[i];
				ObjectSaveData.GetChildObjectFrom(context, target, memberDefinition, collectedObjects);
			}
		}

		private Dictionary<PropertyInfo, PropertySaveData> _propertyValues;

		private Dictionary<FieldInfo, FieldSaveData> _fieldValues;

		private List<MemberSaveData> _stringMembers;

		private TypeDefinition _typeDefinition;

		private Dictionary<MemberDefinition, ObjectSaveData> _childStructs;
	}
}
