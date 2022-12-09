using UnityEngine;

public interface IRoom
{
    public Vector3Int Index3D { get; set; }
    public void SetName(string name);
    public void Reset();
    public void Disable();
    public void RemoveWallWithNeighbour(int neighbourX, int neighbourY, int neighbourZ);
    public bool IsActive { get; }

}