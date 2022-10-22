using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.IO;


namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            TableFun();
            Export();

            //ChartFun();

        }
        
        public class Users
        {
            public string User { get; set; }
            public string Status { get; set; }
            public int Rank { get; set; }
            public string Steps { get; set; }

            public Users(string user, string status, int rank, string steps)
            {
                User = user;
                Status = status;
                Rank = rank;
                Steps = steps;
            }
        }
        public static List<Users> personsList = new List<Users>();
        public static List<UserRes> userRes = new List<UserRes>();
        List<UserRes> clonedList = new List<UserRes>();
        public class UserRes{
            public string name;
            public double step;
            public string maxStep;
            public string minStep;
            public string status;
            public int rank;

            public UserRes(string n, double a, string max, string min, string stat, int r) { name = n; step = a; maxStep = max; minStep = min; status = stat; rank = r; }
            
        }
        int i;
        int N = 31;

        List<string> mass = new List<string>();
        List<int> mass2 = new List<int>();
        
        public void TableFun()
        {
            for (i = 1; i < N; i++)
            {
                var json = System.IO.File.ReadAllText($@"day{i}.json");
                var tbl = JArray.Parse(json)
                                    .Select(j => new
                                    {
                                        User = j["User"],
                                        Status = j["Status"],
                                        Rank = j["Rank"],
                                        Steps = j["Steps"]

                                    });
               
                foreach (var v in tbl)
                {
                    if (i == 1)
                    {
                        personsList.Add(new Users(v.User.ToString(), v.Status.ToString(), int.Parse(v.Rank.ToString()), v.Steps.ToString() + " "));
                        userRes.Add(new UserRes(v.User.ToString(), double.Parse(v.Steps.ToString()), v.Steps.ToString()+" ", v.Steps.ToString()+" ", v.Status.ToString(), int.Parse(v.Rank.ToString())));
                    }
                    else
                    {
                        foreach (var s in userRes)
                        {
                            if (s.name.ToString() == v.User.ToString())
                            {
                                //s.maxStep += v.Steps.ToString() + " ";
                                s.minStep += v.Steps.ToString() + " ";
                                s.step += double.Parse(v.Steps.ToString());
                            }
                        }
                        foreach (var s in personsList)
                        {
                            if (s.User.ToString() == v.User.ToString())
                            {
                                //s.maxStep += v.Steps.ToString() + " ";
                                s.Steps += v.Steps.ToString() + " ";
                                //s.step += double.Parse(v.Steps.ToString());
                            }
                        }
                    }
                }
            }
            //clonedList = userRes.GetRange(0, userRes.Count);
            //clonedList = new List<UserRes>(userRes);
            foreach (var s in userRes)
            {
                s.step = Math.Round(s.step / N,1);
               
                mass = s.minStep.Split(' ').ToList();
                mass.RemoveAll(c => c == "");
                mass2 = mass.ConvertAll(int.Parse);

                s.minStep = mass2.Min().ToString();
                s.maxStep = mass2.Max().ToString();

            }


            var dt = new DataTable();
            List<string> headers = new List<string> { "name", "steps", "max steps", "min steps" };
            // create columns and headers
            int columnCount = headers.Count;
            for (int i = 0; i < columnCount; i++)
                dt.Columns.Add(headers[i]);

            // copy rows data
            for (int i = 0; i < userRes.Count; i++)
            {
                dt.Rows.Add(userRes[i].name, userRes[i].step, userRes[i].maxStep, userRes[i].minStep);
                //if (int.Parse(userRes[i].maxStep)> (userRes[i].step*20)/100 || int.Parse(userRes[i].maxStep)< (userRes[i].step * 20) / 100)
                 //   dt.Rows[i].GetTyp
            }

            // display in a DataGrid
            PersonsGrid.LoadingRow += PersonsGrid_LoadingRow;
            PersonsGrid.ItemsSource = dt.DefaultView;
            //PersonsGrid.ItemsSource = personsList;
        }
        public void ChartFun()
        {
            double[] dataX = new double[] { 1, 2, 3, 4, 5 };
            double[] dataY = new double[] { 1, 4, 9, 16, 25 };
            Chart.Plot.AddScatter(dataX, dataY);
            Chart.Refresh();
        }

        private void PersonsGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
        List<double> dataY = new List<double>();
        private void PersonsGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DataRowView row = (DataRowView)PersonsGrid.SelectedItems[0];
            var str = row["name"];
            Console.WriteLine(str);
            foreach (var s in personsList)
            {
                if (s.User == str)
                {
                    mass = s.Steps.Split(' ').ToList();
                    mass.RemoveAll(c => c == "");
                    dataY = mass.ConvertAll(double.Parse);
                    double[] dataX = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 ,12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
                    Chart.Plot.AddScatter(dataX, dataY.ToArray());
                    Chart.Refresh();
                    Chart.Plot.Clear();
                }
            }
        }

        private void PersonsGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                var row = e.Row;
                var index = row.GetIndex();
                var person = row.DataContext as UserRes;

                double res1 = Math.Abs((userRes[index].step - int.Parse(userRes[index].maxStep)) / ((userRes[index].step + int.Parse(userRes[index].maxStep)) / 2)) * 100;
                double res2 = Math.Abs((userRes[index].step - int.Parse(userRes[index].minStep)) / ((userRes[index].step + int.Parse(userRes[index].minStep)) / 2)) * 100;
                if (res1 > 20 && res2 > 20)
                {
                    row.Background = new SolidColorBrush(Colors.BurlyWood);
                }
                else if (res1 > 20 | res2 > 20)
                {
                    row.Background = new SolidColorBrush(Colors.Chocolate);
                    if (res1 > 20)
                        row.Background = new SolidColorBrush(Colors.Chocolate);
                    else
                        row.Background = new SolidColorBrush(Colors.Brown);
                }
            }
            catch { }
        }

       public static void Export()
       {
            File.WriteAllText("month.json", JsonConvert.SerializeObject(userRes,Formatting.Indented));
        }
    }
}
