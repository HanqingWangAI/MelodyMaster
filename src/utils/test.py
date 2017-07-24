import tensorflow as tf
import pickle
import random
import math
from time import clock
import numpy as np
import data
import ChordTranslator

labels = ["lane_keeping", "left_post_lane_change", "left_pre_lane_change", \
"right_post_lane_change", "right_pre_lane_change"]

class_num = 8
time_step = 8
LSTM_unit = 128
#fc_unit = 16
#batch_size = 64
input_size = 24
output_size = 12
lr = 0.05
num_layers = 2
threshold = 0.6
prob = 0.98

weights={
         'in':tf.Variable(tf.random_normal([input_size,LSTM_unit])),
         'out':tf.Variable(tf.random_normal([LSTM_unit,1]))
         }
biases={
        'in':tf.Variable(tf.constant(1.0,shape=[LSTM_unit,])),
        'out':tf.Variable(tf.constant(1.0,shape=[1,]))
        }

def get_train_data(batch_size=60,time_step=10):
    data = cPickle.load(open("train_data_didi.pik", 'r'))
    train_x = data['features']
    train_y = data['labels']
    train_x = train_x.swapaxes(0,1)
    train_y = train_y.swapaxes(0,1)
    new_train_y = train_y[:,0]
    new_train_y = new_train_y[:,np.newaxis]
    #train_y = train_y[:,:,np.newaxis]
    #train_y = np.zeros((raw_y.shape[0], raw_y.shape[1], class_num), dtype = np.float32)
    batch_index = []
    for i in range(len(train_x)):
        if i % batch_size == 0:
            batch_index.append(i)
        # for j in range(time_step):
        #     train_y[i,j,raw_y[i,j]] = 1
    batch_index.append(len(train_x)-1)
    return batch_index, train_x, train_y

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

    mix_w = tf.get_variable("mix_w", [class_num, class_num], dtype=tf.float32)
    mix_b = tf.get_variable("mix_b", [class_num], dtype=tf.float32)

    #output_rnn,final_states=tf.nn.dynamic_rnn(cell, input_rnn,initial_state=init_state, dtype=tf.float32)
    
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
    output_static = tf.reshape(tf.stack(axis=1, values=output_mix), [-1, class_num])
    #output=tf.reshape(output_rnn,[-1,LSTM_unit])


    # fcn_w = tf.get_variable(
    #     "fcn_w", [LSTM_unit, fc_unit], dtype=tf.float32)
    # fcn_b = tf.get_variable("fcn_b", [fc_unit], dtype=tf.float32) 
    # fc = tf.matmul(output, fcn_w) + fcn_b
    # fc_out = tf.reshape(fc, [-1,time_step*fc_unit])

    # softmax_w = tf.get_variable(
    #     "softmax_w", [num_steps*fc_unit, class_num], dtype=tf.float32)
    # softmax_b = tf.get_variable("softmax_b", [class_num], dtype=tf.float32)

    # logits = tf.nn.softmax(tf.matmul(fc_out, softmax_w) + softmax_b)

    softmax_w = tf.get_variable(
        "softmax_w", [LSTM_unit, class_num], dtype=tf.float32)
    softmax_b = tf.get_variable("softmax_b", [class_num], dtype=tf.float32)
    fc_out = tf.matmul(output, softmax_w) + softmax_b
    logits = tf.nn.softmax(fc_out+output_static)

    #w_out=weights['out']
    #b_out=biases['out']
    #pred=tf.matmul(output,w_out)+b_out
    return logits,final_states

