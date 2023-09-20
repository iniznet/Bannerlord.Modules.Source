using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.Core.ViewModelCollection.Information
{
	// Token: 0x0200001A RID: 26
	public class PropertyBasedTooltipVM : TooltipVM
	{
		// Token: 0x0600011E RID: 286 RVA: 0x00004208 File Offset: 0x00002408
		public PropertyBasedTooltipVM()
		{
			this.TooltipPropertyList = new MBBindingList<TooltipProperty>();
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00004228 File Offset: 0x00002428
		public override void OnShowTooltip(Type type, object[] args)
		{
			if (PropertyBasedTooltipVM._tooltipActions.ContainsKey(type) && (!(this._shownType == type) || this._typeArgs == null || !args.SequenceEqual(this._typeArgs) || !base.IsActive))
			{
				if (base.IsActive)
				{
					this.OnHideTooltip();
				}
				this._typeArgs = args;
				this._shownType = type;
				this._showTimer = 0.1f;
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00004295 File Offset: 0x00002495
		public override void OnHideTooltip()
		{
			base.IsActive = false;
			this._typeArgs = null;
			this._shownType = null;
			this._showTimer = -1f;
			this.TooltipPropertyList.Clear();
		}

		// Token: 0x06000121 RID: 289 RVA: 0x000042C2 File Offset: 0x000024C2
		public static void AddTooltipType(Type type, Action<PropertyBasedTooltipVM, object[]> action)
		{
			PropertyBasedTooltipVM._tooltipActions.Add(type, action);
		}

		// Token: 0x06000122 RID: 290 RVA: 0x000042D0 File Offset: 0x000024D0
		public static void AddKeyType(string keyID, Func<string> getKeyText)
		{
			PropertyBasedTooltipVM._keyTextGetters.Add(keyID, getKeyText);
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000042DE File Offset: 0x000024DE
		public string GetKeyText(string keyID)
		{
			if (PropertyBasedTooltipVM._keyTextGetters.ContainsKey(keyID))
			{
				return PropertyBasedTooltipVM._keyTextGetters[keyID]();
			}
			return "";
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00004304 File Offset: 0x00002504
		public void UpdateTooltip(List<TooltipProperty> list)
		{
			this.Mode = 0;
			foreach (TooltipProperty tooltipProperty in list.Where((TooltipProperty p) => (p.OnlyShowWhenExtended && base.IsExtended) || (!p.OnlyShowWhenExtended && p.OnlyShowWhenNotExtended && !base.IsExtended) || (!p.OnlyShowWhenExtended && !p.OnlyShowWhenNotExtended)))
			{
				this.AddPropertyDuplicate(tooltipProperty);
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00004364 File Offset: 0x00002564
		public override void Tick(float dt)
		{
			base.Tick(dt);
			if (this._showTimer >= 0f)
			{
				this._showTimer -= dt;
				if (this._showTimer < 0f)
				{
					this._showTimer = -1f;
					this.Refresh();
				}
			}
			if (base.IsActive && this._refreshTimer >= 0f)
			{
				this._refreshTimer -= dt;
				if (this._refreshTimer < 0f)
				{
					this._refreshTimer = 2f;
					foreach (TooltipProperty tooltipProperty in this.TooltipPropertyList)
					{
						tooltipProperty.RefreshDefinition();
						tooltipProperty.RefreshValue();
					}
				}
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00004430 File Offset: 0x00002630
		protected override void OnIsExtendedChanged()
		{
			if (base.IsActive)
			{
				base.IsActive = false;
				this.TooltipPropertyList.Clear();
				this.Refresh();
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00004454 File Offset: 0x00002654
		private void Refresh()
		{
			Action<PropertyBasedTooltipVM, object[]> action;
			if (this._shownType != null && PropertyBasedTooltipVM._tooltipActions.TryGetValue(this._shownType, out action))
			{
				action(this, this._typeArgs);
				if (this.TooltipPropertyList.Count > 0)
				{
					base.IsActive = true;
					return;
				}
			}
			else
			{
				string text = "Unsupported tooltip type: ";
				Type shownType = this._shownType;
				Debug.FailedAssert(text + ((shownType != null) ? shownType.Name : null), "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.Core.ViewModelCollection\\Information\\PropertyBasedTooltipVM.cs", "Refresh", 144);
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x000044D8 File Offset: 0x000026D8
		public void AddProperty(string definition, string value, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00004500 File Offset: 0x00002700
		public void AddModifierProperty(string definition, int modifierValue, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			string text = ((modifierValue > 0) ? ("+" + modifierValue.ToString()) : modifierValue.ToString());
			TooltipProperty tooltipProperty = new TooltipProperty(definition, text, textHeight, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00004544 File Offset: 0x00002744
		public void AddProperty(string definition, Func<string> value, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000456C File Offset: 0x0000276C
		public void AddProperty(Func<string> definition, Func<string> value, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00004594 File Offset: 0x00002794
		public void AddColoredProperty(string definition, string value, Color color, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			if (color == Colors.Black)
			{
				this.AddProperty(definition, value, textHeight, TooltipProperty.TooltipPropertyFlags.None);
				return;
			}
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, color, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000045D4 File Offset: 0x000027D4
		public void AddColoredProperty(string definition, Func<string> value, Color color, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			if (color == Colors.Black)
			{
				this.AddProperty(definition, value, textHeight, TooltipProperty.TooltipPropertyFlags.None);
				return;
			}
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, color, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00004614 File Offset: 0x00002814
		public void AddColoredProperty(Func<string> definition, Func<string> value, Color color, int textHeight = 0, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			if (color == Colors.Black)
			{
				this.AddProperty(definition, value, textHeight, TooltipProperty.TooltipPropertyFlags.None);
				return;
			}
			TooltipProperty tooltipProperty = new TooltipProperty(definition, value, textHeight, color, false, propertyFlags);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00004654 File Offset: 0x00002854
		private void AddPropertyDuplicate(TooltipProperty property)
		{
			TooltipProperty tooltipProperty = new TooltipProperty(property);
			this.TooltipPropertyList.Add(tooltipProperty);
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00004674 File Offset: 0x00002874
		// (set) Token: 0x06000131 RID: 305 RVA: 0x0000467C File Offset: 0x0000287C
		[DataSourceProperty]
		public MBBindingList<TooltipProperty> TooltipPropertyList
		{
			get
			{
				return this._tooltipPropertyList;
			}
			set
			{
				if (value != this._tooltipPropertyList)
				{
					this._tooltipPropertyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<TooltipProperty>>(value, "TooltipPropertyList");
				}
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000132 RID: 306 RVA: 0x0000469A File Offset: 0x0000289A
		// (set) Token: 0x06000133 RID: 307 RVA: 0x000046A2 File Offset: 0x000028A2
		[DataSourceProperty]
		public int Mode
		{
			get
			{
				return this._mode;
			}
			set
			{
				if (value != this._mode)
				{
					this._mode = value;
					base.OnPropertyChangedWithValue(value, "Mode");
				}
			}
		}

		// Token: 0x04000075 RID: 117
		private float _showTimer = -1f;

		// Token: 0x04000076 RID: 118
		private const float _showDelay = 0.1f;

		// Token: 0x04000077 RID: 119
		private float _refreshTimer;

		// Token: 0x04000078 RID: 120
		private const float _refreshDelay = 2f;

		// Token: 0x04000079 RID: 121
		private Type _shownType;

		// Token: 0x0400007A RID: 122
		private object[] _typeArgs;

		// Token: 0x0400007B RID: 123
		private static Dictionary<Type, Action<PropertyBasedTooltipVM, object[]>> _tooltipActions = new Dictionary<Type, Action<PropertyBasedTooltipVM, object[]>>();

		// Token: 0x0400007C RID: 124
		private static Dictionary<string, Func<string>> _keyTextGetters = new Dictionary<string, Func<string>>();

		// Token: 0x0400007D RID: 125
		private MBBindingList<TooltipProperty> _tooltipPropertyList;

		// Token: 0x0400007E RID: 126
		private int _mode;

		// Token: 0x0200002B RID: 43
		public enum TooltipMode
		{
			// Token: 0x040000C3 RID: 195
			DefaultGame,
			// Token: 0x040000C4 RID: 196
			DefaultCampaign,
			// Token: 0x040000C5 RID: 197
			Ally,
			// Token: 0x040000C6 RID: 198
			Enemy,
			// Token: 0x040000C7 RID: 199
			War
		}
	}
}
