using UnityEngine;

public class Room : MonoBehaviour, IRoom
{
    public Vector3Int Index3D { get; set; }
    public bool IsActive { get { return gameObject.activeSelf; } }

    private SpriteRenderer spriteRenderer;
    [SerializeField] private LineRenderer NorthWall;
    [SerializeField] private LineRenderer SouthWall;
    [SerializeField] private LineRenderer EastWall;
    [SerializeField] private LineRenderer WestWall;
    [SerializeField] private SpriteRenderer UpStair;
    [SerializeField] private SpriteRenderer DownStair;


    void Awake()  
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.enabled)
        {
            spriteRenderer.sprite = Materials.RoomSprite;
            spriteRenderer.material = Materials.Wall;
        }

        NorthWall.material = Materials.Wall;
        SouthWall.material = Materials.Wall;
        EastWall.material = Materials.Wall;
        WestWall.material = Materials.Wall;

        gameObject.SetActive(false);
    }



    public void SetName(string name) => gameObject.name = name;

    public void Disable() => gameObject.SetActive(false);

    public void Reset() {
        gameObject.SetActive(true);
        if (spriteRenderer != null) spriteRenderer.material = Materials.Wall;
        UpStair.gameObject.SetActive(false);
        DownStair.gameObject.SetActive(false);
    }


    public void RemoveWallWithNeighbour(int x, int y, int z) {

        if (Index3D.x < x) EastWall.gameObject.SetActive(false);
        else if (Index3D.x > x) WestWall.gameObject.SetActive(false);
        else if (Index3D.y < y) NorthWall.gameObject.SetActive(false);
        else if (Index3D.y > y) SouthWall.gameObject.SetActive(false);
        else if (Index3D.z < z)
        {
            UpStair.gameObject.SetActive(true);
        }
        else DownStair.gameObject.SetActive(true);
        spriteRenderer.enabled = false;
    }

}