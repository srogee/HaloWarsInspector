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
* Build and run the solution

## To-do List
* Read/import resources on separate thread so process isn't locked up
* Compile shaders on separate thread
* Allow users to toggle visibility of different meshes/submeshes
* Center terrain mesh in map viewer
* Rotate meshes to match expected +X forward +Z up
* Automatically set camera position based on scene bounds
* Add orbit and fly camera controls
* Add texture support (blocked by HaloWarsTools)
* Add material/shader support (blocked by HaloWarsTools)
* Discard GL vertex buffers/shaders/textures when they're no longer needed
