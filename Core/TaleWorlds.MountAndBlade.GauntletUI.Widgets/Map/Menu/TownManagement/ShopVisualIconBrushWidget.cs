using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.Menu.TownManagement
{
	public class ShopVisualIconBrushWidget : BrushWidget
	{
		public ShopVisualIconBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				string shopId = this.ShopId;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(shopId);
				if (num <= 1487947867U)
				{
					if (num <= 480574884U)
					{
						if (num != 53567524U)
						{
							if (num != 413646574U)
							{
								if (num == 480574884U)
								{
									if (shopId == "tannery")
									{
										this.SetState("Tannery");
										goto IL_314;
									}
								}
							}
							else if (shopId == "empty")
							{
								this.SetState("Default");
								goto IL_314;
							}
						}
						else if (shopId == "wool_weavery")
						{
							this.SetState("WoolWeavery");
							goto IL_314;
						}
					}
					else if (num <= 654125368U)
					{
						if (num != 570845286U)
						{
							if (num == 654125368U)
							{
								if (shopId == "olive_press")
								{
									this.SetState("OlivePress");
									goto IL_314;
								}
							}
						}
						else if (shopId == "silversmithy")
						{
							this.SetState("SilverSmithy");
							goto IL_314;
						}
					}
					else if (num != 1167966747U)
					{
						if (num == 1487947867U)
						{
							if (shopId == "velvet_weavery")
							{
								this.SetState("VelvetWeavery");
								goto IL_314;
							}
						}
					}
					else if (shopId == "pottery_shop")
					{
						this.SetState("PotteryShop");
						goto IL_314;
					}
				}
				else if (num <= 1841523956U)
				{
					if (num != 1527712715U)
					{
						if (num != 1741159131U)
						{
							if (num == 1841523956U)
							{
								if (shopId == "wood_WorkshopType")
								{
									this.SetState("WoodWorkshop");
									goto IL_314;
								}
							}
						}
						else if (shopId == "linen_weavery")
						{
							this.SetState("LinenWeavery");
							goto IL_314;
						}
					}
					else if (shopId == "smithy")
					{
						this.SetState("Smithy");
						goto IL_314;
					}
				}
				else if (num <= 3164387478U)
				{
					if (num != 2207150787U)
					{
						if (num == 3164387478U)
						{
							if (shopId == "stable")
							{
								this.SetState("Stable");
								goto IL_314;
							}
						}
					}
					else if (shopId == "mill")
					{
						this.SetState("Mill");
						goto IL_314;
					}
				}
				else if (num != 3470543696U)
				{
					if (num == 4109692093U)
					{
						if (shopId == "brewery")
						{
							this.SetState("Brewery");
							goto IL_314;
						}
					}
				}
				else if (shopId == "wine_press")
				{
					this.SetState("WinePress");
					goto IL_314;
				}
				this.SetState("Default");
				Debug.FailedAssert("No workshop visual with this type: " + this.ShopId, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\Map\\Menu\\TownManagement\\ShopVisualIconBrushWidget.cs", "OnLateUpdate", 68);
				IL_314:
				this._initialized = true;
			}
		}

		[Editor(false)]
		public string ShopId
		{
			get
			{
				return this._shopId;
			}
			set
			{
				if (this._shopId != value)
				{
					this._shopId = value;
					base.OnPropertyChanged<string>(value, "ShopId");
				}
			}
		}

		private bool _initialized;

		private string _shopId;
	}
}
