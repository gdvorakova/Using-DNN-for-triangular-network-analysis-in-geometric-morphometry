# Using DNN for triangular network analysis in geometric morphometry

This repository contains all the source codes associated with my Bachelor Thesis. Experiments were programmed in Python using Tenorflow library, upporting programs were written in C# and Java.

## Description
The aim of this thesis is to use deep learning for the task of 3D object recognition. The main goal is to propose an alternative mapping of 3D data to the neural network input. 

### Data representations
Three data representations are introduced:
* Treating vertex coordinates as a 1D array
* Projection of triangle mesh to a 2D grid
* Set of surface oblique lines crossing the significant parts of an object

### Experiments
All of the proposed data representations are tested for the gender classification task using NN and CNN on 3D facial models. We analyzed the impact of coordinate relativization and a new modified dataset created by extracting a nose area from original triangle meshes.

<p align="center">
<img src="https://github.com/gdvorakova/Using-DNN-for-triangular-network-analysis-in-geometric-morphometry/blob/master/lines.png?raw=true" width="250">
</p>

### Results 
Experimental results confirmed the quality of the oblique lines approach with achieved classification accuracies of 84,2% using CNN.
