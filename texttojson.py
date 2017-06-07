# Prints named text file in arg1 to stdout as a json file

import sys
import json

def eprint(*args, **kwargs):
    """Print to stderr"""
    print(*args, file=sys.stderr, **kwargs)

def prettydumps(obj):
    return json.dumps(obj, indent=4)

def tofloats(line):
    return map(float, line.split(" "))

def createconfig(phaseName, subjectName, playerMoveSpeed, objTriggerRadius, actionKey, scenes, pauseTime, lookSlerpTime):
    return {
            "scenes" : list(scenes),
            "phaseName": phaseName,
            "subjectName": subjectName,
            "playerMoveSpeed": playerMoveSpeed,
            "objTriggerRadius": objTriggerRadius,
            "actionKey": actionKey,
            "pauseTime": pauseTime,
            "lookSlerpTime": lookSlerpTime,
    }

def createcreateconfig(phaseName, subjectName, playerMoveSpeed, objTriggerRadius, pauseTime, lookSlerpTime, actionKey):
    """Returns a function that only needs a list of scenes to create the config"""
    def f(scenes):
        return createconfig(phaseName, subjectName, playerMoveSpeed, objTriggerRadius, actionKey, scenes, pauseTime, lookSlerpTime)
    return f

def parsenormal(lines, infolines, configfunc):
    def gen():
        for info in map(tuple, zip(*map(tofloats, lines[7:17+1]))): # Grabbing ea column in text file
            yield {
                "mode": "normal",
                "objShowIndex" : int(info[0]),
                "showTime" : info[1],
                "greyScreenTime" : info[2],
                "greyScreenTimeTwo" : info[3],
                "envIndex" : int(info[4]),
                "envTime" : info[5],
                "envTimeTwo" : info[6],
                "objSpawnIndex" : int(info[7]),
                "showObjAlways" : int(info[8] == 1), # Show obj only if value is 1
                "panTime" : info[9],
                "countSeconds" : int(info[10]),
                "playerSpawnIndex" : int(info[11]),
                "landmarkSpawnIndex" : int(info[12]),
            }
    return prettydumps(configfunc(list(gen())))

def parseexplore(alllines, infolines, configfunc):
    return prettydumps(
            configfunc([{
                "mode": "explore",
                "objShowIndex" : -1,
                "showTime" : -1,
                "greyScreenTime" : -1,
                "greyScreenTimeTwo" : -1,
                "envIndex" : infolines[0],
                "envTime" : infolines[2],
                "envTimeTwo" : -1,
                "objSpawnIndex" : -1,
                "showObjAlways" : False,
                "playerSpawnIndex" : infolines[1],
                "landmarkSpawnIndex" : -1,
                "panTime" : 0.0,
                "countSeconds" : 0,
                "playerSpawnIndex" : -1,
            }])
    )

if __name__ == "__main__":
    def err(lines, *args):
        eprint("'{}' is not a recognized mode".format(lines[0]))

    handlers = {
        "normal" : parsenormal,
        "explore" : parseexplore,
    }

    filename = sys.argv[1]
    with open(filename, "r") as f:
        lines = [l.rstrip() for l in f.readlines()]
        print(handlers.get(lines[0], err)(lines, lines[7:], createcreateconfig(*lines[0:7])))
