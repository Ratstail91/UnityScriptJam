using System;
using System.Collections.Generic;
using UnityEngine;
using Toy;

public class ContainerController : MonoBehaviour, IEntity, Toy.IBundle {
	//public creation members
	public object displayName;
	public object spriteName;
	public object type;
	public object capacity;
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
	public object realCapacity { get; private set; } //TODO: (1) type checking in get

	List<object> contents = null; //NOTE: (1) will contain GameObjectWrappers - at least it should

	void Awake() {
		gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;

		spriteRenderer = GetComponent<SpriteRenderer>();

		contents = new List<object>();
	}

	public void Tick() {
		realName = gameController.ExtractFunctions(displayName, new List<object>() { this });
		object newSpriteName = gameController.ExtractFunctions(spriteName, new List<object>() { this });
		realType = gameController.ExtractFunctions(type, new List<object>() { this });
		object newCapacity = gameController.ExtractFunctions(capacity, new List<object>() { this });

		//load a new sprite
		if (newSpriteName != realSpriteName) {
			realSpriteName = newSpriteName;

			gameController.LoadSprite((string)realSpriteName, spriteRenderer);
		}

		if (newCapacity != realCapacity) {
			realCapacity = newCapacity;

			//TODO: (1) spew out the extra items (am I being carried?)
		}

		if (contents.Count > (double)realCapacity) {
			//TODO: (1) spew out the extra items (am I being carried?)
		}

		if (positionX == Int32.MaxValue && positionY == Int32.MaxValue) { //set by GameController
			gameObject.transform.position = new Vector3(Int32.MaxValue, Int32.MaxValue, 0);
		} else {
			gameObject.transform.position = new Vector3(GameController.squareWidth * positionX, GameController.squareHeight * positionY, 0);
		}
	}

	public List<object> GetContents() {
		return contents;
	}

	//IBundle
	public object Property(Toy.Interpreter interpreter, Toy.Token token, object argument) {
		string propertyName = (string)argument;

		switch(propertyName) {
			case "GetContents": return new GetContentsCallable(this);

			case "GetDisplayName": return new GetDisplayNameCallable(this);
			case "GetSpriteName": return new GetSpriteNameCallable(this);
			case "GetType": return new GetTypeCallable(this);
			case "GetCapacity": return new GetCapacityCallable(this);

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

	//ICallable properties
	public class GetContentsCallable : ICallable {
		ContainerController self = null;

		public GetContentsCallable(ContainerController self) {
			this.self = self;
		}

		public int Arity() {
			return 0;
		}

		public object Call(Interpreter interpreter, Token token, List<object> arguments) {
			return new Toy.Plugin.Array.ArrayInstance(self.contents);
		}
	}

	public class GetDisplayNameCallable : ICallable {
		ContainerController self = null;

		public GetDisplayNameCallable(ContainerController self) {
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
		ContainerController self = null;

		public GetSpriteNameCallable(ContainerController self) {
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
		ContainerController self = null;

		public GetTypeCallable(ContainerController self) {
			this.self = self;
		}

		public int Arity() {
			return 0;
		}

		public object Call(Interpreter interpreter, Token token, List<object> arguments) {
			return self.type;
		}
	}

	public class GetCapacityCallable : ICallable {
		ContainerController self = null;

		public GetCapacityCallable(ContainerController self) {
			this.self = self;
		}

		public int Arity() {
			return 0;
		}

		public object Call(Interpreter interpreter, Token token, List<object> arguments) {
			return self.realCapacity;
		}
	}
}