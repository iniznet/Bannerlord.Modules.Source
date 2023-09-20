using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	// Token: 0x02000038 RID: 56
	public class MPChatLineVM : ViewModel
	{
		// Token: 0x060004A7 RID: 1191 RVA: 0x000150AC File Offset: 0x000132AC
		public MPChatLineVM(string chatLine, Color color, string category)
		{
			this.ChatLine = chatLine;
			this.Color = color;
			this.Alpha = 1f;
			this.Category = category;
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x000150D4 File Offset: 0x000132D4
		public void HandleFading(float dt)
		{
			this._timeSinceCreation += dt;
			this.RefreshAlpha();
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x000150EA File Offset: 0x000132EA
		private void RefreshAlpha()
		{
			if (this._forcedVisible)
			{
				this.Alpha = 1f;
				return;
			}
			this.Alpha = this.GetActualAlpha();
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x0001510C File Offset: 0x0001330C
		public void ForceInvisible()
		{
			this._timeSinceCreation = 10.5f;
			this.Alpha = 0f;
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00015124 File Offset: 0x00013324
		private float GetActualAlpha()
		{
			if (this._timeSinceCreation >= 10f)
			{
				return MBMath.ClampFloat(1f - (this._timeSinceCreation - 10f) / 0.5f, 0f, 1f);
			}
			return 1f;
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00015160 File Offset: 0x00013360
		public void ToggleForceVisible(bool visible)
		{
			this._forcedVisible = visible;
			this.RefreshAlpha();
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060004AD RID: 1197 RVA: 0x0001516F File Offset: 0x0001336F
		// (set) Token: 0x060004AE RID: 1198 RVA: 0x00015177 File Offset: 0x00013377
		[DataSourceProperty]
		public string ChatLine
		{
			get
			{
				return this._chatLine;
			}
			set
			{
				if (this._chatLine != value)
				{
					this._chatLine = value;
					base.OnPropertyChangedWithValue<string>(value, "ChatLine");
				}
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060004AF RID: 1199 RVA: 0x0001519A File Offset: 0x0001339A
		// (set) Token: 0x060004B0 RID: 1200 RVA: 0x000151A2 File Offset: 0x000133A2
		[DataSourceProperty]
		public Color Color
		{
			get
			{
				return this._color;
			}
			set
			{
				if (this._color != value)
				{
					this._color = value;
					base.OnPropertyChangedWithValue(value, "Color");
				}
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x060004B1 RID: 1201 RVA: 0x000151C5 File Offset: 0x000133C5
		// (set) Token: 0x060004B2 RID: 1202 RVA: 0x000151CD File Offset: 0x000133CD
		[DataSourceProperty]
		public float Alpha
		{
			get
			{
				return this._alpha;
			}
			set
			{
				if (this._alpha != value)
				{
					this._alpha = value;
					base.OnPropertyChangedWithValue(value, "Alpha");
				}
			}
		}

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x060004B3 RID: 1203 RVA: 0x000151EB File Offset: 0x000133EB
		// (set) Token: 0x060004B4 RID: 1204 RVA: 0x000151F3 File Offset: 0x000133F3
		[DataSourceProperty]
		public string Category
		{
			get
			{
				return this._category;
			}
			set
			{
				if (this._category != value)
				{
					this._category = value;
					base.OnPropertyChangedWithValue<string>(value, "Category");
				}
			}
		}

		// Token: 0x0400025E RID: 606
		private bool _forcedVisible;

		// Token: 0x0400025F RID: 607
		private string _category;

		// Token: 0x04000260 RID: 608
		private const float ChatVisibilityDuration = 10f;

		// Token: 0x04000261 RID: 609
		private const float ChatFadeOutDuration = 0.5f;

		// Token: 0x04000262 RID: 610
		private float _timeSinceCreation;

		// Token: 0x04000263 RID: 611
		private string _chatLine;

		// Token: 0x04000264 RID: 612
		private Color _color;

		// Token: 0x04000265 RID: 613
		private float _alpha;
	}
}
