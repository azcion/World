﻿using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Graphics;
using Assets.Scripts.Makers;
using Assets.Scripts.UI;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Main {

	public class ApplicationController : MonoBehaviour {

		public static int Seed;
		public static bool Ready;

		private const int INFO_REFRESH_FRAMES = 8;

		private static bool _ready;
		private static float _loadTime;
		private static float _startTime;

		private int _infoRefreshFrame;
		private Text _i;

		[UsedImplicitly, SerializeField] private GameObject _background;
		[UsedImplicitly, SerializeField] private GameObject _infoBox;
		[UsedImplicitly, SerializeField] private GameObject _sun;
		[UsedImplicitly, SerializeField] private GameObject _tileMaker;
		[UsedImplicitly, SerializeField] private GameObject _wallMaker;
		[UsedImplicitly, SerializeField] private GameObject _plantMaker;
		[UsedImplicitly, SerializeField] private GameObject _animalMaker;

		[UsedImplicitly]
		public void OnStart () {
			_startTime = Time.realtimeSinceStartup;

			string seedInput = StartInterface.GetSeed;
			Seed = Utils.Seed.Get(seedInput);
			Map.InitializeMapMeasurements(StartInterface.GetMapSize / Map.CSIZE);
			Random.InitState(Seed);
			_i = _infoBox.GetComponent<Text>();
			StartInterface.Hide();
			_background.SetActive(true);
			CameraController.PointCameraAtMapCenter();
			_sun.SetActive(true);
			AverageColor.Initialize();
			TileTint.Initialize();
			SmoothTiles.LoadAssets();
			_tileMaker.SetActive(true);
			_wallMaker.SetActive(true);
			_plantMaker.SetActive(true);
			_animalMaker.SetActive(true);
			_ready = true;

			SetReady();
		}

		public static void NotifyReady () {
			_loadTime = Time.realtimeSinceStartup - _startTime;
		}

		private static void SetReady () {
			Ready = true;
		}

		[UsedImplicitly]
		private void LateUpdate () {
			if (!_ready) {
				return;
			}

			++_infoRefreshFrame;

			if (_infoRefreshFrame % INFO_REFRESH_FRAMES != 0) {
				return;
			}

			Tile t = CameraController.TileUnderCursor;
			_infoRefreshFrame = 0;
			_i.text = "";
			_i.text += "\n" + DayNightCycle.LightLevel + "% lit";

			if (t != null) {
				_i.text += "\n" + t.Chunk.name + " | " + t.name;
				_i.text += "\n" + Enum.GetName(typeof(TileType), t.Type);
			} else {
				_i.text += "\nVoid";
			}

			_i.text += "\nLoad: " + _loadTime.ToString("n2") + "s";
			_i.text += "\nSeed: " + Seed;
		}

	}

}