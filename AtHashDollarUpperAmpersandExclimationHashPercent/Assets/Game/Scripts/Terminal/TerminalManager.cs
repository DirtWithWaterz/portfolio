using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TMPro.Examples;
using System.IO;
using UnityEngine.InputSystem;

public class TerminalManager : MonoBehaviour
{

    [HideInInspector] public RoomNavigation roomNav;
    [HideInInspector] public List<string> interactionDescriptionsInRoom = new List<string>();
    [HideInInspector] public InteractableItems interactableItems;

    bool canTypeWrite = true;
    public GameObject directoryLine;
    public GameObject responseLine;

    public InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect sr;
    public GameObject msgList;

    [SerializeField] GameObject softwareAndVersion;
    [SerializeField] GameObject fakeCopyRight;
    [SerializeField] GameObject memoryTest;
    [SerializeField] GameObject initializeUSB;

    float directoryTextWidth;
    string directoryText = "Input>";

    Interpreter interpreter;

    public bool hasBooted = false;

    Vector2 baseMsgListSizeDelta = new Vector2();

    private void Awake(){

        interactableItems = GetComponent<InteractableItems>();
        roomNav = GetComponent<RoomNavigation>();
    }

    private void Start(){

        baseMsgListSizeDelta = msgList.GetComponent<RectTransform>().sizeDelta;
        interpreter = GetComponent<Interpreter>();
        // directoryText = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + ">";
        directoryText += ">";
        directoryTextWidth = directoryText.Length * 13.3f;
        userInputLine.GetComponentsInChildren<Text>()[0].text = directoryText;
        userInputLine.GetComponentsInChildren<Text>()[0].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(directoryTextWidth, 60);
        terminalInput.caretWidth = 15;
        userInputLine.SetActive(false);
        StartCoroutine(TerminalStartUp());
    }

    public void DisplayRoomText()
    {
        ClearCollectionsForNewRoom();
        UnpackRoom();

        List<string> combinedText = new List<string>();
        
        combinedText.Add(roomNav.currentRoom.description);
        foreach(string s in interactionDescriptionsInRoom)
        {
            combinedText.Add(" ");
            combinedText.Add(s);
        }
        combinedText.Add(" ");

        TypeWrite(combinedText);

    }

    void UnpackRoom()
    {
        roomNav.UnpackExitsInRoom();
        PrepareObjectsToTakeOrExamine(roomNav.currentRoom);
    }

    void PrepareObjectsToTakeOrExamine(Room currentRoom)
    {
        for(int i = 0; i < currentRoom.interactableObjectsInRoom.Length; i++)
        {
            string descriptionNotInInventory = interactableItems.GetObjectsNotInInventory(currentRoom, i);
            if(descriptionNotInInventory != null)
            {
                interactionDescriptionsInRoom.Add(descriptionNotInInventory);
            }

            InteractableObject interactableInRoom = currentRoom.interactableObjectsInRoom[i];

            for(int j = 0; j < interactableInRoom.interactions.Length; j++)
            {
                Interaction interaction = interactableInRoom.interactions[j];
                if(interaction.inputType == "examine")
                {
                    interactableItems.examineDictionary.Add(interactableInRoom.noun, interaction.textResponse);
                }
                if (interaction.inputType == "take")
                {
                    interactableItems.takeDictionary.Add(interactableInRoom.noun, interaction.textResponse);
                }
            }
        }
    }

    public string TestVerbDictionaryWithNoun(Dictionary<string, string> verbDictionary, string verb, string noun)
    {
        if (verbDictionary.ContainsKey(noun))
        {
            return verbDictionary[noun];
        }

        if(noun == "")
        {
            return $"You can't {verb} nothing.";
        }

        return $"You can't {verb} the {noun}.";
    }

    void ClearCollectionsForNewRoom()
    {
        interactableItems.ClearCollections();
        interactionDescriptionsInRoom.Clear();
        roomNav.ClearExits();
    }

