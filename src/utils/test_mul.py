import tensorflow as tf
#import cPickle
import random
import math
from time import clock
import numpy as np
import data
import ChordTranslator


time_step = 8
LSTM_unit = 128
input_size = 24
output_size = 12
lr = 0.05
num_layers = 2
threshold = 0.6
prob = 0.9

weights={
         'in':tf.Variable(tf.random_normal([input_size,LSTM_unit])),
         'out':tf.Variable(tf.random_normal([LSTM_unit,1]))
         }
biases={
        'in':tf.Variable(tf.constant(1.0,shape=[LSTM_unit,])),
        'out':tf.Variable(tf.constant(1.0,shape=[1,]))
        }

def lstm(X, preY): 
    batch_size=tf.shape(X)[0]
    time_step=tf.shape(X)[1]
    num_steps = X.get_shape()[1]
    w_in=weights['in']
    b_in=biases['in']

    input=tf.reshape(X,[-1,input_size])
    input_rnn=tf.matmul(input,w_in)+b_in
    input_rnn=tf.reshape(input_rnn,[-1,time_step,LSTM_unit])
    def single_cell():
        return tf.contrib.rnn.BasicLSTMCell(
                LSTM_unit, forget_bias=1.0, state_is_tuple=True,
                reuse=tf.get_variable_scope().reuse)

    def drop_cell():
        return tf.contrib.rnn.DropoutWrapper(
            single_cell(), output_keep_prob=prob)

    cell = tf.contrib.rnn.MultiRNNCell([drop_cell() for _ in range(num_layers)])
    init_state = state = cell.zero_state(batch_size,dtype=tf.float32)

    mix_w = tf.get_variable("mix_w", [output_size, output_size], dtype=tf.float32)
    mix_b = tf.get_variable("mix_b", [output_size], dtype=tf.float32)
    
    output_rnn = []
    output_mix = []
    with tf.variable_scope("RNN"):
        for i in range(X.get_shape()[1]):
            if i > 0 : tf.get_variable_scope().reuse_variables()

            static_out = tf.matmul(preY[:,i,:], mix_w) + mix_b
            output_mix.append(static_out)
            cell_output, state = cell(input_rnn[:, i, :], state)
            output_rnn.append(cell_output)
    final_states = state
    
    #inputs = tf.unstack(input_rnn, num=num, axis=1)
    #output, final_states = tf.contrib.rnn.static_rnn(
    #     cell, inputs, initial_state=init_state)
    output = tf.reshape(tf.stack(axis=1, values=output_rnn), [-1, LSTM_unit])
    output_static = tf.reshape(tf.stack(axis=1, values=output_mix), [-1, output_size])
    #output=tf.reshape(output_rnn,[-1,LSTM_unit])


    softmax_w = tf.get_variable(
        "softmax_w", [LSTM_unit, output_size], dtype=tf.float32)
    softmax_b = tf.get_variable("softmax_b", [output_size], dtype=tf.float32)
    fc_out = tf.matmul(output, softmax_w) + softmax_b
    logits = tf.nn.sigmoid(fc_out + output_static)

    #w_out=weights['out']
    #b_out=biases['out']
    #pred=tf.matmul(output,w_out)+b_out
    return logits,final_states

sess = 0
logits = 0
X = 0
preY = 0
def restore(time_step):
    global X
    global preY
    X=tf.placeholder(tf.float32, shape=[None,time_step,input_size])
    preY=tf.placeholder(tf.float32, shape=[None,time_step,output_size])

    global logits
    logits,_=lstm(X,preY)

    module_file = tf.train.latest_checkpoint('./')
    saver = tf.train.Saver(tf.global_variables())
    global sess
    sess = tf.Session()
    saver.restore(sess, './../../../../src/utils/Melody.model-100000')


def predict(key_feature,pre_Y):

    prediction = sess.run([logits],feed_dict={X:key_feature,preY:pre_Y})
    return prediction

def predictId(key_feature,pre_Y,times,first):
    if first:
        restore(times)
    output = predict(key_feature,pre_Y)[0]
    translator = ChordTranslator.ChordTranslator()
    outlist = []
    for i in range(len(output)):
        ii = translator.translateOutput(output[i])
        outlist.append(translator.dict[ii])
    return outlist, output