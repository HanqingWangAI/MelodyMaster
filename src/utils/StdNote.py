NOTENUM = 12

class Note:
    '''
        pitch (int):
                -1: the end of a section
                -2: rest
        duration (pair):
                first key:  numerator
                second key: denominator
    '''

    def getNoteId(self):
        return (self.pitch % 12)

    def getOctave(self):
        return (self.pitch / 12)

    def isNote(self):
        return (self.pitch >= 0)

    def getTime(self):
        return (1.0*self.duration[0]/self.duration[1])

    def __init__(self, pitch_, duration_=[1,1]):
        self.pitch = pitch_
        self.duration = duration_

noteEnd = Note(-1)