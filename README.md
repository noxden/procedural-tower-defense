# Pumpkin Peril
> _Project by students of the Darmstadt University of Applied Sciences, developed for „Procedural Level Generation“ elective in SuSe2023._
## Outline
In ***Pumpkin Peril*** you defend your pumpkin garden from hordes of monsters using strategically placed turrets on procedurally generated terrain, where the height of the environment is a key factor to your success.
## Gameplay
The surrounding landscape is procedurally generated and different every time you play. Choose a location for your fortress and choose smart spots to place down your turrets. Each turret works best in specific conditions, like the right height or environment. Once you’re set up, fend off waves of spawning enemies, collect their currency and build new turrets for the next wave. Eventually your garden will be overrun, giving you the opportunity to start anew with a different map and strategy.
## Technologies
This project utilizes the Wave Function Collapse and Random Walk algorithms to generate the map procedurally, ensuring that every round plays out differently from the one that came before it.
Even the animations of the enemies are procedurally generated and can thereby be modified at runtime.
## Controls
- Use **[WASD]** to move the camera
- Use the **[Mouse Wheel]** to zoom in and out
- Use **[Left Click]** to interact with the UI buttons and sliders.
## How to play
### Generating the map
After starting up the game, you will be greeted by the main menu. Configure your grid size as well as desired start and end position of the tower defense path using the sliders and input fields on the left. Once you are satisfied, press the **[Generate Map]** button on the right to generate a map based on your settings. If you don't like it, just click the button again to regenerate. Or if you decide you don't like your initial settings anymore, just change them and click the **[Generate Map]** button again! Repeat this process for however long you want or until you are satisfied with the result. Then you're ready to jump into the tower defense part of the game, which you do by pressing the now enabled **[Start Game]** button on the right.
### Playing the tower defense
First, let's take a look at the UI: In the top-left corner, your can see the tower shop, containing all currently available towers. Each button refers to one tower and displays its cost in its bottom right corner. When hovering over one of those buttons, you can read up on the towers properties in the displayed tooltip. Each of the different towers can only be placed on tiles of certain heights, as indicated on the tooltip.

On the right side of the screen, you can see your pumpkin patch's health and your current money, the latter of which you will spend to buy your towers. Below those to, you will find the **[Start Wave]** button, which you will need to press to start the first wave of enemies to spawn at the grave tile on the map. After spawning, enemies immediately start walking along the path to your pumpkin patch, which will be destroyed if too many enemies get through to it. During the game, you will have the opportunity to start waves early to receive some bonus money, but be careful to not overwhelm your towers with too many enemies!

On the top right, you can find some additional buttons, going from left to right, they are: 
- The **[Pause Game]** button, which... pauses the game, so that you can think about where to place towers without worrying about the monster hordes progressing too far in the mean time.
- The **[Speed Setting]** toggle, which toggles through four different game speeds, so you can adjust the pace of the game to your liking.
- The **[Exit Button]**, which brings you back to the main menu. But beware! You will lose all the progress you made when you return to the main menu.

#### Check out the [Gameplay Video](https://youtu.be/r5JQU-acrh0) here!
