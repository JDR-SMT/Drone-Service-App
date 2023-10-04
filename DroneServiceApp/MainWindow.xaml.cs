using System.Collections.Generic;
using System.Windows;

namespace DroneServiceApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // global variables
        Queue<Drone> RegularService = new Queue<Drone>();
        Queue<Drone> ExpressService = new Queue<Drone>();
        List<Drone> FinishedList = new List<Drone>();

        #region Utility

        #endregion

        #region Service Getter & Setter

        #endregion

        #region Add

        #endregion

        #region Remove

        #endregion
    }
}
