using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Interpreter : MonoBehaviour
{

    [SerializeField] TerminalManager terminal;

    public Dictionary<string, string> colors = new Dictionary<string, string>(){

        {"black", "#021b21"},
        {"gray", "#555d71"},
        {"red", "#ff5879"},
        {"yellow", "#f2f1b9"},
        {"blue", "#9ed9d8"},
        {"purple", "#d926ff"},
        {"orange", "#ef5847"},
        {"white", "#FFFFFF"}
    };

    List<string> response = new List<string>();

    public List<string> Interpret(string userInput){

        response.Clear();

        string[] args = userInput.Split();

        if(args[0] == "StartUp" && !terminal.hasBooted){

            if(args[1] == "0"){

                response.Add("SOIB Date 07/13/05 " + DateTime.Now.ToString("hh:mm:ss"));
                return response;
            }
            if(args[1] == "1"){

                response.Add("CPU: letni(r) CPU 330 @ 40 MHz");
                return response;
            }
            if(args[1] == "2"){

                response.Add("Speed: 40 MHz");
                return response;
            }
            if(args[1] == "3"){

                response.Add("This AVG/ICP Soib is released under the UNG LPGL");
                return response;
            }
            if(args[1] == "4"){

                response.Add("");
                response.Add("Press F11 for SBB POPUP");
                return response;
            }
            if(args[1] == "5"){

                response.Add("Memory Clock: 64 MHz, Tcl:7 Trcd:4 Trp:8 Tras:20 (2T Timing) 8 bit");
                return response;
            }
            if(args[1] == "6"){

                response.Add("PMU ROM Version: 9303");
                return response;
            }
            if(args[1] == "7"){

                response.Add("NVMM ROM Version: 4.092.89");
                return response;
            }
            if(args[1] == "8"){

                response.Add("128MB OK");
                return response;
            }
            if(args[1] == "9"){

                response.Add("USB Device(s): 1 Keyboard, 1 Mouse, 1 Hub, 1 Storage Device");
                return response;
            }
            if(args[1] == "10"){

                response.Add("Auto-detecting USB Mass Storage Devices..");
                return response;
            }
            if(args[1] == "11"){

                response.Add("Device #01: USB 2.0 FlashDisk *Speed*");
                return response;
            }
            if(args[1] == "12"){

                response.Add("01 USB mass storage devices found and configured.");
                return response;
            }
            if(args[1] == "13"){

                response.Add("(C) Malato, Inc.");
                return response;
            }
            if(args[1] == "14"){

                response.Add("45-0010-00010-001010111-063606-79297-1AE0V003-Y2UC");
                return response;
            }
            if(args[1] == "15"){

                response.Add("Booting from Hard Disk...");
                return response;
            }
            if(args[1] == "16"){

                response.Add("");
                terminal.Clear();
                return response;
            }
        }

        if(args[0] == "help"){

            response.Add("quit game -- quits the application. (there is no save file)");
            response.Add("loadtitle [file name] [color] -- loads the file in ascii art in the desired color.");
            response.Add("go [cardinal direction] -- attempts to move you in the direction specified.");
            response.Add("examine [object] -- allows you to examine the specified object.");
            response.Add("take [object] -- allows you to pick up and stow the desired object.");
            response.Add("inventory -- lists what you have in your backpack.");
            response.Add("use [object] -- attempts to use the specified object.");
            response.Add("clear -- clears the terminal.");
            response.Add("");

            return response;

        }

        if(args[0] == "quit" || args[0] == "exit"){

            try{
                if(args[1] == "game"){
                    response.Add("");
                    Application.Quit();

                    return response;
                }
            }
            catch{
                response.Add("if you are attempting to close the game, the correct command is: 'quit game'");
                response.Add("");

                return response;
            }
            response.Add("if you are attempting to close the game, the correct command is: 'quit game'");
            response.Add("");

            return response;
        }

        if(args[0] == "loadtitle"){

            try{

                LoadTitle(args[1], args[2], 2);
                return response;
            }
            catch{

                response.Add("invalid Input, please make sure you typed the file name correctly");
                response.Add("and that you have chosen a valid color.");
                response.Add("");
                return response;
            }
        }

        if (args[0] == "go")
        {
            try
            {
                if (terminal.roomNav.exitDictionary.ContainsKey(args[1]))
                {
                    terminal.roomNav.currentRoom = terminal.roomNav.exitDictionary[args[1]];
                    response.Add($"You head off to the {args[1]}.");
                    response.Add(" ");
                    terminal.DisplayRoomText();
                    return response;
                }
                else
                {
                    response.Add($"There is no path to the {args[1]}.");
                    response.Add(" ");

                    return response;
                }
            }
            catch
            {
                response.Add("You must choose a direction.");
                response.Add(" ");

                return response;
            }
            
        }

        if(args[0] == "examine")
        {
            try
            {
                if (args[1] == "")
                {
                    response.Add("You can't examine nothing.");
                    response.Add(" ");
                    return response;
                }
                else if (terminal.TestVerbDictionaryWithNoun(terminal.interactableItems.examineDictionary, args[0], args[1]) == "You can't examine ")
                {
                    response.Add("You can't examine nothing.");
                    response.Add(" ");
                    return response;
                }
                else
                {
                    terminal.TypeWrite(" ");
                    terminal.TypeWrite(terminal.TestVerbDictionaryWithNoun(terminal.interactableItems.examineDictionary, args[0], args[1]));
                    terminal.TypeWrite(" ");
                    return response;
                }
            }
            catch
            {
                try
                {
                    if(args[1] != "") { 

                        response.Add($"{args[1]} is unexaminable.");
                        response.Add(" ");
                        return response;
                    }
                    else
                    {
                        response.Add("You can't examine nothing.");
                        response.Add(" ");
                        return response;
                    }
                }
                catch
                {
                    response.Add("You can't examine nothing.");
                    response.Add(" ");
                    return response;
                }
            }
        }

        if(args[0] == "take")
        {
            try
            {
                if(args[1] == "")
                {
                    response.Add("You must describe what it is you wish to take.");
                    response.Add(" ");
                    return response;
                }

                Dictionary<string, string> takeDictionary = terminal.interactableItems.Take(args);

                if (takeDictionary != null)
                {
                    terminal.TypeWrite(" ");
                    terminal.TypeWrite(terminal.TestVerbDictionaryWithNoun(takeDictionary, args[0], args[1]));
                    terminal.TypeWrite(" ");
                    return response;
                }
                else
                {
                    response.Add($"There is no {args[1]} to take.");
                    response.Add(" ");
                    return response;
                }
            }
            catch
            {
                response.Add("You must describe what it is you wish to take.");
                response.Add(" ");
                return response;
            }
        }

        if(args[0] == "inventory")
        {
            terminal.interactableItems.DisplayInventory();
            return response;
        }

        if(args[0] == "use")
        {
            try
            {
                if(args[1] == "")
                {
                    response.Add("You need something to use for this command to be applicable.");
                    response.Add(" ");
                    return response;
                }
                else
                {
                    terminal.interactableItems.UseItem(args);
                    return response;
                }
            }
            catch
            {
                response.Add("You need something to use for this command to be applicable.");
                response.Add(" ");
                return response;
            }
        }

        if(args[0] == "clear")
        {
            terminal.Clear();
            terminal.DisplayRoomText();
            return response;
        }

        else{

            response.Add($"'{string.Join(" ", args)}' is not recognized as a valid input,");
            response.Add("please try again.");
            response.Add("");

            return response;
        }
    }

    public string ColorString(string s, string color){

        string leftTag = $"<color={color}>";
        string rightTag = $"</color>";

        return $"{leftTag}{s}{rightTag}";
    }

    void ListEntry(string a, string b){

        response.Add($"{ColorString(a, colors["orange"])} : {ColorString(b, colors["yellow"])}");
    }

    public void LoadTitle(string path, string color, int spacing){

        StreamReader file = new StreamReader(Path.Combine(Application.streamingAssetsPath, path));

        for(int i = 0; i < spacing; i++){

            response.Add("");
        }

        while(!file.EndOfStream){

            response.Add(ColorString(file.ReadLine(), colors[color]));
        }

        for(int i = 0; i < spacing; i++){

            response.Add("");
        }

        file.Close();

    }
}
