# <p align="center">Collect and display a dataset in augmented reality scenario</p>

# About the project

**The project aims to the collection and visualization of a dataset in an augmented reality scenario**.<br/>It has been developed by Giovanni Ambrosi and Kevin Depedri for the Computer Vision course a.y. 2021-2022 Master in Artificial Intelligent Systems at the University of Trento. **The app is intended for Android devices only**.  

# Table of contents

* [SLAM Algorithm](#Slam-algorithm)

* [Getting started](#Getting-started)

* [Description](#Description)

* [Documentation and useful links](#Documentation-and-useful-links)

* [Demo](#Demo)

* [Future implementations](#Future-implementations)

* [Contacts](#Contacts)

# SLAM Algorithm

## Introduction

SLAM (**Simultaneous Localization and Mapping**) is an algorithm born in the 80s.
Its goal is to obtain a global and consistent estimate of a device’s path while reconstructing a map of the surrounding environment. The coupling between these two tasks, initially considered as the core issue, was soon discovered to be the real strength of SLAM methods.<br/> 
This duality has also encouraged its diversification. By dosing the importance given to mapping or to localization, SLAM has been pushed away from the sole robotics field and became a reference to solve problems of many different natures: from **micro aerial vehicles** to **augmented reality (AR) on a smartphone**.<br/>
The general system of the SLAM algorithm is made up of 4 parts:
   - **Sensor data**: on mobile devices, this usually includes the camera, accelerometer and gyroscope. It might be augmented by other sensors like GPS (indicated for outdoor applications), light sensor, depth sensors, etc;
   - **Front-End**: composed of two steps. The first step is **feature extraction**. These features also need to be associated with landmarks, keypoints with a 3D                         position, also called map points. These map points need to be tracked in a video stream. This phase ends up with the **loop                                             closure** step, meaning that the device reduces drift by recognizing places that have been encountered before;
   - **Back-End**: establishes the relationship between different frames, localizing the camera, as well as handling the overall                                                          geometrical reconstruction. This phase can be performed by sparse reconstruction (based on the keypoints) or capturing a dense 3D point cloud of                        the environment;
   - **SLAM estimate**: the result containing the tracked features, their locations and relations, as well as the camera position within the world.<br/><br/>
  
 ![image_0](https://it.mathworks.com/discovery/slam/_jcr_content/mainParsys3/discoverysubsection/mainParsys3/image.adapt.full.medium.png/1640333917572.png)<br/<br/>

## SLAM in Augmented Reality
 
In an Augmented Reality scenario the device has to know its 3D position in the world. It calculates this through the **spatial relationship between itself and multiple keypoints**.<br/>
The useful information to correctly localize itself in a place is acquired through the device camera, perhaps we can call the algorithm **visual SLAM**. SLAM instantiates a cartesian coordinate system with the origin in the initial camera's position and then combines the data from the accelerometer and the gyroscope to estimate the movement of the device. Using these data the algorithm is then able to:
   - **Build a map of the environment**;
   - **Localize the device itself within that environment**;

Since the common devices are equipped with monocular cameras we talk about **visual MonoSLAM**. The four challenges to solve for the best reconstruction of the environment and localization in Augmented Reality using SLAM are:
   - **Unknown space**;
   - **Uncontrolled camera**;
   - **Real-time**;
   - **Drift-free**.

As we know though, the real world is affected by errors due to noise in images and in sensors. For that reason the mapping cannot be performed by the SLAM alone but it has to be improved through some algorithms that are reliable with partial information and uncertainty. These algorithms are **Extended Kalman Filter, Maximum a Posteriori (MAP) estimation or Bundle Adjustment (BA)**.<br/>

Now we know what a SLAM algorithm uses for AR applications in terms of hardware and software. We describe now the most important parts of the entire procedure  

## (Good) Feature Points

The first step is to find distinctive locations in images, such as corners and use them as **feature points.** This points will be used later to build and retrieve the information about the environment. <br/>
While using an AR application many conditions can change, i.e.:
   - **camera angle and perspective**;
   - **rotation**;
   - **scale**;
   - **lighting**
   - **blur from motion or focusing**;
   - **general image noise**

For that reason a feature point alone is not enough to elaborate sufficiently the environment and neighbours of the point are taken into account in order to reinforce the mapping procedure.
 
*Step 1: Good feature points detection*:<br/><br/>

![image_1](https://www.andreasjakl.com/wp-content/uploads/2018/08/arcore-anchors.gif) <br/><br/>
 
 
## Feature points extraction

Finding distinctive feature points in images has been an active research field for quite some time. The most influential algorithms are called **“SIFT” (“Scale Invariant Feature Transform”)** and **“SURF” (Speeded up robust features”)** and both are still in use today. However, both algorithms are patented and usually too slow for real-time use on mobile devices and therefore SLAM algorithms use an ad-hoc tracking algorithm called **ORB** (.<br/>
The process to extract good keypoints is divided in two phases:
   - **keypoint detection**: could be performed by a **corners-detection algorithm**. It is important that the algorithm is scale-invariant and less dependent on noise. SIFT and SURF are good choices for that but due to complexity reasons the most used algorithm is **BRISK**. 
   - **keypoint description**: each of all the detected keypoints have to be unique and most important the algorithm must find the same feature again in the image under different circumstances (i.e. light change). Briefly speaking it **has to be robust**. BRISK is still the best algorithm to perform keypoint description as it is robust to light and perspective. 


*Step 2: Feature points extraction: as we can see the same points are recognized from two different perspectives*.<br/><br/><br/>
![image_2](https://i.stack.imgur.com/hw4UX.jpg)<br/><br/>

 
 ## Converting Keypoints to 3D Landmarks
 
Once keypoints are selected they have to be converted from 2D coordinates acquired from the camera to 3D system of the real world (called “map points” or “landmarks”).<br/>
In order to do that these keypoints are initially matched between two frames. Using gyroscope and accelerometer data the SLAM can compute the position of same keypoints in different frames and this helps with the **real-time requirement**. The matching results in an initial camera pose estimation.<br/>
Next, SLAM tries to improve the estimated camera pose using successive frames. Once the algorithm has acquired a new frame it **projects its map into the new camera frame to search for more keypoint correspondences**. If it is certain enough (verified with a threshold) that the keypoints match, it uses the additional data to refine the camera pose.<br/>
New map points are created by triangulating matching keypoints from connected frames. The triangulation is based on the 2D position of the keypoint in the frames, as well as the translation and rotation between the frames as a whole. 

## Loop Detection and Loop Closing

The last step in a SLAM algorithm is **loop detection and loop closing**: SLAM checks if keypoints in a frame match with previously detected keypoints from a different location. If the similarity exceeds a threshold, the algorithm knows that the user **returned to a known place**; but inaccuracies on the way might have introduced an offset. Using a correction strategy and propagating the error across the whole graph from the current location to the previous place (a sort of backpropagation procedure), the map is updated step-by-step with the new knowledge.<br/>

## Future of SLAM

We have had a look to the principles of the SLAM algorithm in Augmented Reality. Its application is spreading and improving and most of the developers are focused on combining new types of sensors to make SLAM more efficient i.e. HoloLens. Other ideas are thought to improve stability and speed (i.e. ORB-SLAM2 and OKVIS) or to use semantic meaning into SLAM algorithm. 
  

# Getting started
## Setup of the Unity Environment

1) Install Unity editor 2021.3 or later versions;

2) Download and import the project linked with this repo. Once imported an error could appear in the console (***Unable to resolve reference 'UnityEditor.iOS.Extensions.Xcode'***). Do not care about it, it will not affect the build of the application);

3) Unzip the folder arcore-unity-extensions-master.zip in the project folder;

**All the following points should be already correctly configured in the downloaded repo. If problems arises check them**:

4) Install all the following required packages from `Window -> Package Manager -> Search`:
   - **AR Foundation**;
   - **ARCore XR Plugin**;

5) Install the extension package from `Window -> Package Manager -> Add package from disk `:
   - The file to install is named 'package.json' in the unzipped folder arcore-unity-extensions-master;

6) Enable ARCore from `Edit -> Project Settings -> XR Plug-in Management` checking the box `ARCore`

7) Connect to the ARCore Cloud Anchor API from  `Edit -> Project Settings -> XR Plug-in Management -> ARCore Extensions` by selecting the option `API Key` from the drop-down menu `Android Authentication Strategy` and pasting the following key `AIzaSyAfzBVfUoXcPlFHWhEW8Xl5K8NBVBe5RBI`;

8) Always from `Edit -> Project Settings -> XR Plug-in Management -> ARCore Extensions` enable the cloud anchors checking the box `Optional Features -> Cloud Anchors`

9) If the previous `API Key` does not work please [follow this procedure](https://cloud.google.com/docs/authentication/api-keys?ref_topic=6262490&visit_id=637954035540901895-2222129602&rd=1) to generate your own API Key;

## Build of the application

*  Choose the level of debug you prefer from `Hierarchy -> AR Session -> AR DebugManager`, here check the box `Enable Debug` to turn on the debug in its wholeness. Check also the box `Only Critical Messages` if interested in removing the most specific messages and in keeping only the most important ones;

* Click on `File -> Build Settings` and from the open window switch platform clicking on `Android` and then `Switch Platform`;

* Connect your mobile phone to the computer;

* Choose your device from the `Run Device` section (click on `Refresh` if you do not see your device in the dropdown menu);

* Click `Build and Run`;



# Description




## Structure of the script
The application is based in 2 main classes: ARPlacementManager and ARCloudAnchorManager, together with 3 minor classes ARDebugManager, SaveManager and AnchorEntity. 

### ARPlacementManager
The ARPlacementManager class handles all the procedure with regards to the screenshot object present in the environment, such as:
* Acquisition and placement of new screenshot objects (which initially are just a local objects and are not on the cloud yet). The new objects are instantiated with the current position and rotation of the acquisition device exploiting a cartesian coordinates system (x,y,z) and a quaternion;
* Removal of one or more placed screenshot objects from the environment (useful to remove objects of which we are not satisfied before that these are hosted on the cloud);
* Recreation and placement of old screenshot objects (which are retrieved from the cloud after the hosting procedure);

### ARCloudAnchorManager
The ARCloudAnchorManager class handles all the procedure with regards to hosting and resolution of the screenshot object on the cloud, such as:
* First hosting request, it happens when the hosting procedure is performed for the first time in a session (without retrieving the previous session), it effectively creates a new session overwriting the previous one;
* Successive host, it happens all the times that a hosting procedure is performed after a previous hosting procedure, or after retrieving the prevous session. It allows to add elements to the current session on the cloud;
* First resolution, it appens when the resolution procedure is performed for the first time in a session (without hosting any element), it effectively retrieves the previous session, allowing to visualize it and to update it adding new screenshot objects;
* Successive resolution, it happens all the times that a resolution procedure is performed after a previous hosting procedure. It allows to retrieve all the screenshot objects that are currently missing in the scene. The objects are retrieved from the cloud;

### ARDebugManager, SaveManager and AnchorEntity
The ARDebugManager class allows us to get feedbacks from the application, while the SaveManager class together with the AnchrEntity class handles the save of the current session locally in a .Json file, allowing it to be retrieved in the next sessions or to be overwritten with a new one.


## Detailed working of the application
### Start of the application
As the application starts, the user is required to scan the environment for at least 15/30 seconds. This allow the SLAM algorithm to acquire some data from the current environment, and to build a feature map that will be used to position the anchors of the placed screenshoot objects. A longer scan allows to get a higher quality feature map, and so higher quality anchors which will be quicker to resolve and also more precise. A shorter scan leads to lower quality feature map which in the worst case could lead to anchors that are not retrievable.
It is important to remember that environment with extreme light conditions(brightness or darkness) and with important reflections will be very difficult to map, also when performing a scan procedure for 30 seconds or more. In this case the placed object could be positioned in imprecise positions and the anchors could be really difficult to retrieve.

### Moving the first steps
After performing the scan of the environmente the user is able to place as many screenshot objects as he wants. To place an object it is sufficient to perform `One-Tap` on the area of the screen where the acquisition of the camera is visible. When an object is placed the user need to move slightly backward to be able to see the performed acquisition, which will be positioned with the correct inclination and also with the correct rotation (vertical or horizontal).

### Dealing with a multiple list system
The application is based on a multiple list system which allows to have different list for local objects and cloud object. In this way the user is able to acquire the wanted screenshot locally, then to evaluate them and eventually to remove the not good ones pressing the `Remove Last Obj button` to then acquire them again. Finally, the users is able to host them when he is satisfied with the result pressing the `Host Anchors button`.
As soon as the `Host Anchors button` is pressed then the anchors are gradually hosted on the cloud and the removed from the list of the anchors which were waiting for the hosting, in this way each single object won't be hosted more than once.

### Different possibile combinations of actions
This system of multiple list allows to handle any possible combinations of the `Host/Remove/Resolve procedures`, meaning that the user will be able to: start a session, take some screenshot, remove the bad ones and host the good ones, then add other elements to the previously hosted ones and so on. There is no cambination of these actions that is not supported by the script.

### Start a new session or retrieve a previous one
After that the acquisition has been performed, the anchors have been hosted and the application has been closed, when we are starting it again we can perform two different choices:
* If we want to `Start a fresh new session` then we need to start the application, acquire one or more screenshot, and only when we press the `Host Anchors button` then we will effectively overwrite the previous session with the current one.
* If we want to `Retrieve the previous session` then we need to start the application and press the `Resolve Anchors button`, in this way the previously acquired elements will be restored and from now moving on it will be possible to add new elements to that pevious session.


## Summarized phase of the application
<br/>The usage of the application is summarize in the following bullet list:

1) The first phase consists in scanning around the environment for at least 15/30 seconds;

2) After a tap on the screen the application takes a pic of the environment and creates an hologram with a local anchor associated;

