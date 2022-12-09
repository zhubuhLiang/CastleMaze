using UnityEngine;

public class Materials : MonoBehaviour
{
    public static Sprite RoomSprite;
    public static Material Wall;

    private void Awake()
    {
        RoomSprite = Resources.Load<Sprite>("RoomSquare");
        Wall = Resources.Load<Material>("Wall");
    }
}
