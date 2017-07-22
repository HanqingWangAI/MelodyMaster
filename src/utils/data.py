import pickle
import random
import math
import numpy as np

BLOCKSIZE = 4
UNROLL = 2 * BLOCKSIZE

class dataset:

    def getTotalBatch(self):
        return ((self.total_num - 1)/self.batch_size + 1)

    def oneHot(self, chords):
        oneHotlist = []
        for i in range(len(chords)):
            tmp = [0 for _ in range(8)]
            tmp[chords[i]] = 1
            oneHotlist.append(oneHotlist)
        return oneHotlist

    def getBatch(self):
        X = []
        Y = []
        preY = []
        zeroY = [[0 for _ in range(12)]]
        if self.is_id == 1:
            zeroY = [[0 for _ in range(8)]]
        cnt = 0
        while cnt < self.batch_size:
            if (self.index_feature+UNROLL)  > len(self.data['X'][self.index_song]):
                self.index_song = (self.index_song + 1) % len(self.data['X'])
                continue
            X.append(self.data['X'][self.index_song][self.index_feature:self.index_feature+UNROLL])
            Y.append(self.data['Y'][self.index_song][self.index_feature:self.index_feature+UNROLL])
            if self.is_id == 0:
                preY.append(zeroY+self.data['Y'][self.index_song][self.index_feature:self.index_feature+UNROLL-1])
            else:
                preY.append(zeroY+self.oneHot(self.data['Y'][self.index_song][self.index_feature:self.index_feature+UNROLL-1]))
            self.index_feature += BLOCKSIZE
            if (self.index_feature+UNROLL)  > len(self.data['X'][self.index_song]):
                self.index_feature = 0
                self.index_song = (self.index_song + 1) % len(self.data['X'])
            cnt += 1
        return X, Y, preY


    def __init__(self, path_to_data, batch_size):
        with open(path_to_data, 'rb') as f:
            self.data = pickle.load(f)
        self.batch_size = batch_size

        self.total_num = 0
        for song in self.data['X']:
            self.total_num += int((len(song) / BLOCKSIZE)) - 1
        if type(self.data['Y'][0][0]) == type(1):
            self.is_id = 1
        else:
            self.is_id = 0
        self.index_song = 0
        self.index_feature = 0
        return