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
	Statistics playerStatistics = new Statistics();
	GameObject playerCarryObject = null;

	Toy.Environment environment;

	void Awake() {
		//setup the player's default stats
		playerStatistics.SetStat("baseHP", (double)20);
		playerStatistics.SetStat("baseMP", (double)10);
		playerStatistics.SetStat("baseFull", (double)50);
		playerStatistics.SetStat("baseHunger", (double)1);

		//set the maximum stats
		playerStatistics.SetStat("maxHP", playerStatistics.GetStat("baseHP"));
		playerStatistics.SetStat("maxMP", playerStatistics.GetStat("baseMP"));
		playerStatistics.SetStat("maxFull", playerStatistics.GetStat("baseFull"));

		//set the current stats
		playerStatistics.SetStat("currentHP", playerStatistics.GetStat("maxHP"));
		playerStatistics.SetStat("currentMP", playerStatistics.GetStat("maxMP"));
		playerStatistics.SetStat("currentFull", playerStatistics.GetStat("maxFull"));
		playerStatistics.SetStat("currentHunger", playerStatistics.GetStat("baseHunger"));

		//run the code
		environment = new Toy.Environment();

		Toy.Runner.RunFile(environment, Application.streamingAssetsPath + "/" + loadablePackageName + "/main.toy");

		//tick once to allow loading
		TickEntities();
	}

	void FixedUpdate() {
		cachedEntities.RemoveAll(go => go == null);
		cachedSquares.RemoveAll(go => go == null);

		int ticks = playerGameObject.GetComponent<PlayerController>().GetAllowedTicks();

		while(ticks > 0) {
			TickEntities();
			ticks--;
		}
	}

	public void TickEntities() {
		playerStatistics.SetStat("currentFull", (double)playerStatistics.GetStat("currentFull") - (double)playerStatistics.GetStat("currentHunger"));

		//TODO: kill the player if they're out of "currentFull"

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

	public bool CheckIsSquare(int x, int y) {
		GameObject sqr = cachedSquares.Find(square => {
			SquareController controller = square.GetComponent<SquareController>();

			return controller.positionX == x && controller.positionY == y;
		});

		return sqr != null;
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

	public Statistics GetPlayerStatistics() {
		return playerStatistics;
	}

	public GameObject SetPlayerCarryGameObject(GameObject go) {
		ContainerController controller = go.GetComponent<ContainerController>();

		if (controller == null) {
			ShowError("Can't carry that item: " + go.name);
			return null;
		}

		//TODO: (1) must be a carry type

		controller.positionX = Int32.MaxValue;
		controller.positionY = Int32.MaxValue;

		return playerCarryObject = go;
	}

	public GameObject GetPlayerCarryGameObject() {
		return playerCarryObject;
	}
}