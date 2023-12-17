using System;
using System.Collections.Generic;

namespace EditorExtensionsDemo
{
    public static class Orderer
        {
            private static readonly string HighestUC = "Highest Priority".ToUpperInvariant();
            private static readonly string HighUC = "High Priority".ToUpperInvariant();
            private static readonly string DefaultUC = "Default Priority".ToUpperInvariant();
            private static readonly string LowUC = "Low Priority".ToUpperInvariant();
            private static readonly string LowestUC = "Lowest Priority".ToUpperInvariant();

            public static IList<Lazy<TValue, TMetadata>> Order<TValue, TMetadata>(
              IEnumerable<Lazy<TValue, TMetadata>> itemsToOrder)
              where TValue : class
              where TMetadata : IOrderable
            {
                if (itemsToOrder == null)
                    throw new ArgumentNullException(nameof(itemsToOrder));
                Queue<Orderer.Node<TValue, TMetadata>> roots = new Queue<Orderer.Node<TValue, TMetadata>>();
                List<Orderer.Node<TValue, TMetadata>> unsortedItems = new List<Orderer.Node<TValue, TMetadata>>();
                Orderer.PrepareGraph<TValue, TMetadata>(itemsToOrder, roots, unsortedItems);
                return Orderer.TopologicalSort<TValue, TMetadata>(roots, unsortedItems);
            }

            private static void PrepareGraph<TValue, TMetadata>(
              IEnumerable<Lazy<TValue, TMetadata>> items,
              Queue<Orderer.Node<TValue, TMetadata>> roots,
              List<Orderer.Node<TValue, TMetadata>> unsortedItems)
              where TValue : class
              where TMetadata : IOrderable
            {
                Dictionary<string, Orderer.Node<TValue, TMetadata>> map = new Dictionary<string, Orderer.Node<TValue, TMetadata>>();
                foreach (Lazy<TValue, TMetadata> lazy in items)
                {
                    if (lazy != null && (object)lazy.Metadata != null)
                    {
                        Orderer.Node<TValue, TMetadata> node = new Orderer.Node<TValue, TMetadata>(lazy);
                        if (node.Name.Length != 0)
                        {
                            if (!map.ContainsKey(node.Name))
                            {
                                map.Add(node.Name, node);
                                unsortedItems.Add(node);
                            }
                        }
                        else
                            unsortedItems.Add(node);
                    }
                }
                for (int index = unsortedItems.Count - 1; index >= 0; --index)
                    unsortedItems[index].Resolve(map, unsortedItems);
                Orderer.Node<TValue, TMetadata> node1;
                if (map.TryGetValue(Orderer.HighestUC, out node1) && node1.Before.Count != 0)
                {
                    HashSet<Orderer.Node<TValue, TMetadata>> afterHighest = new HashSet<Orderer.Node<TValue, TMetadata>>();
                    Orderer.AddToAfterHighest<TValue, TMetadata>((IEnumerable<Orderer.Node<TValue, TMetadata>>)node1.Before, afterHighest);
                    for (int index = unsortedItems.Count - 1; index >= 0; --index)
                    {
                        Orderer.Node<TValue, TMetadata> unsortedItem = unsortedItems[index];
                        if (unsortedItem != node1 && !afterHighest.Contains(unsortedItem))
                        {
                            unsortedItem.Before.Add(node1);
                            node1.After.Add(unsortedItem);
                        }
                    }
                }
                Orderer.Node<TValue, TMetadata> node2;
                if (map.TryGetValue(Orderer.LowestUC, out node2) && node2.After.Count != 0)
                {
                    HashSet<Orderer.Node<TValue, TMetadata>> beforeLowest = new HashSet<Orderer.Node<TValue, TMetadata>>();
                    Orderer.AddToBeforeLowest<TValue, TMetadata>((IEnumerable<Orderer.Node<TValue, TMetadata>>)node2.After, beforeLowest);
                    for (int index = unsortedItems.Count - 1; index >= 0; --index)
                    {
                        Orderer.Node<TValue, TMetadata> unsortedItem = unsortedItems[index];
                        if (unsortedItem != node2 && !beforeLowest.Contains(unsortedItem))
                        {
                            unsortedItem.After.Add(node2);
                            node2.Before.Add(unsortedItem);
                        }
                    }
                }
                Orderer.AddPlaceHolders<TValue, TMetadata>(map, Orderer.LowestUC, Orderer.LowUC, Orderer.DefaultUC, Orderer.HighUC, Orderer.HighestUC);
                List<Orderer.Node<TValue, TMetadata>> newRoots = new List<Orderer.Node<TValue, TMetadata>>();
                for (int index = unsortedItems.Count - 1; index >= 0; --index)
                {
                    Orderer.Node<TValue, TMetadata> unsortedItem = unsortedItems[index];
                    if (unsortedItem.After.Count == 0)
                        newRoots.Add(unsortedItem);
                }
                Orderer.AddToRoots<TValue, TMetadata>(roots, newRoots);
            }

