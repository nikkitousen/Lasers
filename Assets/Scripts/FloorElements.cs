using UnityEngine;
using System.Collections.Generic;

public class FloorElements : MonoBehaviour {
			
	public int width;
	public int height;
	public float tileSideLength;
	
	public string[,] grid;
	public float[,] gridRotation;
	public GameObject[,] gridObjects;
	
	private ManageSelection selectionManager;
	private List<LaserDrawer> laserDrawers = new List<LaserDrawer>();
	
	void Awake () {
		selectionManager = GameObject.Find("BoxMenu").GetComponent<ManageSelection>();
		GameObject[] laserObjects = GameObject.FindGameObjectsWithTag("Laser");
		foreach(GameObject obj in laserObjects) {
			laserDrawers.Add(obj.GetComponent<LaserDrawer>());
		}
		Debug.Log("Drawers = " + laserDrawers.Count.ToString());
		
		
		// I need to do this in Awake and NOT in Start, since the Start function
		// of the lasers needs it
		
		grid = new string[width,height]; 
		gridRotation = new float[width,height];
		gridObjects = new GameObject[width,height];
		
		foreach(Transform child in transform) {
			int gridX = (int) (child.localPosition.x / tileSideLength);
			int gridY = (int) (child.localPosition.y / tileSideLength);
			if(child.tag == "TileEmpty") {
				grid[gridX, gridY] = "Empty";
			} else if (child.tag == "TileSolid") {
				grid[gridX, gridY] = "Solid";
			} else if (child.tag == "TileTarget") {
				grid[gridX, gridY] = "Target";
				gridObjects[gridX, gridY] = child.gameObject;
			}
		}
		
		Debug.Log("The scale is " + transform.localScale.ToString());
	}
	
	void Start () {
		
	}
	
	void Update () {
		float calculatedTileLength = tileSideLength * transform.localScale.x; // assumes tiles to be squares
		float minX = -calculatedTileLength/2f;
		float maxX = width * calculatedTileLength - calculatedTileLength/2f;
		float minY = -calculatedTileLength/2f;
		float maxY = height * calculatedTileLength - calculatedTileLength/2f;
	
		if(Input.GetMouseButtonDown(0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			if(mousePos.x > minX && mousePos.x < maxX
			&& mousePos.y > minY && mousePos.y < maxY) {
				int gridPosX = (int)((mousePos.x+calculatedTileLength/2f)/calculatedTileLength);
				int gridPosY = (int)((mousePos.y+calculatedTileLength/2f)/calculatedTileLength);
				
				Debug.Log("Clicked on tile " + gridPosX.ToString() + " " + gridPosY.ToString());
				
				if(selectionManager.currentlySelected != null && grid[gridPosX,gridPosY] == "Empty") {
					grid[gridPosX,gridPosY] = selectionManager.currentlySelected.name;
					gridRotation[gridPosX,gridPosY] = selectionManager.currentRotation;
					
					GameObject newBox = Instantiate(selectionManager.currentlySelected) as GameObject;
					newBox.transform.parent = transform;
					newBox.transform.localScale = Vector3.one;
					newBox.transform.localPosition = new Vector2(gridPosX * tileSideLength, gridPosY * tileSideLength);
					newBox.transform.localRotation = Quaternion.Euler(0,0,selectionManager.currentRotation);
					
					
					foreach(LaserDrawer drawer in laserDrawers) {
						drawer.DrawBeam(drawer.location, drawer.direction);
					}
					
				}
				
				if(selectionManager.currentlySelected != null) {
					selectionManager.CancelSelection();
				}
			}
		}
	}
}