def train_lstm(batch_size=80,time_step=8):
    X=tf.placeholder(tf.float32, shape=[None,time_step,input_size])
    Y=tf.placeholder(tf.int64, shape=[None,time_step])
    preY=tf.placeholder(tf.float32, shape=[None,time_step,class_num])
    # Y=tf.placeholder(tf.int64, shape=[None, 1])
    #batch_index,train_x,train_y=get_train_data(batch_size,time_step)
    raw_data = data.dataset("C-major_train3.pkl", batch_size)
    test_data = data.dataset("c-major-test-4_train1.pkl", batch_size)
    #print train_y[batch_index[0]:batch_index[1]].shape

    logits,_=lstm(X, preY)
    X_shape = X.get_shape()
    #loss
    #loss=tf.reduce_mean(tf.square(tf.reshape(pred,[-1])-tf.reshape(Y, [-1])))
    #loss = -tf.reduce_sum(tf.reshape(Y,[-1,class_num])*tf.log(logits))

    # loss_list = []
    # for i in range(batch_size):
    #     for j in range(time_step):
    #         v = math.e**(-2*(time_step-1-j))
    #         loss_list.append(v)
    # loss_weight = tf.constant(loss_list)

    loss_weight = tf.ones([batch_size*time_step])

    loss = tf.contrib.legacy_seq2seq.sequence_loss_by_example(
        [logits],
        [tf.reshape(Y, [-1])],
        [loss_weight],
        class_num)
    #loss = tf.nn.sparse_softmax_cross_entropy_with_logits(logits = logits, labels = tf.reshape(Y,[-1]))

    cost = tf.reduce_mean(loss)
    predict = tf.argmax(logits,1)

    correct_pred = tf.equal(tf.argmax(logits,1), tf.reshape(Y,[-1]))
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
    #module_file = tf.train.latest_checkpoint('./')    
    with tf.Session() as sess:
        sess.run(tf.global_variables_initializer())
        #saver.restore(sess, module_file)
        for i in range(10001):
            train_x, train_y, train_pre_y = raw_data.getBatch()
            _,loss_,accuracy_,pred_train=sess.run([train_op,cost,accuracy,predict],feed_dict={X:train_x,Y:train_y,preY:train_pre_y})
            if i % 100 == 0:
                print("train:",i,loss_,accuracy_)
                #print("save:",saver.save(sess,'.\Melody.model',global_step=i))
                #print(pred_train)
                test_total_loss = 0
                test_total_accuracy = 0
                total_test = int(test_data.getTotalBatch())
                for _ in range(total_test):
                    test_x, test_y, test_pre_y = test_data.getBatch()
                    loss_test,accuracy_test,logits_test,correct_pred_test,pred_test = sess.run([cost,accuracy,logits,correct_pred,predict],feed_dict={X:test_x, Y:test_y,preY:test_pre_y})
                    test_total_loss += loss_test / total_test
                    test_total_accuracy += accuracy_test / total_test
                print("test:",i,test_total_loss,test_total_accuracy)
                translator = ChordTranslator.ChordTranslator()
                #print(logits_test[0:8])
                print(pred_test)
                #print(correct_pred_test)
                print(test_y)
                #for j in range(len(logits_test)):
                #    ii = translator.translateOutput(output[j])
                #    print(translator.dict[ii])
            #if i % 20==0:
                #print("save:",saver.save(sess,'stock2.model',global_step=i))

sess = 0
logits = 0
predict = 0
X = 0
preY = 0
def restore(time_step):
    global X
    global preY
    X=tf.placeholder(tf.float32, shape=[None,time_step,input_size])
    preY=tf.placeholder(tf.float32, shape=[None,time_step,class_num])

    global logits
    global predict
    logits,_=lstm(X,preY)
    predict = tf.argmax(logits,1)

    module_file = tf.train.latest_checkpoint('./')
    saver = tf.train.Saver(tf.global_variables())
    global sess
    sess = tf.Session()
    saver.restore(sess, 'C:/Users/v-donye/Desktop/New folder/MelodyMaster/src/utils/Melody.model-3000')

def predict(key_feature,pre_Y):
    X=tf.placeholder(tf.float32, shape=[None,8,input_size])
    preY=tf.placeholder(tf.float32, shape=[None,8,class_num])

    logits,_=lstm(X,preY)
    predict = tf.argmax(logits,1)

    module_file = tf.train.latest_checkpoint('./')
    saver = tf.train.Saver(tf.global_variables())
    translator = ChordTranslator.ChordTranslator()
    output = []
    with tf.Session() as sess:
        saver.restore(sess, module_file)
        print('Restore success')

        prediction = sess.run([predict],feed_dict={X:key_feature,preY:pre_Y})
        for key in prediction[0]:
            output.append(translator.dict[key])
    return output

def predictId(key_feature,pre_Y,time_step,first):
    if first:
        restore(time_step)
    output, logits_out = sess.run([predict,logits],feed_dict={X:key_feature,preY:pre_Y})
    translator = ChordTranslator.ChordTranslator()
    outlist = []
    for i in range(len(output)):
        outlist.append(translator.dict[output[i]])
    return outlist, logits_out

#train_lstm()
# test_data = data.dataset("c-major-test-4_train1.pkl", 1)
# for i in range(1):
#     key_feature,trueY,preY = test_data.getBatch()
# output = predictId(key_feature,preY,8,0==0)
# print(output)
# print(trueY)