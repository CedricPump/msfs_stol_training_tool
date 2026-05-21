v1.4.32

- fixed Taylorcraft

v1.4.31

- fixed Maule Tire index for 2024
- adde ASL to Pattern Altitude

v1.4.30

- added MiniMax config
- fixed deviations field alignment

v1.4.29

- added Influx Settings to Config
- changed Init Hash from Coords to Config
- disabled setFuel button
- added Config ForcePauseOnTeleportFromMoving (Default: True)

v1.4.28

- preset and config refinement

v1.4.27

- added FSWeekend presets

v1.4.26

- added prop collision disable flag
- fixed ambinent wind reading

v1.4.25

- added T207A
- added Navion B
- added L17B
- added Comanche
- added Kodiak
- added L-19
- added L-4H
- added DH60
- added Tiger Moth
- added C208

v1.4.24

- added DHC6 Twin Otter
- added BN2A Islander
- fixed Maule MX Prop strike threshold

v1.4.23

- added Maule MX 7-180 plane config
- added matching debug output
- added UILogger
- fixed plane change detection
- shortened default config plane name

v1.4.22

- added TL-3000
- added Vixxen
- added Draco X
- fixed C152 20204
- fixed Doubleender 20204

v1.4.21

- changed alignment output
- added rans trike config
- fixed rans s6s config
- added Cap4 config
- added Chinook config
- added Shock Ultra config
- added Draco X config
- added J3 cub CAS config
- fixed Beaver config
- fixed Maule M7 config

v1.4.20

- added C185 config
- added C172 config
- added NXCub config
- added Savage Cub config
- imporved XCub Regex
- fixed gear offeset reading

v1.4.19

- added C170B config variation
- changed config to use Regex match

v1.4.18

- added Set Fuel Standard button

v1.4.17

- added prop rpm to telemetry
- added fuel percent to result
- added pressure to result
- added temperature to result
- added field elevation to result
- added wind speed and dir (absolut) to result
- added UUID to telemetry, result and events

v1.4.16

- fixed telemetry freeze on teleport
- fixed save preset brakes transparency
- added B748
- added B744
- added A388
- changed climout cancelatrion condition to angle 90° off

v1.4.15

- added preset ui improvements

v1.4.14

- added seperate tail touch entry with time

v1.4.13

- added Pattern Alt indicator

v1.4.12

- fixed log touchdown conversion

v1.4.11

- added plane config for XCub 2024
- added REST API for Ingame panel telemetry

v1.4.10

- added tooltips
- added flight controls to recording

v1.4.9

- added plane config Robin DR40

v1.4.8

- reduced popups
- added teleport popup never show again option
- no attitude change on static teleport
- popup fixes (topmost and async)
- improven button visibility

v1.4.7

- added config stop on tail touch

v1.4.6

- added plane config ATR 42-600S
- added ResultBox character limit (configurable)
- added Sim not connected warning
- changed alignment hints
- UI fixes

v1.4.5

- added check update button
- added manual update option

v1.4.4

- added plane config Savage Norden
- added plane config Top Rudder
- added plane config Hawk Arrow
- added plane config XCub
- fixed gear offset Wilga 80P
- increased max vs deviation threshold to 3000
- fixed GPX recording CG offset
- fixed Open World feature
- added teleport while flying handling
- added in flight initialization
- added debug auto pause feature
- added unpause button
- overhauled result textbox log behavior
- added clear result log button
- removed trikes config file
- removed FormMain
- improved console log
- improved debug log
- console cleanup
- changed version file name (...vX.Y.Z.zip)

v1.4.3

- incresed BankAngle Threshold to 75
- renamed violations to deviations
- added deviation severities
- fixed deviation touchdownVS 
- fixed result MaxG MaxVS
- fixed result formatting

v1.4.2

- added Prop strike for Rans S6S
- added max G and VS to result output
- fixed button anchor
- added violations / warnings for: HightBankAngle HighClimbRate OverspeedVNE OverspeedFlaps
- added assits violation

v1.4.1

- added Auto Select Button (selects nearest reference line)
- added live violations box
- added plane type label
- added type specific G-Force and VSpeed Limits
- fixed Prop strike after result bug
- added alignment state down field

v1.4.0

- fixed Wilga Wheel detection
- added Beaver PlaneConfig

v1.3.9

- fixed Aklahoma field direction
- fixed UI GPX checkbock alignment
- fixed Wing Strike Contactpoints
- added Prop Strike detection
- added Prop Strike threshold

v1.3.8

- added wingtip indicators
- added collision indicator
- added plane config
- added collision indexes
- prepare contact point fixes

v1.3.7

