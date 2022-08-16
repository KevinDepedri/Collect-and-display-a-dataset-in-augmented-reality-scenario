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
This duality has also encouraged its diversification. By dosing the importance given to mapping or to localization, SLAM has been pushed away from the sole robotics field and became a reference to solve problems of many different natures: from **micro aerial vehicles** to **augmented reality (AR) on a smartphone**.
The system is made up of 4 parts:
   - **Sensor data**: on mobile devices, this usually includes the camera, accelerometer and gyroscope. It might be augmented by other sensors like GPS, light sensor,                       depth sensors, etc;
   - **Front-End**: the first step is feature extraction, as described in part 1. These features also need to be associated with landmarks – keypoints with a 3D                           position, also called map points. In addition, map points need to be tracked in a video stream. Long-term association reduces drift by recognizing                     places that have been encountered before (loop closure);
   - **Back-End**: takes care of establishing the relationship between different frames, localizing the camera (pose model), as well as handling the overall geometrical                  reconstruction. Some algorithms create a sparse reconstruction (based on the keypoints). Others try to capture a dense 3D point cloud of the                            environment.
   - **SLAM estimate**: the result containing the tracked features, their locations & relations, as well as the camera position within the world.


## Good Feature Points

But how can SLAM perform these actions. The idea behind the algorithm is to find distinctive locations in images, such as corners or blobs and use them as **feature points.** This points will be used later to retrieve the information about the environment. <br/>
While using an AR application many conditions can change, i.e:
   - camera angle / perspective;
   - rotation;
   - scale;
   - lightning;
   - blur from motion or focusing;
   - general image noise.
 For that reason a feature point alone is not enough to elaborate sufficiently the envoronment and that is why neighbours of the point are taken into account in order to reinforce the mapping of the envornment.
 
## Feature points extraction

Finding distinctive feature points in images has been an active research field for quite some time. One of the most influential algorithms is called “SIFT” (“Scale Invariant Feature Transform”). It was developed by David G. Lowe and published in 2004. Another “traditional” method is called “SURF” (Speeded up robust features”) by H. Bay et al. Both are still in use today. However, both algorithms are patented and usually too slow for real-time use on mobile devices.<br/>
The process to extract good keypoints is summarize in two phases:
   - keypoint detection
   - keypoint description
 
 These are the base for tracking & recognizing the environment.
 
 ## SLAM for Augmented Reality
 
For Augmented Reality, the device has to know more: its 3D position in the world. It calculates this through the spatial relationship between itself and multiple keypoints. This process is called “Simultaneous Localization and Mapping”, SLAM for short.<br/>
As said before most of the information is acquired through the device camera. It combines the data from the accelerometer and the gyroscope and from other minor sensors allowing the device to:
   - Build a map of the environment;
   - Locate itself within that environment;
 
Since the real world is affected by errors due to noise in images and in sensors the mapping has to be improved through some algorithms that are reliable with partial information and uncertainty. These algorithms are Extended Kalman Filter, Maximum a Posteriori (MAP) estimation or Bundle Adjustment (BA).<br/>
Since the common devices are equipped with monocular cameras it is correct to talk about MonoSLAM. The four challenges to solve for the best reconstruction of the environment in AR using SLAM are:
   - Unknown space;
   - Uncontrolled camera;
   - Real-time;
   - Drift-free.
 
 
 

 


## 

# Getting started
## Setup of the Unity Environment

1) Install Unity editor 2021.3 or later versions;

2) Download and import the project linked with this repo

All the following points should be already correctly configured in the downloaded repo. If problems arises check them:

3) Install all the following required packages from `Window -> Package Manager -> Search`:
   - **AR Foundation**;
   - **ARCore XR Plugin**;

4) Unzip the following folder:
   - - **ARCore Extensions ([download here](https://github.com/GiovanniAmbrosi/Collect-and-display-a-datasets-in-augmented-reality-scenario/raw/main/arcore_extension_package%20-%20extract_and_install_from_disk.zip)**);

5) Install the extension package from `Window -> Package Manager -> Add package from disk `:
   - The file to install is named 'package.json' in the unzipped folder arcore-unity-extensions-master;

6) Enable ARCore from `Edit -> Project Settings -> XR Plug-in Management` checking the box `ARCore`

