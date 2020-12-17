The MVC model is maintained due to good use of event handlers and events created in the controller to be fired on actions from the view and server.

The game instantiates images at the beginning of creation of the Drawing Panel to be referenced later which significantly improves preformance.

The game sends commands to the server once per frame in order to not overwhelm the server with useless commands.

The game uses private dictionaries to store data in the world which then gets combined and sent to the view in order to draw the world.

The data sent to the view rewrites itself whenever it's redundant and clears out after the info is sent in order to minimize the amount of redrawing that needs to occur.

The game sorts associates up to 8 different colored tanks, projectiles, and turrets with a simple while loop.

Server runs a main method and closes after player writes to it, thus saving the game.

Server control receives inputs and outputs to a client using JSON, usings a sendData method based on FPS to send on Frame actions to client.

We load default values for certain game necessities in case settings file does not contain these. 

Our Load will load a ton of settings for the server to use and it will actually load more than what is in our current settings.xml file.

Database controller is able to get all tank names and ID's and upload them to a DB that can be called upon using http listening.