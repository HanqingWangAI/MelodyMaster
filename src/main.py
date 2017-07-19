import midi

from collections import deque
from utils import *
import pickle
import sys

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
    while te % 2 == 0 and cnt < 6:
        te /= 2
        cnt += 1

    cnt = 1
    #print 'te',te
    while cnt*te <= res*4:
        beats.append(cnt*te)
        cnt += 1

    n_track = 0
    max_track_length = 0
    tracks = []
    length_bar = 4 * res
    length_pitch = 4 * res
    for track_ in pattern:
        #print 'track',n_track
        Q = deque()
        sections = 0
        track = []
        #print >> fp,'Track:',n_track
        n_track += 1
        tick = 0
        last = 0
        last_tempo = 0
        section = []
        for event in track_:

            if event.name == 'Time Signature':
                length_bar = res * event.numerator * 4 / event.denominator
                #print 'length_bar',length_bar,'numerator',event.numerator,'denominator',event.denominator



            tick += event.tick

            #if n_track == 1:
            #    print 'event', event, 'section', sections,'tick',tick

            if event.name not in ['Note On', 'Note Off', 'Set Tempo']:
                continue
            # if it is an Off Event, mark this pitch
            if event.name == 'Note Off' or (event.name == 'Note On' and event.velocity == 0):
                last_tempo = 0
                count = 0
                while count <= len(Q):
                    temp = Q.popleft()
                    if temp[0] == event.pitch:
                        break
                    Q.append(temp)
                    count += 1
                if count>=len(Q):
                    continue

                last = 0
                last_end = temp[1]
                pos = int(last_end/length_bar)
                while True:
                    if tick < length_bar*(pos+1):
                        break

                    if pos < sections:
                        track[pos].append(Note(event.pitch,fraction(length_bar*(pos+1)-last_end,length_pitch)))
                    else:
                        section.append(Note(event.pitch,fraction(length_bar*(pos+1)-last_end,length_pitch)))
                        track.append(section)
                        section = []
                        sections += 1

                    last_end = length_bar*(pos+1)
                    pos += 1

                length = tick - last_end

                if length != 0:
                    beat = findBeat(length,beats)
                    last = beat - length


                    if pos < sections:
                        track[pos].append(Note(event.pitch, fraction(beat, length_pitch)))
                    else:
                        section.append(Note(event.pitch, fraction(beat,length_pitch)))

                # if it just reach the end of a section
                if (tick+last)%length_bar < 2 and (tick+last)/length_bar > sections:
                    track.append(section)
                    sections += 1
                    section = []


            # if it is an On Event
            if event.name == 'Note On' and event.velocity != 0:
                Q.append([event.pitch, tick])
                remain = event.tick+last_tempo - last
                last_tempo = 0
                last = 0
                #if remain > 0:
                #    print 'section',sections,'remain',remain

                # find the end of section
                while remain > 0:
                    temp = tick - remain
                    temp = length_bar - temp % length_bar
                    if remain >= temp and temp > 0:
                        section.append(Note(-2, fraction(temp, length_pitch)))
                        remain -= temp
                        track.append(section)
                        section = []
                        sections += 1

                    elif remain < temp:
                        section.append(Note(-2,fraction(remain,length_pitch)))
                        remain = 0

            if event.name == 'Set Tempo':
                last_tempo += event.tick

        if (tick-last_tempo)%length_bar != 0 and (tick-last_tempo)%length_bar != 1:
            remain = length_bar - (tick-last_tempo)%length_bar
            #if n_track == 1:
            #    print 'tick',tick,'remain',remain,'last_tempo',last_tempo
            section.append(Note(-2,fraction(remain,length_pitch)))
            track.append(section)
            section = []
            #track.append(Note(-2,fraction(remain,length_pitch)))
            #track.append(noteEnd)
            sections += 1

        if sections != 0:
            tracks.append(track)
            max_track_length = max(max_track_length,sections) # mark the maximum length of track


    for track in tracks:
        length = len(track)
        #print length, max_track_length
        while length < max_track_length:
            section = []
            section.append(Note(-2,fraction(length_bar,length_pitch)))
            #section.append(noteEnd)
            track.append(section)
            length += 1
    #print 'max_track',max_track_length
    ret = []
    for track in tracks:
        _t = []
        for section in track:
            for note in section:
                _t.append(note)
            _t.append(noteEnd)
        ret.append(_t)

    return ret

def test():
    pattern = midi.read_midifile(path)

    for track in pattern:
        tick = 0
        for event in track:
            #print event.name
            if event.name in ['Note On','Note Off']:
                tick += event.tick

        print tick


folder = 'D:/Files/Project/melody-master/python-midi-master/good songs/RenameData'

def readfolder(filename,ratio=0.8):
    import os
    from datetime import datetime
    filelist = [file for file in os.listdir(folder)]
    if not os.path.exists("test"):
        os.makedirs("test")
    dic = {}
    songkey = []
    songchord = []
    cnt = 0
    length = len(filelist)
    for file in filelist[0:int(length*ratio)]:
        print file
        file_path = os.path.join(folder,file)
        score = StdScore(SolveMidi(file_path))
        tracks = score.tracks

        #if cnt == 0:
        #    print score.getChordFeature()
        songkey.append(score.getKeyFeature())
        songchord.append(score.getChordFeature())
        # with open("test/%s.txt"%file,"w") as fp:
        #     cnt_track = 0
        #     #print "============="
        #     for track in tracks:
        #         cnt = 0
        #
        #         for event in track:
        #             if event == noteEnd:
        #                 cnt += 1
        #         #print 'section',cnt
        #         cnt = 0
        #         print >>fp,'Track',cnt_track
        #         for Note in track:
        #             if Note.pitch == -1:
        #                 print >>fp,'Section End',cnt
        #                 cnt += 1
        #             else:
        #                 print >>fp,Note.pitch,Note.duration
        #         cnt_track += 1

        # with open("feature/%s.txt" % file, "w") as fp:
        #     print >>fp,score.getChordFeature()
        #     print >>fp,"==========================================\n\n\n\n"
        #     print >>fp,score.getKeyFeature()

        cnt += 1
    dic['X'] = songkey
    dic['Y'] = songchord
    with open("%s_train.pkl"%filename,"wb") as fp:
       pickle.dump(dic,fp,-1)


    dic = {}
    songkey = []
    songchord = []
    for file in filelist[int(length*ratio):length]:
        print file
        file_path = os.path.join(folder, file)
        score = StdScore(SolveMidi(file_path))
        songkey.append(score.getKeyFeature())
        songchord.append(score.getChordFeature())

    dic['X'] = songkey
    dic['Y'] = songchord
    with open("%s_train.pkl"%filename,"wb") as fp:
        pickle.dump(dic,fp,-1)


if __name__ == '__main__':
    filename = sys.argv[1]
    readfolder(filename)
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
