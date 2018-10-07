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
using System.Windows.Shapes;
using QuickGraph;

namespace GKS_kursov
{
    /// <summary>
    /// Логика взаимодействия для GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        private IBidirectionalGraph<object, IEdge<object>> _graphToVisualize;
        public IBidirectionalGraph<object, IEdge<object>> GraphToVisualize
        {
            get { return _graphToVisualize; }
        }

        public GraphWindow()
        {
            CreateGraphToVisualize();
            InitializeComponent();
        }
        private void CreateGraphToVisualize()
        {

            var g = new BidirectionalGraph<object, IEdge<object>>();
            // add the verties to the graph
            string[] verties = new string[5];
            for (int i = 0; i < 5; i++)
            {
                verties[i] = i.ToString();
                g.AddVertex(verties[i]);
            }

            //add some edges to the graph
            g.AddEdge(new Edge<object>(verties[0], verties[1]));
            g.AddEdge(new Edge<object>(verties[1], verties[2]));
            g.AddEdge(new Edge<object>(verties[2], verties[3]));
            g.AddEdge(new Edge<object>(verties[3], verties[1]));
            g.AddEdge(new Edge<object>(verties[1], verties[4]));

            _graphToVisualize = g;
        }
    }
}
