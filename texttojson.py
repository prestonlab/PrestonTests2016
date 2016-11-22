# Prints named text file in arg1 to stdout as a json file

import sys
import json

"""
{
    "mode": "searchfind",
    "objShowIndex" : 0,
    "showTime" : 2,
    "greyScreenTime" : 0,
    "envIndex" : 1,
    "envTime" : -1,
    "objSpawnIndex" : -1,
    "playerSpawnIndex" : 0,
    "landmarkSpawnIndex" : -1,
    "searchObjs" : [ 
        { "objSpriteIndex" : 0, "objSpawnIndex" : 0 },
        { "objSpriteIndex" : 1, "objSpawnIndex" : 1 }
    ]
}
"""

def eprint(*args, **kwargs):
    """Print to std err"""
    print(*args, file=sys.stderr, **kwargs)

def prettydumps(obj):
    return json.dumps(obj, indent=4)

def toints(line):
    return (int(i) for i in line.split(" "))

def wrapscenes(scenes):
    # scenes is a list of scenes
    return {
        "scenes" : list(scenes)
    }

def parsenormal(lines):
    def gen():
        for info in zip(*map(toints, lines[1:8+1])):
            infolist = list(info)
            yield {
                "mode": "normal",
                "objShowIndex" : infolist[0],
                "showTime" : infolist[1],
                "greyScreenTime" : infolist[2],
                "envIndex" : infolist[3],
                "envTime" : infolist[4],
                "objSpawnIndex" : infolist[5],
                "playerSpawnIndex" : infolist[6],
                "landmarkSpawnIndex" : infolist[7],
                "searchObjs" : []
            }
    return prettydumps(wrapscenes(list(gen())))

def parseexplore(lines):
    return prettydumps(
            wrapscenes([{
                "mode": "explore",
                "objShowIndex" : -1,
                "showTime" : -1,
                "greyScreenTime" : -1,
                "envIndex" : lines[1],
                "envTime" : lines[3],
                "objSpawnIndex" : -1,
                "playerSpawnIndex" : lines[2],
                "landmarkSpawnIndex" : -1,
                "searchObjs" : []
            }])
    )

def parsesearchfind(lines):
    return prettydumps(
            wrapscenes([{
                "mode": "searchfind",
                "objShowIndex" : 0,
                "showTime" : 0,
                "greyScreenTime" : 0,
                "envIndex" : lines[1],
                "envTime" : -1,
                "objSpawnIndex" : -1,
                "playerSpawnIndex" : lines[2],
                "landmarkSpawnIndex" : -1,
                "searchObjs" : [
                    { "objSpriteIndex" : top, "objSpawnIndex" : bot } for top, bot in zip(*map(toints, lines[3:4+1]))
                ]
            }])
    )

if __name__ == "__main__":
    def err(lines):
        eprint("'{}' is not a recognized mode".format(lines[0])

    handlers = {
        "normal" : parsenormal,
        "explore" : parseexplore,
        "searchfind" : parsesearchfind,
    }

    filename = sys.argv[1]
    with open(filename, "r") as f:
        lines = [l.rstrip() for l in f.readlines()]
        print(handlers.get(lines[0], default=err)(lines))

