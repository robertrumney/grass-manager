# Unity Grass Manager

Unity Grass Manager is a user-friendly Editor Window tool developed for Unity3D, designed to help manage, edit & paint grass details on terrains throughout your scene with ease.

## Features

- **Bulk Management**: Easily manage grass details across all terrains in the scene.
- **Detail Customization**: Customize grass detail settings including minimum/maximum width and height, noise spread, and colors.
- **Texture Management**: Manage up to four grass textures, including adding them to terrains and applying them as detail layers.
- **Optimization**: Optimize terrain settings and clean terrains by removing or clearing all grass layers.
- **Mass Placement**: Cover all terrains in the scene with grass from top to bottom in a single click.

## How to Install

1. Download or clone this repository.
2. Move the `GrassManagerWindow.cs` file into your Unity project's `Editor` folder (create one if it doesn't exist).
3. Open Unity and navigate to `Window` -> `Grass Manager` in the top menu to open the tool.

## Usage

### Grass Settings

- Define your grass properties including:
  - **Min/Max Width**: Set minimum and maximum width of the grass.
  - **Min/Max Height**: Set minimum and maximum height of the grass.
  - **Noise Spread**: Set the noise spread to create variance in grass placement.

### Grass Textures

- Manage up to 4 grass textures.
- Assign the grass textures to be used across terrains.
- Automatically distribute grass details evenly across a terrain.

### Terrain Operations

- **Optimize Terrain Settings**: Set consistent detail resolution across all terrains.
- **Remove All Detail Layers**: Clear all grass detail prototypes from terrains.
- **Clear All Placed Grass**: Clear all placed grass details across all terrains.
- **Distribute Selected Grass**: Apply chosen grass details on all terrains according to defined settings and selected texture.

## Contributing

Contributions, bug reports, and feature requests are welcome! 
