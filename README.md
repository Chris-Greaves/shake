# shake

shake is a tool to help various parts of your PC stay awake. This is especially useful for Dashboards and Dev machines where you don't necessarily want the speakers or screen to go to sleep.

> **:warning: Notice: this tool is only available on Windows due to the libraries used**

### Origin of the name:
I chose this name for two reasons, firstly it kinda sounds like awake, and secondly you shake people to wake them up, as well as shaking the mouse to wake your PC up.

## Features (Done / ToDo)

- [x] CLI tool
    - [x] Ability to keep audio devices awake by playing inaudiable beeps
        - [x] Ability to control the delay between anti-sleep beeps
        - [x] Ability to specify the audio device to keep awake
        - [x] Extra test audio file to ensure that the correct sound device is being kept awake.
        - [ ] Ability to run multiple 'shakes' from a config (Used for 'on startup')
    - [ ] Ability to keep the screen awake
    - [ ] Ability to move the mouse after a period of time idle 
- [ ] User-friendly UI to setup 'shakes'
    - [ ] Update / Setup 'on startup' config and task

## Install

WIP - I am looking to add an installer to this project that will make installations much easier. 

## Third Party Libraries Used

- [NAudio](https://github.com/naudio/NAudio) - Audio and MIDI library for .NET