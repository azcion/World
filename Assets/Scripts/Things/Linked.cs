﻿using Assets.Scripts.Enums;
using Assets.Scripts.Graphics;
using Assets.Scripts.Makers;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Things {

	[UsedImplicitly]
	public class Linked : Thing, IThing {

		private int _x;
		private int _y;
		private LinkedType _type;
		private ThingMaterial _material;

		public GameObject Go => gameObject;
		public ThingType Type => ThingType.Structure;

		public static Linked Create (Linked linked, LinkedType type, ThingMaterial material) {
			linked._type = type;
			linked._material = material;
			linked.Heir = linked;

			return linked;
		}

		public void Initialize (bool planning = false) {
			PrepareChild();
			gameObject.isStatic = true;
			_x = (int) transform.position.x;
			_y = (int) transform.position.y;
			ChildRenderer.color = Tint.Get(_material);

			if (planning) {
				string assetName = $"{Name.Get(_type)}_Atlas";
				SetSprite(Assets.GetAtlasSprite(assetName, 12), false);
				ChildRenderer.color = AdjustOpacity(ChildRenderer.color, .5f);
				gameObject.SetActive(true);

				return;
			}

			InitializeSelf();
			gameObject.SetActive(true);
		}

		public void Refresh () {
			_x = (int) transform.position.x;
			_y = (int) transform.position.y;

			InitializeSelf();
			InitializeNeighbors();
		}

		private static Color AdjustOpacity (Color c, float opacity) {
			return new Color(c.r, c.g, c.b, opacity);
		}

		private static int GetIndex (int mask) {
			int mod = mask % 4;
			return 12 - (mask - mod) + mod;
		}

		private void InitializeSelf () {
			int mask = WallMaker.GetLinked(_x, _y + 1)?._type == _type ? 1 : 0;
			mask += WallMaker.GetLinked(_x + 1, _y)?._type == _type ? 2 : 0;
			mask += WallMaker.GetLinked(_x, _y - 1)?._type == _type ? 4 : 0;
			mask += WallMaker.GetLinked(_x - 1, _y)?._type == _type ? 8 : 0;

			mask += _y == Map.YTiles - 1 ? 1 : 0;
			mask += _x == Map.YTiles - 1 ? 2 : 0;
			mask += _y == 0 ? 4 : 0;
			mask += _x == 0 ? 8 : 0;
			int index = GetIndex(mask);

			string assetName = $"{Name.Get(_type)}_Atlas";
			SetSprite(Assets.GetAtlasSprite(assetName, index), false); 
			CoverCenterGaps(index);
			CoverEdgeGaps();
		}

		private void InitializeNeighbors () {
			for (int x = -1; x < 2; ++x) {
				for (int y = -1; y < 2; ++y) {
					if (x == 0 && y == 0) {
						continue;
					}

					Linked neighbor = WallMaker.GetLinked(_x + x, _y + y);

					if (neighbor == null) {
						continue;
					}

					for (int i = neighbor.transform.childCount - 1; i > 0; --i) {
						Destroy(neighbor.transform.GetChild(i).gameObject, .2f);
					}

					neighbor.InitializeSelf();
				}
			}
		}

		private void CoverCenterGaps (int index) {
			switch (index) {
				case 2:
				case 3:
				case 10:
				case 11:
					if (WallMaker.GetLinked(_x + 1, _y - 1)?._type == _type) {
						CoverAssembler.Make(_type, _material, _x, _y, .5f, -.25f, 1, 1);
					}

					break;
			}
		}

		private void CoverEdgeGaps () {
			int max = Map.YTiles - 1;

			if (_x == 0) {
				if (_y == 0) {
					// Left bottom corner
					CoverAssembler.Make(_type, _material, _x, _y, 0, 0, .5f, .75f);
				} else if (_y == max) {
					// Left top corner
					CoverAssembler.Make(_type, _material, _x, _y, 0, .75f, .5f, .25f);
				}

				if (WallMaker.GetLinked(0, _y - 1)?._type == _type) {
					// Left edge
					CoverAssembler.Make(_type, _material, _x, _y, 0, -.25f, .5f, 1);
				}
			} else if (_x == max) {
				if (_y == 0) {
					// Right bottom corner
					CoverAssembler.Make(_type, _material, _x, _y, .5f, 0, .5f, .75f);
				} else if (_y == max) {
					// Right top corner
					CoverAssembler.Make(_type, _material, _x, _y, .5f, .75f, .5f, .25f);
				}

				if (WallMaker.GetLinked(max, _y - 1)?._type == _type) {
					// Right edge
					CoverAssembler.Make(_type, _material, _x, _y, .5f, -.25f, .5f, 1);
				}
			}

			if (_y == 0) {
				if (WallMaker.GetLinked(_x - 1, 0)?._type == _type) {
					// Bottom edge
					CoverAssembler.Make(_type, _material, _x, _y, -.5f, 0, 1, .75f);
				}
			} else if (_y == max) {
				if (WallMaker.GetLinked(_x - 1, max)?._type == _type) {
					// Top edge
					CoverAssembler.Make(_type, _material, _x, _y, -.5f, .75f, 1, .25f);
				}
			}
		}

	}

}