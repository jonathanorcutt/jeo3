using Blazor.Diagrams;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using GraphShape.Algorithms.Layout;
using GraphShape.Algorithms.OverlapRemoval;
using QuikGraph;

namespace JEO3.Diagrams
{
    public static partial class DiagramBuilder
    {
        public static async Task ApplyGraphShapeLayoutER(BlazorDiagram diagram, ERParameters parameters)
        {
            if (!diagram.Nodes.Any()) return;

            // Build QuikGraph
            var quikGraph = new BidirectionalGraph<NodeModel, Edge<NodeModel>>();

            foreach (var node in diagram.Nodes)
                quikGraph.AddVertex(node);

            foreach (var link in diagram.Links)
            {
                if (link.Source?.Model is NodeModel src &&
                    link.Target?.Model is NodeModel tgt)
                {
                    quikGraph.AddEdge(new Edge<NodeModel>(src, tgt));
                }
            }

            // Build GraphShape context
            var positions = diagram.Nodes.ToDictionary(
                n => n,
                n => new GraphShape.Point(n.Position.X, n.Position.Y)
            );

            var sizes = diagram.Nodes.ToDictionary(
                n => n,
                n => new GraphShape.Size(n.Size?.Width ?? 100, n.Size?.Height ?? 40)
            );

            // Build ER parameters for GraphShape
            var erParams = new ERParameters()
            {
                CanvasWidth = parameters.CanvasWidth,
                CanvasHeight = parameters.CanvasHeight,
                ForceHorizontal = parameters.ForceHorizontal,
                ForceVertical = parameters.ForceVertical,
                ParentChildSpacing = parameters.ParentChildSpacing,
                SiblingSpacing = parameters.SiblingSpacing,
                Alignment = parameters.Alignment
            };

            // Create ER algorithm
            var context = new LayoutContext<NodeModel, Edge<NodeModel>, BidirectionalGraph<NodeModel, Edge<NodeModel>>>(
                quikGraph,
                positions,
                sizes,
                LayoutMode.Simple
            );
            var algorithm = new ERLayoutAlgorithm(erParams, context);
            await ComputeNodeAlgorithm(algorithm);

            // Overlap removal (FSA)
            var rectangles = algorithm.VerticesPositions.ToDictionary(
                kvp => (TableNodeModel)kvp.Key,
                kvp => new GraphShape.Rect(
                    kvp.Value.X,
                    kvp.Value.Y,
                    (int)(kvp.Key.Size?.Width ?? 100),
                    (int)(kvp.Key.Size?.Height ?? 40)
                )
            );

            var overlapParameters = new OverlapRemovalParameters
            {
                HorizontalGap = 30,
                VerticalGap = 30
            };

            var overlapAlgorithm = new FSAAlgorithm<TableNodeModel>(rectangles, overlapParameters);
            await ComputeQueryNodeOverlapAlgorithm(overlapAlgorithm);

            // Commit positions
            diagram.Batch(() =>
            {
                foreach (var kvp in overlapAlgorithm.Rectangles)
                {
                    kvp.Key.SetPosition(kvp.Value.X, kvp.Value.Y);
                }
            });

            diagram.Batch(() =>
            {
                foreach (var kvp in algorithm.VerticesPositions)
                {
                    kvp.Key.SetPosition(kvp.Value.X, kvp.Value.Y);
                }
            });
        }

