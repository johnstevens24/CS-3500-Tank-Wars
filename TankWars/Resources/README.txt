@ Authors Alyssa Johnson and John Stevens CS 3500 FA21

Our TankWars solution consists of five projects: World, View, GameController, Resources, and Vector2D.

* World
The world contains seven classes: world, wall, tank, beam, projectile, powerup, and tankdeath.
The world class contains a separate dictionary for tanks, walls, beams, projectiles, powerups, and dead tanks.
This keeps track of all objects in the game and allows us to iterate through them when we need to handle events such as drawing them.
We placed tank, beam, projectile, powerup, and tankdeath into the same class library since we felt that beams and such 
related to tanks. We added a dictionary for tanks that have died so that we could keep track of and iterate through 
them when it came time to draw an animation for their deaths.


* View
This project represents the View in terms of MVC and has a program, form, and drawingpanel component.

- In the Form, we add a drawing panel and event listeners for a series of events.
We have a listener for when an update has arrived, which contains all the things that need to be done every time 
a frame arrives such as redrawing the form and sending commands to the server.
We have a listener for errors that is used for throwing errors that came from the Controller/Model.
We have listeners for when a player's position changes that updates the players x and y-coordinates so that the 
drawing panel can be notified and adjust the image accordingly.
We have a listener for when the client connects to the server which allows us to change settings such as no longer 
allowing the connect button to be pressed.
We have a listener for when buttons on the mouse are clicked so that we can know when a projectile is supposed to be fired.
We have a listener for mouse movement so that we know where to draw the turret aiming.
We have listeners for when movement keys (w, a, s ,d) are pressed and released so that we can tell the server 
that a player would like to move.

- The Program is where we call the form to be run.

- In the DrawingPanel, we have the code that details how the game and its objects should be drawn.
At first we were loading images on every frame and this was making the game lag. 
We fixed this by creating a method that would load all of the images once when the game was first initialized with a drawing panel.
Since the images were now only loaded once, it became much faster to use these existing images.
For images having to do with a tank's, turret's, projectile's, or beam's color, we created separate image arrays 
that held all 8 variations of color.
To select a player's color, we decided to use a tank's ID, which is an integer, and modulos it by 8 in order to 
randomly assign a color to the player. The modulos by 8 allows us to select which of 8 index positions in our image array to use.
To draw walls, we are given the x and y positions of the starting and ending wall pieces.
Only the x positions or the y positions should be different between a starting and ending wall.
We implemented four while loops that measure the distance between x or y coordinates.
We subtract two x or y coordinates and find the distance between them. We draw the first wall and then add 50 pixels 
(since walls are 50 apart) to the starting x or y position each iteration until the x or y positions of the start and ending block are nearly the same.
We have a tolerance of 0.000001 because comparing doubles for equality doesn't always work due to floating point precision.
Having this tolerance prevents an infinite loop when the two x-values are nearly the same.
The starting and ending block aren't always drawn in these loops and are drawn at the end of this series of while loops.
For when a tank dies, we drew 8 images to show a small animation of an explosion occurring. 
We made a class for dead tanks that each have an int framecounter variable associated with them. This is how long an animation should last.
We have a DeathAnimation drawer that reads the framecounter of the tank that died.
When the animation is being drawn, it subtracts 1 from a dead tank's framecounter for every frame that has passed.
Depending on the value of the frame counter, we have if/else if statements that determine which explosion image should be displayed.
This allows us to animate our explosion images in the correct order.
For the beam, we decided to draw a line from the center of the tank up until the full length of the world so that a tank's 
beam will always span the full world size.
We added a framecounter similar to the one we had on our dead tanks in order to animate the beam.
We outlined the beam with the player's tank color and then made it get smaller over the course of 30 frames.


* GameController
This project represents the Controller in terms of MVC.
We have a few error checks that will invoke the View to show a messagebox displaying our error.
We throw errors when something has gone wrong, when it takes more than 3 seconds when a client is trying to connect, 
when a player's name is not between 1 and 16 characters, and when the ip address field is empty.
When receiving a JSON, if the message does not end with a newline character, that means its message is incomplete.
This was previously breaking our game since we were trying to read incomplete messages.
To fix this, we save a message when it does not end in a newline character. We then add this to the beginning of the 
next incoming message because this would contain the other parts of the incomplete message.
We process the messages and deserialize them into their proper objects and add them to their respective dictionaries.
When an object such as a tank or projectile has died or disconnected, we remove them from their dictionaries.
When sending instructions for movement, we have a stringbuilder that has they layout of the format for how a message 
to the server needs to be ordered. We are able to choose the first index of our movement array as the move value and
we put moving as none when the value is null. We also get the player's mouse firing direction and send that.


