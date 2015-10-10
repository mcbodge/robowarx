# Description #
The headless interface is designed to provide a lightweight way to run robowar battles and get results. The primary focus of this interface is to support automated usage. Periodically, while running, and on battle completion it displays the state of the battle in the following format.
```
   <newline>
   <newline>
   Chronon: <chronon number>
   Name,Number,Energy,Damage,X,Y,team,Alive,Killer,DeathReason<newline>
   ----------------------------------------<newline>
   <robot1 values, comma seperated matching the header above><newline>
   <robot2 values, comma seperated matching the header above><newline>
   ....
```
Note that the end of a battle is indicated by 40 hash (#) marks on a line.

# Arguments #
The application should be run as follows:
```
RoboWarCL.exe [-cpu number] [-cl number] [-i] robotFile1 [robotFile2 robotFile3...]
```
  * **-cpu number** Sets the number of chronon's per battle update. If this is a positive value a snapshot of the current state of the battle will be written to the console every **number** chronons. If this is zero or negative, there will only be a single update at the end of the battle. The default value is 20.
  * **-cl number** Sets the chronon limit for the battle.  If this is a positive value, the battle is halted once the chronon specified by **number** has been reached. If this is zero or negative, the battle will run until completion (or indefinitely if a bot has an infinite loop). The default value is 0.
  * **-i** Sets the application to run in interactive mode as described below.

# Modes of Operation #
The headless interface operates in two main modes, standard and interactive, in standard mode, the files specified in the command line arguments are loaded, parsed, compiled and then run, after the run, the application terminates.

Interactive mode runs the application in more of a 'service' role which remains open and runs multiple battles. Interactive mode is designed to provide optimal performance for running a large number of battles bypassing process and run-time initialization, as well as file IO costs.
## Interactive Mode Protocol ##
Interactive mode uses standard in and standard out to communicate with the robowar engine.
On startup, any robots specified in the command line will be in the arena for the _first_ battle only.

In interactive mode the application will wait for 1 or more robots to be piped into standard in using the following format (note: currently interactive mode only supports plain-text robots).
```
    <robot name><newline>
    <plain-text robot code><newline>
    <EOT (ctrl+d, or ascii code 0x04)><newline>
```
You may repeat the above format for as many robots as you are adding. Finally, send a second **EOT** character to the stream to trigger an arena run.

You may close the application at any time by sending SIGINT (ctrl+c) or ETX (ascii code 0x03).