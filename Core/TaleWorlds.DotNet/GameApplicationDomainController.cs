using System;
using System.Reflection;
using System.Runtime.InteropServices;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	public class GameApplicationDomainController : MarshalByRefObject
	{
		public GameApplicationDomainController(bool newApplicationDomain)
		{
			Debug.Print("Constructing GameApplicationDomainController.", 0, Debug.DebugColor.White, 17592186044416UL);
			GameApplicationDomainController._instance = this;
			this._newApplicationDomain = newApplicationDomain;
		}

		public GameApplicationDomainController()
		{
			Debug.Print("Constructing GameApplicationDomainController.", 0, Debug.DebugColor.White, 17592186044416UL);
			GameApplicationDomainController._instance = this;
			this._newApplicationDomain = true;
		}

		public void LoadAsHostedByNative(IntPtr passManagedInitializeMethodPointer, IntPtr passManagedCallbackMethodPointer, string gameApiDllName, string gameApiTypeName, Platform currentPlatform)
		{
			Delegate @delegate = (OneMethodPasserDelegate)Marshal.GetDelegateForFunctionPointer(passManagedInitializeMethodPointer, typeof(OneMethodPasserDelegate));
			Delegate delegate2 = (OneMethodPasserDelegate)Marshal.GetDelegateForFunctionPointer(passManagedCallbackMethodPointer, typeof(OneMethodPasserDelegate));
			this.Load(@delegate, delegate2, gameApiDllName, gameApiTypeName, currentPlatform);
		}

		public void Load(Delegate passManagedInitializeMethod, Delegate passManagedCallbackMethod, string gameApiDllName, string gameApiTypeName, Platform currentPlatform)
		{
			try
			{
				Common.SetInvariantCulture();
				GameApplicationDomainController._passManagedInitializeMethod = passManagedInitializeMethod;
				GameApplicationDomainController._passManagedCallbackMethod = passManagedCallbackMethod;
				Assembly assembly;
				if (this._newApplicationDomain)
				{
					assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.DotNet.dll", true);
				}
				else
				{
					assembly = base.GetType().Assembly;
				}
				Assembly assembly2 = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + gameApiDllName, true);
				if (assembly2 == null)
				{
					Console.WriteLine("gameApi is null");
				}
				Type type = assembly.GetType("TaleWorlds.DotNet.Managed");
				if (type == null)
				{
					Console.WriteLine("engineManagedType is null");
				}
				Type type2 = assembly2.GetType(gameApiTypeName);
				if (type2 == null)
				{
					Console.WriteLine("managedType is null");
				}
				type.GetMethod("PassInitializationMethodPointersForDotNet").Invoke(null, new object[]
				{
					GameApplicationDomainController._passManagedInitializeMethod,
					GameApplicationDomainController._passManagedCallbackMethod
				});
				type2.GetMethod("Start").Invoke(null, new object[0]);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.WriteLine(ex.GetType().Name);
				Console.WriteLine(ex.Message);
				if (ex.InnerException != null)
				{
					Console.WriteLine("-");
					Console.WriteLine(ex.InnerException.Message);
					if (ex.InnerException.InnerException != null)
					{
						Console.WriteLine("-");
						Console.WriteLine(ex.InnerException.InnerException.Message);
					}
				}
			}
		}

		private static Delegate _passManagedInitializeMethod;

		private static Delegate _passManagedCallbackMethod;

		private static GameApplicationDomainController _instance;

		private bool _newApplicationDomain;

		private delegate void InitializerDelegate(Delegate argument);
	}
}
