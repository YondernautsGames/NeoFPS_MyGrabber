# NeoFPS_MyGrabber

An integration between NeoFPS and MyGrabber

## Requirements

This repository was created using Unity 2019.4 LTS.

It requires the assets [NeoFPS](https://assetstore.unity.com/packages/templates/systems/neofps-150179?aid=1011l58Ft) and [MyGrabber](https://assetstore.unity.com/packages/tools/physics/mygrabber-3d-objects-grabber-189817?aid=1011l58Ft).

## Installation

This integration example is intended to be dropped in to a fresh project along with NeoFPS and MyGrabber.

1. Import NeoFPS and apply the required Unity settings using the NeoFPS Settings Wizard. You can find more information about this process [here](https://docs.neofps.com/manual/neofps-installation.html).

2. Import the MyGrabber asset.

3. Clone this repository to a folder inside the project Assets folder such as "NeoFPS_MyGrabber"

> [!WARNING]
> Do not place the integration folder inside the NeoFPS asset folder structure. If you do this then all of its scripts will be picked up by the NeoFPS assembly definition, which will limit what other scripts within the project they have access to. For more information on assembly definitions, see [the Unity Manual](https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html).

## Integration

The integration requires the following steps:

1. Add the MyGrabber component to your NeoFPS character's camera object.

2. Add a child object to the camera object in the position you want carried objects to be anchored to.

3. Drag the new child object onto the MyGrabber component's *Grab Pos* property.

4. On the root of the character add either the **NeoFpsMyGrabberInput** component or the **NeoFpsMyGrabberInputWithEncumbrance** component and set up as required.

#### Demo Scene

The demo scene shows a number of features of the integration:

- Different weights of objects affecting movement speed (requires the **NeoFpsMyGrabberInputWithEncumbrance** component).

- Using a turret to demonstrate that damage causes the player character to drop the object they're carrying

- Quick-save and load while carrying an object
