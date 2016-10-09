using UnityEngine;
using System.Collections;

public class Ingredient : MonoBehaviour {
  public enum IngredientType {Color, Pet, Accessory, Style};
  public enum IngredientName {Red, Blue, Yellow, Green, Purple, Orange, Mohawk};

  public IngredientType ingredientType;
  public IngredientName ingredientName;

	// Use this for initialization
	void Start () {	
	}

	// Update is called once per frame
	void Update () {
	}
}
