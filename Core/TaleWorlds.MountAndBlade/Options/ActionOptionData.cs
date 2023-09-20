using System;
using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade.Options
{
	// Token: 0x02000396 RID: 918
	public class ActionOptionData : IOptionData
	{
		// Token: 0x17000905 RID: 2309
		// (get) Token: 0x06003253 RID: 12883 RVA: 0x000D0FD7 File Offset: 0x000CF1D7
		// (set) Token: 0x06003254 RID: 12884 RVA: 0x000D0FDF File Offset: 0x000CF1DF
		public Action OnAction { get; private set; }

		// Token: 0x06003255 RID: 12885 RVA: 0x000D0FE8 File Offset: 0x000CF1E8
		public ActionOptionData(ManagedOptions.ManagedOptionsType managedType, Action onAction)
		{
			this._managedType = managedType;
			this.OnAction = onAction;
		}

		// Token: 0x06003256 RID: 12886 RVA: 0x000D0FFE File Offset: 0x000CF1FE
		public ActionOptionData(NativeOptions.NativeOptionsType nativeType, Action onAction)
		{
			this._nativeType = nativeType;
			this.OnAction = onAction;
		}

		// Token: 0x06003257 RID: 12887 RVA: 0x000D1014 File Offset: 0x000CF214
		public ActionOptionData(string optionTypeId, Action onAction)
		{
			this._actionOptionTypeId = optionTypeId;
			this._nativeType = NativeOptions.NativeOptionsType.None;
			this.OnAction = onAction;
		}

		// Token: 0x06003258 RID: 12888 RVA: 0x000D1031 File Offset: 0x000CF231
		public void Commit()
		{
		}

		// Token: 0x06003259 RID: 12889 RVA: 0x000D1033 File Offset: 0x000CF233
		public float GetDefaultValue()
		{
			return 0f;
		}

		// Token: 0x0600325A RID: 12890 RVA: 0x000D103A File Offset: 0x000CF23A
		public object GetOptionType()
		{
			if (this._nativeType != NativeOptions.NativeOptionsType.None)
			{
				return this._nativeType;
			}
			if (this._managedType != ManagedOptions.ManagedOptionsType.Language)
			{
				return this._managedType;
			}
			return this._actionOptionTypeId;
		}

		// Token: 0x0600325B RID: 12891 RVA: 0x000D106B File Offset: 0x000CF26B
		public float GetValue(bool forceRefresh)
		{
			return 0f;
		}

		// Token: 0x0600325C RID: 12892 RVA: 0x000D1072 File Offset: 0x000CF272
		public bool IsNative()
		{
			return this._nativeType != NativeOptions.NativeOptionsType.None;
		}

		// Token: 0x0600325D RID: 12893 RVA: 0x000D1080 File Offset: 0x000CF280
		public void SetValue(float value)
		{
		}

		// Token: 0x0600325E RID: 12894 RVA: 0x000D1082 File Offset: 0x000CF282
		public bool IsAction()
		{
			return this._nativeType == NativeOptions.NativeOptionsType.None && this._managedType == ManagedOptions.ManagedOptionsType.Language;
		}

		// Token: 0x0600325F RID: 12895 RVA: 0x000D1098 File Offset: 0x000CF298
		public ValueTuple<string, bool> GetIsDisabledAndReasonID()
		{
			return new ValueTuple<string, bool>(string.Empty, false);
		}

		// Token: 0x04001541 RID: 5441
		private ManagedOptions.ManagedOptionsType _managedType;

		// Token: 0x04001542 RID: 5442
		private NativeOptions.NativeOptionsType _nativeType;

		// Token: 0x04001543 RID: 5443
		private string _actionOptionTypeId;
	}
}
