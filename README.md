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
Its goal is to obtain a global and consistent estimate of a device’s path while reconstructing a map of the surrounding environment. 
The coupling between these two tasks, initially considered as the core issue, was soon discovered to be the real strength of SLAM methods.<br/> 
This duality has also encouraged its diversification. By dosing the importance given to mapping or to localization, SLAM has been pushed away from the sole robotics field and became a reference to solve problems of many different natures: from **micro aerial vehicles** to **augmented reality (AR) on a smartphone**.
**SLAM uses devices/sensors to collects visible data (camera) and/or non-visible data (RADAR, SONAR, LiDAR) with basic positional data collected using Inertial Measurement Unit (IMU)**.<br/>
Together these sensors collect data and build a picture of the surrounding environment and along woth the SLAM algorithm estimate the location and position within the surrounding environment.
Moreover SLAM algorithms use two main designs. The first design corresponds to filter-based solutions (Extended Kalman filter or particle filters), the second design utilizes parallel methods derived from PTAM (Parallel Tracking and Mapping). 
A SLAM algorithm is composed of two phases: **front-end data collection** and **back-end data processing**

### Front-end data collection

The front-end data collection of SLAM is of two types: Visual SLAM and LiDAR SLAM.
**Visual SLAM (vSLAM) uses simple cameras (360 degree panoramic, wide angle and fish-eye camera), compound eye cameras (stereo and multi cameras), and RGB-D cameras (depth and Time-of-Flight cameras) to collect data**.
Cameras provide a large volume of information, they can be used i.e. to detect landmarks which can be combined with graph-based optimization, achieving flexibility in SLAM implementation.<br/>
LiDAR SLAM implementation uses a laser sensor. Compared to Visual SLAM, lasers are more precise and accurate. 
The output data of LiDAR sensors often called as point cloud data is available with 2D (x, y) or 3D (x, y, z) positional information and the movement is estimated sequentially by matching the point clouds. The calculated movement (travelled distance) is used for localizing i.e a vehicle. For LiDAR point cloud matching, iterative closest point (ICP) and normal distributions transform (NDT) algorithms are used.

### Back-end data processing

Visual SLAM algorithms can be broadly classified into two categories, **sparse methods** and **dense methods**<br/>
Sparse methods match feature points of images and use algorithms such as PTAM and ORB-SLAM while dense methods use the overall brightness of images and use algorithms such as DTAM, LSD-SLAM, DSO, and SVO.

LiDAR point cloud matching generally requires high processing power, so it is necessary to optimize the processes to improve speed. Due to these challenges, localization for autonomous vehicles may involve fusing other measurement results such as wheel odometry, global navigation satellite system (GNSS), and IMU data. For applications such as warehouse robots, 2D LiDAR SLAM is commonly used, whereas SLAM using 3-D LiDAR point clouds can be used for UAVs and automated parking.<br/>

