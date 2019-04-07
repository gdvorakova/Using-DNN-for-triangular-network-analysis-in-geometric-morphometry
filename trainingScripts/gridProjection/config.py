'''
File name: config.py
Author: Gabriela Dvorakova
Date created: February 24, 2018
Python Version: 3.6.3

This is the script that allows us to modify architecture of a convolutional NN.
'''

from collections import namedtuple
import tensorflow as tf
import os

Layer = namedtuple('Layer', ('name', 'n_nodes'))

home_path = os.path.dirname(
    os.path.dirname(os.getcwd()))

data_path = os.path.join(home_path, "preparedData")

# File with input dataset
FEATURES_FILE = os.path.join(data_path, "projectionsDataset.txt")

# File with labels
LABELS_FILE = os.path.join(data_path, "labels.csv")

# File where the results will be stored
# Will be overwritten if already exists
OUTPUT_FILE = './projectionGridResult.csv'

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

# A probability with which a neuron is kept during dropout
KEEP_RATE = 0.8

# Height of input grid
N_ROWS = 75

# Width of input grid
N_COLUMNS = 87

# Number of channels
N_DEPTH = 3


class Configuration():
    def __init__(self, id):
        self.id = id