3) The hologram is visible through the device camera as a plane with the picture as texture. It is placed in the ARcamera's coordinates with the same inclination and orientation;

4) After having placed the holograms the user has three choices:
   - **Host anchor**: upload the anchor points on the cloud;
   - **Remove last object**: delete the last hologram created;
   - **Resolve anchor**: retrieve the anchors uploaded previously on the cloud and place the holograms in the correct positions.


5) After closing and re-starting the application:
   - **Start a fresh new session**: place a screenshot object and host it to start a new session and overwrite the previous one;
   - **Retrieve the previous session**: resolve the previously hosted anchors to retrieve the previous session and update it.

<br/>**NOTE**:<br/> All the pictures are saved in a predefined path in the device (android/data/com.WreckerCompany.ARHolograms/files/SavedImage/). The images are named as "Image-nr" where nr is an integer index (the path is reacheable from the File Manager folder in the home screen). Along with the pictures there is in the same folder a txt file where the informations about the position and rotation of the camera are written. These infos are saved with the following format: <br/><br/>
**Image-nr<br/><br/>
coordinate x: position.x<br/> coordinate y: position.y<br/> coordinate z: position.z<br/>
quaternion rotation: (a,b,c,d)<br/><br/>**
where position.x, position.y, position.z are the spatial coordinates of the camera and a,b,c,d are the coordinates to represent the rotation of the camera.



