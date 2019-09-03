using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour {
	GameController gameController = null;
	TextMeshProUGUI childText = null;

	public string title;
	public string currentStat;
	public string maxStat;

	double defaultWidth = 0;

	void Awake() {
		gameController = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
		childText = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

		defaultWidth = ((RectTransform)transform).rect.xMax - ((RectTransform)transform).rect.xMin;
	}

	void FixedUpdate() {
		object current = gameController.GetPlayerStatistics().GetStat(currentStat);
		object max = gameController.GetPlayerStatistics().GetStat(maxStat);

		if (!(current is double)) {
			gameController.ShowError(currentStat + " must resolve to a number");
			return;
		}

		if (!(max is double)) {
			gameController.ShowError(maxStat + " must resolve to a number");
			return;
		}

		((RectTransform)transform).sizeDelta = new Vector2((float)( (double)current / (double)max * defaultWidth ), ((RectTransform)transform).sizeDelta.y);
		childText.text = title + ": " + (double)current + "/" + (double)max;
	}
}