# IslandConquer
Unity 3D Multiplayer FPS Game with networked AI bots

<h2>Inspiration:</h2> 
After creating a handful of 2D games, I decided to challenge myself to build a 3D game from scratch. I wanted to have full control of my game; hence, I designed the map, physics, character models, animations, AI logic, and networking from scratch. I tried to stay away from using Unity prefabs/packages since I wanted full control of my game. 
I was quickly able to learn that the math was much more complex. Rendering, physics, and collisions involved an extra dimension now. I enjoyed applying the linear algebra knowledge I obtained while taking course at University to my game. 
I was also quickly able to learn the importance of optimization. While making 2D games, I was able to program without taking into considering the performance of my game (in most cases). In 3D, however, without considering optimizing methods, performance can be greatly impacted. Since, my game is networked, this could cause even more problems. 

<h2>Without AI bots:</h2>
My first implementation of IslandConquor didn’t have AI bots. It was simply a multiplayer game where one player would be the host, and the rest of the players would simply be clients. When a player joins the game session, an reference to the player with a unique id is stored in a dictionary that contains all players currently in the game. Then a function is run on the server which then calls functions on all clients currently in the game to display the new player. Shooting, damage, and movement is communicated from the server to all clients in a similar fashion. 

<h2>With AI bots:</h2>
Upon completing the multiplayer version of the game, I wanted to see if I would be able to build bots that would be able interact, attack, and move freely around the map. I was curious to see if I could instantiate, and communicate movements, damage, and respawning the same way I did with real-players. I started by defining a radius around each AI bot, so that if a player enters that space they will be attacked. In terms of movement, I generated three random numbers, one number that defines how long the bot will move, and another number that defines how long the bot will move. In addition, I made the bots responsive to being attacked, so when a player decides to attack a bot, they will respond by attacking the player. Finally, I made sure that all these factors were well synced over the network so that each player playing the game sees the same bots and is experiencing the same game. 

All in all, this was an excellent opportunity to explore 3-dimensional game development and apricate the math, planning, and design involved. 

Built with: C#, Unity3d, 3ds Maxx.
