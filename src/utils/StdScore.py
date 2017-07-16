#
import StdChord
import StdNote

class StdScore:
    #property
    tracks = []
    '''
    1st axis: track
    2nd axis: notes (-1 ending)
    '''

    keyTrack = []
    chords = []

    def __init__(self, all_tracks):
        self.tracks = all_tracks

        chords_track = all_tracks
        chords_num = len(all_tracks)
        ptr = [0 for _ in range(chords_num)]
        i = 0
        bar_tracks = []
        flag = True
        while ptr[chords_num-1] < len(chords_track[chords_num-1]):
            i_track = []
            while chords_track[i][ptr[i]].pitch != -1 and ptr[i] < len(chords_track[i]):
                note = chords_track[i][ptr[i]]
                if note.isNote():
                    i_track.append(note)
                ptr[i] += 1
            ptr[i] += 1
            if flag and len(i_track) > 0:
                flag = False
                self.keyTrack.append(i_track)
            else:
                bar_tracks.append(i_track)
            i = (i + 1) % chords_num
            if i == 0:
                if flag == False:
                    self.chords.append(StdChord.Chord(bar_tracks))
                flag = True
                bar_tracks = []

        return

    def getKeyFeature(self):
        keyFeature = []
        for i in range(len(self.keyTrack)):
            feature = [0 for _ in range(2*StdNote.NOTENUM)]
            for note in self.keyTrack[i]:
                feature[note.getNoteId()*2] += 1
                feature[note.getNoteId()*2 + 1] += note.getTime()
            keyFeature.append(feature)

        return keyFeature

    def getChordFeature(self):
        chordFeature = []
        for i in range(len(self.chords)):
            feature = self.chords[i].getChordSet()
            chordFeature.append(feature)
        return chordFeature


if __name__ == "__main__":
    note1 = StdNote.Note(60, [1,4])
    note2 = StdNote.Note(61, [2,4])
    note3 = StdNote.Note(63, [4,4])
    noteStop = StdNote.Note(-2, [4,4])
    noteEnd = StdNote.Note(-1, [0,1])

    track = [[noteStop, note1, note2, note3, noteEnd, noteEnd], [noteEnd, note3, note2, noteEnd]]
    #print(track[1][2].pitch)

    score = StdScore(track)
    print(score.getKeyFeature())
    print(score.getChordFeature())