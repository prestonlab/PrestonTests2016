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

def createconfig(phaseName, subjectNumber, playerMoveSpeed, objTriggerRadius, actionKey, scenes, pauseTime, lookSlerpTime):
    return {
            "scenes" : list(scenes),
            "phaseName": phaseName,
            "subjectNumber": subjectNumber,
            "playerMoveSpeed": playerMoveSpeed,
            "objTriggerRadius": objTriggerRadius,
            "actionKey": actionKey,
            "pauseTime": pauseTime,
            "lookSlerpTime": lookSlerpTime,
    }

def createcreateconfig(phaseName, subjectNumber, playerMoveSpeed, objTriggerRadius, pauseTime, lookSlerpTime, actionKey):
    """Returns a function that only needs a list of scenes to create the config"""
    def f(scenes):
        return createconfig(phaseName, subjectNumber, playerMoveSpeed, objTriggerRadius, actionKey, scenes, pauseTime, lookSlerpTime)
    return f

def parsenormal(lines, infolines, configfunc):
    def gen():
        for info in map(list, zip(*map(toints, lines[7:16+1]))): # Grabbing ea column in text file
            yield {
                "mode": "normal",
                "objShowIndex" : info[0],
                "showTime" : info[1],
                "greyScreenTime" : info[2],
                "greyScreenTimeTwo" : info[3],
                "envIndex" : info[4],
                "envTime" : info[5],
                "objSpawnIndex" : info[6],
                "showObjAlways" : info[7] == 1, # Show obj only if value is 1
                "playerSpawnIndex" : info[8],
                "landmarkSpawnIndex" : info[9],
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
                "showObjAlways" : False,
                "playerSpawnIndex" : infolines[1],
                "landmarkSpawnIndex" : -1,
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
        print(handlers.get(lines[0], err)(lines, lines[7:], createcreateconfig(*lines[0:7])))
