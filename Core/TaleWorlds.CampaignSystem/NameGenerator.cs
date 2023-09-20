using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	public class NameGenerator
	{
		public static NameGenerator Current
		{
			get
			{
				return Campaign.Current.NameGenerator;
			}
		}

		public NameGenerator()
		{
			this._nameCodeAndCount = new Dictionary<int, int>();
		}

		internal void Initialize()
		{
			this.InitializePersonNames();
			this.InitializeNameCodeAndCountDictionary();
		}

		private void InitializeNameCodeAndCountDictionary()
		{
			foreach (Hero hero in Hero.AllAliveHeroes)
			{
				if (!hero.FirstName.HasSameValue(hero.Name))
				{
					this.AddName(hero.FirstName);
				}
				this.AddName(hero.Name);
			}
			foreach (Hero hero2 in Hero.DeadOrDisabledHeroes)
			{
				if (!hero2.FirstName.HasSameValue(hero2.Name))
				{
					this.AddName(hero2.FirstName);
				}
				this.AddName(hero2.Name);
			}
		}

		public void GenerateHeroNameAndHeroFullName(Hero hero, out TextObject firstName, out TextObject fullName, bool useDeterministicValues = true)
		{
			firstName = this.GenerateHeroFirstName(hero);
			fullName = this.GenerateHeroFullName(hero, firstName, useDeterministicValues);
		}

		private TextObject GenerateHeroFullName(Hero hero, TextObject heroFirstName, bool useDeterministicValues = true)
		{
			TextObject textObject = heroFirstName;
			Clan clan = hero.Clan;
			uint num = 0U;
			if (hero.IsNotable)
			{
				num = (uint)hero.HomeSettlement.Notables.ToList<Hero>().IndexOf(hero);
			}
			if (hero.IsWanderer)
			{
				textObject = hero.Template.Name.CopyTextObject();
			}
			else if (clan != null && (clan.IsMafia || clan.IsNomad || clan.IsUnderMercenaryService || clan.IsSect))
			{
				textObject = new TextObject("{=4z1t75be}{FIRSTNAME} of the {CLAN_NAME}", null);
				textObject.SetTextVariable("CLAN_NAME", hero.Clan.InformalName);
			}
			else if (hero.IsArtisan)
			{
				int num2 = this.SelectNameIndex(hero, NameGenerator.Current._artisanNames, num, useDeterministicValues);
				this.AddName(this._artisanNames[num2]);
				textObject = this._artisanNames[num2].CopyTextObject();
			}
			else if (hero.IsGangLeader)
			{
				int num3 = this.SelectNameIndex(hero, NameGenerator.Current._gangLeaderNames, num, useDeterministicValues);
				this.AddName(this._gangLeaderNames[num3]);
				textObject = NameGenerator.Current._gangLeaderNames[num3].CopyTextObject();
			}
			else if (hero.IsPreacher)
			{
				int num4 = this.SelectNameIndex(hero, NameGenerator.Current._preacherNames, num, useDeterministicValues);
				this.AddName(this._preacherNames[num4]);
				textObject = NameGenerator.Current._preacherNames[num4].CopyTextObject();
			}
			else if (hero.IsMerchant)
			{
				if (hero.HomeSettlement != null && hero.HomeSettlement.IsTown)
				{
					if (hero.OwnedWorkshops.Count > 0)
					{
						textObject = GameTexts.FindText("str_merchant_name", null);
						TextObject textObject2 = hero.OwnedWorkshops[0].WorkshopType.JobName.CopyTextObject();
						textObject.SetTextVariable("JOB_NAME", textObject2);
					}
					else
					{
						int num5 = this.SelectNameIndex(hero, NameGenerator.Current._merchantNames, num, useDeterministicValues);
						this.AddName(this._merchantNames[num5]);
						textObject = NameGenerator.Current._merchantNames[num5].CopyTextObject();
					}
				}
			}
			else if (hero.IsRuralNotable || hero.IsHeadman)
			{
				textObject = new TextObject("{=YTAdoNHW}{FIRSTNAME} of {VILLAGE_NAME}", null);
				textObject.SetTextVariable("VILLAGE_NAME", hero.HomeSettlement.Name);
			}
			textObject.SetTextVariable("FEMALE", hero.IsFemale ? 1 : 0);
			textObject.SetTextVariable("IMPERIAL", (hero.Culture.StringId == "empire") ? 1 : 0);
			textObject.SetTextVariable("COASTAL", (hero.Culture.StringId == "empire" || hero.Culture.StringId == "vlandia") ? 1 : 0);
			textObject.SetTextVariable("NORTHERN", (hero.Culture.StringId == "battania" || hero.Culture.StringId == "sturgia") ? 1 : 0);
			if (textObject != heroFirstName)
			{
				textObject.SetTextVariable("FIRSTNAME", heroFirstName);
			}
			else
			{
				textObject.SetTextVariable("FIRSTNAME", heroFirstName.ToString());
			}
			return textObject;
		}

		public TextObject GenerateHeroFirstName(Hero hero)
		{
			MBReadOnlyList<TextObject> nameListForCulture = this.GetNameListForCulture(hero.Culture, hero.IsFemale);
			int num = this.SelectNameIndex(hero, nameListForCulture, 0U, false);
			this.AddName(nameListForCulture[num]);
			return nameListForCulture[num].CopyTextObject();
		}

		public TextObject GenerateFirstNameForPlayer(CultureObject culture, bool isFemale)
		{
			MBReadOnlyList<TextObject> nameListForCulture = this.GetNameListForCulture(culture, isFemale);
			int num = MBRandom.NondeterministicRandomInt % nameListForCulture.Count;
			return nameListForCulture[num].CopyTextObject();
		}

		public TextObject GenerateClanName(CultureObject culture, Settlement clanOriginSettlement)
		{
			TextObject[] clanNameListForCulture = this.GetClanNameListForCulture(culture);
			Dictionary<TextObject, int> dictionary = new Dictionary<TextObject, int>();
			TextObject[] array = clanNameListForCulture;
			for (int i = 0; i < array.Length; i++)
			{
				TextObject clanNameElement = array[i];
				if (!dictionary.ContainsKey(clanNameElement))
				{
					int num = Clan.All.Count((Clan t) => t.Name.Equals(clanNameElement)) * 3;
					num += Clan.All.Count((Clan t) => t.Name.HasSameValue(clanNameElement));
					dictionary.Add(clanNameElement, num);
				}
				else
				{
					Debug.FailedAssert("Duplicate name in Clan Name list", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\NameGenerator.cs", "GenerateClanName", 196);
				}
			}
			int num2 = dictionary.Values.Max() + 1;
			List<ValueTuple<TextObject, float>> list = new List<ValueTuple<TextObject, float>>();
			foreach (TextObject textObject in dictionary.Keys)
			{
				list.Add(new ValueTuple<TextObject, float>(textObject, (float)(num2 - dictionary[textObject])));
			}
			int num3;
			MBRandom.ChooseWeighted<TextObject>(list, out num3);
			TextObject textObject2 = dictionary.ElementAt(num3).Key.CopyTextObject();
			if (culture.StringId.ToLower() == "vlandia")
			{
				textObject2.SetTextVariable("ORIGIN_SETTLEMENT", clanOriginSettlement.Name);
			}
			return textObject2;
		}

		private void InitializePersonNames()
		{
			this._imperialNamesMale = new MBList<TextObject>
			{
				new TextObject("{=aeLgc0cU}Acthon", null),
				new TextObject("{=tWDyWroN}Amnon", null),
				new TextObject("{=uTFjknE2}Andros", null),
				new TextObject("{=QjJAbaoT}Apys", null),
				new TextObject("{=zInIqBD0}Arenicos", null),
				new TextObject("{=W1uwgvAQ}Arion", null),
				new TextObject("{=5VzWRMTn}Artimendros", null),
				new TextObject("{=rGuYLmez}Ascyron", null),
				new TextObject("{=J1nh9YiN}Atilon", null),
				new TextObject("{=NBAGX54b}Avigos", null),
				new TextObject("{=9cDw6vPF}Cadomenos", null),
				new TextObject("{=lD1sl2XC}Camnon", null),
				new TextObject("{=LP7yYoGQ}Caribos", null),
				new TextObject("{=VhbasI9J}Castor", null),
				new TextObject("{=VG9ng2n2}Chandion", null),
				new TextObject("{=aezwJGvY}Chenon", null),
				new TextObject("{=acp2IcMs}Crotor", null),
				new TextObject("{=XKxeaF0I}Dalidos", null),
				new TextObject("{=pCBkH35Q}Danos", null),
				new TextObject("{=2OJEyP0d}Dasys", null),
				new TextObject("{=SoTUisL3}Deltisos", null),
				new TextObject("{=HfuAdGQX}Destor", null),
				new TextObject("{=lEHtUGed}Diocosos", null),
				new TextObject("{=5EiKEdi5}Dorion", null),
				new TextObject("{=yAZC6F7P}Ecsorios", null),
				new TextObject("{=rUvnbDi2}Encurion", null),
				new TextObject("{=KjzJjj5n}Eronys", null),
				new TextObject("{=gYit4RTe}Euchor", null),
				new TextObject("{=uW38UtSH}Eupitor", null),
				new TextObject("{=WfAuSpJq}Eutos", null),
				new TextObject("{=RyorjCXF}Galon", null),
				new TextObject("{=ZFlIT1tH}Ganimynos", null),
				new TextObject("{=f3hTGKIP}Garitops", null),
				new TextObject("{=iDxygnVF}Gerotheon", null),
				new TextObject("{=rFbAjSWM}Gorigos", null),
				new TextObject("{=4lEEqyCg}Jacorios", null),
				new TextObject("{=DwzrsJxS}Jamanys", null),
				new TextObject("{=IU63oxWD}Jemynon", null),
				new TextObject("{=t1OpnZph}Jeremos", null),
				new TextObject("{=katzUyMI}Joron", null),
				new TextObject("{=GWaJ7Ksq}Joculos", null),
				new TextObject("{=BCy1KpvR}Lacalion", null),
				new TextObject("{=6LMiTKZz}Lamenon", null),
				new TextObject("{=F13aYbuk}Lavalios", null),
				new TextObject("{=bHbR5Mgy}Losys", null),
				new TextObject("{=CbxLZdVg}Lycos", null),
				new TextObject("{=HCXHcBzT}Mattis", null),
				new TextObject("{=X21PahQn}Menaclys", null),
				new TextObject("{=tTuaiLS6}Meritor", null),
				new TextObject("{=qqW5n5Ox}Milos", null),
				new TextObject("{=5yFy4U4i}Morynon", null),
				new TextObject("{=ud6rhXbn}Mostiros", null),
				new TextObject("{=FF3dQKc1}Nethor", null),
				new TextObject("{=u1lTJoPO}Nemos", null),
				new TextObject("{=sDFb1PRI}Nortos", null),
				new TextObject("{=QGIAbglw}Obron", null),
				new TextObject("{=ZmQx4gT3}Olichor", null),
				new TextObject("{=pq0w8kry}Orachos", null),
				new TextObject("{=JFVseSa3}Oros", null),
				new TextObject("{=7UkzXWTQ}Osarios", null),
				new TextObject("{=cZIhxH9e}Pacarios", null),
				new TextObject("{=9sokmYMZ}Padmos", null),
				new TextObject("{=NXOOJs2X}Patrys", null),
				new TextObject("{=plx8kkxa}Pelicos", null),
				new TextObject("{=1cKxsHSh}Penton", null),
				new TextObject("{=vgBRa2BE}Poraclys", null),
				new TextObject("{=q186AQHt}Phadon", null),
				new TextObject("{=LMWiJi6V}Phirentos", null),
				new TextObject("{=XZxIpCQH}Phorys", null),
				new TextObject("{=5eTb2xr7}Sanion", null),
				new TextObject("{=Zm4a2xsf}Salusios", null),
				new TextObject("{=egCbjHDT}Semnon", null),
				new TextObject("{=GeJobcre}Sinor", null),
				new TextObject("{=E4WhEvu7}Sotherys", null),
				new TextObject("{=FClEZIkT}Sovos", null),
				new TextObject("{=rVUhYuvE}Suterios", null),
				new TextObject("{=Axe5FkET}Talison", null),
				new TextObject("{=PR4IqkTW}Temeon", null),
				new TextObject("{=9xyXdX4I}Tharos", null),
				new TextObject("{=zfUhnG7y}Themestios", null),
				new TextObject("{=P7iNhhPl}Turiados", null),
				new TextObject("{=kfNW01y5}Tynops", null),
				new TextObject("{=wbJzSg3X}Ulbesos", null),
				new TextObject("{=SRncaw79}Urios", null),
				new TextObject("{=pRbKbPK3}Vadrios", null),
				new TextObject("{=TiJOjUi5}Valaos", null),
				new TextObject("{=5nEh20ju}Vasylops", null),
				new TextObject("{=bWb245zO}Voleos", null),
				new TextObject("{=WTePYMNF}Zaraclys", null),
				new TextObject("{=p0hcyZdp}Zenon", null),
				new TextObject("{=3RBVn5yi}Zoros", null),
				new TextObject("{=uCyAnus4}Zostios", null)
			};
			this._imperialNamesFemale = new MBList<TextObject>
			{
				new TextObject("{=BNnLbOkN}Adinea", null),
				new TextObject("{=EGatdCLg}Alena", null),
				new TextObject("{=UPaP0B2L}Alchyla", null),
				new TextObject("{=QyXwJXIV}Andrasa", null),
				new TextObject("{=NM5f1Q6I}Ariada", null),
				new TextObject("{=bmHAYBwX}Catila", null),
				new TextObject("{=AdK4Ilzw}Chalia", null),
				new TextObject("{=xVSOKf0p}Chara", null),
				new TextObject("{=915frsPd}Corena", null),
				new TextObject("{=BNuZ6nvd}Daniria", null),
				new TextObject("{=YBGCXSEx}Debana", null),
				new TextObject("{=Oy5dH7gZ}Elea", null),
				new TextObject("{=rA3KybBX}Ethirea", null),
				new TextObject("{=qiTtJyHE}Gala", null),
				new TextObject("{=fGmpS0Dr}Gandarina", null),
				new TextObject("{=a6qaFH7L}Herena", null),
				new TextObject("{=LE9mRhSs}Hespedia", null),
				new TextObject("{=RHXdqjQY}Ilina", null),
				new TextObject("{=HSlPLC4m}Ira", null),
				new TextObject("{=028iCb8B}Jythea", null),
				new TextObject("{=6vtmYjTW}Jolanna", null),
				new TextObject("{=Ew69yN84}Juthys", null),
				new TextObject("{=Jif1C3X3}Laria", null),
				new TextObject("{=2oy7atk6}Lundana", null),
				new TextObject("{=dwFZFQ6V}Lysica", null),
				new TextObject("{=2fYYfHUI}Martira", null),
				new TextObject("{=Vxt0xTvV}Mavea", null),
				new TextObject("{=lgvtLEDA}Melchea", null),
				new TextObject("{=QvfUqzpF}Mina", null),
				new TextObject("{=11KckWau}Mitara", null),
				new TextObject("{=uLjRHv9p}Nadea", null),
				new TextObject("{=FUT6eXfw}Phaea", null),
				new TextObject("{=WrAMfIG1}Phenoria", null),
				new TextObject("{=XMTD2clw}Rhoe", null),
				new TextObject("{=0XEaaoah}Rosazia", null),
				new TextObject("{=L9weGfoX}Salea", null),
				new TextObject("{=nSoJkeBI}Sittacea", null),
				new TextObject("{=V1QLbhRl}Sora", null),
				new TextObject("{=b2aRoXsb}Tessa", null),
				new TextObject("{=bQKbW8Tx}Thasyna", null),
				new TextObject("{=CvVJyKYA}Thelea", null),
				new TextObject("{=VzhbUL60}Vendelia", null),
				new TextObject("{=a2ajWcI3}Viria", null),
				new TextObject("{=wbLqHvjE}Zerosica", null),
				new TextObject("{=zxZH2WbD}Zimena", null),
				new TextObject("{=AccBcEIt}Zoana", null)
			};
			this._preacherNames = new MBList<TextObject>
			{
				new TextObject("{=UuypR3B4}{FIRSTNAME} of the Gourd", null),
				new TextObject("{=T3x9hVg9}{FIRSTNAME} of the Chalice", null),
				new TextObject("{=N6DvC4hN}{FIRSTNAME} of the Mirror", null),
				new TextObject("{=QUFmBtbk}{FIRSTNAME} of the Sandal", null),
				new TextObject("{=1gMfbcaO}{FIRSTNAME} of the Staff", null),
				new TextObject("{=6M3mbAHQ}{FIRSTNAME} of the Rose", null),
				new TextObject("{=YYYBGdh5}{FIRSTNAME} of the Lamp", null),
				new TextObject("{=6qssnVgo}{FIRSTNAME} of the Pomegranate", null),
				new TextObject("{=TOGBPjmO}{FIRSTNAME} of the Seal", null),
				new TextObject("{=CQWKrd0a}{FIRSTNAME} of the Spinning-Wheel", null),
				new TextObject("{=6ALN43LY}{FIRSTNAME} of the Bell", null),
				new TextObject("{=TJKi43Hv}{FIRSTNAME} of the Scroll", null),
				new TextObject("{=tzuN76ma}{FIRSTNAME} of the Axe", null),
				new TextObject("{=fZXEqTIP}{FIRSTNAME} of the Plough", null),
				new TextObject("{=TVRbkuhC}{FIRSTNAME} of the Trident", null),
				new TextObject("{=SdK678BT}{FIRSTNAME} of the Cavern", null),
				new TextObject("{=bu2rmgY1}{FIRSTNAME} of the Willow-Tree", null),
				new TextObject("{=uyTmrmCW}{FIRSTNAME} of the Reeds", null),
				new TextObject("{=YYyoYwH2}{FIRSTNAME} of the Pasture", null),
				new TextObject("{=QskefraA}{FIRSTNAME} of the Ram", null),
				new TextObject("{=TrGGbtS4}{FIRSTNAME} of the Dove", null),
				new TextObject("{=glTzcivI}{FIRSTNAME} of the Spring", null),
				new TextObject("{=fYe25aEt}{FIRSTNAME} of the Well", null),
				new TextObject("{=TtaEimaV}{FIRSTNAME} of the Bridge", null),
				new TextObject("{=TaouqUu7}{FIRSTNAME} of the Steps", null),
				new TextObject("{=zrDWbEJR}{FIRSTNAME} of the Gate", null),
				new TextObject("{=xdmhzukY}{FIRSTNAME} of the Hearth", null),
				new TextObject("{=UBk50qwW}{FIRSTNAME} of the Mound", null),
				new TextObject("{=4t5zOiVF}{FIRSTNAME} of the Pillar", null),
				new TextObject("{=3raSG4Mi}{FIRSTNAME} of the Covenant", null),
				new TextObject("{=bP3XdKK3}{FIRSTNAME} of the Dawn", null),
				new TextObject("{=36ZmyM8V}{FIRSTNAME} of the Harvest", null),
				new TextObject("{=G6BC8HXY}{FIRSTNAME} of the Leavening", null)
			};
			this._merchantNames = new MBList<TextObject>
			{
				new TextObject("{=KQ1js10G}{FIRSTNAME} the Appraiser", null),
				new TextObject("{=4RWpqxwE}{FIRSTNAME} the Broker", null),
				new TextObject("{=nunbdOY1}{FIRSTNAME} the Supplier", null),
				new TextObject("{=3WYVggyD}{FIRSTNAME} the {?COASTAL}Mariner{?}Horsetrader{\\?}", null),
				new TextObject("{=iCSVZj2e}{FIRSTNAME} the {?NORTHERN}Far-Farer{?}Caravanner{\\?}", null),
				new TextObject("{=asePjBVy}{FIRSTNAME} the {?FEMALE}Freedwoman{?}Freedman{\\?}", null),
				new TextObject("{=KiUVswtx}{FIRSTNAME} the Mercer", null),
				new TextObject("{=wuMJobac}{FIRSTNAME} the Factor", null),
				new TextObject("{=Jin8cj45}{FIRSTNAME} the Minter", null),
				new TextObject("{=w290a2DV}{FIRSTNAME} the {?IMPERIAL}Sutler{?}Goodstrader{\\?}", null),
				new TextObject("{=npuC7IBM}{FIRSTNAME} the Dyer", null),
				new TextObject("{=tx7iJMnc}{FIRSTNAME} the Silkvendor", null),
				new TextObject("{=BC4BC0ZC}{FIRSTNAME} the Spicetrader", null),
				new TextObject("{=vp0FClX1}{FIRSTNAME} the Cargomaster", null),
				new TextObject("{=8trsbRav}{FIRSTNAME} the {?FEMALE}Widow{?}Orphan{\\?}", null),
				new TextObject("{=pbDr5JFs}{FIRSTNAME} the Steward", null),
				new TextObject("{=AhiGlNRG}{FIRSTNAME} the {?NORTHERN}Furtrader{?}Incensetrader{\\?}", null)
			};
			this._artisanNames = new MBList<TextObject>
			{
				new TextObject("{=3TIbxe5d}{FIRSTNAME} the Brewer", null),
				new TextObject("{=TX48zCzF}{FIRSTNAME} the Carpenter", null),
				new TextObject("{=KDOFexQb}{FIRSTNAME} the Chandler", null),
				new TextObject("{=Bsp30p3g}{FIRSTNAME} the Cooper", null),
				new TextObject("{=npuC7IBM}{FIRSTNAME} the Dyer", null),
				new TextObject("{=CpafrIbY}{FIRSTNAME} the Miller", null),
				new TextObject("{=kiJxwqVh}{FIRSTNAME} the Wheeler", null),
				new TextObject("{=tTFUSJoe}{FIRSTNAME} the Smith", null),
				new TextObject("{=zE3sKAb2}{FIRSTNAME} the Turner", null),
				new TextObject("{=gSmXyxue}{FIRSTNAME} the Tanner", null)
			};
			this._gangLeaderNames = new MBList<TextObject>
			{
				new TextObject("{=5utDJYUv}{FIRSTNAME} the Knife", null),
				new TextObject("{=TW4iKHCt}{FIRSTNAME} Foulbreath", null),
				new TextObject("{=7h3wBoIt}Bloody {FIRSTNAME}", null),
				new TextObject("{=kJlOvZEm}Boss {FIRSTNAME}", null),
				new TextObject("{=Oq3OFXyC}Lucky {FIRSTNAME}", null),
				new TextObject("{=AZbJuZwF}{FIRSTNAME} Knucklebones", null),
				new TextObject("{=yG0JIiaS}{FIRSTNAME} the Jackal", null),
				new TextObject("{=aa1lM2MV}{FIRSTNAME} the Angel", null),
				new TextObject("{=EUJlNTrf}Pretty {FIRSTNAME}", null),
				new TextObject("{=EnaT6Ma3}{FIRSTNAME} the Cat", null),
				new TextObject("{=Bk62qb7O}Ironskull {FIRSTNAME}", null),
				new TextObject("{=rFESkhK0}{FIRSTNAME} the Slicer", null),
				new TextObject("{=pL3s39hv}Clever {FIRSTNAME}", null),
				new TextObject("{=nNUZOwhb}Redeye {FIRSTNAME}", null),
				new TextObject("{=xudfzjgJ}Little {FIRSTNAME}", null),
				new TextObject("{=awCsv4UM}Tiny {FIRSTNAME}", null),
				new TextObject("{=u9LBrZnr}{FIRSTNAME} the Shark", null),
				new TextObject("{=uBT9fuIi}Snake-eyes {FIRSTNAME}", null),
				new TextObject("{=UAXaL9ro}Leadfoot {FIRSTNAME}", null),
				new TextObject("{=DCF2JOiJ}Stonehead {FIRSTNAME}", null),
				new TextObject("{=A5Gw3GNn}{FIRSTNAME} the Malady", null),
				new TextObject("{=aqp9ZtXb}{FIRSTNAME} the Wart", null),
				new TextObject("{=FrLta5zf}{FIRSTNAME} the Fist", null),
				new TextObject("{=L6N2YLa6}{FIRSTNAME} the Finger", null),
				new TextObject("{=VtjMGTWH}{FIRSTNAME} the Scorpion", null),
				new TextObject("{=3JOd0l1N}{FIRSTNAME} the Spider", null),
				new TextObject("{=ynwbmuoG}{FIRSTNAME} the Viper", null),
				new TextObject("{=K4MRSU6i}Sleepy {FIRSTNAME}", null),
				new TextObject("{=6jrl3Rbb}{FIRSTNAME} Fishsauce", null),
				new TextObject("{=6gjSupBN}{FIRSTNAME} Mutton-pie", null),
				new TextObject("{=y4vyNZxg}{FIRSTNAME} Sourwine", null),
				new TextObject("{=qhe6SGa3}{FIRSTNAME} Stewbones", null),
				new TextObject("{=c7cdMWA3}Buttermilk {FIRSTNAME}", null),
				new TextObject("{=bqXpBNvF}Cinnamon {FIRSTNAME}", null),
				new TextObject("{=lwLhrGWV}{FIRSTNAME} Flatcakes", null),
				new TextObject("{=r9Tp4UGy}Honeytongue {FIRSTNAME}", null),
				new TextObject("{=MRJ06SU7}{FIRSTNAME} the Thorn", null),
				new TextObject("{=6tBhhNaC}{FIRSTNAME} Rottentooth", null),
				new TextObject("{=Z48lYHBl}{FIRSTNAME} the Lamb", null),
				new TextObject("{=z8LbFyNA}Dogface {FIRSTNAME}", null),
				new TextObject("{=qezuzVuY}{FIRSTNAME} the Goat", null),
				new TextObject("{=JiAmC0NZ}{FIRSTNAME} the Mule", null),
				new TextObject("{=qmwv27To}{FIRSTNAME} the Mouse", null),
				new TextObject("{=ajePb62s}Quicksilver {FIRSTNAME}", null),
				new TextObject("{=3NROvpcO}Slowhand {FIRSTNAME}", null),
				new TextObject("{=zo13Dkoh}Crushfinger {FIRSTNAME}", null),
				new TextObject("{=9Sa3bzlE}{FIRSTNAME} the Anvil", null),
				new TextObject("{=FSa61zD4}{FIRSTNAME} the Hammer", null),
				new TextObject("{=WzBo28iT}{FIRSTNAME} the Scythe", null),
				new TextObject("{=MaK0r9as}{FIRSTNAME} the Cudgel", null),
				new TextObject("{=gbAztaSq}{FIRSTNAME} the Gutting-Knife", null),
				new TextObject("{=tI8aoxXC}{FIRSTNAME} the Needle", null),
				new TextObject("{=4ATx01zS}{FIRSTNAME} the Rock", null),
				new TextObject("{=1Tft1d4A}{FIRSTNAME} the Boulder", null),
				new TextObject("{=3qjJzjZb}{FIRSTNAME} the Beetle", null),
				new TextObject("{=0B6HlgnN}{FIRSTNAME} the Lizard", null),
				new TextObject("{=2wixoeOF}Hairy {FIRSTNAME}", null),
				new TextObject("{=NTPVzs9z}Poxy {FIRSTNAME}", null),
				new TextObject("{=chiIHo4b}Mangy {FIRSTNAME}", null),
				new TextObject("{=aIaRIsw4}Scabby {FIRSTNAME}", null),
				new TextObject("{=ubZmYdMn}Rancid {FIRSTNAME}", null),
				new TextObject("{=xTtHdTsS}Poison {FIRSTNAME}", null),
				new TextObject("{=uO99raT7}Snotnose {FIRSTNAME}", null),
				new TextObject("{=t968gMty}{FIRSTNAME} the {?FEMALE}Lady{?}Bastard{\\?}", null),
				new TextObject("{=lkLNrscj}{FIRSTNAME} the {?FEMALE}Maid{?}Steward{\\?}", null),
				new TextObject("{=ujDbk6Qa}{FIRSTNAME} the {?FEMALE}Widow{?}Widow-maker{\\?}", null),
				new TextObject("{=fh1auwJW}{FIRSTNAME} the {?FEMALE}She-Wolf{?}Stallion{\\?}", null)
			};
		}

		public MBReadOnlyList<TextObject> GetNameListForCulture(CultureObject npcCulture, bool isFemale)
		{
			MBReadOnlyList<TextObject> mbreadOnlyList = (isFemale ? this._imperialNamesFemale : this._imperialNamesMale);
			if (isFemale)
			{
				if (!npcCulture.FemaleNameList.IsEmpty<TextObject>())
				{
					mbreadOnlyList = npcCulture.FemaleNameList;
				}
			}
			else if (!npcCulture.MaleNameList.IsEmpty<TextObject>())
			{
				mbreadOnlyList = npcCulture.MaleNameList;
			}
			return mbreadOnlyList;
		}

		private TextObject[] GetClanNameListForCulture(CultureObject clanCulture)
		{
			TextObject[] array = null;
			if (!clanCulture.ClanNameList.IsEmpty<TextObject>())
			{
				array = clanCulture.ClanNameList.ToArray();
			}
			else
			{
				Debug.FailedAssert("Missing culture in clan name generation", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\NameGenerator.cs", "GetClanNameListForCulture", 810);
			}
			return array;
		}

		public void AddName(TextObject name)
		{
			int num = this.CreateNameCode(name);
			if (this._nameCodeAndCount == null)
			{
				return;
			}
			int num2;
			if (this._nameCodeAndCount.TryGetValue(num, out num2))
			{
				this._nameCodeAndCount[num] = num2 + 1;
				return;
			}
			this._nameCodeAndCount.Add(num, 1);
		}

		private int CreateNameCode(TextObject name)
		{
			return name.GetValueHashCode();
		}

		private int CalculateNameScore(Hero hero, TextObject name)
		{
			int num = 5000;
			IEnumerable<Hero> enumerable;
			if (hero != null)
			{
				if (hero.IsNotable)
				{
					if (hero.IsMerchant && hero.OwnedWorkshops.Count > 0)
					{
						List<Hero> list = new List<Hero>();
						foreach (Town town in Town.AllTowns)
						{
							foreach (Workshop workshop in town.Workshops)
							{
								if (workshop.Owner != hero && workshop.WorkshopType == hero.OwnedWorkshops[0].WorkshopType)
								{
									list.Add(workshop.Owner);
								}
							}
						}
						enumerable = list;
					}
					else
					{
						enumerable = hero.BornSettlement.Notables;
					}
				}
				else if (hero.Template != null && hero.Occupation == Occupation.Wanderer)
				{
					enumerable = Hero.AllAliveHeroes.WhereQ((Hero h) => hero.Template.Equals(h.Template));
				}
				else if (hero.Clan != null && hero.Occupation == Occupation.Lord)
				{
					enumerable = hero.Clan.Lords;
				}
				else
				{
					enumerable = new List<Hero>();
				}
			}
			else
			{
				enumerable = new List<Hero>();
			}
			foreach (Hero hero2 in enumerable)
			{
				if (hero2 != null)
				{
					if (name.HasSameValue(hero2.Name))
					{
						num -= 500;
					}
					if (name.HasSameValue(hero2.FirstName))
					{
						num -= 1000;
					}
				}
			}
			int num2;
			if (this._nameCodeAndCount.TryGetValue(this.CreateNameCode(name), out num2))
			{
				num -= num2;
			}
			return num;
		}

		private int SelectNameIndex(Hero hero, MBReadOnlyList<TextObject> nameList, uint deterministicIndex, bool useDeterministicValues)
		{
			int num = (useDeterministicValues ? hero.HomeSettlement.RandomIntWithSeed(deterministicIndex) : MBRandom.RandomInt()) % nameList.Count;
			int num2 = 0;
			int num3 = int.MinValue;
			for (int i = 0; i < nameList.Count; i++)
			{
				int num4 = (i + num) % nameList.Count;
				TextObject textObject = nameList[num4];
				int num5 = this.CalculateNameScore(hero, textObject);
				if (num3 < num5)
				{
					num3 = num5;
					num2 = num4;
				}
			}
			return num2;
		}

		private readonly Dictionary<int, int> _nameCodeAndCount;

		private MBList<TextObject> _imperialNamesMale;

		private MBList<TextObject> _imperialNamesFemale;

		private MBList<TextObject> _preacherNames;

		private MBList<TextObject> _merchantNames;

		private MBList<TextObject> _artisanNames;

		private MBList<TextObject> _gangLeaderNames;
	}
}
