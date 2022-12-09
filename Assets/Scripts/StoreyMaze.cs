using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class StoreyMaze : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI widthText;
    public TextMeshProUGUI heightText;
    public Slider widthSlider;
    public Slider heightSlider;
    public Button generateButton;
    public TMP_Dropdown storeyDropDown;
    public Toggle unevenToggle;
    public Image StairInstruct;
    public Transform cam;

    [Header("Input")]
    public GameObject CellPrefab;

    private static int MAZE_WIDTH;
    private static int MAZE_HEIGHT;
    private const int MIN_SIZE = 10;
    private const int MAX_SIZE = 250; 

    private IRoom[,,] grid;
    private GameObject Container;
    private int widthInput;
    private int heightInput;
    private int storeyInput;
    private bool unevenSized;


    void Start()
    {
        // Create the parent of generated rooms.
        Container = new GameObject("Container");
        Container.transform.parent = transform;

        // Add Listeners to UI.
        generateButton.onClick.AddListener(GenerateMaze);
        widthSlider.onValueChanged.AddListener(OnWidthValueChanged);
        heightSlider.onValueChanged.AddListener(OnHeightValueChanged);
        storeyDropDown.onValueChanged.AddListener(OnStoreyNumberChanged);
        unevenToggle.onValueChanged.AddListener(OnUnevenValueChanged);

        // Set Initial value of maze width, height and storeys.
        OnWidthValueChanged(10);
        OnHeightValueChanged(10);
        OnStoreyNumberChanged(0);
    }


    private void GenerateMaze()
    {
        StopAllCoroutines();
        EmptyContainer();
        grid = GenerateRooms(CellPrefab, Container.transform);
        StoreyBackTracker.Run(grid, widthInput, heightInput, storeyInput, unevenSized);
        UpdateCameraPosition();
        SwitchStairSign();
    }


    void EmptyContainer()
    {
        foreach (Transform child in Container.transform)
        {
            Destroy(child.gameObject);
        }
    }


    private IRoom[,,] GenerateRooms(GameObject cellPrefab, Transform parent)
    {
        IRoom[,,] grid = new IRoom[widthInput, heightInput, storeyInput]; 

        for(int z=0; z< storeyInput; z++)   
            for (int y = 0; y < heightInput; y++)
                for (int x = 0; x < widthInput; x++)
                {
                    GameObject cellGO = Instantiate(cellPrefab, new Vector3(x + 0.5f + ((z - storeyInput/2f) * (widthInput *1.1f)), y + 0.5f, 0), Quaternion.identity, parent);
                    IRoom room = cellGO.GetComponent<IRoom>();
                    room.Index3D = new Vector3Int(x, y, z);
                    room.SetName($"{x + 1}:{y + 1}:{z + 1}");
                    grid[x, y, z]= room;
                }
        return grid;
    }


    public void UpdateCameraPosition()
    {
        float zDistance;
        float mapWidth = MAZE_WIDTH * 1.1f * storeyInput;
        if (mapWidth > 2 * MAZE_HEIGHT)
            zDistance = -1.2f * mapWidth / Screen.width * Screen.height;
        else
            zDistance = -1.2f * MAZE_HEIGHT;

        cam.position = new Vector3(0f, (float)MAZE_HEIGHT / 2, zDistance);
    }


    #region UI Event Listeners

    private void OnWidthValueChanged(float width)
    {
        //if (width == string.Empty) width = MIN_SIZE.ToString();

        int w = (int) width;
        if (w < MIN_SIZE || w > MAX_SIZE)
        {
            widthInput = Mathf.Clamp(w, MIN_SIZE, MAX_SIZE);            
        }
        else widthInput = w;
        widthText.text = "Width: " + widthInput.ToString();

        MAZE_WIDTH = widthInput;
    }

    private void OnHeightValueChanged(float height)
    {

        int h = (int)height;
        if (h < MIN_SIZE || h > MAX_SIZE)
        {
            heightInput = Mathf.Clamp(h, MIN_SIZE, MAX_SIZE);

        }
        else heightInput = h;
        heightText.text = "Height: " + heightInput.ToString();

        MAZE_HEIGHT = heightInput;
    }


    private void OnUnevenValueChanged(bool uneven)
    {
        unevenSized = uneven;
    }


    private void OnStoreyNumberChanged(int idx)
    {
        if(idx >0)
        {
            widthSlider.maxValue = 100;
            heightSlider.maxValue = 100;
        }
        else
        {
            widthSlider.maxValue = 250;
            heightSlider.maxValue = 250;
        }

        storeyInput = idx + 1;
    }

    private void SwitchStairSign()
    {
        if(storeyInput > 1)
            StairInstruct.gameObject.SetActive(true);
        else
            StairInstruct.gameObject.SetActive(false);
    }



    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
						 Application.Quit();
#endif
    }

    #endregion
}

