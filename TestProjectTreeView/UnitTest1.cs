using NUnit.Framework;
using TestProjectTreeView;
using TreeViewProject;

namespace TestProjectTreeView
{
    public class Tests
    {             

        [Test]
        public void TestSearch()
        {
            // Arrange
            TreeView treeView = new TreeView();
            Folder rootFolder = new Folder("root");
            Folder folder1 = new Folder("DB Backups");
            Folder folder2 = new Folder("Documentation");
            Folder folder3 = new Folder("GIT");
            Folder folder4 = new Folder("Version1");
            Item item1 = new Item("dbTreeView.bak","database");
            Item item2 = new Item("Readme.doc", "textdoc");
            Item item3 = new Item("TreeView.cs", "code");
            Item item4 = new Item("UnitTest.cs", "code");
            Item item5 = new Item("Usings.cs", "code");
            Item item6 = new Item("help.txt", "textdoc");
            treeView.AddRootNode(rootFolder);
            treeView.AddFolder(rootFolder, folder1);
            treeView.AddFolder(rootFolder, folder2);
            treeView.AddFolder(folder1, folder3);
            treeView.AddItem(folder1, item1);
            treeView.AddItem(folder2, item2);
            treeView.AddItem(folder3, item3);
            treeView.AddItem(folder3, item4);
            treeView.AddItem(folder3, item5);
            treeView.AddFolder(folder2, folder4);
            treeView.AddItem(folder4, item6);

            // Assert
            Assert.Throws<ArgumentException>(() => new Item("",""));
            // Assert
            Assert.Throws<ArgumentException>(() => new Folder(""));

            // Act
            List<Node> searchResults = treeView.Search("*cs");

            // Assert
            Assert.That(searchResults.Count, Is.EqualTo(3));
            Assert.IsTrue(searchResults.Contains(item3));
            Assert.IsTrue(searchResults.Contains(item4));
            Assert.IsTrue(searchResults.Contains(item5));

            // Act
            searchResults = treeView.Search("*bak");

            // Assert
            Assert.That(searchResults.Count, Is.EqualTo(1));
            Assert.IsTrue(searchResults.Contains(item1));

            // Act
            searchResults = treeView.Search("*Doc*");

            // Assert
            Assert.That(searchResults.Count, Is.EqualTo(2));
            Assert.IsTrue(searchResults.Contains(folder2));
            Assert.IsTrue(searchResults.Contains(item2));

            // Act
            searchResults = treeView.Search("*xyz*");

            // Assert
            Assert.That(searchResults.Count, Is.EqualTo(0));

            // Act
            searchResults = treeView.SearchByType("textdoc");

            // Assert
            Assert.That(searchResults.Count, Is.EqualTo(2));

            // Act
            treeView.MoveItems(folder1,folder3);

            // Assert
            Assert.IsTrue(folder1.Children.Contains(folder3));

            // Act
            treeView.RemoveFolder(folder1);

            // Assert
            Assert.IsFalse(rootFolder.Children.Contains(folder1));
            Assert.IsFalse(folder3.Children.Contains(folder1));
            Assert.IsFalse(folder1.Children.Contains(item1));

            // Act
            treeView.RemoveFolder(folder2);

            // Assert
            Assert.IsFalse(rootFolder.Children.Contains(folder2));
            Assert.IsFalse(folder2.Children.Contains(folder4));
            Assert.IsFalse(folder4.Children.Contains(item6));

        }
    }
}
