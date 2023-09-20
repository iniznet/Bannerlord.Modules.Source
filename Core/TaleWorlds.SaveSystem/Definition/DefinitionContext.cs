﻿using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
	public class DefinitionContext
	{
		public bool GotError
		{
			get
			{
				return this._errors.Count > 0;
			}
		}

		public IEnumerable<string> Errors
		{
			get
			{
				return this._errors.AsReadOnly();
			}
		}

		public DefinitionContext()
		{
			this._errors = new List<string>();
			this._rootClassDefinitions = new Dictionary<Type, TypeDefinition>();
			this._classDefinitions = new Dictionary<Type, TypeDefinition>();
			this._classDefinitionsWithId = new Dictionary<SaveId, TypeDefinition>();
			this._interfaceDefinitions = new Dictionary<Type, InterfaceDefinition>();
			this._interfaceDefinitionsWithId = new Dictionary<SaveId, InterfaceDefinition>();
			this._enumDefinitions = new Dictionary<Type, EnumDefinition>();
			this._enumDefinitionsWithId = new Dictionary<SaveId, EnumDefinition>();
			this._containerDefinitions = new Dictionary<Type, ContainerDefinition>();
			this._containerDefinitionsWithId = new Dictionary<SaveId, ContainerDefinition>();
			this._genericClassDefinitions = new Dictionary<Type, GenericTypeDefinition>();
			this._structDefinitions = new Dictionary<Type, TypeDefinition>();
			this._structDefinitionsWithId = new Dictionary<SaveId, TypeDefinition>();
			this._genericStructDefinitions = new Dictionary<Type, GenericTypeDefinition>();
			this._basicTypeDefinitions = new Dictionary<Type, BasicTypeDefinition>();
			this._basicTypeDefinitionsWithId = new Dictionary<SaveId, BasicTypeDefinition>();
			this._allTypeDefinitions = new Dictionary<Type, TypeDefinitionBase>();
			this._allTypeDefinitionsWithId = new Dictionary<SaveId, TypeDefinitionBase>();
			this._saveableTypeDefiners = new List<SaveableTypeDefiner>();
			this._autoGeneratedSaveManagers = new List<IAutoGeneratedSaveManager>();
		}

		internal void AddRootClassDefinition(TypeDefinition rootClassDefinition)
		{
			this._rootClassDefinitions.Add(rootClassDefinition.Type, rootClassDefinition);
			this._allTypeDefinitions.Add(rootClassDefinition.Type, rootClassDefinition);
			this._allTypeDefinitionsWithId.Add(rootClassDefinition.SaveId, rootClassDefinition);
		}

		internal void AddClassDefinition(TypeDefinition classDefinition)
		{
			this._classDefinitions.Add(classDefinition.Type, classDefinition);
			this._classDefinitionsWithId.Add(classDefinition.SaveId, classDefinition);
			this._allTypeDefinitions.Add(classDefinition.Type, classDefinition);
			this._allTypeDefinitionsWithId.Add(classDefinition.SaveId, classDefinition);
		}

		internal void AddStructDefinition(TypeDefinition structDefinition)
		{
			this._structDefinitions.Add(structDefinition.Type, structDefinition);
			this._structDefinitionsWithId.Add(structDefinition.SaveId, structDefinition);
			this._allTypeDefinitions.Add(structDefinition.Type, structDefinition);
			this._allTypeDefinitionsWithId.Add(structDefinition.SaveId, structDefinition);
		}

		internal void AddInterfaceDefinition(InterfaceDefinition interfaceDefinition)
		{
			this._interfaceDefinitions.Add(interfaceDefinition.Type, interfaceDefinition);
			this._interfaceDefinitionsWithId.Add(interfaceDefinition.SaveId, interfaceDefinition);
			this._allTypeDefinitions.Add(interfaceDefinition.Type, interfaceDefinition);
			this._allTypeDefinitionsWithId.Add(interfaceDefinition.SaveId, interfaceDefinition);
		}

		internal void AddEnumDefinition(EnumDefinition enumDefinition)
		{
			this._enumDefinitions.Add(enumDefinition.Type, enumDefinition);
			this._enumDefinitionsWithId.Add(enumDefinition.SaveId, enumDefinition);
			this._allTypeDefinitions.Add(enumDefinition.Type, enumDefinition);
			this._allTypeDefinitionsWithId.Add(enumDefinition.SaveId, enumDefinition);
		}

		internal void AddContainerDefinition(ContainerDefinition containerDefinition)
		{
			this._containerDefinitions.Add(containerDefinition.Type, containerDefinition);
			this._containerDefinitionsWithId.Add(containerDefinition.SaveId, containerDefinition);
			this._allTypeDefinitions.Add(containerDefinition.Type, containerDefinition);
			this._allTypeDefinitionsWithId.Add(containerDefinition.SaveId, containerDefinition);
		}

		internal void AddBasicTypeDefinition(BasicTypeDefinition basicTypeDefinition)
		{
			this._basicTypeDefinitions.Add(basicTypeDefinition.Type, basicTypeDefinition);
			this._basicTypeDefinitionsWithId.Add(basicTypeDefinition.SaveId, basicTypeDefinition);
			this._allTypeDefinitions.Add(basicTypeDefinition.Type, basicTypeDefinition);
			this._allTypeDefinitionsWithId.Add(basicTypeDefinition.SaveId, basicTypeDefinition);
		}

		private void AddGenericClassDefinition(GenericTypeDefinition genericClassDefinition)
		{
			this._genericClassDefinitions.Add(genericClassDefinition.Type, genericClassDefinition);
			this._allTypeDefinitions.Add(genericClassDefinition.Type, genericClassDefinition);
			this._allTypeDefinitionsWithId.Add(genericClassDefinition.SaveId, genericClassDefinition);
		}

		private void AddGenericStructDefinition(GenericTypeDefinition genericStructDefinition)
		{
			this._genericStructDefinitions.Add(genericStructDefinition.Type, genericStructDefinition);
			this._allTypeDefinitions.Add(genericStructDefinition.Type, genericStructDefinition);
			this._allTypeDefinitionsWithId.Add(genericStructDefinition.SaveId, genericStructDefinition);
		}

		public void FillWithCurrentTypes()
		{
			this._assemblies = this.GetSaveableAssemblies();
			foreach (Assembly assembly in this._assemblies)
			{
				this.CollectTypes(assembly);
			}
			foreach (SaveableTypeDefiner saveableTypeDefiner in this._saveableTypeDefiners)
			{
				saveableTypeDefiner.Initialize(this);
			}
			foreach (SaveableTypeDefiner saveableTypeDefiner2 in this._saveableTypeDefiners)
			{
				saveableTypeDefiner2.DefineBasicTypes();
			}
			foreach (SaveableTypeDefiner saveableTypeDefiner3 in this._saveableTypeDefiners)
			{
				saveableTypeDefiner3.DefineClassTypes();
			}
			foreach (SaveableTypeDefiner saveableTypeDefiner4 in this._saveableTypeDefiners)
			{
				saveableTypeDefiner4.DefineStructTypes();
			}
			foreach (SaveableTypeDefiner saveableTypeDefiner5 in this._saveableTypeDefiners)
			{
				saveableTypeDefiner5.DefineInterfaceTypes();
			}
			foreach (SaveableTypeDefiner saveableTypeDefiner6 in this._saveableTypeDefiners)
			{
				saveableTypeDefiner6.DefineEnumTypes();
			}
			foreach (SaveableTypeDefiner saveableTypeDefiner7 in this._saveableTypeDefiners)
			{
				saveableTypeDefiner7.DefineRootClassTypes();
			}
			foreach (SaveableTypeDefiner saveableTypeDefiner8 in this._saveableTypeDefiners)
			{
				saveableTypeDefiner8.DefineGenericStructDefinitions();
			}
			foreach (SaveableTypeDefiner saveableTypeDefiner9 in this._saveableTypeDefiners)
			{
				saveableTypeDefiner9.DefineGenericClassDefinitions();
			}
			foreach (SaveableTypeDefiner saveableTypeDefiner10 in this._saveableTypeDefiners)
			{
				saveableTypeDefiner10.DefineContainerDefinitions();
			}
			foreach (TypeDefinition typeDefinition in this._rootClassDefinitions.Values)
			{
				typeDefinition.CollectInitializationCallbacks();
				typeDefinition.CollectProperties();
				typeDefinition.CollectFields();
			}
			TWParallel.ForEach<TypeDefinition>(this._classDefinitions.Values, delegate(TypeDefinition classDefinition)
			{
				classDefinition.CollectInitializationCallbacks();
				classDefinition.CollectProperties();
				classDefinition.CollectFields();
			});
			foreach (TypeDefinition typeDefinition2 in this._classDefinitions.Values)
			{
				this._errors.AddRange(typeDefinition2.Errors);
			}
			TWParallel.ForEach<TypeDefinition>(this._structDefinitions.Values, delegate(TypeDefinition structDefinitions)
			{
				structDefinitions.CollectProperties();
				structDefinitions.CollectFields();
			});
			foreach (TypeDefinition typeDefinition3 in this._structDefinitions.Values)
			{
				this._errors.AddRange(typeDefinition3.Errors);
			}
			this.FindAutoGeneratedSaveManagers();
			this.InitializeAutoGeneratedSaveManagers();
		}

		private Assembly[] GetSaveableAssemblies()
		{
			List<Assembly> list = new List<Assembly>();
			Assembly assembly = typeof(SaveableRootClassAttribute).Assembly;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			list.Add(assembly);
			foreach (Assembly assembly2 in assemblies)
			{
				if (assembly2 != assembly)
				{
					AssemblyName[] referencedAssemblies = assembly2.GetReferencedAssemblies();
					for (int j = 0; j < referencedAssemblies.Length; j++)
					{
						if (referencedAssemblies[j].ToString() == assembly.GetName().ToString())
						{
							list.Add(assembly2);
							break;
						}
					}
				}
			}
			return list.ToArray();
		}

		private void CollectTypes(Assembly assembly)
		{
			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(SaveableTypeDefiner).IsAssignableFrom(type) && !type.IsAbstract)
				{
					SaveableTypeDefiner saveableTypeDefiner = (SaveableTypeDefiner)Activator.CreateInstance(type);
					this._saveableTypeDefiners.Add(saveableTypeDefiner);
				}
			}
		}

		internal TypeDefinitionBase GetTypeDefinition(Type type)
		{
			TypeDefinitionBase typeDefinitionBase;
			if (this._allTypeDefinitions.TryGetValue(type, out typeDefinitionBase))
			{
				return typeDefinitionBase;
			}
			return null;
		}

		internal TypeDefinition GetClassDefinition(Type type)
		{
			if (type.IsContainer())
			{
				return null;
			}
			TypeDefinition typeDefinition;
			if (this._rootClassDefinitions.TryGetValue(type, out typeDefinition))
			{
				return typeDefinition;
			}
			GenericTypeDefinition genericTypeDefinition;
			if (this._genericClassDefinitions.TryGetValue(type, out genericTypeDefinition))
			{
				return genericTypeDefinition;
			}
			TypeDefinition typeDefinition2;
			if (this._classDefinitions.TryGetValue(type, out typeDefinition2))
			{
				return typeDefinition2;
			}
			return null;
		}

		public TypeDefinitionBase TryGetTypeDefinition(SaveId saveId)
		{
			TypeDefinitionBase typeDefinitionBase;
			if (this._allTypeDefinitionsWithId.TryGetValue(saveId, out typeDefinitionBase))
			{
				return typeDefinitionBase;
			}
			if (saveId is GenericSaveId)
			{
				GenericSaveId genericSaveId = (GenericSaveId)saveId;
				SaveId baseId = genericSaveId.BaseId;
				TypeDefinition typeDefinition = this.TryGetTypeDefinition(baseId) as TypeDefinition;
				if (typeDefinition != null)
				{
					TypeDefinitionBase[] array = new TypeDefinitionBase[genericSaveId.GenericTypeIDs.Length];
					for (int i = 0; i < genericSaveId.GenericTypeIDs.Length; i++)
					{
						SaveId saveId2 = genericSaveId.GenericTypeIDs[i];
						TypeDefinitionBase typeDefinitionBase2 = this.TryGetTypeDefinition(saveId2);
						if (typeDefinitionBase2 == null)
						{
							return null;
						}
						array[i] = typeDefinitionBase2;
					}
					Type type = this.ConstructTypeFrom(typeDefinition, array);
					if (type != null)
					{
						GenericTypeDefinition genericTypeDefinition = new GenericTypeDefinition(type, genericSaveId);
						genericTypeDefinition.CollectInitializationCallbacks();
						genericTypeDefinition.CollectFields();
						genericTypeDefinition.CollectProperties();
						if (genericTypeDefinition.IsClassDefinition)
						{
							if (!this._allTypeDefinitions.ContainsKey(genericTypeDefinition.Type))
							{
								this.AddGenericClassDefinition(genericTypeDefinition);
							}
						}
						else
						{
							this.AddGenericStructDefinition(genericTypeDefinition);
						}
						return genericTypeDefinition;
					}
				}
			}
			return null;
		}

		internal GenericTypeDefinition ConstructGenericClassDefinition(Type type)
		{
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			TypeDefinition classDefinition = this.GetClassDefinition(genericTypeDefinition);
			TypeSaveId typeSaveId = (TypeSaveId)classDefinition.SaveId;
			SaveId[] array = new SaveId[type.GenericTypeArguments.Length];
			for (int i = 0; i < type.GenericTypeArguments.Length; i++)
			{
				Type type2 = type.GenericTypeArguments[i];
				TypeDefinitionBase typeDefinition = this.GetTypeDefinition(type2);
				array[i] = typeDefinition.SaveId;
			}
			GenericSaveId genericSaveId = new GenericSaveId(typeSaveId, array);
			GenericTypeDefinition genericTypeDefinition2 = new GenericTypeDefinition(type, genericSaveId);
			foreach (CustomField customField in classDefinition.CustomFields)
			{
				genericTypeDefinition2.AddCustomField(customField.Name, customField.SaveId);
			}
			genericTypeDefinition2.CollectInitializationCallbacks();
			genericTypeDefinition2.CollectFields();
			genericTypeDefinition2.CollectProperties();
			this.AddGenericClassDefinition(genericTypeDefinition2);
			return genericTypeDefinition2;
		}

		internal bool HasDefinition(Type type)
		{
			return this._allTypeDefinitions.ContainsKey(type);
		}

		internal ContainerDefinition ConstructContainerDefinition(Type type, Assembly definedAssembly)
		{
			ContainerType containerType;
			type.IsContainer(out containerType);
			SaveId saveId = null;
			SaveId saveId2 = null;
			if (containerType == ContainerType.List || containerType == ContainerType.CustomList || containerType == ContainerType.CustomReadOnlyList || containerType == ContainerType.Queue)
			{
				Type type2 = type.GenericTypeArguments[0];
				saveId = this.GetTypeDefinition(type2).SaveId;
			}
			else if (containerType == ContainerType.Dictionary)
			{
				Type type3 = type.GenericTypeArguments[0];
				saveId = this.GetTypeDefinition(type3).SaveId;
				Type type4 = type.GenericTypeArguments[1];
				saveId2 = this.GetTypeDefinition(type4).SaveId;
			}
			else if (containerType == ContainerType.Array)
			{
				Type elementType = type.GetElementType();
				saveId = this.GetTypeDefinition(elementType).SaveId;
			}
			ContainerSaveId containerSaveId = new ContainerSaveId(containerType, saveId, saveId2);
			ContainerDefinition containerDefinition = new ContainerDefinition(type, containerSaveId, definedAssembly);
			this.AddContainerDefinition(containerDefinition);
			if (containerType == ContainerType.List)
			{
				this.AddContainerDefinition(new ContainerDefinition(typeof(MBList<>).MakeGenericType(type.GetGenericArguments()), new ContainerSaveId(ContainerType.CustomList, saveId, saveId2), definedAssembly));
				this.AddContainerDefinition(new ContainerDefinition(typeof(MBReadOnlyList<>).MakeGenericType(type.GetGenericArguments()), new ContainerSaveId(ContainerType.CustomReadOnlyList, saveId, saveId2), definedAssembly));
			}
			return containerDefinition;
		}

		private Type ConstructTypeFrom(TypeDefinition baseClassDefinition, TypeDefinitionBase[] parameterDefinitions)
		{
			Type type = baseClassDefinition.Type;
			Type[] array = new Type[parameterDefinitions.Length];
			for (int i = 0; i < parameterDefinitions.Length; i++)
			{
				Type type2 = parameterDefinitions[i].Type;
				if (type2 == null)
				{
					return null;
				}
				array[i] = type2;
			}
			return type.MakeGenericType(array);
		}

		internal TypeDefinition GetStructDefinition(Type type)
		{
			GenericTypeDefinition genericTypeDefinition;
			if (this._genericStructDefinitions.TryGetValue(type, out genericTypeDefinition))
			{
				return genericTypeDefinition;
			}
			TypeDefinition typeDefinition;
			if (this._structDefinitions.TryGetValue(type, out typeDefinition))
			{
				return typeDefinition;
			}
			return null;
		}

		internal InterfaceDefinition GetInterfaceDefinition(Type type)
		{
			InterfaceDefinition interfaceDefinition;
			this._interfaceDefinitions.TryGetValue(type, out interfaceDefinition);
			return interfaceDefinition;
		}

		internal EnumDefinition GetEnumDefinition(Type type)
		{
			EnumDefinition enumDefinition;
			this._enumDefinitions.TryGetValue(type, out enumDefinition);
			return enumDefinition;
		}

		internal ContainerDefinition GetContainerDefinition(Type type)
		{
			ContainerDefinition containerDefinition;
			this._containerDefinitions.TryGetValue(type, out containerDefinition);
			return containerDefinition;
		}

		internal GenericTypeDefinition ConstructGenericStructDefinition(Type type)
		{
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			TypeDefinition structDefinition = this.GetStructDefinition(genericTypeDefinition);
			TypeSaveId typeSaveId = (TypeSaveId)structDefinition.SaveId;
			SaveId[] array = new SaveId[type.GenericTypeArguments.Length];
			for (int i = 0; i < type.GenericTypeArguments.Length; i++)
			{
				Type type2 = type.GenericTypeArguments[i];
				TypeDefinitionBase typeDefinition = this.GetTypeDefinition(type2);
				array[i] = typeDefinition.SaveId;
			}
			GenericSaveId genericSaveId = new GenericSaveId(typeSaveId, array);
			GenericTypeDefinition genericTypeDefinition2 = new GenericTypeDefinition(type, genericSaveId);
			foreach (CustomField customField in structDefinition.CustomFields)
			{
				genericTypeDefinition2.AddCustomField(customField.Name, customField.SaveId);
			}
			genericTypeDefinition2.CollectFields();
			genericTypeDefinition2.CollectProperties();
			this.AddGenericStructDefinition(genericTypeDefinition2);
			return genericTypeDefinition2;
		}

		internal BasicTypeDefinition GetBasicTypeDefinition(Type type)
		{
			BasicTypeDefinition basicTypeDefinition;
			this._basicTypeDefinitions.TryGetValue(type, out basicTypeDefinition);
			return basicTypeDefinition;
		}

		private void FindAutoGeneratedSaveManagers()
		{
			Assembly[] assemblies = this._assemblies;
			for (int i = 0; i < assemblies.Length; i++)
			{
				foreach (Type type in assemblies[i].GetTypes())
				{
					if (typeof(IAutoGeneratedSaveManager).IsAssignableFrom(type) && typeof(IAutoGeneratedSaveManager) != type)
					{
						IAutoGeneratedSaveManager autoGeneratedSaveManager = (IAutoGeneratedSaveManager)Activator.CreateInstance(type);
						this._autoGeneratedSaveManagers.Add(autoGeneratedSaveManager);
					}
				}
			}
		}

		private void InitializeAutoGeneratedSaveManagers()
		{
			foreach (IAutoGeneratedSaveManager autoGeneratedSaveManager in this._autoGeneratedSaveManagers)
			{
				autoGeneratedSaveManager.Initialize(this);
			}
		}

		public void GenerateCode(SaveCodeGenerationContext context)
		{
			foreach (TypeDefinition typeDefinition in this._classDefinitions.Values)
			{
				Assembly assembly = typeDefinition.Type.Assembly;
				SaveCodeGenerationContextAssembly saveCodeGenerationContextAssembly = context.FindAssemblyInformation(assembly);
				if (saveCodeGenerationContextAssembly != null)
				{
					saveCodeGenerationContextAssembly.AddClassDefinition(typeDefinition);
				}
			}
			foreach (TypeDefinition typeDefinition2 in this._structDefinitions.Values)
			{
				Assembly assembly2 = typeDefinition2.Type.Assembly;
				SaveCodeGenerationContextAssembly saveCodeGenerationContextAssembly2 = context.FindAssemblyInformation(assembly2);
				if (saveCodeGenerationContextAssembly2 != null)
				{
					saveCodeGenerationContextAssembly2.AddStructDefinition(typeDefinition2);
				}
			}
			foreach (ContainerDefinition containerDefinition in this._containerDefinitions.Values)
			{
				Assembly definedAssembly = containerDefinition.DefinedAssembly;
				SaveCodeGenerationContextAssembly saveCodeGenerationContextAssembly3 = context.FindAssemblyInformation(definedAssembly);
				if (saveCodeGenerationContextAssembly3 != null)
				{
					saveCodeGenerationContextAssembly3.AddContainerDefinition(containerDefinition);
				}
			}
			context.FillFiles();
		}

		private Dictionary<Type, TypeDefinition> _rootClassDefinitions;

		private Dictionary<Type, TypeDefinition> _classDefinitions;

		private Dictionary<SaveId, TypeDefinition> _classDefinitionsWithId;

		private Dictionary<Type, InterfaceDefinition> _interfaceDefinitions;

		private Dictionary<SaveId, InterfaceDefinition> _interfaceDefinitionsWithId;

		private Dictionary<Type, EnumDefinition> _enumDefinitions;

		private Dictionary<SaveId, EnumDefinition> _enumDefinitionsWithId;

		private Dictionary<Type, ContainerDefinition> _containerDefinitions;

		private Dictionary<SaveId, ContainerDefinition> _containerDefinitionsWithId;

		private Dictionary<Type, GenericTypeDefinition> _genericClassDefinitions;

		private Dictionary<Type, TypeDefinition> _structDefinitions;

		private Dictionary<SaveId, TypeDefinition> _structDefinitionsWithId;

		private Dictionary<Type, GenericTypeDefinition> _genericStructDefinitions;

		private Dictionary<Type, BasicTypeDefinition> _basicTypeDefinitions;

		private Dictionary<SaveId, BasicTypeDefinition> _basicTypeDefinitionsWithId;

		private Dictionary<Type, TypeDefinitionBase> _allTypeDefinitions;

		private Dictionary<SaveId, TypeDefinitionBase> _allTypeDefinitionsWithId;

		private List<IAutoGeneratedSaveManager> _autoGeneratedSaveManagers;

		private Assembly[] _assemblies;

		public List<string> _errors;

		private List<SaveableTypeDefiner> _saveableTypeDefiners;
	}
}
