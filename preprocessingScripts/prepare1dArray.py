import os
import sys
'''
File name: prepare1dArray.py
Author: Gabriela Dvorakova
Date created: November 25, 2017
Python Version: 2.7

This script goes through every file in the directory given
in the first argument. For every file vertex coordinates
are extracted to a new line in a new output file.
This file is stored in the directory preparedData

command: python extractCoordinates.py thesis/rawData/noses
'''


def removeRedundant(input_File, output):
    input = open(input_File, mode='r')
    vertices = 0
    # first determine the number of vertices in a file
    for line in input:
        if line.startswith("v "):
            vertices += 1

    input.close()
    input = open(input_File, mode='r')

    count = 0
    for line in input:
        if line.startswith("v "):
            count += 1
            line = line.replace("\r", '').replace("\n", '')
            output.write(line.split(' ', 1)[1])
            # separate vertices with ','
            if (count != vertices):
                output.write(',')
            else:
                output.write("\n")

    input.close()


def findObjFiles():
    # set path to the directory, which will be searched for OBJ files
    path = sys.argv[1]
    # create directory where new files will be stored
    output_dir = os.path.join(os.path.dirname(
        os.path.dirname(path)), "preparedData")
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)
    output_file = os.path.join(
        output_dir, "noseCoordinatesDataset.txt")
    output = open(output_file, mode='w')

    # recursively search through files in path
    files = os.listdir(path)
    obj_files = []
    for file in files:
        if (file[-4:] == ".obj"):
            obj_files.append(file[:-4])
    obj_files.sort(key=int)

    for file in obj_files:
        # extract only necessary information from OBJ file
        removeRedundant(os.path.join(path, file + ".obj"), output)

    output.close()
    print("Dataset noseCoordinatesDataset.txt was succesfully created.")


def main():
    findObjFiles()


if __name__ == "__main__":
    main()
