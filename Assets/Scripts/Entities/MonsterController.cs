using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Toy;

public class MonsterController : MonoBehaviour, IEntity, Toy.IBundle {
	//public creation members
	public object displayName;
	public object spriteName;
	public object onTick;
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
	}

	public void Tick() {
		realName = gameController.ExtractFunctions(displayName, new List<object>() { this });
		object newSpriteName = gameController.ExtractFunctions(spriteName, new List<object>() { this });
		object alive = gameController.ExtractFunctions(onTick, new List<object>() { this });

		//load a new sprite
		if (newSpriteName != realSpriteName) {
			realSpriteName = newSpriteName;

			gameController.LoadSprite((string)realSpriteName, spriteRenderer);
		}

		if (!(alive is bool) || (bool)alive == false) {
			GameObject.Destroy(gameObject);
		}

		gameObject.transform.position = new Vector3(GameController.squareWidth * positionX, GameController.squareHeight * positionY, 0);
	}

	string GetRealName() {
		if (realName is string) {
			return (string)realName;
		}

		return "null";
	}

	//IBundle
	public object Property(Toy.Interpreter interpreter, Toy.Token token, object argument) {
		string propertyName = (string)argument;

		switch(propertyName) {
//			case "GetStatistics":
			case "Move": return new Move(this);
//			case "Check":
//			case "Attack":
//			case "Pickup":
//			case "Drop":
			case "PositionX": return new AssignableProperty(val => this.positionX = (int)(double)val, x => (double)this.positionX);
			case "positionY": return new AssignableProperty(val => this.positionY = (int)(double)val, x => (double)this.positionY);

			default:
				throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
		}
	}

	//assignable properties
	public class AssignableProperty : AssignableIndex {
		Func<object, object> Set = null;
		Func<object, object> Get = null;

		public AssignableProperty(Func<object, object> Set, Func<object, object> Get) {
			this.Set = Set;
			this.Get = Get;
		}

		public override object Value {
			set {
				Set(value);
			}
			get {
				return Get(null);
			}
		}
	}

	public class Move : ICallable {
		MonsterController self = null;

		public Move(MonsterController self) {
			this.self = self;
		}

		public int Arity() {
			return 2; //direction, distance
		}

		public object Call(Interpreter interpreter, Token token, List<object> arguments) {
			object direction = self.gameController.ExtractFunctions(arguments[0], new List<object>() { self });
			object distance = self.gameController.ExtractFunctions(arguments[1], new List<object>() { self });

			//gameController output
			if (!(direction is string)) {
				self.gameController.ShowError("\"direction\" must be a string");
			}

			if (!(distance is double)) {
				self.gameController.ShowError("\"distance\" must be a number");
			}

			int posX = self.positionX;
			int posY = self.positionY;

			switch((string)direction) {
				case "north":
					posY += (int)(double)distance;
					break;

				case "south":
					posY -= (int)(double)distance;
					break;

				case "east":
					posX += (int)(double)distance;
					break;

				case "west":
					posX -= (int)(double)distance;
					break;

				default:
					self.gameController.ShowError((string)direction + " is not as valid direction");
					break;
			}

			if (self.gameController.CheckIsSquare(posX, posY)) {
				self.positionX = posX;
				self.positionY = posY;
			}

			return null;
		}
	}
}