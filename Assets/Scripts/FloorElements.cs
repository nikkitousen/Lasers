using UnityEngine;
using System.Collections;

public class FloorElements : MonoBehaviour {
	
	public enum TileType { Empty, Dark, Box }; 
			
	public int width;
	public int height;
	
	public TileType[,] grid;
	
	void Start () {
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
	}
}
