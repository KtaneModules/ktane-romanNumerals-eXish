using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using System;

public class NumeralsScript : MonoBehaviour {

    public KMAudio audio;
    public KMBombInfo bomb;

    public KMSelectable[] buttons;
    public TextMesh numeralDisp;
    public TextMesh inputDisp;

    private int ans;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        GetComponent<KMNeedyModule>().OnNeedyActivation += OnNeedyActivation;
        GetComponent<KMNeedyModule>().OnTimerExpired += OnTimerExpired;
        foreach (KMSelectable obj in buttons){
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
        bomb.OnBombExploded += OnExplode;
    }

    void Start () {
        ans = 0;
        numeralDisp.text = "";
        inputDisp.text = "";
        Debug.LogFormat("[Roman Numerals #{0}] Needy Roman Numerals has loaded! Waiting for first activation...", moduleId);
    }

    void OnExplode()
    {
        bombSolved = true;
    }

    void PressButton(KMSelectable pressed)
    {
        if (moduleSolved == true)
        {
            if(inputDisp.text.Length > 3)
            {
                if (pressed == buttons[11])
                {
                    pressed.AddInteractionPunch(0.25f);
                    audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
                    inputDisp.text = "";
                }
                else if (pressed == buttons[7])
                {
                    pressed.AddInteractionPunch(0.25f);
                    audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
                    if (inputDisp.text.Length > 0)
                    {
                        inputDisp.text = inputDisp.text.Substring(0, inputDisp.text.Length - 1);
                    }
                }
            }
            else
            {
                pressed.AddInteractionPunch(0.25f);
                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, pressed.transform);
                if (pressed == buttons[11])
                {
                    inputDisp.text = "";
                }
                else if (pressed == buttons[7])
                {
                    if (inputDisp.text.Length > 0)
                    {
                        inputDisp.text = inputDisp.text.Substring(0, inputDisp.text.Length - 1);
                    }
                }
                else if (pressed == buttons[0])
                {
                    inputDisp.text += "1";
                }
                else if (pressed == buttons[1])
                {
                    inputDisp.text += "2";
                }
                else if (pressed == buttons[2])
                {
                    inputDisp.text += "3";
                }
                else if (pressed == buttons[3])
                {
                    inputDisp.text += "4";
                }
                else if (pressed == buttons[4])
                {
                    inputDisp.text += "5";
                }
                else if (pressed == buttons[5])
                {
                    inputDisp.text += "6";
                }
                else if (pressed == buttons[6])
                {
                    inputDisp.text += "7";
                }
                else if (pressed == buttons[8])
                {
                    inputDisp.text += "8";
                }
                else if (pressed == buttons[9])
                {
                    inputDisp.text += "9";
                }
                else if (pressed == buttons[10])
                {
                    inputDisp.text += "0";
                }
            }
        }
    }

    protected void OnNeedyActivation()
    {
        ans = UnityEngine.Random.Range(1, 4000);
        numeralDisp.text = ToRoman(ans);
        Debug.LogFormat("[Roman Numerals #{0}] The module has activated! The displayed Numeral is: {1}", moduleId, numeralDisp.text);
        Debug.LogFormat("[Roman Numerals #{0}] The submitted arabic number should be {1}", moduleId, ans);
        moduleSolved = true;
    }

    protected void OnTimerExpired()
    {
        string check = "" + ans;
        if (check.Equals(inputDisp.text))
        {
            GetComponent<KMNeedyModule>().HandlePass();
            Debug.LogFormat("[Roman Numerals #{0}] That number was correct! Module temporarily neutralized! Waiting for next activation...", moduleId);
        }
        else
        {
            GetComponent<KMNeedyModule>().HandleStrike();
            Debug.LogFormat("[Roman Numerals #{0}] Incorrect number! Strike! Waiting for next activation...", moduleId);
        }
        ans = 0;
        numeralDisp.text = "";
        inputDisp.text = "";
        moduleSolved = false;
    }

    private string ToRoman(int number)
    {
        if (number < 1) return string.Empty;
        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900);
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        if (number >= 1) return "I" + ToRoman(number - 1);
        throw new ArgumentOutOfRangeException("something went wrong...");
    }

    //twitch plays
    private bool bombSolved = false;
    private bool inputIsValid(string s)
    {
        int temp = 0;
        bool check = int.TryParse(s, out temp);
        if(check == true)
        {
            if(temp >= 1 && temp <= 3999)
            {
                return true;
            }
        }
        return false;
    }

    #pragma warning disable 414
    //private readonly string TwitchHelpMessage = @"!{0} enter <#> [Enters the given number] | !{0} num [Outputs the currently displayed numeral to chat]";
    private readonly string TwitchHelpMessage = @"!{0} enter <#> [Enters the given number]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        /**if (Regex.IsMatch(command, @"^\s*num\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(command, @"^\s*numeral\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if(moduleSolved == true) yield return "sendtochat Roman Numerals: The displayed numeral is " + numeralDisp.text;
            else yield return "sendtochat Roman Numerals: I am currently not active!";
            yield break;
        }*/
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*enter\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if(parameters.Length == 2)
            {
                if (inputIsValid(parameters[1]))
                {
                    yield return null;
                    buttons[11].OnInteract();
                    char[] integers = parameters[1].ToCharArray();
                    foreach (char num in integers)
                    {
                        if (num.Equals('0'))
                        {
                            buttons[10].OnInteract();
                        }
                        else if (num.Equals('1'))
                        {
                            buttons[0].OnInteract();
                        }
                        else if (num.Equals('2'))
                        {
                            buttons[1].OnInteract();
                        }
                        else if (num.Equals('3'))
                        {
                            buttons[2].OnInteract();
                        }
                        else if (num.Equals('4'))
                        {
                            buttons[3].OnInteract();
                        }
                        else if (num.Equals('5'))
                        {
                            buttons[4].OnInteract();
                        }
                        else if (num.Equals('6'))
                        {
                            buttons[5].OnInteract();
                        }
                        else if (num.Equals('7'))
                        {
                            buttons[6].OnInteract();
                        }
                        else if (num.Equals('8'))
                        {
                            buttons[8].OnInteract();
                        }
                        else if (num.Equals('9'))
                        {
                            buttons[9].OnInteract();
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                else
                {
                    yield return "sendtochaterror The specified number to enter '" + parameters[1] + "' is invalid!";
                }
            }
            else if(parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if(parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify a number to enter!";
            }
            yield break;
        }
    }

    void TwitchHandleForcedSolve()
    {
        //The code is done in a coroutine instead of here so that if the solvebomb command was executed this will just input the number right when it activates and it wont wait for its turn in the queue
        StartCoroutine(DealWithNeedy());
    }

    private IEnumerator DealWithNeedy()
    {
        while (!bombSolved)
        {
            while (numeralDisp.text.Equals("")) { yield return new WaitForSeconds(0.1f); }
            yield return ProcessTwitchCommand("enter "+ans);
            while (!numeralDisp.text.Equals("")) { yield return new WaitForSeconds(0.1f); }
        }
    }
}
