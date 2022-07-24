# <p align="center">Collect and display a dataset in augmented reality scenario</p>

# About the project

This project has been developed by Giovanni Ambrosi and Kevin Depedri for the Computer Vision course a.y. 2021-2022 (Master in Artificial Intelligent Systems) at the University of Trento and consists in the collection of a dataset in augmented reality.
The app has been developed using Unity Editor and then built and tested on Android devices.  


# Getting started

* Install Unity editor 2021.3 or later versions;
* Install all the required packages from<br/> Window -> Package Manager and install AR Foundation and ARCore XR Plugin; 
* Download this repo, unzip the package and open the "Sample Scene" file or clone this repo and import from Unity;
* Click on File -> Build Settings and from the window opened switch platform clicking on Android and then Switch Platform;
* Connect your mobile phone to the computer;
* Choose your device from the Run Device section;
* Click Build and Run;

# Description
The first phase consists in scanning around the environment for at least 15 seconds. After that the app is ready to take the pictures.
<br/>The user has to tap on the screen and the device will capture the view field of the camera. <br/>The picture is then placed on a virtual plane (hologram), along with an anchor (for more informations https://www.andreasjakl.com/raycast-anchor-placing-ar-foundation-holograms-part-3/) which is instantiated in the environment.
Once the pictures are taken the user can choose between three options: 
* Host anchor: upload the anchor points on a cloud;
* Remove last object: delete the last hologram created;
* Resolve anchor: retrieve the anchors uploaded and place the holograms in the correct positions.

**NOTE**: all the pictures are saved in a predefined path in the device (android/data/com.WreckerFactory.appname/files/SavedImage/Image-x.png)


# Demo (GIFs or pictures)

# Future implementations and usages
* Add the option to save the pictures in a path choosen by the user;
* Add the possibility to remove a predefined object, not only the last one;
* Renderization of the back of the hologram;
* Collections of more datasets;
* Training of neural networks (NeRF);



# Licence

# Contact
Ditta Demolizioni SPA
