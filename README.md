<center><img src="./doc/icon.png" alt="drawing" width="200"/></center>

# MSFS STOL Training Tool

## Definition

the MSFS STOL Training Tool is an external Application that connects via SimConnect ot MSFS and measures and pilots performace while training for STOL competition stile landings, anywhere in the world or on designated fields.  
It sends telemetry and score data to a database that can be accessed on demand but has not been published yet due to uncertainty with data protection.  
STOL competition stile landings are compettative maneuvers that aim to takeoff and land a plane as in competitions like:  National STOL, The REAL STOL Fest, FFPLUME,  european stol competition or NZ Bush Pilot Championships.  
Therefore The tool supports STOL fields recreated in the simulator of: National STOL, The REAL STOL Fest, FFPLUME,  european stol competition.  
Since Season 2025 the tool is used by National eSTOL for score judgement.  

## Disclaimer

This Tool is intended for training purposes only.  
The numbers give a quick feedback and rough estimate of your performance. They do not guarantee any accuracy.  
Do not challenge any competition score based on this tools' estimation alone.  

This tool is not officially associated with National STOL Series.

## Concept

This tool give quick performance data about a STOL competition run by recording Takeoff and Landing.
Distances are calculated based on an initial start point and heading marking the start line.
from here distances are measures along the heading axis.

<center><img src="./doc/STOL_Training_Overview.jpg" alt="drawing" width="600"/></center>

StartPoint: initial start point on Start Line
TakeoffPoint: Location where takeoff is detected. First point where plane is not on ground.
TouchdownPoint: Location where the Plane touched down first.
StopPoint: Location where plane came to a full stop.

Takeoff distance: Distance from StartPoint to TakeoffPoint
Touchdown distance: Distance from StartPoint to TouchdownPoint
Landing distance: Distance from StartPoint to StopPoint
Stopping distance: Distance from TouchdownPoint to StopPoint

<center><img src="./doc/STOL-Training-Tool.png" alt="drawing" width="600"/></center>

This tool has two modes: OpenWorld and Presets

It's not possible to embed videos directly, but you can put an image which links to a YouTube video:

Video:
[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/Pc77grlt8Zk/0.jpg)](https://www.youtube.com/watch?v=Pc77grlt8Zk)

### Open World 

The user can set the startpoint everywhere using parking breake or smoke.

### Presets

Start Point are provided as preset for known STOL Fields

### Preset Creation

Click the Create Preset Button.
it then provides a preset as JSON you can add to presets.json file.

Example:
```
    {
        "title": "YOUR TITLE",
        "start_lat": 36.378282538897,
        "start_long": -86.409490615829,
        "start_alt": 168,
        "start_hdg": 168
    }
```

## Restrictions

The tool polls aircraft telemety data in an interval off 250ms. This limits the prescision the tool can detect any state changes.
For tochdown and landing it refers to Simconnect variable "SIM ON GROUND". Detection qualaty depends on Sim dettecting this parameter.
The tool does not access Takeoff or Touchdown Events (yet).
The start point and touchdown point are set by plane position. this position most likely refers to planes center of gravity and not to wheels touchdown point.
Since the offset between those is the same for lineup, takeoff and landing if may be ignored. For differen Plane Types, especially those of different size.
to counter this ther is the gear offset feature.
enter an offset to your plane in GearOffset.json until teleport drops you onto the referenceline.

## Usage

start `msts_estol_training_tool.exe`

```
┌─────────────────────┐
│ STOL Training Tool │
└─────────────────────┘

Disclaimer:

This Tool is intended for training purposes only.
The numbers give a quick feedback and rough estimate of your performance.They do not guarantee any accuracy.
Do not challenge any competition score based on this tools' estimation alone.
Make sure to record your flight for any necessary score validation.

```

- setup user name used for InfluxDB upload. Leave empty to ignore.
- select mode OpenWorld or select a preset.
- lineup with start line and takeoff -> "takeoff detected"
- fly pattern and land -> "landing detected"
- result is shown after full stop
  - result summary is printed to console and result box
  - result is saved to .csv file
  - result is pushed to InfluxDB
  - state panel shows detected state of flight
 
<center><img src="./doc/initial.png" alt="states" width="600"/></center>

select a Preset from Dropdown and press "Apply" or set Openworld Start Pos with Button "Set Start"

<center><img src="./doc/teleport.png" alt="states" width="600"/></center>

you are able to teleport directly to referenceline

<center><img src="./doc/panel.png" alt="states" width="600"/></center>

the result is show as usual in the result box

## States

<center><img src="./doc/states.png" alt="states" width="600"/></center>

## Results upload
Results are uploaded for STOL competition Staff to analyse



## Known issues

Application pops up and closes itself directly
wehn executing on console you can see: 
```
user\stol trainig tool> & 'ASTOL Training Tool.exe' You must install or update .NET to run this application. 
App: user\stol trainig tool\STOL Training Tool.exe Architecture: x6U Framework: 'Microsoft.NETCore.App', version '9.0.0' (x6U) .NET location: C:\Program Files\dotnet\ 
The following frameworks were found: 6.0.12 at EC:\Program Files\dotnet\shared\Microsoft.NETCore.App] 
Learn about framework resolution: https://aka.ms/dotnet/app—launch—failed 
To install missing framework, download: https://aka.ms/dotnet-core-applaunch?framework=Microsoft.NETCore.App&framework_version=9.0.0&arch=x6U&rid=win10—x6U 
```

dotNet Framework is not present or old. follow the link provided to install or update dotNet Framework.
https://dotnet.microsoft.com/en-us/download/dotnet/9.0/runtime?cid=getdotnetcore&os=windows

