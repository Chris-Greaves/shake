# shake

shake is a tool to help various parts of your PC stay awake. This is especially useful for Dashboards and Dev machines where you don't necessarily want the speakers or screen to go to sleep.

> **:warning: Notice: this tool is only available on Windows due to the libraries used.**

### Origin of the name:
I chose this name for two reasons, firstly it kinda sounds like awake, and secondly you shake people to wake them up, as well as shaking the mouse to wake your PC up.

## Features (Done / ToDo)

- [x] CLI tool
    - [x] Ability to keep audio devices awake by playing Inaudible beeps
        - [x] Ability to control the delay between anti-sleep beeps
        - [x] Ability to specify the audio device to keep awake
        - [x] Extra test audio file to ensure that the correct sound device is being kept awake.
        - [x] Ability to run multiple 'shakes' from a config (Used for 'on startup')
    - [x] Ability to keep the screen awake
		- [x] Ability to set a timer for keeping the screen awake
    - [ ] Ability to move the mouse after a period of time idle (*)
- [ ] User-friendly UI to setup 'shakes'
    - [ ] Update / Setup 'on startup' config and task

(*) Still debating whether to do this or not, currently the only reason I can think of using this is tricking Teams into thinking you are there. I'd prefer a safe and reasonable work environment over deception.

## Install

WIP - I am looking to add an installer to this project that will make installations much easier. 

## Third Party Libraries Used

- [NAudio](https://github.com/naudio/NAudio) - Audio and MIDI library for .NET