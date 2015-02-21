using UnityEngine;
using System.Collections.Generic;

public class LaserDrawer : MonoBehaviour {
	
	public enum Direction {Up, Down, Left, Right};
	
	public Vector2 location;
	public Direction direction;
	public GameObject longBeam;
	public GameObject shortBeam;
	
	private Transform spawn;
	
	private FloorManager floorManagerScript;
	
	private List<GameObject> currentLongBeams = new List<GameObject>();
	private List<GameObject> currentShortBeams = new List<GameObject>();
	private Stack<GameObject> longBeamPool = new Stack<GameObject>();
	private Stack<GameObject> shortBeamPool = new Stack<GameObject>();
	
	private List<GameObject> targetsReached = new List<GameObject>();
	
	void Awake () {
		floorManagerScript = GameObject.Find("Floor").GetComponent<FloorManager>();
		spawn = transform.FindChild("Spawn");
	}

	void Start () {
		location = transform.localPosition/4f;
		switch((int)spawn.eulerAngles.z) {
			case 90:
				direction = Direction.Up;
				break;
			case 180:
				direction = Direction.Left;
				break;
			case 270:
				direction = Direction.Down;
				break;
			default:
				direction = Direction.Right;
				break;
		}
	
		DrawBeam(Vector2.zero, direction); // not to be called in "Awake", otherwise will mess with the construction of grid
	}
	
	public void DrawBeam(Vector2 startPos, Direction startDir, bool startFromBox = false) {
	// startPos is the position wrt the laser origin, i.e. always starts at (0,0)
	
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
	
		Vector2 currentLocalPos = startPos;
		Direction currentDir = startDir;
		while(true) {
			
			Vector2 currentGridPos = currentLocalPos + location;
		
			if(currentGridPos.x < 0 || currentGridPos.x >= floorManagerScript.width
			|| currentGridPos.y < 0 || currentGridPos.y >= floorManagerScript.height) {
				break;
			}
			
			GameObject newBeam = null;
			
			if(startFromBox && currentLocalPos == startPos) {
				if(shortBeamPool.Count == 0) {
					newBeam = Instantiate(shortBeam) as GameObject;
					newBeam.transform.parent = transform;
					newBeam.transform.localScale = Vector3.one;
				} else {
					newBeam = shortBeamPool.Pop();
					newBeam.renderer.enabled = true;
				}
				currentShortBeams.Add(newBeam);
				
				newBeam.transform.localPosition = currentLocalPos * floorManagerScript.tileSideLength;
				
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
				string currentTile = floorManagerScript.grid[(int)currentGridPos.x, (int)currentGridPos.y];
				
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
					
					newBeam.transform.localPosition = currentLocalPos * floorManagerScript.tileSideLength;
					
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
					
					newBeam.transform.localPosition = currentLocalPos * floorManagerScript.tileSideLength;
					
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
					
					float boxRotation = floorManagerScript.gridRotation[(int)currentGridPos.x, (int)currentGridPos.y];
					
					// Now we need to spawn the other beams, if any, and end the function
					
					if(currentTile == "Target") {
						GameObject tileTargetObj = floorManagerScript.gridObjects[(int)currentGridPos.x, (int)currentGridPos.y];
						tileTargetObj.GetComponent<ToggleTarget>().AddLaser();
						targetsReached.Add(tileTargetObj);
					} else {
						List<Direction> directionsToSpawn = getOutputDirections(currentDir, currentTile, boxRotation);
						foreach(Direction dir in directionsToSpawn) {
							DrawBeam(currentLocalPos, dir, true);
						}
					}
					
					break;
				}
			}
			
			if(currentDir == Direction.Up) {
				currentLocalPos.y += 1f;
			} else if(currentDir == Direction.Down) {
				currentLocalPos.y -= 1f;
			} else if(currentDir == Direction.Left) {
				currentLocalPos.x -= 1f;
			} else if(currentDir == Direction.Right) {
				currentLocalPos.x += 1f;
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
