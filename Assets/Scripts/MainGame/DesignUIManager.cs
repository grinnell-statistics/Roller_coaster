using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;


public class DesignUIManager : MonoBehaviour
{
    public GraphChartRenderer graphChartRenderer;

    //------------------------------| START: UI Components Stuff |---------------------------------
    // UI Text for displaying validation messages
    public TextMeshProUGUI validationText; // Reference to the UI Text component for displaying validation result
    public TextMeshProUGUI errorText; // Display warning invalid inputs

    // Buttons
    public Button drawTrackButton; // Button to draw the track
    public Button checkMathButton; // Button to check validity of the design
    public Button nextSceneButton; // Button to move to roller coaster scene

    // Function fields
    public TMP_InputField inputA1, inputA2, inputA3; // Input field for constant terms A
    public TMP_InputField inputB1, inputB2, inputB3; // Input field for coefficient B (x)
    public TMP_InputField inputC1, inputC2, inputC3; // Input field for coefficient C (x^2)
    public TMP_InputField inputXEnd1, inputXEnd2, inputXEnd3; // Input field for x end
    public TextMeshProUGUI xStart2, xStart3; // Text for x start

    // Support 5 functions and cubic functions
#nullable enable
    public TMP_InputField? inputA4, inputA5;
    public TMP_InputField? inputB4, inputB5;
    public TMP_InputField? inputC4, inputC5;
    public TMP_InputField? inputD1, inputD2, inputD3, inputD4, inputD5;
    public TMP_InputField? inputXEnd4, inputXEnd5;
    public TextMeshProUGUI? xStart4, xStart5;
    public Button? resetAllFunctionsButton; // Button to empty all inputs of the functions
#nullable disable

    //------------------------------| END: UI Components stuff |-----------------------------------

    private string validNumberRegex = @"^[+-]?0$|^[+-]?[1-9]\d*$|^[+-]?\.\d+$|^[+-]?0\.\d*$|^[+-]?[1-9]\d*\.\d*$"; // Valid number format
    private string validNumberRegex0to100 = @"^\+?0$|^\+?[1-9]\d{0,1}$|^\+?\.\d+$|^\+?0\.\d*$|^\+?[1-9]\d{0,1}\.\d*$|^\+?100(.0*)?$"; // Valid 0-100 number format

    //------------------------------| START: Level Config Stuff |----------------------------------
    // Current level config read from segments manager
    private LevelConfig currentLevelConfig;

    // current level
    private int level;

    // RULE: Level 1 is 50, Level 2 is 100
    private float targetLength;

    // maximum number of segments allowed of the whole game
    private int totalNumSegs;

    // does it support cubic function
    private bool isCubic;

    private static int currentMaxTotalNumSegs = 5;
    //------------------------------| END: Level Config stuff |-------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        currentLevelConfig = SegmentsManager.instance.currentLevelConfig;
        level = currentLevelConfig.levelNumber;
        targetLength = currentLevelConfig.maxTotalLength;
        totalNumSegs = currentLevelConfig.maxTotalSegments;
        isCubic = currentLevelConfig.isSupportCubic;

        drawTrackButton.onClick.AddListener(DrawTrack);
        checkMathButton.onClick.AddListener(CheckMath);
        nextSceneButton.onClick.AddListener(NextScene);

        if (resetAllFunctionsButton != null)
        {
            resetAllFunctionsButton.onClick.AddListener(ResetAllFunctions);
        }

        inputA4 = GetInputField("InputA4");
        inputB4 = GetInputField("InputB4");
        inputC4 = GetInputField("InputC4");
        inputD4 = GetInputField("InputD4");

        inputA5 = GetInputField("InputA5");
        inputB5 = GetInputField("InputB5");
        inputC5 = GetInputField("InputC5");
        inputD5 = GetInputField("InputD5");

        Debug.Log($"Auto-assigned InputA4: {inputA4 != null}");
        Debug.Log($"Auto-assigned InputB4: {inputB4 != null}");
        Debug.Log($"Auto-assigned InputC4: {inputC4 != null}");
        Debug.Log($"Auto-assigned InputA4: {inputD4 != null}");
        Debug.Log($"Auto-assigned InputB4: {inputA5 != null}");
        Debug.Log($"Auto-assigned InputC4: {inputB5 != null}");
        Debug.Log($"Auto-assigned InputA4: {inputC5 != null}");
        Debug.Log($"Auto-assigned InputB4: {inputD5 != null}");

