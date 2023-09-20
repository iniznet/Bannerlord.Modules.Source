using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI
{
	public class PropertyOwnerObject
	{
		protected void OnPropertyChanged<T>(T value, [CallerMemberName] string propertyName = null) where T : class
		{
			Action<PropertyOwnerObject, string, object> propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, propertyName, value);
		}

		protected void OnPropertyChanged(int value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, int> action = this.intPropertyChanged;
			if (action == null)
			{
				return;
			}
			action(this, propertyName, value);
		}

		protected void OnPropertyChanged(float value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, float> action = this.floatPropertyChanged;
			if (action == null)
			{
				return;
			}
			action(this, propertyName, value);
		}

		protected void OnPropertyChanged(bool value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, bool> action = this.boolPropertyChanged;
			if (action == null)
			{
				return;
			}
			action(this, propertyName, value);
		}

		protected void OnPropertyChanged(Vec2 value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, Vec2> vec2PropertyChanged = this.Vec2PropertyChanged;
			if (vec2PropertyChanged == null)
			{
				return;
			}
			vec2PropertyChanged(this, propertyName, value);
		}

		protected void OnPropertyChanged(Vector2 value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, Vector2> vector2PropertyChanged = this.Vector2PropertyChanged;
			if (vector2PropertyChanged == null)
			{
				return;
			}
			vector2PropertyChanged(this, propertyName, value);
		}

		protected void OnPropertyChanged(double value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, double> action = this.doublePropertyChanged;
			if (action == null)
			{
				return;
			}
			action(this, propertyName, value);
		}

		protected void OnPropertyChanged(uint value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, uint> action = this.uintPropertyChanged;
			if (action == null)
			{
				return;
			}
			action(this, propertyName, value);
		}

		protected void OnPropertyChanged(Color value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, Color> colorPropertyChanged = this.ColorPropertyChanged;
			if (colorPropertyChanged == null)
			{
				return;
			}
			colorPropertyChanged(this, propertyName, value);
		}

		public event Action<PropertyOwnerObject, string, object> PropertyChanged;

		public event Action<PropertyOwnerObject, string, bool> boolPropertyChanged;

		public event Action<PropertyOwnerObject, string, int> intPropertyChanged;

		public event Action<PropertyOwnerObject, string, float> floatPropertyChanged;

		public event Action<PropertyOwnerObject, string, Vec2> Vec2PropertyChanged;

		public event Action<PropertyOwnerObject, string, Vector2> Vector2PropertyChanged;

		public event Action<PropertyOwnerObject, string, double> doublePropertyChanged;

		public event Action<PropertyOwnerObject, string, uint> uintPropertyChanged;

		public event Action<PropertyOwnerObject, string, Color> ColorPropertyChanged;
	}
}
