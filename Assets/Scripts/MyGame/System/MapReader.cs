using System.Collections.Generic;
using UnityEngine;
using System.IO;

public struct Wave
{
    uint m_TypeOne, m_TypeTwo;

    public uint TypeOne => m_TypeOne;
    public uint TypeTwo => m_TypeTwo;

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

    public List<Wave> Waves => m_Waves;

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

    private static void ReadTiles(string s, ref MapData mapData)
    {
        for(int i = 0; i < s.Length; i++)
        {
            char character = s[i];
            mapData.Grid.Add((int)char.GetNumericValue(character));
        }

    }

    private static void ReadWave(string s, ref MapData mapData)
    {
        uint typeOne = 0;
        uint typeTwo = 0;
        string wave = "";

        for(int i = 0; i < s.Length; i++)
        {
            char character = s[i];
            if(character.Equals(' '))
            {
                if(uint.TryParse(wave, out uint number))
                {
                    typeOne = number;
                    wave = "";
                }
            }
            else
            {
                wave = wave.Insert(wave.Length, character.ToString());
            }

            if(i == s.Length - 1)
            {
                if(uint.TryParse(wave, out uint number))
                {
                    typeTwo = number;
                    mapData.AddWave(typeOne, typeTwo);
                }
            }
        }
    }

    public static void LoadFromFile(string fileName, out MapData mapData)
    {
        mapData = new MapData();
        StreamReader reader = new StreamReader(fileName);
        int columns = 0;
        bool finishedReadingTiles = false;
        while (!reader.EndOfStream)
        {
            string s = reader.ReadLine();

            if(s[0].Equals('#'))
            {
                finishedReadingTiles = true;
            }

            if(!finishedReadingTiles)
            {
                ReadTiles(s, ref mapData);
                columns++;
            }
            else
            {
                ReadWave(s, ref mapData);
            }
        }
        mapData.InitializeBounds(columns);
        mapData.InitializeAccessibles();
    }
}
