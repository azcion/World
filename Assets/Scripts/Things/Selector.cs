﻿using Assets.Scripts.Main;
using Assets.Scripts.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Things {

	public class Selector : MonoBehaviour {

		public static GameObject Instance;
		public static Thing Thing;
		
		private static bool _didSelect;
		
		public static void Select (Transform target, Thing thing) {
			_didSelect = true;
			Instance.transform.SetParent(target);
			Vector2 v = target.position;
			Instance.transform.position = new Vector3(v.x + .5f, v.y, Order.SELECTOR);
			Instance.SetActive(true);
			Thing = thing;
		}

		public static void Deselect () {
			Instance.SetActive(false);
			Instance.transform.SetParent(null);
			Thing.Deselect();
		}

		[UsedImplicitly]
		private void Start () {
			gameObject.SetActive(false);
			Instance = gameObject;
		}

		[UsedImplicitly]
		private void LateUpdate () {
			if (_didSelect) {
				_didSelect = false;
				return;
			}

			if (ApplicationController.Ready) {
				if (Input.GetMouseButtonDown(0)) {
					Deselect();
				}
			}
		}

	}

}