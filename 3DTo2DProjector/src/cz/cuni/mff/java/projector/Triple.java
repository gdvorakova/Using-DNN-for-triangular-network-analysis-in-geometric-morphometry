package cz.cuni.mff.java.projector;

/**
 * A tuple of 3D objects.
 * @param <F> first object
 * @param <S> second object
 * @param <T> third object
 */
public class Triple<F, S, T> {

    public final F first;
    public final S second;
    public final T third;

    public Triple(F first, S second, T third) {
        this.first = first;
        this.second = second;
        this.third = third;
    }
}