        public static async Task ApplyGraphShapeLayoutER2_Orig(BlazorDiagram diagram, ERParameters parameters)
        {
            var nodes = diagram.Nodes.ToList();
            var links = diagram.Links.ToList();

            if (!nodes.Any()) return;

            var positions = nodes.ToDictionary(n => n, n => new Point(n.Position.X, n.Position.Y));
            var displacements = new Dictionary<NodeModel, Point>();

            double kSquared = parameters.IdealDistance * parameters.IdealDistance;
            var rand = new Random(42);

            // Pre-scatter: Initial bounds
            foreach (var node in nodes)
            {
                if (positions[node].X == 0 && positions[node].Y == 0)
                {
                    positions[node] = new Point(rand.NextDouble() * 500, rand.NextDouble() * 500);
                }
            }

            // Map Hierarchy Levels for Spacing Primitives
            // We use the connection flow to determine Parent/Child vs Sibling status
            var parentChildLinks = links
                .Where(l => l.Source?.Model is NodeModel && l.Target?.Model is NodeModel)
                .ToList();

            // Physics Loop (Respects parameters.Iterations)
            for (int step = 0; step < parameters.Iterations; step++)
            {
                foreach (var node in nodes) displacements[node] = new Point(0, 0);

                // Repulsion Pass (Incorporate Sibling Spacing heuristic)
                for (int i = 0; i < nodes.Count; i++)
                {
                    var v = nodes[i];
                    for (int j = i + 1; j < nodes.Count; j++)
                    {
                        var u = nodes[j];
                        double dx = positions[v].X - positions[u].X;
                        double dy = positions[v].Y - positions[u].Y;

                        if (dx == 0 && dy == 0)
                        {
                            dx = (rand.NextDouble() - 0.5) * 5;
                            dy = (rand.NextDouble() - 0.5) * 5;
                        }

                        double distance = Math.Sqrt((dx * dx) + (dy * dy));
                        if (distance > 0)
                        {
                            // 🌟 Apply SiblingSpacing constraint modifier if they share a common parent parent
                            bool areSiblings = links.Any(l1 => l1.Target?.Model == v && links.Any(l2 => l2.Target?.Model == u && l1.Source?.Model == l2.Source?.Model));
                            double targetClearance = areSiblings ? parameters.SiblingSpacing : 100.0;

                            double force = (distance < targetClearance) ? (kSquared / distance) * 4 : (kSquared / distance);

                            // 🌟 Respect ForceHorizontal / ForceVertical constraints on push-away vectors
                            double fx = (parameters.ForceVertical && !parameters.ForceHorizontal) ? 0 : (dx / distance * force);
                            double fy = (parameters.ForceHorizontal && !parameters.ForceVertical) ? 0 : (dy / distance * force);

                            displacements[v] = new Point(displacements[v].X + fx, displacements[v].Y + fy);
                            displacements[u] = new Point(displacements[u].X - fx, displacements[u].Y - fy);
                        }
                    }
                }

                // Attraction Pass (Respect ParentChildSpacing primitive)
                foreach (var link in links)
                {
                    if (link.Source?.Model is not NodeModel v || link.Target?.Model is not NodeModel u) continue;

                    double dx = positions[v].X - positions[u].X;
                    double dy = positions[v].Y - positions[u].Y;
                    double distance = Math.Sqrt((dx * dx) + (dy * dy));

                    if (distance > 0)
                    {
                        // 🌟 Dynamically scale spring length baseline by ParentChildSpacing param
                        double currentIdeal = parameters.IdealDistance + parameters.ParentChildSpacing;
                        double force = (distance * distance) / currentIdeal;

                        // 🌟 Filter vectors based on axis constraints constraints
                        double fx = (parameters.ForceVertical && !parameters.ForceHorizontal) ? 0 : (dx / distance * force);
                        double fy = (parameters.ForceHorizontal && !parameters.ForceVertical) ? 0 : (dy / distance * force);

                        displacements[v] = new Point(displacements[v].X - fx, displacements[v].Y - fy);
                        displacements[u] = new Point(displacements[u].X + fx, displacements[u].Y + fy);
                    }
                }

                // 🌟 2. Apply Alignment Primitive Rule (0 = Left, 1 = Center, 2 = Right)
                if (parameters.ForceVertical || parameters.ForceHorizontal)
                {
                    double averageX = positions.Values.Average(p => p.X);
                    foreach (var node in nodes)
                    {
                        // Pull node columns or rows into hard structural tracks based on selection
                        if (parameters.Alignment == 0) // Left align focus track
                        {
                            displacements[node] = new Point(displacements[node].X - (positions[node].X * 0.02), displacements[node].Y);
                        }
                        else if (parameters.Alignment == 2) // Right align focus track
                        {
                            displacements[node] = new Point(displacements[node].X + ((1200 - positions[node].X) * 0.02), displacements[node].Y);
                        }
                        else // Center Track Alignment
                        {
                            double alignmentOffset = averageX - positions[node].X;
                            displacements[node] = new Point(displacements[node].X + (alignmentOffset * 0.02), displacements[node].Y);
                        }
                    }
                }

                // Apply Velocities & Cool Down (Respects parameters.Temperature)
                foreach (var node in nodes)
                {
                    double dx = displacements[node].X;
                    double dy = displacements[node].Y;
                    double distance = Math.Sqrt((dx * dx) + (dy * dy));

                    if (distance > 0)
                    {
                        double moveX = dx / distance * Math.Min(distance, parameters.Temperature);
                        double moveY = dy / distance * Math.Min(distance, parameters.Temperature);

                        positions[node] = new Point(positions[node].X + moveX, positions[node].Y + moveY);
                    }
                }

                // De-escalate temperature step sequence sequence
                parameters.Temperature *= 0.95;
            }

            // Normalize Coordinates & Commit to Canvas
            double minX = positions.Values.Min(p => p.X);
            double minY = positions.Values.Min(p => p.Y);

            diagram.Batch(() =>
            {
                foreach (var node in nodes)
                {
                    double finalX = positions[node].X - minX + 50;
                    double finalY = positions[node].Y - minY + 50;
                    node.SetPosition(finalX, finalY);
                }
            });
        }

