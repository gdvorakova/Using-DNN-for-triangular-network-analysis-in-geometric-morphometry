package cz.cuni.mff.java.projector;

import java.io.PrintWriter;
import java.util.*;
import org.apache.commons.lang3.StringUtils;

/**
 *  The aim of this class is to place
 *  neighbouring vertices as close
 *  to each other on the grid as possible.
 *  This is done by prioritizing (in means of
 *  order of positioning) those vertices,
 *  which have the highest number of already
 *  placed neighbours, to get better accuracy.
 *  Final projection depends on the order
 *  of vertex processing.
 */
public class Grid {

    private Cell[][] vertexGrid;
    public int Rows;
    public int Columns;
    /**
     * List of points that are to be placed on the grid
     */
    private ArrayList<Vertex> vertices;

    public Cell[][] getVertexGrid() {
        return vertexGrid;
    }

    /**
     * Hashmap that contains pairs of vertex
     * and the number of its neighbours that
     * are already placed on the grid
     */
    private Map<Vertex, Integer> occupiedNeighbouringCellsCounter;

    public Grid(int width, int height, ArrayList<Vertex> vertices) {
        Rows = width;
        Columns = height;
        vertexGrid = new Cell[width][height];
        this.vertices = vertices;

        Initialize();
        // GetNeigbouringCells();
        CreatePlacedNeighboursMap();
    }

    /**
     * Creates an empty grid of size given in parameters
     * @param width number of columns of the grid where vertices are going to be placed
     * @param height number of rows of the grid where vertices are going to be placed
     * @param vertices list of 3D point coordinates
     * @return new instance of Grid class
     */
    public static Grid CreateGrid(int width, int height, ArrayList<Vertex> vertices) {
        if (width < 1 || height < 1)
            return null;
        if (vertices == null)
            return null;
        return new Grid(width, height, vertices);
    }


