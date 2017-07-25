import StdNote

threshold = 0.25

class Chord:

    def getChordSet(self):
        chordSet = [0 for _ in range(StdNote.NOTENUM)]
        for note in self.notes:
            chordSet[note.getNoteId()] = 1
        return chordSet

    def getChordFrequency(self):
        chordF = [0 for _ in range(StdNote.NOTENUM)]
        for note in self.notes:
            chordF[note.getNoteId()] += 1
        return chordF

    def getChordTime(self):
        chordTime = [0 for _ in range(StdNote.NOTENUM)]
        for note in self.notes:
            chordTime[note.getNoteId()] += note.getTime()
        return chordTime

    def translateNotesFeature(self, duration, frequency):
        for i in range(StdNote.NOTENUM):
            frequency[i] += int(duration[i] / threshold)
        
        
        chordFreq = [0 for _ in range(len(self.chord))]
        for i, c in enumerate(self.chord):
            for chordNoteId in c:
                chordFreq [i] += frequency[chordNoteId]
        maxFreq = 0
        chordId = 0
        for index in range(len(chordFreq)):
            if(chordFreq[index] > maxFreq):
                chordId = index
                maxFreq = chordFreq[index]

        return chordId

    def getChordId(self):
    	return self.translateNotesFeature(self.getChordTime(),self.getChordFrequency())

    # Chord definition from music aspect

    def getReadableChord(self):
        # C Dm Em F G Am G7
        self.STDCHORD = [
            Chord([[StdNote.Note(0, [1, 4]), StdNote.Note(4, [1, 4]), StdNote.Note(7, [1, 4])]]),
            Chord([[StdNote.Note(2, [1, 4]), StdNote.Note(5, [1, 4]), StdNote.Note(9, [1, 4])]]),
            Chord([[StdNote.Note(4, [1, 4]), StdNote.Note(7, [1, 4]), StdNote.Note(11, [1, 4])]]),
            Chord([[StdNote.Note(5, [1, 4]), StdNote.Note(9, [1, 4]), StdNote.Note(0, [1, 4])]]),
            Chord([[StdNote.Note(7, [1, 4]), StdNote.Note(11, [1, 4]), StdNote.Note(2, [1, 4])]]),
            Chord([[StdNote.Note(9, [1, 4]), StdNote.Note(0, [1, 4]), StdNote.Note(4, [1, 4])]]),
            Chord([[StdNote.Note(7, [1, 4]), StdNote.Note(11, [1, 4]), StdNote.Note(5, [1, 4])]])
            ]
        return self.STDCHORD[0]

    def __init__(self, chord_tracks):
        self.notes = []
        self.name = ''
        self.chord = ((0,), (0, 4, 7),(7, 11, 2), (5, 9, 0), (9, 0, 4), (4, 7, 11), (2, 5, 9),(7, 11, 5))
        for track in chord_tracks:
            for note in track:
                self.notes.append(note)
        return
