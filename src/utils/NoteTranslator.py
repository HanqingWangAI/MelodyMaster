import StdNote
from fractions import Fraction


class NoteTranslator():
    def __init__(self):
        self.note = {'C': 0, 'D': 2, 'E': 4, 'F': 5, 'G': 7, 'A': 9, 'B': 11, 'Z': -2}

    def run(self, score):
        # input format: C6040999 D6040999 E6040999 C6040999 ,......
        raw_bar_list = [bar.strip().split(' ') for bar in score.strip().split(',') if bar is not '' and bar is not ' ']
        return_bar_list = list()

        for bar in raw_bar_list:
            return_bar = list()

            for note in bar:
                return_note = self.processed_note(note)
                return_bar.append(return_note)

            return_bar_list.append(return_bar)

        return return_bar_list

    def processed_note(self, note):
        midi_ind = (int(note[1]) + 1) * 12
        midi_dur = Fraction(2 ** (int(note[2:4]) - 6))
        midi_dur = [midi_dur.numerator, midi_dur.denominator]

        if note[0] == 'Z':
            return StdNote.Note(-2, midi_dur)
        else:
            return StdNote.Note(midi_ind + self.note[note[0]], midi_dur)


if __name__ == '__main__':
    muse_score = open("C:/Users/v-hanqw.FAREAST/Desktop/hhdhc.txt").readlines()[0]
    a = NoteTranslator()
    a = a.run(muse_score)
    for bar in a:
        for note in bar:
            print note.pitch,note.duration,' ',
        print
