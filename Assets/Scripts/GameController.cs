﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour {
	//public gameobject members
	public GameObject errorDisplay = null;
	public GameObject monsterPrefab = null;
	public GameObject containerPrefab = null;
	public GameObject weaponPrefab = null;
	public GameObject wearablePrefab = null;
	public GameObject usablePrefab = null;
	public GameObject squarePrefab = null;
	public GameObject playerGameObject = null;

	//constants
	public const float squareWidth = 0.64f;
	public const float squareHeight = 0.64f;

	//members
	public string loadablePackageName; //TODO: (1) Load from a script
	List<GameObject> cachedEntities = new List<GameObject>();
	List<GameObject> cachedSquares = new List<GameObject>();

	Toy.Environment environment;

	void Awake() {
		environment = new Toy.Environment();

		Toy.Runner.RunFile(environment, Application.streamingAssetsPath + "/" + loadablePackageName + "/main.toy");
	}

	void FixedUpdate() {
		cachedEntities.RemoveAll(go => go == null);
		cachedSquares.RemoveAll(go => go == null);

		TickEntities(); //TEMP: ticks
	}

	public void TickEntities() {
		foreach(GameObject go in cachedEntities) {
			go.GetComponent<IEntity>().Tick();
		}
	}

	public void CacheEntity(GameObject go) {
		if (!cachedEntities.Contains(go)) {
			cachedEntities.Add(go);
		}
	}

	public void CacheSquare(GameObject go) { //NOTE: done separately, so they don't tick
		if (!cachedSquares.Contains(go)) {
			cachedSquares.Add(go);

			SquareController controller = go.GetComponent<SquareController>();

			go.transform.position = new Vector3(controller.positionX * squareWidth, controller.positionY * squareHeight, 0);
		}
	}

	//error handling
	public void ShowError(string msg) {
		errorDisplay.GetComponent<TextMeshProUGUI>().text += msg + "\n";
	}

	//utility functions
	public object ExtractFunctions(object obj, List<object> arguments) {
		while(obj is Toy.ScriptFunction) {
			obj = Toy.Runner.Run(environment, (Toy.ICallable)obj, arguments);
		}

		return obj;
	}

	public object RunFunction(object func, List<object> arguments) {
		if (func is Toy.ICallable) {
			return Toy.Runner.Run(environment, (Toy.ICallable)func, arguments);
		}

		return null;
	}

	public void LoadSprite(string fname, SpriteRenderer renderer) {
		string filePath = Application.streamingAssetsPath + "/" + loadablePackageName + "/" + fname;
		Texture2D texture = new Texture2D(2, 2); //gets resized

		texture.LoadImage(File.ReadAllBytes(filePath));
		renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
	}

	public GameObject GetPlayerGameObject() {
		return playerGameObject;
	}
}