        public static async Task ApplyGraphShapeLayoutER2(BlazorDiagram diagram, DiagramGraphModel? activeGraph, ERParameters parameters)
        {
            var nodes = diagram.Nodes.ToList();
            var links = diagram.Links.ToList();

            if (!nodes.Any()) return;

            var positions = nodes.ToDictionary(n => n, n => new Point(n.Position.X, n.Position.Y));
            var displacements = new Dictionary<NodeModel, Point>();

            double kSquared = parameters.IdealDistance * parameters.IdealDistance;
            var rand = new Random(42);

            // Pre-scatter nodes sitting at origin to avoid overlapping infinities
            foreach (var node in nodes)
            {
                if (positions[node].X == 0 && positions[node].Y == 0)
                {
                    positions[node] = new Point(rand.NextDouble() * 500, rand.NextDouble() * 500);
                }
            }

            // 🔥 METADATA STEP 1: Correlate Blazor UI Node Models directly with the Query Tracker ViewModels
            var nodeMetadata = new Dictionary<NodeModel, DiagramNodeViewModel>();
            if (activeGraph != null)
            {
                foreach (var canvasNode in nodes)
                {
                    var meta = activeGraph.Nodes.FirstOrDefault(n =>
                        n.Alias == canvasNode.Title ||
                        n.TableName == canvasNode.Title) ?? activeGraph.Nodes.FirstOrDefault(n => n.NodeId.ToString() == canvasNode.Id);

                    if (meta != null)
                    {
                        nodeMetadata[canvasNode] = meta;
                    }
                }
            }

            // 🔥 METADATA STEP 2: Structural Edge Traversal Caching
            var parentIdToChildIds = new Dictionary<Guid, HashSet<Guid>>();
            var childIdToParentIds = new Dictionary<Guid, HashSet<Guid>>();

            if (activeGraph != null)
            {
                foreach (var edge in activeGraph.Edges)
                {
                    if (!parentIdToChildIds.ContainsKey(edge.SourceNodeId))
                        parentIdToChildIds[edge.SourceNodeId] = new HashSet<Guid>();
                    parentIdToChildIds[edge.SourceNodeId].Add(edge.TargetNodeId);

                    if (!childIdToParentIds.ContainsKey(edge.TargetNodeId))
                        childIdToParentIds[edge.TargetNodeId] = new HashSet<Guid>();
                    childIdToParentIds[edge.TargetNodeId].Add(edge.SourceNodeId);
                }
            }

            // 🔥 METADATA STEP 3: O(1) Sibling Identification Cache
            var siblingCache = new Dictionary<NodeModel, HashSet<NodeModel>>();
            foreach (var v in nodes)
            {
                siblingCache[v] = new HashSet<NodeModel>();
                if (nodeMetadata.TryGetValue(v, out var vMeta) && childIdToParentIds.TryGetValue(vMeta.NodeId, out var parents))
                {
                    foreach (var parentId in parents)
                    {
                        if (parentIdToChildIds.TryGetValue(parentId, out var childrenIds))
                        {
                            foreach (var siblingNode in nodes)
                            {
                                if (siblingNode != v && nodeMetadata.TryGetValue(siblingNode, out var sMeta) && childrenIds.Contains(sMeta.NodeId))
                                {
                                    siblingCache[v].Add(siblingNode);
                                }
                            }
                        }
                    }
                }
            }

            // Localize temperature calculation to preserve parameters instance state
            double currentTemperature = parameters.Temperature;
            int maxDepthFound = activeGraph?.Nodes.Any() == true ? activeGraph.Nodes.Max(n => n.Depth) : 1;
            if (maxDepthFound <= 0) maxDepthFound = 1;

            // Physics Loop
            for (int step = 0; step < parameters.Iterations; step++)
            {
                foreach (var node in nodes) displacements[node] = new Point(0, 0);

                // --- Repulsion Pass ---
                for (int i = 0; i < nodes.Count; i++)
                {
                    var v = nodes[i];
                    for (int j = i + 1; j < nodes.Count; j++)
                    {
                        var u = nodes[j];
                        double dx = positions[v].X - positions[u].X;
                        double dy = positions[v].Y - positions[u].Y;

                        if (dx == 0 && dy == 0)
                        {
                            dx = (rand.NextDouble() - 0.5) * 10;
                            dy = (rand.NextDouble() - 0.5) * 10;
                        }

                        double distance = Math.Sqrt((dx * dx) + (dy * dy));
                        if (distance > 0)
                        {
                            bool areSiblings = siblingCache[v].Contains(u);
                            double targetClearance = areSiblings ? parameters.SiblingSpacing : 120.0;

                            double force = kSquared / distance;

                            if (distance < targetClearance)
                            {
                                force *= (targetClearance / distance);
                            }

                            double fx = (parameters.ForceVertical && !parameters.ForceHorizontal) ? 0 : (dx / distance * force);
                            double fy = (parameters.ForceHorizontal && !parameters.ForceVertical) ? 0 : (dy / distance * force);

                            displacements[v] = new Point(displacements[v].X + fx, displacements[v].Y + fy);
                            displacements[u] = new Point(displacements[u].X - fx, displacements[u].Y - fy);
                        }
                    }
                }

                // --- 2. Attraction Pass ---
                foreach (var link in links)
                {
                    if (link.Source?.Model is not NodeModel v || link.Target?.Model is not NodeModel u) continue;

                    double dx = positions[v].X - positions[u].X;
                    double dy = positions[v].Y - positions[u].Y;
                    double distance = Math.Sqrt((dx * dx) + (dy * dy));

                    if (distance > 0)
                    {
                        double currentIdeal = parameters.IdealDistance + parameters.ParentChildSpacing;
                        double force = (distance * distance) / currentIdeal;

                        double fx = (parameters.ForceVertical && !parameters.ForceHorizontal) ? 0 : (dx / distance * force);
                        double fy = (parameters.ForceHorizontal && !parameters.ForceVertical) ? 0 : (dy / distance * force);

                        displacements[v] = new Point(displacements[v].X - fx, displacements[v].Y - fy);
                        displacements[u] = new Point(displacements[u].X + fx, displacements[u].Y + fy);
                    }
                }

                // --- 3. Metadata Depth-Locked Tracking Constraints Pass ---
                if (parameters.ForceHorizontal && !parameters.ForceVertical)
                {
                    double stepWidth = parameters.CanvasWidth / (maxDepthFound + 1);
                    foreach (var node in nodes)
                    {
                        if (nodeMetadata.TryGetValue(node, out var meta))
                        {
                            double targetX = 50 + (meta.Depth * stepWidth);
                            double alignmentOffset = targetX - positions[node].X;

                            // Pull horizontal tracks into clean alignment lanes
                            displacements[node] = new Point(displacements[node].X + (alignmentOffset * 0.25), displacements[node].Y);
                        }
                    }
                }
                else if (parameters.ForceVertical && !parameters.ForceHorizontal)
                {
                    double stepHeight = parameters.CanvasHeight / (maxDepthFound + 1);
                    foreach (var node in nodes)
                    {
                        if (nodeMetadata.TryGetValue(node, out var meta))
                        {
                            double targetY = 50 + (meta.Depth * stepHeight);
                            double alignmentOffset = targetY - positions[node].Y;

                            // Pull vertical tracks into clean alignment lanes
                            displacements[node] = new Point(displacements[node].X, displacements[node].Y + (alignmentOffset * 0.25));
                        }
                    }
                }

                // --- 4. Apply Velocities & Cooling ---
                foreach (var node in nodes)
                {
                    double dx = displacements[node].X;
                    double dy = displacements[node].Y;
                    double distance = Math.Sqrt((dx * dx) + (dy * dy));

                    if (distance > 0)
                    {
                        double moveX = dx / distance * Math.Min(distance, currentTemperature);
                        double moveY = dy / distance * Math.Min(distance, currentTemperature);

                        positions[node] = new Point(positions[node].X + moveX, positions[node].Y + moveY);
                    }
                }

                currentTemperature *= 0.95;
                if (currentTemperature < 0.1) break;
            }

            // Normalize Coordinates & Commit to Canvas
            double finalMinX = positions.Values.Min(p => p.X);
            double finalMinY = positions.Values.Min(p => p.Y);

            diagram.Batch(() =>
            {
                foreach (var node in nodes)
                {
                    double finalX = positions[node].X - finalMinX + 50;
                    double finalY = positions[node].Y - finalMinY + 50;
                    node.SetPosition(finalX, finalY);
                }
            });
        }
    }
}
