class Note:
    '''
        pitch (int):
                -1: the end of a section
                -2: rest
        duration (pair):
                first key:  numerator
                second key: denominator
    '''
    pitch = 0
    duration = []

    def getNoteName(self):

        return

    def getOctave(self):

        return

    def __init__(self, pitch_, duration_=[1,1]):
        self.pitch = pitch_
        self.duration = duration_

noteEnd = Note(-1)