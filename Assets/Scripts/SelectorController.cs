using UnityEngine;
using System.Collections;

public class SelectorController : MonoBehaviour {
	
	public GameObject boxPrefab;
	
	private bool selected = false;
	private float currentRotation = 0f;
	
	private Transform selectionShadow;
	private ManageSelection manageSelectionScript;
	
	void Awake () {
		selectionShadow = transform.FindChild("SelectionShadow");
		manageSelectionScript = GetComponentInParent<ManageSelection>();
	}
	
	void Start () {
		SetSelected(false);
	}
	
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0;
			if(!selected && WithinBounds(mousePos)) {
				SetSelected(true);
			} else if (selected && WithinBounds(mousePos)) {
				currentRotation -= 90f;
				if(currentRotation == -90f) currentRotation = 270f;
				//transform.rotation = Quaternion.Euler(0, 0, currentRotation);
				manageSelectionScript.currentRotation = currentRotation;
			}
		}
		
		transform.rotation = Quaternion.Slerp(transform.rotation,
											  Quaternion.Euler(0, 0, currentRotation),
											  5f * Time.deltaTime);
		selectionShadow.transform.rotation = Quaternion.identity;
		
	}
	
	public void SetSelected(bool b) {
		selected = b;
		selectionShadow.gameObject.renderer.enabled = b;	
		if(b) {
			manageSelectionScript.currentlySelected = boxPrefab;
		} else {
			manageSelectionScript.currentlySelected = null;
		}
		currentRotation = 0f;
		transform.rotation = Quaternion.Euler(0, 0, 0);
	}
	
	bool WithinBounds(Vector3 pos) {
		return pos.x >= transform.position.x-2f
			&& pos.x <= transform.position.x+2f
			&& pos.y >= transform.position.y-2f
			&& pos.y <= transform.position.y+2f;
	}
}
