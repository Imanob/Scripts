using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using sh = StoryHandler;
using UnityEngine.SceneManagement;
using System;

public class TextBoxManager : MonoBehaviour
{
    /// <summary>
    /// Luokka Text boxien ja valintojen hallitsemista varten.
    /// </summary>
    public GameObject textBox;

    public GameObject[] options = new GameObject[5];

    public InventoryManager inv;

    public List<TextAsset> queueList;
    public Text text;

    //public bool normalizeFont;

    public TextAsset textFile;
    public string[] textLines;
    public char[] charLine;
    private int currentChar;
    public int currentLine;
    public int endLine;

    public int nextLine;

    public Player player;

    public bool isActive;

    [HideInInspector]
    public PointOfInterestController poi;
    [HideInInspector]
    public ActivateText hitbox;

    public bool waitForAction;
    private bool skipKeyPressed;
    private bool actionKeyPressed;

    public string lineSkipChoice;
    public string yesChoice;
    public int choiceCount;
    public int currentChoice;
    public bool pickUpItem;

    public GameObject currentObject;

    string[] optionArray;
    int amountOfOptions;

    private float count;
    [Range(1.0f, 15.0f)]
    public float textSpeed;

    public AudioClip grimoireSound;

    William william;
    public GameObject willEditor;
    public static GameObject will;
    public static bool canvasAdded;

    /// <summary>
    /// Alustukset
    /// </summary>
    void Awake()
    {
        poi = FindObjectOfType<PointOfInterestController>();
        player = FindObjectOfType<Player>();
        hitbox = FindObjectOfType<ActivateText>();
        inv = FindObjectOfType<InventoryManager>();
        will = willEditor;
        william = new William(0);
        if (will != null) will.SetActive(false);
        count = -10;

        if (textFile != null)
        {
            textLines = (textFile.text.Split('\n'));
            charLine = textLines[currentLine].ToCharArray();
        }

        if (isActive)
        {
            EnableTextBox();

        }
        else
        {
            DisableTextBox();
        }

        if (!canvasAdded)
        {
            DontDestroyOnLoad(transform.root.gameObject);
            canvasAdded = true;
        }
        else
        {
            Destroy(transform.root.gameObject);
        }

    }
    /// <summary>
    /// Update lisää yhden kirjaimen textboxissa näkyvään tekstiin. 
    /// </summary>
    void LateUpdate()
    {

        if (!isActive) // Ei kayda updatea lapi, jos missaan ei ole auki textboxia
        {
            if (!inv.isOpen) player.canMove = true;
            return;
        }
        player.canMove = false;

        skipKeyPressed = false;
        /*
        if (normalizeFont == false && currentLine <= endLine)
        {
            if (textLines[currentLine].Contains("Mum:") || textLines[currentLine].Contains("Frank:") || textLines[currentLine].Contains("Frank Jr.:") || textLines[currentLine].Contains("William:") || textLines[currentLine].Contains("Emilie:"))
            {
                FontStyle fontStyle = FontStyle.Bold;
                charLine[currentChar].fontStyle = fontStyle;
                if (charLine[currentChar] == ':')
                {
                    text.fontStyle = FontStyle.Normal;
                    normalizeFont = true;
                }
            }
        }
        */

        if (Input.GetKeyDown(KeyCode.Space))
        {
            waitForAction = false;
            text.text = "";
            actionKeyPressed = true;
            if (currentChar > 0 && currentChar < charLine.Length - 1) skipKeyPressed = true;
        }

        if (skipKeyPressed)
        {
            skipText();
            return;
        }
        /*
         * Delay ennen updaten lapikayntia. Maarittaa tekstin nopeuden.
         */
        while (count <= 0)
        {
            if (actionKeyPressed) break;
            count += Time.deltaTime * textSpeed * 80;
            return;
        }
        count = -10;


        skipKeyPressed = false;
        actionKeyPressed = false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            waitForAction = false;
            text.text = "";
            actionKeyPressed = true;
            if (currentChar > 0 && currentChar < charLine.Length - 1) skipKeyPressed = true;
        }


        if (currentLine > endLine)
        {
            if (!actionKeyPressed) return;
            DisableTextBox();
            if (currentObject != null) poi.CheckPOI(currentObject.name, true);
            return;
        }

        if (currentChar > charLine.Length - 1)
        {
            NextLine();
            waitForAction = true;
            return;
        }

        endLine = textLines.Length - 1;

        if (!waitForAction)
        {
            if (charLine[currentChar] == '*' && !options[0].activeSelf)
            {
                ShowChoices(charLine);
                waitForAction = true;
                return;
            }
            else
            {
                text.text += charLine[currentChar++];
            }
        }

