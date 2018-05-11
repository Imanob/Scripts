using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using sh = StoryHandler;

public class LoadNewArea : MonoBehaviour {
    /// <summary>
    /// Scrptiä kutsutaan Scenen latauksen yhteydessä. Hallitsee itemeiden näyttämistä Sceneittäin
    /// </summary>

    [SerializeField]
    public string _levelToLoad;
    public string levelToLoad {  get { return _levelToLoad; } }

    public Vector3 playerStartLocation;

    /// <summary>
    /// Kutsutaan LoadNewSceneä
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && levelToLoad.Length > 0)
        {
            if (LoadNewScene(levelToLoad))
                SetPlayerLocation();

        }
    }

    /// <summary>
    /// Asettaa pelaajan ja kameran oikeaan kohtaan
    /// </summary>
    public void SetPlayerLocation()
    {
        CameraController camera = FindObjectOfType<CameraController>();
        camera.transform.position = new Vector3(playerStartLocation.x, playerStartLocation.y, playerStartLocation.z - 10);
        Player player = FindObjectOfType<Player>();
        player.transform.position = playerStartLocation;
        
    }

    public void ManuallySetPlayerLocation(Vector3 loc)
    {
        Player player = FindObjectOfType<Player>();
        player.transform.position = loc;
    }

    /// <summary>
    /// Lataa uuden scenen ja aktivoi siinä näkyvät itemit
    /// </summary>
    /// <param name="sceneName"></param>
    public bool LoadNewScene(string sceneName)
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "1950s Downstairs":
                if (!sh.dialogue2Finished || sh.dream1Finished) break;
                if (sceneName == "1950s Yard" || sceneName == "1950s Basement")
                {              
                    FindObjectOfType<ActivateText>().ManuallyActivateText(Resources.Load("MOVEMENT_NOT_ALLOWED") as TextAsset, true);
                    return false;                   
                } 
                break;
            case "1950s Basement":
                FindObjectOfType<TextBoxManager>().Thoughts(6);
                if (!sh.basementVisited)
                {
                    FindObjectOfType<ActivateText>().ManuallyActivateText(Resources.Load("NO_INTERACTION_WITH_BOOK") as TextAsset, false);
                    return false;
                }
                break;
        }
        Debug.Log("Loading " + sceneName);
        SceneManager.LoadScene(sceneName);

        List<GameObject> allObjects = GetDontDestroyOnLoadObjects();

        foreach (GameObject go in allObjects) // Aktivoi DontDestroyOnLoadin sceneen kuuluvat itemit ja deaktivoi loput
        {
            if (go.tag == "ItemsRoot" && !go.name.Contains(sceneName))
            {
                go.SetActive(false);
            }
            else if (go.tag == "ItemsRoot" && go.name.Contains(sceneName))
            {
                go.SetActive(true);
            }
        }
        return true;
    }
    /// <summary>
    /// Palauttaa kaikki GameObjectit DontDestroyOnLoadissa. Palauttaa myös inaktiiviset GameObjectit.
    /// </summary>
    /// <returns>DontDestroyOnLoadin gameobjectit listana</returns>
    public static List<GameObject> GetDontDestroyOnLoadObjects()
    {
        List<GameObject> result = new List<GameObject>();

        List<GameObject> rootGameObjectsExceptDontDestroyOnLoad = new List<GameObject>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            rootGameObjectsExceptDontDestroyOnLoad.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
        }

        List<GameObject> rootGameObjects = new List<GameObject>();
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform root = allTransforms[i].root;
            if (root.hideFlags == HideFlags.None && !rootGameObjects.Contains(root.gameObject))
            {
                rootGameObjects.Add(root.gameObject);
            }
        }

        for (int i = 0; i < rootGameObjects.Count; i++)
        {
            if (!rootGameObjectsExceptDontDestroyOnLoad.Contains(rootGameObjects[i]))
                result.Add(rootGameObjects[i]);
        }

        return result;
    }
}
