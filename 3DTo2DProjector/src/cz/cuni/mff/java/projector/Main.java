package cz.cuni.mff.java.projector;

import java.io.*;
import org.apache.commons.cli.*;

/**
 * 3D to 2D projector.
 * @author Gabriela Dvorakova
 */
public class Main {

    public static CommandLine ParseCommandLineArguments(String[] args) {
        Options options = new Options();

        // must be given, otherwise program won't run
        Option input = new Option("i", "input", true, "input file path");
        input.setRequired(true);
        options.addOption(input);

        // of not given, use console
        Option output = new Option("o", "output", true, "output file");
        output.setRequired(false);
        options.addOption(output);

        // if not given, do nothing
        Option neighbours = new Option("n", "neighbours", true, "neighbours file");
        output.setRequired(false);
        options.addOption(neighbours);

        // if not given, do nothing
        Option rate = new Option("d", "distance", true, "distance file");
        output.setRequired(false);
        options.addOption(rate);

        // if not given, do nothing
        Option histogram = new Option("h", "histogram", true, "histogram file");
        output.setRequired(false);
        options.addOption(histogram);

        CommandLineParser parser = new DefaultParser();
        HelpFormatter formatter = new HelpFormatter();
        CommandLine commandLine;

        try {
            commandLine = parser.parse(options, args);
            return commandLine;

        } catch (ParseException e) {
            System.out.println(e.getMessage());
            formatter.printHelp("utility-name", options);

            return null;
        }
    }

    public static void main(String[] args) {

        CommandLine arguments = ParseCommandLineArguments(args);
        if (arguments == null) {
            System.exit(1);
            return;
        }

        String inputFilePath = arguments.getOptionValue("input");
        if (inputFilePath == null) {
            System.out.println("No input file specified.");
            System.out.println("Use command line arguments:  -i,--input <arg>    input file path");

            System.exit(1);
            return;
        }

        cz.cuni.mff.java.projector.Parser parser = cz.cuni.mff.java.projector.Parser.TryOpenFile(inputFilePath);
        if (parser == null) {
            System.exit(1);
            return;
        }

        parser.CreatePointsFromTriangleMesh();

        try {
            String outputFilePath = arguments.getOptionValue("output");
            PrintWriter outputWriter;
            if (outputFilePath == null) {
                outputWriter = new PrintWriter(System.out);
            }
            else {
                outputWriter = new PrintWriter(outputFilePath, "utf-8");
            }

            Grid grid = Grid.CreateGrid((int)(2 * parser.getVertexCount()), (int)(2 * parser.getVertexCount()),
                    parser.getVertices());
            if (grid == null) {
                System.out.println("Grid could not be created. Width or height is too small.");
                System.exit(1);
                return;
            }

            grid.Create2DProjection();
            grid.PrintGrid(outputWriter);

            String neighboursFilePath = arguments.getOptionValue("neighbours");
            if (neighboursFilePath != null) {
                PrintWriter neighbourWriter = new PrintWriter(neighboursFilePath, "utf-8");
                parser.PrintNeighbours(neighbourWriter);
                neighbourWriter.close();
            }

            String distanceFilePath = arguments.getOptionValue("distance");
            String histogramFilePath = arguments.getOptionValue("histogram");
            PrintWriter rateWriter = null;
            PrintWriter histogramWriter = null;
            if (distanceFilePath != null) {
                rateWriter = new PrintWriter(distanceFilePath, "utf-8");
            }
            if (histogramFilePath != null) {
                histogramWriter = new PrintWriter(histogramFilePath, "utf-8");
            }
            if (rateWriter != null || histogramWriter != null) {
                grid.RateGrid(rateWriter, histogramWriter);
            }
        }
        catch (FileNotFoundException ex) {
            System.out.println(ex.getMessage());
            System.exit(1);
            return;
        }
        catch (UnsupportedEncodingException ex) {
            System.out.println(ex.getMessage());
            System.exit(1);
            return;
        }
    }
}
