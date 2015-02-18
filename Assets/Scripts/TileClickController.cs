using UnityEngine;
using System.Collections;

public class TileClickController : MonoBehaviour {
	
	public int x;
	public int y;
	private FloorManager floorManagerScript;

	void Awake () {
		floorManagerScript = GameObject.Find("Floor").GetComponent<FloorManager>();
	}
	
	void OnMouseDown() {
		if(floorManagerScript.currentlySelectedScript != null
		&& floorManagerScript.grid[x,y] == "Empty") {
			floorManagerScript.PlaceCurrenlySelectedBox(x, y);
		}
	}
}
