'''
File name: parse.py
Author: Gabriela Dvorakova
Date created: March 24, 2018
Python Version: 3.6.3

This script takes transforms input text data files into data structure
that fits convolutional neural networks.
'''

import csv
import sys
import numpy as np
import config


def create_feature_label_set(features_file, labels_file):
    """Parse input files.
    Return an array of features and list of labels.

    parameters:
        features_file -- filename of the file with grid of coordinates
        separated by a colon, one example per line

            e.g.: 4.027986 -24.92605 19.12924,-2.53098 30.35624...

        label_file -- filename of a .csv file with classes,
        one example per line

            columns: id,sex,weight.kg,height.cm,age.yr,group.idx,sample,valid

            e.g.: 0,M,88,175,79,3,mima,TRUE

    output:
        feature_set, label_set

        feature_set: [[[[0, 0, 0], [0, 0, 0], [0, 0, 0]],
                     [[0, 0, 0], [0, 0, 0], [0, 0, 0]]],...]

        label_set -- [0,1] -- male
                     [1,0] -- female
                     [[0,1], [1,0], [0,1], [0,1]...]
    """

    label_set = []

    # Read label file
    with open(labels_file, 'r') as file:
        rownum = 0
        data_reader = csv.reader(file)
        for line in data_reader:
            if rownum:
                label = []
                if line[1] == 'M':
                    label = [0, 1]
                elif line[1] == 'F':
                    label = [1, 0]
                label_set.append(label)
            rownum += 1

    feature_set = []

    # Read feature file
    with open(features_file, 'r') as file:
        count = 0
        data_size = 0
        row_list = []
        # Each line represents one row of a grid
        for line in file:
            col_list = []
            count += 1
            # Get array of vertex coordinates
            current_line = line.split(",")
            # Create list of features
            coord_list = []
            # Read data into features
            for vertex in current_line:
                current_vertex = vertex.split()
                for i in current_vertex:
                    # Add coordinates to coord_list
                    coord_list.append(float(i))
                # coord_list is now of length 3 (x, y, z coordinates)
                col_list.append(coord_list)
                coord_list = []
            row_list.append(col_list)

            if count % config.N_ROWS == 0:
                feature_set.append(row_list)
                row_list = []
                data_size += 1

    return feature_set, label_set, data_size


def create_data_set(feature_set, label_set):
    """Take feature set and label set
    and merge them into a data_set, where each training example
    has a label appended at the end of the list.
    """
    # Iterate through feature set
    for i in range(0, len(label_set)):
        # Append a label to each example and add to data_set list
        feature_set[i].append(label_set[i])

    return feature_set


def feature_label_from_dataset(data_set):
    """Take data_set and return data_set split into
    feature_set and label_set

    output: shuffled feature_set and equivalently shuffled label_set
    """
    feature_set = []
    label_set = []
    size = len(data_set[0]) - 1

    for entry in data_set:
        feature_set.append(entry[:size])
        label_set = label_set + entry[size:]

    return feature_set, label_set


def shuffle_data(feature_set, labels):
    """Take feature set and shuffle
    the ordering of data examples in the set

    output: shuffled feature_set and equivalently shuffled label_set
    """
    permutation = np.random.permutation(labels.shape[0])
    shuffled_dataset = feature_set[permutation, :, :]
    shuffled_labels = labels[permutation]
    return shuffled_dataset, shuffled_labels


if __name__ == "__main__":
    feature_set, label_set = create_feature_label_set(sys.argv[1], sys.argv[2])
    data_input = create_data_set(feature_set, label_set)
