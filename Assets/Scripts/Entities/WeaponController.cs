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
	public object realName { get; private set; }
	public object realSpriteName { get; private set; }
	public object realType { get; private set; }
	public object realDamage { get; private set; }

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

	//IBundle
	public object Property(Toy.Interpreter interpreter, Toy.Token token, object argument) {
		string propertyName = (string)argument;

		switch(propertyName) {
			case "GetDisplayName": return new GetDisplayNameCallable(this);
			case "GetSpriteName": return new GetSpriteNameCallable(this);
			case "GetType": return new GetTypeCallable(this);
			case "GetDamage": return new GetDamageCallable(this);

			case "PositionX": return new AssignableProperty(val => this.positionX = (int)(double)val, x => (double)this.positionX);
			case "PositionY": return new AssignableProperty(val => this.positionY = (int)(double)val, x => (double)this.positionY);

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

	public class GetDisplayNameCallable : ICallable {
		WeaponController self = null;

		public GetDisplayNameCallable(WeaponController self) {
			this.self = self;
		}

		public int Arity() {
			return 0;
		}

		public object Call(Interpreter interpreter, Token token, List<object> arguments) {
			return self.realName;
		}
	}

	public class GetSpriteNameCallable : ICallable {
		WeaponController self = null;

		public GetSpriteNameCallable(WeaponController self) {
			this.self = self;
		}

		public int Arity() {
			return 0;
		}

		public object Call(Interpreter interpreter, Token token, List<object> arguments) {
			return self.realSpriteName;
		}
	}

	public class GetTypeCallable : ICallable {
		WeaponController self = null;

		public GetTypeCallable(WeaponController self) {
			this.self = self;
		}

		public int Arity() {
			return 0;
		}

		public object Call(Interpreter interpreter, Token token, List<object> arguments) {
			return self.type;
		}
	}

	public class GetDamageCallable : ICallable {
		WeaponController self = null;

		public GetDamageCallable(WeaponController self) {
			this.self = self;
		}

		public int Arity() {
			return 0;
		}

		public object Call(Interpreter interpreter, Token token, List<object> arguments) {
			return self.realDamage;
		}
	}
}