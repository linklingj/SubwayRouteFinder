using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class StationData
{
    public string bldn_nm;
    public int bldn_id;
    public string route; 
    public float lot;
    public float lat;

    public StationData(string _bldn_nm, string _bldn_id, string _route, string _lot, string _lat)
    {
        bldn_nm = _bldn_nm;
        bldn_id = int.Parse(_bldn_id);
        route = _route;
        lot = float.Parse(_lot);
        lat = float.Parse(_lat);
    }
}

[System.Serializable]
public class StationsData
{
    public List<StationData> DATA;
}

[System.Serializable]
public class EdgesData
{
    public List<EdgeData> DATA;
}

[System.Serializable]
public class EdgeData
{
    public int startStation;
    public int endStation;
    public float time;
}

// [System.Serializable]
// public class C_tmtmp
// {
//     public List<C_tmp> lliisstt = new List<C_tmp>();
// }
//
// [System.Serializable]
// public class C_tmp
// {
//     public int startStation;
//     public int endStation;
//     public float time;
//     public C_tmp(int a, int b, float c)
//     {
//         startStation = a;
//         endStation = b;
//         time = c;
//     }
// }

public class Generator : MonoBehaviour
{
    public TextAsset stationPositionFile, timeBetweenStationsFile;
    public GameObject StationGameObject;
    public List<Line> lineObj;
    [SerializeField] private Transform stationParent;
    
    private StationName info;
    private StationsData data;
    private List<Station> stations;
    private Dictionary<int, List<Tuple<int, float>>> route;
    private LineRenderer lr;

    void Start()
    {
        info = new StationName();
        LoadRoutes();
        GenerateRoute();
        VisualizeRoute();
    }

    private void LoadRoutes()
    {
        if (stationPositionFile == null)
        {
            Debug.Log("No Data");
            return;
        }
        
        data = JsonUtility.FromJson<StationsData>(stationPositionFile.text);
        stations = new List<Station>();
        // var tmp = new Dictionary<string, int>();
        // List<C_tmp> list = new List<C_tmp>();

        foreach (StationData stationData in data.DATA)
        {
            GameObject go = Instantiate(StationGameObject, Vector2.zero, Quaternion.identity, stationParent);
            Line line = lineObj.Find((el) => el.Name == info.GetStation(stationData.route)) ?? lineObj[0];
            Station station = go.GetComponent<Station>();
            station.Init(stationData, line);
            stations.Add(station);
            
            //같은 역 검사 이제 필요 없음
            // try
            // {
            //     tmp.Add(stationData.bldn_nm, stationData.bldn_id);
            // }
            // catch
            // {
            //     list.Add(new C_tmp(tmp[stationData.bldn_nm], stationData.bldn_id, 0.0f));
            // }
        }

        // var listt = new C_tmtmp();
        // listt.lliisstt = list;
        // System.IO.File.WriteAllText(Application.dataPath + "/Scripts/Data/tmp.json", JsonUtility.ToJson(listt));
    }

    void GenerateRoute()
    {
        if (timeBetweenStationsFile == null)
        {
            Debug.Log("No Data");
            return;
        }
        
        var edgesData = JsonUtility.FromJson<EdgesData>(timeBetweenStationsFile.text);
        route = new Dictionary<int, List<Tuple<int, float>>>();
        
        foreach (var edge in edgesData.DATA)
        {
            if (route.ContainsKey(edge.startStation))
                route[edge.startStation].Add(Tuple.Create<int, float>(edge.endStation, edge.time));
            else
            {
                List<Tuple<int, float>> list = new List<Tuple<int, float>>();
                list.Add(Tuple.Create<int,float>(edge.endStation, edge.time));
                route.Add(edge.startStation, list);
            }
            
            if (route.ContainsKey(edge.endStation))
                route[edge.endStation].Add(Tuple.Create<int, float>(edge.startStation, edge.time));
            else
            {
                List<Tuple<int, float>> list = new List<Tuple<int, float>>();
                list.Add(Tuple.Create<int,float>(edge.startStation, edge.time));
                route.Add(edge.endStation, list);
            }
            //todo: 6호선 일방통행 예외처리
            //todo: 수인분당선 서울숲<->왕십리 왕십리<->청량리 추가
            //todo: 공덕역 뭔가 문제있음
        }
    }

    void VisualizeRoute()
    {
        foreach (var startPoint in route)
        {
            foreach (var endPoint in startPoint.Value)
            {
                Station startNode = FindStation(startPoint.Key);
                Station endNode = FindStation(endPoint.Item1);
                if (startNode && endNode)
                    Debug.DrawLine(startNode.transform.position, endNode.transform.position, startNode.line.GetColor(), 9999f);
            }
        }
    }

    public Dictionary<int, List<Tuple<int, float>>> GetRoute()
    {
        return route;
    }

    public List<Station> GetStations()
    {
        return stations;
    }

    public Station FindStation(int num)
    {
        return stations.Find(s => s.data.bldn_id == num);
    }
}
