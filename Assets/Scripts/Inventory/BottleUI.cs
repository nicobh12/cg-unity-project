using UnityEngine;
using TMPro;

public class BottleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text bottleText;
    private InventorySystem inventory;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inventory = player.GetComponent<InventorySystem>();
        }
    }

    private void Update()
    {
        if (inventory == null) return;

        int bottles = inventory.GetItemQuantity("bottle");
        bottleText.text = "Botellas: " + bottles;
    }
}