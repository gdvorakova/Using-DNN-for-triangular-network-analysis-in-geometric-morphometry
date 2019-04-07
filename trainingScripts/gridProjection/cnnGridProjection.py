'''
File name: cnnGridProjection.py
Author: Gabriela Dvorakova
Date created: February 24, 2018
Python Version: 3.6.3

This is the main script that controls calls of all the other scripts
used for loading input data, running the training of convolutional
neural network and then computing the accuracy using K-fold cross validation.

command: python cnnGridProjection.py
'''

import parse
import trainModelNet1 as train
import config
import kFold
import csv


def run():
    id = 1

    # Prepare data sets
    feature_set, label_set, data_size = parse.create_feature_label_set(
        features_file=config.FEATURES_FILE, labels_file=config.LABELS_FILE)

    # Prepare output file
    csv_file = open(config.OUTPUT_FILE, mode="w")
    result_writer = csv.writer(csv_file)
    result_writer.writerow((
        'Id', 'Epochs', 'Learning rate', 'Keep rate', 'Accuracy'))

    # create new configuration class instance
    configuration = config.Configuration(id=id)
    id = id + 1

    # Apply k-fold cross validation on data and repeat k times
    cross_validation(
        feature_set, label_set, configuration, result_writer, data_size)

    csv_file.close()


def cross_validation(
        feature_set, label_set, configuration, result_writer, data_size):
    errors = list()
    epoch_count = config.N_EPOCHS

    # Apply k-fold cross validation on prepared data and repeat k times
    for l in range(config.K):
        # Get k subsets of data set
        k_train_test_folds, k_train_label_folds = kFold.create_k_folds(
            feature_set, label_set, config.K, data_size)

        it_errors = list()
        # Train neural network
        for i in range(config.K):
            test_data = k_train_test_folds[i]
            test_labels = k_train_label_folds[i]
            train_data = []
            train_labels = []

            for j in range(config.K):
                if j != i:
                    train_data = train_data + k_train_test_folds[j]
                    train_labels = train_labels + k_train_label_folds[j]

            # print("Round " + str(i) + ": ")
            accuracy = train.train_convolutional_neural_network(
                train_data, train_labels, test_data, test_labels,
                configuration, shuffle=True)
            it_errors.append(accuracy)
            s = 'Repetition: ' + str(l) + ', round: ' + str(i) + ', accuracy: ' + str(accuracy)
            print(s)
        errors.append(sum(it_errors)/config.K)

    # Calculate statistics
    mean = sum(errors)/(config.K)

    # Write result into .csv file
    #        'Epochs', 'Learning rate', 'Keep rate, ''Accuracy'
    acc_rounded = format(mean * 100, '.2f')
    result_writer.writerow((
        configuration.id, epoch_count,
        config.LEARNING_RATE, config.KEEP_RATE, acc_rounded))

    print('Number of epochs: ', config.N_EPOCHS)
    print('Accuracy: ', mean)


if __name__ == "__main__":
    run()
