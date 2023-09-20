using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.SaveSystem.Definition;
using TaleWorlds.SaveSystem.Resolvers;

namespace TaleWorlds.SaveSystem
{
	// Token: 0x02000014 RID: 20
	public abstract class SaveableTypeDefiner
	{
		// Token: 0x06000060 RID: 96 RVA: 0x000035C9 File Offset: 0x000017C9
		protected SaveableTypeDefiner(int saveBaseId)
		{
			this._saveBaseId = saveBaseId;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000035D8 File Offset: 0x000017D8
		internal void Initialize(DefinitionContext definitionContext)
		{
			this._definitionContext = definitionContext;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000035E1 File Offset: 0x000017E1
		protected internal virtual void DefineBasicTypes()
		{
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000035E3 File Offset: 0x000017E3
		protected internal virtual void DefineClassTypes()
		{
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000035E5 File Offset: 0x000017E5
		protected internal virtual void DefineStructTypes()
		{
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000035E7 File Offset: 0x000017E7
		protected internal virtual void DefineInterfaceTypes()
		{
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000035E9 File Offset: 0x000017E9
		protected internal virtual void DefineEnumTypes()
		{
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000035EB File Offset: 0x000017EB
		protected internal virtual void DefineRootClassTypes()
		{
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000035ED File Offset: 0x000017ED
		protected internal virtual void DefineGenericClassDefinitions()
		{
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000035EF File Offset: 0x000017EF
		protected internal virtual void DefineGenericStructDefinitions()
		{
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000035F1 File Offset: 0x000017F1
		protected internal virtual void DefineContainerDefinitions()
		{
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000035F3 File Offset: 0x000017F3
		protected void ConstructGenericClassDefinition(Type type)
		{
			this._definitionContext.ConstructGenericClassDefinition(type);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003602 File Offset: 0x00001802
		protected void ConstructGenericStructDefinition(Type type)
		{
			this._definitionContext.ConstructGenericStructDefinition(type);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003614 File Offset: 0x00001814
		protected void AddBasicTypeDefinition(Type type, int saveId, IBasicTypeSerializer serializer)
		{
			BasicTypeDefinition basicTypeDefinition = new BasicTypeDefinition(type, this._saveBaseId + saveId, serializer);
			this._definitionContext.AddBasicTypeDefinition(basicTypeDefinition);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003640 File Offset: 0x00001840
		protected void AddClassDefinition(Type type, int saveId, IObjectResolver resolver = null)
		{
			TypeDefinition typeDefinition = new TypeDefinition(type, this._saveBaseId + saveId, resolver);
			this._definitionContext.AddClassDefinition(typeDefinition);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000366C File Offset: 0x0000186C
		protected void AddClassDefinitionWithCustomFields(Type type, int saveId, IEnumerable<Tuple<string, short>> fields, IObjectResolver resolver = null)
		{
			TypeDefinition typeDefinition = new TypeDefinition(type, this._saveBaseId + saveId, resolver);
			this._definitionContext.AddClassDefinition(typeDefinition);
			foreach (Tuple<string, short> tuple in fields)
			{
				typeDefinition.AddCustomField(tuple.Item1, tuple.Item2);
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000036DC File Offset: 0x000018DC
		protected void AddStructDefinitionWithCustomFields(Type type, int saveId, IEnumerable<Tuple<string, short>> fields, IObjectResolver resolver = null)
		{
			TypeDefinition typeDefinition = new TypeDefinition(type, this._saveBaseId + saveId, resolver);
			this._definitionContext.AddStructDefinition(typeDefinition);
			foreach (Tuple<string, short> tuple in fields)
			{
				typeDefinition.AddCustomField(tuple.Item1, tuple.Item2);
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000374C File Offset: 0x0000194C
		protected void AddRootClassDefinition(Type type, int saveId, IObjectResolver resolver = null)
		{
			TypeDefinition typeDefinition = new TypeDefinition(type, this._saveBaseId + saveId, resolver);
			this._definitionContext.AddRootClassDefinition(typeDefinition);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003778 File Offset: 0x00001978
		protected void AddStructDefinition(Type type, int saveId, IObjectResolver resolver = null)
		{
			TypeDefinition typeDefinition = new TypeDefinition(type, this._saveBaseId + saveId, resolver);
			this._definitionContext.AddStructDefinition(typeDefinition);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000037A4 File Offset: 0x000019A4
		protected void AddInterfaceDefinition(Type type, int saveId)
		{
			InterfaceDefinition interfaceDefinition = new InterfaceDefinition(type, this._saveBaseId + saveId);
			this._definitionContext.AddInterfaceDefinition(interfaceDefinition);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000037CC File Offset: 0x000019CC
		protected void AddEnumDefinition(Type type, int saveId, IEnumResolver enumResolver = null)
		{
			EnumDefinition enumDefinition = new EnumDefinition(type, this._saveBaseId + saveId, enumResolver);
			this._definitionContext.AddEnumDefinition(enumDefinition);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000037F8 File Offset: 0x000019F8
		protected void ConstructContainerDefinition(Type type)
		{
			if (!this._definitionContext.HasDefinition(type))
			{
				Assembly assembly = base.GetType().Assembly;
				this._definitionContext.ConstructContainerDefinition(type, assembly);
			}
		}

		// Token: 0x0400001A RID: 26
		private DefinitionContext _definitionContext;

		// Token: 0x0400001B RID: 27
		private readonly int _saveBaseId;
	}
}
