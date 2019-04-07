import os
import sys

'''
File name: prepare_grids.py
Author: Gabriela Dvorakova
Date created: December 13, 2018
Python Version: 2.7

Goes through directory given in the first argument
and relativizes each mesh by vertex center index given in the
second argument (int). Each mesh is saved into directory
relativized.

command: python relativize.py /thesis/rawData/noses 2470
'''


def main():
    # path to the mesh directory
    path = sys.argv[1]
    # path to the directory where relativized meshes are supposed to be saved
    rel_path = os.path.join(path, "relativized")
    # create new path
    if not os.path.exists(rel_path):
        os.makedirs(rel_path)
    # default vertex index pointing to the nose tip
    center_index = sys.argv[2]

    # recursively search through files in path
    files = os.listdir(path)
    obj_files = []
    for file in files:
        if (file[-4:] == ".obj"):
            obj_files.append(file[:-4])
    obj_files.sort(key=int)

    for file in obj_files:
        # create new output file where relativized mesh will be stored
        filename = os.path.join(rel_path, file + ".obj")
        output = open(filename, mode='w')

        filename = os.path.join(path, file + ".obj")
        input = open(filename, mode='r')

        index = 0
        x, y, z = 0.0, 0.0, 0.0
        # get values of the center vertex index
        for line in input:
            if line.startswith("v "):
                if index == int(center_index):
                    line = line.split(' ', 1)[1]
                    x, y, z = line.split(' ')
                    x = float(x)
                    y = float(y)
                    z = float(z)
                    break
                index += 1

        input.seek(0)

        # substract x, y, z from each vertex coordinates
        for line in input:
            if line.startswith("v "):
                line = line.split(' ', 1)[1]
                old_x, old_y, old_z = line.split(' ')
                old_x = float(old_x)
                old_y = float(old_y)
                old_z = float(old_z)

                new_x = old_x - x
                new_y = old_y - y
                new_z = old_z - z
                output.write(
                        'v ' + str(new_x) + ' ' + str(
                            new_y) + ' ' + str(new_z))
                output.write("\n")
            else:
                output.write(line)

        output.close()
        input.close()

    print("Relativized dataset was succesfully created and stored in the directory \"relativized\".")


if __name__ == "__main__":
    main()
