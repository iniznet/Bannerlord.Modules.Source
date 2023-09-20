using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Core
{
	// Token: 0x020000C6 RID: 198
	public class VirtualPlayer
	{
		// Token: 0x17000335 RID: 821
		// (get) Token: 0x0600098E RID: 2446 RVA: 0x0001FA56 File Offset: 0x0001DC56
		public static Dictionary<Type, object> PeerComponents
		{
			get
			{
				return VirtualPlayer._peerComponents;
			}
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x0001FA5D File Offset: 0x0001DC5D
		static VirtualPlayer()
		{
			VirtualPlayer.FindPeerComponents();
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x0001FA70 File Offset: 0x0001DC70
		private static void FindPeerComponents()
		{
			Debug.Print("Searching Peer Components", 0, Debug.DebugColor.White, 17592186044416UL);
			VirtualPlayer._peerComponentIds = new Dictionary<Type, uint>();
			VirtualPlayer._peerComponentTypes = new Dictionary<uint, Type>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			List<Type> list = new List<Type>();
			foreach (Assembly assembly in assemblies)
			{
				if (VirtualPlayer.CheckAssemblyForPeerComponent(assembly))
				{
					Type[] types = assembly.GetTypes();
					list.AddRange(types.Where((Type q) => typeof(PeerComponent).IsAssignableFrom(q) && typeof(PeerComponent) != q));
				}
			}
			foreach (Type type in list)
			{
				uint djb = (uint)Common.GetDJB2(type.Name);
				VirtualPlayer._peerComponentIds.Add(type, djb);
				VirtualPlayer._peerComponentTypes.Add(djb, type);
			}
			Debug.Print("Found " + list.Count + " peer components", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x0001FB98 File Offset: 0x0001DD98
		private static bool CheckAssemblyForPeerComponent(Assembly assembly)
		{
			Assembly assembly2 = Assembly.GetAssembly(typeof(PeerComponent));
			if (assembly == assembly2)
			{
				return true;
			}
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < referencedAssemblies.Length; i++)
			{
				if (referencedAssemblies[i].FullName == assembly2.FullName)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x0001FBED File Offset: 0x0001DDED
		private static void EnsurePeerTypeList<T>() where T : PeerComponent
		{
			if (!VirtualPlayer._peerComponents.ContainsKey(typeof(T)))
			{
				VirtualPlayer._peerComponents.Add(typeof(T), new List<T>());
			}
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x0001FC20 File Offset: 0x0001DE20
		private static void EnsurePeerTypeList(Type type)
		{
			if (!VirtualPlayer._peerComponents.ContainsKey(type))
			{
				IList list = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { type })) as IList;
				VirtualPlayer._peerComponents.Add(type, list);
			}
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x0001FC6A File Offset: 0x0001DE6A
		public static List<T> Peers<T>() where T : PeerComponent
		{
			VirtualPlayer.EnsurePeerTypeList<T>();
			return VirtualPlayer._peerComponents[typeof(T)] as List<T>;
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x0001FC8A File Offset: 0x0001DE8A
		public static void Reset()
		{
			VirtualPlayer._peerComponents = new Dictionary<Type, object>();
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x06000996 RID: 2454 RVA: 0x0001FC96 File Offset: 0x0001DE96
		// (set) Token: 0x06000997 RID: 2455 RVA: 0x0001FCB1 File Offset: 0x0001DEB1
		public string BannerCode
		{
			get
			{
				if (this._bannerCode == null)
				{
					this._bannerCode = "11.8.1.4345.4345.770.774.1.0.0.133.7.5.512.512.784.769.1.0.0";
				}
				return this._bannerCode;
			}
			set
			{
				this._bannerCode = value;
			}
		}

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x06000998 RID: 2456 RVA: 0x0001FCBA File Offset: 0x0001DEBA
		// (set) Token: 0x06000999 RID: 2457 RVA: 0x0001FCC2 File Offset: 0x0001DEC2
		public BodyProperties BodyProperties { get; set; }

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x0600099A RID: 2458 RVA: 0x0001FCCB File Offset: 0x0001DECB
		// (set) Token: 0x0600099B RID: 2459 RVA: 0x0001FCD3 File Offset: 0x0001DED3
		public int Race { get; set; }

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x0600099C RID: 2460 RVA: 0x0001FCDC File Offset: 0x0001DEDC
		// (set) Token: 0x0600099D RID: 2461 RVA: 0x0001FCE4 File Offset: 0x0001DEE4
		public bool IsFemale { get; set; }

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x0600099E RID: 2462 RVA: 0x0001FCED File Offset: 0x0001DEED
		// (set) Token: 0x0600099F RID: 2463 RVA: 0x0001FCF5 File Offset: 0x0001DEF5
		public PlayerId Id { get; set; }

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x060009A0 RID: 2464 RVA: 0x0001FCFE File Offset: 0x0001DEFE
		// (set) Token: 0x060009A1 RID: 2465 RVA: 0x0001FD06 File Offset: 0x0001DF06
		public int Index { get; private set; }

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x060009A2 RID: 2466 RVA: 0x0001FD0F File Offset: 0x0001DF0F
		public bool IsMine
		{
			get
			{
				return MBNetwork.MyPeer == this;
			}
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x060009A3 RID: 2467 RVA: 0x0001FD19 File Offset: 0x0001DF19
		// (set) Token: 0x060009A4 RID: 2468 RVA: 0x0001FD21 File Offset: 0x0001DF21
		public string UserName { get; private set; }

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x060009A5 RID: 2469 RVA: 0x0001FD2A File Offset: 0x0001DF2A
		// (set) Token: 0x060009A6 RID: 2470 RVA: 0x0001FD32 File Offset: 0x0001DF32
		public int ChosenBadgeIndex { get; set; }

		// Token: 0x060009A7 RID: 2471 RVA: 0x0001FD3B File Offset: 0x0001DF3B
		public VirtualPlayer(int index, string name, PlayerId playerID, ICommunicator communicator)
		{
			this._peerEntitySystem = new EntitySystem<PeerComponent>();
			this.UserName = name;
			this.Index = index;
			this.Id = playerID;
			this.Communicator = communicator;
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x0001FD6C File Offset: 0x0001DF6C
		public T AddComponent<T>() where T : PeerComponent, new()
		{
			T t = this._peerEntitySystem.AddComponent<T>();
			t.Peer = this;
			t.TypeId = VirtualPlayer._peerComponentIds[typeof(T)];
			VirtualPlayer.EnsurePeerTypeList<T>();
			(VirtualPlayer._peerComponents[typeof(T)] as List<T>).Add(t);
			this.Communicator.OnAddComponent(t);
			t.Initialize();
			return t;
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x0001FDF4 File Offset: 0x0001DFF4
		public PeerComponent AddComponent(Type peerComponentType)
		{
			PeerComponent peerComponent = this._peerEntitySystem.AddComponent(peerComponentType);
			peerComponent.Peer = this;
			peerComponent.TypeId = VirtualPlayer._peerComponentIds[peerComponentType];
			VirtualPlayer.EnsurePeerTypeList(peerComponentType);
			(VirtualPlayer._peerComponents[peerComponentType] as IList).Add(peerComponent);
			this.Communicator.OnAddComponent(peerComponent);
			peerComponent.Initialize();
			return peerComponent;
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x0001FE56 File Offset: 0x0001E056
		public PeerComponent AddComponent(uint componentId)
		{
			return this.AddComponent(VirtualPlayer._peerComponentTypes[componentId]);
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x0001FE69 File Offset: 0x0001E069
		public PeerComponent GetComponent(uint componentId)
		{
			return this.GetComponent(VirtualPlayer._peerComponentTypes[componentId]);
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x0001FE7C File Offset: 0x0001E07C
		public T GetComponent<T>() where T : PeerComponent
		{
			return this._peerEntitySystem.GetComponent<T>();
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x0001FE89 File Offset: 0x0001E089
		public PeerComponent GetComponent(Type peerComponentType)
		{
			return this._peerEntitySystem.GetComponent(peerComponentType);
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x0001FE98 File Offset: 0x0001E098
		public void RemoveComponent<T>(bool synched = true) where T : PeerComponent
		{
			T component = this._peerEntitySystem.GetComponent<T>();
			if (component != null)
			{
				this._peerEntitySystem.RemoveComponent(component);
				(VirtualPlayer._peerComponents[typeof(T)] as List<T>).Remove(component);
				if (synched)
				{
					this.Communicator.OnRemoveComponent(component);
				}
			}
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x0001FEFE File Offset: 0x0001E0FE
		public void RemoveComponent(PeerComponent component)
		{
			this._peerEntitySystem.RemoveComponent(component);
			(VirtualPlayer._peerComponents[component.GetType()] as IList).Remove(component);
			this.Communicator.OnRemoveComponent(component);
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x0001FF33 File Offset: 0x0001E133
		public void OnDisconnect()
		{
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x0001FF38 File Offset: 0x0001E138
		public void SynchronizeComponentsTo(VirtualPlayer peer)
		{
			foreach (PeerComponent peerComponent in this._peerEntitySystem.Components)
			{
				this.Communicator.OnSynchronizeComponentTo(peer, peerComponent);
			}
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x0001FF98 File Offset: 0x0001E198
		public void UpdateIndexForReconnectingPlayer(int playerIndex)
		{
			this.Index = playerIndex;
		}

		// Token: 0x040005A9 RID: 1449
		private const string DefaultPlayerBannerCode = "11.8.1.4345.4345.770.774.1.0.0.133.7.5.512.512.784.769.1.0.0";

		// Token: 0x040005AA RID: 1450
		private static Dictionary<Type, object> _peerComponents = new Dictionary<Type, object>();

		// Token: 0x040005AB RID: 1451
		private static Dictionary<Type, uint> _peerComponentIds;

		// Token: 0x040005AC RID: 1452
		private static Dictionary<uint, Type> _peerComponentTypes;

		// Token: 0x040005AD RID: 1453
		private string _bannerCode;

		// Token: 0x040005B2 RID: 1458
		public readonly ICommunicator Communicator;

		// Token: 0x040005B3 RID: 1459
		private EntitySystem<PeerComponent> _peerEntitySystem;

		// Token: 0x040005B7 RID: 1463
		public Dictionary<int, List<int>> UsedCosmetics;
	}
}
