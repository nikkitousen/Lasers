using UnityEngine;
using System.Collections;

public class TileClickController : MonoBehaviour {
	
	public int x;
	public int y;
	private FloorManager floorManagerScript;

	void Awake () {
		floorManagerScript = GameObject.Find("Floor").GetComponent<FloorManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		Debug.Log("clicked!");
		if(floorManagerScript.currentlySelectedScript != null) {
			floorManagerScript.PlaceCurrenlySelectedBox(x, y);
		}
	}
}
