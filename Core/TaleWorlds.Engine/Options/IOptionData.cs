using System;

namespace TaleWorlds.Engine.Options
{
	// Token: 0x0200009C RID: 156
	public interface IOptionData
	{
		// Token: 0x06000BAE RID: 2990
		float GetDefaultValue();

		// Token: 0x06000BAF RID: 2991
		void Commit();

		// Token: 0x06000BB0 RID: 2992
		float GetValue(bool forceRefresh);

		// Token: 0x06000BB1 RID: 2993
		void SetValue(float value);

		// Token: 0x06000BB2 RID: 2994
		object GetOptionType();

		// Token: 0x06000BB3 RID: 2995
		bool IsNative();

		// Token: 0x06000BB4 RID: 2996
		bool IsAction();

		// Token: 0x06000BB5 RID: 2997
		ValueTuple<string, bool> GetIsDisabledAndReasonID();
	}
}
