# Maximum flow
# Interpret a directed graph as a "flow network" and use it to answer questions about material flows.
# From source to sink (vector-calculus: the extent to which the vector field flux behaves like a source)

# 2 methods for solving maximum-flow: 
#   1. Ford and Fulkerson - application findintg maximum matching in an undirected bipartite graph
#   2. Push label method
#   2.1 Relabel-to-front imlementation of push label method O(V^3)
# Fromalize flow network, and flows

# Flow-networks G = (V,E) directed graph - each edge (u, v) : E has nonegative capacity c(u,v) >=0
# If (u, v) /: E , then c(u, v) =0
# 2 sources in network - source, and sink. Every vertex lies on the path from source to sink
# So the graph is connected, |E| >= |V|-1
# A flow in G is function F: V x V -> R such that:
# 1. Capacity constraint:  u, v : V, f(u,v) <= c(u,v)
# 2. Skew symmetry:  u, v: V then f(u,v)=-f(v,u)
# 3. Flow constraint: u : V - {s,t} then sum_v:V ( f(u,v)=0 )  - there are no other source/sink 
# Lemma: 
# 1. X \subset V => f(X, X) = 0
# 2. X, Y \subset V => f(X,Y) = -f(Y, X)
# 3. X, Y, Z \subset V and X \intesect Y = Ø => sum(X \union Y, Z ) = f(X,Z)+f(Y,Z) and f(Z,X\union Y)=f(Z,X)+f(Z,Y)

# Fold and Fulkerson: 1. Residual networks, augmenting paths, and cuts.
#def Ford-Fulkerson-method(G, s, t):
#    f = 0 #flow
#    while augmenting_path!={}:
#        augment(f, p)
#    return f

from collections import defaultdict

class Graph: 
    def __init__(self, graph):
        self.graph = graph  # residual graph
        self. ROW = len(graph)
 
    #Returns true if there is a path from source 's' to sink 't' in
    #residual graph. Also fills parent[] to store the path 
    def BFS(self, s, t, parent): #breath first search
        visited = [False]*(self.ROW) # Mark all the vertices as not visited
        queue = [] # Create a queue for BFS
        queue.append(s) # Mark the source node as visited and enqueue it
        visited[s] = True

        while queue: #BFS
            u = queue.pop(0) # Dequeue a vertex from queue and print it
 
            # Get all adjacent vertices of the dequeued vertex u
            # If a adjacent has not been visited, then mark it
            # visited and enqueue it
            for ind, val in enumerate(self.graph[u]):
                if visited[ind] == False and val > 0:
                    # If we find a connection to the sink node,
                    # then there is no point in BFS anymore
                    # We just have to set its parent and can return true
                    queue.append(ind)
                    visited[ind] = True
                    parent[ind] = u
                    if ind == t:
                        return True
 
        # We didn't reach sink in BFS starting
        # from source, so return false
        return False
             
    # Returns tne maximum flow from s to t in the given graph
    def FordFulkerson(self, source, sink):
 
        # This array is filled by BFS and to store path
        parent = [-1]*(self.ROW)
 
        max_flow = 0 # There is no flow initially
 
        # Augment the flow while there is path from source to sink
        while self.BFS(source, sink, parent) :
 
            # Find minimum residual capacity of the edges along the
            # path filled by BFS. Or we can say find the maximum flow
            # through the path found.
            path_flow = float("Inf")
            s = sink
            while(s !=  source):
                path_flow = min (path_flow, self.graph[parent[s]][s])
                s = parent[s]
 
            # Add path flow to overall flow
            max_flow +=  path_flow
 
            # update residual capacities of the edges and reverse edges
            # along the path
            v = sink
            while(v !=  source):
                u = parent[v]
                self.graph[u][v] -= path_flow
                self.graph[v][u] += path_flow
                v = parent[v]
 
        return max_flow
 
  
# Create a graph given in the above diagram
 
graph = [[0, 16, 13, 0, 0, 0],
        [0, 0, 10, 12, 0, 0],
        [0, 4, 0, 0, 14, 0],
        [0, 0, 9, 0, 0, 20],
        [0, 0, 0, 7, 0, 4],
        [0, 0, 0, 0, 0, 0]]
 
g = Graph(graph)
 
source = 0; sink = 5
  
print ("The maximum possible flow is %d " % g.FordFulkerson(source, sink))
 
# This code is contributed by Neelam Yadav



