using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TaleWorlds.Library
{
	public abstract class ViewModel : IViewModel, INotifyPropertyChanged
	{
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

		private PropertyInfo GetProperty(string name)
		{
			PropertyInfo propertyInfo;
			if (this._propertiesAndMethods != null && this._propertiesAndMethods.Properties.TryGetValue(name, out propertyInfo))
			{
				return propertyInfo;
			}
			return null;
		}

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

		public object GetViewModelAtPath(BindingPath path, bool isList)
		{
			return this.GetViewModelAtPath(path);
		}

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

		public object GetPropertyValue(string name, PropertyTypeFeeder propertyTypeFeeder)
		{
			return this.GetPropertyValue(name);
		}

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

		public Type GetPropertyType(string name)
		{
			PropertyInfo property = this.GetProperty(name);
			if (property != null)
			{
				return property.PropertyType;
			}
			return null;
		}

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

		public virtual void OnFinalize()
		{
		}

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

		public virtual void RefreshValues()
		{
		}

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

		public static bool UIDebugMode;

		private List<PropertyChangedEventHandler> _eventHandlers;

		private List<PropertyChangedWithValueEventHandler> _eventHandlersWithValue;

		private List<PropertyChangedWithBoolValueEventHandler> _eventHandlersWithBoolValue;

		private List<PropertyChangedWithIntValueEventHandler> _eventHandlersWithIntValue;

		private List<PropertyChangedWithFloatValueEventHandler> _eventHandlersWithFloatValue;

		private List<PropertyChangedWithUIntValueEventHandler> _eventHandlersWithUIntValue;

		private List<PropertyChangedWithColorValueEventHandler> _eventHandlersWithColorValue;

		private List<PropertyChangedWithDoubleValueEventHandler> _eventHandlersWithDoubleValue;

		private List<PropertyChangedWithVec2ValueEventHandler> _eventHandlersWithVec2Value;

		private Type _type;

		private ViewModel.DataSourceTypeBindingPropertiesCollection _propertiesAndMethods;

		private static Dictionary<Type, ViewModel.DataSourceTypeBindingPropertiesCollection> _cachedViewModelProperties = new Dictionary<Type, ViewModel.DataSourceTypeBindingPropertiesCollection>();

		public interface IViewModelGetterInterface
		{
			bool IsValueSynced(string name);

			Type GetPropertyType(string name);

			object GetPropertyValue(string name);

			void OnFinalize();
		}

		public interface IViewModelSetterInterface
		{
			void SetPropertyValue(string name, object value);

			void OnFinalize();
		}

		private class DataSourceTypeBindingPropertiesCollection
		{
			public Dictionary<string, PropertyInfo> Properties { get; set; }

			public Dictionary<string, MethodInfo> Methods { get; set; }

			public DataSourceTypeBindingPropertiesCollection(Dictionary<string, PropertyInfo> properties, Dictionary<string, MethodInfo> methods)
			{
				this.Properties = properties;
				this.Methods = methods;
			}
		}
	}
}
