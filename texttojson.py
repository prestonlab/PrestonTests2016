# Prints named text file in arg1 to stdout as a json file

import sys
import json

def eprint(*args, **kwargs):
    """Print to stderr"""
    print(*args, file=sys.stderr, **kwargs)

def prettydumps(obj):
    return json.dumps(obj, indent=4)

def toints(line):
    return (int(i) for i in line.split(" "))

def wrapscenes(scenes):
    # TODO Remove this function
    return {
        "scenes" : list(scenes)
    }

def createconfig(phaseName, subjectNumber, playerMoveSpeed, objTriggerRadius, actionKey, scenes):
    return {
            "scenes" : list(scenes),
            "phaseName": phaseName,
            "subjectNumber": subjectNumber,
            "playerMoveSpeed": playerMoveSpeed,
            "objTriggerRadius": objTriggerRadius,
            "actionKey": actionKey,
    }

def parsenormal(lines):
    def gen():
        for info in zip(*map(toints, lines[5:13+1])): # Grabbing ea column in text file
            infolist = list(info)
            yield {
                "mode": "normal",
                "objShowIndex" : infolist[0],
                "showTime" : infolist[1],
                "greyScreenTime" : infolist[2],
                "greyScreenTimeTwo" : infolist[3],
                "envIndex" : infolist[4],
                "envTime" : infolist[5],
                "objSpawnIndex" : infolist[6],
                "playerSpawnIndex" : infolist[7],
                "landmarkSpawnIndex" : infolist[8],
                "searchObjs" : []
            }
    return prettydumps(createconfig("normal", lines[1], lines[2], lines[3], lines[4], list(gen())))

def parseexplore(lines):
    return prettydumps(
            createconfig("explore", lines[1], lines[2], lines[3], lines[4], [{
                "mode": "explore",
                "objShowIndex" : -1,
                "showTime" : -1,
                "greyScreenTime" : -1,
                "greyScreenTimeTwo" : -1,
                "envIndex" : lines[5],
                "envTime" : lines[8],
                "objSpawnIndex" : -1,
                "playerSpawnIndex" : lines[6],
                "landmarkSpawnIndex" : -1,
                "searchObjs" : []
            }])
    )

def parsesearchfind(lines):
    return prettydumps(
            createconfig("searchfind", lines[1], lines[2], lines[3], lines[4], [{
                "mode": "searchfind",
                "objShowIndex" : 0,
                "showTime" : 0,
                "greyScreenTime" : 0,
                "greyScreenTimeTwo" : 0,
                "envIndex" : lines[5],
                "envTime" : -1,
                "objSpawnIndex" : -1,
                "playerSpawnIndex" : lines[6],
                "landmarkSpawnIndex" : -1,
                "searchObjs" : [
                    { "objSpriteIndex" : top, "objSpawnIndex" : bot } for top, bot in zip(*map(toints, lines[7:8+1]))
                ]
            }])
    )

if __name__ == "__main__":
    def err(lines):
        eprint("'{}' is not a recognized mode".format(lines[0]))

    handlers = {
        "normal" : parsenormal,
        "explore" : parseexplore,
        "searchfind" : parsesearchfind,
    }

    filename = sys.argv[1]
    with open(filename, "r") as f:
        lines = [l.rstrip() for l in f.readlines()]
        print(handlers.get(lines[0], err)(lines))
