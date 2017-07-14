import midi

from collections import deque
from utils import *

path = '../233.mid'

pattern = midi.read_midifile(path)



def findBeat(beat, beats):
    for i in beats:
        if beat <= i:
            return i
    return beats[-1]

def fraction(a , b):
    def gcd(x,y):
        if y == 0:
            return x
        return gcd(y,x%y)
    _gcd = gcd(a , b)
    return a/_gcd,b/_gcd


def SolveMidi(path):
    '''

    :param path: the path of midi file
    :return:     the tracks of this song

    '''
    pattern = midi.read_midifile(path)
    res = pattern.resolution
    beats = []
    te = res
    cnt = 0

    while te % 2 == 0 and cnt < 4:
        te /= 2
        cnt += 1

    cnt = 1
    while cnt*te <= res*4:
        beats.append(cnt*te)
        cnt += 1

    n_track = 0
    max_track_length = 0
    tracks = []
    for track_ in pattern:
        Q = deque()
        sections = 0
        track = []
        #print >> fp,'Track:',n_track
        n_track += 1
        tick = 0
        last = 0
        current_bar = 0
        length_bar = 4*res
        for event in track_:
            if event.name == 'Time Signature':
                length_bar = res * event.numerator / event.denominator
                #print 'length_bar',length_bar

            if event.name not in ['Note On','Note Off']:
                continue

            tick += event.tick
            current_bar += event.tick

            # if it is an Off Event, mark this pitch
            if event.name == 'Note Off' or event.velocity == 0:
                temp = Q.popleft()
                length = tick - temp[1]
                beat = findBeat(length,beats)
                last = beat - length
                track.append(Note(event.pitch, fraction(beat,length_bar)))
                #print >>fp, event.pitch, fraction(beat,length_bar)

            # if it is an On Event
            if event.name == 'Note On' and event.velocity != 0:
                Q.append([event.pitch,tick])
                remain = event.tick - last
                last = 0

                # find the end of section
                while True:
                    temp = current_bar - remain
                    temp = length_bar - temp
                    if remain >= temp and remain > 0 and temp > 0:
                        track.append(Note(-2,fraction(temp, length_bar)))
                        #print >>fp, -2,fraction(temp,length_bar)
                        remain -= temp
                    elif remain < temp and remain > 0:
                        track.append(Note(-2,fraction(remain,length_bar)))
                        #print >>fp, -2, fraction(remain,length_bar)
                        remain = 0
                    if current_bar < length_bar:
                        break
                    track.append(noteEnd)
                    #print>>fp, 'section end'
                    sections += 1
                    current_bar -= length_bar
        if current_bar != 0:
            remain = length_bar - current_bar
            track.append(Note(-2,fraction(remain,length_bar)))
            track.append(noteEnd)
            sections += 1

        if sections != 0:
            tracks.append(track)
            max_track_length = max(max_track_length,sections) # mark the maximum length of track

    for track in tracks:
        length = len(track)
        while length < max_track_length:
            track.append(Note(-2,fraction(length_bar,length_bar)))
            track.append(noteEnd)
            length += 1

    return tracks

def test():
    pattern = midi.read_midifile(path)

    for track in pattern:
        tick = 0
        for event in track:
            #print event.name
            if event.name in ['Note On','Note Off']:
                tick += event.tick

        print tick


folder = 'D:/Files/Project/melody-master/python-midi-master/good songs/test'

def readfolder():
    import os
    filelist = [file for file in os.listdir(folder)]
    if not os.path.exists("test"):
        os.makedirs("test")
    for file in filelist:
        file_path = os.path.join(folder,file)
        #score = StdScore(SolveMidi(file_path))
        tracks = SolveMidi(file_path)
        print file
        with open("test/%s.txt"%file,"w") as fp:
            cnt_track = 0
            for track in tracks:
                print >>fp,'Track',cnt_track
                for Note in track:
                    if Note.pitch == -1:
                        print >>fp,'Section End'
                    else:
                        print >>fp,Note.pitch,Note.duration

if __name__ == '__main__':
    readfolder()
    #tracks = SolveMidi(path)
    #score = StdScore(tracks)

    #with open("test.txt","w") as fp:
    #    cnt_track = 0
    #    for track in tracks:
    #        print >>fp,'Track',cnt_track

    #        for Note in track:
    #            if Note.pitch == -1:
    #                print >>fp,'Section End'
    #            else:
    #                print >>fp,Note.pitch,Note.duration

    #test()