# Documentation and useful links
* [Unity Documentation](https://docs.unity3d.com/Manual/index.html)

* [Unity Fundamentals](https://www.andreasjakl.com/ar-foundation-fundamentals-with-unity-part-1/)

* [Anchors and Raycasting](https://www.andreasjakl.com/ar-foundation-fundamentals-with-unity-part-3/)

* [SLAM Algorithm](https://www.andreasjakl.com/basics-of-ar-slam-simultaneous-localization-and-mapping/)



# Demo

For a demonstration of the app download the videos folder in this repo. In this section we provide a brief description of each video.<br/>
In the folder you find four videos that summirize the main aspects of the application:
   - **ScanfOfTheArea.mp4**: in this video the user scans the area around the device. The scan has to last from 15 to 30 seconds;
   - **PlacementOfObjects.mp4**: the holograms are instantiated. They are placed according to the position of the camera and its rotation;
   - **AnchorHosting.mp4**: once the holograms are taken they are hosted on a cloud. The debugger shows the quality of the features in the environment and tells the user when an hologram is hosted. At the end a message "END OF HOSTING PROCEDURE" is shown;
   - **AnchorSolving.mp4**: the device recognize the environment and its current position and is able to correctly retrieve the holograms created.

# Future implementations

- [ ] Add support for API to host also images and Json file on the cloud allowing for a full information retrieval from other devices;

- [ ] Add real-time information about the quality of the generated feature map to encurage users to explore more the environment;

- [ ] Add the option to save the pictures in a path choosen by the user and to show the pictures in the image gallery;

- [ ] Add the possibility to remove a precise object aiming at it instead of removing the last one placed;

- [ ] Renderization of the back of the hologram to visualize holograms from every view angle;

- [ ] Extension of the anchors survival time up to 365 days;

- [ ] Training of neural networks (NeRF);



# Contacts
Giovanni Ambrosi - (giovanni.ambrosi@studenti.unitn.it)

Kevin Depedri - (kevin.depedri@studenti.unitn.it)
