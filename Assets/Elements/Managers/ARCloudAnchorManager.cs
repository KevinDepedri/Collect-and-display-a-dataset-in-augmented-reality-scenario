using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;


public class UnityEventResolver : UnityEvent<Transform>{} // Event used as soon as we RESOLVE AN ANCHOR to pass the
// transform relative to that anchor to the ARPlacementManager in a way that it can instantiate the respective GameObject.

public class ARCloudAnchorManager : Singleton<ARCloudAnchorManager>
{
    [SerializeField] private Camera arCamera = null;
    [SerializeField] private float resolveAnchorPassedTimeout = 10.0f;
    private ARAnchorManager _arAnchorManager = null;
    private ARPlacementManager _arPlacementManager = null;

    private List<ARAnchor> _pendingCloudAnchorList = new List<ARAnchor>();
    private List<bool> _pendingVertical = new List<bool>();
    private int _pendingCounter = 0;

    private List<ARCloudAnchor> _cloudAnchorList = new List<ARCloudAnchor>();
    private List<string> _cloudAnchorIdList = new List<string>();
    private List<bool> _cloudAnchorVerticalList = new List<bool>();
    private List<bool> _anchorUploadInProgressList = new List<bool>();

    private List<bool> _anchorResolveInProgressList = new List<bool>();
    private List<ARCloudAnchor> _retrievedCloudAnchorList = new List<ARCloudAnchor>();
    
    private float _safeToResolvePassed = 0;
    private UnityEventResolver _resolver = null;
    private AnchorEntity _anchorEntity;
    
    private int _index = 0;
    private bool _startHostingProcedure = false;
    private bool _host = false;
    private bool _startResolvingProcedure = false;
    private bool _resolve = false;
    
    
    // EXECUTION BEFORE THE FIRST FRAME
    private void Awake() 
    {
        //Identify the needed components
        _resolver = new UnityEventResolver();   
        _resolver.AddListener((anchorTransf) => ARPlacementManager.Instance.ReCreatePlacement(anchorTransf, _index, _cloudAnchorVerticalList[_index], _cloudAnchorIdList[_index]));
        _anchorEntity = GetComponent<AnchorEntity>();
        _arAnchorManager = GetComponent<ARAnchorManager>();
        _arPlacementManager = GetComponent<ARPlacementManager>();
    }

    // FUNCTION USED TO EXTRACT THE POSE OF THE CAMERA
    private Pose GetCameraPose()
    {
        Transform arCameraTransform = arCamera.transform;
        return new Pose(arCameraTransform.position, arCameraTransform.rotation);
    }

    
    #region CallableFunctions
    // FUNCTION CALLED FROM THE CLASS "ARPlacementManager" WHEN AN OBJECT IS PLACED TO QUEUE IT FOR THE HOSTING PROCEDURE
    public void QueueAnchor(ARAnchor arAnchor, bool vertical)  
    {
        _pendingCloudAnchorList.Insert(_pendingCounter, arAnchor);
        _pendingVertical.Insert(_pendingCounter, vertical);
        ARDebugManager.Instance.LogInfo($"Added pending anchor {_pendingCounter}: {_pendingCloudAnchorList[_pendingCounter]}", false);
        ARDebugManager.Instance.LogInfo("Successfully added pending anchor", true);
        _pendingCounter += 1;
    }
    
    // FUNCTION CALLED FROM THE CLASS "ARPlacementManager" WHEN AN OBJECT IS REMOVED TO DEQUEUE IT FROM THE HOSTING PROCEDURE
    public void DeQueueAnchor()  
    {
        ARDebugManager.Instance.LogInfo($"Removed pending anchor {_pendingCounter-1}: {_pendingCloudAnchorList[_pendingCounter-1]}", false);
        ARDebugManager.Instance.LogInfo("Successfully removed pending anchor", true);
        
        //Remove the element from the pendingCloudAnchorList, and remove its vertical information
        _pendingCloudAnchorList.RemoveAt(_pendingCounter-1);
        _pendingVertical.RemoveAt(_pendingCounter-1);
        
        //Decrease the PhotoCounter index to be able to overwrite the photo. Finally update pendingCounter
        _arPlacementManager.DecreasePhotoCounter();
        _pendingCounter -= 1;
        
        //TODO: Add exception to avoid the request of DeQueue for elements which are already hosted. Currently Dequeue is not performed for these elements
        //TODO: since we ask for an element: {_pendingCloudAnchorList[_pendingCounter-1]} which does not exists, so the function crashes and avoids an error
    }
    
