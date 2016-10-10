using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : MonoBehaviour {
	public int maxContents = 1;
	public List<Portable> contents = new List<Portable>();
	public List<Ingredient> allowedIngredients = new List<Ingredient>();

	void Start () {}
	void Update () {}

	public bool isEmpty {
		get { return contents.Count == 0; }
	}

	public bool isFull {
		get { return contents.Count >= maxContents; }
	}

	public void FillWith(Portable portable) {
		if(!isFull){ contents.Add(portable); }
	}

	public void Clear() {
		foreach(Portable curPortable in contents){ curPortable.Drop(); }
		contents.Clear();
	}
}