            private static void AddPlaceHolders<TValue, TMetadata>(
              Dictionary<string, Orderer.Node<TValue, TMetadata>> map,
              params string[] names)
              where TValue : class
              where TMetadata : IOrderable
            {
                Orderer.Node<TValue, TMetadata> node1 = (Orderer.Node<TValue, TMetadata>)null;
                for (int index = 0; index < names.Length; ++index)
                {
                    Orderer.Node<TValue, TMetadata> node2;
                    if (map.TryGetValue(names[index], out node2))
                    {
                        if (node1 != null)
                        {
                            node1.Before.Add(node2);
                            node2.After.Add(node1);
                        }
                        node1 = node2;
                    }
                }
            }

            private static void AddToAfterHighest<TValue, TMetadata>(
              IEnumerable<Orderer.Node<TValue, TMetadata>> nodes,
              HashSet<Orderer.Node<TValue, TMetadata>> afterHighest)
              where TValue : class
              where TMetadata : IOrderable
            {
                foreach (Orderer.Node<TValue, TMetadata> node in nodes)
                {
                    if (afterHighest.Add(node) && node.Before.Count != 0)
                        Orderer.AddToAfterHighest<TValue, TMetadata>((IEnumerable<Orderer.Node<TValue, TMetadata>>)node.Before, afterHighest);
                }
            }

            private static void AddToBeforeLowest<TValue, TMetadata>(
              IEnumerable<Orderer.Node<TValue, TMetadata>> nodes,
              HashSet<Orderer.Node<TValue, TMetadata>> beforeLowest)
              where TValue : class
              where TMetadata : IOrderable
            {
                foreach (Orderer.Node<TValue, TMetadata> node in nodes)
                {
                    if (beforeLowest.Add(node) && node.After.Count != 0)
                        Orderer.AddToBeforeLowest<TValue, TMetadata>((IEnumerable<Orderer.Node<TValue, TMetadata>>)node.After, beforeLowest);
                }
            }

            private static IList<Lazy<TValue, TMetadata>> TopologicalSort<TValue, TMetadata>(
              Queue<Orderer.Node<TValue, TMetadata>> roots,
              List<Orderer.Node<TValue, TMetadata>> unsortedItems)
              where TValue : class
              where TMetadata : IOrderable
            {
                List<Lazy<TValue, TMetadata>> lazyList = new List<Lazy<TValue, TMetadata>>();
                while (unsortedItems.Count > 0)
                {
                    Orderer.Node<TValue, TMetadata> node = roots.Count == 0 ? Orderer.BreakCircularReference<TValue, TMetadata>(unsortedItems) : roots.Dequeue();
                    if (node.Item != null)
                        lazyList.Add(node.Item);
                    unsortedItems.Remove(node);
                    node.ClearBefore(roots);
                }
                return (IList<Lazy<TValue, TMetadata>>)lazyList;
            }

