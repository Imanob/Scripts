using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterestController : MonoBehaviour {

    /// <summary>
    /// Luokka Point of Interestin (=UI-huutomerkki, kun pelaaja on lähellä itemiä, jota voi tutkia) hallitsemista varten
    /// </summary>

    public List<string> checkedPOILocations = new List<string>();
    GameObject poi;
    TextBoxManager textBoxMgr;
    public static bool poiExists;

    /// <summary>
    /// Alustukset
    /// </summary>
    void Start ()
    {
        textBoxMgr = FindObjectOfType<TextBoxManager>();
        poi = GameObject.Find("Point of Interest");
        DisablePoI();
        if (!poiExists)
        {
            DontDestroyOnLoad(transform.root.gameObject);
            poiExists = true;
        }
        else
        {
            Destroy(transform.root.gameObject);
        }
	}

    private void OnLevelWasLoaded(int level)
    {
        try { DisablePoI(); } catch {}
    }

    void Update () {
		
	}
    /// <summary>
    /// Enable PoI
    /// </summary>
    public void EnablePoI()
    {
        poi.SetActive(true);
    }
    
    /// <summary>
    /// Disable PoI
    /// </summary>
    public void DisablePoI()
    {
        if (poi != null) poi.SetActive(false);
    }

    /// <summary>
    /// Liikuttaa PoI:n halutun GameObjectin päälle
    /// </summary>
    /// <param name="target">GameObject, jonka päälle PoI liikutetaan</param>
    public void Move(Vector3 target)
    {
        switch (textBoxMgr.currentObject.name)
        {
            case "Basement Talk Zone":
                poi.transform.position = new Vector3(target.x - 0.15f, target.y - 0.45f, target.z);
                break;
            case "1950s Basement Talk Zone":
                poi.transform.position = new Vector3(target.x - 0.15f, target.y - 0.45f, target.z);
                break;
            case "Tree Talk Zone":
                poi.transform.position = new Vector3(target.x + 0.2f, target.y - 0.8f, target.z);
                break;
            default:
                poi.transform.position = new Vector3(target.x + 0.2f, target.y + 0.5f, target.z);
                break;
        }
    }

    /// <summary>
    /// Vaihtaa PoI:n paikkaa halutun esineen päälle
    /// </summary>
    /// <param name="currentObjectName">Esineen nimi, jonka päälle PoI halutaan</param>
    public void FindPoITarget(string currentObjectName)
    {
        GameObject poiTarget = GameObject.Find(currentObjectName);
        if (CheckPOI(currentObjectName, false)) return;
        Move(poiTarget.transform.parent.position);
        textBoxMgr.currentObject = GameObject.Find(currentObjectName);
        EnablePoI();
    }

    /// <summary>
    /// Katsoo, onko Point of Interest löydetty aikaisemmin.
    /// </summary>
    /// <param name="currentObjectName">GameObjectin nimi</param>
    /// <param name="add">Jos true, niin löydetty POI lisätään löydettyjen listaan. Jos false, niin halutaan vain tietää halutaanko POI:ta näyttää</param>
    /// <returns>False, jos Point of Interestiä ei ole löydetty aikaisemmin ja se lisättiin listaan. Muuten true</returns>
    public bool CheckPOI(string currentObjectName, bool add)
    {
        switch (currentObjectName)
        {
            case "Grimoire Talk Zone":
                return false;
            case "1950s Basement Talk Zone":
                return false;
        }

        bool isAdded = false;

        foreach(string loc in checkedPOILocations)
        {
            if (loc == currentObjectName)
                isAdded = true;
        }
        if (!isAdded)
        {
            if (add)
                checkedPOILocations.Add(currentObjectName);
            return false;
        }
        return true;
    }
}
