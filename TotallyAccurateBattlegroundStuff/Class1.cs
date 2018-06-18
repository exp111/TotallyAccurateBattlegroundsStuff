using Landfall.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TotallyAccurateBattlegroundsStuff
{
    public class Class1
    {
		public static GameObject _loadObject;

		public static void Load()
		{
			_loadObject = new GameObject();
			_loadObject.AddComponent<Hack>();
			UnityEngine.Object.DontDestroyOnLoad(_loadObject);
		}

		public static void Unload()
		{
			UnityEngine.Object.Destroy(_loadObject);
		}
    }
}