using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.SaveSystem.Resolvers;

namespace TaleWorlds.SaveSystem.Definition
{
	// Token: 0x02000068 RID: 104
	public class TypeDefinition : TypeDefinitionBase
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000321 RID: 801 RVA: 0x0000E003 File Offset: 0x0000C203
		// (set) Token: 0x06000322 RID: 802 RVA: 0x0000E00B File Offset: 0x0000C20B
		public List<MemberDefinition> MemberDefinitions { get; private set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000323 RID: 803 RVA: 0x0000E014 File Offset: 0x0000C214
		public IEnumerable<MethodInfo> InitializationCallbacks
		{
			get
			{
				return this._initializationCallbacks;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000324 RID: 804 RVA: 0x0000E01C File Offset: 0x0000C21C
		public IEnumerable<MethodInfo> LateInitializationCallbacks
		{
			get
			{
				return this._lateInitializationCallbacks;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000325 RID: 805 RVA: 0x0000E024 File Offset: 0x0000C224
		public IEnumerable<string> Errors
		{
			get
			{
				return this._errors.AsReadOnly();
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000326 RID: 806 RVA: 0x0000E031 File Offset: 0x0000C231
		public bool IsClassDefinition
		{
			get
			{
				return this._isClass;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000327 RID: 807 RVA: 0x0000E039 File Offset: 0x0000C239
		// (set) Token: 0x06000328 RID: 808 RVA: 0x0000E041 File Offset: 0x0000C241
		public List<CustomField> CustomFields { get; private set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000329 RID: 809 RVA: 0x0000E04A File Offset: 0x0000C24A
		// (set) Token: 0x0600032A RID: 810 RVA: 0x0000E052 File Offset: 0x0000C252
		public CollectObjectsDelegate CollectObjectsMethod { get; private set; }

		// Token: 0x0600032B RID: 811 RVA: 0x0000E05C File Offset: 0x0000C25C
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

		// Token: 0x0600032C RID: 812 RVA: 0x0000E0D6 File Offset: 0x0000C2D6
		public TypeDefinition(Type type, int saveId, IObjectResolver objectResolver)
			: this(type, new TypeSaveId(saveId), objectResolver)
		{
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000E0E6 File Offset: 0x0000C2E6
		public bool CheckIfRequiresAdvancedResolving(object originalObject)
		{
			return this._objectResolver != null && this._objectResolver.CheckIfRequiresAdvancedResolving(originalObject);
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000E0FE File Offset: 0x0000C2FE
		public object ResolveObject(object originalObject)
		{
			if (this._objectResolver != null)
			{
				return this._objectResolver.ResolveObject(originalObject);
			}
			return originalObject;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000E116 File Offset: 0x0000C316
		public object AdvancedResolveObject(object originalObject, MetaData metaData, ObjectLoadData objectLoadData)
		{
			if (this._objectResolver != null)
			{
				return this._objectResolver.AdvancedResolveObject(originalObject, metaData, objectLoadData);
			}
			return originalObject;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0000E130 File Offset: 0x0000C330
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

		// Token: 0x06000331 RID: 817 RVA: 0x0000E1F8 File Offset: 0x0000C3F8
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

		// Token: 0x06000332 RID: 818 RVA: 0x0000E2FB File Offset: 0x0000C4FB
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

		// Token: 0x06000333 RID: 819 RVA: 0x0000E30C File Offset: 0x0000C50C
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

		// Token: 0x06000334 RID: 820 RVA: 0x0000E510 File Offset: 0x0000C710
		public void AddCustomField(string fieldName, short saveId)
		{
			this.CustomFields.Add(new CustomField(fieldName, saveId));
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0000E524 File Offset: 0x0000C724
		public PropertyDefinition GetPropertyDefinitionWithId(MemberTypeId id)
		{
			PropertyDefinition propertyDefinition;
			this._properties.TryGetValue(id, out propertyDefinition);
			return propertyDefinition;
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0000E544 File Offset: 0x0000C744
		public FieldDefinition GetFieldDefinitionWithId(MemberTypeId id)
		{
			FieldDefinition fieldDefinition;
			this._fields.TryGetValue(id, out fieldDefinition);
			return fieldDefinition;
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000337 RID: 823 RVA: 0x0000E561 File Offset: 0x0000C761
		public Dictionary<MemberTypeId, PropertyDefinition>.ValueCollection PropertyDefinitions
		{
			get
			{
				return this._properties.Values;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000338 RID: 824 RVA: 0x0000E56E File Offset: 0x0000C76E
		public Dictionary<MemberTypeId, FieldDefinition>.ValueCollection FieldDefinitions
		{
			get
			{
				return this._fields.Values;
			}
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0000E57B File Offset: 0x0000C77B
		public void InitializeForAutoGeneration(CollectObjectsDelegate collectObjectsDelegate)
		{
			this.CollectObjectsMethod = collectObjectsDelegate;
		}

		// Token: 0x040000FE RID: 254
		private Dictionary<MemberTypeId, PropertyDefinition> _properties;

		// Token: 0x040000FF RID: 255
		private Dictionary<MemberTypeId, FieldDefinition> _fields;

		// Token: 0x04000101 RID: 257
		private List<string> _errors;

		// Token: 0x04000102 RID: 258
		private List<MethodInfo> _initializationCallbacks;

		// Token: 0x04000103 RID: 259
		private List<MethodInfo> _lateInitializationCallbacks;

		// Token: 0x04000104 RID: 260
		private bool _isClass;

		// Token: 0x04000105 RID: 261
		private readonly IObjectResolver _objectResolver;
	}
}
