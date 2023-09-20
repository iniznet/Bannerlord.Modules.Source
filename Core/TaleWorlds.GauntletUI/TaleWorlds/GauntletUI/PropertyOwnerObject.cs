using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;

namespace TaleWorlds.GauntletUI
{
	// Token: 0x02000036 RID: 54
	public class PropertyOwnerObject
	{
		// Token: 0x06000383 RID: 899 RVA: 0x0000EFDE File Offset: 0x0000D1DE
		protected void OnPropertyChanged<T>(T value, [CallerMemberName] string propertyName = null) where T : class
		{
			Action<PropertyOwnerObject, string, object> propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, propertyName, value);
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0000EFF8 File Offset: 0x0000D1F8
		protected void OnPropertyChanged(int value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, int> action = this.intPropertyChanged;
			if (action == null)
			{
				return;
			}
			action(this, propertyName, value);
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0000F00D File Offset: 0x0000D20D
		protected void OnPropertyChanged(float value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, float> action = this.floatPropertyChanged;
			if (action == null)
			{
				return;
			}
			action(this, propertyName, value);
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0000F022 File Offset: 0x0000D222
		protected void OnPropertyChanged(bool value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, bool> action = this.boolPropertyChanged;
			if (action == null)
			{
				return;
			}
			action(this, propertyName, value);
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0000F037 File Offset: 0x0000D237
		protected void OnPropertyChanged(Vec2 value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, Vec2> vec2PropertyChanged = this.Vec2PropertyChanged;
			if (vec2PropertyChanged == null)
			{
				return;
			}
			vec2PropertyChanged(this, propertyName, value);
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0000F04C File Offset: 0x0000D24C
		protected void OnPropertyChanged(Vector2 value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, Vector2> vector2PropertyChanged = this.Vector2PropertyChanged;
			if (vector2PropertyChanged == null)
			{
				return;
			}
			vector2PropertyChanged(this, propertyName, value);
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0000F061 File Offset: 0x0000D261
		protected void OnPropertyChanged(double value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, double> action = this.doublePropertyChanged;
			if (action == null)
			{
				return;
			}
			action(this, propertyName, value);
		}

		// Token: 0x0600038A RID: 906 RVA: 0x0000F076 File Offset: 0x0000D276
		protected void OnPropertyChanged(uint value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, uint> action = this.uintPropertyChanged;
			if (action == null)
			{
				return;
			}
			action(this, propertyName, value);
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0000F08B File Offset: 0x0000D28B
		protected void OnPropertyChanged(Color value, [CallerMemberName] string propertyName = null)
		{
			Action<PropertyOwnerObject, string, Color> colorPropertyChanged = this.ColorPropertyChanged;
			if (colorPropertyChanged == null)
			{
				return;
			}
			colorPropertyChanged(this, propertyName, value);
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600038C RID: 908 RVA: 0x0000F0A0 File Offset: 0x0000D2A0
		// (remove) Token: 0x0600038D RID: 909 RVA: 0x0000F0D8 File Offset: 0x0000D2D8
		public event Action<PropertyOwnerObject, string, object> PropertyChanged;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600038E RID: 910 RVA: 0x0000F110 File Offset: 0x0000D310
		// (remove) Token: 0x0600038F RID: 911 RVA: 0x0000F148 File Offset: 0x0000D348
		public event Action<PropertyOwnerObject, string, bool> boolPropertyChanged;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000390 RID: 912 RVA: 0x0000F180 File Offset: 0x0000D380
		// (remove) Token: 0x06000391 RID: 913 RVA: 0x0000F1B8 File Offset: 0x0000D3B8
		public event Action<PropertyOwnerObject, string, int> intPropertyChanged;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000392 RID: 914 RVA: 0x0000F1F0 File Offset: 0x0000D3F0
		// (remove) Token: 0x06000393 RID: 915 RVA: 0x0000F228 File Offset: 0x0000D428
		public event Action<PropertyOwnerObject, string, float> floatPropertyChanged;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000394 RID: 916 RVA: 0x0000F260 File Offset: 0x0000D460
		// (remove) Token: 0x06000395 RID: 917 RVA: 0x0000F298 File Offset: 0x0000D498
		public event Action<PropertyOwnerObject, string, Vec2> Vec2PropertyChanged;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000396 RID: 918 RVA: 0x0000F2D0 File Offset: 0x0000D4D0
		// (remove) Token: 0x06000397 RID: 919 RVA: 0x0000F308 File Offset: 0x0000D508
		public event Action<PropertyOwnerObject, string, Vector2> Vector2PropertyChanged;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000398 RID: 920 RVA: 0x0000F340 File Offset: 0x0000D540
		// (remove) Token: 0x06000399 RID: 921 RVA: 0x0000F378 File Offset: 0x0000D578
		public event Action<PropertyOwnerObject, string, double> doublePropertyChanged;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x0600039A RID: 922 RVA: 0x0000F3B0 File Offset: 0x0000D5B0
		// (remove) Token: 0x0600039B RID: 923 RVA: 0x0000F3E8 File Offset: 0x0000D5E8
		public event Action<PropertyOwnerObject, string, uint> uintPropertyChanged;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x0600039C RID: 924 RVA: 0x0000F420 File Offset: 0x0000D620
		// (remove) Token: 0x0600039D RID: 925 RVA: 0x0000F458 File Offset: 0x0000D658
		public event Action<PropertyOwnerObject, string, Color> ColorPropertyChanged;
	}
}
