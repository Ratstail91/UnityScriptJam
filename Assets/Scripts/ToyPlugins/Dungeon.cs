using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toy {
	namespace Plugin {
		class Dungeon : IPlugin, IBundle {
			//members
			Dictionary<string, SquareTuple> registeredSquares = new Dictionary<string, SquareTuple>();
			GameController gameController;

			//container class
			public struct SquareTuple {
				public object displayName;
				public object spriteName;
				public object onEnter;
				public object onExit;
			}

			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Dungeon();
					}
					return singleton;
				}
			}
			static Dungeon singleton = null;

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(String.IsNullOrEmpty(alias) ? "Dungeon" : alias, this, true);

				gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
			}

			//IBundle
			public object Property(Interpreter interpreter, Token token, object argument) {
				string propertyName = (string)argument;

				switch(propertyName) {
					case "RegisterTile": return new RegisterSquare(this);
					case "SpawnTile": return new SpawnSquare(this);

					default:
						throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
				}
			}
			
			//ICallable classes
			public class RegisterSquare : ICallable {
				Dungeon self = null;

				public RegisterSquare(Dungeon self) {
					this.self = self;
				}

				public int Arity() {
					return 5; //key, displayName, spriteName, onEnter, onExit
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					SquareTuple tuple = new SquareTuple();

					tuple.displayName = arguments[1];
					tuple.spriteName = arguments[2];
					tuple.onEnter = arguments[3];
					tuple.onExit = arguments[4];

					self.registeredSquares[(string)arguments[0]] = tuple;

					return null;
				}
			}

			public class SpawnSquare : ICallable {
				Dungeon self = null;

				public SpawnSquare(Dungeon self) {
					this.self = self;
				}

				public int Arity() {
					return 3; //key, x, y
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					//spawn the square, which contains the tuple's information

					GameObject go = GameObject.Instantiate(self.gameController.squarePrefab);

					SquareController controller = go.GetComponent<SquareController>();
					SquareTuple tuple = self.registeredSquares[(string)arguments[0]];

					controller.displayName = tuple.displayName;
					controller.spriteName = tuple.spriteName;
					controller.onEnter = tuple.onEnter;
					controller.onExit = tuple.onExit;
					controller.positionX = (int)(double)arguments[1];
					controller.positionY = (int)(double)arguments[2];

					controller.UpdateMembers();

					self.gameController.CacheSquare(go);

					return null;
				}
			}
		}
	}
}