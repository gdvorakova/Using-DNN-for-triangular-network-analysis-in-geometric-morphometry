'''
File name: nn1dArray.py
Author: Gabriela Dvorakova
Date created: February 24, 2018
Python Version: 3.6.3

This is the main script that controls calls of all the other scripts
used for loading input data, running the training of neural network
then computing the accuracy using K-fold cross validation.

command: python nn1dArray.py
'''

import parse
import train
import config
import kFold
import csv

# Sequence of number of nodes that will be used to find configuration
# that gives the best accuracy
WIDTH_OF_HIDDEN_NODES = [2, 16, 64, 128, 256, 512, 640, 768, 896]

feature_set = []
label_set = []

# Number of neuron in input layer
n_input = 0

# max number of tested layers
MAX_N_LAYERS = 2


def run():
    # Prepare data sets in the correct form (training data and labels)
    feature_set, label_set = parse.create_feature_label_set(
        features_file=config.FEATURES_FILE, labels_file=config.LABELS_FILE)

    # Prepare output file and its header
    csv_file = open(config.OUTPUT_FILE, mode="w")
    result_writer = csv.writer(csv_file)
    result_writer.writerow((
        'Id', 'Input data size', 'Number of hidden layers',
        'Hidden layers'' size', 'Epochs', 'Learning rate', 'Accuracy'))

    # id variable is a simple counter for the number of tested architectures
    id = 1

    # Repeat for 1,....,max_n_layers hidden layers
    # Test all combinations of nodes in hidden layers
    # Number of neurons in consecutive layers is always non-growing
    for n_layers in range(1, MAX_N_LAYERS + 1):
        # Append labels with correct data and merge them into data_set
        data_set = parse.create_data_set(feature_set, label_set)
        # Get the size of input neurons
        n_input = len(data_set[0]) - 1

        # Numeric vector with number of neurons for each hidden layer
        # For n_layers = 2 we get: hidden_layer_nodes_list = [2, 2]
        hidden_layer_nodes_list = [WIDTH_OF_HIDDEN_NODES[0]] * n_layers

        # Create new configuration class instance with current settings
        configuration = config.Configuration(
            n_input=n_input, n_hidden_layers=n_layers,
            hidden_layer_nodes_list=hidden_layer_nodes_list, id=id)
        id = id + 1
        # Apply k-fold cross validation on data and repeat k times
        cross_validation(data_set, configuration, result_writer)

        # For 1 hidden layer just run one loop
        if (n_layers == 1):
            # Append labels with correct data and merge them into data_set
            data_set = parse.create_data_set(feature_set, label_set)

            run_in_loop(hidden_layer_nodes_list, 1, id, result_writer)
            id = id + 1
            continue

        hidden_layer_nodes_list = [WIDTH_OF_HIDDEN_NODES[0]] * n_layers
        for curr_val in range(1, len(WIDTH_OF_HIDDEN_NODES)):
            for layer in range(1, n_layers):
                hidden_layer_nodes_list[layer] = WIDTH_OF_HIDDEN_NODES[curr_val]
                run_in_loop(hidden_layer_nodes_list, curr_val, id, result_writer)
                id += 1

    csv_file.close()


def run_in_loop(hidden_layer_nodes_list, start_val, id, result_writer):
        for i in range(start_val, len(WIDTH_OF_HIDDEN_NODES)):
            hidden_layer_nodes_list[
                0] = WIDTH_OF_HIDDEN_NODES[i]
            # Create new configuration class instance with current settings
            configuration = config.Configuration(
                n_input=n_input, n_hidden_layers=len(hidden_layer_nodes_list),
                hidden_layer_nodes_list=hidden_layer_nodes_list, id=id)
            # Append labels with correct data and merge them into data_set
            print(len(feature_set))
            print(len(label_set))
            data_set = parse.create_data_set(feature_set, label_set)
            # Apply k-fold cross validation on data and repeat k times
            cross_validation(data_set, configuration, result_writer)
            print(hidden_layer_nodes_list)


def cross_validation(data_set, configuration, result_writer):
    errors = list()

    # Apply k-fold cross validation on prepared data and repeat k times
    for l in range(config.K):
        # Get k subsets of data set
        k_train_test_folds = kFold.create_k_folds(data_set, config.K)
        it_errors = list()

        # Train neural network
        for i in range(config.K):
            test_set = k_train_test_folds[i]
            train_set = []

            for j in range(config.K):
                if j != i:
                    train_set = train_set + k_train_test_folds[j]

            train_data, train_labels = parse.feature_label_from_dataset(
                train_set)
            test_data, test_labels = parse.feature_label_from_dataset(test_set)

            accuracy = train.train_neural_network(
                train_data, train_labels, test_data, test_labels,
                configuration, shuffle=True)
            it_errors.append(accuracy)
            s = 'Repetition: ' + str(l) + ', round: ' + str(i) + ', accuracy: ' + str(accuracy)
            print(s)
        errors.append(sum(it_errors)/config.K)

    # Calculate statistics
    mean = sum(errors)/(config.K)

    # Write result into .csv file
    #       'Input data size', 'Number of hidden layers',
    #       'Hidden layers'' size', 'Epochs', 'Learning rate', 'Accuracy'
    hidden_layers_size = ','.join(
        map(str, configuration.hidden_layer_n_nodes_list))
    acc_rounded = format(mean * 100, '.2f')
    result_writer.writerow((
        configuration.id, configuration.n_nodes_input,
        configuration.n_hidden_layers, hidden_layers_size, config.N_EPOCHS,
        config.LEARNING_RATE, acc_rounded))

    print('Number of hidden layers: ', configuration.n_hidden_layers)
    print('Number of neurons in hidden layers: ', hidden_layers_size)
    print('Number of epochs: ', config.N_EPOCHS)
    print('Accuracy: ', mean)


if __name__ == "__main__":
    run()
