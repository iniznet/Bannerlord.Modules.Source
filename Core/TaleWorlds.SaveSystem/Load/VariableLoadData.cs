using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
	// Token: 0x0200003E RID: 62
	internal abstract class VariableLoadData
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000238 RID: 568 RVA: 0x0000A14E File Offset: 0x0000834E
		// (set) Token: 0x06000239 RID: 569 RVA: 0x0000A156 File Offset: 0x00008356
		public LoadContext Context { get; private set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600023A RID: 570 RVA: 0x0000A15F File Offset: 0x0000835F
		// (set) Token: 0x0600023B RID: 571 RVA: 0x0000A167 File Offset: 0x00008367
		public MemberTypeId MemberSaveId { get; private set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600023C RID: 572 RVA: 0x0000A170 File Offset: 0x00008370
		// (set) Token: 0x0600023D RID: 573 RVA: 0x0000A178 File Offset: 0x00008378
		public SavedMemberType SavedMemberType { get; private set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600023E RID: 574 RVA: 0x0000A181 File Offset: 0x00008381
		// (set) Token: 0x0600023F RID: 575 RVA: 0x0000A189 File Offset: 0x00008389
		public object Data { get; private set; }

		// Token: 0x06000240 RID: 576 RVA: 0x0000A192 File Offset: 0x00008392
		protected VariableLoadData(LoadContext context, IReader reader)
		{
			this.Context = context;
			this._reader = reader;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000A1A8 File Offset: 0x000083A8
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

		// Token: 0x06000242 RID: 578 RVA: 0x0000A348 File Offset: 0x00008548
		public void SetCustomStructData(object customStructObject)
		{
			this._customStructObject = customStructObject;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000A354 File Offset: 0x00008554
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

		// Token: 0x040000BA RID: 186
		private IReader _reader;

		// Token: 0x040000BF RID: 191
		private TypeDefinitionBase _typeDefinition;

		// Token: 0x040000C0 RID: 192
		private SaveId _saveId;

		// Token: 0x040000C1 RID: 193
		private object _customStructObject;
	}
}
