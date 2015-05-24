using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SKYPE4COMLib;
using System.Drawing;
using System.Windows.Forms;

namespace Skype_Status_Helper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Skype _skype;
        private string _currentStatus;
        private TUserStatus _desiredStatus;
        private NotifyIcon notificationIcon;

        private readonly Dictionary<TUserStatus, string> _statuses;
        public Dictionary<TUserStatus, string> Statuses
        {
            get { return _statuses; }
        }

        public MainWindow()
        {
            _statuses = new Dictionary<TUserStatus, string>()
            {
                {TUserStatus.cusOnline, "Online"},
                {TUserStatus.cusAway, "Away"},
                {TUserStatus.cusDoNotDisturb, "Do not disturb"},
                {TUserStatus.cusInvisible, "Invisible"},
            };

            _skype = new Skype();
            _skype.Attach(8, true);

            _skype.UserStatus += skype_UserStatusChanges;
            _desiredStatus = _skype.CurrentUserStatus;
            setCurrentStatus(_desiredStatus);

            Console.WriteLine("current status: {0}", CurrentStatus);

            // initialize window
            DataContext = this;
            InitializeComponent();

            notificationIcon = new NotifyIcon();
            notificationIcon.Icon = Properties.Resources.Icon;
            notificationIcon.Visible = true;
            notificationIcon.DoubleClick += notificationIcon_DoubleClick;

            MenuItem quitMenuItem = new MenuItem("Quit");
            ContextMenu notifyIconMenu = new ContextMenu();
            notifyIconMenu.MenuItems.Add(quitMenuItem);
            notificationIcon.ContextMenu = notifyIconMenu;

            quitMenuItem.Click += quitMenuItem_Click;

        }

        void quitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            notificationIcon.Dispose();
        }

        void notificationIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void setCurrentStatus(TUserStatus status)
        {
            if (_statuses.ContainsKey(status))
            {
                CurrentStatus = _statuses[status];
            }
            else
            {
                CurrentStatus = "Me not know this status :S";
            }
        }

        void skype_UserStatusChanges(TUserStatus Status)
        {
            setCurrentStatus(Status);
            Console.WriteLine("Status chanhed, current status: {0}", CurrentStatus);
            if (Status != _desiredStatus)
            {
                Console.WriteLine("Current status is different from desired status! Lets change it");
                _skype.CurrentUserStatus = _desiredStatus;
            }
        }

        public string CurrentStatus
        {
            get { return _currentStatus; }
            set
            {
                if (value != _currentStatus)
                {
                    _currentStatus = value;
                    if (CurrentStatusTextblock != null)
                    {
                        CurrentStatusTextblock.Text = value;
                    }
                }
            }
        }

        public TUserStatus DesiredStatus
        {
            get { return _desiredStatus; }
            set { _desiredStatus = value; _skype.CurrentUserStatus = value; }
        }
    }
}
