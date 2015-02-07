using UnityEngine;
using System.Collections;

public class SelectorController : MonoBehaviour {
	
	public GameObject boxPrefab;
	
	private bool selected = false;
	
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
			} else if (selected) {
				//SetSelected(false);
			}
		}
	}
	
	public void SetSelected(bool b) {
		selected = b;
		selectionShadow.gameObject.renderer.enabled = b;	
		if(b) {
			manageSelectionScript.currentlySelected = boxPrefab;
		} else manageSelectionScript.currentlySelected = null;
	}
	
	bool WithinBounds(Vector3 pos) {
		return pos.x >= transform.position.x-2f
			&& pos.x <= transform.position.x+2f
			&& pos.y >= transform.position.y-2f
			&& pos.y <= transform.position.y+2f;
	}
}
