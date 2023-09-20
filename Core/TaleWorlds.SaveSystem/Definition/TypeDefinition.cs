using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.SaveSystem.Resolvers;

namespace TaleWorlds.SaveSystem.Definition
{
	public class TypeDefinition : TypeDefinitionBase
	{
		public List<MemberDefinition> MemberDefinitions { get; private set; }

		public IEnumerable<MethodInfo> InitializationCallbacks
		{
			get
			{
				return this._initializationCallbacks;
			}
		}

		public IEnumerable<MethodInfo> LateInitializationCallbacks
		{
			get
			{
				return this._lateInitializationCallbacks;
			}
		}

		public IEnumerable<string> Errors
		{
			get
			{
				return this._errors.AsReadOnly();
			}
		}

		public bool IsClassDefinition
		{
			get
			{
				return this._isClass;
			}
		}

		public List<CustomField> CustomFields { get; private set; }

		public CollectObjectsDelegate CollectObjectsMethod { get; private set; }

		public TypeDefinition(Type type, SaveId saveId, IObjectResolver objectResolver)
			: base(type, saveId)
		{
			this._isClass = base.Type.IsClass;
			this._errors = new List<string>();
			this._properties = new Dictionary<MemberTypeId, PropertyDefinition>();
			this._fields = new Dictionary<MemberTypeId, FieldDefinition>();
			this.MemberDefinitions = new List<MemberDefinition>();
			this.CustomFields = new List<CustomField>();
			this._initializationCallbacks = new List<MethodInfo>();
			this._lateInitializationCallbacks = new List<MethodInfo>();
			this._objectResolver = objectResolver;
		}

		public TypeDefinition(Type type, int saveId, IObjectResolver objectResolver)
			: this(type, new TypeSaveId(saveId), objectResolver)
		{
		}

		public bool CheckIfRequiresAdvancedResolving(object originalObject)
		{
			return this._objectResolver != null && this._objectResolver.CheckIfRequiresAdvancedResolving(originalObject);
		}

		public object ResolveObject(object originalObject)
		{
			if (this._objectResolver != null)
			{
				return this._objectResolver.ResolveObject(originalObject);
			}
			return originalObject;
		}

		public object AdvancedResolveObject(object originalObject, MetaData metaData, ObjectLoadData objectLoadData)
		{
			if (this._objectResolver != null)
			{
				return this._objectResolver.AdvancedResolveObject(originalObject, metaData, objectLoadData);
			}
			return originalObject;
		}

		public void CollectInitializationCallbacks()
		{
			Type type = base.Type;
			while (type != typeof(object))
			{
				foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (methodInfo.DeclaringType == type)
					{
						if (methodInfo.GetCustomAttributes(typeof(LoadInitializationCallback)).ToArray<Attribute>().Length != 0 && !this._initializationCallbacks.Contains(methodInfo))
						{
							this._initializationCallbacks.Insert(0, methodInfo);
						}
						if (methodInfo.GetCustomAttributes(typeof(LateLoadInitializationCallback)).ToArray<Attribute>().Length != 0 && !this._lateInitializationCallbacks.Contains(methodInfo))
						{
							this._lateInitializationCallbacks.Insert(0, methodInfo);
						}
					}
				}
				type = type.BaseType;
			}
		}

		public void CollectProperties()
		{
			foreach (PropertyInfo propertyInfo in base.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				Attribute[] array = propertyInfo.GetCustomAttributes(typeof(SaveablePropertyAttribute)).ToArray<Attribute>();
				if (array.Length != 0)
				{
					SaveablePropertyAttribute saveablePropertyAttribute = (SaveablePropertyAttribute)array[0];
					byte classLevel = TypeDefinitionBase.GetClassLevel(propertyInfo.DeclaringType);
					MemberTypeId memberTypeId = new MemberTypeId(classLevel, saveablePropertyAttribute.LocalSaveId);
					PropertyDefinition propertyDefinition = new PropertyDefinition(propertyInfo, memberTypeId);
					if (this._properties.ContainsKey(memberTypeId))
					{
						this._errors.Add(string.Concat(new object[]
						{
							"SaveId ",
							memberTypeId,
							" of property ",
							propertyDefinition.PropertyInfo.Name,
							" is already defined in type ",
							base.Type.FullName
						}));
					}
					else
					{
						this._properties.Add(memberTypeId, propertyDefinition);
						this.MemberDefinitions.Add(propertyDefinition);
					}
				}
			}
		}

