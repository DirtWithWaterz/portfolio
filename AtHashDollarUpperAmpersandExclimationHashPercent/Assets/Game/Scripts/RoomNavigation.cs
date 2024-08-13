using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNavigation : MonoBehaviour
{
    public Room currentRoom;

    public Dictionary<string, Room> exitDictionary = new Dictionary<string, Room>();
    TerminalManager terminal;

    private void Awake()
    {
        terminal = GetComponent<TerminalManager>();
    }

    public void UnpackExitsInRoom()
    {
        for(int i = 0; i < currentRoom.exits.Length; i++)
        {
            exitDictionary.Add(currentRoom.exits[i].keyString, currentRoom.exits[i].valueRoom);
            terminal.interactionDescriptionsInRoom.Add(currentRoom.exits[i].exitDescription);
        }
    }

    public void ClearExits()
    {
        exitDictionary.Clear();
    }
}
