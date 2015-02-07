using UnityEngine;
using System.Collections;

public class ManageSelection : MonoBehaviour {
	
	public GameObject currentlySelected;
	
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
	
	public void cancelSelection() {
		foreach(SelectorController selector in childSelectionController) {
			selector.SetSelected(false);
		}
	}
}