7) Connect to the ARCore Cloud Anchor API from  `Edit -> Project Settings -> XR Plug-in Management -> ARCore Extensions` by selecting the option `API Key` from the drop-down menu `Android Authentication Strategy` and pasting the following key `AIzaSyAfzBVfUoXcPlFHWhEW8Xl5K8NBVBe5RBI`;

8) Always from `Edit -> Project Settings -> XR Plug-in Management -> ARCore Extensions` enable the cloud anchors checking the box `Optional Features -> Cloud Anchors`

9) If the previous `API Key` does not work please [follow this procedure](https://cloud.google.com/docs/authentication/api-keys?ref_topic=6262490&visit_id=637954035540901895-2222129602&rd=1) to generate your own API Key;



## Build of the application

* Download this repo and open the "Sample Scene" file;

* Click on `File -> Build Settings` and from the open window switch platform clicking on `Android` and then `Switch Platform`;

* Connect your mobile phone to the computer;

* Choose your device from the `Run Device` section;

* Choose the level of debug you prefer from `Hierarchy -> AR Session -> AR DebugManager`, here check the box `Enable Debug` to turn on the debug in its wholeness, check also the box `Only Critical Messages` if interested in removing the most specific messages and in keeping only the most important ones;

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
It is important to remember that environment with extreme light conditions(brightness or darkness) and with important reflections will be very difficult to map, also when performing a scanning procedure for 30 seconds or more. In this case the placed object could be positioned in imprecise positions and the anchors could be really difficult to retrieve.

### Moving the first steps
After performing the scan of the environmente the user is able to place as many screenshot objects as he wants. To place an object it is sufficient to perform `One-Tap` on the area of the screen where the acquisition of the camera is visible. When an object is placed the user need to move slightly backward to be able to see the performed acquisition, which will be positioned with the correct inclination and also with the correct rotation (vertical or horizontal).

### Dealing with a multiple list system
The application is based on a multiple list system which allows to have different list for local objects and cloud object. In this way the user is able to acquire the wanted screenshot locally, then to evaluate them and eventually to remove the not good ones pressing the `Remove Last Obj button` to then acquire them again. Fianlly the users is able to host them when he is satisfied with the result pressing the `Host Anchors button`.
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

<br/>**NOTE**:<br/> all the pictures are saved in a predefined path in the device (android/data/com.WreckerCompany.ARHolograms/files/SavedImage/). The images are named as follow Image-nr where nr is an integer index. Along with the pictures there is in the same folder a txt file where the informations about the position and rotation of the camera are written. These infos are saved with the following format: <br/><br/>
**Image-nr<br/><br/>
coordinate x: position.x<br/> coordinate y: position.y<br/> coordinate z: position.z<br/>
quaternion rotation: (a,b,c,d)<br/><br/>**
where position.x, position.y, position.z are the spatial coordinates of the camera and a,b,c,d are the coordinates to represent the rotation of the camera.



# Documentation and useful links
* [Unity Documentation](https://docs.unity3d.com/Manual/index.html)

* [Unity Fundamentals](https://www.andreasjakl.com/ar-foundation-fundamentals-with-unity-part-1/)

* [Anchors and Raycasting](https://www.andreasjakl.com/ar-foundation-fundamentals-with-unity-part-3/)

* [SLAM Algorithm](https://gisresources.com/what-is-slam-algorithm-and-why-slam-matters/)

* [vSLAM and viSLAM methods](https://www.hindawi.com/journals/js/2021/2054828/)



# Demo

For a demonstration of the app download the videos folder in this repo

##  Placement of objects

# Future implementations

- [ ] Add support for API to host also images and Json file on the cloud allowing for a full information retrieval from other devices;

- [ ] Add real-time information about the quaity of the generated feature map to encurage users to explore more the environment;

- [ ] Add the option to save the pictures in a path choosen by the user and to show the pictures in the image gallery;

- [ ] Add the possibility to remove a precise object aiming at it instead of removing the last one placed;

- [ ] Renderization of the back of the hologram to visualize holograms from every view angle;

- [ ] Extension of the anchors survival time up to 365 days;

- [ ] Training of neural networks (NeRF);



# Contacts
Giovanni Ambrosi - (giovanni.ambrosi@studenti.unitn.it)

Kevin Depedri - (kevin.depedri@studenti.unitn.it)
