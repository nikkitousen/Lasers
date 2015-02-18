using UnityEngine;
using System.Collections.Generic;

public class FloorManager : MonoBehaviour {
			
	public int width;
	public int height;
	public float tileSideLength;
	
	public string[,] grid;
	public float[,] gridRotation;
	public GameObject[,] gridObjects;
	
	public SelectorController currentlySelectedScript;
	
	private Transform tiles;
	private Transform elements;
	private List<LaserDrawer> laserDrawers = new List<LaserDrawer>();
	
	void Awake () {
		tiles = transform.FindChild("Tiles");
		elements = transform.FindChild("Elements");
		GameObject[] laserObjects = GameObject.FindGameObjectsWithTag("Laser");
		foreach(GameObject obj in laserObjects) {
			laserDrawers.Add(obj.GetComponent<LaserDrawer>());
		}
		
		// I need to do this in Awake and NOT in Start, since the Start function
		// of the lasers needs it
		
		grid = new string[width,height]; 
		gridRotation = new float[width,height];
		gridObjects = new GameObject[width,height];
		
		foreach(Transform tile in tiles) {
			int gridX = (int) (tile.localPosition.x / tileSideLength);
			int gridY = (int) (tile.localPosition.y / tileSideLength);
			if(tile.tag == "TileEmpty") {
				grid[gridX, gridY] = "Empty";
				TileClickController tileScript = tile.GetComponent<TileClickController>();
				tileScript.x = (int)gridX;
				tileScript.y = (int)gridY;
			} else if (tile.tag == "TileSolid") {
				grid[gridX, gridY] = "Solid";
			} else if (tile.tag == "TileTarget") {
				grid[gridX, gridY] = "Target";
				gridObjects[gridX, gridY] = tile.gameObject;
			}
		}
	}
	
	
	public void PlaceCurrenlySelectedBox(int x, int y) {
		if(currentlySelectedScript == null) return;
		
		GameObject prefab = currentlySelectedScript.boxPrefab;
		float rotation = currentlySelectedScript.currentRotation;
		int boxCount = currentlySelectedScript.boxCount;
		
		grid[x,y] = prefab.name;
		gridRotation[x,y] = rotation;
		
		GameObject newBox = Instantiate(prefab) as GameObject;
		newBox.transform.parent = elements;
		newBox.transform.localScale = Vector3.one;
		newBox.transform.localPosition = new Vector2(x * tileSideLength, y * tileSideLength);
		newBox.transform.localRotation = Quaternion.Euler(0,0,rotation);
		
		currentlySelectedScript.SetCounter(boxCount-1);
		currentlySelectedScript.SetSelected(false);
		
		foreach(LaserDrawer drawer in laserDrawers) {
			drawer.DrawBeam(drawer.location, drawer.direction);
		}
	}
}
