using UnityEngine;
using System.Collections;

public class TileClickManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseUp() {
		Debug.Log("Holaaaa");
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0;
		
		int gridPosX = (int)(mousePos.x / 4f);
		int gridPosY = (int)(mousePos.y / 4f);
		
		Debug.Log(gridPosX.ToString() + " " + gridPosY.ToString());
	}
}
