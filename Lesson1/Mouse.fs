module Mouse

open System
open System.Drawing
open Serilog


let private rnd (value: int): int =
    let random = Random()
    random.Next(value)

module private Screen  =

    let private width = 1920
    let private height = 1080
    let private maxCoordinateValue = 65535

    let convertToMouseCoords (x: int, y: int) =
        let mouseX = (x * maxCoordinateValue) / width
        let mouseY = (y * maxCoordinateValue) / height
        mouseX, mouseY

module private Interop =

    open System.Runtime.InteropServices
    [<DllImport("user32.dll", SetLastError = true)>]
    extern bool GetCursorPos(Point& lpPoint)

    [<DllImport("user32.dll", SetLastError = true)>]
    extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo)

    [<Flags>]
    type MouseEventFlags =
        | LEFTDOWN = 0x00000002
        | LEFTUP = 0x00000004
        | MIDDLEDOWN = 0x00000020
        | MIDDLEUP = 0x00000040
        | MOVE = 0x00000001
        | ABSOLUTE = 0x00008000
        | RIGHTDOWN = 0x00000008
        | RIGHTUP = 0x00000010

module private Move =
    let Linearly (newPosition: Point) (steps: int) =
        let mutable start = Point(0, 0)
        Interop.GetCursorPos(&start) |> ignore
        let slopeX = float32 (newPosition.X - start.X) / float32 steps
        let slopeY = float32 (newPosition.Y - start.Y) / float32 steps
        for i in 0 .. steps do
            let deltaX = int(float32 start.X + float32 slopeX * float32 i)
            let deltaY = int(float32 start.Y + float32 slopeY * float32 i)
            let dx, dy = Screen.convertToMouseCoords(deltaX, deltaY)
            Interop.mouse_event((int)Interop.MouseEventFlags.MOVE + (int)Interop.MouseEventFlags.ABSOLUTE, dx, dy, 0, 0)
            System.Threading.Thread.Sleep(rnd 6) // Sleep for a short time to make the movement smooth
        let newX, newY = Screen.convertToMouseCoords(newPosition.X, newPosition.Y)
        Interop.mouse_event((int)Interop.MouseEventFlags.MOVE + (int)Interop.MouseEventFlags.ABSOLUTE, newX, newY, 0, 0)

module Diagnostics =
    let getPosAsResolution () : Point =
        let mutable point = Point(0, 0)
        Interop.GetCursorPos(&point) |> ignore
        point

    let getPosAsCoordinates () =
        let mutable point = Point(0, 0)
        Interop.GetCursorPos(&point) |> ignore
        let x, y = Screen.convertToMouseCoords(point.X, point.Y)
        x, y

module Events =
    let moveLinearly (newPosition: Point) (steps: int) =
        Move.Linearly newPosition steps
