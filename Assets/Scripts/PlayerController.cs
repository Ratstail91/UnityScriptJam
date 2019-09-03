using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	//public creation members
	public object displayName;
	public object spriteName;
	public int positionX {
		get {
			return _posX;
		}
		set {
			_posX = value;
			transform.position = new Vector3(_posX * GameController.squareWidth, _posY * GameController.squareHeight, 0);
		}
	}
	public int positionY {
		get {
			return _posY;
		}
		set {
			_posY = value;
			transform.position = new Vector3(_posX * GameController.squareWidth, _posY * GameController.squareHeight, 0);
		}
	}

	int _posX = 0;
	int _posY = 0;

	//references
	GameController gameController;

	//components
	SpriteRenderer spriteRenderer;

	//members
	object realName;
	object realSpriteName;

	//movement
	float horizontalMovement = 0f;
	float verticalMovement = 0f;
	float lastMovementTime = float.NegativeInfinity;
	const float deadZone = 0.25f;

	void Awake() {
		gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;

		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
		HandleInput();
	}

	void FixedUpdate() {
		HandleMovement();
	}

	void HandleInput() {
		horizontalMovement = GamePad.GetAxis(CAxis.LX);
		verticalMovement = GamePad.GetAxis(CAxis.LY);
	}

	void HandleMovement() {
		if (Time.time - lastMovementTime > 0.2 && (Mathf.Abs(horizontalMovement) > deadZone || Mathf.Abs(verticalMovement) > deadZone)) {
			lastMovementTime = Time.time;

			//horizontal
			int horizontal = 0;
			if (Mathf.Abs(horizontalMovement) > deadZone) {
				horizontal = horizontalMovement > 0 ? 1 : -1;
			}

			//vertical
			int vertical = 0;
			if (Mathf.Abs(verticalMovement) > deadZone) {
				vertical = verticalMovement > 0 ? -1 : 1;
			}

			//check if this position is available
			if (gameController.CheckIsSquare(positionX + horizontal, positionY + vertical)) {
				positionX += horizontal;
				positionY += vertical;
			}
		}

		if (Mathf.Abs(horizontalMovement) < deadZone && Mathf.Abs(verticalMovement) < deadZone) {
			lastMovementTime = float.NegativeInfinity;
		}
	}

	//utility methods
	public void UpdateMembers() {
		realName = gameController.ExtractFunctions(displayName, new List<object>() { this });
		object newSpriteName = gameController.ExtractFunctions(spriteName, new List<object>() { this });

		//load a new sprite
		if (newSpriteName != realSpriteName) {
			realSpriteName = newSpriteName;

			gameController.LoadSprite((string)realSpriteName, spriteRenderer);
		}
	}

	public string GetRealName() {
		if (realName is string) {
			return (string)realName;
		}

		return "null";
	}
}