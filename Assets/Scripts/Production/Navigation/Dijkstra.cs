using System;
using UnityEngine;
using System.Collections.Generic;

namespace AI
{
	//TODO: Implement IPathFinder using Dijsktra algorithm.
	public class Dijkstra : IPathFinder
	{
        HashSet<Vector2Int> m_RightPositions;
        
        public Dijkstra(IEnumerable<Vector2Int> accessibles)
        {
            m_RightPositions = new HashSet<Vector2Int>(accessibles);
        }

        public IEnumerable<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            Dictionary<Vector2Int, Vector2Int?> ancestors = new Dictionary<Vector2Int, Vector2Int?>();
            ancestors.Add(start, null);

            Queue<Vector2Int> frontier = new Queue<Vector2Int>();
            frontier.Enqueue(start);

            while(frontier.Count > 0)
            {
                Vector2Int current = frontier.Dequeue();
                if(current == goal)
                {
                    break;
                }
                foreach(Vector2Int dir in Tools.DirectionTools.Dirs)
                {
                    Vector2Int next = current + dir;
                    if (m_RightPositions.Contains(next))
                    {
                        if (!ancestors.ContainsKey(next))
                        {
                            ancestors[next] = current;
                            frontier.Enqueue(next);
                        }
                    }
                }
            }
            if (ancestors.ContainsKey(goal))
            {
                List<Vector2Int> path = new List<Vector2Int>();

                for (Vector2Int? run = goal; run.HasValue; run = ancestors[run.Value])
                {
                    path.Add(run.Value);
                }
                path.Reverse();
                return path;
            }

            return System.Linq.Enumerable.Empty<Vector2Int>();
        }
	}    
}