        if (textLines[currentLine].Contains("[end]"))
        {
            currentLine = endLine;
            text.text.Replace("[end]", "");
        }
        actionKeyPressed = false;
    }

    void NextLine()
    {
        currentChar = 0;
        currentLine++;
        if (currentLine <= endLine) charLine = textLines[currentLine].ToCharArray();
    }
    /// <summary>
    /// Skippaa tekstiä, jos pelaaja on painanut spacebaria.
    /// </summary>
    /// <returns></returns>
    void skipText()
    {
        bool showOptions = false;
        text.text = "";
        for (int i = 0; i < charLine.Length; i++)
        {
            if (charLine[i] == '[' && charLine[i+1] == 'e')
            {
                currentLine = endLine;
                break;
            }
            if (charLine[i] != '*')
            {
                text.text += charLine[i];
            }
            else
            {
                showOptions = true;
                break;
            }
        }
        waitForAction = true;

        if (showOptions && !options[0].activeSelf)
        {
            currentChar = charLine.Length - 1;
            ShowChoices(charLine);
        }
        else
        {
            NextLine();
        }
    }
    /// <summary>
    /// Laittaa textbox-UI:n päälle
    /// </summary>
    public void EnableTextBox()
    {
        poi.DisablePoI();
        textBox.SetActive(true);
        isActive = true;
        player.canMove = false;

    }
    /// <summary>
    /// Sulkee textbox-UI:n ja alustaa arvot seuraavaa luettavaa tekstitiedostoa varten
    /// </summary>
    public void DisableTextBox()
    {
        textBox.SetActive(false);
        isActive = false;
        player.canMove = true;
        currentLine = 0;
        text.text = "";
        currentChar = 0;
        //normalizeFont = false;
        if (poi != null) poi.DisablePoI();
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i] != null) options[i].SetActive(false);
        }
        hitbox.changeStatus(true);
        StoryProgression();   
    }

    /// <summary>
    /// Textboxin sulkemisen jälkeen koodissa tapahtuvat tarinalliset progressiot
    /// </summary>
    public void StoryProgression()
    {
        if (currentObject == null)
        {
            CheckQueue();
            if (william.Conv1 == 1)
            {
                if (will != null) will.SetActive(true);
                sh.dialogue2Finished = true;
                sh.enter1950sBasement = false;
            }

            if (sh.dialogue2TalkWithEmilie)
            {
                FindObjectOfType<BlackScreenFade>().FadeIn();
            }
            return;
        }
        switch (currentObject.name)
        {
            case "1950s Frank's Bed Talk Zone":
                Dream dream = FindObjectOfType<Dream>();
                dream.FadeOut();
                break;
            case "Frank's Bed":
                Dream dream1 = FindObjectOfType<Dream>();
                dream1.FadeOut();
                break;
            case "1950s Grimoire Talk Zone":
                william.Conv1 = 1;
                PickUpGrimoire();
                CheckQueue();
                break;
            default:
                break;
        }
    }

   /// <summary>
   /// Nostaa Grimoiren pelaajan inventoryyn 50-luvulla
   /// </summary>
    public void PickUpGrimoire()
    {
        sh.basementVisited = true;
        inv.AddItem(currentObject.transform.parent.gameObject);
        Destroy(currentObject.transform.parent.gameObject);
    }
    /// <summary>
    /// Lukee tekstitiedoston.
    /// </summary>
    /// <param name="textToLoad"></param>
    /// 
    public bool ReloadDialogue(TextAsset textToLoad)
    {
        if (textToLoad == null)
        {
            if (currentObject.tag == "lightswitch")
            {
                LightController controller = currentObject.GetComponent<LightController>();
                controller.SwitchLights();
                return false;
            }
            Debug.Log("Text file not found");
            return false;
        }
        choiceCount = textToLoad.text.Count(x => x.Equals('*'));
        textLines = new string[1];
        textLines = (textToLoad.text.Split('\n'));
        charLine = new char[1];
        charLine = textLines[currentLine].ToCharArray();
        return true;
    }
    /// <summary>
    /// Controlleri buttonien kasittelemista varten
    /// </summary>
    public void ButtonPressController()
    {
        if (pickUpItem && currentObject != null)
        {
            inv.AddItem(currentObject.transform.parent.gameObject);
            currentChoice = 0;
            pickUpItem = false;
        }
    }
    /// <summary>
    /// Nayttaa valinnat.
    /// </summary>
    /// <param name="dialogueLine">koko luettava rivi char arrayna</param>
    public string[] ShowChoices(char[] dialogueLine)
    {
        currentChoice++;
        if (currentChoice == choiceCount) pickUpItem = true;
        optionArray = new string[5];
        bool startParsing = false;
        amountOfOptions = 0;
        for (int i = 0; i < dialogueLine.Length; i++)
        {
            if (startParsing)
            {
                if (dialogueLine[i] == ';')
                {
                    amountOfOptions++;
                    i++;
                }
                optionArray[amountOfOptions] += dialogueLine[i];
            }
            else if (dialogueLine[i] == '*') startParsing = true;
        }
        Text choiceText;
        for (int i = 0; i < amountOfOptions + 1; i++)
        {
            /*
             * Vaihtaa currentLinen optioneissa määrättyyn lukuun, sekä poistaa luvun pelaajan näkyvistä.
             */
            if (optionArray[i].Any(char.IsDigit) && optionArray[i].Contains("]"))
            {
                string numberString = Regex.Match(optionArray[i], @"\d+").Value;
                nextLine = Int32.Parse(numberString);
                optionArray[i] = optionArray[i].Replace("[" + numberString + "]", "");
                lineSkipChoice = optionArray[i];
            }
            /*
             * Määrittää yes-valinnan optioneitten perusteella, sekä poistaa "[yes]"-stringin pelaajan näkyvistä.
             */
            if (optionArray[i].Contains("[yes]"))
            {
                optionArray[i] = optionArray[i].Replace("[yes]", "");
                yesChoice = optionArray[i];
            }
            choiceText = options[i].GetComponentInChildren(typeof(Text)) as Text;
            choiceText.text = optionArray[i];
            options[i].SetActive(true);

            if (i == amountOfOptions)
            {
                Button highestButton = choiceText.GetComponentInParent(typeof(Button)) as Button;
                highestButton.Select();
                highestButton.OnSelect(null);
            }
        }

        return optionArray;
    }


    public void CheckForMeaningfulChoice(string choice)
    {
        switch (choice)
        {
            case "You're not going to the bar?":
                sh.dialogue2TalkWithEmilie = true;
                break;
        }
    }
    /// <summary>
    /// Tyhjentää katsotut Point of Interestit ja sanoo LoadNewArealle, että lataa uuden scenen
    /// </summary>
    /// <param name="scene">Ladattavan scenen nimi</param>
    public void LoadNewScene(string scene)
    {
        poi.checkedPOILocations = new List<string>();
        FindObjectOfType<LoadNewArea>().LoadNewScene(scene);
    }

    /// <summary>
    /// Aikahypätään jos pelaajalla on kynä inventoryssä. 
    /// </summary>
    /// <returns>False jos ei kynää, true jos on</returns>
    public bool ActivateGrimoire()
    {
        bool penFound = false;
        foreach (Item i in inv.inventoryItems)
        {
            if (i.name == "Frank's Cabinet") penFound = true;
        }
        if (penFound == false)
        {
            TextAsset grimoireText = Resources.Load("NO_PEN") as TextAsset;
            hitbox.ManuallyActivateText(grimoireText, true);
            return false;
        }
        Destroy(currentObject.transform.parent.gameObject);
        LoadNewScene("1950s Downstairs");
        return true;
    }
    /// <summary>
    /// Aktivoi ja palauttaa Frankin ajatuksia numeron perusteella.
    /// </summary>
    /// <param name="thought"></param>
    /// <returns>Ajatuksen TextAssettina</returns>
    public TextAsset Thoughts(int thought)
    {
        TextAsset text = null;
        switch (thought)
        {
            case 6:
                if (!sh.basementVisited || sh.dialogue2Finished == true) break;
                text = Resources.Load("Thoughts6") as TextAsset;
                hitbox.ManuallyActivateText(text, false);
                Queue(Dialogue(2));
                //Queue(Thoughts(8));
                return null;
            case 8:
                text = Resources.Load("Thoughts8") as TextAsset;
                return text;
            default:
                return null;
        }
        return null;
    }
    /// <summary>
    /// Palauttaa dialogin numeron perusteella.
    /// </summary>
    /// <param name="dialogueNumber"></param>
    /// <returns>Dialogin textAssettina</returns>
    public TextAsset Dialogue(int dialogueNumber)
    {
        TextAsset text = null;
        switch (dialogueNumber)
        {
            case 2:
                text = Resources.Load("Dialogue2") as TextAsset;
                return text;
        }
        return null;
    }
    /// <summary>
    /// Lisää jonoon.
    /// </summary>
    /// <param name="text"></param>
    public void Queue(TextAsset text)
    {
        queueList.Add(text);
    }
    /// <summary>
    /// Palauttaa jonon ensimmäisen alkion.
    /// </summary>
    /// <returns></returns>
    public TextAsset Queue()
    {
        if (queueList.Count > 0)
        {
            TextAsset textToReturn = null;
            textToReturn = queueList.First();
            return textToReturn;
        }
        return null;
    }
    /// <summary>
    /// Jos jonossa on jotain, niin enablee textboxin ja poistaa jonosta kauiten jonottaneen tekstin.
    /// </summary>
    public void CheckQueue()
    {
        if (Queue() != null)
        {
            ReloadDialogue(Queue());
            EnableTextBox();
            queueList.Remove(queueList.First());
        }
    }
}
