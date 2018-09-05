﻿using System;
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

namespace JiraEX.View
{
    /// <summary>
    /// Interaction logic for IssueListView.xaml
    /// </summary>
    public partial class IssueListView : UserControl
    {

        private IssueListViewModel _viewModel;

        public IssueListView(JiraToolWindowNavigator parent)
        {
            InitializeComponent();

            this._viewModel = new IssueListViewModel(parent);
            this.DataContext = this._viewModel;
        }
    }
}