using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : Toy.IBundle {
	//members
	Dictionary<string, object> dictionary = new Dictionary<string, object>();
	GameController gameController = null;

	//can't use a constructor
	public void FindGameController() {
		if (gameController == null) {
			gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
		}
	}

	//accessors & mutators
	public object GetStat(string name) {
		FindGameController();
		return gameController.ExtractFunctions(dictionary[name], new List<object>());
	}

	public void SetStat(string name, object val) {
		dictionary[name] = val;
	}

	//IBundle
	public object Property(Toy.Interpreter interpreter, Toy.Token token, object argument) {
		string propertyName = (string)argument;

		switch(propertyName) {
			case "GetStat": return new GetStatCallable(this);
			case "SetStat": return new SetStatCallable(this);

			default:
				throw new Toy.ErrorHandler.RuntimeError(token, "Unknown property '" + propertyName + "'");
		}
	}

	//ICallable functions
	public class GetStatCallable : Toy.ICallable {
		Statistics self = null;

		public GetStatCallable(Statistics self) {
			this.self = self;
		}

		public int Arity() {
			return 1; //name
		}

		public object Call(Toy.Interpreter interpreter, Toy.Token token, List<object> arguments) {
			if (!(arguments[0] is string)) {
				self.FindGameController();
				self.gameController.ShowError("stat name must be a string");
			}

			return self.GetStat((string)arguments[0]);
		}
	}

	public class SetStatCallable : Toy.ICallable {
		Statistics self = null;

		public SetStatCallable(Statistics self) {
			this.self = self;
		}

		public int Arity() {
			return 2; //name, value
		}

		public object Call(Toy.Interpreter interpreter, Toy.Token token, List<object> arguments) {
			if (!(arguments[0] is string)) {
				self.FindGameController();
				self.gameController.ShowError("stat name must be a string");
				return null;
			}

			self.SetStat((string)arguments[0], arguments[1]);
			return null;
		}
	}
}