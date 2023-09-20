using System;

// Token: 0x02000003 RID: 3
public class MonoPInvokeCallbackAttribute : Attribute
{
	public MonoPInvokeCallbackAttribute(Type type)
	{
		this.Type = type;
	}

	public Type Type;
}
