using UnityEngine;
using System.Collections;

public class ManageSelection : MonoBehaviour {
	
	public GameObject currentlySelected;
	public float currentRotation = 0;
	
	private SelectorController[] childSelectionController;
	
	void Awake () {
		childSelectionController = GetComponentsInChildren<SelectorController>();
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void CancelSelection() {
		foreach(SelectorController selector in childSelectionController) {
			selector.SetSelected(false);
		}
	}
}
