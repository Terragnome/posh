using UnityEngine;
using System.Collections;

public class Container : MonoBehaviour {
	Portable contents = null;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	public bool isFilled {
		get { return contents != null; }
	}

	public void FillWith(Portable portable) {
		contents = portable;
	}

	public void Clear() {
		contents = null;
	}
}
