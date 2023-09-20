using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
	// Token: 0x0200002F RID: 47
	internal abstract class VariableSaveData
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x000082F4 File Offset: 0x000064F4
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x000082FC File Offset: 0x000064FC
		public ISaveContext Context { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x00008305 File Offset: 0x00006505
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x0000830D File Offset: 0x0000650D
		public SavedMemberType MemberType { get; private set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x00008316 File Offset: 0x00006516
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000831E File Offset: 0x0000651E
		public object Value { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x00008327 File Offset: 0x00006527
		// (set) Token: 0x060001BA RID: 442 RVA: 0x0000832F File Offset: 0x0000652F
		public MemberTypeId MemberSaveId { get; private set; }

		// Token: 0x060001BB RID: 443 RVA: 0x00008338 File Offset: 0x00006538
		protected VariableSaveData(ISaveContext context)
		{
			this.Context = context;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00008347 File Offset: 0x00006547
		protected void InitializeDataAsNullObject(MemberTypeId memberSaveId)
		{
			this.MemberSaveId = memberSaveId;
			this.MemberType = SavedMemberType.Object;
			this.Value = -1;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00008363 File Offset: 0x00006563
		protected void InitializeDataAsCustomStruct(MemberTypeId memberSaveId, int structId)
		{
			this.MemberSaveId = memberSaveId;
			this.MemberType = SavedMemberType.CustomStruct;
			this.Value = structId;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00008380 File Offset: 0x00006580
		protected void InitializeData(MemberTypeId memberSaveId, Type memberType, TypeDefinitionBase definition, object data)
		{
			this.MemberSaveId = memberSaveId;
			this._typeDefinition = definition;
			TypeDefinition typeDefinition = this._typeDefinition as TypeDefinition;
			if (this._typeDefinition is ContainerDefinition)
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
			else if ((typeDefinition != null && typeDefinition.IsClassDefinition) || this._typeDefinition is InterfaceDefinition || (this._typeDefinition == null && memberType.IsInterface))
			{
				int num2 = -1;
				if (data != null)
				{
					num2 = this.Context.GetObjectId(data);
				}
				this.MemberType = SavedMemberType.Object;
				this.Value = num2;
			}
			else if (this._typeDefinition is EnumDefinition)
			{
				this.MemberType = SavedMemberType.Enum;
				this.Value = data;
			}
			else if (this._typeDefinition is BasicTypeDefinition)
			{
				this.MemberType = SavedMemberType.BasicType;
				this.Value = data;
			}
			else
			{
				this.MemberType = SavedMemberType.CustomStruct;
				this.Value = data;
			}
			if (this._typeDefinition == null && !memberType.IsInterface)
			{
				string text = string.Format("Cant find definition for: {0}. Save id: {1}", memberType.Name, this.MemberSaveId);
				Debug.Print(text, 0, Debug.DebugColor.Red, 17592186044416UL);
				Debug.FailedAssert(text, "C:\\Develop\\MB3\\TaleWorlds.Shared\\Source\\Base\\TaleWorlds.SaveSystem\\Save\\VariableSaveData.cs", "InitializeData", 96);
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x000084F0 File Offset: 0x000066F0
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
				this._typeDefinition.SaveId.WriteTo(writer);
				writer.WriteString(this.Value.ToString());
				return;
			}
			if (this.MemberType == SavedMemberType.BasicType)
			{
				this._typeDefinition.SaveId.WriteTo(writer);
				((BasicTypeDefinition)this._typeDefinition).Serializer.Serialize(writer, this.Value);
				return;
			}
			if (this.MemberType == SavedMemberType.CustomStruct)
			{
				writer.WriteInt((int)this.Value);
			}
		}

		// Token: 0x04000084 RID: 132
		private TypeDefinitionBase _typeDefinition;
	}
}