        // Attach validation handlers to input fields
        inputXEnd1.onValueChanged.AddListener((value) => UpdateXEnd(1, value));
        inputXEnd2.onValueChanged.AddListener((value) => UpdateXEnd(2, value));
        inputXEnd3.onValueChanged.AddListener((value) => UpdateXEnd(3, value));
        inputXEnd4?.onValueChanged.AddListener((value) => UpdateXEnd(4, value));
        inputXEnd5?.onValueChanged.AddListener((value) => UpdateXEnd(5, value));

        inputA1.onValueChanged.AddListener(ValidateInput);
        inputA2.onValueChanged.AddListener(ValidateInput);
        inputA3.onValueChanged.AddListener(ValidateInput);
        inputA4?.onValueChanged.AddListener(ValidateInput);
        inputA5?.onValueChanged.AddListener(ValidateInput);

        inputB1.onValueChanged.AddListener(ValidateInput);
        inputB2.onValueChanged.AddListener(ValidateInput);
        inputB3.onValueChanged.AddListener(ValidateInput);
        inputB4?.onValueChanged.AddListener(ValidateInput);
        inputB5?.onValueChanged.AddListener(ValidateInput);

        inputC1.onValueChanged.AddListener(ValidateInput);
        inputC2.onValueChanged.AddListener(ValidateInput);
        inputC3.onValueChanged.AddListener(ValidateInput);
        inputC4?.onValueChanged.AddListener(ValidateInput);
        inputC5?.onValueChanged.AddListener(ValidateInput);

