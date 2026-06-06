using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Definition", fileName = "NewItemDefinition")]
public class ItemDefinition : ScriptableObject
{
    public string itemId;
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;
    public bool stackable = true;
    public int maxStackSize = 99;
}
