'''
File name: trainModelNet1.py
Author: Gabriela Dvorakova
Date created: February 24, 2018
Python Version: 3.6.3

This script first creates the model of convolutional neural network,
,,ModelNet1" and then trains neural network.
'''

import config
import parse
import tensorflow as tf
import numpy as np
import math


def conv2d(x, W):
    return tf.nn.conv2d(
        # 4D Tensor containing the batch of input images
        input=x,
        # 4D Tensor containing the weights of the convolutional filter
        filter=W,
        # How much the convolutional kernel should skip
        # positions in each of the four dimension
        strides=[1, 1, 1, 1],
        # with padding ‘SAME’ the image gets zero-padded
        # and the output size does not change
        padding='SAME')


def max_pool2d(x):
    return tf.nn.max_pool(
        x,
        ksize=[1, 1, 2, 1],
        strides=[1, 2, 2, 1],
        padding='SAME')


def create_model(configuration):
    # input data
    x = tf.placeholder('float')
    y = tf.placeholder('float')

    # Calculate size of fully connected layer
    s_columns = config.N_COLUMNS
    s_columns = math.ceil(s_columns / 2)
    s_columns = math.ceil(s_columns / 2)

    weights = {'W_conv1': tf.Variable(tf.random_normal([1, 3, config.N_DEPTH, 32])),
               'W_conv2': tf.Variable(tf.random_normal([1, 3, 32, 64])),
               'W_fully_connected1': tf.Variable(tf.random_normal(
                    [1*s_columns*64, 1024])),
               'output': tf.Variable(tf.random_normal([1024, config.N_CLASSES]))}
    biases = {'b_conv1': tf.Variable(tf.random_normal([32])),
              'b_conv2': tf.Variable(tf.random_normal([64])),
              'b_fully_connected1': tf.Variable(tf.random_normal([1024])),
              'output': tf.Variable(tf.random_normal([config.N_CLASSES]))}

    x = tf.reshape(x, shape=[
        -1, config.N_ROWS, config.N_COLUMNS,
        config.N_DEPTH])

    # Convolutional Layer 1
    # Uses a 3x3 filter with ReLU activation function.
    # Takes 1 input, computes 32 features.
    conv1 = tf.nn.relu(conv2d(x, weights['W_conv1']) + biases['b_conv1'])

    # Max Pooling Layer 1 (down-sampling)
    # Size of pooling filter is 2x2x2 and a stride of 2
    pool1 = max_pool2d(conv1)

    # Convolutional Layer 2
    # Uses a 3x3 filter with ReLU activation function.
    # Takes 32 inputs, computes 64 features.
    conv2 = tf.nn.relu(conv2d(pool1, weights['W_conv2']) + biases['b_conv2'])

    # Max Pooling Layer 1 (down-sampling)
    # Size of pooling filter is 2x2x2 and a stride of 2
    pool2 = max_pool2d(conv2)

    # Flatten layer 1
    # Reshape pool2 output to fit fully connected layer
    # Flatten tensor into a batch of vectors
    flatten1 = tf.reshape(pool2, [-1, 1 * s_columns * 64])

    # Fully connected layer
    fully_connected1 = tf.nn.relu(
        tf.matmul(flatten1, weights['W_fully_connected1'])
        + biases['b_fully_connected1'])

    # Dropout layer
    # 80% of neurons will be kept
    dropout = tf.nn.dropout(
        fully_connected1,
        config.KEEP_RATE)

    output = tf.matmul(dropout, weights['output']) + biases['output']
    return x, y, output


def train_convolutional_neural_network(
        train, train_labels, test, test_labels, configuration, shuffle):
    tf.reset_default_graph()
    x, y, prediction = create_model(configuration)

    cost = tf.reduce_mean(
        tf.nn.softmax_cross_entropy_with_logits(logits=prediction, labels=y))
    optimizer = tf.train.AdamOptimizer(
        config.LEARNING_RATE).minimize(cost)

    with tf.Session() as sess:
        sess.run(tf.global_variables_initializer())

        # Repeat training for N_EPOCHS
        for epoch in range(config.N_EPOCHS):
            epoch_loss = 0
            i = 0

            train_np = np.array(train)
            train_labels_np = np.array(train_labels)

            if (shuffle is True):
                # shuffle data in each epoch
                train_np, train_labels_np = parse.shuffle_data(
                    train_np, train_labels_np)

            while i < len(train):
                start = i
                end = i + config.N_BATCH

                batch_x = np.array(train_np[start:end], dtype=object)
                batch_y = np.array(train_labels_np[start:end])
                _, c = sess.run(
                    [optimizer, cost], feed_dict={x: batch_x, y: batch_y})
                epoch_loss += c
                i += config.N_BATCH

            # After each epoch, calculate loss
            s = 'Epoch ' + str(epoch) + ', loss: ' + str(epoch_loss)
            print(s)

        correct = tf.equal(tf.argmax(prediction, 1), tf.argmax(y, 1))
        accuracy = tf.reduce_mean(tf.cast(correct, 'float'))
        accuracy_value = accuracy.eval({x: test, y: test_labels})

        return accuracy_value
