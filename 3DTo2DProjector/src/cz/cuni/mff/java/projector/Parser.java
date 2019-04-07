package cz.cuni.mff.java.projector;

import java.io.*;
import java.util.ArrayList;

/**
 * The primary purpoe of this class is to
 * process input file
 * (triangle mesh) and extract the information
 * about its vertices and faces
 */
public class Parser {

    private BufferedReader reader;
    private FileInputStream fIn;
    private ArrayList<Vertex> vertices;

    public ArrayList<Vertex> getVertices() {
        return vertices;
    }

    public Parser(BufferedReader reader, FileInputStream fIn){
        this.reader = reader;
        this.fIn = fIn;
    }

    public int getVertexCount() {
        return vertices == null ? 0 : vertices.size();
    }

    /**
     * If file exists, creates a new instance
     * of parser class to read file.
     * @param inputFilePath path where 3D mesh in OBJ format is stored
     * @return new instance of Parser class, if file exists
     */
    public static Parser TryOpenFile(String inputFilePath) {

        Parser parser = null;
        try {
            FileInputStream fIn = new FileInputStream(inputFilePath);
            BufferedReader br = new BufferedReader(new InputStreamReader(fIn));
            parser = new Parser(br, fIn);

        }
        catch (FileNotFoundException ex) {
            ex.printStackTrace();
        }

        return parser;
    }

    /**
     * The main method that is called from outside the class
     * to run file processing.
     */
    public void CreatePointsFromTriangleMesh() {

        int count = GetVertexCount();
        vertices = new ArrayList<>();

        for (int i = 1; i <= count; i++) {
            // create new vertex of index i - 1, identifier i
            vertices.add(new Vertex(i - 1, i));
        }

        FindNeighbours();
        RemoveVerticesWithNoNeighbours();
    }

    /**
     * Removes every vertex from vertices
     * which has no neighbours.
     */
    private void RemoveVerticesWithNoNeighbours() {
        ArrayList<Vertex> toRemove = new ArrayList<>();
        for (Vertex vertex : vertices) {
            if (vertex.getNeighbours().size() == 0) {
                toRemove.add(vertex);
            }
        }
        vertices.removeAll(toRemove);
    }

    private void FindNeighbours() {

        String line;

        try {
            line = reader.readLine();

            // continue to read until you reach end of file
            while (line != null) {
                // line contains face vertex indices
                if (line.startsWith("f ")) {
                    int vertexA, vertexB, vertexC;
                    Triple<Integer, Integer, Integer> vertexCoordinates = ParseTriangle(line);

                    if (vertexCoordinates == null) {
                        System.out.println("Unable to parse file.");
                        return;
                    }

                    // add all vertices from triangle face to each other's neighbour lists
                    AddNeighbours(vertexCoordinates.first, vertexCoordinates.second, vertexCoordinates.third);
                    AddNeighbours(vertexCoordinates.second, vertexCoordinates.first, vertexCoordinates.third);
                    AddNeighbours(vertexCoordinates.third, vertexCoordinates.first, vertexCoordinates.second);

                }

                line = reader.readLine();
            }

            reader.close();
        }
        catch (IOException ex) {
            System.out.println(ex.getMessage());
            System.exit(1);
        }
    }

    /**
     * Add neighbour1 and neighbour2 vertices to vertex neighbour lists
     * @param vertex instance of vertex
     * @param neighbour1 index of vertex
     * @param neighbour2 index of vertex
     */
    private void AddNeighbours(Integer vertex, Integer neighbour1, Integer neighbour2) {
        if (!vertices.get(vertex - 1).getNeighbours().contains(vertices.get(neighbour1 - 1))) {
            vertices.get(vertex - 1).getNeighbours().add(vertices.get(neighbour1 - 1));
        }
        if (!vertices.get(vertex - 1).getNeighbours().contains(vertices.get(neighbour2 - 1))) {
            vertices.get(vertex - 1).getNeighbours().add(vertices.get(neighbour2 - 1));
        }
    }

    /**
     * Gets a String in format "f 1 2 3",
     * and returns a triple of integer 1, 2, 3,
     * where 1, 2, 3 are vertex indices
     * @param line a line from OBJ file that contains face information.
     * @return Triple of vertex indices
     */
    private Triple<Integer,Integer,Integer> ParseTriangle(String line) {
        String[] tokens = line.split(" ");

        if (tokens.length != 4) {
            System.out.println("Invalid data format");
            return null;
        }

        try {
            int vertexA = Integer.parseInt(tokens[1]);
            int vertexB = Integer.parseInt(tokens[2]);
            int vertexC = Integer.parseInt(tokens[3]);

            Triple<Integer, Integer, Integer> triple = new Triple<>(vertexA, vertexB, vertexC);

            return triple;
        }
        catch (NumberFormatException ex) {
            System.out.println(ex.getMessage());
            System.exit(1);
            return null;
        }

    }

    /**
     * Reads the input file and counts the number
     * of lines that start with "v" = number of vertices
     * @return number of vertices in the input triangle mesh
     */
    private int GetVertexCount() {
        String line;
        int count = 0;

        try {
            line = reader.readLine();

            // continue to read until you reach end of file
            while (line != null) {
                if (line.startsWith("f")) {
                    // return to beginning of file
                    fIn.getChannel().position(0);
                    reader = new BufferedReader(new InputStreamReader(fIn));
                    break;
                }

                // line contains vertex coordinates
                // increase counter
                if (line.startsWith("v ")) {
                    count++;
                }

                line = reader.readLine();
            }

        }
        catch (IOException ex) {
            ex.printStackTrace();
        }

        return count;
    }

    /**
     * Prints list of neighbours
     * for each vertex into a writer
     * @param writer neighbour file writer
     */
    public void PrintNeighbours(PrintWriter writer) {
        for (Vertex vertex : vertices)
        {
            writer.print(vertex.Index + " - ");
            for (Vertex neighbour : vertex.getNeighbours())
            {
                writer.print(neighbour.Identifier + " ");
            }
            writer.println();
        }

    }

}
