import StdNote
from fractions import Fraction


class ChordTranslator():
    def __init__(self):
        self.note = {'C': 0, 'D': 2, 'E': 4, 'F': 5, 'G': 7, 'A': 9, 'B': 11, 'Z': -2}
        # C G F Am Em Dm G7
        self.chord = ((0), (0, 4, 7),(7, 11, 2), (5, 9, 0), (9, 0, 4), (4, 7, 11), (2, 5, 9),(7, 11, 5))
        self.dict = ['None', 'C', 'G', 'F', 'Am', 'Em', 'Dm', 'G7']

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

    def translateOutput(self, prob):
        chordFreq = [0 for _ in range(len(self.chord))]
        for i, c in enumerate(self.chord):
            for chordNoteId in c:
                chordFreq [i] += prob[chordNoteId]

        maxFreq = 0
        chordId = 0
        for index in range(len(chordFreq)):
            if(chordFreq[index] > maxFreq):
                chordId = index
                maxFreq = chordFreq[index]

        return chordId
        
    def translateABar(self,notes):
        for item in notes:
            duration += item.duration
            frequency += item.frequency
        id = translateNotesFeature(duration, frequency)
        return chordId

if __name__ == '__main__':
    # muse_score = open("C:/Users/v-hanqw.FAREAST/Desktop/hhdhc.txt").readlines()[0]
    a = ChordTranslator()
    # a = a.fw_run(muse_score)
    bar = [1,2,3,4,5,6]
    ind = a.random_generate(b)
    print(ind)
    # for bar in a:
    #     for note in bar:
    #         print note.pitch, note.duration, ' ',
    #     print
