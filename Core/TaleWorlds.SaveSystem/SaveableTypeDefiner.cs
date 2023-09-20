using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.SaveSystem.Definition;
using TaleWorlds.SaveSystem.Resolvers;

namespace TaleWorlds.SaveSystem
{
	public abstract class SaveableTypeDefiner
	{
		protected SaveableTypeDefiner(int saveBaseId)
		{
			this._saveBaseId = saveBaseId;
		}

		internal void Initialize(DefinitionContext definitionContext)
		{
			this._definitionContext = definitionContext;
		}

		protected internal virtual void DefineBasicTypes()
		{
		}

		protected internal virtual void DefineClassTypes()
		{
		}

		protected internal virtual void DefineStructTypes()
		{
		}

		protected internal virtual void DefineInterfaceTypes()
		{
		}

		protected internal virtual void DefineEnumTypes()
		{
		}

		protected internal virtual void DefineRootClassTypes()
		{
		}

		protected internal virtual void DefineGenericClassDefinitions()
		{
		}

		protected internal virtual void DefineGenericStructDefinitions()
		{
		}

		protected internal virtual void DefineContainerDefinitions()
		{
		}

		protected void ConstructGenericClassDefinition(Type type)
		{
			this._definitionContext.ConstructGenericClassDefinition(type);
		}

		protected void ConstructGenericStructDefinition(Type type)
		{
			this._definitionContext.ConstructGenericStructDefinition(type);
		}

		protected void AddBasicTypeDefinition(Type type, int saveId, IBasicTypeSerializer serializer)
		{
			BasicTypeDefinition basicTypeDefinition = new BasicTypeDefinition(type, this._saveBaseId + saveId, serializer);
			this._definitionContext.AddBasicTypeDefinition(basicTypeDefinition);
		}

		protected void AddClassDefinition(Type type, int saveId, IObjectResolver resolver = null)
		{
			TypeDefinition typeDefinition = new TypeDefinition(type, this._saveBaseId + saveId, resolver);
			this._definitionContext.AddClassDefinition(typeDefinition);
		}

		protected void AddClassDefinitionWithCustomFields(Type type, int saveId, IEnumerable<Tuple<string, short>> fields, IObjectResolver resolver = null)
		{
			TypeDefinition typeDefinition = new TypeDefinition(type, this._saveBaseId + saveId, resolver);
			this._definitionContext.AddClassDefinition(typeDefinition);
			foreach (Tuple<string, short> tuple in fields)
			{
				typeDefinition.AddCustomField(tuple.Item1, tuple.Item2);
			}
		}

		protected void AddStructDefinitionWithCustomFields(Type type, int saveId, IEnumerable<Tuple<string, short>> fields, IObjectResolver resolver = null)
		{
			TypeDefinition typeDefinition = new TypeDefinition(type, this._saveBaseId + saveId, resolver);
			this._definitionContext.AddStructDefinition(typeDefinition);
			foreach (Tuple<string, short> tuple in fields)
			{
				typeDefinition.AddCustomField(tuple.Item1, tuple.Item2);
			}
		}

		protected void AddRootClassDefinition(Type type, int saveId, IObjectResolver resolver = null)
		{
			TypeDefinition typeDefinition = new TypeDefinition(type, this._saveBaseId + saveId, resolver);
			this._definitionContext.AddRootClassDefinition(typeDefinition);
		}

		protected void AddStructDefinition(Type type, int saveId, IObjectResolver resolver = null)
		{
			TypeDefinition typeDefinition = new TypeDefinition(type, this._saveBaseId + saveId, resolver);
			this._definitionContext.AddStructDefinition(typeDefinition);
		}

		protected void AddInterfaceDefinition(Type type, int saveId)
		{
			InterfaceDefinition interfaceDefinition = new InterfaceDefinition(type, this._saveBaseId + saveId);
			this._definitionContext.AddInterfaceDefinition(interfaceDefinition);
		}

		protected void AddEnumDefinition(Type type, int saveId, IEnumResolver enumResolver = null)
		{
			EnumDefinition enumDefinition = new EnumDefinition(type, this._saveBaseId + saveId, enumResolver);
			this._definitionContext.AddEnumDefinition(enumDefinition);
		}

		protected void ConstructContainerDefinition(Type type)
		{
			if (!this._definitionContext.HasDefinition(type))
			{
				Assembly assembly = base.GetType().Assembly;
				this._definitionContext.ConstructContainerDefinition(type, assembly);
			}
		}

		private DefinitionContext _definitionContext;

		private readonly int _saveBaseId;
	}
}
