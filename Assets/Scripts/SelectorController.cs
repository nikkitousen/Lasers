using UnityEngine;
using System.Collections;

public class SelectorController : MonoBehaviour {
	
	public int boxCount;
	public GameObject boxPrefab;
	public float currentRotation = 0f;
	
	private bool selected = false;
	
	private Transform boxSprite;
	private Transform selectionShadow;
	private FloorManager floorManagerScript;
	private TextMesh boxCountMesh;
	
	void Awake () {
		boxSprite = transform.FindChild("BoxSprite");
		selectionShadow = transform.FindChild("SelectionShadow");

		boxCountMesh = transform.FindChild("BoxCounter").GetComponent<TextMesh>();
		floorManagerScript = GameObject.Find("Floor").GetComponent<FloorManager>();
	}
	
	void Start () {
		SetSelected(false);
		SetCounter(boxCount);
	}
	
	void OnMouseDown() {
		if(!selected) {
			SetSelected(true);
		} else if (selected) {
			currentRotation -= 90f;
			if(currentRotation == -90f) currentRotation = 270f;
		}
	}
	
	void Update () {		
		boxSprite.transform.rotation = Quaternion.Slerp(boxSprite.transform.rotation,
											  			Quaternion.Euler(0, 0, currentRotation),
											  			5f * Time.deltaTime);
	}
	
	public void SetSelected(bool b) {
		if(b && boxCount == 0) return;
		selected = b;
		selectionShadow.gameObject.renderer.enabled = b;	
		if(b) {
			if(floorManagerScript.currentlySelectedScript != null)
				floorManagerScript.currentlySelectedScript.SetSelected(false);
			floorManagerScript.currentlySelectedScript = this;
		} else {
			floorManagerScript.currentlySelectedScript = null;
		}
		currentRotation = 0f;
		boxSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
	}
	
	public void SetCounter(int count) {
		boxCountMesh.text = "x" + count.ToString();
		
		if(boxCount == 0 && count != 0){
			Color newColor = boxSprite.GetComponent<SpriteRenderer>().color;
			newColor.a = 1f;
			boxSprite.GetComponent<SpriteRenderer>().color = newColor;
		}
		
		boxCount = count;
		
		if(boxCount == 0) {
			Color newColor = boxSprite.GetComponent<SpriteRenderer>().color;
			newColor.a = 0.25f;
			boxSprite.GetComponent<SpriteRenderer>().color = newColor;
		}
	}
}