    public void TypeWrite(string text, bool shake = false)
    {
        StartCoroutine(TypeWriteInternal(text, shake));
    }

    public void TypeWrite(List<string> interpretation, bool shake = false)
    {
        StartCoroutine(AddInterpreterLinesTypewrite(interpretation, shake));
    }

    private IEnumerator TypeWriteInternal(string text, bool shake = false)
    {
        yield return new WaitUntil(() => canTypeWrite);
        canTypeWrite = false;
        AddInterpreterLineTypewrite(text, shake);
    }

    private IEnumerator TerminalStartUp(){
        
        yield return new WaitForSeconds(0.8f);
        softwareAndVersion.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        fakeCopyRight.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        AddInterpreterLine(interpreter.Interpret("StartUp 0")[0]);
        yield return new WaitForSeconds(0.07f);
        AddInterpreterLine(interpreter.Interpret("StartUp 1")[0]);
        yield return new WaitForSeconds(0.07f);
        AddInterpreterLine(interpreter.Interpret("StartUp 2")[0]);
        yield return new WaitForSeconds(0.07f);
        AddInterpreterLine(interpreter.Interpret("StartUp 3")[0]);
        yield return new WaitForSeconds(0.07f);
        AddInterpreterLines(interpreter.Interpret("StartUp 4"));
        yield return new WaitForSeconds(1.2f);
        AddInterpreterLine(interpreter.Interpret("StartUp 5")[0]);
        yield return new WaitForSeconds(0.5f);
        memoryTest.transform.SetAsLastSibling();
        memoryTest.SetActive(true);
        AddInterpreterLine("");
        yield return new WaitForSeconds(1.85f);
        AddInterpreterLine(interpreter.Interpret("StartUp 6")[0]);
        yield return new WaitForSeconds(0.3f);
        AddInterpreterLine(interpreter.Interpret("StartUp 7")[0]);
        yield return new WaitForSeconds(0.3f);
        initializeUSB.transform.SetAsLastSibling();
        initializeUSB.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        AddInterpreterLine(interpreter.Interpret("StartUp 8")[0]);
        yield return new WaitForSeconds(0.1f);
        AddInterpreterLine(interpreter.Interpret("StartUp 9")[0]);
        yield return new WaitForSeconds(0.15f);
        AddInterpreterLine(interpreter.Interpret("StartUp 10")[0]);
        yield return new WaitForSeconds(0.3f);
        AddInterpreterLine(interpreter.Interpret("StartUp 11")[0]);
        yield return new WaitForSeconds(0.3f);
        AddInterpreterLine(interpreter.Interpret("StartUp 12")[0]);
        yield return new WaitForSeconds(0.3f);
        AddInterpreterLine(interpreter.Interpret("StartUp 13")[0]);
        yield return new WaitForSeconds(0.6f);
        AddInterpreterLine(interpreter.Interpret("StartUp 14")[0]);
        yield return new WaitForSeconds(0.4f);
        AddInterpreterLine(interpreter.Interpret("StartUp 15")[0]);
        yield return new WaitForSeconds(0.7f);
        AddInterpreterLine(interpreter.Interpret("StartUp 16")[0]);

        yield return new WaitForSeconds(0.3f);
        AddInterpreterLines(interpreter.Interpret("loadtitle title.txt red"), true);
        yield return new WaitForSeconds(0.3f);

        ScrollToBottom(10);
        DisplayRoomText();
        yield return new WaitUntil(() => canTypeWrite);

        userInputLine.transform.SetAsLastSibling();
        userInputLine.SetActive(true);

        terminalInput.ActivateInputField();
        terminalInput.Select();

        hasBooted = true;
    }

