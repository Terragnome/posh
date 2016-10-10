using UnityEngine;
using System.Collections;

public class Generator : MonoBehaviour {

  public Ingredient payload;

	void Start () {}
  void Update () {}

  static GameObject _portablesManager;
  GameObject portablesManager {
    get {
      if(_portablesManager == null){ _portablesManager = GameObject.Find("Portables"); }
      return _portablesManager;
    }
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
      newIngredient.transform.parent = portablesManager.transform;

      newPortable = newIngredient.GetComponent<Portable>();
      newPortable.DropOn(container);
    }
    return newPortable;
  }
}