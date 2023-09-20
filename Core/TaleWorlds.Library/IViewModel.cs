using System;
using System.ComponentModel;

namespace TaleWorlds.Library
{
	// Token: 0x02000044 RID: 68
	public interface IViewModel : INotifyPropertyChanged
	{
		// Token: 0x0600021D RID: 541
		object GetViewModelAtPath(BindingPath path);

		// Token: 0x0600021E RID: 542
		object GetViewModelAtPath(BindingPath path, bool isList);

		// Token: 0x0600021F RID: 543
		object GetPropertyValue(string name);

		// Token: 0x06000220 RID: 544
		object GetPropertyValue(string name, PropertyTypeFeeder propertyTypeFeeder);

		// Token: 0x06000221 RID: 545
		void SetPropertyValue(string name, object value);

		// Token: 0x06000222 RID: 546
		void ExecuteCommand(string commandName, object[] parameters);

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000223 RID: 547
		// (remove) Token: 0x06000224 RID: 548
		event PropertyChangedWithValueEventHandler PropertyChangedWithValue;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000225 RID: 549
		// (remove) Token: 0x06000226 RID: 550
		event PropertyChangedWithBoolValueEventHandler PropertyChangedWithBoolValue;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06000227 RID: 551
		// (remove) Token: 0x06000228 RID: 552
		event PropertyChangedWithIntValueEventHandler PropertyChangedWithIntValue;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06000229 RID: 553
		// (remove) Token: 0x0600022A RID: 554
		event PropertyChangedWithFloatValueEventHandler PropertyChangedWithFloatValue;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x0600022B RID: 555
		// (remove) Token: 0x0600022C RID: 556
		event PropertyChangedWithUIntValueEventHandler PropertyChangedWithUIntValue;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x0600022D RID: 557
		// (remove) Token: 0x0600022E RID: 558
		event PropertyChangedWithColorValueEventHandler PropertyChangedWithColorValue;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x0600022F RID: 559
		// (remove) Token: 0x06000230 RID: 560
		event PropertyChangedWithDoubleValueEventHandler PropertyChangedWithDoubleValue;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06000231 RID: 561
		// (remove) Token: 0x06000232 RID: 562
		event PropertyChangedWithVec2ValueEventHandler PropertyChangedWithVec2Value;
	}
}
