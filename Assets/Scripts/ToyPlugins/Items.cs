using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toy {
	namespace Plugin {
		public class Items : IPlugin, IBundle {
			//members
			Dictionary<string, ContainerTuple> registeredContainers = new Dictionary<string, ContainerTuple>();
			Dictionary<string, WeaponTuple> registeredWeapons = new Dictionary<string, WeaponTuple>();
			Dictionary<string, WearableTuple> registeredWearables = new Dictionary<string, WearableTuple>();
			Dictionary<string, UsableTuple> registeredUsables = new Dictionary<string, UsableTuple>();
			GameController gameController;

			//container classes
			public struct ContainerTuple {
				public object displayName;
				public object spriteName;
				public object type; //carry, static
				public object capacity;
			}

			public struct WeaponTuple {
				public object displayName;
				public object spriteName;
				public object type; //melee, ranged, magic, etc.
				public object damage;
			}

			public struct WearableTuple {
				public object displayName;
				public object spriteName;
				public object onEquip;
				public object onUnequip;
			}

			public struct UsableTuple {
				public object displayName;
				public object spriteName;
				public object onUse;
			}

			//singleton pattern
			public IPlugin Singleton {
				get {
					if (singleton == null) {
						return singleton = new Items();
					}
					return singleton;
				}
			}
			static Items singleton = null;

			//IPlugin
			public void Initialize(Environment env, string alias) {
				env.Define(String.IsNullOrEmpty(alias) ? "Items" : alias, this, true);

				gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
			}

			//IBundle
			public object Property(Interpreter interpreter, Token token, object argument) {
				string propertyName = (string)argument;

				switch(propertyName) {
					case "RegisterContainer": return new RegisterContainer(this);
					case "RegisterWeapon": return new RegisterWeapon(this);
					case "RegisterWearable": return new RegisterWearable(this);
					case "RegisterUsable": return new RegisterUsable(this);

					case "SpawnContainer": return new SpawnContainer(this);
					case "SpawnWeapon": return new SpawnWeapon(this);
					case "SpawnWearable": return new SpawnWearable(this);
					case "SpawnUsable": return new SpawnUsable(this);

					default:
						throw new ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
				}
			}

			//ICallable classes
			public class RegisterContainer : ICallable {
				Items self = null;

				public RegisterContainer(Items self) {
					this.self = self;
				}

				public int Arity() {
					return 5; //key, displayName, spriteName, type, capacity
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					ContainerTuple tuple = new ContainerTuple();

					tuple.displayName = arguments[1];
					tuple.spriteName = arguments[2];
					tuple.type = arguments[3];
					tuple.capacity = arguments[4];

					self.registeredContainers[(string)arguments[0]] = tuple;

					return null;
				}
			}

			public class RegisterWeapon : ICallable {
				Items self = null;

				public RegisterWeapon(Items self) {
					this.self = self;
				}

				public int Arity() {
					return 5; //key, displayName, spriteName, type, capacity
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					WeaponTuple tuple = new WeaponTuple();

					tuple.displayName = arguments[1];
					tuple.spriteName = arguments[2];
					tuple.type = arguments[3];
					tuple.damage = arguments[4];

					self.registeredWeapons[(string)arguments[0]] = tuple;

					return null;
				}
			}

			public class RegisterWearable : ICallable {
				Items self = null;

				public RegisterWearable(Items self) {
					this.self = self;
				}

				public int Arity() {
					return 5; //key, displayName, spriteName, type, capacity
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					WearableTuple tuple = new WearableTuple();

					tuple.displayName = arguments[1];
					tuple.spriteName = arguments[2];
					tuple.onEquip = arguments[3];
					tuple.onUnequip = arguments[4];

					self.registeredWearables[(string)arguments[0]] = tuple;

					return null;
				}
			}

			public class RegisterUsable : ICallable {
				Items self = null;

				public RegisterUsable(Items self) {
					this.self = self;
				}

				public int Arity() {
					return 4; //key, displayName, spriteName, onUse
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					UsableTuple tuple = new UsableTuple();

					tuple.displayName = arguments[1];
					tuple.spriteName = arguments[2];
					tuple.onUse = arguments[3];

					self.registeredUsables[(string)arguments[0]] = tuple;

					return null;
				}
			}

			public class SpawnContainer : ICallable { //TODO: (1) spawn with contents
				Items self = null;

				public SpawnContainer(Items self) {
					this.self = self;
				}

				public int Arity() {
					return 3; //key, x, y
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					//spawn the item, which contains the tuple's information

					GameObject go = GameObject.Instantiate(self.gameController.containerPrefab);

					ContainerController controller = go.GetComponent<ContainerController>();
					ContainerTuple tuple = self.registeredContainers[(string)arguments[0]];

					controller.displayName = tuple.displayName;
					controller.spriteName = tuple.spriteName;
					controller.type = tuple.type;
					controller.capacity = tuple.capacity;
					controller.positionX = (int)(double)arguments[1];
					controller.positionY = (int)(double)arguments[2];

					self.gameController.CacheEntity(go);

					return null;
				}
			}

			public class SpawnWeapon : ICallable {
				Items self = null;

				public SpawnWeapon(Items self) {
					this.self = self;
				}

				public int Arity() {
					return 3; //key, x, y
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					//spawn the item, which contains the tuple's information

					GameObject go = GameObject.Instantiate(self.gameController.weaponPrefab);

					WeaponController controller = go.GetComponent<WeaponController>();
					WeaponTuple tuple = self.registeredWeapons[(string)arguments[0]];

					controller.displayName = tuple.displayName;
					controller.spriteName = tuple.spriteName;
					controller.type = tuple.type;
					controller.damage = tuple.damage;
					controller.positionX = (int)(double)arguments[1];
					controller.positionY = (int)(double)arguments[2];

					self.gameController.CacheEntity(go);

					return null;
				}
			}

			public class SpawnWearable : ICallable {
				Items self = null;

				public SpawnWearable(Items self) {
					this.self = self;
				}

				public int Arity() {
					return 3; //key, x, y
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					//spawn the item, which contains the tuple's information

					GameObject go = GameObject.Instantiate(self.gameController.wearablePrefab);

					WearableController controller = go.GetComponent<WearableController>();
					WearableTuple tuple = self.registeredWearables[(string)arguments[0]];

					controller.displayName = tuple.displayName;
					controller.spriteName = tuple.spriteName;
					controller.onEquip = tuple.onEquip;
					controller.onUnequip = tuple.onUnequip;
					controller.positionX = (int)(double)arguments[1];
					controller.positionY = (int)(double)arguments[2];

					self.gameController.CacheEntity(go);

					return null;
				}
			}

			public class SpawnUsable : ICallable {
				Items self = null;

				public SpawnUsable(Items self) {
					this.self = self;
				}

				public int Arity() {
					return 3; //key, x, y
				}

				public object Call(Interpreter interpreter, Token token, List<object> arguments) {
					//spawn the item, which contains the tuple's information

					GameObject go = GameObject.Instantiate(self.gameController.usablePrefab);

					UsableController controller = go.GetComponent<UsableController>();
					UsableTuple tuple = self.registeredUsables[(string)arguments[0]];

					controller.displayName = tuple.displayName;
					controller.spriteName = tuple.spriteName;
					controller.onUse = tuple.onUse;
					controller.positionX = (int)(double)arguments[1];
					controller.positionY = (int)(double)arguments[2];

					self.gameController.CacheEntity(go);

					return null;
				}
			}
		}
	}
}