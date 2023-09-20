using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	internal abstract class VariableSaveData
	{
		public ISaveContext Context { get; private set; }

		public SavedMemberType MemberType { get; private set; }

		public object Value { get; private set; }

		public MemberTypeId MemberSaveId { get; private set; }

		public TypeDefinitionBase TypeDefinition { get; private set; }

		protected VariableSaveData(ISaveContext context)
		{
			this.Context = context;
		}

		protected void InitializeDataAsNullObject(MemberTypeId memberSaveId)
		{
			this.MemberSaveId = memberSaveId;
			this.MemberType = SavedMemberType.Object;
			this.Value = -1;
		}

		protected void InitializeDataAsCustomStruct(MemberTypeId memberSaveId, int structId, TypeDefinitionBase typeDefinition)
		{
			this.MemberSaveId = memberSaveId;
			this.MemberType = SavedMemberType.CustomStruct;
			this.Value = structId;
			this.TypeDefinition = typeDefinition;
		}

		protected void InitializeData(MemberTypeId memberSaveId, Type memberType, TypeDefinitionBase definition, object data)
		{
			this.MemberSaveId = memberSaveId;
			this.TypeDefinition = definition;
			TypeDefinition typeDefinition = this.TypeDefinition as TypeDefinition;
			if (this.TypeDefinition is ContainerDefinition)
			{
				int num = -1;
				if (data != null)
				{
					num = this.Context.GetContainerId(data);
				}
				this.MemberType = SavedMemberType.Container;
				this.Value = num;
			}
			else if (typeof(string) == memberType)
			{
				this.MemberType = SavedMemberType.String;
				this.Value = data;
			}
			else if ((typeDefinition != null && typeDefinition.IsClassDefinition) || this.TypeDefinition is InterfaceDefinition || (this.TypeDefinition == null && memberType.IsInterface))
			{
				int num2 = -1;
				if (data != null)
				{
					num2 = this.Context.GetObjectId(data);
				}
				this.MemberType = SavedMemberType.Object;
				this.Value = num2;
			}
			else if (this.TypeDefinition is EnumDefinition)
			{
				this.MemberType = SavedMemberType.Enum;
				this.Value = data;
			}
			else if (this.TypeDefinition is BasicTypeDefinition)
			{
				this.MemberType = SavedMemberType.BasicType;
				this.Value = data;
			}
			else
			{
				this.MemberType = SavedMemberType.CustomStruct;
				this.Value = data;
			}
			if (this.TypeDefinition == null && !memberType.IsInterface)
			{
				string text = string.Format("Cant find definition for: {0}. Save id: {1}", memberType.Name, this.MemberSaveId);
				Debug.Print(text, 0, Debug.DebugColor.Red, 17592186044416UL);
				Debug.FailedAssert(text, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\Save\\VariableSaveData.cs", "InitializeData", 97);
			}
		}

		public void SaveTo(IWriter writer)
		{
			writer.WriteByte((byte)this.MemberType);
			writer.WriteByte(this.MemberSaveId.TypeLevel);
			writer.WriteShort(this.MemberSaveId.LocalSaveId);
			if (this.MemberType == SavedMemberType.Object)
			{
				writer.WriteInt((int)this.Value);
				return;
			}
			if (this.MemberType == SavedMemberType.Container)
			{
				writer.WriteInt((int)this.Value);
				return;
			}
			if (this.MemberType == SavedMemberType.String)
			{
				int stringId = this.Context.GetStringId((string)this.Value);
				writer.WriteInt(stringId);
				return;
			}
			if (this.MemberType == SavedMemberType.Enum)
			{
				this.TypeDefinition.SaveId.WriteTo(writer);
				writer.WriteString(this.Value.ToString());
				return;
			}
			if (this.MemberType == SavedMemberType.BasicType)
			{
				this.TypeDefinition.SaveId.WriteTo(writer);
				((BasicTypeDefinition)this.TypeDefinition).Serializer.Serialize(writer, this.Value);
				return;
			}
			if (this.MemberType == SavedMemberType.CustomStruct)
			{
				writer.WriteInt((int)this.Value);
			}
		}
	}
}
