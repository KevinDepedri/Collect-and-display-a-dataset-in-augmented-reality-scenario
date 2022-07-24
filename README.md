# <p align="center">Collect and display a dataset in augmented reality scenario</p>

# About the project

**The project aims to the collection and visualization of a dataset in an augmented reality scenario**.<br/>It has been developed by Giovanni Ambrosi and Kevin Depedri for the Computer Vision course a.y. 2021-2022 Master in Artificial Intelligent Systems at the University of Trento. **The app is intended for Android devices only**.  


# Getting started
## Setup of the Unity Environment

* Install Unity editor 2021.3 or later versions;

* Install all the required packages from `Window -> Package Manager` and install AR Foundation and ARCore XR Plugin; 

## Build of the application

* Download this repo and open the "Sample Scene" file or clone this repo from Unity;

* Click on `File -> Build Settings` and from the open window switch platform clicking on `Android` and then `Switch Platform`;

* Connect your mobile phone to the computer;

* Choose your device from the `Run Device` section;

* Click `Build and Run`;




# Description
At the start the ARSession instantiates a cartesian coordinates system (x,y,z) with the origin in the position of the device's camera. These values, along with the inclination of the ARcamera, will be used for the istantiation and retrieve of the anchors. <br/>The usage of the application is summarize in the following bullet list:

1) The first phase consists in scanning around the environment for at least 15 seconds;

2) After a tap on the screen the application takes a pic of the environment and creates an hologram with an anchor associated;

3) The hologram is visible through the device camera as a plane with the picture as texture. It is placed in the ARcamera's coordinates with the same inclination;

4) After having placed the holograms the user has three choices:
   - **Host anchor**: upload the anchor points on a cloud;
   - **Remove last object**: delete the last hologram created;
   - **Resolve anchor**: retrieve the anchors uploaded and place the holograms in the correct positions.

<br/>**NOTE**: all the pictures are saved in a predefined path in the device (android/data/com.WreckerCompany.appname/files/SavedImage/Image-x.png)


# Documentation and useful links
* [Unity Documentation](https://docs.unity3d.com/Manual/index.html)

* [Unity Fundamentals](https://www.andreasjakl.com/ar-foundation-fundamentals-with-unity-part-1/)

* [Anchors and Raycasting](https://www.andreasjakl.com/ar-foundation-fundamentals-with-unity-part-3/)



# Demo (GIFs or pictures)

![this is an image](https://github.com/GiovanniAmbrosi/Collect-and-display-a-datasets-in-augmented-reality-scenario/blob/main/Il mio progetto-1 (1).png?raw=true)

# Future implementations

- [ ] Add the option to save the pictures in a path choosen by the user;

- [ ] Add the possibility to remove a predefined object, not only the last one;

- [ ] Renderization of the back of the hologram;

- [ ] Extension of the anchors survival time;

- [ ] Training of neural networks (NeRF);


# Contacts
Giovanni Ambrosi - (giovanni.ambrosi@studenti.unitn.it) -

Kevin Depedri - (kevin.depedri@studenti.unitn.it) -
