package cz.cuni.mff.java.projector;

/**
 * A 2D rectangle with integer coordinates.
 */
public class Rectangle {

    public int x;
    public int y;
    public int width;
    public int height;

    /**
     * Rectangle is defined by the upper left coordinate,
     * its width and height.
     * @param x x integer coordinate.
     * @param y y integer coordinate
     * @param width width of rectangle
     * @param height height of rectangle
     */
    public Rectangle(int x, int y, int width, int height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}
