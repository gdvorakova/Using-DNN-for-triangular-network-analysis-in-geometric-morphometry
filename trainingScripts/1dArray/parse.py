'''
File name: parse.py
Author: Gabriela Dvorakova
Date created: December 17, 2017
Python Version: 3.6.3

This script takes transforms input text data files into data structure s
that fit neural networks.
'''

import csv
import sys
import random


def create_feature_label_set(features_file, labels_file):
    """Parse input files.
    Return a list of features and list of labels.

    parameters:
        features_file -- filename of the file with coordinates
        separated by a colon, one example per line

            e.g.: 4.027986 -24.92605 19.12924,-2.53098 30.35624...

        label_file -- filename of a .csv file with classes,
        one example per line

            columns: id,sex,weight.kg,height.cm,age.yr,group.idx,sample,valid

            e.g.: 0,M,88,175,79,3,mima,TRUE

    output:
        feature_set, label_set

        feature_set: [[4.027986, -24.92605, 19.12924, -2.53098, 30.35624...],
                      [12.61018, 5.998771, 13.81683, 17.29926, 3.555573...]]

        label_set -- [0,1] -- male
                     [1,0] -- female
                     [[0,1], [1,0], [0,1], [0,1]...]
    """

    label_set = []
    column = 1
    # Read label file
    with open(labels_file, 'r') as file:
        rownum = 0
        data_reader = csv.reader(file)
        for line in data_reader:
            if rownum:
                label = []
                if line[column] == 'M':
                    label = [0, 1]
                elif line[column] == 'F':
                    label = [1, 0]
                label_set.append(label)
            rownum += 1

    feature_set = []

    # Read feature file
    with open(features_file, 'r') as file:
        for line in file:
            line = line.replace("\x00", '').replace("\n", '')
            current_line = line.split(",")
            # Create list of features
            features = []
            # Read data into features
            for vertex in current_line:
                current_vertex = vertex.split()
                for i in current_vertex:
                    features.append(float(i))
            features = list(features)
            feature_set.append(features)
    return feature_set, label_set


def create_data_set(feature_set, label_set):
    """Take feature set and label set
    and merge them into a data_set.

    parameters:
        feature_set -- [[features_example1], [features_example2]...]

            e.g.: [[4.027986, -24.92605, 19.12924, -2.53098, 30.35624...],
                   [12.61018, 5.998771, 13.81683, 17.29926, 3.555573...]]

        label_set -- [label_example1, label_example2...]

            e.g.: [[0,1], [1,0], [0,1], [0,1]...]

    output:
        data_set -- list of features together with their label

            e.g.: [[4.027986, -24.92605, 19.12924, -2.53098, 30.35624..., [0,1]],
                   [12.61018, 5.998771, 13.81683, 17.29926, 3.555573..., [1,0]]]
    """

    # Iterate over feature set
    feature_set_copy = list(feature_set)
    for i in range(len(feature_set_copy)):
        # Append a label to each example and add to data_set list
        feature_set_copy[i].append(label_set[i])

    return feature_set


def feature_label_from_dataset(data_set):
    """Take data_set and return data_set split into
    feature_set and label_set

    parameters:
        data_set -- list of features together with their label

        e.g.: [[4.027986, -24.92605, 19.12924, -2.53098, 30.35624..., [0,1]],
               [12.61018, 5.998771, 13.81683, 17.29926, 3.555573..., [1,0]]]

    output:
        feature_set -- [[features_example1], [features_example2]...]

            e.g.: [[4.027986, -24.92605, 19.12924, -2.53098, 30.35624...],
                   [12.61018, 5.998771, 13.81683, 17.29926, 3.555573...]]

        label_set -- [label_example1, label_example2...]

            e.g.: [[0,1], [1,0], [0,1], [0,1]...]
    """
    feature_set = []
    label_set = []
    size = len(data_set[0]) - 1

    for entry in data_set:
        feature_set.append(entry[:size])
        label_set = label_set + entry[size:]

    return feature_set, label_set


def shuffle_data(feature_set, label_set):
    """Take feature set and shuffle
    the ordering of data examples in the set

    output: shuffled feature_set and equivalently shuffled label_set
    """
    # Merge feature set and labels into one array
    data_set = create_data_set(feature_set, label_set)

    # Shuffle
    random.shuffle(data_set)

    # Separate feature and label sets
    return feature_label_from_dataset(data_set)


if __name__ == "__main__":
    feature_set, label_set = create_feature_label_set(sys.argv[1], sys.argv[2])
    data_input = create_data_set(feature_set, label_set)
