# RollPunk Architecture Rules

## General

This is a Godot C# project.

Do not move responsibilities between layers without discussion.

## Architecture

Entities:
- Base serializable objects.

Fields:
- Dynamic game state.
- Fields are not UI.

UI:
- Only visual representation.

Session:
- Stores synchronized state.

Server:
- Dumb synchronizer.
- Does not execute Lua.

## Modding

Lua runs only on client.

Lua interacts with C# through API wrappers.

Never expose raw internal objects.

## Code style

Prefer explicit types.
Avoid static global state.