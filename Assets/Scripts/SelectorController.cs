using UnityEngine;
using System.Collections;

public class SelectorController : MonoBehaviour {
	
	private bool selected = false;
	
	private Transform selectionShadow;
	
	void Awake () {
		selectionShadow = transform.FindChild("SelectionShadow");
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
				SetSelected(false);
			}
		}
	}
	
	void SetSelected(bool b) {
		selected = b;
		selectionShadow.gameObject.renderer.enabled = b;	
	}
	
	bool WithinBounds(Vector3 pos) {
		return pos.x >= transform.position.x-2f
			&& pos.x <= transform.position.x+2f
			&& pos.y >= transform.position.y-2f
			&& pos.y <= transform.position.y+2f;
	}
}