    // FUNCTION CALLED FROM THE BUTTON: "HOST" TO REQUIRE THE HOSTING OF THE PENDING ANCHORS
    public void HostingEnabled()  
    {
        //Enable the start of the hosting procedure
        _startHostingProcedure = true;
        ARDebugManager.Instance.LogInfo("STARTING HOSTING PROCEDURE", true);
    }
    
    // FUNCTION CALLED FROM THE BUTTON: "RESOLVE" TO REQUIRE THE RESOLUTION OF THE HOSTED ANCHORS
    public void ResolvingEnabled()  
    {
        //Enable the start of the resolving procedure
        _startResolvingProcedure = true;    
        ARDebugManager.Instance.LogInfo("STARTING RESOLVING PROCEDURE", true);

        //If it is possible load the information of the CloudAnchors from local file
        if (SaveManager.Instance.LoadFromFile("SaveData.dat", out string json))
        {
            _anchorEntity.LoadFromJson(json);
            for (int i = 0; i < _anchorEntity.anchorIdList.Count ; i++)
            {
                _cloudAnchorIdList.Insert(i, _anchorEntity.anchorIdList[i]);
                _cloudAnchorVerticalList.Insert(i, _anchorEntity.verticalList[i]);
            }
            _arPlacementManager.UpdateRetrievedPhotoCounter(_anchorEntity.photoCounter);
            ARDebugManager.Instance.LogSuccess($"Successfully retrieved {_anchorEntity.anchorIdList.Count} anchors", true);
        }
        //Otherwise give error
        else
        {
            ARDebugManager.Instance.LogError("No available anchors to retrieve", true);
        }
    }
    #endregion
    
    
    #region AnchorCycle
    // FUNCTION CALLED IN THE FIRST PHASE OF HOSTING FOR EACH ANCHOR
    private void HostAnchor(int anchorIndex)
    {
        //Host has been executed, put it to true(done) so that it will not be executed again for this specific anchor
        _host = true;
        //If there is at least one anchor in list start the hosting procedure
        if (_pendingCloudAnchorList.Count > 0)
        {
            ARDebugManager.Instance.LogInfo($"Hosting anchor N.{anchorIndex}", true);
            
            //Estimate the quality of the generated feature map and output it (insufficient means really low)
            FeatureMapQuality quality = _arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
            ARDebugManager.Instance.LogInfo($"Quality of the generated feature map: {quality}", true);
            
            //Add in the list of cloud anchors the new hosted anchors, returned from the anchor manager
            _cloudAnchorList.Insert(anchorIndex, _arAnchorManager.HostCloudAnchor(_pendingCloudAnchorList[anchorIndex], 1));
        
            //If the anchors is null it means we couldn't host it
            if(_cloudAnchorList[anchorIndex] == null)
            {
                ARDebugManager.Instance.LogError($"Unable to host cloud anchor N.{anchorIndex}", true);
                _anchorUploadInProgressList.Insert(anchorIndex, false);
            }
            //Otherwise it means that the upload is still in progress
            else
            {
                _anchorUploadInProgressList.Insert(anchorIndex, true);
            }
        }
        //If there are no anchor in list give error and stop
        else
        {
            ARDebugManager.Instance.LogError("No available anchors to host", true);
        }
    }

