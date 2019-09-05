using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	[SerializeField] GameObject slotPrefab = null;

	GameController gameController = null;

	bool active = false;
	public bool trigger = false;

	void Awake() {
		gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
	}

	void Update() {
		SetActive(trigger);
	}

	public void SetActive(bool active) {
		if (this.active == active) {
			return;
		}

		//switch modes
		this.active = active;

		if (this.active) { //TODO: (1) handle no carry object
			ContainerController containerController = gameController.GetPlayerCarryGameObject().GetComponent<ContainerController>();

			List<object> contents = containerController.GetContents();

			for (int i = 0; i < (int)(double)containerController.realCapacity; i++) {
				GameObject go = GameObject.Instantiate(slotPrefab, gameObject.transform);

				if (i < contents.Count) {
					go.GetComponent<Image>().sprite = ((Toy.GameObjectWrapper)contents[i]).GetSelf().GetComponent<SpriteRenderer>().sprite;
				}
			}
		} else {
			foreach(Transform child in transform) {
				GameObject.Destroy(child.gameObject);
			}
		}
	}
}