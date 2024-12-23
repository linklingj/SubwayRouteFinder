using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Station : MonoBehaviour
{
    public StationData data;
    public Settings settings;
    public Line line;
    public GameObject uiEl, lineSprite, stationText;

    Vector2 pos;
    private SpriteRenderer sr;
    private LineRenderer lr;
    private Camera mainCamera;
    private Vector3 initialScale;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        lr = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
    }

    public void Init(StationData _data, Line _line)
    {
        data = _data;
        pos = new Vector2(_data.lot, _data.lat);
        transform.position = GetGamePos(pos);
        line = _line;
        sr.color = line.GetColor();
        initialScale = transform.localScale;
        lineSprite.GetComponent<Image>().color = line.GetColor();
        stationText.GetComponent<TextMeshProUGUI>().text = data.bldn_nm;
    }

    private Vector2 GetGamePos(Vector2 pos)
    {
        Vector2 gamePos = new Vector2(pos.x - settings.centerLOT, pos.y - settings.centerLAT) * settings.degToUnit;
        return gamePos;
    }

    private void Update()
    {
        AdjustSpriteSize();
        if (sr.enabled)
            CheckUI();
    }

    void AdjustSpriteSize()
    {
        if (mainCamera == null) return;
        float sizeAdjustment = Math.Min(mainCamera.orthographicSize / settings.targetNodeSize, settings.maxNodeSize);
        transform.localScale = initialScale * sizeAdjustment;
    }

    void CheckUI()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            uiEl.SetActive(true);
        }
        else
        {
            uiEl.SetActive(false);
        }
    }

    public void Activate()
    {
        sr.enabled = true;
    }

    public void DeActivate()
    {
        sr.enabled = false;
        HideLine();
    }

    public void MakeLine(Vector3 pos)
    {
        lr.enabled = true;
        lr.startColor = line.color;
        lr.endColor = line.color;
        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, pos);
    }

    public void HideLine()
    {
        lr.enabled = false;
    }
}
