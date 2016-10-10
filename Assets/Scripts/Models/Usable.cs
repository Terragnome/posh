using UnityEngine;
using System.Collections;

public class Usable : MonoBehaviour {

	void Start () {}
	void Update () {}

	public bool isUsable {
		get {
			if(container.isEmpty){ return false; }

			foreach(Portable curPortable in container.contents){
				Ingredient curIngredient = curPortable.GetComponent<Ingredient>();
				if(curIngredient != null && !curIngredient.isPrepared) { return true; }
			}

			return false;
		}
	}

	Collider col {
		get {
			BoxCollider boxCollider = GetComponent<BoxCollider>();
			if(boxCollider == null){
				boxCollider = gameObject.AddComponent<BoxCollider>();
				boxCollider.size = new Vector3(1f, 1f, 1f);
			}
			return boxCollider;
		}
	}

	Container container {
		get { return GetComponent<Container>(); }
	}

	public bool Use(float dT) {
		bool isUsing = false;
		foreach(Portable curPortable in container.contents){
			Ingredient curIngredient = curPortable.GetComponent<Ingredient>();
			if(curIngredient != null && !curIngredient.isPrepared) {
				curIngredient.Prepare(dT);
				isUsing = true;
			}
		}

		UpdateProgress(dT);
		return isUsing;
	}

	void UpdateProgress(float dT){
		Debug.Log("Using!");
	}
}
