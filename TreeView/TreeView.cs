
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TreeViewProject
{
    public class Node
    {
        public string Name { get; set; }
        public List<Node> Children { get; set; }
    }

    public class Folder : Node
    { 

        public Folder(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("string cannot be empty", nameof(name));
            }
            Name = name;
            Children = new List<Node>();
        }
    }

    public class Item : Node
    {
        public string ItemType { get; set; }
        public Item(string name, string itemType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("string cannot be empty", nameof(name));
            }
            Name = name;
            Children = null;
            ItemType = itemType;
        }
    }

    public class TreeView
    {
        public Folder RootNode { get; set; }

        public TreeView()
        {
            RootNode = null;
        }

        public void AddRootNode(Folder rootNode)
        {
            RootNode = rootNode;
        }

        public void AddFolder(Folder parentFolder, Folder newFolder)
        {
            parentFolder.Children.Add(newFolder);
        }

        public void AddItem(Folder parentFolder, Item newItem)
        {
            parentFolder.Children.Add(newItem);
        }

        public void MoveItems(Folder sourceFolder, Folder destinationFolder)
        {
            List<Item> itemsToMove = sourceFolder.Children.OfType<Item>().ToList();
            foreach (Item item in itemsToMove)
            {
                sourceFolder.Children.Remove(item);
                destinationFolder.Children.Add(item);
            }
        }

        public void RemoveFolder(Folder folderToRemove)
        {
            for (int i = 0; i < folderToRemove.Children.Count; i++)
            {
                Node childNode = folderToRemove.Children[i];
                if (childNode is Folder)
                {
                    RemoveFolder(childNode as Folder);
                    i--; //decrement to avoid skipping a node as folderToRemove.Children.Count is decreased when we remove a node
                }
                else if (childNode is Item)
                {
                    folderToRemove.Children.Remove(childNode);
                    i--; //decrement to avoid skipping a node as folderToRemove.Children.Count is decreased when we remove a node
                }
            }
            folderToRemove.Children.Clear();

            if (folderToRemove != RootNode)
            {
                Folder parentFolder = FindParentFolder(RootNode, folderToRemove);
                parentFolder.Children.Remove(folderToRemove);
            }
            else
            {
                RootNode = null;
            }
        }

        public List<Node> Search(string searchText)
        {
            List<Node> results = new List<Node>();
            SearchNode(RootNode, searchText, results);
            return results;
        }

        private void SearchNode(Folder currentNode, string searchText, List<Node> results)
        {
            foreach (Node childNode in currentNode.Children)
            {
                if (childNode is Folder)
                {
                    Folder childFolder = childNode as Folder;
                    if (MatchesWildcard(childFolder.Name, searchText))
                    {
                        results.Add(childFolder);
                    }
                    SearchNode(childFolder, searchText, results);
                }
                else if (childNode is Item)
                {
                    Item childItem = childNode as Item;
                    if (MatchesWildcard(childItem.Name, searchText))
                    {
                        results.Add(childItem);
                    }
                }
            }
        }
        public List<Node> SearchByType(string itemType)
        {
            List<Node> results = new List<Node>();
            SearchNodeByType(RootNode, itemType, results);
            return results;
        }

        private void SearchNodeByType(Folder currentNode, string itemType, List<Node> results)
        {
            foreach (Node childNode in currentNode.Children)
            {
                if (childNode is Folder)
                {
                    Folder childFolder = childNode as Folder;
                    SearchNodeByType(childFolder, itemType, results);
                }
                else if (childNode is Item)
                {
                    Item childItem = childNode as Item;
                    if (childItem.ItemType == itemType)
                    {
                        results.Add(childItem);
                    }
                }
            }
        }

        private bool MatchesWildcard(string input, string pattern)
        {
            string regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
            Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(input);
        }

        private Folder FindParentFolder(Folder currentNode, Folder folderToFind)
        {
            Folder nodeToReturn = null;
            foreach (Node childNode in currentNode.Children)
            {
                if (childNode == folderToFind)
                {
                    nodeToReturn= currentNode;
                }
                else if (childNode is Folder)
                {
                    Folder parentFolder = FindParentFolder(childNode as Folder, folderToFind);
                    if (parentFolder != null)
                    {
                        nodeToReturn= parentFolder;
                    }
                }
            }
            return nodeToReturn;
        }
    }
}
    