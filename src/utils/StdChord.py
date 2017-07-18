import StdNote

class Chord:

    def getChordSet(self):
    	chordSet = [0 for _ in range(StdNote.NOTENUM)]
    	for note in self.notes:
    		chordSet[note.getNoteId()] = 1
    	return chordSet

    def getChordFrequency(self):
    	chordF = [0 for _ in range(StdNote.NOTENUM)]
    	for note in self.notes:
    		chordSet[note.getNoteId()] += 1
    	return chordF

    def getChordTime(self):
    	chordTime = [0 for _ in range(StdNote.NOTENUM)]
    	for note in self.notes:
    		chordSet[note.getNoteId()] += note.getTime()
    	return chordTime

    def __init__(self, chord_tracks):
		self.notes = []
		self.name = ''
		for track in chord_tracks:
			for note in track:
				self.notes.append(note)
		return
