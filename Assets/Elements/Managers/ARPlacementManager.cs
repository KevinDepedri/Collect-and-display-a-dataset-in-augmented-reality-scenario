using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class ARPlacementManager : DilmerGames.Core.Singletons.Singleton<ARPlacementManager>
{
    private GameObject _placedGameObject = null;
    private List<GameObject> _placedGameObjectList = new List<GameObject>();
    public Dictionary<IntPtr, string> loclaAnchorPtrCloudAnchorIdMapping = new Dictionary<IntPtr, string>();
    
    private int _spawnedElementCounter = 0;
    private int _retrievedPhotoCounter = 0;

    private bool _cleanNullsInNextFrame = false;
    
    private ARCameraManager _arCameraManager = null;
    private Quaternion _photoRotation;
    private Quaternion _backPhotoRotation;
    private Vector3 _photoPosition;
    
    public List<GameObject> elementsUI = new List<GameObject>();
    private bool _nextFrameAcquisition = false;
    private float _screenWidth = 0;
    private float _screenHeight = 0;
    private bool _vertical;
    public string file_name = "coordinates&rotation.txt";
    
    
    
    // EXECUTION BEFORE THE FIRST FRAME
    void Awake()
    {
        //Identify the needed components
        _arCameraManager = GetComponent<ARCameraManager>();
        if (_arCameraManager == null)
        {
            _arCameraManager = FindObjectOfType<ARCameraManager>();
        }
    }

    // EXECUTION OF THE FIRST FRAME
    void Start()
    {
        //Save screen width and screen height
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;
        
        //Print a starting message
        ARDebugManager.Instance.LogSuccess("AR HOLOGRAPHIC PHOTO SESSION STARTED", true);
        ARDebugManager.Instance.LogInfo("Scan the interested area for at least 15/30sec before placing objects", true);
    }

    // FUNCTION USED TO GET THE DETECT A TOUCH IN THE BEGINNING PHASE THAT HAS NOT HAPPENED ON GUI 
    bool TryGetTouchPosition()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                var touchPosition = touch.position;
                bool isOverUI = touchPosition.IsPointOverUIObject();
                return isOverUI ? false : true;
            }
        }
        return false;
    }
    
    // EXECUTION FOR ALL THE FRAMES AFTER THE FIRST ONE
    void Update()
    {
        // USED TO REMOVE "MISSING GAME-OBJECTS" FROM THE LIST IN THE EXACT NEXT FRAME AFTER THAT "RemoveLastPlacement()" HAS BEEN CALLED
        if (_cleanNullsInNextFrame)
        {
            for (int i = 0; i < _placedGameObjectList.Count; i++)
            {
                if (_placedGameObjectList[i] != null) continue;
                _placedGameObjectList.RemoveAt(i);
                ARDebugManager.Instance.LogInfo($"Current list elements: {_placedGameObjectList.Count}, updated counter {_spawnedElementCounter}", false);
            }
            //Avoid to repeat the procedure if no elements have been removed from the list
            _cleanNullsInNextFrame = false;
        }
        
        // AS SOON AS THE TOUCH IS DETECTED (NOT ON GUI) THEN START THE PROCEDURE, OTHERWISE BREAK THE UPDATE LOOP
        if (TryGetTouchPosition())
        {
            //Extract camera transform
            var arCameraTransform = _arCameraManager.transform;
            
            //Extract current camera position and generate a vector with position coordinates
            Vector3 arCameraTransformPosition = arCameraTransform.position;
            _photoPosition = new Vector3(arCameraTransformPosition.x, arCameraTransformPosition.y);
            
            //Extract current camera rotation and generate a rotation for the screenshot that will we will take after
            Quaternion arCameraTransformRotation = arCameraTransform.rotation;
            _photoRotation = arCameraTransformRotation * Quaternion.Euler(90, 270, 90);
        
            //Take the screenshot
            TakeScreenshot();
        }
        // AFTER THAT THE CO-ROUTINE "SCREENSHOT" HAS BEEN PERFORMED PERFORM THIS NEXT PART OF SCRIPT
        else if (_nextFrameAcquisition)
        {
            //Make sure to not execute again this script if no new acquisition are performed
            _nextFrameAcquisition = !_nextFrameAcquisition;
            
            //Generate the screenshot GameObject and the respective local anchor
            _placedGameObject = GenerateGameObject(_photoPosition, _photoRotation);
            var anchor = _placedGameObject.AddComponent<ARAnchor>();
        
            //Link the anchor to the GameObject, add the GameObject to the list of GameObjects in the first available spot and increase the counter for the next one
            _placedGameObject.transform.parent = anchor.transform;
            _placedGameObjectList.Insert(_spawnedElementCounter, _placedGameObject);
            ARDebugManager.Instance.LogInfo($"Object: [{_placedGameObjectList[_spawnedElementCounter]}] in list in position [{_spawnedElementCounter}]", false);
            ARDebugManager.Instance.LogInfo("Object successfully placed", true);
            _spawnedElementCounter += 1;
            
            //Add the GameObject anchor pointer to the dictionary that performs the mapping between local anchors (Type: ARAnchor) and cloud ones (Type: ARCloudAnchor)
            loclaAnchorPtrCloudAnchorIdMapping.Add(_placedGameObject.GetComponent<ARAnchor>().nativePtr, null);

            //Put the anchor of the new GameObject in queue to be updated on the cloud when the button "HOST" is pressed
            ARCloudAnchorManager.Instance.QueueAnchor(anchor, _vertical);
        }
    }
    
    // FUNCTION USED TO REMOVED UI ELEMENTS AND TO START THE SCREEN-SHOOTING PROCEDURE
    private void TakeScreenshot()
    {
        //Turn off all the elements that compose the UI (UserInterface) to take a full-screen screenshot without overlapping elements
        foreach (var element in elementsUI)
        {
            element.SetActive(false);
        }

        //Call the Co-routine "ScreenShot" that is executed into the next frame, this allows us to effectively 
        //remove the UI elements since they are removed only at the end of the current frame
        StartCoroutine(nameof(Screenshot));
    }
    
    // CO-ROUTINE CALLED BY THE FUNCTION "TakeScreenshot()"
    private IEnumerator Screenshot()
    {
        //Wait for the current frame to end (in this way UI elements are removed)
        yield return new WaitForEndOfFrame();
        
        //Enable the procedure that need to be executed in the frame next to the acquisition
        _nextFrameAcquisition = true;
        
        //Create a texture with screen resolution, acquire the current pixel configuration and apply it to the texture
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        //Encode the texture to PNG and define the saving path as: Android/Data/com.DittaDemolizioni.Ar_NEW2/files/SavedImage/image-X.png
        var bytes = texture.EncodeToPNG();
        var dirpath = Application.persistentDataPath + "/SavedImage/"; 
        var filepath = Application.persistentDataPath + "/SavedImage/" + file_name;
        
        //If the directory does not exists then create it. Write the image and finally destroy the texture
        if (!Directory.Exists(dirpath)) Directory.CreateDirectory(dirpath);
        File.WriteAllBytes(dirpath + "Image-" + (_retrievedPhotoCounter) + ".png", bytes);
        
        using (StreamWriter writer = new StreamWriter(filepath, true))
        {
            writer.WriteLine("Image-" + _retrievedPhotoCounter);
            writer.WriteLine("\n");
            writer.WriteLine("coordinate x: " + _photoPosition.x);
            writer.WriteLine("coordinate y: " + _photoPosition.y);
            writer.WriteLine("coordinate z: " + _photoPosition.z);
            writer.WriteLine("quaternion rotation: " + _photoRotation);
            writer.WriteLine("\n");

        }
        
        Destroy(texture);
        
        //Turn on all the UI elements
        foreach (var element in elementsUI)
        {
            element.SetActive(true);
        }
    }
    
    // FUNCTION CALLED BY "GenerateGameObject()" TO DEFINE THE SCREEN-RATIO
    private List<float> ScreenRatio() 
    {
        //Define the variables to save the results
        List<float> ratiosList = new List<float>();
        float widthScreenRatio, heightScreenRatio;
        
        //In the case of a vertical photo setup
        if (Screen.width < Screen.height)
        {
            _vertical = true;
            widthScreenRatio = (float)Screen.width/Screen.height;
            heightScreenRatio = 1;
        }
        //In the case of an horizontal photo setup
        else
        {
            _vertical = false;
            widthScreenRatio = 1;
            heightScreenRatio = (float)Screen.height/Screen.width;
        }
        
        //Return the computed ratios
        ratiosList.Add(widthScreenRatio);
        ratiosList.Add(heightScreenRatio);
        return ratiosList;
    }

    // FUNCTION USED TO GENERATE A NEW SCREENSHOT GAMEOBJECT
    private GameObject GenerateGameObject(Vector3 position, Quaternion rotation) 
    {
        //Create a primitive plane GameObject
        GameObject screenShot = GameObject.CreatePrimitive(PrimitiveType.Plane);
        
        //Apply the computed ScreenRatio to the plane
        List<float> ratiosList = ScreenRatio();
        var widthScreenRatio = ratiosList[0];
        var heightScreenRatio = ratiosList[1];
        Vector3 objectDimension = new Vector3((float)0.04 * widthScreenRatio, (float)0.01, (float)0.04 * heightScreenRatio);
        screenShot.transform.localScale = objectDimension;
        
        //Create a texture with the correct dimension, load the image from the local directory and associate the texture to the plane
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        string filePath = Application.persistentDataPath + "/SavedImage/" + "Image-" + _retrievedPhotoCounter + ".png";
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            texture.LoadImage(fileData);
            texture.Apply();
            screenShot.GetComponent<Renderer>().material.mainTexture = texture;
        }
        
        //Increase the PhotoCounter and associate to the correct position and rotation, finally return it
        _retrievedPhotoCounter += 1;
        screenShot.transform.position = position;
        screenShot.transform.rotation = rotation;
        return screenShot;
    }
    
    // USED BY THE FUNCTION "ResolveAnchor()" OF THE CLASS "ARCloudAnchorManager" TO RESPAWN OBJECT LINKED WITH CLOUD ANCHORS
    public void ReCreatePlacement(Transform cloudAnchorTransform, int photoCounter, bool vertical, string cloudAnchorId) 
    {
        //Instantiate the GameObject(Screenshot) in the given Transform of the CloudAnchor (Given by the function "ResolveAnchor()" of the ARCloudAnchorManager)
        _placedGameObject = LoadGameObject(cloudAnchorTransform.position, cloudAnchorTransform.rotation, photoCounter, vertical);
        //Generate a local anchor and attach it to the GameObject 
        var anchor = _placedGameObject.AddComponent<ARAnchor>();
        _placedGameObject.transform.parent = anchor.transform;

        //Add the GameObject to the list of GameObjects in the first available spot and increase the counter for the next one
        _placedGameObjectList.Insert(_spawnedElementCounter, _placedGameObject);
        ARDebugManager.Instance.LogInfo($"NOW IN LIST: [{_placedGameObjectList[_spawnedElementCounter]}] in position [{_spawnedElementCounter}]", false);
        ARDebugManager.Instance.LogInfo("Object successfully retrieved", true);
        _spawnedElementCounter += 1;
        
        //Add the GameObject local anchor and the cloud ones in the dictionary that maps them together
        loclaAnchorPtrCloudAnchorIdMapping.Add(anchor.nativePtr, cloudAnchorId);
    }
    
    // USED BY THE FUNCTION "ReCreatePlacement()" TO GENERATE THE CORRECT OBJECT
    private GameObject LoadGameObject(Vector3 position, Quaternion rotation, int photoCounter, bool vertical)
    {
        //Create a primitive plane GameObject
        GameObject screenShot = GameObject.CreatePrimitive(PrimitiveType.Plane);
       
        //Apply the correct ScreenRatio according to the original direction of the acquisition
        float widthScreenRatio,  heightScreenRatio;
        //In the case of a vertical photo setup
        if (vertical)
        {
            widthScreenRatio = _screenWidth/_screenHeight;
            heightScreenRatio = 1;
        } 
        //In the case of an horizontal photo setup
        else
        {
            widthScreenRatio = 1;
            heightScreenRatio = _screenHeight/_screenWidth;
        }
        Vector3 objectDimension = new Vector3((float)0.04 * widthScreenRatio, (float)0.01, (float)0.04 / heightScreenRatio);
        screenShot.transform.localScale = objectDimension;

        //Create a texture with the correct dimension, load the image from the local directory and associate the texture to the plane
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        string filePath = Application.persistentDataPath + "/SavedImage/" + "Image-" + photoCounter + ".png";
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            texture.LoadImage(fileData);
            texture.Apply();
            screenShot.GetComponent<Renderer>().material.mainTexture = texture;
        }
        
        //Associate to the correct position and rotation to the GameObject, finally return it
        screenShot.transform.position = position;
        screenShot.transform.rotation = rotation;
        return screenShot;
    }    
    
    // FUNCTION CALLED FROM THE BUTTON: "REMOVE LAST OBJ" TO REMOVE THE LATEST PLACED/RESOLVED OBJECT
    public void RemoveLastPlacement() 
    {
        //If we have at least one element in the scene execute the function
        if (_spawnedElementCounter > 0)
        {
            ARDebugManager.Instance.LogWarning($"REMOVING FROM LIST ELEMENT: [{_placedGameObjectList[_spawnedElementCounter - 1]}] in position [{_spawnedElementCounter - 1}]", false);
            ARDebugManager.Instance.LogInfo("Object successfully removed", true);

            //Extract object to remove, place it in the list with a null (useful to remove the element in the next Update() loop and also to call the GarbageCollector).
            GameObject toDestroy = _placedGameObjectList[_spawnedElementCounter - 1];
            _placedGameObjectList[_spawnedElementCounter - 1] = null;
            
            //If the element is also present in the mapping remove it from the mapping to allow it to be resoled again from the cloud when required
            if (loclaAnchorPtrCloudAnchorIdMapping.Count > 0)
            {
                loclaAnchorPtrCloudAnchorIdMapping.Remove(toDestroy.GetComponent<ARAnchor>().nativePtr);
            }
           
            //Finally destroy the element and update the counter
            Destroy(toDestroy);
            _spawnedElementCounter -= 1;

            //Enable for the next frame the cleaning of the nulls in the list
            _cleanNullsInNextFrame = true;

            //Dequeue the anchor if it has still not been hosted
            ARCloudAnchorManager.Instance.DeQueueAnchor();
        }
        //Otherwise give error
        else
        {
            ARDebugManager.Instance.LogError("No other elements to remove", true);
        }
    }
    
    // FUNCTION CALLED FROM THE CLASS "ARCloudAnchorManager" TO KNOW THE NUMBER OF OBJECT IN THE SCENE
    public int NumberOfObjects() 
    {
        return _placedGameObjectList.Count;
    }

    // FUNCTION CALLED FROM THE CLASS "ARCloudAnchorManager" TO KNOW THE IMAGE-NUMBER FOR THE NEXT IMAGE SAVE
    public void UpdateRetrievedPhotoCounter(int retrievedPhotoCounter)
    {
        _retrievedPhotoCounter = retrievedPhotoCounter;
    }

    // FUNCTION CALLED FROM THE CLASS "ARCloudAnchorManager" TO OVERWRITE THE IMAGE OF THE REMOVED ELEMENTS
    public void DecreasePhotoCounter()
    {
        _retrievedPhotoCounter -= 1;
    }

    // FUNCTION CALLED FROM THE CLASS "ARCloudAnchorManager" TO UPDATE THE MAPPING BETWEEN THE LocalAnchor AND THE CloudAnchorID
    public void UpdateDictionaryValueUsingAnchor(IntPtr anchorPtr, string cloudAnchorId = null)
    {
        var keys = loclaAnchorPtrCloudAnchorIdMapping.Keys;
        foreach (var key in keys)
        {
            if (key == anchorPtr)
            {
                loclaAnchorPtrCloudAnchorIdMapping[key] = cloudAnchorId;
                return;
            }
        }
        
    }
}
