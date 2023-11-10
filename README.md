[Modules/Code of Interest](https://github.com/TailwindMechanics/HexSim/tree/446ac68c9af73ab1e5ac42758f669bf8f0310ef9/Assets/_Modules)

# HexSim
- Created by Seamus Dunne

## Unity Version:
   2022.3.8f1

## Dependencies:
   1. UniRx
   2. Zenject
   3. Odin Inspector

## Imported Modules
### EditorTooling:
   1. My usual toolbox of shortcuts
### Utils:
   1. A couple extension methods used by EditorTooling


## New Modules
### MouseInput:
   1. Capture mouse input

### CameraController:
   1. Use input to pan and zoom camera

### Spawner:
   1. Simple service for spawning
   2. Bonus: Pool spawnable actors

### Gameplay
   1. Mainly just data types for describing a game

### TickServer
   1. Defines start/end criteria
   2. Provides API for server 
   3. Observable for receiving ticks 
   4. Subject for notifying state change 
   5. Provides spawn points for players red/blue
   6. Init the noise map here 
      - Perlin with seed is deterministic, no need to serialize heights
      - Pathfinding and ai decided here
      - Complete game loop ai vs ai first
      - Server is source of truth for game state
      - Everything else reads and does view stuff
      - Later allow user to send click command to server

### HexGrid:
   1. Contains data Neese structure
   2. And suite of utility extension methods

### HexTile:
   1. Spawns tiles 
   2. SpawnPoint tiles get height of 1 
   3. Outer rim tiles get height of 0 
   4. Loop from the height 1 tiles and tiles an elevation of the average of their neighbours

### Actors:
   1. Contains health/damage info
   2. Actor movement interpolation
   3. Actor AI
      1. Simple A* that updates per tick
      2. Spawn X number of red and blue balls
      3. Both colours attack one another on sight 
      4. When enemy is spotted
         - Blue balls flock for strength in numbers
         - Red balls beeline for the fastest kill 
      5. Else both wander, blues still flock though 
      6. Once within X tiles they attack one another 
      7. Teammate balls can occupy same cell, opponents cannot