		private static IEnumerable<FieldInfo> GetFieldsOfType(Type type)
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!fieldInfo.IsPrivate)
				{
					yield return fieldInfo;
				}
			}
			FieldInfo[] array = null;
			Type typeToCheck = type;
			while (typeToCheck != typeof(object))
			{
				FieldInfo[] fields2 = typeToCheck.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo2 in fields2)
				{
					if (fieldInfo2.IsPrivate)
					{
						yield return fieldInfo2;
					}
				}
				array = null;
				typeToCheck = typeToCheck.BaseType;
			}
			yield break;
		}

		public void CollectFields()
		{
			foreach (FieldInfo fieldInfo in TypeDefinition.GetFieldsOfType(base.Type).ToArray<FieldInfo>())
			{
				Attribute[] array2 = fieldInfo.GetCustomAttributes(typeof(SaveableFieldAttribute)).ToArray<Attribute>();
				if (array2.Length != 0)
				{
					SaveableFieldAttribute saveableFieldAttribute = (SaveableFieldAttribute)array2[0];
					byte classLevel = TypeDefinitionBase.GetClassLevel(fieldInfo.DeclaringType);
					MemberTypeId memberTypeId = new MemberTypeId(classLevel, saveableFieldAttribute.LocalSaveId);
					FieldDefinition fieldDefinition = new FieldDefinition(fieldInfo, memberTypeId);
					if (this._fields.ContainsKey(memberTypeId))
					{
						this._errors.Add(string.Concat(new object[]
						{
							"SaveId ",
							memberTypeId,
							" of field ",
							fieldDefinition.FieldInfo,
							" is already defined in type ",
							base.Type.FullName
						}));
					}
					else
					{
						this._fields.Add(memberTypeId, fieldDefinition);
						this.MemberDefinitions.Add(fieldDefinition);
					}
				}
			}
			foreach (CustomField customField in this.CustomFields)
			{
				string name = customField.Name;
				short saveId = customField.SaveId;
				FieldInfo field = base.Type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				byte classLevel2 = TypeDefinitionBase.GetClassLevel(field.DeclaringType);
				MemberTypeId memberTypeId2 = new MemberTypeId(classLevel2, saveId);
				FieldDefinition fieldDefinition2 = new FieldDefinition(field, memberTypeId2);
				if (this._fields.ContainsKey(memberTypeId2))
				{
					this._errors.Add(string.Concat(new object[]
					{
						"SaveId ",
						memberTypeId2,
						" of field ",
						fieldDefinition2.FieldInfo,
						" is already defined in type ",
						base.Type.FullName
					}));
				}
				else
				{
					this._fields.Add(memberTypeId2, fieldDefinition2);
					this.MemberDefinitions.Add(fieldDefinition2);
				}
			}
		}

		public void AddCustomField(string fieldName, short saveId)
		{
			this.CustomFields.Add(new CustomField(fieldName, saveId));
		}

		public PropertyDefinition GetPropertyDefinitionWithId(MemberTypeId id)
		{
			PropertyDefinition propertyDefinition;
			this._properties.TryGetValue(id, out propertyDefinition);
			return propertyDefinition;
		}

		public FieldDefinition GetFieldDefinitionWithId(MemberTypeId id)
		{
			FieldDefinition fieldDefinition;
			this._fields.TryGetValue(id, out fieldDefinition);
			return fieldDefinition;
		}

		public Dictionary<MemberTypeId, PropertyDefinition>.ValueCollection PropertyDefinitions
		{
			get
			{
				return this._properties.Values;
			}
		}

		public Dictionary<MemberTypeId, FieldDefinition>.ValueCollection FieldDefinitions
		{
			get
			{
				return this._fields.Values;
			}
		}

		public void InitializeForAutoGeneration(CollectObjectsDelegate collectObjectsDelegate)
		{
			this.CollectObjectsMethod = collectObjectsDelegate;
		}

		private Dictionary<MemberTypeId, PropertyDefinition> _properties;

		private Dictionary<MemberTypeId, FieldDefinition> _fields;

		private List<string> _errors;

		private List<MethodInfo> _initializationCallbacks;

		private List<MethodInfo> _lateInitializationCallbacks;

		private bool _isClass;

		private readonly IObjectResolver _objectResolver;
	}
}
