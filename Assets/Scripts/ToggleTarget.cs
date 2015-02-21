using UnityEngine;
using System.Collections;

public class ToggleTarget : MonoBehaviour {

	private int lasersIn = 0;
	private Transform greenTransform;
	private Transform yellowTransform;
	
	private FloorManager floorManagerScript;
	
	void Awake () {
		greenTransform = transform.FindChild("TargetGreen");
		yellowTransform = transform.FindChild("TargetYellow");
		
		floorManagerScript = GameObject.Find("Floor").GetComponent<FloorManager>();
	}
	
	public void AddLaser() {
		if(lasersIn == 0) {
			// We need to turn on the green light
			yellowTransform.renderer.enabled = false;
			greenTransform.renderer.enabled = true;
			floorManagerScript.MarkTargetAsReached();
		}
		lasersIn += 1;
	}
	
	public void RemoveLaser() {
		lasersIn -= 1;
		if(lasersIn == 0) {
			// We need to turn on the yellow light
			greenTransform.renderer.enabled = false;
			yellowTransform.renderer.enabled = true;
			floorManagerScript.UnmarkTargetAsReached();
		}
	}
}