        inputD1?.onValueChanged.AddListener(ValidateInput);
        inputD2?.onValueChanged.AddListener(ValidateInput);
        inputD3?.onValueChanged.AddListener(ValidateInput);
        inputD4?.onValueChanged.AddListener(ValidateInput);
        inputD5?.onValueChanged.AddListener(ValidateInput);
    }

    private TMP_InputField GetInputField(string fieldName)
    {
        GameObject fieldObject = GameObject.Find(fieldName);

        if (fieldObject == null)
        {
            Debug.LogError($"Input field '{fieldName}' not found!");
            return null;
        }

        TMP_InputField inputField = fieldObject.GetComponent<TMP_InputField>();

        if (inputField == null)
        {
            Debug.LogError($"'{fieldName}' does not have an InputField component!");
            return null;
        }

        return inputField;
    }

    private void ValidateInput(string input)
    {
        // Allow partial inputs like '+' or '-'
        if (input == "+" || input == "-" || input == "." || input == "+." || input == "-.") return;

        // Allow only valid numbers or reset to blank
        TMP_InputField inputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<TMP_InputField>();
        if (inputField == null) return;

        if (!Regex.IsMatch(input, validNumberRegex))
        {
            Debug.LogWarning("Please enter a valid integer or decimal number!");
            DisplayError("Please enter a valid integer or decimal number!");
            inputField.text = ""; // Reset input field
        }
        else 
        {
            ClearError(); // Delete validation message
        }
    }

    /**
     * Purpose: "Build" button to move to Roller Coaster scene.
     * Current design for Level 1: Have to pass all math checks in order to build.
     */
    private void NextScene()
    {
        if (SegmentsManager.instance.Validate(out string validationMessage))
        {
            Segment[] segments = SegmentsManager.instance.segments;

            for (int i = 0; i < segments.Length; i++)
            {
                Segment segment = i < segments.Length ? segments[i] : null;

                if (segment == null)
                {
                    SetSegmentData(i, 0, 0, 0, 0, 0, level);
                }
                else
                {
                    if (level == 1) {
                        SetSegmentData(i, segment.Func.A, segment.Func.B, segment.Func.C, 0, segment.XEnd, 1);
                    } else
                    {
                        SetSegmentData(i, segment.Func.A, segment.Func.B, segment.Func.C, segment.Func.D, segment.XEnd, level);
                    }
                }
            }
            for (int i = 1; i <= 5; i++)
            {
                Debug.Log($"Segment {i}: A={GetSegmentData(i - 1, "A")}, B={GetSegmentData(i - 1, "B")}, C={GetSegmentData(i - 1, "C")}{(level == 2 ? $", D={GetSegmentData(i - 1, "D")}" : "")}");
            }
            
            SceneManager.LoadScene(SceneNames.RollerCoaster);
        }
        else
        {
            validationText.text = "Your design is not up to basic standards yet!";
        }
    }

    private void SetSegmentData(int index, float a, float b, float c, float d, float xMax, int level)
    {
        switch (index)
        {
            case 0:
                DataManager.gameData.eq1a = a;
                DataManager.gameData.eq1b = b;
                DataManager.gameData.eq1c = c;
                if (level == 2) {
                    DataManager.gameDataLvl2.eq1d = d;
                }
                DataManager.gameData.x1max = xMax;
                break;
            case 1:
                DataManager.gameData.eq2a = a;
                DataManager.gameData.eq2b = b;
                DataManager.gameData.eq2c = c;
                if (level == 2) {
                    DataManager.gameDataLvl2.eq2d = d;
                }
                DataManager.gameData.x2max = xMax;
                break;
            case 2:
                DataManager.gameData.eq3a = a;
                DataManager.gameData.eq3b = b;
                DataManager.gameData.eq3c = c;
                if (level == 2)
                {
                    DataManager.gameDataLvl2.eq3d = d;
                }
                DataManager.gameData.x3max = xMax;
                break;
            case 3:
                DataManager.gameDataLvl2.eq4a = a;
                DataManager.gameDataLvl2.eq4b = b;
                DataManager.gameDataLvl2.eq4c = c;
                DataManager.gameDataLvl2.eq4d = d;
                DataManager.gameDataLvl2.x4max = xMax;
                break;
            case 4:
                DataManager.gameDataLvl2.eq5a = a;
                DataManager.gameDataLvl2.eq5b = b;
                DataManager.gameDataLvl2.eq5c = c;
                DataManager.gameDataLvl2.eq5d = d;
                DataManager.gameDataLvl2.x5max = xMax;
                break;
        }
    }

    private float GetSegmentData(int index, string coeff)
    {
        float value = index switch
        {
            0 => coeff == "A" ? DataManager.gameData.eq1a : coeff == "B" ? DataManager.gameData.eq1b : coeff == "C" ? DataManager.gameData.eq1c : (level == 2 ? DataManager.gameDataLvl2.eq1d : 0),
            1 => coeff == "A" ? DataManager.gameData.eq2a : coeff == "B" ? DataManager.gameData.eq2b : coeff == "C" ? DataManager.gameData.eq2c : (level == 2 ? DataManager.gameDataLvl2.eq2d : 0),
            2 => coeff == "A" ? DataManager.gameData.eq3a : coeff == "B" ? DataManager.gameData.eq3b : coeff == "C" ? DataManager.gameData.eq3c : (level == 2 ? DataManager.gameDataLvl2.eq3d : 0),
            3 => coeff == "A" ? DataManager.gameDataLvl2.eq4a : coeff == "B" ? DataManager.gameDataLvl2.eq4b : coeff == "C" ? DataManager.gameDataLvl2.eq4c : (level == 2 ? DataManager.gameDataLvl2.eq4d : 0),
            4 => coeff == "A" ? DataManager.gameDataLvl2.eq5a : coeff == "B" ? DataManager.gameDataLvl2.eq5b : coeff == "C" ? DataManager.gameDataLvl2.eq5c : (level == 2 ? DataManager.gameDataLvl2.eq5d : 0),
            _ => 0
        };
        return value;
    }

    // Helper function to show error message when x-end input is larger than 50
    private bool MessageWhenRangeLargerThanTargetLength()
    {
        if (Math.Max(Math.Max(ParseInput(inputXEnd1.text), ParseInput(inputXEnd2.text)), ParseInput(inputXEnd3.text)) > targetLength) 
        {
            DisplayError($"Invalid range: Range should not go over {targetLength}.");
            return true;
        }
        return false;
    }

    // Helper function: validate xEnd input and parse only if valid
    private void UpdateXEnd(int index, string newValue)
    {
        // Allow partial inputs like '+' or '-'
        if (newValue == "+" || newValue == "." || newValue == "+.") 
        {
            switch (index) 
            {
                case 1:
                    xStart2.text = newValue;
                    break;
                case 2: 
                    xStart3.text = newValue;
                    break;
                case 3:
                    if (xStart4 != null) xStart4.text = newValue;
                    break;
                case 4: 
                    if (xStart5 != null) xStart5.text = newValue;
                    break;
            }
            return;
        }

        // Allow only valid numbers or reset to blank
        TMP_InputField inputField = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<TMP_InputField>();
        if (inputField == null) return;

        if (!Regex.IsMatch(newValue, validNumberRegex0to100)) 
        {
            DisplayError($"Please enter a valid integer or decimal number between 0 and {targetLength}!");
            switch (index) 
            {
                case 1:
                    xStart2.text = "";
                    inputXEnd1.text = "";
                    break;
                case 2:
                    xStart3.text = "";
                    inputXEnd2.text = "";
                    break;
                case 3:
                    if (xStart4 != null) xStart4.text = "";
                    inputXEnd3.text = "";
                    break;
                case 4:
                    if (xStart5 != null) xStart5.text = "";
                    if (inputXEnd4 != null) inputXEnd4.text = "";
                    break;
                case 5:
                    if (inputXEnd5 != null) inputXEnd5.text = "";
                    break;
            }
            return;
        }

        float xEndValue = ParseInput(newValue);
        switch (index) 
        {
            case 1:
                xStart2.text = newValue;
                break;
            case 2:
                xStart3.text = newValue;
                break;
            case 3:
                if (xStart4 != null) xStart4.text = newValue;
                break;
            case 4:
                if (xStart5 != null) xStart5.text = newValue;
                break;
        }
        ValidateSegmentRange(index, xEndValue);
    }

    // Helper function: Show error message if xStart and xEnd not increasing
    private void ValidateSegmentRange(int index, float xEndValue)
    {
        float xEnd1Value = ParseInput(inputXEnd1.text);
        float xEnd2Value = ParseInput(inputXEnd2.text);
        float xEnd3Value = ParseInput(inputXEnd3.text);

        switch (index) 
        {
            case 1: 
                if (!MessageWhenRangeLargerThanTargetLength()) 
                {
                    if (xEndValue <= 0) DisplayError("Invalid range: First end value must be greater than 0.");
                    else if (xEnd2Value != 0 && xEndValue >= xEnd2Value) DisplayError("Invalid range: Second end value must be greater than the first.");
                    else ClearError();
                }
                break;
            case 2:
                if (!MessageWhenRangeLargerThanTargetLength()) 
                {
                    if (xEndValue <= xEnd1Value) DisplayError("Invalid range: Second end value must be greater than the first.");
                    else if (xEnd3Value != 0 && xEndValue >= xEnd3Value) DisplayError("Invalid range: Third end value must be greater than the second.");
                    else ClearError();
                } 
                break;
            case 3:
                if (!MessageWhenRangeLargerThanTargetLength()) 
                {
                    if (xEndValue <= xEnd2Value) DisplayError("Invalid range: Third end value must be greater than the second.");
                    else if (inputXEnd4 != null && ParseInput(inputXEnd4.text) != 0 && xEndValue >= ParseInput(inputXEnd4.text)) DisplayError("Invalid range: Fourth end value must be greater than the third.");
                    else ClearError();
                } 
                break;
            case 4:
                if (!MessageWhenRangeLargerThanTargetLength()) 
                {
                    if (xEndValue <= xEnd3Value) DisplayError("Invalid range: Fourth end value must be greater than the third.");
                    else if (inputXEnd5 != null && ParseInput(inputXEnd5.text) != 0 && xEndValue >= ParseInput(inputXEnd5.text)) DisplayError("Invalid range: Fifth end value must be greater than the fourth.");
                    else ClearError();
                } 
                break;
            case 5:
                if (!MessageWhenRangeLargerThanTargetLength()) 
                {
                    if (xEndValue <= ParseInput(inputXEnd4.text)) DisplayError("Invalid range: Fifth end value must be greater than the fourth.");
                    else ClearError();
                }   
                break;
        }
    }

    // Upon clicking Build, run checking for validity
    private void CheckMath()
    {
        DataManager.gameData.mathCheck = true;
        Debug.Log("Checking your's design validity...");
        parseAllFunc();
        // Call the ValidateAndDisplay method from SegmentsManager
        SegmentsManager.instance.ValidateAndDisplay();

        // Optionally, display the validation message in the UI
        if (validationText != null)
        {
            validationText.text = SegmentsManager.instance.GetHint(); // This should be the captured validation message
        }
    }

    // Upon clicking Reset, empty all inputs, redraw graph
    private void ResetAllFunctions()
    {
        Debug.Log("Reset Clicked!");
        // Clear inputs in UI
        TMP_InputField[] inputFields = FindObjectsOfType<TMP_InputField>();
        Debug.Log(inputFields.Length);

        foreach (TMP_InputField inputField in inputFields)
        {
            inputField.text = "0";
        }

        Debug.Log("All input fields cleared.");

        xStart2.text = "";
        xStart3.text = "";

        // Clear graphs
        SegmentsManager.instance.ClearAll();
        validationText.text = "";
        graphChartRenderer.DrawTrack();
    }

    private void parseAllFunc()
    {
        SegmentsManager.instance.ClearAll();

        // Arrays of the input fields
        TMP_InputField[] inputA = { inputA1, inputA2, inputA3, inputA4, inputA5 };
        TMP_InputField[] inputB = { inputB1, inputB2, inputB3, inputB4, inputB5 };
        TMP_InputField[] inputC = { inputC1, inputC2, inputC3, inputC4, inputC5 };
        TMP_InputField[] inputXEnd = { inputXEnd1, inputXEnd2, inputXEnd3, inputXEnd4, inputXEnd5 };
        TMP_InputField[] inputD = { inputD1, inputD2, inputD3, inputD4, inputD5 };

        // To handle the start and end values
        float[] xStarts = new float[currentMaxTotalNumSegs];
        float[] xEnds = new float[currentMaxTotalNumSegs];

        // Parse the values for a, b, c, d, and xEnds
        for (int i = 0; i < totalNumSegs; i++)
        {
            float a = ParseInput(inputA[i].text);
            float b = ParseInput(inputB[i].text);
            float c = ParseInput(inputC[i].text);
            float xStart = (i == 0) ? 0f : xEnds[i - 1];  // First segment starts at 0, others use the previous xEnd
            float xEnd = ParseInput(inputXEnd[i].text);

            xStarts[i] = xStart;
            xEnds[i] = xEnd;

            // Check if cubic and parse d values if needed
            float? d = isCubic ? (float?)ParseInput(inputD[i].text) : null;

            // Add the segment
            AddSegment(a, b, c, d, xStart, xEnd, i);
        }
    }

    // Upon clicking Graph, the inputs will be parsed and the segments will be added according to their index
    private void DrawTrack()
    {
        parseAllFunc();
        CheckMath();
        graphChartRenderer.DrawTrack();
        Debug.Log("Drawn!");
    }

    // Parse input to function
    private void AddSegment(float a, float b, float c, float? d, float xStart, float xEnd, int index)
    {
        // Don't add segment if xEnd is 0
        if (xEnd == 0)
        {
            Debug.Log($"Function {index + 1} because xEnd is 0");
            return;
        }

        // Don't add segment if range is invalid
        if (xEnd <= xStart)
        {
            Debug.Log($"Can't add segment {index + 1} because xEnd: {xEnd} <= xStart: {xStart}");
            return;
        }

        // Add the new segment to the segments manager
        try
        {
            // if d is not null then add d to newFunction
            Function newFunction = (d != null) ? new Function(a, b, c, (float)d) : new Function(a, b, c);
            SegmentsManager.instance.SetSegment(index, new Segment(newFunction, xStart, xEnd));
            Debug.Log($"Segment added: {xStart} to {xEnd} with function: {d}x^3 + {c}x^2 + {b}x + {a}");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }


    // Parse input or default to 0 if empty
    float ParseInput(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return 0f;
        }

        if (float.TryParse(input, out float result))
        {
            return result;
        }

        Debug.LogWarning($"Invalid input: {input}. Defaulting to 0.");
        return 0f;
    }

    private void DisplayError(string message)
    {
        if (errorText != null) errorText.text = message;
    }

    private void ClearError()
    {
        if (errorText != null) errorText.text = "";
    }
}
