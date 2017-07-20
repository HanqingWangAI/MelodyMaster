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

	# Chord definition from music aspect


	def getReadableChord(self):
		self.STDCHORD = [
		Chord([StdNote.Note(0, [1,4]), StdNote.Note(4, [1,4]), StdNote.Note(7, [1,4])]),
		Chord([StdNote.Note(2, [1, 4]), StdNote.Note(5, [1, 4]), StdNote.Note(9, [1, 4])])
	]
		return self.STDCHORD[0]


    def __init__(self, chord_tracks):
		self.notes = []
		self.name = ''
		for track in chord_tracks:
			for note in track:
				self.notes.append(note)
		return

