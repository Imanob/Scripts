using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{

    GameObject[] inventorySlots;
    public int itemsInInventory;
    GameObject inv;

    public TextBoxManager textbox;

    Player player;

    public bool isOpen;

    public Text examineText;

    public Object[] itemArray;
    public List<Item> items = new List<Item>();
    public List<Item> inventoryItems = new List<Item>();

    /// <summary>
    /// Alustukset ja estetään canvasin tuhoutuminen scenevaihtojen yhteydessä.
    /// </summary>
    public void Start()
    {

        itemArray = Resources.LoadAll("PickableItems", typeof(Item));
        for (int i = 0; i < itemArray.Length; i++)
        {
            items.Add((Item)itemArray[i]);
        }
        
        textbox = FindObjectOfType<TextBoxManager>();
        examineText = GameObject.Find("TextArea").GetComponentInChildren(typeof(Text)) as Text;
        player = FindObjectOfType<Player>();
        inventorySlots = GameObject.FindGameObjectsWithTag("Slot");
        Sort(inventorySlots);
        inv = GameObject.Find("Inventory");
        inv.SetActive(false);
    }

    /// <summary>
    /// Updatessa kaikki inventoryn avaamiseen ja sulkemiseen liittyvä
    /// </summary>
    void Update()
    {
        if (textbox.isActive) return;
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (isOpen)
            {
                player.canMove = true;
                inv.SetActive(false);
                isOpen = false;
            }
            else
            {
                player.canMove = false;
                inv.SetActive(true);
                isOpen = true;
                Button firstButton = inventorySlots[0].GetComponentInParent(typeof(Button)) as Button;
                firstButton.Select();
                firstButton.OnSelect(null);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
        {
            player.canMove = true;
            inv.SetActive(false);
            isOpen = false;
        }
    }
    /// <summary>
    /// Algortimi inventoryn paikkojen järjestämiseen. Ei tapahdu automaattisesti Start-funktion FindGameObjectsWithTag-funktiossa.
    /// </summary>
    /// <param name="itemSlotArray">Järjestettävä GameObject-array</param>
    public void Sort(GameObject[] itemSlotArray)
    {
        List<string> nameList = new List<string>();
        GameObject[] sortedItemSlotArray = new GameObject[itemSlotArray.Length];

        foreach (GameObject item in itemSlotArray)
        {
            nameList.Add(item.name);
        }
        nameList.Sort();
        int itemsAdded = 0;

        foreach (string slot in nameList)
        {
            for (int i = 0; i < itemSlotArray.Length; i++)
            {
                if (itemSlotArray[i].name == slot)
                {
                    sortedItemSlotArray[itemsAdded++] = itemSlotArray[i];
                    break;
                }
            }
        }
        inventorySlots = sortedItemSlotArray;
    }
    /// <summary>
    /// Lisää itemin inventoryyn
    /// </summary>
    /// <param name="item">Tieto itemistä, joka halutaan lisätä. Se, joka oikeasti lisätään, löytyy items-listasta</param>
    public void AddItem(GameObject item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name == item.name)
            {
                Image img = inventorySlots[itemsInInventory++].GetComponent<Image>();
                img.sprite = items[i].sprite;
                img.color = new Color(195f, 195f, 195f, 255f); // Kaikki napit ovat defaulttina läpinäkyviä (195f, 195f, 195f, 0f)
                inventoryItems.Add(items[i]);
                if (items[i].destroyOnPickUp)
                {
                    Destroy(item);
                }
                else
                {
                    if (textbox.currentObject != null && textbox.currentObject.name == "Spike Talk Zone") FindObjectOfType<Spike>().Fetch();
                    Destroy(item.transform.GetChild(0).gameObject); // Tuhoaa talk zonen, olettaen että se on sen 0:s lapsi. Parempi systeemi myöhemmin
                }
                Debug.Log("Item picked up from " + items[i].name);

                if (items[i].name != "Spike") GameObject.Find("ItemSounds").GetComponent<ItemSoundController>().PlayItemPickupSound();
            }
        }
    }
    /// <summary>
    /// Controlleri, joka triggeröityy kun inventorystä highlightataan itemi. Näyttää examine textin.
    /// </summary>
    /// <param name="pos">Monennesko slot inventoryssä</param>
    public void HighlightController(int pos)
    {
        if (inventoryItems.Count >= pos + 1)
            examineText.text = inventoryItems[pos].examineText;
        else examineText.text = "";
    }
}
