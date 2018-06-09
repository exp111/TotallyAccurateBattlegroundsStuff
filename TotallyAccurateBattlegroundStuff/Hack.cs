using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Landfall.Network;
using System.Reflection;
using HighlightingSystem;

namespace TotallyAccurateBattlegroundsStuff
{
	class Hack : MonoBehaviour
	{
		private PhotonServerHandler _serverHandler;

		void Start()
		{
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.ActDetectorBase>());
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.InjectionDetector>());
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.ObscuredCheatingDetector>());
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.SpeedHackDetector>());
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.TimeCheatingDetector>());
			DestroyObject(FindObjectOfType<CodeStage.AntiCheat.Detectors.WallHackDetector>());
		}

		void Update()
		{
			//No Recoil + Screenshake
			DestroyObject(FindObjectOfType<Recoil>());
			DestroyObject(FindObjectOfType<AddScreenShake>());

			if (Player.localPlayer != null)
			{
				//unlim ammo + firemodes + rapidfire + nospread
				Player.localPlayer.m_inventory.weaponHandler.rightWeapon.gun.bullets = 10;
				Player.localPlayer.m_inventory.weaponHandler.rightWeapon.gun.hasFullAuto = true;
				Player.localPlayer.m_inventory.weaponHandler.rightWeapon.gun.hasSingleFire = true;
				Player.localPlayer.m_inventory.weaponHandler.rightWeapon.gun.rateOfFire = 0.001f;
				Player.localPlayer.m_inventory.weaponHandler.rightWeapon.gun.extraSpread = 0;

				Player.localPlayer.m_inventory.weaponHandler.leftWeapon.gun.bullets = 10;
				Player.localPlayer.m_inventory.weaponHandler.leftWeapon.gun.hasFullAuto = true;
				Player.localPlayer.m_inventory.weaponHandler.leftWeapon.gun.hasSingleFire = true;
				Player.localPlayer.m_inventory.weaponHandler.leftWeapon.gun.rateOfFire = 0.001f;
				Player.localPlayer.m_inventory.weaponHandler.leftWeapon.gun.extraSpread = 0;

				//Launch in Direction
				if (Input.GetKeyDown(KeyCode.F1))
				{
					Skydiving dive = Player.localPlayer.gameObject.GetComponent<Skydiving>();
					if (dive == null)
					{
						dive = Player.localPlayer.gameObject.AddComponent<Skydiving>();
					}

					dive.Launch(Player.localPlayer.m_playerCamera.transform.forward * 1f /* Increase for speed */);
				}
			}
		}

		void OnGUI()
		{
			GUI.Label(new Rect(10, 10, 300, 300), "Hello World");

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
