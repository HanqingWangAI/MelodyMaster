import tensorflow as tf
#import cPickle
import random
import math
from time import clock
import numpy as np
import data
import ChordTranslator


#class_num = 6
time_step = 8
LSTM_unit = 128
#fc_unit = 16
#batch_size = 64
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
    #cell = single_cell()
    init_state = state = cell.zero_state(batch_size,dtype=tf.float32)

    #output_rnn,final_states=tf.nn.dynamic_rnn(cell, input_rnn,initial_state=init_state, dtype=tf.float32)

    mix_w = tf.get_variable("mix_w", [output_size, output_size], dtype=tf.float32)
    mix_b = tf.get_variable("mix_b", [output_size], dtype=tf.float32)
    
    output_rnn = []
    output_mix = []
    #cell_output = tf.constant([0.0 for _ in range(LSTM_unit)])
    with tf.variable_scope("RNN"):
        for i in range(X.get_shape()[1]):
            if i > 0 : tf.get_variable_scope().reuse_variables()

            static_out = tf.matmul(preY[:,i,:], mix_w) + mix_b
            #static_out = cell_output*mix_w+mix_b
            #print(static_out.get_shape()[1])
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

def train_lstm(batch_size=80,time_step=8):
    X=tf.placeholder(tf.float32, shape=[None,time_step,input_size])
    Y=tf.placeholder(tf.float32, shape=[None,time_step,output_size])
    preY=tf.placeholder(tf.float32, shape=[None,time_step,output_size])
    # Y=tf.placeholder(tf.int64, shape=[None, 1])
    #batch_index,train_x,train_y=get_train_data(batch_size,time_step)
    raw_data = data.dataset("Mozart_train1.pkl", batch_size)
    test_data = data.dataset("Mozart_test1.pkl", batch_size)
    print(raw_data.getTotalBatch())
    train_x = [[[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0]\
    ,[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0]]]
    print(len(train_x[0][0]))
    train_y = [[[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0]\
    ,[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0],[1,1,1,0,0,0,0,0,0,0,0,0]]]
    #print train_y[batch_index[0]:batch_index[1]].shape

    logits,_=lstm(X, preY)
    X_shape = X.get_shape()
    print('success')

    reshapeY = tf.reshape(Y, [-1, output_size])
    loss = -tf.reduce_sum(((reshapeY*tf.log(logits + 1e-9)) + ((1-reshapeY) * tf.log(1 - logits + 1e-9))), name='xentropy')
    cost = loss / batch_size

    #correct_pred = tf.equal(tf.argmax(logits,1), tf.reshape(Y,[-1]))
    output_with_threshold = tf.reshape(logits,[-1]) > threshold
    Y_with_threshold = tf.reshape(Y,[-1]) > threshold
    correct_pred = tf.equal(output_with_threshold, Y_with_threshold)
    accuracy = tf.reduce_mean(tf.cast(correct_pred, tf.float32))

    #cost = tf.reduce_sum(loss) / batch_size
    #tf.summary.scalar('loss', cost)
    #lr = tf.Variable(0.0001, trainable=False)
    tvars = tf.trainable_variables()
    grads,_ = tf.clip_by_global_norm(tf.gradients(cost, tvars), 5)
    optimizer = tf.train.AdamOptimizer()
    train_op = optimizer.apply_gradients(zip(grads, tvars))

    train_op=tf.train.AdamOptimizer().minimize(cost)
    saver=tf.train.Saver(tf.global_variables(),max_to_keep=15)
    batch_index=[1,1,1]
    #module_file = tf.train.latest_checkpoint('./')    
    with tf.Session() as sess:
        sess.run(tf.global_variables_initializer())
        #saver.restore(sess, module_file)
        for i in range(100001):
            for step in range(len(batch_index)-2):
                train_x, train_y, train_pre_y = raw_data.getBatch()
                _,loss_,accuracy_,logits_=sess.run([train_op,cost,accuracy,logits],feed_dict={X:train_x,Y:train_y, preY:train_pre_y})
                #_,loss_,accuracy_=sess.run([train_op,cost,accuracy],feed_dict={X:train_x[batch_index[step]:batch_index[step+1]],Y:train_y[batch_index[step]:batch_index[step+1]]})
            if i % 1000 == 0:
                print("train:",i,loss_,accuracy_)
                print("save:",saver.save(sess,'.\Melody.model',global_step=i))
                test_total_loss = 0
                test_total_accuracy = 0
                total_test = int(test_data.getTotalBatch())
                for _ in range(total_test):
                    test_x, test_y, test_pre_y = test_data.getBatch()
                    loss_test,accuracy_test,logits_test,correct_pred_test = sess.run([cost,accuracy,logits,correct_pred],feed_dict={X:test_x, Y:test_y, preY:test_pre_y})
                    test_total_loss += loss_test / total_test
                    test_total_accuracy += accuracy_test / total_test
                print("test:",i,test_total_loss,test_total_accuracy)
                #print(len(correct_pred_test),len(logits_test)*12)
                print(correct_pred_test[48:72])
                print(logits_test[4:6]>0.6)
                #print(logits_test[4:6]>0.3)
                #print(logits_test[0:8]>0.6)
                #print(logits_test[0:8])
                #print(logits_test[0:8]>0.3)
                #for _ in range(8):
                #    print(test_y[0][_])
            #if i % 20==0:
                #print("save:",saver.save(sess,'stock2.model',global_step=i))
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
    #print('Restore success')


def predict(key_feature,pre_Y):
    # X=tf.placeholder(tf.float32, shape=[None,time_step,input_size])
    # preY=tf.placeholder(tf.float32, shape=[None,time_step,output_size])

    # logits,_=lstm(X,preY)

    # module_file = tf.train.latest_checkpoint('./')
    # saver = tf.train.Saver(tf.global_variables())
    # with tf.Session() as sess:
    #     saver.restore(sess, module_file)
    #     print('Restore success')

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
#train_lstm()
# test_data = data.dataset("major-test.pkl", 1)
# key_feature,trueY,preY = test_data.getBatch()
# output = predict(key_feature,preY)[0]
# print(output)
# translator = ChordTranslator.ChordTranslator()
# for i in range(len(output)):
#     ii = translator.translateOutput(output[i])
#     print(translator.dict[ii])
# print("-----------")
# for i in range(len(trueY[0])):
#     ii = translator.translateOutput(trueY[0][i])
#     print(translator.dict[ii])
