using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Landfall.Network;
using System.Reflection;
using HighlightingSystem;
using Harmony;

namespace TotallyAccurateBattlegroundsStuff
{
	class Hack : MonoBehaviour
	{
		public bool _menuVisible = true;
		public Rect _menuRect = new Rect(10, 10, 300, 200);
		
		public bool _itemMenuVisible = false;
		public Rect _itemMenuRect = new Rect(350, 10, 250, 500);
		public Vector2 _itemScrollPosition = Vector2.zero;

		public static List<Player> _players = new List<Player>();
		public static List<Pickup> _items = new List<Pickup>();

		public bool _ESP = true;
		public bool _distanceESP = true;
		public bool _itemESP = true;
		public float _speedHack = 1.0f;

		void Start()
		{
			Logger.Log("Hack: Start");

			var harmony = HarmonyInstance.Create("exp.tabgstuff");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		void Update()
		{
			//Menu Toggle
			if (Input.GetKeyDown(KeyCode.Insert))
			{
				_menuVisible = !_menuVisible;
			}
			
			//Anti Anti Cheat
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.ActDetectorBase>());
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.InjectionDetector>());
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector>());
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.SpeedHackDetector>());
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.TimeCheatingDetector>());
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.WallHackDetector>());

			if (Player.localPlayer != null)
			{
				if (_players.Count == 0)
					_players.AddRange(FindObjectsOfType<Player>());

				if (_items.Count == 0)
					_items.AddRange(FindObjectsOfType<Pickup>());

				//No Recoil + Screenshake
				DestroyObject(FindObjectOfType<Recoil>());
				DestroyObject(FindObjectOfType<AddScreenShake>());

				//unlim ammo + firemodes + rapidfire + nospread
				Player.localPlayer.m_inventory.weaponHandler.rightWeapon.gun.bullets = 10;
				Player.localPlayer.m_inventory.weaponHandler.rightWeapon.gun.hasFullAuto = true;
				Player.localPlayer.m_inventory.weaponHandler.rightWeapon.gun.hasSingleFire = true;
				Player.localPlayer.m_inventory.weaponHandler.rightWeapon.gun.rateOfFire = 0;
				Player.localPlayer.m_inventory.weaponHandler.rightWeapon.gun.extraSpread = 0;

				Player.localPlayer.m_inventory.weaponHandler.leftWeapon.gun.bullets = 10;
				Player.localPlayer.m_inventory.weaponHandler.leftWeapon.gun.hasFullAuto = true;
				Player.localPlayer.m_inventory.weaponHandler.leftWeapon.gun.hasSingleFire = true;
				Player.localPlayer.m_inventory.weaponHandler.leftWeapon.gun.rateOfFire = 0;
				Player.localPlayer.m_inventory.weaponHandler.leftWeapon.gun.extraSpread = 0;

				Time.timeScale = _speedHack;

				//Don't seem to work
				/*Player.localPlayer.m_playerDeath.health = 100;
				Player.localPlayer.m_playerDeath.isDown = false;
				Player.localPlayer.m_playerDeath.dead = false;*/
				Player.localPlayer.GetComponent<PlayerDeath>().AssignNewHealth(100);
			}
		}

		void OnGUI()
		{
			if (_menuVisible)
			{
				_menuRect = GUI.Window(1337, _menuRect, MenuFunction, "TABG Hax");

				if (_itemMenuVisible)
					_itemMenuRect = GUI.Window(1338, _itemMenuRect, ItemMenuFunction, "Items");
			}

			if (_ESP)
			{
				if (_distanceESP)
				{
					foreach (Player player in _players)
					{
						if (!IsValid(player))
						{
							_players.Remove(player);
							continue;
						}
						Hip hip = player.m_hip;
						// Check for dead player & local player
						if (player.gameObject != Player.localPlayer.gameObject)
						{
							//Highlighter highlighter = player.GetComponent<Highlighter>();
							//highlighter.FlashingOff();
							//highlighter.ConstantOnImmediate(Color.red);
							// Get screen point
							Vector3 pos = Camera.main.WorldToScreenPoint(hip.transform.position);
							pos.y = Screen.height - pos.y;
							// Make sure it is on screen
							if (pos.z > 0f)
							{
								GUI.Label(new Rect(pos.x, pos.y, 50, 20), Vector3.Distance(hip.transform.position, Camera.main.transform.position).ToString());
							}
						}
					}
				}

				if (_itemESP)
				{
					foreach (Pickup item in _items)//PickupManager.instance.m_Pickups)
					{
						if (!IsValid(item))
						{
							_items.Remove(item);
							continue;
						}

						Vector3 pos = Camera.main.WorldToScreenPoint(item.transform.position);
						pos.y = Screen.height - pos.y;
						if (pos.z > 0f)
						{
							GUI.Label(new Rect(pos.x, pos.y, 100, 20), item.name);
						}
					}
				}
			}

			//Launch in Direction
			if (Input.GetKeyDown(KeyCode.F1))
			{
				Skydiving dive = Player.localPlayer.gameObject.GetComponent<Skydiving>();
				if (dive == null)
				{
					Player.localPlayer.gameObject.AddComponent<Skydiving>();
				}

				dive.Launch(Player.localPlayer.m_playerCamera.transform.forward * 0.5f);
			}

			//Aimbot
			if (Input.GetKeyDown(KeyCode.F2))
			{
				var playerDeaths = FindObjectsOfType<PlayerDeath>();
				float lowestDist = 1000;
				GameObject bestPlayer = null;
				foreach (PlayerDeath playerDeath in playerDeaths)
				{
					// Check for dead player & local player
					if (!playerDeath.dead && playerDeath.gameObject != Player.localPlayer.gameObject)
					{
						float dist = Vector3.Distance(Player.localPlayer.transform.position, playerDeath.gameObject.transform.position);
						if (dist < lowestDist)
						{
							lowestDist = dist;
							bestPlayer = playerDeath.gameObject;
						}
					}
				}

				if (bestPlayer != null)
				{
					Head head = bestPlayer.GetComponentInChildren<Head>();
					Player.localPlayer.m_playerCamera.transform.LookAt(head.transform.position);
				}
			}
		}

		void MenuFunction(int windowID)
		{
			GUI.DragWindow(new Rect(0, 0, _menuRect.width, 20));

			GUI.Label(new Rect(10, 10, 300, 300), "Hello World");
			
			if (GUI.Button(new Rect(10, 30, 100, 20), "Unload"))
			{
				//TODO: Unload somehow
				Class1.Unload();
			}

			_itemMenuVisible = GUI.Toggle(new Rect(10, 50, 100, 20), _itemMenuVisible, "Show Item Menu");
			_distanceESP = GUI.Toggle(new Rect(10, 70, 100, 20), _distanceESP, "Distance ESP");
			_itemESP = GUI.Toggle(new Rect(10, 90, 100, 20), _itemESP, "Item ESP");
			_speedHack = GUI.HorizontalSlider(new Rect(10, 110, 100, 20), _speedHack, 0f, 10f);
		}

		void ItemMenuFunction(int windowID)
		{
			GUI.DragWindow(new Rect(0, 0, _itemMenuRect.width, 20));

			var pickups = _items;//PickupManager.instance.m_Pickups;
			_itemScrollPosition = GUI.BeginScrollView(new Rect(20, 20, _itemMenuRect.width, _itemMenuRect.height), _itemScrollPosition, new Rect(0, 0, _itemMenuRect.width - 50, pickups.Count * 20));
			var y = 0;
			foreach (Pickup pickup in pickups)
			{
				if (GUI.Button(new Rect(5, y++ * 25, 200, 20), pickup.name))
				{
					//Player.localPlayer.m_inventory.NLHLPHMNOCB(pickup.m_itemIndex, 1);
					//InventoryUI.RepaintInventory();
					Player.localPlayer.GetComponent<InteractionHandler>().PickUp(pickup);
				}
			}
			GUI.EndScrollView();
		}

		bool IsValid(Player player)
		{
			if (player == null || player.m_playerDeath == null)
				return false;

			if (player.m_playerDeath.dead)
				return false;

			return true;
		}

		bool IsValid(Pickup item)
		{
			if (item == null)
				return false;

			//if (!PickupManager.instance.m_Pickups.Contains(item))
			//	return false;

			return true;
		}

		object GetFieldValue(object obj, string name)
		{
			var field = obj.GetType().GetField(name, BindingFlags.Public |
													 BindingFlags.NonPublic |
													 BindingFlags.Static |
													 BindingFlags.Instance);

			if (field != null)
			{
				return field.GetValue(obj);
			}
			return null;
		}


	}
}

