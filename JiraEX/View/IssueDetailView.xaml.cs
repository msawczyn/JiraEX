﻿using JiraEX.ViewModel;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
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

namespace JiraEX.View
{
    /// <summary>
    /// Interaction logic for IssueDetailView.xaml
    /// </summary>
    public partial class IssueDetailView : UserControl
    {

        private IssueDetailViewModel _viewModel;

        public IssueDetailView(JiraToolWindowNavigatorViewModel parent, Issue issue, Project project,
            IIssueService issueService, IPriorityService priorityService, ITransitionService transitionService,
            IAttachmentService attachmentService, IUserService userService, IBoardService boardService, IProjectService projectService)
        {
            InitializeComponent();

            this._viewModel = new IssueDetailViewModel(parent, issue, project,
                issueService, priorityService, transitionService,
                attachmentService, userService, boardService, projectService);
            this.DataContext = this._viewModel;
        }

        void AttachmentSelected_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = sender as ListBoxItem;
            Attachment attachment = listBoxItem.Content as Attachment;

            this._viewModel.DownloadAttachment(attachment);
        }

        void cbAssignee_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchString = cbAssignee.Text;

            if (cbAssignee.SelectedItem != null && !searchString.Equals(((User)cbAssignee.SelectedItem).DisplayName))
            {
                cbAssignee.IsDropDownOpen = true;
                this._viewModel.SearchAssignee(searchString);
            }
        }

        void cbLabels_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchString = cbLabels.Text;

            cbLabels.IsDropDownOpen = true;
            this._viewModel.SearchLabels(searchString);
        }

        void cbLinkedIssue_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchString = cbLinkedIssue.Text;

            string pattern = "-(\\d+)";

            if (System.Text.RegularExpressions.Regex.IsMatch(searchString, pattern) &&
                cbLinkedIssue.SelectedItem != null && 
                !searchString.Equals(((Issue)cbLinkedIssue.SelectedItem).Key))
            {
                cbLinkedIssue.IsDropDownOpen = true;
                this._viewModel.SearchLinkedIssues(searchString);
            }
            else if (searchString.Equals(""))
            {
                this._viewModel.RefreshIssuesAsync();
            }
        }
    }
}
