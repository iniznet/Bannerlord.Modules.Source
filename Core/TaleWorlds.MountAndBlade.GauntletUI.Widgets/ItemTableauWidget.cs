using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000024 RID: 36
	public class ItemTableauWidget : TextureWidget
	{
		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x00006E3C File Offset: 0x0000503C
		// (set) Token: 0x060001C3 RID: 451 RVA: 0x00006E44 File Offset: 0x00005044
		[Editor(false)]
		public string ItemModifierId
		{
			get
			{
				return this._itemModifierId;
			}
			set
			{
				if (value != this._itemModifierId)
				{
					this._itemModifierId = value;
					base.OnPropertyChanged<string>(value, "ItemModifierId");
					base.SetTextureProviderProperty("ItemModifierId", value);
				}
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001C4 RID: 452 RVA: 0x00006E73 File Offset: 0x00005073
		// (set) Token: 0x060001C5 RID: 453 RVA: 0x00006E7B File Offset: 0x0000507B
		[Editor(false)]
		public string StringId
		{
			get
			{
				return this._stringId;
			}
			set
			{
				if (value != this._stringId)
				{
					this._stringId = value;
					base.OnPropertyChanged<string>(value, "StringId");
					if (value != null)
					{
						base.SetTextureProviderProperty("StringId", value);
					}
				}
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001C6 RID: 454 RVA: 0x00006EAD File Offset: 0x000050AD
		// (set) Token: 0x060001C7 RID: 455 RVA: 0x00006EB5 File Offset: 0x000050B5
		[Editor(false)]
		public float InitialTiltRotation
		{
			get
			{
				return this._initialTiltRotation;
			}
			set
			{
				if (value != this._initialTiltRotation)
				{
					this._initialTiltRotation = value;
					base.OnPropertyChanged(value, "InitialTiltRotation");
					base.SetTextureProviderProperty("InitialTiltRotation", value);
				}
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x00006EE4 File Offset: 0x000050E4
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x00006EEC File Offset: 0x000050EC
		[Editor(false)]
		public float InitialPanRotation
		{
			get
			{
				return this._initialPanRotation;
			}
			set
			{
				if (value != this._initialPanRotation)
				{
					this._initialPanRotation = value;
					base.OnPropertyChanged(value, "InitialPanRotation");
					base.SetTextureProviderProperty("InitialPanRotation", value);
				}
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060001CA RID: 458 RVA: 0x00006F1B File Offset: 0x0000511B
		// (set) Token: 0x060001CB RID: 459 RVA: 0x00006F23 File Offset: 0x00005123
		[Editor(false)]
		public string BannerCode
		{
			get
			{
				return this._bannerCode;
			}
			set
			{
				if (value != this._bannerCode)
				{
					this._bannerCode = value;
					base.OnPropertyChanged<string>(value, "BannerCode");
					base.SetTextureProviderProperty("BannerCode", value);
				}
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00006F52 File Offset: 0x00005152
		public ItemTableauWidget(UIContext context)
			: base(context)
		{
			base.TextureProviderName = "ItemTableauTextureProvider";
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00006F66 File Offset: 0x00005166
		protected override void OnMousePressed()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", true);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00006F79 File Offset: 0x00005179
		protected override void OnRightStickMovement()
		{
			base.OnRightStickMovement();
			base.SetTextureProviderProperty("RotateItemVertical", base.EventManager.RightStickVerticalScrollAmount);
			base.SetTextureProviderProperty("RotateItemHorizontal", base.EventManager.RightStickHorizontalScrollAmount);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00006FB7 File Offset: 0x000051B7
		protected override void OnMouseReleased()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", false);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00006FCA File Offset: 0x000051CA
		protected override bool OnPreviewRightStickMovement()
		{
			return true;
		}

		// Token: 0x040000DD RID: 221
		private string _itemModifierId;

		// Token: 0x040000DE RID: 222
		private string _stringId;

		// Token: 0x040000DF RID: 223
		private float _initialTiltRotation;

		// Token: 0x040000E0 RID: 224
		private float _initialPanRotation;

		// Token: 0x040000E1 RID: 225
		private string _bannerCode;
	}
}
