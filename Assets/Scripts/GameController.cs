using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	//public static gameobject members
	public static GameObject monsterPrefab = null;

	//references to prefabs
	[SerializeField] GameObject monsterPrefabReference = null;

	//members
	public string loadablePackageName;

	Toy.Environment environment;

	void Awake() {
		environment = new Toy.Environment();

		monsterPrefab = monsterPrefabReference;

		Toy.Runner.RunFile(environment, Application.streamingAssetsPath + "/" + loadablePackageName + "/main.toy");
	}
}