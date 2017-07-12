import midi
from collections import deque
path = '../233.mid'

pattern = midi.read_midifile(path)

def SolveMidi(path):
    pattern = midi.read_midifile(path)
    res = pattern.resolution
    with open(path[:-4]+"parse.txt","w") as fp:
        n_track = 0
        for track in pattern[1:]:
            Q = deque([])
            print >> fp,'Track:',n_track
            n_track += 1
            tick = 0
            for event in track:
                if event.name not in ['Note On','Note Off']:
                    continue

                tick += event.tick

                if event.name == 'Note Off' or event.velocity == 0:
                    temp = Q.popleft()
                    if temp == event.pitch:
                        print >>fp, event.tick, temp
                if event.name == 'Note On' and event.velocity != 0:
                    Q.append(event.pitch)

                while tick >= res*4:
                    tick -= res * 4
                    print >>fp, "section end"

def test():
    pattern = midi.read_midifile(path)

    for track in pattern:
        tick = 0
        for event in track:
            #print event.name
            if event.name in ['Note On','Note Off']:
                tick += event.tick

        print tick

if __name__ == '__main__':
    SolveMidi(path)
    #test()
