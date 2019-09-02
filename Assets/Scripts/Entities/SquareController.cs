using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareController : MonoBehaviour {
	//public creation members
	public object displayName;
	public object spriteName;
	public object onEnter;
	public object onExit;
	public int positionX { get; set; }
	public int positionY { get; set; }

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

		UpdateMembers();
	}

	public void UpdateMembers() {
		realName = gameController.ExtractFunctions(displayName, new List<object>() { this });
		object newSpriteName = gameController.ExtractFunctions(spriteName, new List<object>() { this });

		//load a new sprite
		if (newSpriteName != realSpriteName) {
			realSpriteName = newSpriteName;

			gameController.LoadSprite((string)realSpriteName, spriteRenderer);
		}
	}

	public object TriggerOnEnter(GameObject go) {
		object ret = gameController.RunFunction(onEnter, new List<object>() { go });

		UpdateMembers();

		return ret;
	}

	public object TriggerOnExit(GameObject go) {
		object ret = gameController.RunFunction(onExit, new List<object>() { go });

		UpdateMembers();

		return ret;
	}

	public string GetRealName() {
		if (realName is string) {
			return (string)realName;
		}

		return "null";
	}
}