import xml.etree.ElementTree as ET
import json

def time_to_minutes(time_str):
    minutes, seconds = map(int, time_str.split(":"))
    total_minutes = minutes + round((seconds / 60), 2)
    return total_minutes

tree = ET.parse('timeTable/everline.xml')
root = tree.getroot()
pathList = root.find('sPath').findall('pathList')
data = []

for path in pathList:
    startStation = int(path.find('startStationCode').text) #int
    endStation = int(path.find('endStationCode').text) #int
    time = time_to_minutes(path.find('runTime').text) #float
    data.append({"startStation" : startStation, "endStation" : endStation, "time" : time})

with open("timeBetweenStations.json", "r") as file:
    fd = json.load(file)
    fl = fd["DATA"]

for i in data:
    fl.append(i)

with open("timeBetweenStations.json", "w") as file:
    json.dump(fd, file, indent=4)