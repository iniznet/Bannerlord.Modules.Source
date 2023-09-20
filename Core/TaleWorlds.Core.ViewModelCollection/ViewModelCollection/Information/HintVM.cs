using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	// Token: 0x02000017 RID: 23
	public class HintVM : TooltipVM
	{
		// Token: 0x06000105 RID: 261 RVA: 0x00003F76 File Offset: 0x00002176
		public override void OnShowTooltip(Type type, object[] args)
		{
			if (type == this._registeredType)
			{
				this.Text = args[0] as string;
				this._tryShowHint = true;
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00003F9B File Offset: 0x0000219B
		public override void OnHideTooltip()
		{
			this._tryShowHint = false;
			base.IsActive = false;
			this._currentDelay = 0f;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00003FB6 File Offset: 0x000021B6
		public override void Tick(float dt)
		{
			if (this._tryShowHint)
			{
				if (this._currentDelay > 0f)
				{
					base.IsActive = true;
					return;
				}
				this._currentDelay += dt;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000108 RID: 264 RVA: 0x00003FE3 File Offset: 0x000021E3
		// (set) Token: 0x06000109 RID: 265 RVA: 0x00003FEB File Offset: 0x000021EB
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (this._text != value)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		// Token: 0x04000067 RID: 103
		private readonly Type _registeredType = typeof(string);

		// Token: 0x04000068 RID: 104
		private const float _delay = 0f;

		// Token: 0x04000069 RID: 105
		private float _currentDelay;

		// Token: 0x0400006A RID: 106
		private bool _tryShowHint;

		// Token: 0x0400006B RID: 107
		private string _text = "";
	}
}
