import StdNote
from fractions import Fraction
import numpy as np


class NoteTranslator():
    def __init__(self):
        self.note = {'C': 0, 'D': 2, 'E': 4, 'F': 5, 'G': 7, 'A': 9, 'B': 11, 'Z': -2}
        self.chord = ((0, 4, 7), (2, 5, 9), (4, 7, 11), (5, 9, 0), (7, 11, 2), (9, 0, 4), (7, 11, 5))

    def fw_run(self, score):
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

    def bw_run(self, chord):
        for bar in chord:
            process_bar(bar)

    def processed_bar(self, bar):
        bar_score = []
        bar_score_ind = []
        for chord in self.chord:
            bar_score.append(bar[chord[0]] + bar[chord[1]] + bar[chord[2]])
            bar_score_ind.append(bar[chord[0]] + bar[chord[1]] + bar[chord[2]])
        bar_score.sort()
        bar_score.reverse()
        ind = self.random_generate(bar_score)
        return bar_score_ind.index(bar_score[ind])

    def random_generate(self, bar_score):
        r = np.random.rand()
        bar = np.asarray(bar_score)
        bar = bar / sum(float(bar))
        for i in range(len(bar)):
            if i == 0:
                if r < bar[0]:
                    return i
            else:
                if sum(bar[0:i]) < r < sum(bar[0:i + 1]):
                    return i


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
