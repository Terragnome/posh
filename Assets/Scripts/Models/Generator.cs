using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {
  public Ingredient payload;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {	
	}

  Container container {
    get {
      Container curContainer = GetComponent<Container>();
      if(curContainer == null){ curContainer = gameObject.AddComponent<Container>(); }
      return curContainer;
    }
  }

  public bool CanGenerate {
    get { return !container.isFull; }
  }

  public Portable Generate() {
    Portable newPortable = null;
    if(!container.isFull){
      Ingredient newIngredient = Object.Instantiate(payload);
      newPortable = newIngredient.GetComponent<Portable>();
      Debug.Log(newPortable.transform.position);
      // newPortable.DropOn(container);
    }
    return newPortable;
  }
}