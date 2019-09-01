using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toy {
	namespace Plugin {
		public class Monsters : IPlugin, IBundle {
			//members
			Dictionary<string, MonstersTuple> registeredMonsters = new Dictionary<string, MonstersTuple>();
			GameController gameController;

			//container class
			public struct MonstersTuple {
				public object displayName;
				public object spriteName;
				public object onTick;
			}

			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Monsters();
					}
					return singleton;
				}
			}
			static Monsters singleton = null;

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(String.IsNullOrEmpty(alias) ? "Monsters" : alias, this, true);

				gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
			}

			//IBundle
			public object Property(Interpreter interpreter, Token token, object argument) {
				string propertyName = (string)argument;

				switch(propertyName) {
					case "RegisterMonster": return new RegisterMonster(this);
					case "SpawnMonster": return new SpawnMonster(this);

					default:
						throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
				}
			}

			//ICallable classes
			public class RegisterMonster : ICallable {
				Monsters self = null;

				public RegisterMonster(Monsters self) {
					this.self = self;
				}

				public int Arity() {
					return 4; //key, displayName, spriteName, onTick
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					MonstersTuple tuple = new MonstersTuple();

					tuple.displayName = arguments[1];
					tuple.spriteName = arguments[2];
					tuple.onTick = arguments[3];

					self.registeredMonsters[(string)arguments[0]] = tuple;

					return null;
				}
			}

			public class SpawnMonster : ICallable {
				Monsters self = null;

				public SpawnMonster(Monsters self) {
					this.self = self;
				}

				public int Arity() {
					return 3; //key, x, y
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					//spawn the monster, which contains the tuple's information

					GameObject go = GameObject.Instantiate(self.gameController.monsterPrefab);

					MonsterController controller = go.GetComponent<MonsterController>();
					MonstersTuple tuple = self.registeredMonsters[(string)arguments[0]];

					controller.displayName = tuple.displayName;
					controller.spriteName = tuple.spriteName;
					controller.onTick = tuple.onTick;
					controller.positionX = (int)(double)arguments[1];
					controller.positionY = (int)(double)arguments[2];

					self.gameController.CacheEntity(go);

					return null;
				}
			}
		}
	}
}