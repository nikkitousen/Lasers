using UnityEngine;
using System.Collections.Generic;

public class LaserDrawer : MonoBehaviour {
	
	public enum Direction {Up, Down, Left, Right};
	
	public Vector2 location;
	public Direction direction;
	public GameObject longBeam;
	public GameObject shortBeam;
	
	private FloorElements floorElements;
	
	private List<GameObject> currentLongBeams = new List<GameObject>();
	private List<GameObject> currentShortBeams = new List<GameObject>();
	private Stack<GameObject> longBeamPool = new Stack<GameObject>();
	private Stack<GameObject> shortBeamPool = new Stack<GameObject>();
	
	private List<GameObject> targetsReached = new List<GameObject>();
	
	void Awake () {
		floorElements = GameObject.Find("Floor").GetComponent<FloorElements>();
	}

	// Use this for initialization
	void Start () {
		DrawBeam(location, direction);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void DrawBeam(Vector2 startPos, Direction startDir, bool startFromBox = false) {
		if(!startFromBox) {
			// I should erase the previous laser
			foreach(GameObject beam in currentLongBeams){
				beam.transform.rotation = Quaternion.Euler(0,0,0);
				beam.renderer.enabled = false;
				longBeamPool.Push(beam);
			}
			currentLongBeams = new List<GameObject>();
			
			foreach(GameObject beam in currentShortBeams){
				beam.transform.rotation = Quaternion.Euler(0,0,0);
				beam.renderer.enabled = false;
				shortBeamPool.Push(beam);
			}
			currentShortBeams = new List<GameObject>();
			
			foreach(GameObject target in targetsReached) {
				target.GetComponent<ToggleTarget>().RemoveLaser();
			}
			targetsReached = new List<GameObject>();
				
		}
		
		Debug.Log("Draw Laser at " + startPos.ToString() + ", " + startFromBox.ToString());
	
		Vector2 currentPos = startPos;
		Direction currentDir = startDir;
		while(true) {
		
			if(currentPos.x < 0 || currentPos.x >= floorElements.width
			|| currentPos.y < 0 || currentPos.y >= floorElements.height) {
				break;
			}
			
			GameObject newBeam = null;
			
			if(startFromBox && currentPos == startPos) {
				if(shortBeamPool.Count == 0) {
					newBeam = Instantiate(shortBeam) as GameObject;
					newBeam.transform.parent = transform;
					newBeam.transform.localScale = Vector3.one;
				} else {
					newBeam = shortBeamPool.Pop();
					newBeam.renderer.enabled = true;
				}
				currentShortBeams.Add(newBeam);
				
				newBeam.transform.localPosition = currentPos * floorElements.tileSideLength;
				
				// The beam is "leaving" the box
				switch(currentDir) {
					case Direction.Down:
						newBeam.transform.localRotation = Quaternion.Euler(0, 0, 90);
						break;
					case Direction.Right:
						newBeam.transform.localRotation = Quaternion.Euler(0, 0, 180);
						break;
					case Direction.Up:
						newBeam.transform.localRotation = Quaternion.Euler(0, 0, 270);
						break;
					default:
						newBeam.transform.localRotation = Quaternion.Euler(0, 0, 0);
						break;
				}
			} else {
				Debug.Log("trying to access " + currentPos.ToString());
				Debug.Log(floorElements.grid);
				string currentTile = floorElements.grid[(int)currentPos.x, (int)currentPos.y];
				
				if(currentTile == "Solid") {
					break;
				} else if(currentTile == "Empty") {
					if(longBeamPool.Count == 0) {
						newBeam = Instantiate(longBeam) as GameObject;
						newBeam.transform.parent = transform;
						newBeam.transform.localScale = Vector3.one;
					} else {
						newBeam = longBeamPool.Pop();
						newBeam.renderer.enabled = true;
					}
					currentLongBeams.Add(newBeam);
					
					
					newBeam.transform.localPosition = currentPos * floorElements.tileSideLength;
					
					if(currentDir == Direction.Up || currentDir == Direction.Down) {
						newBeam.transform.localRotation = Quaternion.Euler(0, 0, 90);
					}
				} else {
					// It's a box!
					if(shortBeamPool.Count == 0) {
						newBeam = Instantiate(shortBeam) as GameObject;
						newBeam.transform.parent = transform;
						newBeam.transform.localScale = Vector3.one;
					} else {
						newBeam = shortBeamPool.Pop();
						newBeam.renderer.enabled = true;
					}
					currentShortBeams.Add(newBeam);
					
					newBeam.transform.localPosition = currentPos * floorElements.tileSideLength;
					
					// The beam is "entering" the box
					switch(currentDir) {
						case Direction.Down:
							newBeam.transform.localRotation = Quaternion.Euler(0, 0, 270);
							break;
						case Direction.Right:
							newBeam.transform.localRotation = Quaternion.Euler(0, 0, 0);
							break;
						case Direction.Up:
							newBeam.transform.localRotation = Quaternion.Euler(0, 0, 90);
							break;
						default:
							newBeam.transform.localRotation = Quaternion.Euler(0, 0, 180);
							break;
					}
					
					float boxRotation = floorElements.gridRotation[(int)currentPos.x, (int)currentPos.y];
					
					// Now we need to spawn the other beams, if any, and end the function
					
					if(currentTile == "Target") {
						//Debug.Log("Reached a target");
						GameObject tileTargetObj = floorElements.gridObjects[(int)currentPos.x, (int)currentPos.y];
						tileTargetObj.GetComponent<ToggleTarget>().AddLaser();
						targetsReached.Add(tileTargetObj);
					} else {
						List<Direction> directionsToSpawn = getOutputDirections(currentDir, currentTile, boxRotation);
						foreach(Direction dir in directionsToSpawn) {
							DrawBeam(currentPos, dir, true);
						}	
					}
					
					break;
				}
			}
			
			if(currentDir == Direction.Up) {
				currentPos.y += 1f;
			} else if(currentDir == Direction.Down) {
				currentPos.y -= 1f;
			} else if(currentDir == Direction.Left) {
				currentPos.x -= 1f;
			} else if(currentDir == Direction.Right) {
				currentPos.x += 1f;
			}
		}
	}
	
	List<Direction> getOutputDirections(Direction inDir, string boxType, float boxRotation) {
		
		List<Direction> results = new List<Direction>();
		
		// Let's see if we add "UP"
		if(inDir != Direction.Down) {
			if(boxType == "Box2") {
				if((boxRotation == 0f && inDir == Direction.Right)
				|| (boxRotation == 270f && inDir == Direction.Left))
					results.Add(Direction.Up);
			} else if(boxType == "Box3") {
				if((boxRotation == 0f && inDir != Direction.Up)
				|| (boxRotation == 90f && inDir != Direction.Left)
				|| (boxRotation == 270f && inDir != Direction.Right))
					results.Add(Direction.Up);
			} else if(boxType == "Box4") {
				results.Add(Direction.Up);
			}
		}
		
		// Let's see if we add "DOWN"
		if(inDir != Direction.Up) {
			if(boxType == "Box2") {
				if((boxRotation == 90f && inDir == Direction.Right)
				|| (boxRotation == 180f && inDir == Direction.Left))
					results.Add(Direction.Down);
			} else if(boxType == "Box3") {
				if((boxRotation == 180f && inDir != Direction.Down)
				|| (boxRotation == 90f && inDir != Direction.Left)
				|| (boxRotation == 270f && inDir != Direction.Right))
					results.Add(Direction.Down);
			} else if(boxType == "Box4") {
				results.Add(Direction.Down);
			}
		}
		
		// Let's see if we add "LEFT"
		if(inDir != Direction.Right) {
			if(boxType == "Box2") {
				if((boxRotation == 0f && inDir == Direction.Down)
			    || (boxRotation == 90f && inDir == Direction.Up))
					results.Add(Direction.Left);
			} else if(boxType == "Box3") {
				if((boxRotation == 0f && inDir != Direction.Up)
				|| (boxRotation == 90f && inDir != Direction.Left)
				|| (boxRotation == 180f && inDir != Direction.Down))
					results.Add(Direction.Left);
			} else if(boxType == "Box4") {
				results.Add(Direction.Left);
			}
		}
		
		// Let's see if we add "RIGHT"
		if(inDir != Direction.Left) {
			if(boxType == "Box2") {
				if((boxRotation == 180f && inDir == Direction.Up)
				|| (boxRotation == 270f && inDir == Direction.Down))
					results.Add(Direction.Right);
			} else if(boxType == "Box3") {
				if((boxRotation == 0f && inDir != Direction.Up)
				|| (boxRotation == 270f && inDir != Direction.Right)
				|| (boxRotation == 180f && inDir != Direction.Down))
					results.Add(Direction.Right);
			} else if(boxType == "Box4") {
				results.Add(Direction.Right);
			}
		}
		
		return results;
	}
}
