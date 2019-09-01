using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MonsterController : MonoBehaviour, IEntity, Toy.IBundle {
	//public creation members
	public object displayName;
	public object spriteName;
	public object onTick;
	public int positionX;
	public int positionY;

	//references
	GameController gameController;

	//components
	SpriteRenderer spriteRenderer;

	//members
	object realName;
	object realSpriteName;

	void Awake() {
		gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;

		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Tick() {
		realName = gameController.ExtractFunctions(displayName, new List<object>() { this });
		object newSpriteName = gameController.ExtractFunctions(spriteName, new List<object>() { this });
		object alive = gameController.ExtractFunctions(onTick, new List<object>() { this });

		if (!(alive is bool) || (bool)alive == false) {
			GameObject.Destroy(gameObject);
		}

		if (newSpriteName != realSpriteName) {
			realSpriteName = newSpriteName;

			gameController.LoadSprite((string)realSpriteName, spriteRenderer);
		}
	}

	string GetRealName() {
		if (realName is string) {
			return (string)realName;
		}

		return "null";
	}

	//IBundle
	public object Property(Toy.Interpreter interpreter, Toy.Token token, object argument) {
		return null;
	}
}