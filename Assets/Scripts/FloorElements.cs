using UnityEngine;
using System.Collections.Generic;

public class FloorElements : MonoBehaviour {
	
	public enum TileType { Empty, Dark, Box }; 
			
	public int width;
	public int height;
	
	public TileType[,] grid;
	
	private ManageSelection selectionManager;
	
	void Awake () {
		selectionManager = GameObject.Find("BoxMenu").GetComponent<ManageSelection>();
	}
	
	void Start () {
		Debug.Log("Holaaaa start");
		grid = new TileType[width,height];
		foreach(Transform child in transform) {
			int gridX = (int) (child.position.x / 4);
			int gridY = (int) (child.position.y / 4);
			if(child.name == "Tile") {
				grid[gridX, gridY] = TileType.Empty;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			if(mousePos.x < -2f || mousePos.x > width * 4f - 2f
			|| mousePos.y < -2f || mousePos.y > height * 4f - 2f) {
				return;
			}
			
			int gridPosX = (int)((mousePos.x+2f)/4f);
			int gridPosY = (int)((mousePos.y+2f)/4f);
			
			Debug.Log(gridPosX.ToString() + " " + gridPosY.ToString());
			
			if(selectionManager.currentlySelected != null) {
				Debug.Log("Enter");
				GameObject newBox = Instantiate(selectionManager.currentlySelected) as GameObject;
				newBox.transform.position = new Vector2(gridPosX * 4f, gridPosY * 4f);
				selectionManager.cancelSelection();
			}
		}
	}
}
