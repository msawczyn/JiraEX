﻿using AtlassianConnector.Model.Exceptions;
using JiraEX.ViewModel.Navigation;
using JiraRESTClient.Model;
using JiraRESTClient.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraEX.ViewModel
{
    public class FilterListViewModel : ViewModelBase, ITitleable, IReinitializable
    {

        private IJiraToolWindowNavigatorViewModel _parent;

        private IIssueService _issueService;

        private bool _noFilters = false;

        private ObservableCollection<Filter> _filterList;

        public FilterListViewModel(IJiraToolWindowNavigatorViewModel parent, IIssueService issueService)
        {
            this._parent = parent;
            this._parent.SetRefreshCommand(RefreshFilters);

            this._issueService = issueService;

            this._filterList = new ObservableCollection<Filter>();

            GetFiltersAsync();

            SetPanelTitles();
        }

        private void RefreshFilters(object sender, EventArgs e)
        {
            GetFiltersAsync();
        }

        private async void GetFiltersAsync()
        {
            this._parent.StartLoading();

            this.FilterList.Clear();

            Task<FilterList> filterTask = this._issueService.GetAllFiltersAsync();

            try { 
                var filterList = await filterTask as FilterList;

                if (filterList.Count > 0)
                {
                    foreach (Filter f in filterList)
                    {
                        this.FilterList.Add(f);
                    }

                    this.NoFilters = false;
                }
                else
                {
                    this.NoFilters = true;
                }

                this._parent.StopLoading();

                HideErrorMessages(this._parent);
            }
            catch (JiraException ex)
            {
                ShowErrorMessages(ex, this._parent);

                this._parent.StopLoading();
            }
        }

        public void OnItemSelected(object sender)
        {
            Filter filter = sender as Filter;

            this._parent.ShowIssuesOfFilter(filter);
        }

        public void SetPanelTitles()
        {
            this._parent.SetPanelTitles("JiraEX", "Favourite filters");
        }

        public void Reinitialize()
        {
            this._parent.SetRefreshCommand(RefreshFilters);

            GetFiltersAsync();
        }

        public ObservableCollection<Filter> FilterList
        {
            get { return this._filterList; }
            set
            {
                this._filterList = value;
                OnPropertyChanged("FilterList");
            }
        }

        public bool NoFilters
        {
            get { return this._noFilters; }
            set
            {
                this._noFilters = value;
                OnPropertyChanged("NoFilters");
            }
        }
    }
}
