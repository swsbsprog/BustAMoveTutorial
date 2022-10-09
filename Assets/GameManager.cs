using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake() => instance = this;
    Dictionary<Vector2Int, Bubble> posMap = new();
    HashSet<Vector2Int> checkedCoord = new();
    HashSet<Bubble> sameBlockSet = new();
    int minY = int.MaxValue;
    int maxY = int.MinValue;
    int minX = int.MaxValue;
    int maxX = int.MinValue;
    public void AddBlock(Bubble bubble, Vector2Int coord)
    {
        posMap[coord] = bubble;
        minX = Math.Min(minX, coord.x);
        maxX = Math.Max(maxX, coord.x);
        minY = Math.Min(minY, coord.y);
        maxY = Math.Max(maxY, coord.y);
    }
    public void DestroyMatch3(ColorType colorType, Vector2Int coord)
    {
        CheckNearSameBubble(colorType, coord);

        if (sameBlockSet.Count >= 3)
        {
            foreach (var item in sameBlockSet)
            {
                posMap.Remove(item.coord);
                Destroy(item.gameObject);
            }
        }

        checkedCoord.Clear();
        sameBlockSet.Clear();

        // 천장에 붙은게 없는 버블도 같이 터트리기
        DestroyIsolation();
    }

    private void DestroyIsolation()
    {
        //천장에 붙어 있는거랑 연결된게 없으면 하늘에 뜬 블럭.
        HashSet<Vector2Int> aliveBlockSet = new();
        HashSet<Vector2Int> isolateBlockSet = new();

        for (int x = minX; x < maxX; x++)
        {
            var topCoord = new Vector2Int(x, minY);
            if(posMap.ContainsKey(topCoord))
                aliveBlockSet.Add(topCoord);
        }

        for (int y = minY + 1; y < maxY; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                var checkCoord = new Vector2Int(x, y);
                if (posMap.ContainsKey(checkCoord) == false)
                    continue;

                bool isAliveBlock = IsAliveBlock(y, checkCoord, aliveBlockSet);
                if(isAliveBlock)
                    aliveBlockSet.Add(checkCoord);
                else
                    isolateBlockSet.Add(checkCoord);
            }
        }

        foreach (var item in isolateBlockSet)
        {
            Destroy(posMap[item]);
            posMap.Remove(item);
        }
    }

    private bool IsAliveBlock(int y, Vector2Int checkCoord, HashSet<Vector2Int> aliveBlockSet)
    {
        if (aliveBlockSet.Contains(checkCoord + new Vector2Int(0, 1)))
            return true;

        if (y % 2 == 0) //y가 짝수 일땐 x-1
        {
            if (aliveBlockSet.Contains(checkCoord + new Vector2Int(-1, 1)))
                return true; 
        }
        else //y가 홀 수 일땐 x+1
        {
            if (aliveBlockSet.Contains(checkCoord + new Vector2Int(1, 1)))
                return true;
        }
        return false;
    }

    private void CheckNearSameBubble(ColorType colorType, Vector2Int coord)
    {
        AddSameBubble(colorType, coord,-1, 0); // 왼쪽
        AddSameBubble(colorType, coord, 1, 0); // 오른쪽
        AddSameBubble(colorType, coord, 0, 1); // 위
        AddSameBubble(colorType, coord, 0,-1); // 아래
        //y가 짝 수 일때
        if (coord.y % 2 == 0)
        {
            AddSameBubble(colorType, coord, -1, 1);
            AddSameBubble(colorType, coord, -1, -1);
        }
        else //y가 홀 수 일때
        {
            AddSameBubble(colorType, coord, 1, 1);
            AddSameBubble(colorType, coord, 1, -1);
        }
    }

    private void AddSameBubble(ColorType colorType, Vector2Int targetCoord, int x, int y)
    {
        var coord = new Vector2Int(x, y) + targetCoord;
        bool isSame = IsSame(colorType, coord);
        if (isSame)
        {
            sameBlockSet.Add(posMap[coord]);
            CheckNearSameBubble(colorType, coord);
        }
    }

    private bool IsSame(ColorType colorType, Vector2Int coord)
    {
        if (posMap.ContainsKey(coord) == false)
            return false;
        if (checkedCoord.Contains(coord))
            return false;
        checkedCoord.Add(coord);
        return posMap[coord].type == colorType;
    }
}