    /**
     * Initializes grid by filling
     * it with empty cells.
     */
    private void Initialize() {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Cell cell = new Cell(i, j);
                vertexGrid[i][j] = cell;
            }
        }
    }

    /**
     *  For each position on a grid (cell)
     *  get the list of neighbouring cells.
     */
    private void GetNeigbouringCells() {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Cell cell = vertexGrid[i][j];
                cell.FreeNeighbouringCells = GetNeighbouringCells(i, j);
            }
        }
    }

    /**
     * Neighbours are defined according
     *  to 8-neighbourhood pixel connectivity.
     *  Wikipedia: 8-connected pixels are neighbors
     *  to every pixel that touches one of their edges or corners.
     *  These pixels are connected horizontally, vertically, and diagonally.
     *  In addition to 4-connected pixels, each pixel with coordinates
     *  (x ± 1, y ± 1) is connected to the pixel at (x, y).
     * @param x x coordinate of a cell on the grid
     * @param y y coordinate of a cell on the grid
     * @return list of pairs x, y (coordinates of neighbouring cells)
     */
    private ArrayList<AbstractMap.SimpleEntry<Integer,Integer>> GetNeighbouringCells(int x, int y) {
        ArrayList<AbstractMap.SimpleEntry<Integer,Integer>> neighbouringCells
                = new ArrayList<AbstractMap.SimpleEntry<Integer, Integer>>();

        // add neighbour from above
        if (x != 0)
        {
            if (y != 0)
            {
                // \
                neighbouringCells.add(new AbstractMap.SimpleEntry<>(x - 1, y - 1));
            }
            // |
            neighbouringCells.add(new AbstractMap.SimpleEntry<>(x - 1, y));


            if (y != Columns - 1)
            {
                // /
                neighbouringCells.add(new AbstractMap.SimpleEntry<>(x - 1, y + 1));
            }
        }
        // add neighbour from the right
        if (y != Columns - 1)
        {
            // -
            neighbouringCells.add(new AbstractMap.SimpleEntry<>(x, y + 1));
            if (x != Rows - 1)
            {
                // \
                neighbouringCells.add(new AbstractMap.SimpleEntry<>(x + 1, y + 1));
            }
        }
        // add neighbour from below
        if (x != Rows - 1)
        {
            // |
            neighbouringCells.add(new AbstractMap.SimpleEntry<>(x + 1, y));

            if (y != 0)
            {
                // /
                neighbouringCells.add(new AbstractMap.SimpleEntry<>(x + 1, y - 1));
            }
        }
        // add neighbour from the left
        if (y != 0)
        {
            // -
            neighbouringCells.add(new AbstractMap.SimpleEntry<>(x, y - 1));
        }

        return neighbouringCells;
    }

    /**
     * Creates a list of pairs vertex, number of neighbouring cells
     * that are occupied - it is initialized with 0.
     */
    private void CreatePlacedNeighboursMap() {
        occupiedNeighbouringCellsCounter = new LinkedHashMap<>();

        for (int i = 0; i < vertices.size(); i++)
        {
            // initialize map with 0 as number of placed points
            occupiedNeighbouringCellsCounter.put(vertices.get(i), 0);
        }
    }

    /**
     * Positions every mesh vertex onto a 2D grid.
     */
    public void Create2DProjection() {
        // find first vertex which will be placed on the grid
        // and first cell where that vertex will be placed
        Vertex initialVertex = SetInitialVertex();
        Cell initialCell = SetInitialCell();

        // place initial vertex on initial cell
        PlaceVertexOnGrid(initialVertex, initialCell);

        // cell is occupied, remove it from free cells
        // and edit the number of occupied neighbouring
        // cells for every neighbour
        // RemoveFromFreeCells(initialCell);
        UpdateOccupiedNeighbours(initialVertex);

        // placed vertex is no longer considered
        // for positioning, therefore it can be removed
        // from the list
        occupiedNeighbouringCellsCounter.remove(initialVertex);

        // place the rest of the vertices in occupiedNeighbouringCellsCounter
        FillGrid();

        RemoveEmptyGridLines();
    }

    private void FillGrid() {
        Cell newCell = null;

        // choose vertex with max count of occupied neighbouring cells
        Vertex nextVertex = occupiedNeighbouringCellsCounter.entrySet()
                .stream().max((entry1, entry2) -> entry1.getValue() > entry2.getValue() ? 1 : -1).get().getKey();

        // get rectangle area that is defined by vertex's
        // already placed neighbours on the grid
        // x, y - upper left coordinates of rectangle, width, height
        Rectangle rectangle = GetMinimumBoundingBox(nextVertex);
        // get unoccupied cells from bounding rectangle
        ArrayList<Cell> freeCells = GetFreeCells(rectangle);

        // if there are no free cells found
        // where nextVertex can be placed
        if (freeCells.size() == 0) {
            // increase the resolution of the bounding box
            // until there is a free cell
            while (freeCells.size() == 0) {
                rectangle.x = rectangle.x > 0 ? rectangle.x-- : 0;
                rectangle.y = rectangle.y > 0 ? rectangle.y-- : 0;
                rectangle.width = rectangle.width + 2 <= Columns ? rectangle.width + 2 : rectangle.width;
                rectangle.height = rectangle.height + 2 <= Rows ? rectangle.height + 2 : rectangle.height;

                freeCells = GetFreeCells(rectangle);
            }
        }

        // find free cell from the rectangle that has lowest distance to all placed points
        newCell = Score.ChooseFreeCell(freeCells, nextVertex).getKey();

        // cell is occupied, remove it from free cells
        // and edit the number of occupied neighbouring
        // cells for every neighbour
        if (newCell != null)
        {
            // RemoveFromFreeCells(newCell);
            PlaceVertexOnGrid(nextVertex, newCell);
            UpdateOccupiedNeighbours(nextVertex);
            occupiedNeighbouringCellsCounter.remove(nextVertex);
        }

        // until occupiedNeighbouringCellsCounter is not empty,
        // keep placing points on the grid
        if (occupiedNeighbouringCellsCounter.size() != 0)
        {
            newCell = null;
            nextVertex = null;
            freeCells = null;
            rectangle = null;

            FillGrid();
        }
    }

    /**
     * Returns a list of cells chosen
     * from the bounding rectangle,
     * which are not yet occupied by any point.
     * @param rectangle rectangular area defined by its left corner, width and height
     * @return list of unoccupied cells in the rectangle
     */
    private ArrayList<Cell> GetFreeCells(Rectangle rectangle) {
        ArrayList<Cell> list = new ArrayList<>();

        for (int i = rectangle.x; i < rectangle.x + rectangle.width; i++)
        {
            for (int j = rectangle.y; j < rectangle.y + rectangle.height; j++)
            {
                if (!vertexGrid[i][j].HasPoint())
                {
                    list.add(vertexGrid[i][j]);
                }
            }
        }

        return list;
    }

    /**
     * Gets next vertex to be placed on the grid,
     * returns the minimum area rectangle
     * for vertex's already placed neighbours.
     * @param nextVertex instance of Vertex for which position is being found
     * @return rectangular area of possible positions
     */
    private Rectangle GetMinimumBoundingBox(Vertex nextVertex) {
        int leftX = Integer.MAX_VALUE;
        int rightX = Integer.MIN_VALUE;
        int upY = Integer.MAX_VALUE;
        int downY = Integer.MIN_VALUE;

        for (Vertex neighbour : nextVertex.getNeighbours())
        {
            if (!neighbour.IsPlaced())
            {
                continue;
            }

            int coordX = neighbour.PlacedCell.getX();
            int coordY = neighbour.PlacedCell.getY();

            if (coordX < leftX)
            {
                leftX = coordX;
            }

            if (coordX > rightX)
            {
                rightX = coordX;
            }

            if (coordY < upY)
            {
                upY = coordY;
            }

            if (coordY > downY)
            {
                downY = coordY;
            }
        }

        int x = leftX - 1;
        int y = upY - 1;

        int width = downY - upY + 3;
        int height = rightX - leftX + 3;

        return new Rectangle(x, y, width, height);
    }

    /**
     * @return first vertex to be placed
     * - that is median of vertex indices.
     */
    private Vertex SetInitialVertex() {
        int index = vertices.size() / 2;
        return vertices.get(index);
    }

    /**
     * Returns cell from the grid which will
     * be occupied by the first vertex.
     * Initial set is chosen from
     * the middle of the grid.
     * @return calculated initial cell
     */
    private Cell SetInitialCell() {
        int x = Rows / 2;
        int y = Columns / 2;

        return vertexGrid[x][y];
    }

    /**
     * Remove cell from list of unoccupied
     * neighbouring cells for each
     * of its neighbouring cell
     * @param cell position in a grid that got recently occupied
     */
    private void RemoveFromFreeCells(Cell cell) {
        ArrayList<AbstractMap.SimpleEntry<Integer,Integer>> allNeighbours
                = GetNeighbouringCells(cell.getX(), cell.getY());
        for (AbstractMap.SimpleEntry<Integer,Integer> neighbour : allNeighbours)
        {
            vertexGrid[neighbour.getKey()][neighbour.getValue()]
                    .FreeNeighbouringCells.remove(new AbstractMap.SimpleEntry<Integer,Integer>(cell.getX(), cell.getY()));
        }
    }

    /**
     * Places vertex on a given cell.
     * @param vertex instance of 3D vertex
     * @param cell position in a grid
     */
    private void PlaceVertexOnGrid(Vertex vertex, Cell cell) {
        cell.PlacedVertex = vertex;
        vertex.PlacedCell = cell;
    }

    /**
     * Increase count of occupied neighbours for each
     * vertex's occupied neighbour
     * @param vertex updated vertex
     */
    private void UpdateOccupiedNeighbours(Vertex vertex) {
        for (Vertex neighbour : vertex.getNeighbours())
        {
            if (occupiedNeighbouringCellsCounter.containsKey(neighbour))
            {
                occupiedNeighbouringCellsCounter.put(neighbour, occupiedNeighbouringCellsCounter.get(neighbour) + 1);
            }
        }
    }

    /**
     * This method removes redundant lines from the grid,
     * which do not contain any placed vertices.
     */
    private void RemoveEmptyGridLines() {
        int minX = -1;
        int maxX = 0;
        int minY = 0;
        int maxY = 0;

        //
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                // minX value is not known
                // grid at current position [i][j] contains a vertex
                // add new boundaries with current cell's coordinates
                if (vertexGrid[i][j].PlacedVertex != null && minX == -1)
                {
                    minX = i;
                    maxX = i;
                    minY = j;
                    maxY = j;
                }
                // if there are new boudnaries found,
                // update them
                else if (vertexGrid[i][j].PlacedVertex != null)
                {
                    if (j > maxY)
                        maxY = j;
                    if (i > maxX)
                        maxX = i;
                    if (j < minY)
                        minY = j;
                }
            }
        }

        // calculate new number of rows and columns
        Rows = maxX - minX + 1;
        Columns = maxY - minY + 1;
        Cell[][] newGrid = new Cell[Rows][Columns];

        // generate new grid without reduntant lines
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                newGrid[i][j] = vertexGrid[i + minX][j + minY];
            }
        }
        this.vertexGrid = newGrid;
    }

    /**
     * Prints formatted result of projecting algorithm
     * to output writer. If output file is not specified,
     * prints to console output.
     * @param writer writer that sends output to output file
     */
    public void PrintGrid(PrintWriter writer) {

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                int identifier = vertexGrid[i][j].PlacedVertex == null ? 0 : vertexGrid[i][j].PlacedVertex.Identifier;
                sb.append(
                        StringUtils.rightPad(
                                StringUtils.defaultString(
                                        Integer.toString(identifier)), 6));
                sb.append(' ');
            }
            writer.print(sb);
            writer.println();
            sb = new StringBuilder();
        }

        writer.close();
    }

    /**
     * Calculates the success of vertex placement.
     * @param rateWriter output for rating
     * @param histogramWriter output for histogram
     */
    public void RateGrid(PrintWriter rateWriter, PrintWriter histogramWriter) {
        int max = 0;

        // get vertex with highest ID number
        for (int i = 0; i < vertices.size(); i++)
        {
            if (vertices.get(i).Identifier > max)
            {
                max = vertices.get(i).Identifier;
            }
        }

        int misplacedCount = 0;

        // create histogram for distances
        // of every point from its every neighbour
        TreeMap<Integer, Integer> distanceHistogram = new TreeMap<>();

        boolean[][] pairs = new boolean[max + 1][max + 1];

        if (rateWriter != null) {
            rateWriter.println("Misplaced points: distance:");
        }

        for (Vertex vertex : vertices)
        {
            // get list of all cells,
            // which are neighbouring to the cells
            // where vertex is placed
            ArrayList<AbstractMap.SimpleEntry<Integer,Integer>> allNeigbouringCells
                    = GetNeighbouringCells(vertex.PlacedCell.getX(), vertex.PlacedCell.getY());

            // for each neighbour of the vertex
            for (Vertex neighbour : vertex.getNeighbours())
            {
                // calculate distance between pair vertex, neighbour
                int distance = Math.max(Math.abs(
                        vertex.PlacedCell.getX() - neighbour.PlacedCell.getX()),
                        Math.abs(vertex.PlacedCell.getY() - neighbour.PlacedCell.getY())) - 1;
                if (distance != 0) {
                    if (rateWriter != null) {
                        rateWriter.println(vertex.Identifier + " and " + neighbour.Identifier + ": " + distance);
                    }
                }

                // neighbour is not 8-connected to vertex,
                // it is misplaced
                if (distance != 0) {
                    misplacedCount++;
                }

                // add distance to histogram
                if (distanceHistogram.containsKey(distance)) {
                    distanceHistogram.put(distance, distanceHistogram.get(distance) + 1);
                }
                else
                    distanceHistogram.put(distance, 1);
                //}
            }
        }

        if (rateWriter != null) {
            rateWriter.close();
        }

        if (histogramWriter != null) {

            histogramWriter.println(misplacedCount + " misplaced pairs");
            histogramWriter.println();

            // print histogram
            for (Integer key : distanceHistogram.keySet())
            {
                if (key > 0) {
                    histogramWriter.println("distance " + key + " - " + distanceHistogram.get(key));
                }
            }

            histogramWriter.close();
        }
    }
}
