open System
open System.Drawing
open Serilog

[<EntryPoint>]
let main argv =
    Logs.Init() // Initialize SeriLog
    let programName = "Pixel Seeker"
    let version = 0.1

    Log.Information("Starting {0} {1}", programName, version)

    while (true) do
        Threading.Thread.Sleep(1500)
        let mutable pos = Point(0,0)
    
        // Get and Log Resolution Cursor Position
        pos <- Mouse.Diagnostics.getPosAsResolution()
        Log.Information("X: {0} Y: {1}", pos.X, pos.Y)
        
        // Get and Log Coodinates Cursor Position
        let cordX, cordY = Mouse.Diagnostics.getPosAsCoordinates()
        Log.Information("X: {0} Y: {0}", cordX, cordY)
        
        // Move the mouse cursor
        Log.Information("Moving Mouse Cursor")
        let newPos = Point(200,400)
        Mouse.Events.moveLinearly newPos 14
        Log.Information("Mouse moved to target position: {0}", newPos)
        
    0
