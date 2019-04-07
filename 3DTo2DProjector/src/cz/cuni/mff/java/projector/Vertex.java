package cz.cuni.mff.java.projector;

import java.util.ArrayList;

/**
 * A 3D point, that contains information
 * about its neighbours, and a cell at which
 * it lies, if exists.
 */
public class Vertex {
    /**
     * Index in array of vertices
     */
    public int Index;

    /**
     * Unique name of the vertex
     */
    public int Identifier;

    /**
     * Set of all vertices connected by an edge
     *  to the node specified by identifier
     */
    private ArrayList<Vertex> Neighbours;

    /**
     * Cell on which this instance
     *  of a vertex is placed.
     */
    public Cell PlacedCell;

    /**
     * Vertices are indexed from 0,
     * identifiers start from 1.
     * 0 identifies no vertex.
     * @param index index of vertex given by the ordering of vertex coordinates in OBJ file
     * @param id index identifier
     */
    public Vertex(int index, int id) {
        this.Index = index;
        this.Identifier = id;

        Neighbours = new ArrayList<>();
    }

    public boolean IsPlaced()
    {
        return PlacedCell == null ? false : true;
    }

    public ArrayList<Vertex> getNeighbours() {
        return this.Neighbours;
    }
}
