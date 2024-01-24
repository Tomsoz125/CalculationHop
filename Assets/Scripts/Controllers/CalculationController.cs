using System.Collections;
using System.Collections.Generic;
using System.Data;
using System;
using UnityEngine;
using TMPro;

public class CalculationController : MonoBehaviour
{
    private string[] op;
    private string equation;
    private string answer;
    [SerializeField] private TMP_Text display;
    [SerializeField] private TMP_InputField input;

    public bool canJump = false;

    void Awake() {
        Init();
    }

    void Update() {
        string validated = "";
        for (int i = 0; i < input.text.Length; i++) {
            if (Char.IsDigit(input.text, i)) validated += input.text[i];
        }

        if (validated == answer || canJump) {
            canJump = true;
            input.text = "";
            validated = "";
            display.text = "Correct!";
        }

        if (display.text != equation + validated) {
            display.text = equation + validated;
            return;
        }
    }

    public void Init() {
        canJump = false;

        Debug.Log("init");
        switch (PlayerPrefs.GetString("operator")) {
            case "ADD":
                op = new string[]{"+", "+"};
                break;
            case "SUBTRACT":
                op = new string[]{"-", "-"};
                break;
            case "MULTIPLY":
                op = new string[]{"x", "*"};
                break;
            case "DIVIDE":
                op = new string[]{"÷", "/"};
                break;
            default:
                op = new string[]{"+", "+"};
                break;
        }

        int num1 = new System.Random().Next(1, 100);
        int num2 = new System.Random().Next(1, 100);
        equation = num1 + " " + op[0] + " " + num2 + " = ";
        DataTable dt = new DataTable();
        answer = dt.Compute(num1+op[1]+num2, "").ToString();
        Debug.Log(equation + answer);
    }
}
