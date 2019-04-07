'''
File name: kFold.py
Author: Gabriela Dvorakova
Date created: March 11, 2018
Python Version: 3.6.3

Support for K-fold cross validation for CNN with projection grids.
'''

import random


def create_k_folds(feature_set, label_set, k, data_size):
    k_train_test_folds = []
    k_train_label_folds = []
    
    # Calculate number of examples in a subset for k subsets
    examples_count = int(data_size / k)

    range_list = list(range(0, data_size))

    # Create k random subsets of data_set
    for i in range(k):
        # Pick k random elements that will be used as a training set
        subset = random.sample(range_list, examples_count)
        subset_data = []
        subset_label_data = []

        for e in subset:
            subset_data.append(feature_set[e])
            subset_label_data.append(label_set[e])

        k_train_test_folds.append(subset_data)
        k_train_label_folds.append(subset_label_data)

        for e in subset:
            range_list.remove(e)

    return k_train_test_folds, k_train_label_folds