### Mathematical theory
Mathematically speaking given:<br/>
<br/>![equation](https://wikimedia.org/api/rest_v1/media/math/render/svg/15cd7daffb06c3fa7898e10fe16953895cb3a369) -> map of the environment;<br/>
<br/>![equation](https://wikimedia.org/api/rest_v1/media/math/render/svg/f279a30bc8eabc788f3fe81c9cfb674e72e858db) -> agent's current state;<br/>
<br/>![equation](https://wikimedia.org/api/rest_v1/media/math/render/svg/3cd2cf4bfdabc8ae396ce3fa32aeb871efb3d732) -> sensor's observation;<br/>
<br/>![equation](https://wikimedia.org/api/rest_v1/media/math/render/svg/ffc45a5286dc4ff8b99c89d5fbf0c0b9760babf1) -> series of controls<br/>

the objective is to compute:<br/>

![equation](https://wikimedia.org/api/rest_v1/media/math/render/svg/c607fe964e04d2d7c46f8420596205eb67737000)<br/>
<br/>Using Bayes's rule and given a map and a transition function ![equation](https://wikimedia.org/api/rest_v1/media/math/render/svg/18bc6b8abe9221154012d7bf365049d4755902e8) we can compute:<br/>
<br/>![equation](https://wikimedia.org/api/rest_v1/media/math/render/svg/a9af46b0bc5e00ee32f838783ee48004379e32a0)<br/>
Similarly the map can be updated sequentially by:<br/>
<br/>![equation](https://wikimedia.org/api/rest_v1/media/math/render/svg/15a2717a2788d8cb12aaa07295d6278ddbf7044b)<br/>

In the following we will introduce the main principles of vSLAM and viSLAM.<br/>In this project we have focused mainly on visual-inertial SLAM (viSLAM) an evolution of visual SLAM (vSLAM) tecniques

## Classical Structure of vSLAM
Four main blocks describe the overall operation of any vSLAM algorithm. They are the following:
* ***Input search***: finding the required information in the sensor measurements
* ***Pose tracking***: determining the current camera pose from the new perceptions
* ***Mapping***: adding a landmark to the map
* ***Loop closing***: producing a proper map and drift-free localization<br/><br/><br/>

![image_1](https://static-02.hindawi.com/articles/js/volume-2021/2054828/figures/2054828.fig.002.svgz)<br/><br/><br/>

## From vSLAM to viSLAM
Visual-inertial simultaneous localization and mapping (VI-SLAM) that fuses camera and IMU
data for localization and environmental perception has become increasingly popular for several
reasons. First, the technology is used in robotics, especially in extensive research and applications
involving the autonomous navigation of micro aerial vehicles (MAV). Second, augmented reality
(AR) and virtual reality (VR) are growing rapidly. Third, unmanned technology and artificial
intelligence has expanded tremendously.
VI-SLAM is generally divided into two approaches: **filtering-based** and **optimization-based**.<br/> 

### Filtering-Based Methods
The filthering-based methods are divided into two categories: loosely coupled method and tightly coupled method.<br/> The loosely coupled
method usually only fuses the IMU to estimate the orientation and possible the change in
position, but not the full pose. In contrast, the tightly coupled method fuses the state of the
camera and IMU together into a motion and observation equation, and then performs state estimation.
Tightly coupled methods presently constitute the main research focus, thanks to advances in
computer technology.

### Optimization-Based Methods
With the development of computer technology, optimization-based VI-SLAM has proliferated
rapidly. Optimization-based methods divide the entire SLAM frame into a front-end and back-end
according to image processing; the front-end is responsible for map construction, whereas the
back-end is responsible for pose optimization. Back-end optimization techniques are usually
implemented on g2o, ceres-solver, and gtsam. Many excellent datasets can be used to
study visual-inertial methods, such as EuRoC, Canoe, Zurich urban MAV, TUM VI
Benchmark, and PennCOSYVIO. Examples of Optimization-Based Methods are Loop Closure, Okvis, VIORB (based on ORB-SLAM)

# Getting started
## Setup of the Unity Environment

1) Install Unity editor 2021.3 or later versions;

2) Download and import the project linked with this repo

All the following points should be already correctly configured in the downloaded repo. If problems arises check them:

3) Install all the following required packages from `Window -> Package Manager -> Search`:
   - **AR Foundation**;
   - **ARCore XR Plugin**;

4) Install all the following extension package from `Window -> Package Manager -> Add package from disk `:
   - **ARCore Extensions ([download here](https://github.com/GiovanniAmbrosi/Collect-and-display-a-datasets-in-augmented-reality-scenario/raw/main/arcore_extension_package%20-%20extract_and_install_from_disk.zip)**);
   - ** unzip the folder. The extension to install is 'package.json'; 

5) Enable ARCore from `Edit -> Project Settings -> XR Plug-in Management` checking the box `ARCore`

6) Connect to the ARCore Cloud Anchor API from  `Edit -> Project Settings -> XR Plug-in Management -> ARCore Extensions` by selecting the option `API Key` from the drop-down menu `Android Authentication Strategy` and pasting the following key `AIzaSyAfzBVfUoXcPlFHWhEW8Xl5K8NBVBe5RBI`;

7) Always from `Edit -> Project Settings -> XR Plug-in Management -> ARCore Extensions` enable the cloud anchors checking the box `Optional Features -> Cloud Anchors`

8) If the previous `API Key` does not work please [follow this procedure](https://cloud.google.com/docs/authentication/api-keys?ref_topic=6262490&visit_id=637954035540901895-2222129602&rd=1) to generate your own API Key;



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

<br/>**NOTE**: all the pictures are saved in a predefined path in the device (android/data/com.WreckerCompany.ARHolograms/files/SavedImage/). The images are named as follow Image-nr where nr is an integer index. Along with the pictures there is in the same folder a txt file where the informations about the position and rotation of the camera (and the pictures) are written. These infos are saved as Image-nr-coord_x-coord_y-coord_z-(rotation Quaternion) where coord_x, coord_y, coord_z and rotation Quaternion are respectuvely the coordinates and rotation of the camera and nr is an integer index.



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
