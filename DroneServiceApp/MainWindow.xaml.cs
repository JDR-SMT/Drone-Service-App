using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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
        DispatcherTimer timer = new DispatcherTimer();

        #region Utility
        private void DisplayService(Queue<Drone> Queue, ListView ListView)
        {
            // clear all items
            ListView.Items.Clear();

            // add item variables to associated columns
            foreach (var drone in Queue)
            {
                ListView.Items.Add(new
                {
                    Tag = drone.getTag(),
                    Name = drone.GetName(),
                    Drone = drone.GetDrone(),
                    Problem = drone.GetProblem(),
                    Cost = drone.GetCost()
                });
            }
        }

        private void DisplayFinished()
        {
            // clear all items
            ListBoxFinished.Items.Clear();

            // display item name and cost
            foreach (var drone in FinishedList)
            {
                ListBoxFinished.Items.Add(drone.ToString());
            }
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // item is selected
            if (e.AddedItems.Count != 0)
            {
                // display regular item
                if (ListViewRegular == sender)
                {
                    ClearSelection(ListViewExpress);
                    ClearSelection(ListBoxFinished);
                    ListView_SelectionChanged(RegularService, ListViewRegular);
                }

                // display express item
                if (ListViewExpress == sender)
                {
                    ClearSelection(ListViewRegular);
                    ClearSelection(ListBoxFinished);
                    ListView_SelectionChanged(ExpressService, ListViewExpress);
                }

                // display finished item
                if (ListBoxFinished == sender)
                {
                    ClearSelection(ListViewRegular);
                    ClearSelection(ListViewExpress);
                    ListBoxFinished_SelectionChanged();
                }
            }
        }

        private void ListView_SelectionChanged(Queue<Drone> Queue, ListView ListView)
        {
            // output selected item variables in boxes
            try
            {
                IntegerUpDownTag.Value = Queue.ElementAt(ListView.SelectedIndex).getTag();
                SetServicePriority();
                TextBoxName.Text = Queue.ElementAt(ListView.SelectedIndex).GetName();
                TextBoxDrone.Text = Queue.ElementAt(ListView.SelectedIndex).GetDrone();
                TextBoxProblem.Text = Queue.ElementAt(ListView.SelectedIndex).GetProblem();
                TextBoxCost.Text = Queue.ElementAt(ListView.SelectedIndex).GetCost().ToString();
            }
            // selected item is changed or deleted
            catch (ArgumentOutOfRangeException)
            {
                ClearDisplay();
                return;
            }
        }

        private void ListBoxFinished_SelectionChanged()
        {
            // output selected item variables in display boxes
            try
            {
                IntegerUpDownTag.Value = null;
                TextBoxName.Text = FinishedList[ListBoxFinished.SelectedIndex].GetName();
                TextBoxDrone.Text = FinishedList[ListBoxFinished.SelectedIndex].GetDrone();
                TextBoxProblem.Text = FinishedList[ListBoxFinished.SelectedIndex].GetProblem();
                TextBoxCost.Text = FinishedList[ListBoxFinished.SelectedIndex].GetCost().ToString();
            }
            // selected item is changed or deleted
            catch (ArgumentOutOfRangeException)
            {
                ClearDisplay();
                return;
            }
        }

        // clear all display boxes
        private void ClearDisplay()
        {
            IntegerUpDownTag.Value = IntegerUpDownTag.Minimum;
            RadioButtonRegular.IsChecked = false;
            RadioButtonExpress.IsChecked = false;
            TextBoxName.Clear();
            TextBoxDrone.Clear();
            TextBoxProblem.Clear();
            TextBoxCost.Clear();
        }

        // clear listview
        private void ClearSelection(ListView ListView)
        {
            ListView.SelectedItems.Clear();
        }

        // clear listbox
        private void ClearSelection(ListBox ListBox)
        {
            ListBoxFinished.SelectedItem = null;
        }

        // sets statusbar and starts timer (10s)
        private void SetStatusBarInfo(string text)
        {
            StatusBarInfo.Text = text;
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }

        // clears statusbar and stops timer
        private void TimerTick(object sender, EventArgs e)
        {
            StatusBarInfo.Text = "";
            timer.Stop();
        }
        #endregion

        #region Service Getter & Setter
        private string GetServicePriority()
        {
            string selected = "";

            if (RadioButtonRegular.IsChecked == true)
            {
                selected = (string)RadioButtonRegular.Content;
            }
            else if (RadioButtonExpress.IsChecked == true)
            {
                selected = (string)RadioButtonExpress.Content;
            }

            // string from selected radio button
            return selected;
        }


        private void SetServicePriority()
        {
            // check radio button depending on selected ListView
            if (ListViewRegular.SelectedIndex >= 0)
            {
                RadioButtonRegular.IsChecked = true;
            }
            else if (ListViewExpress.SelectedIndex >= 0)
            {
                RadioButtonExpress.IsChecked = true;
            }
        }
        #endregion

        #region Add
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            // valid tag and cost, all inputs completed
            if (ValidTag() && ValidCost()
                && (RadioButtonRegular.IsChecked == true || RadioButtonExpress.IsChecked == true)
                && !string.IsNullOrWhiteSpace(TextBoxName.Text)
                && !string.IsNullOrWhiteSpace(TextBoxDrone.Text)
                && !string.IsNullOrWhiteSpace(TextBoxProblem.Text)
                && !string.IsNullOrWhiteSpace(TextBoxCost.Text))
            {
                AddNewItem();
                ClearSelection(ListViewRegular);
                ClearSelection(ListViewExpress);
                ClearSelection(ListBoxFinished);
                ClearDisplay();
            }
            // invalid tag
            else if (!ValidTag())
            {
                SetStatusBarInfo("Tag number is already taken. Please choose another number.");
            }
            // invalid cost
            else if (!ValidCost())
            {
                SetStatusBarInfo("Please enter a price of two digits and two decimals.");
            }
            // empty input
            else
            {
                SetStatusBarInfo("Please complete all fields.");
            }
        }

        private void AddNewItem()
        {
            // create new Drone object and set variables
            Drone addDrone = new Drone();
            addDrone.SetTag((int)IntegerUpDownTag.Value);
            addDrone.SetName(TextBoxName.Text);
            addDrone.SetDrone(TextBoxDrone.Text);
            addDrone.SetProblem(TextBoxProblem.Text);
            decimal.TryParse(TextBoxCost.Text, out decimal resultCost);

            // check priority
            switch (GetServicePriority())
            {
                case "Regular":
                    // set cost and add object to RegularService queue
                    addDrone.SetCost(resultCost);
                    RegularService.Enqueue(addDrone);
                    DisplayService(RegularService, ListViewRegular);
                    break;
                case "Express":
                    // increase cost by 15% and add object to ExpressService queue
                    addDrone.SetCost(decimal.Round(resultCost *= 1.15M, 2));
                    ExpressService.Enqueue(addDrone);
                    DisplayService(ExpressService, ListViewExpress);
                    break;
            }
        }

        private bool ValidTag()
        {
            // search RegularService
            foreach (var regularDrone in RegularService)
            {
                // tag exists
                if (regularDrone.getTag() == (int)IntegerUpDownTag.Value)
                {
                    return false;
                }
            }

            // search ExpressService
            foreach (var expressDrone in ExpressService)
            {
                // tag exists
                if (expressDrone.getTag() == (int)IntegerUpDownTag.Value)
                {
                    return false;
                }
            }

            return true; // tag does not exist
        }

        // 00.00
        private bool ValidCost()
        {
            // matches
            if (Regex.IsMatch(TextBoxCost.Text, "^\\d\\d\\.\\d\\d$"))
            {
                return true;
            }
            // does not match
            else
            {
                return false;
            }
        }
        #endregion

        #region Remove
        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            // prompt
            MessageBoxResult result = MessageBox.Show("Mark this service as finished?", "", MessageBoxButton.YesNo);

            // top item selected on regular
            if (result == MessageBoxResult.Yes && ListViewRegular.SelectedIndex == 0)
            {
                ProcessRemove(RegularService, ListViewRegular);
            }
            // top item selected on express
            else if (result == MessageBoxResult.Yes && ListViewExpress.SelectedIndex == 0)
            {
                ProcessRemove(ExpressService, ListViewExpress);
            }
            // top item not selected
            else if (ListViewRegular.SelectedIndex >= 1 || ListViewExpress.SelectedIndex >= 1)
            {
                SetStatusBarInfo("Services must be completed in order.");
                ClearSelection(ListViewRegular);
                ClearSelection(ListViewExpress);
                ClearDisplay();
                return;
            }
            // no item selected
            else
            {
                SetStatusBarInfo("Please select a service to mark as finished.");
                ClearDisplay();
                return;
            }
        }

        private void ProcessRemove(Queue<Drone> Queue, ListView ListView)
        {
            // add dequeued object to FinishedList
            FinishedList.Add(Queue.Dequeue());
            DisplayService(Queue, ListView);
            DisplayFinished();
            ClearDisplay();
        }

        private void ListBoxFinished_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // item selected
            if (ListBoxFinished.SelectedIndex >= 0)
            {
                // prompt
                MessageBoxResult result = MessageBox.Show("Mark this service as paid?", "", MessageBoxButton.YesNo);

                // yes
                if (result == MessageBoxResult.Yes)
                {
                    // remove item from FinishedList
                    FinishedList.RemoveAt(ListBoxFinished.SelectedIndex);
                    DisplayFinished();
                    ClearDisplay();
                }
            }
            // no item selected
            else
            {
                SetStatusBarInfo("Please select a service to mark as paid.");
                ClearDisplay();
                return;
            }
        }
        #endregion
    }
}