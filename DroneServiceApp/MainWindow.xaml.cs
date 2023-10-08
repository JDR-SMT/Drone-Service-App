using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private void DisplayRegular()
        {
            ListViewRegular.Items.Clear(); // clear all items

            foreach (var drone in RegularService) // add item variables to associated columns
            {
                ListViewRegular.Items.Add(new
                {
                    RegularTag = drone.getTag(),
                    RegularName = drone.GetName(),
                    RegularDrone = drone.GetDrone(),
                    RegularProblem = drone.GetProblem(),
                    RegularCost = drone.GetCost()
                });
            }
        }

        private void DisplayExpress()
        {
            ListViewExpress.Items.Clear(); // clear all items

            foreach (var drone in ExpressService) // add item variables to associated columns
            {
                ListViewExpress.Items.Add(new
                {
                    ExpressTag = drone.getTag(),
                    ExpressName = drone.GetName(),
                    ExpressDrone = drone.GetDrone(),
                    ExpressProblem = drone.GetProblem(),
                    ExpressCost = drone.GetCost()
                });
            }
        }

        private void DisplayFinished()
        {
            ListBoxFinished.Items.Clear(); // clear all items

            foreach (var drone in FinishedList) // display item name and cost
            {
                ListBoxFinished.Items.Add(drone.Display());
            }
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0) // item is selected
            {
                if (ListViewRegular == sender) // display regular item
                {
                    ListViewExpress.SelectedItems.Clear();
                    ListBoxFinished.SelectedItem = null;
                    ListViewRegular_SelectionChanged(sender, e);
                }

                if (ListViewExpress == sender) // display express item
                {
                    ListViewRegular.SelectedItems.Clear();
                    ListBoxFinished.SelectedItem = null;
                    ListViewExpress_SelectionChanged(sender, e);
                }

                if (ListBoxFinished == sender) // display finished item
                {
                    ListViewRegular.SelectedItems.Clear();
                    ListViewExpress.SelectedItems.Clear();
                    ListBoxFinished_SelectionChanged(sender, e);
                }
            }
        }
        
        private void ListViewRegular_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try // output selected item variables in boxes
            {
                IntegerUpDownTag.Value = RegularService.ElementAt(ListViewRegular.SelectedIndex).getTag();
                SetServicePriority();
                TextBoxName.Text = RegularService.ElementAt(ListViewRegular.SelectedIndex).GetName();
                TextBoxDrone.Text = RegularService.ElementAt(ListViewRegular.SelectedIndex).GetDrone();
                TextBoxProblem.Text = RegularService.ElementAt(ListViewRegular.SelectedIndex).GetProblem();
                TextBoxCost.Text = RegularService.ElementAt(ListViewRegular.SelectedIndex).GetCost().ToString();
            }
            catch (ArgumentOutOfRangeException) // selected item is changed or deleted
            {
                Clear();
                return;
            }
        }
        private void ListViewExpress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try // output selected item variables in boxes
            {
                IntegerUpDownTag.Value = ExpressService.ElementAt(ListViewExpress.SelectedIndex).getTag();
                SetServicePriority();
                TextBoxName.Text = ExpressService.ElementAt(ListViewExpress.SelectedIndex).GetName();
                TextBoxDrone.Text = ExpressService.ElementAt(ListViewExpress.SelectedIndex).GetDrone();
                TextBoxProblem.Text = ExpressService.ElementAt(ListViewExpress.SelectedIndex).GetProblem();
                TextBoxCost.Text = ExpressService.ElementAt(ListViewExpress.SelectedIndex).GetCost().ToString();
            }
            catch (ArgumentOutOfRangeException) // selected item is changed or deleted
            {
                Clear();
                return;
            }
        }
        
        private void ListBoxFinished_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try // output selected item variables in display boxes
            {
                TextBoxName.Text = FinishedList[ListBoxFinished.SelectedIndex].GetName();
                TextBoxDrone.Text = FinishedList[ListBoxFinished.SelectedIndex].GetDrone();
                TextBoxProblem.Text = FinishedList[ListBoxFinished.SelectedIndex].GetProblem();
                TextBoxCost.Text = FinishedList[ListBoxFinished.SelectedIndex].GetCost().ToString();
            }
            catch (ArgumentOutOfRangeException) // selected item is changed or deleted
            {
                Clear();
                return;
            }
        }

        private void Clear() // clear all display boxes
        {
            IntegerUpDownTag.Value = IntegerUpDownTag.Minimum;
            RadioButtonRegular.IsChecked = false;
            RadioButtonExpress.IsChecked = false;
            TextBoxName.Clear();
            TextBoxDrone.Clear();
            TextBoxProblem.Clear();
            TextBoxCost.Clear();
        }
        #endregion

        #region Service Getter & Setter
        private string GetServicePriority() // return value from selected radio button
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

            return selected;
        }

        private void SetServicePriority() // check radio button depending on selected ListView
        {
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
            if (ValidTag() && ValidCost() // valid tag and cost, all inputs completed
                && (RadioButtonRegular.IsChecked == true || RadioButtonExpress.IsChecked == true)
                && !string.IsNullOrWhiteSpace(TextBoxName.Text)
                && !string.IsNullOrWhiteSpace(TextBoxDrone.Text)
                && !string.IsNullOrWhiteSpace(TextBoxProblem.Text)
                && !string.IsNullOrWhiteSpace(TextBoxCost.Text))
            {
                AddNewItem();
                Clear();
            }
            else if (!ValidTag()) // invalid tag
            {
                StatusBarInfo.Text = "Tag number is already taken. Please choose another number.";
            }
            else if (!ValidCost()) // invalid cost
            {
                StatusBarInfo.Text = "Please enter a price of two digits and two decimals.";
            }
            else // empty input
            {
                StatusBarInfo.Text = "Please complete all fields.";
            }
        }

        private void AddNewItem()
        {
            Drone addDrone = new Drone(); // create new Drone object and set variables
            addDrone.SetTag((int)IntegerUpDownTag.Value);
            addDrone.SetName(TextBoxName.Text);
            addDrone.SetDrone(TextBoxDrone.Text);
            addDrone.SetProblem(TextBoxProblem.Text);
            decimal.TryParse(TextBoxCost.Text, out decimal resultCost); // return decimal cost

            switch (GetServicePriority()) // check priority
            {
                case "Regular":
                    addDrone.SetCost(resultCost);
                    RegularService.Enqueue(addDrone); // add object to RegularService queue
                    DisplayRegular();
                    break;
                case "Express":
                    addDrone.SetCost(decimal.Round(resultCost *= 1.15M, 2)); // increase cost by 15%
                    ExpressService.Enqueue(addDrone); // add object to ExpressService queue
                    DisplayExpress();
                    break;
            }
        }

        private bool ValidTag()
        {
            foreach (var regularDrone in RegularService) // search RegularService
            {
                if (regularDrone.getTag() == (int)IntegerUpDownTag.Value) // tag exists
                {
                    return false;
                }
            }

            foreach (var expressDrone in ExpressService) // search ExpressService
            {
                if (expressDrone.getTag() == (int)IntegerUpDownTag.Value) // tag exists
                {
                    return false;
                }
            }

            return true; // tag does not exist
        }

        private bool ValidCost() // 00.00
        {
            if (Regex.IsMatch(TextBoxCost.Text, "^\\d\\d\\.\\d\\d$")) // matches
            {
                return true;
            }
            else // does not match
            {
                return false;
            }
        }
        #endregion

        #region Remove
        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ListViewRegular.SelectedIndex == 0 || ListViewExpress.SelectedIndex == 0) // top item selected
            {
                MessageBoxResult result = MessageBox.Show("Mark this service as finished?", "", MessageBoxButton.YesNo); // prompt

                if (result == MessageBoxResult.Yes && ListViewRegular.SelectedIndex == 0) // yes on regular
                {
                    FinishedList.Add(RegularService.Dequeue()); // add dequeued object to FinishedList
                    DisplayRegular();
                }
                else if (result == MessageBoxResult.Yes && ListViewExpress.SelectedIndex == 0) // yes on express
                {
                    FinishedList.Add(ExpressService.Dequeue()); // add dequeued object to FinishedList
                    DisplayExpress();
                }

                DisplayFinished();
                Clear();
            }
            else if (ListViewRegular.SelectedIndex >= 1 || ListViewExpress.SelectedIndex >= 1) // top item not selected
            {
                StatusBarInfo.Text = "Services must be completed in order.";
                Clear();
                return;
            }
            else // no item selected
            {
                StatusBarInfo.Text = "Please select a service to mark as finished.";
                Clear();
                return;
            }
        }

        private void ListBoxFinished_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxFinished.SelectedIndex >= 0) // item selected
            {
                MessageBoxResult result = MessageBox.Show("Mark this service as paid?", "", MessageBoxButton.YesNo); // prompt

                if (result == MessageBoxResult.Yes) // yes
                {
                    FinishedList.RemoveAt(ListBoxFinished.SelectedIndex); // remove item from FinishedList
                    Clear();
                    DisplayFinished();
                }
            }
            else // no item selected
            {
                StatusBarInfo.Text = "Please select a service to mark as paid.";
                Clear();
                return;
            }
        }
        #endregion
    }
}