    // FUNCTION CALLED IN THE SECOND PHASE OF HOSTING FOR EACH ANCHOR
    private void CheckHostingProgress(int anchorIndex)
    {
        //Extract the current state of the cloud anchors in processing
        CloudAnchorState cloudAnchorState = _cloudAnchorList[anchorIndex].cloudAnchorState;
        
        //If it is "Success" then the hosting is terminated and we can complete its mapping and write it to file
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            //Insert the anchor in the cloudAnchorIdList
            _cloudAnchorIdList.Insert(anchorIndex, _cloudAnchorList[anchorIndex].cloudAnchorId);
            ARDebugManager.Instance.LogSuccess($"Anchor N.{anchorIndex} successfully hosted with id: {_cloudAnchorIdList[anchorIndex]}", false);
            ARDebugManager.Instance.LogInfo("Anchor successfully hosted", true);
            
            //Complete the mapping between local anchors and cloud anchor with the cloudAnchorId
            _arPlacementManager.UpdateDictionaryValueUsingAnchor(_pendingCloudAnchorList[anchorIndex].nativePtr, _cloudAnchorIdList[anchorIndex]);
            
            //Turn its value of UploadInProgress to false
            _anchorUploadInProgressList.Insert(anchorIndex, false);
            
            //Update values inside class AnchorEntity
            _anchorEntity.anchorIdList.Add(_cloudAnchorIdList[anchorIndex]);
            _anchorEntity.photoCounter = _anchorEntity.anchorIdList.Count;
            _anchorEntity.verticalList.Add(_pendingVertical[anchorIndex]);
            
            //Write anchor entity to a local file so that it can be retrieved when required
            if (SaveManager.Instance.WriteToFile("SaveData.dat", _anchorEntity.ToJson()))
            {
                ARDebugManager.Instance.LogSuccess("AnchorID saved successfully", false);
            }
            else
            {
                ARDebugManager.Instance.LogError("Error in saving AnchorID", true);
            }
            
        }
        //If it is not "TaskInProgress" then it means that something went wrong
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            //Give information about the error and then turn its value of UploadInProgress to false
            ARDebugManager.Instance.LogError($"Fail to host anchor N.{anchorIndex} with state: {cloudAnchorState}", true);
            _anchorUploadInProgressList.Insert(anchorIndex, false);
        }
    }

    // FUNCTION CALLED IN THE FIRST PHASE OF RESOLUTION FOR EACH ANCHOR
    private void ResolveAnchor(int anchorIndex)
    {
        //Resolve has been executed, put it to true(done) so that it will not be executed again for this specific anchor
        _resolve = true;
        ARDebugManager.Instance.LogInfo($"Retrieving cloud anchor N.{anchorIndex} with id: {_cloudAnchorIdList[anchorIndex]}", false);
        
        //For all the keys(local anchors) in the mapping dictionary, verify if a key (local anchor) has a value (cloud anchor id) that is
        //the same as the anchor that we want to resolve currently, if it is true that element is already present. No resolve needed 
        var keys = _arPlacementManager.loclaAnchorPtrCloudAnchorIdMapping.Keys;
        foreach (var key in keys)
        {
            if (_arPlacementManager.loclaAnchorPtrCloudAnchorIdMapping[key] == _cloudAnchorIdList[anchorIndex])
            {
                ARDebugManager.Instance.LogSuccess($"Cloud anchor N.{anchorIndex} with id: {_cloudAnchorIdList[anchorIndex]} already spawned", true);
                _retrievedCloudAnchorList.Insert(anchorIndex, null);
                _anchorResolveInProgressList.Insert(anchorIndex, false);
                return;
            }
        }
        
        //Otherwise ask the resolution fo rhe current cloud anchor Id
        _retrievedCloudAnchorList.Insert(anchorIndex, _arAnchorManager.ResolveCloudAnchorId(_cloudAnchorIdList[anchorIndex]));
        //If the anchors is null it means we couldn't resolve it
        if (_retrievedCloudAnchorList[anchorIndex] == null)
        {
            ARDebugManager.Instance.LogError($"Fail to resolve cloud anchor N.{anchorIndex} with id: {_cloudAnchorIdList[anchorIndex]}", true);
            _anchorResolveInProgressList.Insert(anchorIndex, false);
        }
        //Otherwise it means that the resolution is still in progress
        else
        {
            _anchorResolveInProgressList.Insert(anchorIndex, true);
        }
    }

    // FUNCTION CALLED IN THE SECOND PHASE OF RESOLUTION FOR EACH ANCHOR
    private void CheckResolveProgress(int anchorIndex)
    {
        //Extract the current state of the cloud anchors in processing
        CloudAnchorState cloudAnchorState = _retrievedCloudAnchorList[anchorIndex].cloudAnchorState;
        ARDebugManager.Instance.LogInfo($"Resolve CloudAnchor N.{anchorIndex} state: {cloudAnchorState}", false);
        ARDebugManager.Instance.LogInfo("Resolving CloudAnchor, look at the interested zone", true);

        //If it is "Success" then the resolution is terminated and we can place it in the environment
        if (cloudAnchorState == CloudAnchorState.Success)
        {
            _resolver.Invoke(_retrievedCloudAnchorList[anchorIndex].transform);
            ARDebugManager.Instance.LogSuccess($"CloudAnchor Id: {_retrievedCloudAnchorList[anchorIndex].cloudAnchorId} resolved", false);
            ARDebugManager.Instance.LogSuccess("CloudAnchor successfully resolved", true);
            _anchorResolveInProgressList.Insert(anchorIndex, false);
        }
        //If it is not "TaskInProgress" then it means that something went wrong
        else if (cloudAnchorState != CloudAnchorState.TaskInProgress)
        {
            ARDebugManager.Instance.LogError($"Fail to resolve Cloud Anchor N.{anchorIndex} with state: {cloudAnchorState}", false);
            ARDebugManager.Instance.LogError("Fail to resolve CloudAnchor", true);
            _anchorResolveInProgressList.Insert(anchorIndex, false);
        }
    }
    #endregion

    
    // EXECUTION AFTER THE FIRST FRAME
    void Update()
    {
        //If we are in the hosting procedure loop here
        if (_startHostingProcedure)
        {
            //Run the first phase of hosting just once
            if (!_host)
            {
                HostAnchor(_index);
            }
            
            //Loop on the second phase of hosting until it is completed
            if (_anchorUploadInProgressList[_index])
            {
                CheckHostingProgress(_index);
                return;
            }
            
            //If we have still anchors to process and the previous phases are completed then move to the next anchor in queue
            if (_index < _pendingCloudAnchorList.Count - 1 && _host && !_anchorUploadInProgressList[_index])
            {
                _index += 1;
                _host = false;
            }
            //Otherwise if the previous phases are completed but there are no more anchors to process we will communicate that
            //and then every list will be cleared for the next run of the hosting procedure
            else if (_index == _pendingCloudAnchorList.Count - 1 && _host && !_anchorUploadInProgressList[_index])
            {
                ARDebugManager.Instance.LogSuccess($"ALL NEW {_pendingCloudAnchorList.Count} ANCHORS SUCCESSFULLY HOSTED", true);
                ARDebugManager.Instance.LogSuccess($"TOTAL NUMBER OF ANCHORS HOSTED: {_anchorEntity.anchorIdList.Count}", true);
                ARDebugManager.Instance.LogInfo("END OF HOSTING PROCEDURE", true);
                _startHostingProcedure = false;
                _host = false;
                _index = 0;
                _anchorUploadInProgressList.Clear();
                _cloudAnchorIdList.Clear();
                _cloudAnchorList.Clear(); 
                _pendingCloudAnchorList.Clear();
                _pendingVertical.Clear();
                _pendingCounter = 0;
                
                //TODO: Add case in which if anchors are not uploaded then they are left in the list so that we can run it again.
                //TODO: Verify, it could happen that if we have an error in uploading 1 anchor then we have:
                //TODO: ALL NEW 1 ANCHORS SUCCESSFULLY HOSTED also if in reality 0 anchors have been hosted (index decrease error)
            }
        }
        
        
        //If we are in the resolution procedure loop here
        if (_startResolvingProcedure)
        {
            //Run the first phase of resolution just once
            if (!_resolve)
            {
                ResolveAnchor(_index);
            }

            //Loop on the second phase of resolution until it is completed giving updates based on a time counter
            if (_anchorResolveInProgressList[_index] && _safeToResolvePassed <= 0)
            {
                _safeToResolvePassed = resolveAnchorPassedTimeout;

                if (!string.IsNullOrEmpty(_cloudAnchorIdList[_index]))
                {
                    CheckResolveProgress(_index);
                }
            }
            //If the procedure is not completed update the time-counter
            else
            {
                _safeToResolvePassed -= Time.deltaTime * 1.0f;
            }

            //If we have still anchors to process and the previous phases are completed then move to the next anchor in queue
            if (_index < _cloudAnchorIdList.Count - 1 && _resolve && !_anchorResolveInProgressList[_index])
            {
                _index += 1;
                _resolve = false;
            }
            //Otherwise if the previous phases are completed but there are no more anchors to process we will communicate that
            //and then every list will be cleared for the next run of the resolution procedure
            else if (_index == _cloudAnchorIdList.Count - 1 && _resolve && !_anchorResolveInProgressList[_index])
            {
                ARDebugManager.Instance.LogSuccess($"ALL {_cloudAnchorIdList.Count} ANCHORS SUCCESSFULLY RETRIEVED", true);
                ARDebugManager.Instance.LogSuccess($"TOTAL NUMBER OF SPAWNED ELEMENTS: {_arPlacementManager.NumberOfObjects()}", true);
                ARDebugManager.Instance.LogInfo("END OF RESOLVING PROCEDURE", true);
                _startResolvingProcedure = false;
                _resolve = false;
                _index = 0;
                _anchorResolveInProgressList.Clear();
                _cloudAnchorIdList.Clear();
                _retrievedCloudAnchorList.Clear();
                _cloudAnchorList.Clear();
                //TODO: Add case in which if anchors are not downloaded then they are left in the list so that we can run it again.
                //TODO: In that case, in the moment in which we have an error add also txt"Error in anchors,.. the anchor will be skipped. when process ended try to re-lunch the
                //TODO: procedure to process again the anchors with error"
            }
        }
    }
}