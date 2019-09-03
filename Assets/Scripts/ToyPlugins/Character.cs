using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toy {
	namespace Plugin {
		public class Character : IPlugin, IBundle {
			//members
			GameController gameController;
			PlayerController playerController;

			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Character();
					}
					return singleton;
				}
			}
			static Character singleton = null;

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(String.IsNullOrEmpty(alias) ? "Character" : alias, this, true);

				gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
				playerController = gameController.GetPlayerGameObject().GetComponent<PlayerController>();
			}

			//IBundle
			public object Property(Interpreter interpreter, Token token, object argument) {
				string propertyName = (string)argument;

				switch(propertyName) {
					case "SetName": return new SetName(this);
					case "LoadSprite": return new LoadSprite(this);
					case "GetStatistics": return new GetStatistics(this);
					case "SetCarry": return new SetCarry(this);
					case "GetCarry": return new GetCarry(this);
					case "PositionX": return new AssignableProperty(val => playerController.positionX = (int)(double)val, x => (double)playerController.positionX);
					case "PositionY": return new AssignableProperty(val => playerController.positionY = (int)(double)val, x => (double)playerController.positionY);

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

			//ICallable classes
			public class SetName : ICallable {
				Character self = null;

				public SetName(Character self) {
					this.self = self;
				}

				public int Arity() {
					return 1; //name
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					self.gameController.GetPlayerGameObject().GetComponent<PlayerController>().displayName = arguments[0];

					self.gameController.GetPlayerGameObject().GetComponent<PlayerController>().UpdateMembers();

					return null;
				}
			}

			public class LoadSprite : ICallable {
				Character self = null;

				public LoadSprite(Character self) {
					this.self = self;
				}

				public int Arity() {
					return 1; //sprite
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					self.playerController.spriteName = arguments[0];

					self.playerController.UpdateMembers();

					return null;
				}
			}

			public class GetStatistics : ICallable {
				Character self = null;

				public GetStatistics(Character self) {
					this.self = self;
				}

				public int Arity() {
					return 0;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return self.gameController.GetPlayerStatistics();
				}
			}

			public class SetCarry : ICallable {
				Character self = null;

				public SetCarry(Character self) {
					this.self = self;
				}

				public int Arity() {
					return 1;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					self.gameController.SetPlayerCarryGameObject( ((GameObjectWrapper)arguments[0]).GetSelf() );

					return null;
				}
			}

			public class GetCarry : ICallable {
				Character self = null;

				public GetCarry(Character self) {
					this.self = self;
				}

				public int Arity() {
					return 0;
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					return new GameObjectWrapper(self.gameController.GetPlayerCarryGameObject());
				}
			}
		}
	}
}