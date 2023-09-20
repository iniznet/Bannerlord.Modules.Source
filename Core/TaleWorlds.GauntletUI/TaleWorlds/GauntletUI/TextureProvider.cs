using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x0200002B RID: 43
	public abstract class TextureProvider
	{
		// Token: 0x060002F0 RID: 752 RVA: 0x0000DFC5 File Offset: 0x0000C1C5
		public virtual void SetTargetSize(int width, int height)
		{
		}

		// Token: 0x060002F1 RID: 753
		public abstract Texture GetTexture(TwoDimensionContext twoDimensionContext, string name);

		// Token: 0x060002F2 RID: 754 RVA: 0x0000DFC7 File Offset: 0x0000C1C7
		public virtual void Tick(float dt)
		{
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x0000DFC9 File Offset: 0x0000C1C9
		public virtual void Clear()
		{
			this._getGetMethodCache.Clear();
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0000DFD8 File Offset: 0x0000C1D8
		public void SetProperty(string name, object value)
		{
			PropertyInfo property = base.GetType().GetProperty(name);
			if (property != null)
			{
				property.GetSetMethod().Invoke(this, new object[] { value });
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000E014 File Offset: 0x0000C214
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

		// Token: 0x0400017E RID: 382
		private Dictionary<string, MethodInfo> _getGetMethodCache = new Dictionary<string, MethodInfo>();
	}
}
