using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager instance;
    public Tilemap tilemap;
    private void Awake() => instance = this;

    public Vector3Int WorldToCell(Vector3 pos)
    {
        pos.z = 0;
        Vector3Int worldPos = tilemap.WorldToCell(pos);
        return worldPos;
    }
    public Vector2 GetCellCenterWorld(Vector3Int pos)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
        return worldPos;
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var coord = WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            print($"Click:{Input.mousePosition}, coord:{coord}");
        }
    }
}
