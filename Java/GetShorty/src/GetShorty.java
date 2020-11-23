import java.io.*;
import java.util.*;

public class GetShorty     {
    static class Node implements Comparable<Node>
    {
        public List<Edge> corridors;
        public int index;
        public double bestWeight;
        public Node(int index)
        {
            this.bestWeight = 0;
            this.index = index;
            corridors = new ArrayList<>();
        }
        @Override
        public int compareTo(Node other)
        {
            if (this.bestWeight < other.bestWeight)
                return 1;
            if (this.bestWeight > other.bestWeight)
                return -1;
            return 0;
        }
    }
    public static class Edge
    {
        public double weight;
        public int child;
        public Edge(double weight, int dest)
        {
            this.weight = weight;
            this.child = dest;
        }
    }

    public static void main(String[] args) throws IOException {
        Reader reader = new InputStreamReader(System.in);
        BufferedReader myObj = new BufferedReader(reader);
        BufferedOutputStream output = new BufferedOutputStream(System.out);
        String userInput = myObj.readLine();
        String[] data = userInput.split( " " );
        int n = Integer.parseInt(data[0]);
        int m = Integer.parseInt(data[1]);
        int x;
        int y;
        double w;

        while (m > 0 ||  n > 0)
        {
            Node[] rooms = new Node[n];

            //builds labyrinth in m lines
            for (int i = 0; i < m; i++)
            {
                userInput = myObj.readLine();
                data = userInput.split( " " );
                x = Integer.parseInt(data[0]);
                y = Integer.parseInt(data[1]);
                w = Double.parseDouble(data[2]);
                addEdge(x,y,w,rooms);
                //addEdge(x,y,w,rooms,n);
            }
            rooms[0].bestWeight = 1;
            //traverse labyrinth
            double result = SSSP(rooms, n);
            //round and format output
            //System.out.print(String.format("%.4g%n", result));
            output.write(String.format("%.4f%n", result).getBytes());
            output.flush();
            //read next line of input
            userInput = myObj.readLine();
            data = userInput.split( " " );
            n = Integer.parseInt(data[0]);
            m = Integer.parseInt(data[1]);
        }
    }

    public static void addEdge(int x, int y, double w,  Node[] rooms)
    {
        //adds room if it doesn't exist
        if (rooms[x] == null)
        {
            rooms[x] = new Node(x);
        }
        //adds edge to y with weight given
        rooms[x].corridors.add(new Edge(w, y));
        //repeats above but for y
        if (rooms[y] == null)
        {
            rooms[y] = new Node(y);
        }
        rooms[y].corridors.add(new Edge(w, x));
    }

    public static double SSSP(Node[] rooms, Integer n)
    {
        List<Edge> childrenToVisit;
        PriorityQueue<Node> roomsToVisit = new PriorityQueue<>();
        roomsToVisit.add(rooms[0]);
        while (true)
        {
            int visiting = roomsToVisit.remove().index;
            if(visiting == n-1)
                break;
            childrenToVisit = rooms[visiting].corridors;
            for (Edge e : childrenToVisit)
            {
                if(e == null)
                    break;
                double corridorResult = e.weight * rooms[visiting].bestWeight;
                if (corridorResult > rooms[e.child].bestWeight)
                {
                    roomsToVisit.remove(rooms[e.child]);
                    rooms[e.child].bestWeight = corridorResult;
                    roomsToVisit.add(rooms[e.child]);
                }
            }
        }
        return rooms[n-1].bestWeight;
    }
}