            private static void AddToRoots<TValue, TMetadata>(
              Queue<Orderer.Node<TValue, TMetadata>> roots,
              List<Orderer.Node<TValue, TMetadata>> newRoots)
              where TValue : class
              where TMetadata : IOrderable
            {
                newRoots.Sort((Comparison<Orderer.Node<TValue, TMetadata>>)((l, r) => string.CompareOrdinal(l.Name, r.Name)));
                for (int index = 0; index < newRoots.Count; ++index)
                    roots.Enqueue(newRoots[index]);
            }

            private static Orderer.Node<TValue, TMetadata> BreakCircularReference<TValue, TMetadata>(
              List<Orderer.Node<TValue, TMetadata>> unsortedItems)
              where TValue : class
              where TMetadata : IOrderable
            {
                List<List<Orderer.Node<TValue, TMetadata>>> cycles = Orderer.FindCycles<TValue, TMetadata>(unsortedItems);
                int num1 = int.MaxValue;
                List<Orderer.Node<TValue, TMetadata>> nodeList1 = (List<Orderer.Node<TValue, TMetadata>>)null;
                for (int index1 = 0; index1 < cycles.Count; ++index1)
                {
                    List<Orderer.Node<TValue, TMetadata>> nodeList2 = cycles[index1];
                    int num2 = 0;
                    for (int index2 = 0; index2 < nodeList2.Count; ++index2)
                    {
                        Orderer.Node<TValue, TMetadata> node1 = nodeList2[index2];
                        foreach (Orderer.Node<TValue, TMetadata> node2 in node1.After)
                        {
                            if (node2.LowIndex != node1.LowIndex)
                            {
                                ++num2;
                                break;
                            }
                        }
                    }
                    if (num2 < num1)
                    {
                        nodeList1 = nodeList2;
                        num1 = num2;
                    }
                }
                Orderer.Node<TValue, TMetadata> node3;
                if (nodeList1 == null)
                {
                    node3 = unsortedItems[0];
                }
                else
                {
                    node3 = nodeList1[0];
                    for (int index = 1; index < nodeList1.Count; ++index)
                    {
                        Orderer.Node<TValue, TMetadata> node4 = nodeList1[index];
                        if (node4.After.Count < node3.After.Count)
                            node3 = node4;
                    }
                }
                foreach (Orderer.Node<TValue, TMetadata> node5 in node3.After)
                    node5.Before.Remove(node3);
                node3.After.Clear();
                return node3;
            }

            private static List<List<Orderer.Node<TValue, TMetadata>>> FindCycles<TValue, TMetadata>(
              List<Orderer.Node<TValue, TMetadata>> unsortedItems)
              where TValue : class
              where TMetadata : IOrderable
            {
                for (int index = 0; index < unsortedItems.Count; ++index)
                {
                    Orderer.Node<TValue, TMetadata> unsortedItem = unsortedItems[index];
                    unsortedItem.Index = -1;
                    unsortedItem.LowIndex = -1;
                    unsortedItem.ContainedInKnownCycle = false;
                }
                List<List<Orderer.Node<TValue, TMetadata>>> cycles = new List<List<Orderer.Node<TValue, TMetadata>>>();
                Stack<Orderer.Node<TValue, TMetadata>> stack = new Stack<Orderer.Node<TValue, TMetadata>>(unsortedItems.Count);
                int index1 = 0;
                for (int index2 = 0; index2 < unsortedItems.Count; ++index2)
                {
                    Orderer.Node<TValue, TMetadata> unsortedItem = unsortedItems[index2];
                    if (unsortedItem.Index == -1)
                        Orderer.FindCycles<TValue, TMetadata>(unsortedItem, stack, ref index1, cycles);
                }
                return cycles;
            }

