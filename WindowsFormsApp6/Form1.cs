using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            treeView1.BeforeSelect += TreeView1_BeforeSelect;
            treeView1.BeforeExpand += TreeViev1_BeforeExpand;
            FillDriveNodes();
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            ImageList imageList1 = new ImageList();
            imageList1.Images.Add(Bitmap.FromFile(currentDirectory+@"\file.jpg"));
            listView1.SmallImageList = imageList1;
            ImageList imageList2 = new ImageList();
            imageList2.Images.Add(Bitmap.FromFile(currentDirectory +@"\folder.jpg"));
            treeView1.ImageList = imageList2;
        }

        /// <summary>
        /// Получение дочерних узлов для определенного узла
        /// </summary>
        /// <param name="node">Узел, для которого хотим получить</param>
        /// <param name="path">Путь к узлу</param>
        private void FillTreeNode (TreeNode node, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (var directory in dirs)
                {
                    var Node = new TreeNode();
                    Node.Text = directory.Remove(0, directory.LastIndexOf("\\") + 1);
                    node.Nodes.Add(Node);
                }
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// Получение всех дисков на ПК
        /// </summary>
        private void FillDriveNodes()
        {
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    var Node = new TreeNode(Text = drive.Name) ;
                    Node.Tag = drive;
                    FillTreeNode(Node, drive.Name);
                    treeView1.Nodes.Add(Node);
                }
            }
            catch
            {
            }
        }


        // Событие перед раскрытием узла
        void TreeViev1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                dirs = Directory.GetDirectories(e.Node.FullPath);
                if (dirs.Length != 0)
                {
                    for (int i = 0; i < dirs.Length; i++)
                    {
                      DirectoryInfo info = new DirectoryInfo(dirs[i]);
                      var Node = new TreeNode(info.Name);
                      Node.Tag = info;
                      FillTreeNode(Node, dirs[i]);
                      e.Node.Nodes.Add(Node);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        //Событие перед выделением узла
        void TreeView1_BeforeSelect (object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            if (Directory.Exists(e.Node.FullPath))
            {
                dirs = Directory.GetDirectories(e.Node.FullPath);
                if (dirs.Length != 0)
                {
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        DirectoryInfo info = new DirectoryInfo(dirs[i]);
                        var Node = new TreeNode(info.Name);
                        Node.Tag = info;
                        FillTreeNode(Node, dirs[i]);
                        e.Node.Nodes.Add(Node);
                    }
                }
            }
        }

        //Событие, при проишествии которого открывается содержимое папки (двойной клик)
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            listView1.Items.Clear();
            var nodeDirInfo = (DirectoryInfo)e.Node.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name + " " + file.Length + " bytes", 0);
                subItems = new ListViewItem.ListViewSubItem[]
                {
                    new ListViewItem.ListViewSubItem(item, "File"),
                    new ListViewItem.ListViewSubItem(item,file.LastAccessTime.ToShortDateString())
                };
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            listView1.Sorting = SortOrder.Ascending;
        }

    }
}
