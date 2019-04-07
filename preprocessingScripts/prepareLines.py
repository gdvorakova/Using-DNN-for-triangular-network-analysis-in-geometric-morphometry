'''
File name: prepareLines.py
Author: Gabriela Dvorakova
Date created: March 8, 2018
Python Version: 2.7

The first argument that this script requires is a path where line files
are stored. The second argument is a path to triangle meshes. The output
is a dataset stored in preparedData directory with a name linesDataset.txt,
which contains sequences of vertex indices substituted for vertex coordinates,
on a new line for each file.

command: python prepareLines.py /thesis/data/preparedData/lines
/theses/rawData/faces
'''
import os
import sys


def main():
    # path to directory with extracted lines
    line_path = sys.argv[1]
    # path to the mesh directory
    mesh_path = sys.argv[2]
    # number of zeros between two lines
    zero_filling = 43
    # number of zeros between two lines
    n_lines = 9

    # path to the directory where grids are supposed to be saved
    output_path = os.path.join(os.path.dirname(
        os.path.dirname(mesh_path)), "preparedData")
    if not os.path.exists(output_path):
        os.makedirs(output_path)
    output_file = os.path.join(
        output_path, "linesDataset" + str(zero_filling) + ".txt")
    output = open(output_file, mode='w')

    # list of indices found in line files
    index_list = list()

    count = 0
    for i in range(1, n_lines + 1):
        temp = "log" + str(i)
        input = open(os.path.join(line_path, temp + ".txt"), mode='r')
        for index in input:
            count += 1
            index_list.append(int(index))
        # add zeros to separate lines
        if not i == n_lines:
            for j in range(0, zero_filling):
                index_list.append(0)
        input.close()

    # search through files in path
    files = os.listdir(mesh_path)
    obj_files = []
    for file in files:
        if (file[-4:] == ".obj"):
            obj_files.append(file[:-4])
    obj_files.sort(key=int)

    # for each file in directory map its vertices to indices in lines
    for file in obj_files:
        coord_list = list()

        # open file with triangle mesh
        filename = os.path.join(mesh_path, file + ".obj")
        input = open(filename, mode='r')

        for line in input:
            # line contains vertex coordinates
            if line.startswith("v "):
                # convert string to list of coordinates
                line = line.split(' ', 1)[1]
                line = line.replace("\r", '').replace("\n", '')
                coord_list.append(line)

        # iterate over index_list and print result into output file
        for j in range(len(index_list)):
            index = index_list[j] - 1
            if index == -1:
                coord = "0 0 0"
            else:
                coord = coord_list[index]
            output.write(coord)
            if not (j == (len(index_list) - 1)):
                output.write(',')

        input.close()
        output.write("\n")

    output.close()
    print("Dataset linesDataset" + str(zero_filling) + ".txt was succesfully created.")


if __name__ == "__main__":
    main()
