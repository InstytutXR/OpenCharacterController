# Open Character Controller

Framework for building first and third person character controllers in Unity.


## Early work-in-progress!

This is a pretty early work in progress. While I'm building this off of my experience putting together first person controllers for a few prototypes, it's likely that this is going to take a while before it's complete for a game. By all means use it and let me know how I can improve it, but don't expect it to be a drop-in solution for a first person controller just yet.


## Direction

My goal with Open Character Controller is to provide a set of building blocks and basic framework for building first and third person character controllers in Unity. At the heart is the [CharacterBody component](Assets/Open%20Character%20Controller/Runtime/CharacterBody.cs) which is logically (but not API wise) a replacement for the built in CharacterController component. The CharacterBody provides custom physics and collision detection in a way that is more customizable than the CharacterController in Unity.

On top of that I'm working to build a framework for the character controllers themselves. The hope is to have something that, while potentially requiring code to set up, makes it easy to assemble a character controller meeting the needs of a game without a lot of manual toil work.


## Current status

Currently I'm focused on first person player movement abilities. I've implemented the following so far:

- Walk
- Run
- Crouch
- Jump
- Lean 
- Slide

I'm hoping to eventually add climbing up/down ladders and mantling to the list.

Once movement is feeling solid I plan to move into more interaction based systems like opening doors, inspecting items, and some form of holding things in first person (be it a flashlight, a gun, etc).

