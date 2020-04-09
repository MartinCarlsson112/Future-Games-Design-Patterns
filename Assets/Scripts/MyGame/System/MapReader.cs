using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapData
{
    private List<int> m_Grid;
    private Vector2Int m_Bounds;
    private List<Vector2Int> m_Accessibles;
    //towers
    //Spawn waves
    public List<int> Grid => m_Grid;
    public Vector2Int Bounds => m_Bounds;

    public List<Vector2Int> Accessibles => m_Accessibles;

    public MapData()
    {
        m_Grid = new List<int>();
        m_Bounds = new Vector2Int();
        m_Accessibles = new List<Vector2Int>();
        //Spawn waves
    }

    public void InitializeBounds(int columns)
    {
        if (columns > 0)
        {
            m_Bounds.x = m_Grid.Count / (columns);
        }
        else
        {
            m_Bounds.x = m_Grid.Count;
        }
        m_Bounds.y = columns;
    }

    public void InitializeAccessibles()
    {
        for (int j = 0; j < m_Bounds.y; j++)
        {
            for (int i = 0; i < m_Bounds.x; i++)
            {
                TileType type = TileMethods.TypeById[m_Grid[i + j * m_Bounds.x]];
                if (type == TileType.Path || type == TileType.End)
                {
                    m_Accessibles.Add(new Vector2Int(i, j));
                }
            }
        }
    }
}

public static class MapReader
{
    public static void LoadFromFile(string fileName, out MapData mapData)
    {
        mapData = new MapData();
        StreamReader reader = new StreamReader(fileName);
        int columns = -1;
        bool finished = false;
        while (!reader.EndOfStream && !finished)
        {
            string s = reader.ReadLine();
            for (int i = 0; i < s.Length; i++)
            {
                char character = s[i];
                if (character.Equals('#'))
                {
                    finished = true;
                    //Spawn waves
                    break;
                }
                else
                {
                    mapData.Grid.Add((int)char.GetNumericValue(character));
                }
            }
            columns++;
        }
        mapData.InitializeBounds(columns);
        mapData.InitializeAccessibles();
        //towers
    }
}
