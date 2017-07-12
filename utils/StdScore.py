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

    chords = []

    def __init__(self, all_tracks):
        self.tracks = all_tracks

        chords_track = all_tracks[1:]
        chords_num = len(all_tracks) - 1
        ptr = [0 for i in range(chords_num)]
        i = 0
        bar_tracks = []
        while ptr[0] < len(chords_track[0]):
            i_track = []
            while chords_track[i][ptr[i]].pitch != -1 and ptr[i] < len(chords_track[i]):
                i_track.append(chords_track[i][ptr[i]])
                ptr[i] += 1
            ptr[i] += 1
            bar_tracks.append(i_track)
            i = (i + 1) % chords_num
            if i == 0:
                self.chords.append(StdChord.Chord(bar_tracks))
                print("success!")
                bar_tracks.clear()

        return

if __name__ == "__main__":
    note1 = StdNote.Note(60, 2)
    note2 = StdNote.Note(61, 4)
    note3 = StdNote.Note(63, 8)
    noteEnd = StdNote.Note(-1, 0)

    track = [[note1, note2, note3, noteEnd], [note3, note2, noteEnd]]
    print(track[1][2].pitch)

    score = StdScore(track)