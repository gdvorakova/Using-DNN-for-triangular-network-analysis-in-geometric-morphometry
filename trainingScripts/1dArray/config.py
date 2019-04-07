'''
File name: config.py
Author: Gabriela Dvorakova
Date created: February 24, 2018
Python Version: 3.6.3

This is the script that allows us to modify architecture of classic NN.
'''

from collections import namedtuple
import tensorflow as tf
import os

# Data structure representing a hidden layer
Layer = namedtuple('Layer', ('name', 'n_nodes'))

home_path = os.path.dirname(
    os.path.dirname(os.getcwd()))

data_path = os.path.join(home_path, "preparedData")

# File with input dataset
FEATURES_FILE = os.path.join(data_path, "nose1dArrayDataset.txt")

# File with labels
LABELS_FILE = os.path.join(data_path, "labels.csv")

# File where the results will be stored
# Will be overwritten if already exists
OUTPUT_FILE = './1dArrayResults.csv'

# Function that is applied on the output of each neuron
ACTIVATION_FUNCTION = tf.nn.relu

# k in k-fold cross validation
K = 10

# Size of learning step
LEARNING_RATE = 0.001

# Number of training samples that are going to be propagated
# through the network in a single batch
N_BATCH = 100

# Number of times that the whole dataset is going to be propagated
# through the network
N_EPOCHS = 100

# Number of output classes
N_CLASSES = 2


class Configuration():
    def __init__(
            self, n_input, n_hidden_layers, hidden_layer_nodes_list, id):
        # Number of nodes in input layer
        self.n_nodes_input = n_input  # = 9492
        # Number of hidden layers
        self.n_hidden_layers = n_hidden_layers
        # List of number of nodes in each hidden layer
        # hidden_layer_nodes_list[0] = number of nodes in hidden layer 1...
        self.hidden_layer_n_nodes_list = hidden_layer_nodes_list
        self.layers_list = self.get_layers()
        self.id = id

    # Creates list of hidden layers
    def get_layers(self):
        layers_list = [Layer('input_layer', self.n_nodes_input)]
        name_hidden_layer = 'hidden_layer_'
        for i in range(0, self.n_hidden_layers):
            name_index = str(i + 1)
            name = name_hidden_layer + name_index
            layers_list.append(Layer(name, self.hidden_layer_n_nodes_list[i]))
        layers_list.append(Layer('output_layer', N_CLASSES))

        return layers_list
