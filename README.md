During development of university projects (Pong, BreakOut, SpaceInvaders) using the SFML.Net binding and C#, i created some tools for working with the sfml classes.
Inspired by Unity's code structure, i developed a very basic game-engine.

Installing sfml:
1. Open a folder in vscode
2. In a terminal: `dotnet new console`
2. followed by `dotnet add package sfml.net`
3. wait for all files to be generated (especially the bin folder can take its while :P)
4. Ctrl + Shift + P to open command pallete and run `.Net: Generate Assets for Build and Debug`  
and done!

Using the engine:
1. Paste the `engine` folder into your project
2. Change any settings by accessing the static class Engine.Settings
3. Initialize all your Objects
4. Call the `Engine.Start()` method. Note: this method contains the game-loop and will not exit until the window is closed.  
Tada!

To create a engine-handled object, simply inherit from the `GameObject` class. 
You can now override methods like Update() and Draw() which get called automatically by the engine.

An example of how your code could now look like:
```csharp
using SFML.System;
using SFML.Graphics;
using SilverRaven.SFML;
using SilverRaven.SFML.Tools;

public static class Program
{
    private static void Main()
    {
        _ = new Player() { Position = Engine.Settings.WINDOW_SIZE / 2f };
        Engine.Start();
    }
}

public class Player : GameObject
{
    private float moveSpeed = 5f;
    private RectangleShape shape = new (new Vector2f(50f, 80f));
    private Vector2f input;

    protected override void OnUpdatePosition() => shape.Position = position;
    protected override void HandleInput() => input = new Vector2f(Engine.INPUT.GetAxis("Horizontal"), -Engine.INPUT.GetAxis("Vertical")).ClampMagnitude();
    protected override void FixedUpdate() => Position += input * moveSpeed;
    protected override void Draw(RenderWindow window) => window.Draw(shape);
}
```
