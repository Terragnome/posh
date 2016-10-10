using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : MonoBehaviour {
	public int maxContents = 1;
	public List<Portable> contents = new List<Portable>();
	public List<Ingredient> allowedIngredients = new List<Ingredient>();

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	public bool isFull {
		get { return contents.Count >= maxContents; }
	}

	public void FillWith(Portable portable) {
		if(!isFull){ contents.Add(portable); }
	}

	public void Clear() {
		contents.Clear();
	}
}
