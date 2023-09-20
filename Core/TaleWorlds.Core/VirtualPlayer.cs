using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.Core
{
	public class VirtualPlayer
	{
		public static Dictionary<Type, object> PeerComponents
		{
			get
			{
				return VirtualPlayer._peerComponents;
			}
		}

		static VirtualPlayer()
		{
			VirtualPlayer.FindPeerComponents();
		}

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

		private static void EnsurePeerTypeList<T>() where T : PeerComponent
		{
			if (!VirtualPlayer._peerComponents.ContainsKey(typeof(T)))
			{
				VirtualPlayer._peerComponents.Add(typeof(T), new List<T>());
			}
		}

		private static void EnsurePeerTypeList(Type type)
		{
			if (!VirtualPlayer._peerComponents.ContainsKey(type))
			{
				IList list = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { type })) as IList;
				VirtualPlayer._peerComponents.Add(type, list);
			}
		}

		public static List<T> Peers<T>() where T : PeerComponent
		{
			VirtualPlayer.EnsurePeerTypeList<T>();
			return VirtualPlayer._peerComponents[typeof(T)] as List<T>;
		}

		public static void Reset()
		{
			VirtualPlayer._peerComponents = new Dictionary<Type, object>();
		}

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

		public BodyProperties BodyProperties { get; set; }

		public int Race { get; set; }

		public bool IsFemale { get; set; }

		public PlayerId Id { get; set; }

		public int Index { get; private set; }

		public bool IsMine
		{
			get
			{
				return MBNetwork.MyPeer == this;
			}
		}

		public string UserName { get; private set; }

		public int ChosenBadgeIndex { get; set; }

		public VirtualPlayer(int index, string name, PlayerId playerID, ICommunicator communicator)
		{
			this._peerEntitySystem = new EntitySystem<PeerComponent>();
			this.UserName = name;
			this.Index = index;
			this.Id = playerID;
			this.Communicator = communicator;
		}

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

		public PeerComponent AddComponent(uint componentId)
		{
			return this.AddComponent(VirtualPlayer._peerComponentTypes[componentId]);
		}

		public PeerComponent GetComponent(uint componentId)
		{
			return this.GetComponent(VirtualPlayer._peerComponentTypes[componentId]);
		}

		public T GetComponent<T>() where T : PeerComponent
		{
			return this._peerEntitySystem.GetComponent<T>();
		}

		public PeerComponent GetComponent(Type peerComponentType)
		{
			return this._peerEntitySystem.GetComponent(peerComponentType);
		}

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

		public void RemoveComponent(PeerComponent component)
		{
			this._peerEntitySystem.RemoveComponent(component);
			(VirtualPlayer._peerComponents[component.GetType()] as IList).Remove(component);
			this.Communicator.OnRemoveComponent(component);
		}

		public void OnDisconnect()
		{
		}

		public void SynchronizeComponentsTo(VirtualPlayer peer)
		{
			foreach (PeerComponent peerComponent in this._peerEntitySystem.Components)
			{
				this.Communicator.OnSynchronizeComponentTo(peer, peerComponent);
			}
		}

		public void UpdateIndexForReconnectingPlayer(int playerIndex)
		{
			this.Index = playerIndex;
		}

		private const string DefaultPlayerBannerCode = "11.8.1.4345.4345.770.774.1.0.0.133.7.5.512.512.784.769.1.0.0";

		private static Dictionary<Type, object> _peerComponents = new Dictionary<Type, object>();

		private static Dictionary<Type, uint> _peerComponentIds;

		private static Dictionary<uint, Type> _peerComponentTypes;

		private string _bannerCode;

		public readonly ICommunicator Communicator;

		private EntitySystem<PeerComponent> _peerEntitySystem;

		public Dictionary<int, List<int>> UsedCosmetics;
	}
}