namespace TotallyAccurateBattlegroundsStuff.HarmonyPatches
{
	#region HarmonyPatches
	//Players
	[HarmonyPatch(typeof(Player), "Start", null)]
	public static class CatchNewPlayer
	{
		private static void Postfix(Player __instance)
		{
			if (__instance != null)
			{
				if (!Hack._players.Contains(__instance))
				{
					Logger.Log("Added player " + __instance.name);
					Hack._players.Add(__instance);
				}
			}
		}
	}

	[HarmonyPatch(typeof(Player), "OnDestroy", null)]
	public static class RemovePlayer
	{
		private static void Postfix(Player __instance)
		{
			if (__instance != null)
			{
				if (Hack._players.Contains(__instance))
				{
					Logger.Log("Removed player " + __instance.name);
					Hack._players.Remove(__instance);
				}
			}
		}
	}

	//Items
	[HarmonyPatch(typeof(Pickup), "Start", null)]
	public static class CatchNewItem
	{
		private static void Postfix(Pickup __instance)
		{
			if (__instance != null)
			{
				if (!Hack._items.Contains(__instance))
				{
					Logger.Log("Added item " + __instance.name);
					Hack._items.Add(__instance);
				}
			}
		}
	}

	[HarmonyPatch(typeof(Pickup), "OnDestroy", null)]
	public static class RemoveItem
	{
		private static void Postfix(Pickup __instance)
		{
			if (__instance != null)
			{
				if (Hack._items.Contains(__instance))
				{
					Logger.Log("Removed item " + __instance.name);
					Hack._items.Remove(__instance);
				}
			}
		}
	}
	#endregion
}