using UnityEngine;
using System.Collections;

public class Ingredient : MonoBehaviour {
  public enum IngredientType {Color, Pet, Accessory, Style};
  public enum IngredientName {Red, Blue, Yellow, Green, Purple, Orange, Mohawk};

  public IngredientType ingredientType;
  public IngredientName ingredientName;
  public int ingredientProgress = 0;

	void Start () {
  }
	void Update () {
  }

  public bool isPrepared {
    get { return ingredientProgress >= 100; }
  }

  public void Prepare(float dT) {
    if(!isPrepared){
      ingredientProgress += 1;
      Debug.Log("TRACE "+this+" ("+ingredientProgress+"%)");
    }
  }
}