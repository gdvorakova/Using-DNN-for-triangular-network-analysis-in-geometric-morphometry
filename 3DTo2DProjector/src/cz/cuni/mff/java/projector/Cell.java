package cz.cuni.mff.java.projector;

import java.util.AbstractMap;
import java.util.ArrayList;

/**
 * A position in a grid.
 * Cell can be occupied by a Vertex instance.
 */
public class Cell {
    /**
     * x coordinate
     */
    private int x;
    /**
     * y coordinate
     */
    private int y;

    public int getX() {
        return x;
    }

    public int getY() {
        return y;
    }

    /**
     * Instance of vertex that lies in the cell
     */
    public Vertex PlacedVertex;

    /**
     * List of neighbouring cells which are occupied
     * by any vertex.
     * Neighbouring cells are defined as a pair of x, y
     * coordinates.
     */
    public ArrayList<AbstractMap.SimpleEntry<Integer, Integer>> FreeNeighbouringCells;

    public Cell(int x, int y) {
        this.x = x;
        this.y = y;
    }

    /**
     * @return true if cell is occupied, otherwise false
     */
    public boolean HasPoint()
    {
        return PlacedVertex == null ? false : true;
    }
}
