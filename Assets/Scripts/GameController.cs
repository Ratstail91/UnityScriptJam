using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour {
	//public gameobject members
	public GameObject monsterPrefab = null;

	//members
	public string loadablePackageName;
	List<GameObject> cachedEntities = new List<GameObject>();

	Toy.Environment environment;

	void Awake() {
		environment = new Toy.Environment();

		Toy.Runner.RunFile(environment, Application.streamingAssetsPath + "/" + loadablePackageName + "/main.toy");
	}

	void FixedUpdate() {
		cachedEntities.RemoveAll(go => go == null);

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

	//utility functions
	public object ExtractFunctions(object obj, List<object> arguments) {
		while(obj is Toy.ScriptFunction) {
			obj = Toy.Runner.Run(environment, (Toy.ICallable)obj, arguments);
		}

		return obj;
	}

	public void LoadSprite(string fname, SpriteRenderer renderer) {
		string filePath = Application.streamingAssetsPath + "/" + loadablePackageName + "/" + fname;
		Texture2D texture = new Texture2D(2, 2); //gets resized

		texture.LoadImage(File.ReadAllBytes(filePath));
		renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
	}
}