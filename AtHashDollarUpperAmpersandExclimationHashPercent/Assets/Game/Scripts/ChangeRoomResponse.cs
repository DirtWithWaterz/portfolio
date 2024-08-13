using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TextAdventure/ActionResponses/ChangeRoom")]
public class ChangeRoomResponse : ActionResponse
{
    public Room roomToChangeTo;

    public override bool DoActionResponse(TerminalManager terminal)
    {
        if(terminal.roomNav.currentRoom.roomName == requiredString)
        {
            terminal.roomNav.currentRoom = roomToChangeTo;
            terminal.DisplayRoomText();
            return true;
        }

        return false;
    }
}
