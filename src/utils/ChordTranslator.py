import StdNote
from fractions import Fraction


class NoteTranslator():
    def __init__(self):
        self.note = {'C': 0, 'D': 2, 'E': 4, 'F': 5, 'G': 7, 'A': 9, 'B': 11, 'Z': -2}
        # C Dm Em F G Am G7
        self.chord = ((0, 4, 7), (2, 5, 9), (4, 7, 11), (5, 9, 0), (7, 11, 2), (9, 0, 4), (7, 11, 5))

    def translateNotesFeature(self, duration, frequency):

        return chordId

    def translateABar(self,notes):
        for item in notes
            duration += item.duration
            frequency += item.frequency
        id = translateNotesFeature(duration, frequency)
        return chordId

if __name__ == '__main__':
    # muse_score = open("C:/Users/v-hanqw.FAREAST/Desktop/hhdhc.txt").readlines()[0]
    a = NoteTranslator()
    # a = a.fw_run(muse_score)
    bar = [1,2,3,4,5,6]
    ind = a.random_generate(b)
    print(ind)
    # for bar in a:
    #     for note in bar:
    #         print note.pitch, note.duration, ' ',
    #     print
