using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Xml;
using System.ServiceModel.Syndication;
using System.IO;
using System.Collections.ObjectModel;

namespace DevelopFeed
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ObservableCollection<SyndicationItem> newsItems;
        private ObservableCollection<SyndicationItem> featuresItems;
        private ObservableCollection<SyndicationItem> blogItems;

        private const String FAILED_CONNECTION_STRING = "Unable To Connect To Network.";

        public MainPage()
        {
            InitializeComponent();

            if (NavigationContext == null || NavigationContext.QueryString == null || NavigationContext.QueryString.Count == 0)
            {
                (App.Current as App).rateReminder.Notify();
            }

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No Network Connection Available! This App Requires an active network Connection!");
            }

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        void client_newsDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                newsFeedListBox.EmptyContent = FAILED_CONNECTION_STRING;
            }
            else
            {
                this.State["newsFeed"] = e.Result;
                UpdateNewsFeedList(e.Result);
            }
        }

        void client_featuresDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                featuresFeedListBox.EmptyContent = FAILED_CONNECTION_STRING;
            }
            else
            {
                this.State["featuresFeed"] = e.Result;
                UpdateFeaturesFeedList(e.Result);
            }
        }

        void client_blogDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                blogFeedListBox.EmptyContent = FAILED_CONNECTION_STRING;
            }
            else
            {
                this.State["blogFeed"] = e.Result;
                UpdateBlogFeedList(e.Result);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (this.State.ContainsKey("newsFeed"))
            {
                if (newsFeedListBox.ItemCount == 0)
                {
                    UpdateNewsFeedList(State["newsFeed"] as string);
                }
            }
            if (this.State.ContainsKey("featuresFeed"))
            {
                if (newsFeedListBox.ItemCount == 0)
                {
                    UpdateFeaturesFeedList(State["featuresFeed"] as string);
                }
            }
            if (this.State.ContainsKey("blogFeed"))
            {
                if (newsFeedListBox.ItemCount == 0)
                {
                    UpdateBlogFeedList(State["blogFeed"] as string);
                }
            }
        }

        private void UpdateNewsFeedList(string feedXML)
        {
            StringReader stringReader = new StringReader(feedXML);
            XmlReader xmlReader = XmlReader.Create(stringReader);

            newsItems.Clear();
            
            try
            {
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

                foreach (SyndicationItem item in feed.Items)
                {
                    newsItems.Insert(0, item);
                }
            }
            catch (Exception) { newsFeedListBox.EmptyContent = FAILED_CONNECTION_STRING; }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                newsFeedListBox.StopPullToRefreshLoading(true);
            });
        }

        private void UpdateFeaturesFeedList(string feedXML)
        {
            StringReader stringReader = new StringReader(feedXML);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            
            featuresItems.Clear();
            try
            {
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

                foreach (SyndicationItem item in feed.Items)
                {
                    featuresItems.Insert(0, item);
                }
            }
            catch (Exception) { featuresFeedListBox.EmptyContent = FAILED_CONNECTION_STRING; }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                featuresFeedListBox.StopPullToRefreshLoading(true);
            });
        }

        private void UpdateBlogFeedList(string feedXML)
        {
            StringReader stringReader = new StringReader(feedXML);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            
            blogItems.Clear();
            try
            {
                SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

                foreach (SyndicationItem item in feed.Items)
                {
                    blogItems.Insert(0, item);
                }
            }
            catch (Exception) { blogFeedListBox.EmptyContent = FAILED_CONNECTION_STRING; }

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                blogFeedListBox.StopPullToRefreshLoading(true);
            });
        }

        private void FeedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Telerik.Windows.Controls.RadDataBoundListBox box = sender as Telerik.Windows.Controls.RadDataBoundListBox;
            if (box != null && box.SelectedItem != null)
            {
                SyndicationItem sItem = (SyndicationItem)box.SelectedItem;

                if (sItem.Links.Count > 0)
                {
                    Uri uri = sItem.Links.FirstOrDefault().Uri;

                    WebBrowserTask task = new WebBrowserTask();
                    task.Uri = uri;
                    task.Show();
                }
            }
        }

        private void newsPivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            newsItems = new ObservableCollection<SyndicationItem>();
            newsFeedListBox.ItemsSource = newsItems;
            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_newsDownloadStringCompleted;
            client.DownloadStringAsync(new System.Uri("http://feeds.feedburner.com/developmag/ifbh"));
        }

        private void featuresPivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            featuresItems = new ObservableCollection<SyndicationItem>();
            featuresFeedListBox.ItemsSource = featuresItems;
            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_featuresDownloadStringCompleted;
            client.DownloadStringAsync(new System.Uri("http://feeds.feedburner.com/develop/features"));
        }

        private void blogPivotItem_Loaded(object sender, RoutedEventArgs e)
        {
            blogItems = new ObservableCollection<SyndicationItem>();
            blogFeedListBox.ItemsSource = blogItems;
            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_blogDownloadStringCompleted;
            client.DownloadStringAsync(new System.Uri("http://feeds.feedburner.com/develop-online/game-development-news"));
        }

        private void newsFeedListBox_RefreshRequested(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_newsDownloadStringCompleted;
            client.DownloadStringAsync(new System.Uri("http://feeds.feedburner.com/developmag/ifbh"));
        }

        private void featuresFeedListBox_RefreshRequested(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_featuresDownloadStringCompleted;
            client.DownloadStringAsync(new System.Uri("http://feeds.feedburner.com/develop/features"));
        }

        private void blogFeedListBox_RefreshRequested(object sender, EventArgs e)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += client_blogDownloadStringCompleted;
            client.DownloadStringAsync(new System.Uri("http://feeds.feedburner.com/develop-online/game-development-news"));
        }
    }
}