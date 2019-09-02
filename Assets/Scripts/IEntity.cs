using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity {
	void Tick();
	int positionX { get; set; }
	int positionY { get; set; }
}