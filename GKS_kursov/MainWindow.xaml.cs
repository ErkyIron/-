using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;


namespace GKS_kursov
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Data (*.txt)|*.txt|All files (*.*)|*.*";

            if (ofd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(readData.Document.ContentStart, readData.Document.ContentEnd);
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".rtf")
                        doc.Load(fs, DataFormats.Rtf);
                    else if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".txt")
                        doc.Load(fs, DataFormats.Text);
                    else
                        doc.Load(fs, DataFormats.Xaml);
                }
            }
        }

        private void ButtonShow_Click(object sender, RoutedEventArgs e)
        {

            grid3.ShowGridLines = true;

            int n = 0, k = 0;
            grid3.Children.Clear();
            grid3.RowDefinitions.Clear();
            grid3.ColumnDefinitions.Clear();

            tb.Text = "";

            string rData = new TextRange(readData.Document.ContentStart,
                                           readData.Document.ContentEnd).Text;
            List<string> listOperation = rData.Split(new[] { Environment.NewLine },
                                    StringSplitOptions.RemoveEmptyEntries).ToList();

            n = listOperation.Count();

            List<string>[] arr1list = new List<string>[n];
            for (int i = 0; i < arr1list.Length - 1; i++)
            {
                arr1list[i] = new List<string>();
            }

            int it = 0;

            foreach (string item in listOperation)
            {
                arr1list[it] = item.Split(new char[] { ' ' },
                                         StringSplitOptions.RemoveEmptyEntries).ToList();
                it++;
            }


            #region CreateUniqueList
            List<string> unique_list = new List<string>();

            for (int i = 0; i < n; i++)
            {
                foreach (string item in arr1list[i])
                {
                    if (unique_list.FindIndex(x => x == item) == -1)
                    {
                        unique_list.Add(item);
                    }
                }
            }
            #endregion

            #region OutUniqueList
            tb.Text += "Unique elements\n";

            for (int i = 0; i < unique_list.Count(); i++)
            {
                foreach (var item in unique_list[i])
                {
                    tb.Text += item;
                }
                tb.Text += " ";
            }
            #endregion


            #region CreateHelpingMatrix

            int[,] helpingMatrix = new int[n, n];
            for (int i = 1; i < n; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    int uniqEl = 0;
                    foreach (string item in arr1list[i])
                    {
                        if (arr1list[j].FindIndex(x => x == item) == -1)
                        {
                            uniqEl++;
                        }
                    }
                    foreach (string item in arr1list[j])
                    {
                        if (arr1list[i].FindIndex(x => x == item) == -1)
                        {
                            uniqEl++;
                        }
                    }
                    helpingMatrix[i, j] = unique_list.Count() - uniqEl;
                }
            }
            #endregion

            #region Out Helping Matrix
            tb.Text += "\n";
            tb.Text += "\nHelping Matrix:\n";
            PrintIntMatrix(helpingMatrix, n);

            #endregion

            #region Create First variant of groups

            List<int>[] groups = new List<int>[1];
            int iterator = 0;

            while (SumOfAllElemMatrix(helpingMatrix, n) != 0)
            {
                groups[iterator] = new List<int>();

                int max = 0;
                List<int> MaxI = new List<int>();
                List<int> MaxJ = new List<int>();
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (helpingMatrix[i, j] > max)
                        {
                            MaxI.Clear();
                            MaxJ.Clear();
                            max = helpingMatrix[i, j];
                            MaxI.Add(i);
                            MaxJ.Add(j);
                        }
                        if ((helpingMatrix[i, j] == max) &&
                            ((MaxI.FindIndex(x => x == i) != -1) ||
                                (MaxI.FindIndex(x => x == j) != -1) ||
                                (MaxJ.FindIndex(x => x == i) != -1) ||
                                (MaxJ.FindIndex(x => x == j) != -1)))
                        {
                            MaxI.Add(i);
                            MaxJ.Add(j);
                        }
                    }
                }

                for (int i = 1; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if ((MaxI.FindIndex(x => x == i) != -1) ||
                            (MaxJ.FindIndex(x => x == j) != -1))
                            helpingMatrix[i, j] = 0;

                foreach (int item in MaxI)
                {
                    if ((groups[iterator].FindIndex(x => x == item) == -1)
                        && !FindElement(groups, iterator, item))
                    {
                        groups[iterator].Add(item);
                    }
                }
                foreach (int item in MaxJ)
                {
                    if ((groups[iterator].FindIndex(x => x == item) == -1)
                        && !FindElement(groups, iterator, item))
                    {
                        groups[iterator].Add(item);
                    }
                }
                Array.Resize(ref groups, groups.Length + 1);
                iterator++;
            }

            #endregion

            #region OutGroups
            tb.Text += "\n";
            tb.Text += "\nGroups:\n";
            PrintList(groups);
            #endregion

            for (int q = 0; q < groups.Length - 1; q++)
            {
                #region CreateGroupList
                List<string>[] uniqueGroupList = new List<string>[1];

                for (int iter = 0; iter < groups.Length - 1; iter++)
                {
                    uniqueGroupList[iter] = new List<string>();
                    foreach (int item in groups[iter])
                    {

                        for (int i = 0; i < arr1list.Length; i++)
                        {
                            if (item == i)
                            {
                                foreach (string elem in arr1list[i])
                                {
                                    if ((uniqueGroupList[iter].FindIndex(x => x == elem) == -1)
                                        )
                                    {
                                        uniqueGroupList[iter].Add(elem);
                                    }
                                }
                            }
                        }

                    }
                    Array.Resize(ref uniqueGroupList, uniqueGroupList.Length + 1);
                }
                #endregion

                #region OutGroupList
                /* tb.Text += "\n UGroups:";
                 for (int i = 0; i < uniqueGroupList.Length - 1; i++)
                 {
                     tb.Text += "\n " + i + " - { ";
                     foreach (var item in uniqueGroupList[i])
                     {
                         tb.Text += (item) + " ";
                     }
                     tb.Text += "}";
                 }*/
                #endregion

                #region Sort groups

                int max_length = 0, max_index = 0, counter = 0;
                List<string> uniqGroup = new List<string>();
                List<int> id_repeat_group = new List<int>();

                for (int t = q; t < groups.Length - 1; t++)
                {
                    List<string> temp_list = new List<string>();

                    foreach (int item in groups[t])
                    {
                        foreach (string elem in arr1list[item])
                        {
                            if (temp_list.FindIndex(x => x == elem) == -1)
                            {
                                temp_list.Add(elem);
                            }
                        }
                    }
                    if (temp_list.Count() > max_length)
                    {
                        max_length = temp_list.Count();
                        max_index = t;
                        uniqGroup = temp_list;
                        counter = 0;
                        id_repeat_group.Clear();
                    }
                    if (temp_list.Count == max_length && max_length != 0 && counter != 0)
                    {
                        if (id_repeat_group.FindIndex(x => x == t) == -1)
                            id_repeat_group.Add(t);
                        counter++;
                    }

                }


                if (counter > 1)
                {
                    int id_max_group = 0;
                    int max_count_deteils = 0;

                    for (int t = 0; t < groups.Length - 1; t++)
                    {
                        int count_details = 0;
                        if (t >= 1)
                        {
                            List<int> id_temp = new List<int>();
                            foreach (int idArr1List in groups[t])
                            {
                                int count = 0;
                                foreach (string elem in arr1list[idArr1List])
                                {
                                    if (uniqGroup.FindIndex(x => x == elem) != -1)
                                    {
                                        count++;
                                    }
                                }
                                if (count == arr1list[idArr1List].Count())
                                {
                                    foreach (var item in groups)

                                        if (groups[q].FindIndex(x => x == idArr1List) == -1)
                                        {
                                            // id_temp.Add(idArr1List);
                                            count_details++;
                                        }
                                }
                            }


                        }
                        if (count_details > max_count_deteils)
                        {
                            max_count_deteils = count_details;
                            id_max_group = t;
                        }
                    }
                    List<int> temp = new List<int>();
                    temp = groups[id_max_group];
                    groups[id_max_group] = groups[max_index];
                    groups[max_index] = temp;
                }
                else
                {
                    List<int> temp = new List<int>();
                    temp = groups[q];
                    groups[q] = groups[max_index];
                    groups[max_index] = temp;
                }


                #endregion

                #region Out Sort Groups
                /*
                 tb.Text += "\n SortGroups:";
                 for (int i = 0; i < groups.Length - 1; i++)
                 {
                     tb.Text += "\n " + i + " - { ";
                     foreach (var item in groups[i])
                     {
                         tb.Text += (item + 1) + " ";
                     }
                     tb.Text += "}";
                 }*/
                #endregion

                #region Update Groups

                for (int t = 0; t < groups.Length - 1; t++)
                {
                    if (t >= 1)
                    {
                        List<int> id_temp = new List<int>();
                        foreach (int idArr1List in groups[t])
                        {
                            int count = 0;
                            foreach (string elem in arr1list[idArr1List])
                            {
                                if (uniqGroup.FindIndex(x => x == elem) != -1)
                                {
                                    count++;
                                }
                            }
                            if (count == arr1list[idArr1List].Count())
                            {
                                foreach (var item in groups)

                                    if (groups[q].FindIndex(x => x == idArr1List) == -1)
                                    {
                                        id_temp.Add(idArr1List);
                                        groups[q].Add(idArr1List);
                                    }
                            }
                        }
                        foreach (int id in id_temp)
                        {
                            groups[t].Remove(id);
                        }

                    }
                }

                #endregion

                #region Out Update Groups
                /* tb.Text += "\n Update Groups:";
                 for (int i = 0; i < groups.Length - 1; i++)
                 {
                     tb.Text += "\n " + i + " - { ";
                     foreach (var item in groups[i])
                     {
                         tb.Text += (item + 1) + " ";
                     }
                     tb.Text += "}";
                 }*/
                #endregion

            }
            #region Uniq New Group
            List<int>[] new_groups = new List<int>[1];
            int iteration = 0;
            for (int i = 0; i < groups.Length - 1; i++)
            {
                if (groups[i].Count() != 0)
                {
                    new_groups[iteration] = groups[i];
                    Array.Resize(ref new_groups, new_groups.Length + 1);
                    iteration++;
                }

            }
            #endregion

            #region Out Ytoch Groups
            tb.Text += "\n";
            tb.Text += "\nNew Uniq Groups:\n";
            PrintList(new_groups);

            #endregion

            #region Create List Uniq Operation and Graf Matrix

            for (int p = 0; p < new_groups.Length - 1; p++)
            {
                List<string>[] uniqueGroupList2 = new List<string>[1];

                for (int iter = 0; iter < groups.Length - 1; iter++)
                {
                    uniqueGroupList2[iter] = new List<string>();
                    foreach (int item in groups[iter])
                    {

                        for (int i = 0; i < arr1list.Length; i++)
                        {
                            if (item == i)
                            {
                                foreach (string elem in arr1list[i])
                                {
                                    if ((uniqueGroupList2[iter].FindIndex(x => x == elem) == -1)
                                        )
                                    {
                                        uniqueGroupList2[iter].Add(elem);
                                    }
                                }
                            }
                        }

                    }
                    Array.Resize(ref uniqueGroupList2, uniqueGroupList2.Length + 1);
                }
                if (p == new_groups.Length - 2)
                {
                    #region Out operation of uniq group
                    /*tb.Text += "\n UGroups:";
                    for (int i = 0; i < uniqueGroupList2.Length - 1; i++)
                    {
                        tb.Text += "\n " + i + " - { ";
                        foreach (var item in uniqueGroupList2[i])
                        {
                            tb.Text += (item) + " ";
                        }
                        tb.Text += "}";
                    }
                    tb.Text += "\n";*/
                    #endregion

                    #region Creating Graf Matrix

                    for (int tem = 0; tem < uniqueGroupList2.Length - 1; tem++)
                    {

                        int size = uniqueGroupList2[tem].Count() + 1;
                        string[,] graf_matrix = new string[size, size];
                        int m = 1;
                        foreach (string uniq_elem in uniqueGroupList2[tem])
                        {
                            graf_matrix[0, m] = uniq_elem;
                            graf_matrix[m, 0] = uniq_elem;
                            m++;
                        }

                        foreach (int index in new_groups[tem])
                        {
                            string[] arr1Array = arr1list[index].ToArray();
                            /* Out Uniq operations in groups
                             for (int l = 0; l < arr1Array.Length; l++)
                            {
                                tb.Text += arr1Array[l];
                            }
                            tb.Text += "\n";*/

                            for (int l = 0; l < arr1Array.Length - 1; l++)
                            {
                                for (m = 1; m < size; m++)
                                {
                                    if (arr1Array[l] == graf_matrix[m, 0])
                                    {
                                        int locationI = m;
                                        for (int m1 = 1; m1 < size; m1++)
                                        {
                                            if (arr1Array[l + 1] == graf_matrix[0, m1])
                                            {
                                                int locationJ = m1;
                                                graf_matrix[locationI, locationJ] = "1";
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < size; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                if (graf_matrix[i, j] != "1" && i != 0 && j != 0)
                                    graf_matrix[i, j] = "0";
                            }
                        }

                        int[,] graf_matrix_int = new int[size - 1, size - 1];
                        for (int i = 0; i < size - 1; i++)
                        {
                            for (int j = 0; j < size - 1; j++)
                            {
                                graf_matrix_int[i, j] = Int16.Parse(graf_matrix[i + 1, j + 1]);
                            }
                        }
                        #endregion

                        #region Out graf matrix
                        tb.Text += "\n";
                        tb.Text += "\nGraf " + (tem + 1) + "\n";
                        PrintStringMatrix(graf_matrix, size);
                        // PrintIntMatrix(graf_matrix_int,size-1);
                        #endregion
                    }

                }
            }
            #endregion            
        }

        public int SumOfAllElemMatrix(int[,] array, int size)
        {
            int sum = 0;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    sum += array[i, j];
            return sum;
        }
        public bool FindElement(List<int>[] array, int size, int findValue)
        {

            for (int i = 0; i < size; i++)
                if (array[i].FindIndex(x => x == findValue) != -1)
                    return true;

            return false;
        }

        public int PrintList(List<int>[] array)
        {

            for (int i = 0; i < array.Length - 1; i++)
            {
                tb.Text += "\n " + i + " - { ";
                foreach (var item in array[i])
                {
                    tb.Text += (item + 1) + " ";
                }
                tb.Text += "}";

            }
            return 0;
        }
        public int PrintStringMatrix(string[,] ArrayMatrix, int sizeMatrix)
        {

            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {

                    tb.Text += ArrayMatrix[i, j] + "\t";
                }
                tb.Text += "\n";
            }
            return 0;
        }
        public int PrintIntMatrix(int[,] ArrayMatrix, int sizeMatrix)
        {

            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    tb.Text += ArrayMatrix[i, j].ToString() + "\t";
                }
                tb.Text += "\n";
            }
            return 0;
        }

    }
}