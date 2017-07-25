import pickle
import numpy as np
from utils import *
from main import SolveMidi
import os

def test():
    path = "all_train2.pkl"
    with open(path,"rb") as fp:
        k = pickle.load(fp)
    X = np.asarray(k['X'])
    Y = np.asarray(k['Y'])

    print X.shape,Y.shape
    id = 0
    total = 0
    for song in Y:
        id += 1
        length = len(song)
        empty_section = 0
        for section in song:
            cnt = 0
            for _ in section:
                if _ == 0:
                    cnt += 1

            if cnt == 24:
                empty_section += 1
        if length == empty_section:
            print "empty chord id",id
            total += 1
    print 'Total',total


def check(song):
    length = len(song)
    empty_section = 0
    for section in song:
        cnt = 0
        for _ in section:
            if _ == 0:
                cnt += 1
        if cnt == 24:
            empty_section += 1
    if empty_section == length:
        return True
    return False

def find_empty_chord():

    import os

    #path = 'D:/Files/Project/melody-master/python-midi-master/good songs/RenameData'
    path = 'D:/Files/Project/melody-master/python-midi-master/good songs/Mozart'
    files = [file for file in os.listdir(path) if file[-3:]=='mid']
    cnt = 0
    total = 0
    for file in files:
        try:
            score = StdScore(SolveMidi(os.path.join(path,file)))
            if check(score.getChordFeature()):
                total += 1
                print 'Empty chord song',file
        except Exception,ex:
            print ex
            continue
        cnt += 1
        if cnt == 5000:
            break

def test2():
    path = 'D:/Files/Project/melody-master/python-midi-master/good songs/Mozart'
    files = [file for file in os.listdir(path) if file[-3:] == 'mid']
    _cnt = 0
    for file in files:
        _cnt += 1
        if _cnt != 63:
            continue
        print file
        #if _cnt > 1:
        #    break
        try:
            tracks = SolveMidi(os.path.join(path,file))
            with open("parse/%s.txt"%file,"w") as fp:
                cnt = 0
                for track in tracks:
                    cnt += 1
                    print >>fp,'Track',cnt
                    for note in track:
                        if note.pitch == -1:
                            print >>fp,'NoteEnd'
                        else:
                            print >>fp,note.pitch,note.duration
            with open("feature/%s.txt"%file,"w") as fp:
                score = StdScore(tracks)
                print >>fp,score.getKeyFeature()
                print >>fp,"\n\n===========================\n\n"
                print >>fp,score.getChordFeature()
        except Exception,ex:
            print ex

def test3():
    path = 'D:/Files/Project/melody-master/python-midi-master/good songs/Arabic - A 3tbak 3la Eih.mid'
    #path = 'D:/Files/Project/melody-master/python-midi-master/good songs/yq.mid'
    tracks = SolveMidi(path)
    print tracks
    #score = StdScore(tracks)
    #temp = score.getChordId()
    #cnt = 0
    #translator = ChordTranslator()
    #for _ in temp:
    #    cnt += 1
    #    print translator.dict[_],cnt
    #print score.getChordId()
    #print score.getChordFeature()
    #with open() as fp:


if __name__ == '__main__':
    test3()
    #test2()
    #find_empty_chord()
    #test()