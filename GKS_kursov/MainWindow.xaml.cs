using System;
using System.IO;
using Microsoft.Win32;
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
using QuickGraph;
using GraphSharp.Controls;
using GraphX.Controls;

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
            int n = 0;
            tb.Text = "";

            string rData = new TextRange(readData.Document.ContentStart,
                                           readData.Document.ContentEnd).Text;
            List<string> listOperation = rData.Split(new[] { Environment.NewLine },
                                    StringSplitOptions.RemoveEmptyEntries).ToList();

            n = listOperation.Count();

            List<string>[] arr1list = new List<string>[n];
            for (int i = 0; i < arr1list.Length-1; i++)
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

                int maximum = 0;
                List<int> MaxI = new List<int>();
                List<int> MaxJ = new List<int>();

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (helpingMatrix[i, j] > maximum)
                        {
                            MaxI.Clear();
                            MaxJ.Clear();
                            maximum = helpingMatrix[i, j];
                            MaxI.Add(i);
                            MaxJ.Add(j);
                        }
                        if ((helpingMatrix[i, j] == maximum) &&
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
            while (groups[iterator] == null || groups[iterator].Count() == 0)
            {
                Array.Resize(ref groups, groups.Length-1);
                iterator--;
            }
            #endregion

            #region OutGroups
            tb.Text += "\n";
            tb.Text += "\nGroups:\n";
            PrintList(groups);
            #endregion

            for (int q = 0; q < groups.Length ; q++)
            {

                #region Sort groups

                int max_length = 0, max_index = 0, counter = 0;
                List<string> uniqGroup = new List<string>();
                List<int> id_repeat_group = new List<int>();

                for (int t = q; t < groups.Length -1; t++)
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

                    for (int t = 0; t < groups.Length-1; t++)
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
                 for (int i = 0; i < groups.Length; i++)
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

                for (int t = 0; t < groups.Length-1; t++)
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
                 for (int i = 0; i < groups.Length; i++)
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
            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i].Count() != 0)
                {
                    new_groups[iteration] = groups[i];
                    Array.Resize(ref new_groups, new_groups.Length + 1);
                    iteration++;
                }

            }
            Array.Resize(ref new_groups, new_groups.Length - 1);
            #endregion

            #region Out Ytoch Groups
            tb.Text += "\n";
            tb.Text += "\nNew Uniq Groups:\n";
            PrintList(new_groups);

            #endregion

            #region Create Unique Operations List

            List<string>[] uniqueGroupList = new List<string>[1];

            for (int p = 0; p < new_groups.Length; p++)
            {
                for (int iter = 0; iter < new_groups.Length; iter++)
                {
                    uniqueGroupList[iter] = new List<string>();

                    foreach (int item in new_groups[iter])
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
            }

            #endregion

            #region Creating Graf Matrix

            List<int[,]> listOfAdjacencyMatrix = new List<int[,]>();

            for (int tem = 0; tem < new_groups.Length; tem++)
            {

                int size = uniqueGroupList[tem].Count() + 1;
                string[,] graf_matrix = new string[size, size];
                int m = 1;
                foreach (string uniq_elem in uniqueGroupList[tem])
                {
                    graf_matrix[0, m] = uniq_elem;
                    graf_matrix[m, 0] = uniq_elem;
                    m++;
                }

                foreach (int index in new_groups[tem])
                {
                    string[] arr1Array = arr1list[index].ToArray();

                    for (int l = 0; l < arr1Array.Length-1; l++)
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


                int[,] adjacencyMatrix = new int[size - 1, size - 1];
                for (int i = 0; i < size - 1; i++)
                {
                    for (int j = 0; j < size - 1; j++)
                    {
                        adjacencyMatrix[i, j] = Int16.Parse(graf_matrix[i + 1, j + 1]);
                    }
                }
                listOfAdjacencyMatrix.Add(adjacencyMatrix);
                #endregion

                #region Out graf matrix
                tb.Text += "\n";
                tb.Text += "\nGraf " + (tem + 1) + "\n";
                PrintStringMatrix(graf_matrix, size);
                // PrintIntMatrix(graf_matrix_int,size-1);
            }
            #endregion

            #region Create Grafs

            tabControl.Items.Clear();

            for (int i = 0; i < new_groups.Length; i++)
            {
                var g = new BidirectionalGraph<object, IEdge<object>>();
                var g2 = new BidirectionalGraph<object, IEdge<object>>();

                List<string> uniqueOpertionForGroup = FindUniqueOperationInGroup(new_groups[i], arr1list);
                int cntUniqueOperations = uniqueOpertionForGroup.Count();

                foreach (var operations in uniqueOpertionForGroup)
                {
                    g.AddVertex(operations);
                }

                foreach (var idItem in new_groups[i])
                {
                    string[] operations = arr1list[idItem].ToArray();
                    int countOperations = operations.Length;
                    for (int j = 1; j < countOperations; j++)
                    {
                        g.AddEdge(new Edge<object>(operations[j - 1], operations[j]));
                    }
                }

                GraphLayout gl = new GraphLayout();

                gl.LayoutAlgorithmType = "KK"; //"FR"
                gl.OverlapRemovalAlgorithmType = "FSA";
                // gl.HighlightAlgorithmType = "Simple";
                gl.Graph = g;

                //ZoomControl zc = new ZoomControl();
                //zc.Content = gl;

                TabItem ti = new TabItem();
                ti.Header = "Group" + (i + 1);
                //ti.Content = zc;
                ti.Content = gl;

                tabControl.Items.Add(ti);
            }

            #endregion

            #region Create Models

            List<List<string>>[] arrayOfGroupModels = new List<List<string>>[new_groups.Length];

            for (int i = 0; i < new_groups.Length; i++)
            {
                List<string> uniqueOpeationInGroup = FindUniqueOperationInGroup(new_groups[i], arr1list);
                List<List<string>> listOfModels = new List<List<string>>();
                for (int j = 0; j < uniqueOpeationInGroup.Count; j++)
                {
                    List<string> model = new List<string>();
                    model.Add(uniqueOpeationInGroup[j]);
                    listOfModels.Add(model);
                }
                arrayOfGroupModels[i] = listOfModels;
            }

            PrintArrayOfModels(arrayOfGroupModels);
            #endregion

            #region Marging Models        
                        
            //PrintModel(arrayOfGroupModels);

            #region circuit check

            bool checkCircuit = false;

            for (int i = 0; i < new_groups.Length; i++)
            {
                int[,] currentAndjancencyMatrix = CreateAdjacencyMatrixToModel(arrayOfGroupModels[i], listOfAdjacencyMatrix[i], FindUniqueOperationInGroup(new_groups[i], arr1list));

                for (int r = 0; r < currentAndjancencyMatrix.GetLength(0) && !checkCircuit; r++)
                    for (int c = 0; c < currentAndjancencyMatrix.GetLength(0) && !checkCircuit; c++)
                    {
                        if (currentAndjancencyMatrix[r, c] == 1)
                        {
                            List<int> usedIndex = new List<int>();
                            usedIndex.Add(c);
                            checkCircuit = CheckCircuit(ref arrayOfGroupModels[i], currentAndjancencyMatrix, usedIndex, r);

                            if (checkCircuit)
                            {
                                if ((arrayOfGroupModels[i][r].Count + arrayOfGroupModels[i][c].Count) <= 5)
                                    MergeModel(ref arrayOfGroupModels[i], r, c);
                                arrayOfGroupModels[i].RemoveAll(x => x.Count == 0);
                            }

                        }
                    }

                if (checkCircuit)
                {
                    checkCircuit = false;
                    i = -1;
                }
            }
            #endregion
          
            //   PrintModel(arrayOfGroupModels);
            #region check on closed cirle

            bool circleCheck = false;

            for (int i = 0; i < new_groups.Length; i++)
            {
                int[,] currentAndjancencyMatrix = CreateAdjacencyMatrixToModel(arrayOfGroupModels[i], listOfAdjacencyMatrix[i], FindUniqueOperationInGroup(new_groups[i], arr1list));

                for (int r = 0; r < currentAndjancencyMatrix.GetLength(0) && !circleCheck; r++)
                    for (int c = 0; c < currentAndjancencyMatrix.GetLength(0) && !circleCheck; c++)
                    {
                        if (currentAndjancencyMatrix[r, c] == 1)
                        {
                            List<int> usedIndex = new List<int>();
                            usedIndex.Add(c);
                            circleCheck = CheckCirlce(ref arrayOfGroupModels[i], currentAndjancencyMatrix, usedIndex, r);

                            if (circleCheck)
                            {
                                if ((arrayOfGroupModels[i][r].Count + arrayOfGroupModels[i][c].Count) <= 5)
                                    MergeModel(ref arrayOfGroupModels[i], r, c);
                                arrayOfGroupModels[i].RemoveAll(x => x.Count == 0);
                            }

                        }
                    }

                if (circleCheck)
                {
                    circleCheck = false;
                    i = -1;
                }
            }

            #endregion

            tb.Text += "\nMerge Models:\n";
            PrintArrayOfModels(arrayOfGroupModels);

            #endregion

            #region Model Clarifying

            List<List<string>> listOfAllModel = new List<List<string>>();
            for (int i = 0; i < arrayOfGroupModels.Length; i++)
                foreach (var listOfModel in arrayOfGroupModels[i])
                    listOfAllModel.Add(listOfModel);

            #region sortList
            int max = 0;
            for (int i = 0; i < listOfAllModel.Count - 1; i++)
            {
                max = i;
                for (int j = i + 1; j < listOfAllModel.Count; j++)
                    if (listOfAllModel[j].Count > listOfAllModel[max].Count)
                        max = j;

                if (max != i)
                {
                    var tmp = listOfAllModel[i];
                    listOfAllModel[i] = listOfAllModel[max];
                    listOfAllModel[max] = tmp;
                }
            }
            #endregion

            tb.Text += "\nAll Models:\n";
            PrintModel(listOfAllModel);

            List<List<string>> listOfClarifyingModel = new List<List<string>>();
            for (int i = 0; i < listOfAllModel.Count; i++)
                if (!FindInListOfAllModel(listOfAllModel[i], listOfClarifyingModel))
                    listOfClarifyingModel.Add(listOfAllModel[i]);


            tb.Text += "\nClarifying Models 1 part:\n";
            PrintModel(listOfClarifyingModel);

            for (int i = 0; i < listOfClarifyingModel.Count; i++)
            {
                for (int j = 0; j < listOfClarifyingModel[i].Count; j++)
                {
                    int index = 0;
                    if ((index = FindOperationInListOfModel(listOfClarifyingModel[i][j], listOfClarifyingModel, i)) != -1)
                    {
                        if (listOfClarifyingModel[i].Count > listOfClarifyingModel[index].Count)
                            listOfClarifyingModel[i].Remove(listOfClarifyingModel[i][j]);
                        else
                            listOfClarifyingModel[index].Remove(listOfClarifyingModel[i][j]);

                        i = -1;
                        break;
                    }
                }
            }

            tb.Text += "\nClarifying Models 2 part:\n";
            PrintModel(listOfClarifyingModel);

            #endregion

        }

        private void ButtonCreateGraf_Click(object sender, RoutedEventArgs e)
        {
            // CreateGraphToVisualize();     
            /*  GraphWindow graph = new GraphWindow();
              graph.Show();*/
        }

        public int SumOfAllElemMatrix(int[,] array, int size)
        {
            int sum = 0;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    sum += array[i, j];
            return sum;
        }

        public int SumOfAllElementsInRow(int[,] ajdencecyMatrix, int row)
        {
            int sum = 0;
            for (int i = 0; i < ajdencecyMatrix.GetLength(1); i++)
                sum += ajdencecyMatrix[row, i];
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
            for (int i = 0; i < array.Length; i++)
            {
                tb.Text += "\n " + (i + 1) + " - { ";
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

        public List<string> FindUniqueOperationInGroup(List<int> detailsInGroup, List<string>[] details)
        {
            List<string> uniqueOperationList = new List<string>();
            foreach (var id in detailsInGroup)
                foreach (var operation in details[id])
                    if (uniqueOperationList.FindIndex(x => x == operation) == -1)
                        uniqueOperationList.Add(operation);
            return uniqueOperationList;
        }


        public void PrintArrayOfModels(List<List<string>>[] arrayOfGroupModels)
        {
            for (int i = 0; i < arrayOfGroupModels.Length; i++)
            {
                tb.Text += "\nModels of " + (i + 1) + " group\n";
                PrintModel(arrayOfGroupModels[i]);
            }
        }

        public void PrintModel(List<List<string>> listOfModel)
        {
            for (int p = 0; p < listOfModel.Count; p++)
            {
                tb.Text += "Model " + (p + 1) + ": { ";
                foreach (var operation in listOfModel[p])
                    tb.Text += operation + " ";
                tb.Text += "}\n";
            }
        }

        public int[,] CreateAdjacencyMatrixToModel(List<List<string>> listOfModels, int[,] adjacencyMatrix, List<string> uniqueOperationInGroup)
        {
            int[,] adjacencyMatrixToModel = new int[listOfModels.Count, listOfModels.Count];
            //выбор модели
            for (int i = 0; i < listOfModels.Count; i++)
                //выбор операции
                for (int j = 0; j < listOfModels[i].Count; j++)
                {
                    int indexInAdjacencyMatrix = uniqueOperationInGroup.FindIndex(x => x == listOfModels[i][j]);
                    for (int iam = 0; iam < adjacencyMatrix.GetLength(0); iam++)
                    {
                        var currentUniqueValue = uniqueOperationInGroup[iam];
                        if (adjacencyMatrix[indexInAdjacencyMatrix, iam] == 1
                            && listOfModels[i].FindIndex(x => x == currentUniqueValue) == -1)
                        {
                            int indexModel = -1;
                            foreach (var model in listOfModels)
                                if (model.FindIndex(x => x == currentUniqueValue) != -1)
                                    indexModel = listOfModels.FindIndex(x => x == model);
                            adjacencyMatrixToModel[i, indexModel] = 1;
                        }
                    }

                }
            return adjacencyMatrixToModel;
        }

        public void MergeModel(ref List<List<string>> listOfModel, int indexToWrite, int indexToRemove)
        {
            while (listOfModel[indexToRemove].Count > 0)
            {
                listOfModel[indexToWrite].Add(listOfModel[indexToRemove].First());
                listOfModel[indexToRemove].Remove(listOfModel[indexToRemove].First());
            }
        }

        public bool CheckCircuit(ref List<List<string>> listOfModel, int[,] currentAndjancencyMatrix, List<int> usedIndexList, int mainIndex)
        {
            int indexOfParent = usedIndexList.Last();
            if (SumOfAllElementsInRow(currentAndjancencyMatrix, indexOfParent) > 1) return false;

            for (int i = 0; i < currentAndjancencyMatrix.GetLength(0); i++)
            {
                if (currentAndjancencyMatrix[indexOfParent, i] == 1)
                {
                    if (currentAndjancencyMatrix[mainIndex, i] == 1)
                    {
                        if ((listOfModel[indexOfParent].Count + listOfModel[i].Count) <= 5)
                        {
                            MergeModel(ref listOfModel, indexOfParent, i);
                            return true;
                        }
                    }
                    else if (usedIndexList.FindIndex(x => x == i) == -1)
                    {
                        usedIndexList.Add(i);
                        if (CheckCircuit(ref listOfModel, currentAndjancencyMatrix, usedIndexList, mainIndex))
                        {
                            if ((listOfModel[indexOfParent].Count + listOfModel[i].Count) <= 5)
                            {
                                MergeModel(ref listOfModel, indexOfParent, i);
                                return true;
                            }
                        }
                    }
                }

            }
            return false;
        }

        public bool CheckCirlce(ref List<List<string>> listOfModel, int[,] currentAndjancencyMatrix, List<int> usedIndexList, int mainIndex)
        {

            int indexOfParent = usedIndexList.Last();

            for (int i = 0; i < currentAndjancencyMatrix.GetLength(0); i++)
            {
                if (currentAndjancencyMatrix[indexOfParent, i] == 1)
                {
                    if (currentAndjancencyMatrix[i, mainIndex] == 1)
                    {
                        if ((listOfModel[indexOfParent].Count + listOfModel[i].Count) <= 5)
                        {
                            MergeModel(ref listOfModel, indexOfParent, i);
                            return true;
                        }
                    }
                    else if (usedIndexList.FindIndex(x => x == i) == -1)
                    {
                        usedIndexList.Add(i);
                        if (CheckCirlce(ref listOfModel, currentAndjancencyMatrix, usedIndexList, mainIndex))
                        {
                            if ((listOfModel[indexOfParent].Count + listOfModel[i].Count) <= 5)
                            {
                                MergeModel(ref listOfModel, indexOfParent, i);
                                return true;
                            }
                        }
                    }
                }

            }
            return false;
        }

        public bool FindInListOfAllModel(List<string> currentModel, List<List<string>> listOfAllModels)
        {
            foreach (var model in listOfAllModels)
            {
                int cntSameOperation = 0;
                foreach (var operation in currentModel)
                    if (model.FindIndex(x => x == operation) != -1)
                        cntSameOperation++;
                if (cntSameOperation == currentModel.Count)
                    return true;
            }
            return false;
        }

        public int FindOperationInListOfModel(string value, List<List<string>> listOfModels, int blockIndex)
        {
            for (int i = 0; i < listOfModels.Count; i++)
                if (listOfModels[i].FindIndex(x => x == value) != -1 && blockIndex != i)
                    return i;
            return -1;
        }

    }
}