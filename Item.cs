using UnityEngine;

/// <summary>
/// Luokka Item-olioiden luontia varten.
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

    new public string name = "New Item";
    public Sprite sprite = null;
    public string examineText = "";
    public bool destroyOnPickUp;

}
