using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	public abstract class TextureProvider
	{
		public virtual void SetTargetSize(int width, int height)
		{
		}

		public abstract Texture GetTexture(TwoDimensionContext twoDimensionContext, string name);

		public virtual void Tick(float dt)
		{
		}

		public virtual void Clear(bool clearNextFrame)
		{
			this._getGetMethodCache.Clear();
		}

		public void SetProperty(string name, object value)
		{
			PropertyInfo property = base.GetType().GetProperty(name);
			if (property != null)
			{
				property.GetSetMethod().Invoke(this, new object[] { value });
			}
		}

		public object GetProperty(string name)
		{
			MethodInfo methodInfo;
			if (this._getGetMethodCache.TryGetValue(name, out methodInfo))
			{
				return methodInfo.Invoke(this, null);
			}
			PropertyInfo property = base.GetType().GetProperty(name);
			if (property != null)
			{
				MethodInfo getMethod = property.GetGetMethod();
				this._getGetMethodCache.Add(name, getMethod);
				return getMethod.Invoke(this, null);
			}
			return null;
		}

		private Dictionary<string, MethodInfo> _getGetMethodCache = new Dictionary<string, MethodInfo>();
	}
}
