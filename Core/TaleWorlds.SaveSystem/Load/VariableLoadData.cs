using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	internal abstract class VariableLoadData
	{
		public LoadContext Context { get; private set; }

		public MemberTypeId MemberSaveId { get; private set; }

		public SavedMemberType SavedMemberType { get; private set; }

		public object Data { get; private set; }

		protected VariableLoadData(LoadContext context, IReader reader)
		{
			this.Context = context;
			this._reader = reader;
		}

		public void Read()
		{
			this.SavedMemberType = (SavedMemberType)this._reader.ReadByte();
			this.MemberSaveId = new MemberTypeId
			{
				TypeLevel = this._reader.ReadByte(),
				LocalSaveId = this._reader.ReadShort()
			};
			if (this.SavedMemberType == SavedMemberType.Object)
			{
				this.Data = this._reader.ReadInt();
				return;
			}
			if (this.SavedMemberType == SavedMemberType.Container)
			{
				this.Data = this._reader.ReadInt();
				return;
			}
			if (this.SavedMemberType == SavedMemberType.String)
			{
				this.Data = this._reader.ReadInt();
				return;
			}
			if (this.SavedMemberType == SavedMemberType.Enum)
			{
				this._saveId = SaveId.ReadSaveIdFrom(this._reader);
				this._typeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this._saveId);
				string text = this._reader.ReadString();
				EnumDefinition enumDefinition = (EnumDefinition)this._typeDefinition;
				if (((enumDefinition != null) ? enumDefinition.Resolver : null) != null)
				{
					this.Data = enumDefinition.Resolver.ResolveObject(text);
					return;
				}
				this.Data = text;
				return;
			}
			else
			{
				if (this.SavedMemberType == SavedMemberType.BasicType)
				{
					this._saveId = SaveId.ReadSaveIdFrom(this._reader);
					this._typeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this._saveId);
					BasicTypeDefinition basicTypeDefinition = (BasicTypeDefinition)this._typeDefinition;
					this.Data = basicTypeDefinition.Serializer.Deserialize(this._reader);
					return;
				}
				if (this.SavedMemberType == SavedMemberType.CustomStruct)
				{
					this.Data = this._reader.ReadInt();
				}
				return;
			}
		}

		public void SetCustomStructData(object customStructObject)
		{
			this._customStructObject = customStructObject;
		}

		public object GetDataToUse()
		{
			object obj = null;
			if (this.SavedMemberType == SavedMemberType.Object)
			{
				ObjectHeaderLoadData objectWithId = this.Context.GetObjectWithId((int)this.Data);
				if (objectWithId != null)
				{
					obj = objectWithId.Target;
				}
			}
			else if (this.SavedMemberType == SavedMemberType.Container)
			{
				ContainerHeaderLoadData containerWithId = this.Context.GetContainerWithId((int)this.Data);
				if (containerWithId != null)
				{
					obj = containerWithId.Target;
				}
			}
			else if (this.SavedMemberType == SavedMemberType.String)
			{
				int num = (int)this.Data;
				obj = this.Context.GetStringWithId(num);
			}
			else if (this.SavedMemberType == SavedMemberType.Enum)
			{
				if (this._typeDefinition == null)
				{
					obj = (string)this.Data;
				}
				else
				{
					Type type = this._typeDefinition.Type;
					if (Enum.IsDefined(type, this.Data))
					{
						obj = Enum.Parse(type, (string)this.Data);
					}
				}
			}
			else if (this.SavedMemberType == SavedMemberType.BasicType)
			{
				obj = this.Data;
			}
			else if (this.SavedMemberType == SavedMemberType.CustomStruct)
			{
				obj = this._customStructObject;
			}
			return obj;
		}

		private IReader _reader;

		private TypeDefinitionBase _typeDefinition;

		private SaveId _saveId;

		private object _customStructObject;
	}
}
