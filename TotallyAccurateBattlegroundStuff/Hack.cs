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

		void Start()
		{
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

			if (Input.GetKey(KeyCode.LeftAlt))
			{
				// Get all player "deaths" (fucking retarded name)
				var playerDeaths = FindObjectsOfType<PlayerDeath>();
				foreach (PlayerDeath playerDeath in playerDeaths)
				{
					// Get the respective hip object
					Hip hip = playerDeath.GetComponentInChildren<Hip>();
					// Check for dead player & local player
					if (!playerDeath.dead && playerDeath.gameObject != Player.localPlayer.gameObject)
					{
						// Get screen point
						Vector3 pos = Camera.main.WorldToScreenPoint(hip.transform.position);
						pos.y = Screen.height - pos.y;
						// Make sure it is on screen
						if (pos.z > 0f)
						{
							GUI.Label(new Rect(pos.x, pos.y, 30f, 20f), Vector3.Distance(hip.transform.position, Camera.main.transform.position).ToString());
						}
					}

				}
			}

			if (Input.GetKey(KeyCode.RightAlt))
			{
				var items = FindObjectsOfType<Pickup>();
				foreach (Pickup item in items)
				{
					Vector3 pos = Camera.main.WorldToScreenPoint(item.transform.position);
					pos.y = Screen.height - pos.y;
					if (pos.z > 0f)
					{
						GUI.Label(new Rect(pos.x, pos.y, 30f, 20f), item.name);
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
			}

			_itemMenuVisible = GUI.Toggle(new Rect(10, 50, 100, 20), _itemMenuVisible, "Show Item Menu");

		}

		void ItemMenuFunction(int windowID)
		{
			GUI.DragWindow(new Rect(0, 0, _itemMenuRect.width, 20));

			var pickups = FindObjectsOfType<Pickup>();
			_itemScrollPosition = GUI.BeginScrollView(new Rect(20, 20, _itemMenuRect.width, _itemMenuRect.height), _itemScrollPosition, new Rect(0, 0, _itemMenuRect.width - 50, pickups.Length * 20));
			var y = 0;
			foreach (var pickup in pickups)
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

		List<Pickup> GetPickups(LootDatabase db)
		{
			List<Pickup> ret = new List<Pickup>();
			for (int i = 0; i < db.ItemCount; i++)
			{
				ret.Add(db.PHNKCHKGHJC(i).pickup);
			}
			return ret;
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
