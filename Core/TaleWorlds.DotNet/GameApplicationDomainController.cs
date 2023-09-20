using System;
using System.Reflection;
using System.Runtime.InteropServices;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000008 RID: 8
	public class GameApplicationDomainController : MarshalByRefObject
	{
		// Token: 0x0600001A RID: 26 RVA: 0x00002573 File Offset: 0x00000773
		public GameApplicationDomainController(bool newApplicationDomain)
		{
			Debug.Print("Constructing GameApplicationDomainController.", 0, Debug.DebugColor.White, 17592186044416UL);
			GameApplicationDomainController._instance = this;
			this._newApplicationDomain = newApplicationDomain;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000259E File Offset: 0x0000079E
		public GameApplicationDomainController()
		{
			Debug.Print("Constructing GameApplicationDomainController.", 0, Debug.DebugColor.White, 17592186044416UL);
			GameApplicationDomainController._instance = this;
			this._newApplicationDomain = true;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000025CC File Offset: 0x000007CC
		public void LoadAsHostedByNative(IntPtr passManagedInitializeMethodPointer, IntPtr passManagedCallbackMethodPointer, string gameApiDllName, string gameApiTypeName, Platform currentPlatform)
		{
			Delegate @delegate = (OneMethodPasserDelegate)Marshal.GetDelegateForFunctionPointer(passManagedInitializeMethodPointer, typeof(OneMethodPasserDelegate));
			Delegate delegate2 = (OneMethodPasserDelegate)Marshal.GetDelegateForFunctionPointer(passManagedCallbackMethodPointer, typeof(OneMethodPasserDelegate));
			this.Load(@delegate, delegate2, gameApiDllName, gameApiTypeName, currentPlatform);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002614 File Offset: 0x00000814
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

		// Token: 0x04000010 RID: 16
		private static Delegate _passManagedInitializeMethod;

		// Token: 0x04000011 RID: 17
		private static Delegate _passManagedCallbackMethod;

		// Token: 0x04000012 RID: 18
		private static GameApplicationDomainController _instance;

		// Token: 0x04000013 RID: 19
		private bool _newApplicationDomain;

		// Token: 0x02000033 RID: 51
		// (Invoke) Token: 0x0600012C RID: 300
		private delegate void InitializerDelegate(Delegate argument);
	}
}
