'''
File name: prepareGrids.py
Author: Gabriela Dvorakova
Date created: March 7, 2018
Python Version: 2.7

This script reads a file with 2D grid map given in the first argument
and stores its values into matrix. Then it goes through the directory given
in the second argument and for each OBJ file in the directory creates
a new grid, that is equivalent to the 2D projection grid, but it substitutes
vertex indices for its coordinates. All such grid are then
merged and saved in the directory preparedData.

command: python /prepareGrids.py /thesis/preparedData/2dGrid.txt
/thesis/rawData/noses
'''

import os
import os.path
import sys

cols, rows = 87, 75
index_map_array = [[0 for x in range(cols)] for y in range(rows)]


def main():
    # path to the 2D index map
    index_map_path = sys.argv[1]
    # path to the directory with triangle meshes
    mesh_path = sys.argv[2]
    # path to the directory where grids are supposed to be saved
    output_path = os.path.join(os.path.dirname(
        os.path.dirname(mesh_path)), "preparedData")
    if not os.path.exists(output_path):
        os.makedirs(output_path)
    output_file = os.path.join(
        output_path, "projectionsDataset.txt")
    output = open(output_file, mode='w')

    # open file with a grid of indices
    index_map_file = open(index_map_path, mode='r')
    i, j = 0, 0

    # read file line by line
    for line in index_map_file:
        # split line into array of indices
        tokens = line.split("\t")
        # place index into array
        for token in tokens:
            if not token.isspace():
                index_map_array[i][j] = int(token)
                # increase column pointer
                j = j + 1
        # end of line
        # return to the first column
        # inrease row pointer
        j = 0
        i = i + 1

    # search through files in path
    files = os.listdir(mesh_path)
    obj_files = []
    for file in files:
        if (file[-4:] == ".obj"):
            obj_files.append(file[:-4])
    obj_files.sort(key=int)

    for filename in obj_files:
        # list to store coordinates in the order of projected indices
        coord_list = list()

        # open file with triangle mesh
        filename = os.path.join(mesh_path, filename + ".obj")
        input = open(filename, mode='r')

        for line in input:
            # line contains vertex coordinates
            if line.startswith("v "):
                # convert string to list of coordinates
                line = line.split(' ', 1)[1]
                line = line.replace("\r", '').replace("\n", '')
                coord_list.append(line)

        # iterate over index_map_array and print result into output
        for i in range(rows):
            for j in range(cols):
                index = index_map_array[i][j] - 1
                # no vertex is stored on position [i,j]
                if index == -1:
                    coord = "0 0 0"
                else:
                    coord = coord_list[index]
                    coord.strip()
                # write coordinates to output file
                output.write(coord)
                if not (j == (cols - 1)):
                    output.write(',')
            output.write("\n")

        input.close()
    output.close()
    print("Dataset projectionsDataset.txt was succesfully created.")


if __name__ == "__main__":
    main()
