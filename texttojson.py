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

def createconfig(phaseName, subjectNumber, playerMoveSpeed, objTriggerRadius, actionKey, scenes, pauseTime):
    return {
            "scenes" : list(scenes),
            "phaseName": phaseName,
            "subjectNumber": subjectNumber,
            "playerMoveSpeed": playerMoveSpeed,
            "objTriggerRadius": objTriggerRadius,
            "actionKey": actionKey,
            "pauseTime": pauseTime,
    }

def createcreateconfig(phaseName, subjectNumber, playerMoveSpeed, objTriggerRadius, pauseTime, actionKey):
    """Returns a function that only needs a list of scenes to create the config"""
    def f(scenes):
        return createconfig(phaseName, subjectNumber, playerMoveSpeed, objTriggerRadius, actionKey, scenes, pauseTime)
    return f

def parsenormal(lines, _, configfunc):
    def gen():
        for info in map(list, zip(*map(toints, lines[6:14+1]))): # Grabbing ea column in text file
            yield {
                "mode": "normal",
                "objShowIndex" : info[0],
                "showTime" : info[1],
                "greyScreenTime" : info[2],
                "greyScreenTimeTwo" : info[3],
                "envIndex" : info[4],
                "envTime" : info[5],
                "objSpawnIndex" : info[6],
                "playerSpawnIndex" : info[7],
                "landmarkSpawnIndex" : info[8],
                "searchObjs" : []
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
                "objSpawnIndex" : -1,
                "playerSpawnIndex" : infolines[1],
                "landmarkSpawnIndex" : -1,
                "searchObjs" : []
            }])
    )

if __name__ == "__main__":
    def err(lines):
        eprint("'{}' is not a recognized mode".format(lines[0]))

    handlers = {
        "normal" : parsenormal,
        "explore" : parseexplore,
    }

    filename = sys.argv[1]
    with open(filename, "r") as f:
        lines = [l.rstrip() for l in f.readlines()]
        print(handlers.get(lines[0], err)(lines, lines[6:], createcreateconfig(*lines[0:6])))
