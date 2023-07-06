# RandomThings
# Features


# Installation
* You currently need a UnityModManager (version >= 0.25.5) with a custom profile for this game
* Download the latest release zip and install it with one of the following methods:
  * (Prefered) Start UnityModManager.exe again. Make sure you still have the game selected. Switch to the Mods tab and drag the zip into the "Drop zip files here" field.
  * Open the game folder. Enter the mods directory and unzip the mods archive there.

# Building
* Clone the repository. 
* Open the solution with Visual Studio.
* Make sure the Nuggets are installed as expected. This should automatically happen.
* To ensure portability I use a system variable $(MagicalMixturePath) for references and the build script. If you want to build the project yourself you either:
  * Add the variable yourself. 
    * Go to Properties > Environment Variables (or just search for variables and pick the option that appears)
    * Under User Variables click new, with the variable name being *SuperFantasyPath* 
      and the value being your path to the game directory 
      e.g. *D:\Games\Steam\steamapps\common\Super Fantasy Kingdom Demo*.
  * **Or** replace every reference to the variable in the .csproj file with the path. I don't really recommend doing that.
* Now you should be able to build the project without problems. If you still encounter trouble please contact me on Discord or create a GitHub issue.

# v0.1.0
* This is a supposed to be a Proof of Concept.
* The idea was to use UMM to create a simple Mod. 

# List of somewhat interesting classes to look at when decompiling:
* CityManager
* DaytimeManager
* AchievementManager 
* GameManager
* AlertManager
* CombaatManager
* BoostManager - Buffs; special events etc.
* CombatEventManager
* DamageNumberManager - Damage Numbers
* DebugManager - Maybe Debug Window?
* DialogueManager
* FogManager - FogOfWar
* GameEventManager - Monster Kills
* GridManager
* HeroSelectionManager - Responsible For Loading saves and picking a hero in main menu
* InvasionManager
* ItemManager
* JobManager - Register Worker and Jobs
* MainManager - load main save; main menu; settings; stuff
* RaceManager & RaceEventManage & RaceDataManager - probably for more races in the future
* Resource Manager - Starting Resources; PowerUps; etc.
* SaveManager - This is where magic happens; Creating a serializable GameData from a running game or creating a running game from a save data
* StatisticsManager - exactly that; stats
* StatusManager - status effects etc.
* TavernFoodManager 
* TavernSaveManager - Despite the name this actually handles everything related to day change (exp, glory, ...)
* UnitManager
* WeatherManager - change weather
* WorldManager - Glory and Guests
* BossSpawner - spawn bosses
There are more but those are the more or less interesting ones; just searching for Manager and filtering class will show all of them.