* Resources
In here we have our README file and also the files for all of the images used in TankWars.

* Vector2D
This class provided by our instructor represents a 2D vector in space and allows us to keep track of and use
players' x and y-coordinates and angles. This greatly aids in where objects should be drawn.



========================= Server Read Me =========================

* World
- Our world has two counter variables that increment in order to assign projectiles and beams with unique ID's.
This prevents tanks from killing themself. We have an Update method that handles updating tanks,
projectiles, powerups, and beams. We added a powerup counter variable to tanks so that we can keep track of 
when a beam is allowed to fire. 
- If the world is not surrounded by walls, the tank wraps around to the opposite side after its center 
point passes the world size boundary. Even though part of the tank might hang outside the world's boundary, 
we liked this implementation because we don't feel a tank should be teleported for just barely touching the edge 
of a boundary. To implement this, if the X or Y value of a tank matches a boundary of the world, we change that
X or Y value to be negative and then add or subtract 3 from it. If we did not subtract a number from this new 
location, the tank would infinitely be stuck on the world's borders. We also made sure to check that the tank would 
only wraparound if there wasn't a wall on the other side. If the left boundary of the world had no walls and the 
right boundary of the world did have walls, we wanted to prevent a wraparound since the tank would then hit the right 
boundary walls. 
- Every time a tank dies, whether by projectile, beam, or disconnect, we also make the hp = 0 or else 
the tank still gets drawn even though we say the tank has died. 
- We only allow a projectiles position to be updated if its new location does not collide with a wall or tank. 
Depending on the distance, this may make it appear to not hit a wall or tank only because the projectiles newest 
position hits the tank or wall. We didn't want to draw the new position on top of tanks or walls because then it 
might appear as if the projectile went through the wall. 
- We felt that removing dead objects was a concern of the world rather than the server and have a method that goes 
through our tanks, beams, projectiles, and powerups and removes them from their respective dictionaries if they have 
died. If a tank has died, we instead set its RespawnCounter to the number of frames that must pass before it can 
respawn rather than removing it from the tank dictionary. We don't want to remove it from the tank dictionary 
because we want to be able to maintain the tank's score and how many powerups it holds.
- When spawning tanks or powerups, we prevent either from spawning on top of existing walls, tanks, or powerups.
We think it would be unfair for a tank to spawn on top of a powerup.

* ServerController
The server controller basically acts as an in between between the clients and the world. It intakes controlCommands from the clients
and sends the world to be processed. We used the same method that Ella the TA used for implementing updates every 1/60 of a second,
so just a stopwatch and a while loop. Some things to note: If we recieve multiple control commands from a client during the same frame,
we prioritize the one that contains a firing command. When tanks die we have a countdown that begins at 300 and goes down by one for each
frame. The timing of when died messages being sent out, hp being reset, and respawn locations being selected are all part of this countdown.
We have a second constructor that intakes a filepath but other than that is the same. This is just one of those things that we thought might 
be nice to have if this games developement were being taken further than what we've done. Also we decided to give our tanks and our clients 
the same ID numbers to make everything easier. 

* ControllerCommand
Bascially just allows us to make objects out of the json messages.

* Settings
Reads in an xml file and creates an object that contains all the characteristics of the xml file. We also decided to give the settings object
a reference to the same world object that the serverController is using that way it can add walls to the world as it reads them in. We 
decided to make the ReadXML method return a bool (false if an error occured, true if everything went ok) so that the serverController knows that
something happened instead of just an error. If something does happens and the serverController gets a false boolean returned, it will output a
message describing the error, output a message saying that its shutting down, and stop the rest of the server from being loaded.

* Wall
- We added variables for wall thickness, borders, and how much to expand from the centerpoint in order to detect 
collisions. We added two methods, one that detects when a tank collides with a wall and one that detects when a 
projectile hits a wall. We used two separate methods so that we could expand the wall thickness by different amounts 
based on the size of tanks and projectiles.

* Tank
- We added a counter for how many frames have passed since the tank last shot so that projectiles can't rapid
fire. We added a respawn counter that counts down from the number of frames until a tank is allowed to respawn. 
We also added a powerup counter to keep track of how many powerups a tank holds so we can determine if a tank 
is allowed to fire a beam or not. We added a method for determining when a projectile collides with a tank.

* Beam
- We added Daniel Kopta's provided beam intersection code.

* Projectile
- To create a projectile, we decided to pass in a tank since a projectile's location, direction, and owner all 
all rely on its firing tank.

* Powerup
- We added a method that checks for if a powerup collides with another powerup. We wanted to prevent powerups 
from spawning on top of each other.
