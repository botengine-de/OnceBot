# OnceBot
This is a bot for the game Elvenar.

## Features

+ No configuration required: the bot uses image processing to find the game elements to interact with.
+ Collects and starts workshop production.
+ Collects coins from residences.
+ Independent of localization of the game.

## Using the bot

### Setting up the game
This bot uses image processing to find game elements to interact with.
To ensure the bot can read the game, the game must be scaled at 100% and the ingame camera must be zoomed out max.
Set the scaling level in windows display settings to 100%.

### Selecting the game window
Select the window the bot should operate on in the "pick window" tab.
To select the game window, check the checkbox "pick next focused window" and then click on the game window.
If the window has been successfully selected, its title will be shown under "last picked window".

### Starting the bot
To start the bot, press the "play" button at the top of the bot app.
<br>
To stop the bot, press CTRL+ALT or click the "pause" button.

## Build
Visual Studio 2015 is the recommended tool for building.
