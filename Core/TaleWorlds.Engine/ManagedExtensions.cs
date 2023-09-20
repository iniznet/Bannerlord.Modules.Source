using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000052 RID: 82
	public static class ManagedExtensions
	{
		// Token: 0x060006F1 RID: 1777 RVA: 0x00005238 File Offset: 0x00003438
		[EngineCallback]
		internal static void SetObjectField(DotNetObject managedObject, string fieldName, ref ScriptComponentFieldHolder scriptComponentHolder, int type, int callFieldChangeEventAsInteger)
		{
			bool flag = callFieldChangeEventAsInteger != 0;
			FieldInfo fieldOfClass = Managed.GetFieldOfClass(managedObject.GetType().Name, fieldName);
			switch (type)
			{
			case 0:
				fieldOfClass.SetValue(managedObject, scriptComponentHolder.s);
				break;
			case 1:
				fieldOfClass.SetValue(managedObject, Convert.ChangeType(scriptComponentHolder.d, fieldOfClass.FieldType));
				break;
			case 2:
				fieldOfClass.SetValue(managedObject, Convert.ChangeType(scriptComponentHolder.f, fieldOfClass.FieldType));
				break;
			case 3:
			{
				bool flag2 = scriptComponentHolder.b > 0;
				fieldOfClass.SetValue(managedObject, flag2);
				break;
			}
			case 4:
				fieldOfClass.SetValue(managedObject, Convert.ChangeType(scriptComponentHolder.i, fieldOfClass.FieldType));
				break;
			case 5:
			{
				Vec3 vec = new Vec3(scriptComponentHolder.v3.x, scriptComponentHolder.v3.y, scriptComponentHolder.v3.z, scriptComponentHolder.v3.w);
				fieldOfClass.SetValue(managedObject, vec);
				break;
			}
			case 6:
				fieldOfClass.SetValue(managedObject, (scriptComponentHolder.entityPointer != UIntPtr.Zero) ? new GameEntity(scriptComponentHolder.entityPointer) : null);
				break;
			case 7:
				fieldOfClass.SetValue(managedObject, (scriptComponentHolder.texturePointer != UIntPtr.Zero) ? new Texture(scriptComponentHolder.texturePointer) : null);
				break;
			case 8:
				fieldOfClass.SetValue(managedObject, (scriptComponentHolder.meshPointer != UIntPtr.Zero) ? new MetaMesh(scriptComponentHolder.meshPointer) : null);
				break;
			case 9:
			{
				object obj = Enum.Parse(fieldOfClass.FieldType, scriptComponentHolder.enumValue);
				fieldOfClass.SetValue(managedObject, obj);
				break;
			}
			case 10:
				fieldOfClass.SetValue(managedObject, (scriptComponentHolder.materialPointer != UIntPtr.Zero) ? new Material(scriptComponentHolder.materialPointer) : null);
				break;
			case 13:
			{
				MatrixFrame matrixFrame = scriptComponentHolder.matrixFrame;
				fieldOfClass.SetValue(managedObject, matrixFrame);
				break;
			}
			}
			if (type != 11 && flag && managedObject is ScriptComponentBehavior)
			{
				((ScriptComponentBehavior)managedObject).OnEditorVariableChanged(fieldName);
			}
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x0000547C File Offset: 0x0000367C
		[EngineCallback]
		internal static void GetObjectField(DotNetObject managedObject, ref ScriptComponentFieldHolder scriptComponentFieldHolder, string fieldName, int type)
		{
			FieldInfo fieldOfClass = Managed.GetFieldOfClass(managedObject.GetType().Name, fieldName);
			switch (type)
			{
			case 0:
				scriptComponentFieldHolder.s = (string)fieldOfClass.GetValue(managedObject);
				return;
			case 1:
				scriptComponentFieldHolder.d = (double)Convert.ChangeType(fieldOfClass.GetValue(managedObject), typeof(double));
				return;
			case 2:
				scriptComponentFieldHolder.f = (float)Convert.ChangeType(fieldOfClass.GetValue(managedObject), typeof(float));
				return;
			case 3:
				scriptComponentFieldHolder.b = (((bool)fieldOfClass.GetValue(managedObject)) ? 1 : 0);
				return;
			case 4:
				scriptComponentFieldHolder.i = (int)Convert.ChangeType(fieldOfClass.GetValue(managedObject), typeof(int));
				return;
			case 5:
			{
				Vec3 vec = (Vec3)fieldOfClass.GetValue(managedObject);
				scriptComponentFieldHolder.v3 = new Vec3(vec, vec.w);
				return;
			}
			case 6:
			{
				GameEntity gameEntity = (GameEntity)fieldOfClass.GetValue(managedObject);
				scriptComponentFieldHolder.entityPointer = ((gameEntity != null) ? ((UIntPtr)Convert.ChangeType(gameEntity.Pointer, typeof(UIntPtr))) : ((UIntPtr)0UL));
				return;
			}
			case 7:
			{
				Texture texture = (Texture)fieldOfClass.GetValue(managedObject);
				scriptComponentFieldHolder.texturePointer = ((texture != null) ? ((UIntPtr)Convert.ChangeType(texture.Pointer, typeof(UIntPtr))) : ((UIntPtr)0UL));
				return;
			}
			case 8:
			{
				MetaMesh metaMesh = (MetaMesh)fieldOfClass.GetValue(managedObject);
				scriptComponentFieldHolder.meshPointer = ((metaMesh != null) ? ((UIntPtr)Convert.ChangeType(metaMesh.Pointer, typeof(UIntPtr))) : ((UIntPtr)0UL));
				return;
			}
			case 9:
				scriptComponentFieldHolder.enumValue = fieldOfClass.GetValue(managedObject).ToString();
				return;
			case 10:
			{
				Material material = (Material)fieldOfClass.GetValue(managedObject);
				scriptComponentFieldHolder.materialPointer = ((material != null) ? ((UIntPtr)Convert.ChangeType(material.Pointer, typeof(UIntPtr))) : ((UIntPtr)0UL));
				break;
			}
			case 11:
			case 12:
				break;
			case 13:
			{
				MatrixFrame matrixFrame = (MatrixFrame)fieldOfClass.GetValue(managedObject);
				scriptComponentFieldHolder.matrixFrame = matrixFrame;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x000056DC File Offset: 0x000038DC
		[EngineCallback]
		internal static void CopyObjectFieldsFrom(DotNetObject dst, DotNetObject src, string className, int callFieldChangeEventAsInteger)
		{
			bool flag = callFieldChangeEventAsInteger != 0;
			foreach (KeyValuePair<string, FieldInfo> keyValuePair in Managed.GetEditableFieldsOfClass(className))
			{
				FieldInfo value = keyValuePair.Value;
				value.SetValue(dst, value.GetValue(src));
				if (flag && dst is ScriptComponentBehavior)
				{
					((ScriptComponentBehavior)dst).OnEditorVariableChanged(keyValuePair.Key);
				}
			}
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x00005760 File Offset: 0x00003960
		[EngineCallback]
		internal static DotNetObject CreateScriptComponentInstance(string className, GameEntity entity, ManagedScriptComponent managedScriptComponent)
		{
			ScriptComponentBehavior scriptComponentBehavior = null;
			Func<ScriptComponentBehavior> func = (Func<ScriptComponentBehavior>)Managed.GetConstructorDelegateOfClass(className);
			if (func != null)
			{
				scriptComponentBehavior = func();
				if (scriptComponentBehavior != null)
				{
					scriptComponentBehavior.Construct(entity, managedScriptComponent);
				}
			}
			else
			{
				ConstructorInfo constructorOfClass = Managed.GetConstructorOfClass(className);
				if (constructorOfClass != null)
				{
					scriptComponentBehavior = constructorOfClass.Invoke(new object[0]) as ScriptComponentBehavior;
					if (scriptComponentBehavior != null)
					{
						scriptComponentBehavior.Construct(entity, managedScriptComponent);
					}
				}
				else
				{
					MBDebug.ShowWarning("CreateScriptComponentInstance failed: " + className);
				}
			}
			return scriptComponentBehavior;
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x000057D4 File Offset: 0x000039D4
		[EngineCallback]
		internal static string GetScriptComponentClassNames()
		{
			List<Type> list = new List<Type>();
			foreach (Type type in Managed.ModuleTypes.Values)
			{
				if (!type.IsAbstract && typeof(ScriptComponentBehavior).IsAssignableFrom(type))
				{
					list.Add(type);
				}
			}
			string text = "";
			for (int i = 0; i < list.Count; i++)
			{
				Type type2 = list[i];
				text += type2.Name;
				text += "-";
				text += type2.BaseType.Name;
				if (i + 1 != list.Count)
				{
					text += " ";
				}
			}
			return text;
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x000058B8 File Offset: 0x00003AB8
		[EngineCallback]
		internal static bool GetEditorVisibilityOfField(string className, string fieldName)
		{
			object[] customAttributes = Managed.GetFieldOfClass(className, fieldName).GetCustomAttributes(typeof(EditorVisibleScriptComponentVariable), true);
			return customAttributes.Length == 0 || (customAttributes[0] as EditorVisibleScriptComponentVariable).Visible;
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x000058F0 File Offset: 0x00003AF0
		[EngineCallback]
		internal static int GetTypeOfField(string className, string fieldName)
		{
			FieldInfo fieldOfClass = Managed.GetFieldOfClass(className, fieldName);
			if (fieldOfClass == null)
			{
				return -1;
			}
			Type fieldType = fieldOfClass.FieldType;
			if (fieldOfClass.FieldType == typeof(string))
			{
				return 0;
			}
			if (fieldOfClass.FieldType == typeof(double))
			{
				return 1;
			}
			if (fieldOfClass.FieldType.IsEnum)
			{
				return 9;
			}
			if (fieldOfClass.FieldType == typeof(float))
			{
				return 2;
			}
			if (fieldOfClass.FieldType == typeof(bool))
			{
				return 3;
			}
			if (fieldType == typeof(byte) || fieldType == typeof(sbyte) || fieldType == typeof(short) || fieldType == typeof(ushort) || fieldType == typeof(int) || fieldType == typeof(uint) || fieldType == typeof(long) || fieldType == typeof(ulong))
			{
				return 4;
			}
			if (fieldOfClass.FieldType == typeof(Vec3))
			{
				return 5;
			}
			if (fieldOfClass.FieldType == typeof(GameEntity))
			{
				return 6;
			}
			if (fieldOfClass.FieldType == typeof(Texture))
			{
				return 7;
			}
			if (fieldOfClass.FieldType == typeof(MetaMesh))
			{
				return 8;
			}
			if (fieldOfClass.FieldType == typeof(Material))
			{
				return 10;
			}
			if (fieldOfClass.FieldType == typeof(SimpleButton))
			{
				return 11;
			}
			if (fieldOfClass.FieldType == typeof(MatrixFrame))
			{
				return 13;
			}
			return -1;
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x00005AD0 File Offset: 0x00003CD0
		[EngineCallback]
		internal static void ForceGarbageCollect()
		{
			Utilities.FlushManagedObjectsMemory();
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x00005AD8 File Offset: 0x00003CD8
		[EngineCallback]
		internal static void CollectCommandLineFunctions()
		{
			foreach (string text in CommandLineFunctionality.CollectCommandLineFunctions())
			{
				Utilities.AddCommandLineFunction(text);
			}
		}
	}
}