            private static void FindCycles<TValue, TMetadata>(
              Orderer.Node<TValue, TMetadata> node,
              Stack<Orderer.Node<TValue, TMetadata>> stack,
              ref int index,
              List<List<Orderer.Node<TValue, TMetadata>>> cycles)
              where TValue : class
              where TMetadata : IOrderable
            {
                node.Index = index;
                node.LowIndex = index;
                ++index;
                stack.Push(node);
                foreach (Orderer.Node<TValue, TMetadata> node1 in node.Before)
                {
                    if (node1.Index == -1)
                    {
                        Orderer.FindCycles<TValue, TMetadata>(node1, stack, ref index, cycles);
                        node.LowIndex = Math.Min(node.LowIndex, node1.LowIndex);
                    }
                    else if (!node1.ContainedInKnownCycle)
                        node.LowIndex = Math.Min(node.LowIndex, node1.Index);
                }
                if (node.Index != node.LowIndex)
                    return;
                List<Orderer.Node<TValue, TMetadata>> nodeList = new List<Orderer.Node<TValue, TMetadata>>();
                while (stack.Count > 0)
                {
                    Orderer.Node<TValue, TMetadata> node2 = stack.Pop();
                    nodeList.Add(node2);
                    node2.ContainedInKnownCycle = true;
                    if (node2 == node)
                    {
                        if (nodeList.Count <= 1)
                            break;
                        cycles.Add(nodeList);
                        break;
                    }
                }
            }

            private class Node<TValue, TMetadata>
              where TValue : class
              where TMetadata : IOrderable
            {
                public readonly string Name;
                public readonly Lazy<TValue, TMetadata> Item;
                private HashSet<Orderer.Node<TValue, TMetadata>> _after = new HashSet<Orderer.Node<TValue, TMetadata>>();
                private HashSet<Orderer.Node<TValue, TMetadata>> _before = new HashSet<Orderer.Node<TValue, TMetadata>>();
                public int Index = -1;
                public int LowIndex = -1;
                public bool ContainedInKnownCycle;

                public HashSet<Orderer.Node<TValue, TMetadata>> After => this._after;

                public HashSet<Orderer.Node<TValue, TMetadata>> Before => this._before;

                public Node(Lazy<TValue, TMetadata> item)
                {
                    string name = item.Metadata.Name;
                    this.Name = string.IsNullOrEmpty(name) ? string.Empty : name.ToUpperInvariant();
                    this.Item = item;
                }

                public Node(string name) => this.Name = name;

                public void Resolve(
                  Dictionary<string, Orderer.Node<TValue, TMetadata>> map,
                  List<Orderer.Node<TValue, TMetadata>> unsortedItems)
                {
                    this.Resolve(map, this.Item.Metadata.After, this._after, unsortedItems);
                    this.Resolve(map, this.Item.Metadata.Before, this._before, unsortedItems);
                    foreach (Orderer.Node<TValue, TMetadata> node in this._before)
                        node._after.Add(this);
                    foreach (Orderer.Node<TValue, TMetadata> node in this._after)
                        node._before.Add(this);
                }

                public void ClearBefore(Queue<Orderer.Node<TValue, TMetadata>> roots)
                {
                    List<Orderer.Node<TValue, TMetadata>> newRoots = new List<Orderer.Node<TValue, TMetadata>>();
                    foreach (Orderer.Node<TValue, TMetadata> node in this.Before)
                    {
                        node.After.Remove(this);
                        if (node.After.Count == 0)
                            newRoots.Add(node);
                    }
                    this.Before.Clear();
                    Orderer.AddToRoots<TValue, TMetadata>(roots, newRoots);
                }

                public override string ToString() => this.Name;

                private void Resolve(
                  Dictionary<string, Orderer.Node<TValue, TMetadata>> map,
                  IEnumerable<string> links,
                  HashSet<Orderer.Node<TValue, TMetadata>> results,
                  List<Orderer.Node<TValue, TMetadata>> unsortedItems)
                {
                    if (links == null)
                        return;
                    foreach (string link in links)
                    {
                        if (!string.IsNullOrEmpty(link))
                        {
                            string upperInvariant = link.ToUpperInvariant();
                            Orderer.Node<TValue, TMetadata> node;
                            if (!map.TryGetValue(upperInvariant, out node))
                            {
                                node = new Orderer.Node<TValue, TMetadata>(upperInvariant);
                                map.Add(upperInvariant, node);
                                unsortedItems.Add(node);
                            }
                            if (node != this)
                                results.Add(node);
                        }
                    }
                }
            }
        }
}
