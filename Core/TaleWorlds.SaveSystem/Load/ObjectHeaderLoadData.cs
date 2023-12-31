﻿using System;
using System.Runtime.Serialization;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	public class ObjectHeaderLoadData
	{
		public int Id { get; private set; }

		public object LoadedObject { get; private set; }

		public object Target { get; private set; }

		public int PropertyCount { get; private set; }

		public int ChildStructCount { get; private set; }

		public TypeDefinition TypeDefinition { get; private set; }

		public LoadContext Context { get; private set; }

		public ObjectHeaderLoadData(LoadContext context, int id)
		{
			this.Context = context;
			this.Id = id;
		}

		public void InitialieReaders(SaveEntryFolder saveEntryFolder)
		{
			BinaryReader binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Basics)).GetBinaryReader();
			this._saveId = SaveId.ReadSaveIdFrom(binaryReader);
			this.PropertyCount = (int)binaryReader.ReadShort();
			this.ChildStructCount = (int)binaryReader.ReadShort();
		}

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

		public void AdvancedResolveObject(MetaData metaData, ObjectLoadData objectLoadData)
		{
			this.Target = this.TypeDefinition.AdvancedResolveObject(this.LoadedObject, metaData, objectLoadData);
		}

		public void ResolveObject()
		{
			this.Target = this.TypeDefinition.ResolveObject(this.LoadedObject);
		}

		private SaveId _saveId;
	}
}
