using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class RouteData
{
    public Station start;
    public Station end;
    public float time;
    public List<Station> stations;
    public List<(int,int,float)> transfer;
}

public class RouteFinder : MonoBehaviour
{
    private Generator generator;
    private UIController uiController;

    private void Start()
    {
        generator = GetComponent<Generator>();
        uiController = FindAnyObjectByType<UIController>();
    }

    public IEnumerator FindRoute(string startSt, string endSt)
    {
        var stations = generator.GetStations();
        List<Station> startStations = stations.FindAll(s => s.data.bldn_nm == startSt);
        if (startStations.Count == 0)
        {
            Debug.Log("Cannot find " + startSt);
            yield break;
        }
        List<Station> endStations = stations.FindAll(s => s.data.bldn_nm == endSt);
        if (endStations.Count == 0)
        {
            Debug.Log("Cannot find " + endSt);
            yield break;
        }

        if (startStations[0].data.bldn_nm == endStations[0].data.bldn_nm)
        {
            Debug.Log("Same Station Error");
            yield break;
        }
        
        Dictionary<int, float> distance = new Dictionary<int, float>();
        RouteData route = new RouteData();
        Dictionary<int, List<Tuple<int, float>>> edges = generator.GetRoute();
        
        HideAllStations();
        float visualizeSpeed = uiController.GetVisualizeSpeed();
        
        //Dijkstra
        foreach (var node in edges.Keys)
        {
            distance[node] = float.MaxValue;
        }

        var pq = new List<(float dist, int station)>();
        //var pq = new SortedSet<(float dist, int station)>(Comparer<(float, int)>.Create((a,b) => a.Item1.CompareTo(b.Item1))); //거리 같을 경우 추가
        var visited = new HashSet<int>();
        Dictionary<int, int> parent = new Dictionary<int, int>();

        foreach (var st in startStations)
        {
            distance[st.data.bldn_id] = 0;
            pq.Add((0, st.data.bldn_id));
            parent.Add(st.data.bldn_id, st.data.bldn_id);
        }

        Station startStation = startStations[0], endStation = null;
        
        while(pq.Count > 0)
        {
            pq.Sort((a, b) => a.dist.CompareTo(b.dist));
            var (dist, curNode) = pq[0];
            pq.Remove(pq[0]);

            if (visited.Contains(curNode)) continue;
            visited.Add(curNode);

            if (!edges.ContainsKey(curNode)) continue;

            if (endStations.Exists((st) => st.data.bldn_id == curNode))
            {
                endStation = endStations.Find((st) => st.data.bldn_id == curNode);
                break;
            }

            List<int> toAnimate = new List<int>();
            foreach (var next in edges[curNode])
            {
                int nextNode = next.Item1;
                float nextDist = dist + next.Item2;
                if (nextDist < distance[nextNode])
                {
                    distance[nextNode] = nextDist;
                    parent.TryAdd(nextNode, curNode);
                    pq.Add((nextDist, nextNode));
                    toAnimate.Add(nextNode);
                }
            }
            AnimateNodes(toAnimate, curNode);
            yield return new WaitForSeconds(visualizeSpeed);
        }
        
        if (endStation == null)
        {
            Debug.Log("route finding failed");
            yield break;
        }
        
        float totalTime = distance[endStation.data.bldn_id];
        route.time = totalTime;

        int tmp = endStation.data.bldn_id; 
        List<Station> tmpList = new List<Station>();

        while (!startStations.Exists((st) => st.data.bldn_id == tmp))
        {
            tmpList.Add(stations.Find(s => s.data.bldn_id == tmp));
            tmp = parent[tmp];
        }
        tmpList.Add(stations.Find(s => s.data.bldn_id == tmp));
        tmpList.Reverse();
        startStation = startStations.Find((st) => st.data.bldn_id == tmp);
        route.stations = tmpList;
        
        route.start = startStation;
        route.end = endStation;

        var transfer = new List<(int,int,float)>();
        for (int i = 0; i < tmpList.Count - 1; i++)
        {
            if (tmpList[i].line != tmpList[i + 1].line)
            {
                if (tmpList[i].data.bldn_nm == startStation.data.bldn_nm || tmpList[i+1].data.bldn_nm == endStation.data.bldn_nm) continue;
                transfer.Add((tmpList[i].data.bldn_id, tmpList[i+1].data.bldn_id, distance[tmpList[i+1].data.bldn_id]));
            }
        }
        route.transfer = transfer;
        
        ShowFinalStations(route.stations);
        uiController.ShowResult(route);
    }


    void ShowFinalStations(List<Station> activeStation)
    {
        var stations = generator.GetStations();
        for (int i = 0; i < stations.Count; i++)
        {
            if (activeStation.Contains(stations[i]))
            {
                stations[i].Activate();
            }
            else
            {
                stations[i].DeActivate();
            }
        }

        for (int i = 0; i < activeStation.Count-1; i++)
        {
            activeStation[i].MakeLine(activeStation[i + 1].transform.position);
        }
    }

    void HideAllStations()
    {
        var stations = generator.GetStations();
        foreach (var s in stations)
        {
            s.DeActivate();
        }
    }

    void AnimateNodes(List<int> nodes, int parent)
    {
        foreach (var node in nodes)
        {
            Station s = generator.FindStation(node);
            if (s) s.Activate();
        }
    }
}
