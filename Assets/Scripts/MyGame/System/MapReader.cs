using System.Collections.Generic;
using UnityEngine;
using System.IO;

public struct Wave
{
    uint m_TypeOne, m_TypeTwo;

    public Wave(uint typeOne, uint typeTwo)
    {
        m_TypeOne = typeOne;
        m_TypeTwo = typeTwo;
    }
}

public class MapData
{
    private List<int> m_Grid;
    private Vector2Int m_Bounds;
    private List<Vector2Int> m_Accessibles;
    private List<Wave> m_Waves;
    public List<int> Grid => m_Grid;
    public Vector2Int Bounds => m_Bounds;

    public List<Vector2Int> Accessibles => m_Accessibles;

    public MapData()
    {
        m_Grid = new List<int>();
        m_Bounds = new Vector2Int();
        m_Accessibles = new List<Vector2Int>();
        m_Waves = new List<Wave>();
    }

    public void AddWave(uint typeOne, uint typeTwo)
    {
        m_Waves.Add(new Wave(typeOne, typeTwo));
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
        bool finishedReadingTiles = false;
        while (!reader.EndOfStream && !finishedReadingTiles)
        {
            int typeOne = -1;
            int typeTwo = -1;
            string wave = "";
            string s = reader.ReadLine();
            for (int i = 0; i < s.Length; i++)
            {
                char character = s[i];
                if (character.Equals('#'))
                {
                    finishedReadingTiles = true;
                }
                else if(!finishedReadingTiles)
                {
                    mapData.Grid.Add((int)char.GetNumericValue(character));
                }
                else
                {
                    if(character.Equals(' ') || character.Equals('\n'))
                    {
                        int number = int.Parse(wave);
                        if(typeOne != -1)
                        {
                            typeOne = number;
                        }
                        else
                        {
                            typeTwo = number;
                        }
                    }
                    wave.Insert(wave.Length, character.ToString());
                }
            }
            columns++;
        }
        mapData.InitializeBounds(columns);
        mapData.InitializeAccessibles();
        //towers
    }
}
