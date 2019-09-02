using System;
using System.Collections.Generic;
using UnityEngine;
using Toy;

public class WeaponController : MonoBehaviour, IEntity, Toy.IBundle {
	//public creation members
	public object displayName;
	public object spriteName;
	public object type;
	public object damage;
	public int positionX { get; set; }
	public int positionY { get; set; }

	//references
	GameController gameController;

	//components
	SpriteRenderer spriteRenderer;

	//members
	object realName;
	object realSpriteName;
	object realType;
	object realDamage;

	void Awake() {
		gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;

		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Tick() {
		realName = gameController.ExtractFunctions(displayName, new List<object>() { this });
		object newSpriteName = gameController.ExtractFunctions(spriteName, new List<object>() { this });
		realType = gameController.ExtractFunctions(type, new List<object>() { this });
		realDamage = gameController.ExtractFunctions(damage, new List<object>() { this });

		//load a new sprite
		if (newSpriteName != realSpriteName) {
			realSpriteName = newSpriteName;

			gameController.LoadSprite((string)realSpriteName, spriteRenderer);
		}

		gameObject.transform.position = new Vector3(GameController.squareWidth * positionX, GameController.squareHeight * positionY, 0);
	}

	string GetRealName() {
		if (realName is string) {
			return (string)realName;
		}

		return "null";
	}

	string GetRealType() {
		if (realType is string) {
			return (string)realType;
		}

		return "null";
	}

	int GetRealDamage() {
		if (realDamage is double) {
			return (int)(double)realDamage;
		}

		return -1;
	}

	//IBundle
	public object Property(Toy.Interpreter interpreter, Toy.Token token, object argument) {
		string propertyName = (string)argument;

		switch(propertyName) {
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
}