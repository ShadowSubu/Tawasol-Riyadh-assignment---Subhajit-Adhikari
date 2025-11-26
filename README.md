# Tawasol-Riyadh-assignment---Subhajit-Adhikari

## Game Concept
**Sync Dash** is a hyper-casual endless runner where:
- The **right-side player** is controlled by the user  
- The **left-side ghost** mirrors all actions with simulated network delay  
- The objective is to **survive as long as possible** while avoiding obstacles and collecting orbs  

## Core Mechanics
- Automatic forward movement with increasing speed  
- Tap / Space to **jump**  
- **Obstacle collision** ends the run  
- **Glowing orbs** increase score  
- A ghost player copies all actions using a **state synchronization system**  
- Synced events: movement, jump, collision, and orb collection  
- Smooth interpolation prevents jitter in ghost movement  
- Difficulty increases as speed ramps up  