- fixed scout offset
- fixed recor time zulu
- fixed recording broken by plane type

v1.3.6

- adapted startline for new KAXQ Scenery

v1.3.5

- fixed too many popups
- updated gear offset for Scout
- added event unflip
- removed event ExcessiveTouchdownSpin

v1.3.4

- added unflip
- added confirmation option in config
- added debug log
- minor UI fixes

v1.3.3

- fixed blocking export file from 1.3.1

v1.3.2

- added darkmode
- added transparency
- added telport popup
- added gpx recording checkbox
- removed console window

v1.3.1

- added GPX Recorder Feature

v1.3.0

- added session key
- added anitstall detection wip
- fixed typo in wingstrike
- added sores xml server
- added takeoff score to xml server

v1.2.9

- added aligned state display to UI
- added selected preset to telemetry
- added excessive spin buffer threshold of 1°

v1.2.8

- added spin violaton angle to flags calculation

v1.2.7

- changed simvars access to RequestDataOnSimObject
- added get date by frames setting
- increased polling interval

v1.2.6

- increased polling intervall for more pescise results 250 ms -> 100 ms
- seperated excessive spin events stop, touchdown and max
- added gear offset for C152 Trike
- added plane change detection
- changed disclaimer text
- added sim not connected error handling

v1.2.5

- added Maule Tundro to GearOffset
- added gear offset for maule m7
- added exception handling to events
- added send data check to events
- added Violations
- added Events: EXCESSIVE_VSPEED, SCRATCH, EXCESSIVE_SPIN, EXCESSIVE_G, WINGSTRIKE, TOUCH_N_GO

v1.2.4

- added piper comanche gear offset
- fixed Rans Gear offset for TD
- added eSTOL Events

v1.2.3

- added debug plane position
- added wind indicator
- added wind indicator before init
- added BN2 Islander
- added auto update
- added Plane alignment helper output
- improved field canvas autoscale
- improved gneral scaling (for vr users)
- improved showing TD / TO / Stop positions correctly
- improved result decimals
- fixed husky gear offset
- fixed c170bt gear offset
- fixed always on top on start

v1.2.2

- introduced settings saving
- added cycle state climbout (2nd rebound point on takeoff feature)
- added privacy disclaimer popup
- added always on top selection persistence
- added unit selection persistence
- added send telemetry persistence
- added send results persistence
- added UI loading early
- updated verison

v1.2.1

- corrected default gear offset by +1.5 for default wheel deflation
- unsing contact point vars for ongorund reading
- splitted preset.json
- added reload clickspot
- added link to privacy policy
- refactoring
- updated version
- splitted preset.json
- added reload clickspot
- added link to privacy policy
- refactoring

v1.2.0

- updated default gear offset
- updated scenery coordinates
fixes for field coordinates
pony coordinate fix
- fixed gear offset in telemetry
- updated old and Europe STOL fields

note:
This Update overhauls the underlying position reference system.
Please report any misalignments for plane types at teleport.

Wheel middle should align with the edge of the reference line.

v1.1.6

- fixed some spelling issues
- added custom timer offset
- changed stol field allignment
- added violations
- added 42VA Virginia Beach Heritage (old system)
- added debug button
- added debug output
- added Wheel RPM check for taildragging detection
- added max spin, pitch and bank checks
- added version to telemetry

v1.1.5

- added requested always on top feature

v1.1.4

- added draggin tail before line feature ... hopefully ... for BrownDog942
- improved disclaimer
- improved update link

Know Issues: requres .net Framework. see: known_issues.md

v1.1.3

- fixed fuel percent
- added unlimited fuel
- added GForce
- added takeoff -> hold to state machine
- added start (no offset button)
- fixed timer running on taxi
- fixed timer refresh rate

v1.1.2

- added Timer

v.1.1.1

- added config.json for setting custom intervals
- fixed PilotWeight
- added version update notification
reduced Telemetry Intervall (performance and traffic improvement)

v1.1.0

- changed database bucket
- added telemetry
- added send data checkboxes
- grafana magic

v1.0.4

- added unit selection

v1.0.3

- added unit selection

v1.0.2

- added visualizer
- added teleport feature
- added Gear to CG offset
- added basic GUI
- fixed create preset button

v1.0.1

- added teleport feature
- added Gear to CG offset
- added basic GUI
- fixed create preset button

v.1.0.0

- added teleport feature
- added Gear to CG offset
- added basic GUI

v0.2.0 Pre-release

- added preset creation mode
- added Music City field
- added Lonestar field

v.0.1.0 Pre-release

initiated Project
- added flight tracking
- added result calculation
- added result display
- added .csv export
- added influx send
- added presets
- added custom start
- added inithash
- added username