﻿using ConfluenceEX.Command;
using ConfluenceEX.Helper;
using DevDefined.OAuth.Framework;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using JiraRESTClient.Service.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class IssueDetailViewModel : ViewModelBase
    {
        private bool _isEditingSummary = false;
        private bool _isEditingDescription = false;
        private bool _isEditingPriority = false;

        private JiraToolWindowNavigatorViewModel _parent;

        private IIssueService _issueService;
        private IPriorityService _priorityService;
        private ITransitionService _transitionService;

        private Issue _issue;

        private Priority _selectedPriority;
        private Transition _selectedTransition;

        private ObservableCollection<Priority> _priorityList;
        private ObservableCollection<Transition> _transitionList;

        public DelegateCommand EditSummaryCommand { get; private set; }
        public DelegateCommand ConfirmEditSummaryCommand { get; private set; }
        public DelegateCommand CancelEditSummaryCommand { get; private set; }

        public DelegateCommand EditDescriptionCommand { get; private set; }
        public DelegateCommand ConfirmEditDescriptionCommand { get; private set; }
        public DelegateCommand CancelEditDescriptionCommand { get; private set; }

        public DelegateCommand EditPriorityCommand { get; private set; }
        public DelegateCommand CancelEditPriorityCommand { get; private set; }

        public IssueDetailViewModel(JiraToolWindowNavigatorViewModel parent, Issue issue)
        {
            this._parent = parent;

            this._issueService = new IssueService();
            this._priorityService = new PriorityService();

            this._issue = issue;
            this._priorityList = new ObservableCollection<Priority>();
            this._transitionList = new ObservableCollection<Transition>();

            GetPrioritiesAsync();
            GetTransitionsAsync();

            this.EditSummaryCommand = new DelegateCommand(EnableEditSummary);
            this.ConfirmEditSummaryCommand = new DelegateCommand(ConfirmEditSummary);
            this.CancelEditSummaryCommand = new DelegateCommand(CancelEditSummary);

            this.EditDescriptionCommand = new DelegateCommand(EnableEditDescription);
            this.ConfirmEditDescriptionCommand = new DelegateCommand(ConfirmEditDescription);
            this.CancelEditDescriptionCommand = new DelegateCommand(CancelEditDescription);

            this.EditPriorityCommand = new DelegateCommand(EnableEditPriority);
            this.CancelEditPriorityCommand = new DelegateCommand(CancelEditPriority);
        }

        private async void GetPrioritiesAsync()
        {
            System.Threading.Tasks.Task<PriorityList> priorityTask = this._priorityService.GetAllPrioritiesAsync();

            var priorityList = await priorityTask as PriorityList;

            this.PriorityList.Clear();

            foreach (Priority p in priorityList)
            {
                this.PriorityList.Add(p);

                if(p.Id == this._issue.Fields.Priority.Id)
                {
                    this.SelectedPriority = p;
                }
            }
        }

        private async void GetTransitionsAsync()
        {
            System.Threading.Tasks.Task<TransitionList> transitionTask = this._transitionService.GetAllTransitionsForIssueByIssueKey(this._issue.Key);

            var transitionList = await transitionTask as TransitionList;

            this.TransitionList.Clear();

            foreach (Transition t in transitionList.Transitions)
            {
                this.TransitionList.Add(t);

                if (t.Name == this._issue.Fields.Status.Name)
                {
                    this.SelectedTransition = t;
                }
            }
        }

        private async void UpdatePriorityAsync()
        {
            await this._issueService.UpdateIssuePropertyAsync(this._issue.Key, "priority", this.SelectedPriority);

            UpdateIssueAsync();
        }

        private async void UpdateIssueAsync()
        {
            this.Issue = await  this._issueService.GetIssueByIssueKeyAsync(this._issue.Key);
        }

        private async void DoTransitionAsync()
        {
            await this._transitionService.DoTransitionAsync(this._issue.Key, this.SelectedTransition);

            UpdateIssueAsync();
        }

        private void EnableEditSummary(object parameter)
        {
            this.IsEditingSummary = true;
        }

        private async void ConfirmEditSummary(object parameter)
        {
            await this._issueService.UpdateIssuePropertyAsync(this._issue.Key, "summary", this._issue.Fields.Summary);

            UpdateIssueAsync();

            this.IsEditingSummary = false;
        }

        private void CancelEditSummary(object parameter)
        {
            this.IsEditingSummary = false;
        }

        private void EnableEditDescription(object parameter)
        {
            this.IsEditingDescription = true;
        }

        private async void ConfirmEditDescription(object parameter)
        {
            await this._issueService.UpdateIssuePropertyAsync(this._issue.Key, "description", this._issue.Fields.Issuetype.Description);

            UpdateIssueAsync();

            this.IsEditingDescription = false;
        }

        private void CancelEditDescription(object parameter)
        {
            this.IsEditingDescription = false;
        }

        private void EnableEditPriority(object parameter)
        {
            this.IsEditingPriority = true;
        }

        private void CancelEditPriority(object parameter)
        {
            this.IsEditingPriority = false;
        }

        public Issue Issue
        {
            get { return this._issue; }
            set
            {
                this._issue = value;
                OnPropertyChanged("Issue");
            }
        }

        public bool IsEditingSummary
        {
            get { return this._isEditingSummary; }
            set
            {
                this._isEditingSummary = value;
                OnPropertyChanged("IsEditingSummary");
            }
        }

        public bool IsEditingDescription
        {
            get { return this._isEditingDescription; }
            set
            {
                this._isEditingDescription = value;
                OnPropertyChanged("IsEditingDescription");
            }
        }

        public bool IsEditingPriority
        {
            get { return this._isEditingPriority; }
            set
            {
                this._isEditingPriority = value;
                OnPropertyChanged("IsEditingPriority");
            }
        }

        public ObservableCollection<Priority> PriorityList
        {
            get { return this._priorityList; }
            set
            {
                this._priorityList = value;
                OnPropertyChanged("PriorityList");
            }
        }

        public Priority SelectedPriority
        {
            get { return this._selectedPriority; }
            set
            {
                this._selectedPriority = value;
                OnPropertyChanged("SelectedPriority");
                this.UpdatePriorityAsync();
                this.IsEditingPriority = false;
            }
        }

        public ObservableCollection<Transition> TransitionList
        {
            get { return this._transitionList; }
            set
            {
                this._transitionList = value;
                OnPropertyChanged("TransitionList");
            }
        }

        public Transition SelectedTransition
        {
            get { return this._selectedTransition; }
            set
            {
                this._selectedTransition = value;
                OnPropertyChanged("SelectedTransition");
                this.DoTransitionAsync();
                this.IsEditingTransition = false;
            }
        }
    }
}
