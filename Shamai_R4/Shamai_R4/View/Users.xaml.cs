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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Shamai_R4.Class;

namespace Shamai_R4.View
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : MetroWindow
    {
        public Users()
        {
            InitializeComponent();
            _addresource();
            ListUserInDataBase();
        }
        /// <summary>
        /// load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Users_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
        /// <summary>
        /// add ResourceDictionary
        /// </summary>
        internal void _addresource()
        {
            try
            {
                List<TemplateLang> lang = Clanguage.Langs;
                ResourceDictionary d = new ResourceDictionary();
                foreach (TemplateLang i in lang)
                {
                    if (i.Lang.ToUpper() == Clanguage.GetLangInClient())
                    {
                        d.Source = new Uri(i.FileLink, UriKind.Absolute);
                        Resources.MergedDictionaries.Add(d);
                    }
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);    
            }
           
        }

        /// <summary>
        /// get list all user(s) database
        /// </summary>
        internal void ListUserInDataBase()
        {
            ListUser.Items.Clear();
            List<TemplateMyUsersDataBase> t = MainWindow.SqliteSource.GetListMyUsersDataBase();

            foreach (TemplateMyUsersDataBase i in t)
            {
                ListUser.Items.Add(i.User);
            }
        }

        private void CmdAddUser_OnClick(object sender, RoutedEventArgs e)
        {
            // check exist user if update
            TemplateMyUsersDataBase u = MainWindow.SqliteSource.GetUserEdit(TxtUser.Text);

            // existe usuario update
            if (TxtUser.Text != string.Empty && TxtPw.Password != string.Empty &&
                MainWindow.SqliteSource.CheckUserExist(TxtUser.Text))
            {
                if (TxtUser.Text != u.User || TxtPw.Password != u.Password || TxtOwner.Text != u.Owner)
                {
                    // update algum valor alterado
                    MainWindow.SqliteSource.UpdateUserIn(TxtUser.Text,TxtPw.Password,TxtOwner.Text,u.User);
                }
            }

            // cria novo usuario
            if (TxtUser.Text != string.Empty && TxtPw.Password != string.Empty && !MainWindow.SqliteSource.CheckUserExist(TxtUser.Text))
            {
                MainWindow.SqliteSource.CreatUser(TxtUser.Text,TxtPw.Password,TxtOwner.Text);
                this.ShowMessageAsync(@"Sucess", @"User created");

            }

            // refresh list user
            ListUserInDataBase();

            // clear txt
            TxtUser.Text = string.Empty;
            TxtPw.Password = string.Empty;
            TxtOwner.Text = string.Empty;

            TxtUser.Focus();

        }

        private void CmdRemoveUser_OnClick(object sender, RoutedEventArgs e)
        {
            TemplateMyUsersDataBase u = MainWindow.SqliteSource.GetUserEdit(TxtUser.Text);
            if(u == null) { return;}
            // check password 
            if(TxtPw.Password != u.Password || TxtUser.Text != u.User || TxtOwner.Text != u.Owner && MainWindow.SqliteSource.CheckUserExist(TxtUser.Text)) { return;}

            MainWindow.SqliteSource.DeleteUser(TxtUser.Text);
            // refresh list user
            ListUserInDataBase();

            // clear txt
            TxtUser.Text = string.Empty;
            TxtPw.Password = string.Empty;
            TxtOwner.Text = string.Empty;

            TxtUser.Focus();
            this.ShowMessageAsync(@"Sucess", @"User deleted");
        }

        private void MnuItemClearAll_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.SqliteSource.DeleteAllUsers();
            // refresh list user
            ListUserInDataBase();

            // clear txt
            TxtUser.Text = string.Empty;
            TxtPw.Password = string.Empty;
            TxtOwner.Text = string.Empty;

            TxtUser.Focus();
            this.ShowMessageAsync(@"Sucess", @"deleted all");
        }

        private void ListUser_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListUser.SelectedItem != null)
            {
                TemplateMyUsersDataBase user = MainWindow.SqliteSource.GetUserEdit(ListUser.SelectedItem.ToString());
                // load 
                TxtUser.Text = user.User;
                TxtPw.Password = user.Password;
                TxtOwner.Text = user.Owner;
            }

        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
