# Parallax System
![parallaxgif2](https://user-images.githubusercontent.com/42221923/144687400-8057f8bc-d1fd-43f0-9823-d5a82f555771.gif)

Parallax system gives some life to the background by moving different background layers at different speeds.

## How it works
Background images are assigned in the Raw Image components. Raw Image is used because it's an element that's a part of Unity UI system that allows to modify the UV of the image.

UV is moved according to the set multiplier and order of the background. On scene launch, the background is cloned and offset in order to make the background wider. This allows to move the camera to the right during one of the boss sequences.
