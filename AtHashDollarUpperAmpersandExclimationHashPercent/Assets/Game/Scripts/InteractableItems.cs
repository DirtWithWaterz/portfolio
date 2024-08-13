using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItems : MonoBehaviour
{
    public List<InteractableObject> usableItemList;
    public Dictionary<string, string> examineDictionary = new Dictionary<string, string>();
    public Dictionary<string, string> takeDictionary = new Dictionary<string, string>();

    [HideInInspector] public List<string> nounsInRoom = new List<string>();

    Dictionary<string, ActionResponse> useDictionary = new Dictionary<string, ActionResponse>();
    List<string> nounsInInventory = new List<string>();
    TerminalManager terminal;

    private void Awake()
    {
        terminal = GetComponent<TerminalManager>();
    }

    public string GetObjectsNotInInventory(Room currentRoom, int i)
    {
        InteractableObject interactableInRoom = currentRoom.interactableObjectsInRoom[i];

        if (!nounsInInventory.Contains(interactableInRoom.noun))
        {
            nounsInRoom.Add(interactableInRoom.noun);
            return interactableInRoom.description;
        }

        return null;
    }

    public void AddActionResponsesToUseDictionary()
    {
        for(int i = 0; i < nounsInInventory.Count; i++)
        {
            string noun = nounsInInventory[i];
            InteractableObject interactableObjectInInventory = GetInteractableObjectFromUsableList(noun);
            if (interactableObjectInInventory == null)
                continue;

            for(int j = 0; j < interactableObjectInInventory.interactions.Length; j++)
            {
                Interaction interaction = interactableObjectInInventory.interactions[j];
                if (interaction.actionResponse == null)
                    continue;

                if (!useDictionary.ContainsKey(noun))
                {
                    useDictionary.Add(noun, interaction.actionResponse);
                }
            }
        }
    }

    InteractableObject GetInteractableObjectFromUsableList(string noun)
    {
        for(int i = 0; i < usableItemList.Count; i++)
        {
            if(usableItemList[i].noun == noun)
            {
                return usableItemList[i];
            }
        }
        return null;
    }

    public void DisplayInventory()
    {
        List<string> output = new List<string>();
        output.Add("You look in your backpack, inside you have: ");
        for(int i = 0; i < nounsInInventory.Count; i++)
        {
            output.Add("        " + nounsInInventory[i]);
        }
        output.Add(" ");
        terminal.TypeWrite(output);
    }

    public void ClearCollections()
    {
        examineDictionary.Clear();
        takeDictionary.Clear();
        nounsInRoom.Clear();
    }

    public Dictionary<string, string> Take (string[] seperatedInputWords)
    {
        string noun = seperatedInputWords[1];
        if (nounsInRoom.Contains(noun))
        {
            nounsInInventory.Add(noun);
            AddActionResponsesToUseDictionary();
            nounsInRoom.Remove(noun);
            return takeDictionary;
        }
        else
        {
            return null;
        }
    }

    public void UseItem(string[] seperatedInputWords)
    {
        string nounToUse = seperatedInputWords[1];

        if (nounsInInventory.Contains(nounToUse))
        {
            if (useDictionary.ContainsKey(nounToUse))
            {
                bool actionResult = useDictionary[nounToUse].DoActionResponse(terminal);
                if (!actionResult)
                {
                    terminal.TypeWrite("Nothing happened...");
                }
            }
            else
            {
                terminal.TypeWrite($"You can't use the {nounToUse}.");
            }
        }
        else
        {
            terminal.TypeWrite($"There is no {nounToUse} in your inventory to use.");
        }
    }
}
