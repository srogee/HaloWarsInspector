# HaloWarsInspector

Simple Halo Wars Definitive Edition map/model viewer built on top of my [HaloWarsTools](https://github.com/srogee/HaloWarsTools) project (which is currently private). Very barebones at the moment (no camera rotation, models are rotated wrong, etc).

![Preview](https://raw.githubusercontent.com/srogee/HaloWarsInspector/main/preview.png)

## How To Use
* Clone [HaloWarsTools](https://github.com/srogee/HaloWarsTools) and follow its setup instructions
* Clone this repository into the same folder, like so:
```
Some Folder
    HaloWarsDocs
    HaloWarsTools
    HaloWarsInspector
```
* Download the [Visual Studio Image Library](https://www.microsoft.com/en-us/download/details.aspx?id=35825)
* Edit this line to point to your image library:
```
private const string imageLibraryPath = "C:\\Users\\rid3r\\Documents\\GitHub\\VS2019 Image Library\\vswin2019";
```
* Build and run the solution

## To-do List
* Add simple lighting
* Allow users to toggle visibility of different meshes/submeshes
* Center terrain mesh in map viewer
* Rotate meshes to match expected +X forward +Z up
* Automatically set camera position based on scene bounds
* Add orbit and fly camera controls
* Add texture support (blocked by HaloWarsTools)
* Add material/shader support (blocked by HaloWarsTools)
* Discard GL vertex buffers/shaders/textures when they're no longer needed
