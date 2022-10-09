using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ColorType
{
    Red,
    Blue,
}
public class Bubble : MonoBehaviour
{
    public ColorType type;

    public Rigidbody2D rb;
    public Collider2D col;
    public float speed = 10;
    public bool isOnBoard = true;
    public bool isMoving = false;
    public Vector2Int coord;
    private void Awake(){
        rb = GetComponentInChildren<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    private void Start()
    {
        if(isOnBoard)
            MoveToSnap();
    }

    private void MoveToSnap()
    {
        Vector3Int coord3D = TilemapManager.instance.WorldToCell(transform.position);
        transform.SetParent(TilemapManager.instance.transform);
        transform.position = TilemapManager.instance.GetCellCenterWorld(coord3D);
        print($"pos:{transform.position}, coord:{coord3D}");
        GetComponentInChildren<TextMesh>(true).text = $"{coord3D.x + 5}, {coord3D.y}";
        coord = new Vector2Int(coord3D.x, coord3D.y);
        GameManager.instance.AddBlock(this, coord);
    }

    public ForceMode2D forceMode = ForceMode2D.Force;   
    [ContextMenu("Fire")]
    internal void Fire(Vector2 direction)
    { 
        if(rb == null)
            return;

        isMoving = true;
        col.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(direction * speed, forceMode);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isMoving == false)
            return;

        if (collision.collider.CompareTag("Bubble") == false)
            return;
        isMoving = false;
        rb.bodyType = RigidbodyType2D.Static;
        MoveToSnap();

        // 3개이상일때 부스기
        GameManager.instance.DestroyMatch3(type, coord);
    }
}
