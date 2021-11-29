using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using UnityEngine.UI;

public class LSystemsGenerator : MonoBehaviour
{
    public static int NUM_OF_TREES = 8;
    public static int MAX_ITERATIONS = 7;

    public int title = 1;
    public int iterations = 4;
    public float angle = 30f;
    public float width = 1f;
    public float length = 2f;
    public float variance = 10f;
    public bool hasTreeChanged = false;
    public GameObject Tree = null;

    [SerializeField] private GameObject treeParent;
    [SerializeField] private GameObject branch;
    [SerializeField] private GameObject leaf;
    [SerializeField] private HUDScript HUD;

    private const string axiom = "X";

    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;
    private int titleLastFrame;
    private int iterationsLastFrame;
    private float angleLastFrame;
    private float widthLastFrame;
    private float lengthLastFrame;
    private string currentString = string.Empty;
    private Vector3 initialPosition = Vector3.zero;
    private float[] randomRotationValues = new float[100];

    [SerializeField] private InputField title_text;
    [SerializeField] private InputField iterations_text;
    [SerializeField] private InputField angle_text;
    [SerializeField] private Text rule1;
    [SerializeField] private Text rule2;

    private KeyCode keyCode;

    public HUDScript script;

    private void Start()
    {
        titleLastFrame = title;
        iterationsLastFrame = iterations;
        angleLastFrame = angle;
        widthLastFrame = width;
        lengthLastFrame = length;

        for (int i = 0; i < randomRotationValues.Length; i++)
        {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }

        transformStack = new Stack<TransformInfo>();

        rules = new Dictionary<char, string>
        {
            { 'X', "[F-[[X]+X]+F[+FX]-X]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void Update()
    {
        if (HUD.hasGenerateBeenPressed || Input.GetKeyDown(KeyCode.G))
        {
            ResetRandomValues();
            HUD.hasGenerateBeenPressed = false;
            Generate();
        }

        if (HUD.hasResetBeenPressed || Input.GetKeyDown(KeyCode.R))
        {
            ResetTreeValues();
            HUD.hasResetBeenPressed = false;
            HUD.Start();
            Generate();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectTreeOne();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectTreeTwo();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectTreeThree();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SelectTreeFour();

        if (Input.GetKeyDown(KeyCode.Alpha5))
            SelectTreeFive();

        if (Input.GetKeyDown(KeyCode.Alpha6))
            SelectTreeSix();

        if (Input.GetKeyDown(KeyCode.Alpha7))
            SelectTreeSeven();

        if (Input.GetKeyDown(KeyCode.Alpha8))
            SelectTreeEight();



        if (Input.GetKeyDown(KeyCode.UpArrow))
            script.IterationsUp();

        if (Input.GetKeyDown(KeyCode.DownArrow))
            script.IterationsDown();

        if (Input.GetKey(KeyCode.LeftArrow))
            script.AngleDown();

        if (Input.GetKey(KeyCode.RightArrow))
            script.AngleUp();

        if (Input.GetKeyDown(KeyCode.LeftBracket))
            script.VarianceDown();

        if (Input.GetKeyDown(KeyCode.RightBracket))
            script.VarianceUp();


        if (titleLastFrame != title)
        {

            switch (title)
            {
                case 1:
                    SelectTreeOne();
                    break;

                case 2:
                    SelectTreeTwo();
                    break;

                case 3:
                    SelectTreeThree();
                    break;

                case 4:
                    SelectTreeFour();
                    break;

                case 5:
                    SelectTreeFive();
                    break;

                case 6:
                    SelectTreeSix();
                    break;

                case 7:
                    SelectTreeSeven();
                    break;

                case 8:
                    SelectTreeEight();
                    break;

                default:
                    SelectTreeOne();
                    break;
            }

            titleLastFrame = title;
        }

        if (iterationsLastFrame != iterations)
        {
            if (iterations >= 6)
            {
                HUD.warning.gameObject.SetActive(true);
                StopCoroutine("TextFade");
                StartCoroutine("TextFade");
            }
            else
            {
                HUD.warning.gameObject.SetActive(false);
            }
        }

        if (iterationsLastFrame != iterations ||
                angleLastFrame  != angle ||
                widthLastFrame  != width ||
                lengthLastFrame != length)
        {
            ResetFlags();
            Generate();
        }

    }

    private void Generate()
    {
        //Destroy(Tree);
        if(Tree != null)
        {
            Debug.Log("TREE: " + Tree);
            Destroy(Tree);
        }

        Tree = Instantiate(treeParent);

        currentString = axiom;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }

            currentString = sb.ToString();
            sb = new StringBuilder();
        }

        Debug.Log(currentString);
        
        for (int i = 0; i < currentString.Length; i++)
        {
            switch (currentString[i])
            {
                case 'F':                    
                    initialPosition = transform.position;
                    transform.Translate(Vector3.up * 2 * length);                    

                    GameObject fLine = currentString[(i + 1) % currentString.Length] == 'X' || currentString[(i + 3) % currentString.Length] == 'F' && currentString[(i + 4) % currentString.Length] == 'X' ? Instantiate(leaf) : Instantiate(branch);
                    fLine.transform.SetParent(Tree.transform);
                    fLine.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    fLine.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    fLine.GetComponent<LineRenderer>().startWidth = width;
                    fLine.GetComponent<LineRenderer>().endWidth = width;
                    break;

                case 'X':                
                    break;

                case '+':
                    transform.Rotate(Vector3.back * angle * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '-':                                      
                    transform.Rotate(Vector3.forward * angle * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '*':
                    transform.Rotate(Vector3.up * 120 * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '/':
                    transform.Rotate(Vector3.down* 120 * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '[':
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;

                case ']':
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;

                default:
                    throw new InvalidOperationException("Invalid L-tree operation");
            }
        }
    }

    private void ChangeUIText()
    {
        title_text.text = title.ToString();
        iterations_text.text = iterations.ToString();
        angle_text.text = angle.ToString();

        rule1.text = "X -> " + rules['X'].ToString();
        rule2.text = "F -> " + rules['F'].ToString();
    }

    private void SelectTreeOne()
    {
        title = 1;
        iterations = 5;
        angle = 25.7f;

        rules = new Dictionary<char, string>
        {
            { 'X', "[F-[X+X]+F[+FX]-X]" },
            { 'F', "FF" }
        };

        Generate();
        ChangeUIText();
    }

    private void SelectTreeTwo()
    {
        title = 2;
        iterations = 5;
        angle = 20f;

        rules = new Dictionary<char, string>
        {
            { 'X', "[-FX][+FX][FX]" },
            { 'F', "FF" }
        };

        Generate();
        ChangeUIText();
    }

    private void SelectTreeThree()
    {
        title = 3;
        iterations = 4;
        angle = 22.5f;

        rules = new Dictionary<char, string>
        {
            { 'X', "[-FX]X[+FX][+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
        ChangeUIText();
    }

    private void SelectTreeFour()
    {
        title = 4;
        iterations = 4;
        angle = 20f;

        rules = new Dictionary<char, string>
        {
            { 'X', "[FF[+XF-F+FX]--F+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
        ChangeUIText();
    }

    private void SelectTreeFive()
    {
        title = 5;
        iterations = 5;
        angle = 25.7f;

        rules = new Dictionary<char, string>
        {
            { 'X', "[FX[+F[-FX]FX][-F-FXFX]]" },
            { 'F', "FF" }
        };

        Generate();
        ChangeUIText();
    }

    private void SelectTreeSix()
    {
        title = 6;
        iterations = 5;
        angle = 22.5f;

        rules = new Dictionary<char, string>
        {
            { 'X', "[F[+FX][*+FX][/+FX]]" },
            { 'F', "FF" }
        };

        Generate();
        ChangeUIText();
    }

    private void SelectTreeSeven()
    {
        title = 7;
        iterations = 6;
        angle = 20f;

        rules = new Dictionary<char, string>
        {
            { 'X', "[*+FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
        ChangeUIText();
    }

    private void SelectTreeEight()
    {
        title = 8;
        iterations = 5;
        angle = 20f;

        rules = new Dictionary<char, string>
        {
            { 'X', "[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]" },
            { 'F', "FF" }
        };

        Generate();
        ChangeUIText();
    }

    private void ResetRandomValues()
    {
        for (int i = 0; i < randomRotationValues.Length; i++)
        {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    private void ResetFlags()
    {
        iterationsLastFrame = iterations;
        angleLastFrame = angle;
        widthLastFrame = width;
        lengthLastFrame = length;
    }

    private void ResetTreeValues()
    {
        iterations = 4;
        angle = 30f;
        width = 1f;
        length = 2f;
        variance = 10f;
    }

    IEnumerator TextFade()
    {
        Color c = HUD.warning.color;

        float TOTAL_TIME = 4f;
        float FADE_DURATION = .25f;

        for (float timer = 0f; timer <= TOTAL_TIME; timer += Time.deltaTime)
        {
            if (timer > TOTAL_TIME - FADE_DURATION)
            {
                c.a = (TOTAL_TIME - timer) / FADE_DURATION;
            }
            else if (timer > FADE_DURATION)
            {
                c.a = 1f;
            }
            else
            {
                c.a = timer / FADE_DURATION;
            }

            HUD.warning.color = c;

            yield return null;
        }
    }
}