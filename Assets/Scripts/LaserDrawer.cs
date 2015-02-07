using UnityEngine;
using System.Collections;

public class LaserDrawer : MonoBehaviour {
	
	public enum Direction {Up, Down, Left, Right};
	
	public Vector2 location;
	public Direction direction;
	public GameObject longBeam;
	public GameObject shortBeam;
	
	private FloorElements floorElements;
	
	
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
	
	void DrawBeam(Vector2 startPos, Direction startDir) {
		Direction currentDir = startDir;
		Vector2 currentPos = startPos;
		while(true) {
		
			if(currentPos.x < 0 || currentPos.x >= floorElements.width
			|| currentPos.y < 0 || currentPos.y >= floorElements.height) {
				break;
			}
			
			GameObject newBeam = Instantiate(longBeam) as GameObject;
			newBeam.transform.position = currentPos * 4f;
			if(currentDir == Direction.Up || currentDir == Direction.Down) {
				// rotate 90 degrees
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
}
