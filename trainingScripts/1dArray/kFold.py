'''
File name: kFold.py
Author: Gabriela Dvorakova
Date created: December 31, 2017
Python Version: 3.6.3

Support for K-fold cross validation.
'''

import random


def create_k_folds(data_set, k):
    """Returns data_set, that is split into k random disjoint sets.

    parameters:
        data_set -- list of features together with their label

            e.g.: [[4.027986, -24.92605, 19.12924, -2.53098, 30.35624..., [0,1]],
                   [12.61018, 5.998771, 13.81683, 17.29926, 3.555573..., [1,0]]]

        k -- number of parts the dataset is supposed to be split into
    """
    data = list(data_set)
    k_train_test_folds = []
    data_count = len(data)

    # Calculate number of examples in a subset for k subsets
    examples_count = int(data_count / k)

    # Create k random subsets of data_set
    for i in range(k):
        # Pick k random elements that will be used as a training set
        subset = random.sample(data, examples_count)
        k_train_test_folds.append(subset)

        for e in subset:
            data.remove(e)

    # If number of examples in data set is not divisible by k,
    # then there are some examples left in data_set
    if len(data) != 0:
        for e in range(len(data)):
            k_train_test_folds[e].append(data[e])

    return k_train_test_folds
