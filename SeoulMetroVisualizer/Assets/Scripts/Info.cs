using System.Collections.Generic;
using UnityEngine;

public class StationName
{
    public string GetStation(string name)
    {
        return stationName[name];
    }
    
    private Dictionary<string, string> stationName = new Dictionary<string, string>()
    {
        {"1호선", "1호선"},
        {"2호선", "2호선"},
        {"3호선", "3호선"},
        {"4호선", "4호선"},
        {"5호선", "5호선"},
        {"6호선", "6호선"},
        {"7호선", "7호선"},
        {"8호선", "8호선"},
        {"9호선", "9호선"},
        {"수도권 광역급행철도", "GTX-A"},
        {"김포골드라인", "김포골드라인"},
        {"서해선", "서해선"},
        {"우이신설선", "우이신설선"},
        {"의정부선", "의정부경전철"},
        {"에버라인선", "용인에버라인"},
        {"신림선", "신림선"},
        {"신분당선(연장)", "신분당선"},
        {"신분당선", "신분당선"},
        {"신분당선(연장2)", "신분당선"},
        {"공항철도1호선", "공항철도"},
        {"9호선(연장)", "9호선"},
        {"7호선(인천)", "7호선"},
        {"인천2호선", "인천2호선"},
        {"인천1호선", "인천1호선"},
        {"별내선", "8호선"},
        {"일산선", "3호선"},
        {"경원선", "1호선"},
        {"수인선", "수인분당선"},
        {"분당선", "수인분당선"},
        {"경인선", "1호선"},
        {"안산선", "4호선"},
        {"경부선", "1호선"},
        {"경강선", "경강선"},
        {"과천선", "4호선"},
        {"장항선", "1호선"},
        {"경춘선", "경춘선"},
        {"경의중앙선", "경의중앙선"},
        {"중앙선", "경의중앙선"},  
        {"진접선", "4호선"},
        //{"경원선", "경의중앙선"}
    };
}