    private void OnGUI(){

        if(!hasBooted){return;}
        
        if(terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return)){

            // Store whatever the user typed
            string userInput = terminalInput.text;

            // Clear the input field
            ClearInputField();

            // Instantiate a gameobject with a directory prefix
            AddDirectoryLine(userInput);


            int lines = 0;
            // Add interpretation lines
            if(userInput.Contains("open eyes")){

                interpreter.Interpret(userInput);
            }
            else{

                lines = AddInterpreterLines(interpreter.Interpret(userInput));
            }

            // Scroll to the bottom of the scrollrect
            ScrollToBottom(lines);

            // Move the user input line to the end
            userInputLine.transform.SetAsLastSibling();

            // Refocus the input field
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    void ClearInputField(){

        terminalInput.text = "";
    }

    void AddDirectoryLine(string userInput){

        // Resize the command line container
        Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 35.0f);

        // Instantiate the directory line
        GameObject msg = Instantiate(directoryLine, msgList.transform);

        // Set msg child index to end
        msg.transform.SetAsLastSibling();

        // Set msg text
        msg.GetComponentsInChildren<Text>()[1].text = userInput;
        msg.GetComponentsInChildren<Text>()[0].text = directoryText;
        msg.GetComponentsInChildren<Text>()[0].gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(directoryTextWidth, 60);

    }

    int AddInterpreterLines(List<string> interpretation, bool shake = false){

        for(int i = 0; i < interpretation.Count; i++){

            // Instantiate the response line
            GameObject res = Instantiate(responseLine, msgList.transform);

            if(shake){res.transform.GetChild(0).gameObject.AddComponent<VertexJitter>(); res.transform.GetChild(0).GetComponent<VertexJitter>().CurveScale = 1;}

            // Set it to the end of all the messages
            res.transform.SetAsLastSibling();

            // Get size of message list, and resize
            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);

            // Set response line text
            res.GetComponentInChildren<TMP_Text>().text = interpretation[i];

        }

        return interpretation.Count;
    }

    void AddInterpreterLine(string interpretation){

            // Instantiate the response line
            GameObject res = Instantiate(responseLine, msgList.transform);

            // Set it to the end of all the messages
            res.transform.SetAsLastSibling();

            // Get size of message list, and resize
            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);

            // Set response line text
            res.GetComponentInChildren<TMP_Text>().text = interpretation;
    }

    void AddInterpreterLineTypewrite(string interpretation, bool shake = false){

        // Instantiate the response line
        GameObject res = Instantiate(responseLine, msgList.transform);

        if(shake){res.transform.GetChild(0).gameObject.AddComponent<VertexJitter>(); res.transform.GetChild(0).GetComponent<VertexJitter>().CurveScale = 25; res.transform.GetChild(0).GetComponent<TMP_Text>().color = new Color(255, 0, 0, 255);}

        // Set it to the end of all the messages
        res.transform.SetAsLastSibling();

        // Get size of message list, and resize
        Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);

        res.transform.GetChild(0).GetComponent<TMP_Text>().text = "";

        // Set response line text
        TextWriter.AddWriter_Static(res.transform.GetChild(0).GetComponent<TMP_Text>(), interpretation, 0.06f, false, false, WriteDone);

        userInputLine.transform.SetAsLastSibling();
    }

    IEnumerator AddInterpreterLinesTypewrite(List<string> interpretation, bool shake = false)
    {
        for(int i = 0; i < interpretation.Count; i++)
        {
            yield return new WaitUntil(() => canTypeWrite);
            canTypeWrite = false;
            AddInterpreterLineTypewrite(interpretation[i], shake);
            userInputLine.transform.SetAsLastSibling();
        }
    }

    void WriteDone(){

        canTypeWrite = true;
    }

    public void Clear(){

        foreach(Transform child in msgList.transform){
            
            if(child == userInputLine.transform){continue;}
            Destroy(child.transform.gameObject);
        }
        msgList.GetComponent<RectTransform>().sizeDelta = baseMsgListSizeDelta;
    }

    public void ScrollToBottom(int lines){

        if(lines > 4){

            sr.velocity = new Vector2(0, 5000);
        }
        else{

            sr.verticalNormalizedPosition = 0;
        }
    }
}
