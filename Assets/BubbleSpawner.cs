using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public List<GameObject> bubbles;
    public Bubble currentBubble;
    public Bubble nextBubble;
    public Transform fireTr;
    public Transform nextBubbleTr;
    float currentX;
    public float rotateSpeed = 100f;
    public float maxAngle = 70;

    public int randomSeed;
    private void Start()
    {
        UnityEngine.Random.InitState(randomSeed);
        currentBubble = Instantiate(GetNextBubble(), fireTr.position, Quaternion.identity);
        nextBubble = Instantiate(GetNextBubble(), nextBubbleTr.position, Quaternion.identity);
        currentBubble.col.enabled = false;
        nextBubble.col.enabled = false;
        currentBubble.isOnBoard = false;
        nextBubble.isOnBoard = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
            currentX = Mathf.Max(-maxAngle, currentX - rotateSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            currentX = Mathf.Min(+maxAngle, currentX + rotateSpeed * Time.deltaTime);

        fireTr.rotation = Quaternion.Euler(0, 0, currentX);

        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(FireBubbleCo());
    }

    public Vector2 direction = new(0, 1);
    private IEnumerator FireBubbleCo()
    {
        //currentBubble 에 물리 힘주기. -> 발사.
        currentBubble.transform.rotation = fireTr.rotation;
        var angle = -currentBubble.transform.rotation.eulerAngles.z;
        direction = new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
        currentBubble.Fire(direction);
        yield return new WaitForSeconds(0.5f);
        // next에 있는걸 current로 옮기기
        currentBubble = nextBubble;
        currentBubble.transform.position = fireTr.position;

        // nextBubble생성
        var newBubble = GetNextBubble();
        nextBubble = Instantiate(newBubble, nextBubbleTr.position, Quaternion.identity);
        nextBubble.col.enabled = false;
        nextBubble.isOnBoard = false;
    }
    Bubble GetNextBubble() => bubbles.OrderBy(x => UnityEngine.Random.Range(0, 1f)).First().GetComponentInChildren<Bubble>();

}
