[Modules](https://github.com/TailwindMechanics/HexSim/tree/main/Assets/Modules)

# HexSim
- Created by [Seamus Dunne]([url](https://www.linkedin.com/in/s-dunne/))

## Unity Version (plus/pro *not* required):
   2023.2.3f1

## Dependencies:
   1. [UniRx]([url](https://assetstore.unity.com/packages/tools/integration/unirx-reactive-extensions-for-unity-17276)) (free)
   2. [Zenject]([url](https://assetstore.unity.com/packages/tools/utilities/extenject-dependency-injection-ioc-157735)) (free)

## Summary:
HexSim is a simple hexagon tile tick-based combat game where you control a flock of croissants by clicking and directing them en masse, while the enemy Beer Kegs attempt to kill and wipe out the Croissants. You can hit enter/return/esc/r to restart the scene. It's rough around the edges and there is a bug you can exploit to gain a strategic advantage. 
- It uses the Blind Modules pattern for project layout and architecture. 
- It mocks a client/server relationship. 
- I have created a new hexagon coordinate system, Neese. 
- The hexagon tiles are procedural.
- The tile map is procedural, and the map structure uses Perlin noise to define the terrain height and colour.
- It contains an AStarNav modules for a* pathfinding.

## Relevant LinkedIn articles:
- [Blind Modules](https://www.linkedin.com/pulse/blind-modules-pattern-3-rules-sustainable-unity-seamus-dunne-rlpwe)
- [Neese](https://www.linkedin.com/pulse/hexagon-grids-made-easy-neese-seamus-dunne-iupfe)

## License:
This project is licensed under the MIT License. For more details, see the [LICENSE](https://github.com/TailwindMechanics/HexSim/blob/main/LICENSE) file in the repository. Feel free to clone, modify, and use it in your own projects, commercial or otherwise, with no restrictions.
