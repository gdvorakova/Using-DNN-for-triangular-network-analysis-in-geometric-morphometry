'''
File name: train.py
Author: Gabriela Dvorakova
Date created: December 17, 2017
Python Version: 3.6.3

This script first creates the model of neural network and then trains
neural network.
'''

import config
import parse
import tensorflow as tf
import numpy as np


def create_model(configuration):
    # Input sample
    x = tf.placeholder(
        'float', shape=[None, configuration.n_nodes_input])
    # Label
    y = tf.placeholder('float')

    # Create a hidden layer that consist of weights and biases of given shape
    def add_weight_and_bias(in_nodes, out_nodes):
        weight = tf.random_normal([in_nodes.n_nodes, out_nodes.n_nodes])
        bias = tf.Variable(tf.random_normal([out_nodes.n_nodes]))

        layer = {
            'weights': tf.Variable(tf.Variable(weight)),
            'biases': tf.Variable(bias)
            }
        return layer

    # Create a list of weights and biases for each layer
    weights_and_biases = [add_weight_and_bias(
        layer, output_layer) for layer, output_layer in zip(
        configuration.layers_list, configuration.layers_list[1:])]

    previous = tf.add(tf.matmul(
        x, weights_and_biases[0]['weights']), weights_and_biases[0]['biases'])

    # Multiply values in consecutive layers
    i = 1
    end = len(weights_and_biases) - 1
    for layer in weights_and_biases[1:end]:
        hidden_layer = tf.add(tf.matmul(
            previous, weights_and_biases[i]['weights']),
            weights_and_biases[i]['biases'])
        hidden_layer = config.ACTIVATION_FUNCTION(hidden_layer)
        previous = hidden_layer
        i += 1

    # Output is a one-hot value in an array
    output = tf.matmul(
        previous, weights_and_biases[end]['weights']) + weights_and_biases[
        end]['biases']

    return x, y, output


def train_neural_network(
        train, train_labels, test, test_labels, configuration, shuffle):
    tf.reset_default_graph()
    # Prepare tensor shapes, hidden layers, weights and biases
    x, y, prediction = create_model(configuration)

    cost = tf.reduce_mean(
        tf.nn.softmax_cross_entropy_with_logits(logits=prediction, labels=y))
    optimizer = tf.train.AdamOptimizer(
        config.LEARNING_RATE).minimize(cost)

    with tf.Session() as sess:
        sess.run(tf.global_variables_initializer())

        for epoch in range(config.N_EPOCHS):
            epoch_loss = 0
            i = 0

            # Shuffle data in each epoch
            if (shuffle is True):
                train, train_labels = parse.shuffle_data(train, train_labels)

            # Present whole training set to neural network in batches
            while i < len(train):
                start = i
                end = i + config.N_BATCH

                batch_x = np.array(train[start:end])
                batch_y = np.array(train_labels[start:end])

                _, c = sess.run(
                    [optimizer, cost], feed_dict={x: batch_x, y: batch_y})

                # Count lost
                epoch_loss += c
                i += config.N_BATCH

            # After each epoch, calculate loss
            s = 'Epoch ' + str(epoch) + ', loss: ' + str(epoch_loss)
            print(s)

        correct = tf.equal(tf.argmax(prediction, 1), tf.argmax(y, 1))
        accuracy = tf.reduce_mean(tf.cast(correct, 'float'))
        accuracy_value = accuracy.eval({x: test, y: test_labels})

        return accuracy_value
