using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TaleWorlds.Library
{
	// Token: 0x0200009B RID: 155
	public abstract class ViewModel : IViewModel, INotifyPropertyChanged
	{
		// Token: 0x14000015 RID: 21
		// (add) Token: 0x0600059B RID: 1435 RVA: 0x00011EDA File Offset: 0x000100DA
		// (remove) Token: 0x0600059C RID: 1436 RVA: 0x00011EFB File Offset: 0x000100FB
		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				if (this._eventHandlers == null)
				{
					this._eventHandlers = new List<PropertyChangedEventHandler>();
				}
				this._eventHandlers.Add(value);
			}
			remove
			{
				if (this._eventHandlers != null)
				{
					this._eventHandlers.Remove(value);
				}
			}
		}

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x0600059D RID: 1437 RVA: 0x00011F12 File Offset: 0x00010112
		// (remove) Token: 0x0600059E RID: 1438 RVA: 0x00011F33 File Offset: 0x00010133
		public event PropertyChangedWithValueEventHandler PropertyChangedWithValue
		{
			add
			{
				if (this._eventHandlersWithValue == null)
				{
					this._eventHandlersWithValue = new List<PropertyChangedWithValueEventHandler>();
				}
				this._eventHandlersWithValue.Add(value);
			}
			remove
			{
				if (this._eventHandlersWithValue != null)
				{
					this._eventHandlersWithValue.Remove(value);
				}
			}
		}

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x0600059F RID: 1439 RVA: 0x00011F4A File Offset: 0x0001014A
		// (remove) Token: 0x060005A0 RID: 1440 RVA: 0x00011F6B File Offset: 0x0001016B
		public event PropertyChangedWithBoolValueEventHandler PropertyChangedWithBoolValue
		{
			add
			{
				if (this._eventHandlersWithBoolValue == null)
				{
					this._eventHandlersWithBoolValue = new List<PropertyChangedWithBoolValueEventHandler>();
				}
				this._eventHandlersWithBoolValue.Add(value);
			}
			remove
			{
				if (this._eventHandlersWithBoolValue != null)
				{
					this._eventHandlersWithBoolValue.Remove(value);
				}
			}
		}

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x060005A1 RID: 1441 RVA: 0x00011F82 File Offset: 0x00010182
		// (remove) Token: 0x060005A2 RID: 1442 RVA: 0x00011FA3 File Offset: 0x000101A3
		public event PropertyChangedWithIntValueEventHandler PropertyChangedWithIntValue
		{
			add
			{
				if (this._eventHandlersWithIntValue == null)
				{
					this._eventHandlersWithIntValue = new List<PropertyChangedWithIntValueEventHandler>();
				}
				this._eventHandlersWithIntValue.Add(value);
			}
			remove
			{
				if (this._eventHandlersWithIntValue != null)
				{
					this._eventHandlersWithIntValue.Remove(value);
				}
			}
		}

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x060005A3 RID: 1443 RVA: 0x00011FBA File Offset: 0x000101BA
		// (remove) Token: 0x060005A4 RID: 1444 RVA: 0x00011FDB File Offset: 0x000101DB
		public event PropertyChangedWithFloatValueEventHandler PropertyChangedWithFloatValue
		{
			add
			{
				if (this._eventHandlersWithFloatValue == null)
				{
					this._eventHandlersWithFloatValue = new List<PropertyChangedWithFloatValueEventHandler>();
				}
				this._eventHandlersWithFloatValue.Add(value);
			}
			remove
			{
				if (this._eventHandlersWithFloatValue != null)
				{
					this._eventHandlersWithFloatValue.Remove(value);
				}
			}
		}

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x060005A5 RID: 1445 RVA: 0x00011FF2 File Offset: 0x000101F2
		// (remove) Token: 0x060005A6 RID: 1446 RVA: 0x00012013 File Offset: 0x00010213
		public event PropertyChangedWithUIntValueEventHandler PropertyChangedWithUIntValue
		{
			add
			{
				if (this._eventHandlersWithUIntValue == null)
				{
					this._eventHandlersWithUIntValue = new List<PropertyChangedWithUIntValueEventHandler>();
				}
				this._eventHandlersWithUIntValue.Add(value);
			}
			remove
			{
				if (this._eventHandlersWithUIntValue != null)
				{
					this._eventHandlersWithUIntValue.Remove(value);
				}
			}
		}

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x060005A7 RID: 1447 RVA: 0x0001202A File Offset: 0x0001022A
		// (remove) Token: 0x060005A8 RID: 1448 RVA: 0x0001204B File Offset: 0x0001024B
		public event PropertyChangedWithColorValueEventHandler PropertyChangedWithColorValue
		{
			add
			{
				if (this._eventHandlersWithColorValue == null)
				{
					this._eventHandlersWithColorValue = new List<PropertyChangedWithColorValueEventHandler>();
				}
				this._eventHandlersWithColorValue.Add(value);
			}
			remove
			{
				if (this._eventHandlersWithColorValue != null)
				{
					this._eventHandlersWithColorValue.Remove(value);
				}
			}
		}

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x060005A9 RID: 1449 RVA: 0x00012062 File Offset: 0x00010262
		// (remove) Token: 0x060005AA RID: 1450 RVA: 0x00012083 File Offset: 0x00010283
		public event PropertyChangedWithDoubleValueEventHandler PropertyChangedWithDoubleValue
		{
			add
			{
				if (this._eventHandlersWithDoubleValue == null)
				{
					this._eventHandlersWithDoubleValue = new List<PropertyChangedWithDoubleValueEventHandler>();
				}
				this._eventHandlersWithDoubleValue.Add(value);
			}
			remove
			{
				if (this._eventHandlersWithDoubleValue != null)
				{
					this._eventHandlersWithDoubleValue.Remove(value);
				}
			}
		}

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x060005AB RID: 1451 RVA: 0x0001209A File Offset: 0x0001029A
		// (remove) Token: 0x060005AC RID: 1452 RVA: 0x000120BB File Offset: 0x000102BB
		public event PropertyChangedWithVec2ValueEventHandler PropertyChangedWithVec2Value
		{
			add
			{
				if (this._eventHandlersWithVec2Value == null)
				{
					this._eventHandlersWithVec2Value = new List<PropertyChangedWithVec2ValueEventHandler>();
				}
				this._eventHandlersWithVec2Value.Add(value);
			}
			remove
			{
				if (this._eventHandlersWithVec2Value != null)
				{
					this._eventHandlersWithVec2Value.Remove(value);
				}
			}
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x000120D4 File Offset: 0x000102D4
		protected ViewModel()
		{
			this._type = base.GetType();
			ViewModel.DataSourceTypeBindingPropertiesCollection dataSourceTypeBindingPropertiesCollection;
			ViewModel._cachedViewModelProperties.TryGetValue(this._type, out dataSourceTypeBindingPropertiesCollection);
			if (dataSourceTypeBindingPropertiesCollection == null)
			{
				this._propertiesAndMethods = ViewModel.GetPropertiesOfType(this._type);
				ViewModel._cachedViewModelProperties.Add(this._type, this._propertiesAndMethods);
				return;
			}
			this._propertiesAndMethods = dataSourceTypeBindingPropertiesCollection;
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x00012138 File Offset: 0x00010338
		private PropertyInfo GetProperty(string name)
		{
			PropertyInfo propertyInfo;
			if (this._propertiesAndMethods != null && this._propertiesAndMethods.Properties.TryGetValue(name, out propertyInfo))
			{
				return propertyInfo;
			}
			return null;
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x00012165 File Offset: 0x00010365
		protected bool SetField<T>(ref T field, T value, string propertyName)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
			{
				return false;
			}
			field = value;
			this.OnPropertyChanged(propertyName);
			return true;
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x0001218C File Offset: 0x0001038C
		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (this._eventHandlers != null)
			{
				for (int i = 0; i < this._eventHandlers.Count; i++)
				{
					PropertyChangedEventHandler propertyChangedEventHandler = this._eventHandlers[i];
					PropertyChangedEventArgs propertyChangedEventArgs = new PropertyChangedEventArgs(propertyName);
					propertyChangedEventHandler(this, propertyChangedEventArgs);
				}
			}
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x000121D4 File Offset: 0x000103D4
		public void OnPropertyChangedWithValue<T>(T value, [CallerMemberName] string propertyName = null) where T : class
		{
			if (this._eventHandlersWithValue != null)
			{
				for (int i = 0; i < this._eventHandlersWithValue.Count; i++)
				{
					PropertyChangedWithValueEventHandler propertyChangedWithValueEventHandler = this._eventHandlersWithValue[i];
					PropertyChangedWithValueEventArgs propertyChangedWithValueEventArgs = new PropertyChangedWithValueEventArgs(propertyName, value);
					propertyChangedWithValueEventHandler(this, propertyChangedWithValueEventArgs);
				}
			}
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x00012220 File Offset: 0x00010420
		public void OnPropertyChangedWithValue(bool value, [CallerMemberName] string propertyName = null)
		{
			if (this._eventHandlersWithBoolValue != null)
			{
				for (int i = 0; i < this._eventHandlersWithBoolValue.Count; i++)
				{
					PropertyChangedWithBoolValueEventHandler propertyChangedWithBoolValueEventHandler = this._eventHandlersWithBoolValue[i];
					PropertyChangedWithBoolValueEventArgs propertyChangedWithBoolValueEventArgs = new PropertyChangedWithBoolValueEventArgs(propertyName, value);
					propertyChangedWithBoolValueEventHandler(this, propertyChangedWithBoolValueEventArgs);
				}
			}
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x00012268 File Offset: 0x00010468
		public void OnPropertyChangedWithValue(int value, [CallerMemberName] string propertyName = null)
		{
			if (this._eventHandlersWithIntValue != null)
			{
				for (int i = 0; i < this._eventHandlersWithIntValue.Count; i++)
				{
					PropertyChangedWithIntValueEventHandler propertyChangedWithIntValueEventHandler = this._eventHandlersWithIntValue[i];
					PropertyChangedWithIntValueEventArgs propertyChangedWithIntValueEventArgs = new PropertyChangedWithIntValueEventArgs(propertyName, value);
					propertyChangedWithIntValueEventHandler(this, propertyChangedWithIntValueEventArgs);
				}
			}
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x000122B0 File Offset: 0x000104B0
		public void OnPropertyChangedWithValue(float value, [CallerMemberName] string propertyName = null)
		{
			if (this._eventHandlersWithFloatValue != null)
			{
				for (int i = 0; i < this._eventHandlersWithFloatValue.Count; i++)
				{
					PropertyChangedWithFloatValueEventHandler propertyChangedWithFloatValueEventHandler = this._eventHandlersWithFloatValue[i];
					PropertyChangedWithFloatValueEventArgs propertyChangedWithFloatValueEventArgs = new PropertyChangedWithFloatValueEventArgs(propertyName, value);
					propertyChangedWithFloatValueEventHandler(this, propertyChangedWithFloatValueEventArgs);
				}
			}
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x000122F8 File Offset: 0x000104F8
		public void OnPropertyChangedWithValue(uint value, [CallerMemberName] string propertyName = null)
		{
			if (this._eventHandlersWithUIntValue != null)
			{
				for (int i = 0; i < this._eventHandlersWithUIntValue.Count; i++)
				{
					PropertyChangedWithUIntValueEventHandler propertyChangedWithUIntValueEventHandler = this._eventHandlersWithUIntValue[i];
					PropertyChangedWithUIntValueEventArgs propertyChangedWithUIntValueEventArgs = new PropertyChangedWithUIntValueEventArgs(propertyName, value);
					propertyChangedWithUIntValueEventHandler(this, propertyChangedWithUIntValueEventArgs);
				}
			}
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x00012340 File Offset: 0x00010540
		public void OnPropertyChangedWithValue(Color value, [CallerMemberName] string propertyName = null)
		{
			if (this._eventHandlersWithColorValue != null)
			{
				for (int i = 0; i < this._eventHandlersWithColorValue.Count; i++)
				{
					PropertyChangedWithColorValueEventHandler propertyChangedWithColorValueEventHandler = this._eventHandlersWithColorValue[i];
					PropertyChangedWithColorValueEventArgs propertyChangedWithColorValueEventArgs = new PropertyChangedWithColorValueEventArgs(propertyName, value);
					propertyChangedWithColorValueEventHandler(this, propertyChangedWithColorValueEventArgs);
				}
			}
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x00012388 File Offset: 0x00010588
		public void OnPropertyChangedWithValue(double value, [CallerMemberName] string propertyName = null)
		{
			if (this._eventHandlersWithDoubleValue != null)
			{
				for (int i = 0; i < this._eventHandlersWithDoubleValue.Count; i++)
				{
					PropertyChangedWithDoubleValueEventHandler propertyChangedWithDoubleValueEventHandler = this._eventHandlersWithDoubleValue[i];
					PropertyChangedWithDoubleValueEventArgs propertyChangedWithDoubleValueEventArgs = new PropertyChangedWithDoubleValueEventArgs(propertyName, value);
					propertyChangedWithDoubleValueEventHandler(this, propertyChangedWithDoubleValueEventArgs);
				}
			}
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x000123D0 File Offset: 0x000105D0
		public void OnPropertyChangedWithValue(Vec2 value, [CallerMemberName] string propertyName = null)
		{
			if (this._eventHandlersWithVec2Value != null)
			{
				for (int i = 0; i < this._eventHandlersWithVec2Value.Count; i++)
				{
					PropertyChangedWithVec2ValueEventHandler propertyChangedWithVec2ValueEventHandler = this._eventHandlersWithVec2Value[i];
					PropertyChangedWithVec2ValueEventArgs propertyChangedWithVec2ValueEventArgs = new PropertyChangedWithVec2ValueEventArgs(propertyName, value);
					propertyChangedWithVec2ValueEventHandler(this, propertyChangedWithVec2ValueEventArgs);
				}
			}
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x00012416 File Offset: 0x00010616
		public object GetViewModelAtPath(BindingPath path, bool isList)
		{
			return this.GetViewModelAtPath(path);
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x00012420 File Offset: 0x00010620
		public object GetViewModelAtPath(BindingPath path)
		{
			BindingPath subPath = path.SubPath;
			if (subPath != null)
			{
				PropertyInfo property = this.GetProperty(subPath.FirstNode);
				if (property != null)
				{
					object obj = property.GetGetMethod().InvokeWithLog(this, null);
					ViewModel viewModel;
					if ((viewModel = obj as ViewModel) != null)
					{
						return viewModel.GetViewModelAtPath(subPath);
					}
					if (obj is IMBBindingList)
					{
						return ViewModel.GetChildAtPath(obj as IMBBindingList, subPath);
					}
				}
				return null;
			}
			return this;
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x0001248C File Offset: 0x0001068C
		private static object GetChildAtPath(IMBBindingList bindingList, BindingPath path)
		{
			BindingPath subPath = path.SubPath;
			if (subPath == null)
			{
				return bindingList;
			}
			if (bindingList.Count > 0)
			{
				int num = Convert.ToInt32(subPath.FirstNode);
				if (num >= 0 && num < bindingList.Count)
				{
					object obj = bindingList[num];
					if (obj is ViewModel)
					{
						return (obj as ViewModel).GetViewModelAtPath(subPath);
					}
					if (obj is IMBBindingList)
					{
						return ViewModel.GetChildAtPath(obj as IMBBindingList, subPath);
					}
				}
			}
			return null;
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x00012500 File Offset: 0x00010700
		public object GetPropertyValue(string name, PropertyTypeFeeder propertyTypeFeeder)
		{
			return this.GetPropertyValue(name);
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x0001250C File Offset: 0x0001070C
		public object GetPropertyValue(string name)
		{
			PropertyInfo property = this.GetProperty(name);
			object obj = null;
			if (property != null)
			{
				obj = property.GetGetMethod().InvokeWithLog(this, null);
			}
			return obj;
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x0001253C File Offset: 0x0001073C
		public Type GetPropertyType(string name)
		{
			PropertyInfo property = this.GetProperty(name);
			if (property != null)
			{
				return property.PropertyType;
			}
			return null;
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x00012564 File Offset: 0x00010764
		public void SetPropertyValue(string name, object value)
		{
			PropertyInfo property = this.GetProperty(name);
			if (property != null)
			{
				MethodInfo setMethod = property.GetSetMethod();
				if (setMethod == null)
				{
					return;
				}
				setMethod.InvokeWithLog(this, new object[] { value });
			}
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x0001259E File Offset: 0x0001079E
		public virtual void OnFinalize()
		{
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x000125A0 File Offset: 0x000107A0
		public void ExecuteCommand(string commandName, object[] parameters)
		{
			MethodInfo methodInfo;
			MethodInfo methodInfo2;
			if (this._propertiesAndMethods != null && this._propertiesAndMethods.Methods.TryGetValue(commandName, out methodInfo))
			{
				methodInfo2 = methodInfo;
			}
			else
			{
				methodInfo2 = this._type.GetMethod(commandName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			if (methodInfo2 != null)
			{
				if (methodInfo2.GetParameters().Length == parameters.Length)
				{
					object[] array = new object[parameters.Length];
					ParameterInfo[] parameters2 = methodInfo2.GetParameters();
					for (int i = 0; i < parameters.Length; i++)
					{
						object obj = parameters[i];
						Type parameterType = parameters2[i].ParameterType;
						array[i] = obj;
						if (obj is string && parameterType != typeof(string))
						{
							object obj2 = ViewModel.ConvertValueTo((string)obj, parameterType);
							array[i] = obj2;
						}
					}
					methodInfo2.InvokeWithLog(this, array);
					return;
				}
				if (methodInfo2.GetParameters().Length == 0)
				{
					methodInfo2.InvokeWithLog(this, null);
				}
			}
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x00012680 File Offset: 0x00010880
		private static object ConvertValueTo(string value, Type parameterType)
		{
			object obj = null;
			if (parameterType == typeof(string))
			{
				obj = value;
			}
			else if (parameterType == typeof(int))
			{
				obj = Convert.ToInt32(value);
			}
			else if (parameterType == typeof(float))
			{
				obj = Convert.ToSingle(value);
			}
			return obj;
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x000126E4 File Offset: 0x000108E4
		public virtual void RefreshValues()
		{
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x000126E8 File Offset: 0x000108E8
		public static void CollectPropertiesAndMethods()
		{
			foreach (Assembly assembly in ViewModel.GetViewModelAssemblies())
			{
				Type[] array = new Type[0];
				try
				{
					array = assembly.GetTypes();
				}
				catch (Exception ex)
				{
					Debug.Print(ex.Message, 0, Debug.DebugColor.White, 17592186044416UL);
					Debug.Print(ex.StackTrace, 0, Debug.DebugColor.White, 17592186044416UL);
				}
				foreach (Type type in array)
				{
					if (typeof(IViewModel).IsAssignableFrom(type) && typeof(IViewModel) != type)
					{
						ViewModel.DataSourceTypeBindingPropertiesCollection propertiesOfType = ViewModel.GetPropertiesOfType(type);
						ViewModel._cachedViewModelProperties[type] = propertiesOfType;
					}
				}
			}
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x000127B8 File Offset: 0x000109B8
		private static Assembly[] GetViewModelAssemblies()
		{
			List<Assembly> list = new List<Assembly>();
			Assembly assembly = typeof(ViewModel).Assembly;
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

		// Token: 0x060005C6 RID: 1478 RVA: 0x00012854 File Offset: 0x00010A54
		private static ViewModel.DataSourceTypeBindingPropertiesCollection GetPropertiesOfType(Type t)
		{
			string name = t.Name;
			Dictionary<string, PropertyInfo> dictionary = new Dictionary<string, PropertyInfo>();
			Dictionary<string, MethodInfo> dictionary2 = new Dictionary<string, MethodInfo>();
			foreach (PropertyInfo propertyInfo in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				dictionary.Add(propertyInfo.Name, propertyInfo);
			}
			foreach (MethodInfo methodInfo in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (!dictionary2.ContainsKey(methodInfo.Name))
				{
					dictionary2.Add(methodInfo.Name, methodInfo);
				}
			}
			return new ViewModel.DataSourceTypeBindingPropertiesCollection(dictionary, dictionary2);
		}

		// Token: 0x0400019F RID: 415
		public static bool UIDebugMode;

		// Token: 0x040001A0 RID: 416
		private List<PropertyChangedEventHandler> _eventHandlers;

		// Token: 0x040001A1 RID: 417
		private List<PropertyChangedWithValueEventHandler> _eventHandlersWithValue;

		// Token: 0x040001A2 RID: 418
		private List<PropertyChangedWithBoolValueEventHandler> _eventHandlersWithBoolValue;

		// Token: 0x040001A3 RID: 419
		private List<PropertyChangedWithIntValueEventHandler> _eventHandlersWithIntValue;

		// Token: 0x040001A4 RID: 420
		private List<PropertyChangedWithFloatValueEventHandler> _eventHandlersWithFloatValue;

		// Token: 0x040001A5 RID: 421
		private List<PropertyChangedWithUIntValueEventHandler> _eventHandlersWithUIntValue;

		// Token: 0x040001A6 RID: 422
		private List<PropertyChangedWithColorValueEventHandler> _eventHandlersWithColorValue;

		// Token: 0x040001A7 RID: 423
		private List<PropertyChangedWithDoubleValueEventHandler> _eventHandlersWithDoubleValue;

		// Token: 0x040001A8 RID: 424
		private List<PropertyChangedWithVec2ValueEventHandler> _eventHandlersWithVec2Value;

		// Token: 0x040001A9 RID: 425
		private Type _type;

		// Token: 0x040001AA RID: 426
		private ViewModel.DataSourceTypeBindingPropertiesCollection _propertiesAndMethods;

		// Token: 0x040001AB RID: 427
		private static Dictionary<Type, ViewModel.DataSourceTypeBindingPropertiesCollection> _cachedViewModelProperties = new Dictionary<Type, ViewModel.DataSourceTypeBindingPropertiesCollection>();

		// Token: 0x020000E0 RID: 224
		public interface IViewModelGetterInterface
		{
			// Token: 0x0600071A RID: 1818
			bool IsValueSynced(string name);

			// Token: 0x0600071B RID: 1819
			Type GetPropertyType(string name);

			// Token: 0x0600071C RID: 1820
			object GetPropertyValue(string name);

			// Token: 0x0600071D RID: 1821
			void OnFinalize();
		}

		// Token: 0x020000E1 RID: 225
		public interface IViewModelSetterInterface
		{
			// Token: 0x0600071E RID: 1822
			void SetPropertyValue(string name, object value);

			// Token: 0x0600071F RID: 1823
			void OnFinalize();
		}

		// Token: 0x020000E2 RID: 226
		private class DataSourceTypeBindingPropertiesCollection
		{
			// Token: 0x170000F8 RID: 248
			// (get) Token: 0x06000720 RID: 1824 RVA: 0x00015BC3 File Offset: 0x00013DC3
			// (set) Token: 0x06000721 RID: 1825 RVA: 0x00015BCB File Offset: 0x00013DCB
			public Dictionary<string, PropertyInfo> Properties { get; set; }

			// Token: 0x170000F9 RID: 249
			// (get) Token: 0x06000722 RID: 1826 RVA: 0x00015BD4 File Offset: 0x00013DD4
			// (set) Token: 0x06000723 RID: 1827 RVA: 0x00015BDC File Offset: 0x00013DDC
			public Dictionary<string, MethodInfo> Methods { get; set; }

			// Token: 0x06000724 RID: 1828 RVA: 0x00015BE5 File Offset: 0x00013DE5
			public DataSourceTypeBindingPropertiesCollection(Dictionary<string, PropertyInfo> properties, Dictionary<string, MethodInfo> methods)
			{
				this.Properties = properties;
				this.Methods = methods;
			}
		}
	}
}
