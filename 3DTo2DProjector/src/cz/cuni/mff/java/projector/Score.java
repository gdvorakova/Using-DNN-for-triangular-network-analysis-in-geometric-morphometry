package cz.cuni.mff.java.projector;

import java.util.AbstractMap;
import java.util.ArrayList;

/**
 * This class helps to choose the best position
 * for new vertex.
 */
public class Score {
    public static AbstractMap.SimpleEntry<Cell, Integer> ChooseFreeCell(ArrayList<Cell> freeCells, Vertex nextVertex) {
        int bestScore = Integer.MAX_VALUE;
        AbstractMap.SimpleEntry<Cell, Integer> finalCell = null;

        // calculate score for each free cell
        // and find the one with the smallest score (= smallest distance)
        for (Cell c : freeCells)
        {
            AbstractMap.SimpleEntry<Cell, Integer> temp = Score.GetScore(c, nextVertex);
            if (temp.getValue() < bestScore)
            {
                bestScore = temp.getValue();
                finalCell = temp;
            }
        }

        return finalCell;
    }

    /**
     * Calculates unique score for a cell
     * where the current vertex could be placed.
     * Score depends on the distance from all
     * neighbouring vertices that are already placed on the grid.
     * @param cell position on a grid
     * @param vertex instance of 3D vertex
     * @return a list that maps cell with its score
     */
    private static AbstractMap.SimpleEntry<Cell, Integer> GetScore(Cell cell, Vertex vertex)
    {
        int score = 0;

        // calculate distance between every placed neighbour
        for (Vertex neighbour : vertex.getNeighbours())
        {
            // if the vertex is not yet placed,
            // do not change the score
            if (!neighbour.IsPlaced())
            {
                continue;
            }

            // increase score by the distance from neighbour
            score += CalculateDistance(cell, neighbour.PlacedCell);
        }

        if (score == 0)
        {
            score = Integer.MAX_VALUE;
        }

        return new AbstractMap.SimpleEntry<Cell, Integer>(cell, score);
    }

    /**
     * Calculates distance between two cells.
     * @param a position on a grid
     * @param b position on a grid
     * @return integer distance between two cells
     */
    public static int CalculateDistance(Cell a, Cell b)
    {
        return Integer.max(Math.abs(a.getX() - b.getX()), Math.abs(a.getY() - b.getY()));
    }
}
