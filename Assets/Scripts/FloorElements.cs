﻿using UnityEngine;
using System.Collections.Generic;

public class FloorElements : MonoBehaviour {
	
//	public enum TileType { Empty, Dark, Box }; 
			
	public int width;
	public int height;
	
//	public TileType[,] grid;
	public string[,] grid;
	public float[,] gridRotation;
	
	private ManageSelection selectionManager;
	private List<LaserDrawer> laserDrawers = new List<LaserDrawer>();
	
	void Awake () {
		selectionManager = GameObject.Find("BoxMenu").GetComponent<ManageSelection>();
		GameObject[] laserObjects = GameObject.FindGameObjectsWithTag("Laser");
		foreach(GameObject obj in laserObjects) {
			laserDrawers.Add(obj.GetComponent<LaserDrawer>());
		}
		Debug.Log("Drawers = " + laserDrawers.Count.ToString());
	}
	
	void Start () {
		grid = new string[width,height];
		gridRotation = new float[width,height];
		foreach(Transform child in transform) {
			int gridX = (int) (child.position.x / 4);
			int gridY = (int) (child.position.y / 4);
			if(child.name == "Tile") {
				grid[gridX, gridY] = "Empty";
				gridRotation[gridX, gridY] = 0f;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			if(mousePos.x > -2f && mousePos.x < width * 4f - 2f
			&& mousePos.y > -2f && mousePos.y < height * 4f - 2f) {
				int gridPosX = (int)((mousePos.x+2f)/4f);
				int gridPosY = (int)((mousePos.y+2f)/4f);
				
				Debug.Log(gridPosX.ToString() + " " + gridPosY.ToString());
				
				if(selectionManager.currentlySelected != null) {
					grid[gridPosX,gridPosY] = selectionManager.currentlySelected.name;
					gridRotation[gridPosX,gridPosY] = selectionManager.currentRotation;
					
					GameObject newBox = Instantiate(selectionManager.currentlySelected) as GameObject;
					newBox.transform.position = new Vector2(gridPosX * 4f, gridPosY * 4f);
					newBox.transform.rotation = Quaternion.Euler(0,0,selectionManager.currentRotation);
					
					
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
