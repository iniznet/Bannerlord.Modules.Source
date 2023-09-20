using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	public class MultiplayerArmoryCosmeticCategoryButtonWidget : ButtonWidget
	{
		public MultiplayerArmoryCosmeticCategoryButtonWidget(UIContext context)
			: base(context)
		{
			this.CosmeticTypeName = string.Empty;
			this.CosmeticCategoryName = string.Empty;
		}

		private void UpdateCategorySprite()
		{
			if (string.IsNullOrEmpty(this.CosmeticCategoryName) || string.IsNullOrEmpty(this.CosmeticTypeName))
			{
				return;
			}
			Sprite sprite = null;
			if (this.CosmeticTypeName == "Clothing")
			{
				sprite = this.GetClothingCategorySprite(this.CosmeticCategoryName);
			}
			else if (this.CosmeticTypeName == "Taunt")
			{
				sprite = this.GetTauntCategorySprite(this.CosmeticCategoryName);
			}
			if (sprite != null)
			{
				base.Brush.DefaultLayer.Sprite = sprite;
				base.Brush.Sprite = sprite;
			}
		}

		private Sprite GetClothingCategorySprite(string clothingCategory)
		{
			Brush clothingCategorySpriteBrush = this.ClothingCategorySpriteBrush;
			if (clothingCategorySpriteBrush == null)
			{
				return null;
			}
			BrushLayer layer = clothingCategorySpriteBrush.GetLayer(clothingCategory);
			if (layer == null)
			{
				return null;
			}
			return layer.Sprite;
		}

		private Sprite GetTauntCategorySprite(string tauntCategory)
		{
			Brush tauntCategorySpriteBrush = this.TauntCategorySpriteBrush;
			if (tauntCategorySpriteBrush == null)
			{
				return null;
			}
			BrushLayer layer = tauntCategorySpriteBrush.GetLayer(tauntCategory);
			if (layer == null)
			{
				return null;
			}
			return layer.Sprite;
		}

		[DataSourceProperty]
		public Brush ClothingCategorySpriteBrush
		{
			get
			{
				return this._clothingCategorySpriteBrush;
			}
			set
			{
				if (value != this._clothingCategorySpriteBrush)
				{
					this._clothingCategorySpriteBrush = value;
					base.OnPropertyChanged<Brush>(value, "ClothingCategorySpriteBrush");
					this.UpdateCategorySprite();
				}
			}
		}

		[DataSourceProperty]
		public Brush TauntCategorySpriteBrush
		{
			get
			{
				return this._tauntCategorySpriteBrush;
			}
			set
			{
				if (value != this._tauntCategorySpriteBrush)
				{
					this._tauntCategorySpriteBrush = value;
					base.OnPropertyChanged<Brush>(value, "TauntCategorySpriteBrush");
					this.UpdateCategorySprite();
				}
			}
		}

		[DataSourceProperty]
		public string CosmeticTypeName
		{
			get
			{
				return this._cosmeticTypeName;
			}
			set
			{
				if (value != this._cosmeticTypeName)
				{
					this._cosmeticTypeName = value;
					base.OnPropertyChanged<string>(value, "CosmeticTypeName");
					this.UpdateCategorySprite();
				}
			}
		}

		[DataSourceProperty]
		public string CosmeticCategoryName
		{
			get
			{
				return this._cosmeticCategoryName;
			}
			set
			{
				if (value != this._cosmeticCategoryName)
				{
					this._cosmeticCategoryName = value;
					base.OnPropertyChanged<string>(value, "CosmeticCategoryName");
					this.UpdateCategorySprite();
				}
			}
		}

		private const string _clothingTypeName = "Clothing";

		private const string _tauntTypeName = "Taunt";

		private Brush _clothingCategorySpriteBrush;

		private Brush _tauntCategorySpriteBrush;

		private string _cosmeticTypeName;

		private string _cosmeticCategoryName;
	}
}
