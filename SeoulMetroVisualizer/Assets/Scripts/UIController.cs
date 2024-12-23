using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Settings setting;
    [SerializeField] private TMP_InputField startInput, endInput;
    [SerializeField] private Scrollbar speedInput;
    [SerializeField] private TextMeshProUGUI timeText, transferText;
    [SerializeField] private GameObject routeUIBig, routeUISmall;
    [SerializeField] private Transform routeUIParent;
    private Station startStation, endStation;
    private List<Station> stations;
    private RouteFinder routeFinder;
    private Generator generator;

    private void Start()
    {
        routeFinder = GetComponent<RouteFinder>();
        generator = GetComponent<Generator>();
    }

    public void FindButtonPressed()
    {
        String startStationName = startInput.text;
        String endStationName = endInput.text;
        if (startStationName == null || endStationName == null)
        {
            Debug.Log("empty input");
            return;
        }

        StartCoroutine(routeFinder.FindRoute(startStationName, endStationName));
    }

    public void ShowResult(RouteData route)
    {
        timeText.text = Mathf.CeilToInt(route.time) + "분";
        transferText.text = "환승 " + route.transfer.Count + "회";

        DeleteChilds(routeUIParent);
        
        GenerateUIBig(route.start.data.bldn_nm, route.start.line.name, 0);
        float t = 0;
        foreach (var s in route.transfer)
        {
            GenerateUISmall(s.Item3 - t - setting.transferTime);
            t = s.Item3;
            Station from = generator.FindStation(s.Item1);
            Station to = generator.FindStation(s.Item2);
            GenerateUIBig(from.data.bldn_nm,null ,1);
            GenerateUIBig(to.data.bldn_nm, to.line.name ,0);
        }
        GenerateUISmall(route.time - t);
        GenerateUIBig(route.end.data.bldn_nm, null, 1);
    }

    void GenerateUIBig(string station, string line, int inout) //in:0 out:1
    {
        string text = "";
        if (line != null) text += line + " ";

        text += station + "역" ;

        if (inout == 1) text += "하차";
        else text += "승차";

        GameObject uiEl = Instantiate(routeUIBig, Vector3.zero, Quaternion.identity, routeUIParent);
        uiEl.GetComponent<TextMeshProUGUI>().text = text;
    }

    void GenerateUISmall(float time)
    {
        string text = Mathf.RoundToInt(time) + "분";
        
        GameObject uiEl = Instantiate(routeUISmall, Vector3.zero, Quaternion.identity, routeUIParent);
        uiEl.GetComponent<TextMeshProUGUI>().text = text;
    }

    void DeleteChilds(Transform parent)
    {
        foreach(Transform child in parent)
            Destroy(child.gameObject);
    }

    public bool MouseOnUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public float GetVisualizeSpeed()
    {
        return (1 - speedInput.value) * 0.2f;
    }
}
