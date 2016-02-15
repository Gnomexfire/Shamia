using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Shamai_R4.Class;
using Shamai_R4.View;
using Color = System.Windows.Media.Brushes;


namespace Shamai_R4
{
   
    public partial class MainWindow : MetroWindow
    {
        #region declare
        public static Configuration Configuration = new Configuration();
        public static SqliteSource SqliteSource = new SqliteSource();
        public static UiFontConf UiFont = new UiFontConf();
        public static string VersionCore => @"Shamia 2.0.T [PUBLIC]"; // T = versao teste 
        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // hide console window
            Showconsole.ShowConsole(false);

            LblversionCore.Content = VersionCore;

            CanvasAllShowHide(false);
            // load configuration in database (SQLite) \\Shamia.db
            SqliteSource.CreateOrAccessDataBase();
            string lang = SqliteSource.GetLang();

            if (lang == string.Empty)
            {
                // load default language
                Clanguage.LoadDefaultLang();
            }
            else
            {
                // load language set in database
                Clanguage.SetLanguage(lang);
            }

            // load user(s) TOP 1 database
            List<TemplateUserDataBase> user = SqliteSource.EnumUserDataBase();
            foreach (TemplateUserDataBase i in user)
            {
                Configuration.StConfiguration.Password = i.Password; // user password
                Configuration.StConfiguration.Nick = i.Nick; // user login
                Configuration.StConfiguration.AuthSsl = i.Auth; // flag auth user (true or false)
                Configuration.StConfiguration.Owner = i.Owner ; // default nick chat is Shamia
                Configuration.StConfiguration.Port = i.Port;  // default port used quakenet (6667)
                Configuration.StConfiguration.Channel = SqliteSource.GetLastChannelUsedDataBase(); // last channel connected saved in database
            }
            Configuration.StConfiguration.Server = SqliteSource.GetLastServerUsedDataBase(); // get last server in database used to connection

            // list all game(s) SQLite
            Game.EnumGamesIn();

            // call method create menu
            CreatTopMenu();


            CanvasWelcome.Visibility = Visibility.Visible;
            CanvasContent.Visibility = Visibility.Hidden;
            TxtSend.Visibility = Visibility.Collapsed;
            CanvasProgressring.Visibility = Visibility.Collapsed;
            RingP.IsActive = false;

            // mensagem de boas vindas
            Effectfade(LblChan, Efeitos.Surgir,Resources.MergedDictionaries[0]["Status"].ToString(), 5, Color.DodgerBlue);
        }

        private void TxtSend_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    // check command 
                    if (TxtSend.Text != string.Empty && TxtSend.Text.Trim() != string.Empty && TxtSend.Text.StartsWith("/"))
                    {
                        SendToIrc(TxtSend.Text,true);
                    }
                    // simples mensagem
                    else
                    {
                        SendToIrc(TxtSend.Text);
                    }
                    TxtSend.Text = string.Empty;
                    break;                     
            }
        }
        #region internal methods
        /// <summary>
        /// method show or hide all canvas object
        /// </summary>
        /// <param name="show">parameter default true if hide CanvasAllShowHide(false)</param>
        internal void CanvasAllShowHide(bool show = true)
        {
            if (show)
            {
                CanvasWelcome.Visibility = Visibility.Visible;
                CanvasContent.Visibility = Visibility.Visible;
                TxtSend.Visibility = Visibility.Visible;
                CanvasProgressring.Visibility = Visibility.Collapsed;
                RingP.IsActive = false;
                return;
            }
            CanvasWelcome.Visibility = Visibility.Collapsed;
            CanvasContent.Visibility = Visibility.Collapsed;
            TxtSend.Visibility = Visibility.Collapsed;
            CanvasProgressring.Visibility = Visibility.Visible;
            RingP.IsActive = true;
        }

        public MenuItem MenuLanguage { get; set; }
        public MenuItem MenuConnect { get; set; }
        public MenuItem MenuChannels { get; set; }
        public MenuItem MenuGames { get; set; }
        public MenuItem MenuUsers { get; set; }
        public MenuItem MenuClose { get; set; }
        public MenuItem MenuFlood { get; set; }
        public MenuItem MenuHelpShamia { get; set; }
        //public MenuItem MenuAddGame { get; set; }
        /// <summary>
        /// create menu and menuitens
        /// </summary>
        public void CreatTopMenu()
        {
            Menu.Items.Clear();

            // Menu language 
            MenuLanguage = new MenuItem()
            {
                Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["L"].ToString(),
                ToolTip = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuLanguageToolTip"].ToString()
            };
            foreach (TemplateLang i in Clanguage.Langs)
            {
                MenuItem temp = new MenuItem()
                {
                    Header = i.Lang,

                };
                MenuLanguage.Items.Add(temp);
                temp.Click += delegate { MnuItemClick(temp); };
            }
            Menu.Items.Add(MenuLanguage);

            // Menu select server
            MenuConnect = new MenuItem()
            {
                Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["ChannelUi"].ToString(),
                ToolTip = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuConnectToolTip"].ToString()
            };

            foreach (string i in SqliteSource.EnumServersDataBase())
            {
                // listar server e criar submenu
                RadioButton mnuChan = new RadioButton()
                {
                    Content = i.ToString()
                };
                MenuConnect.Items.Add(mnuChan);
                mnuChan.Click += delegate { MnuChanClick(i); };

                // check last channel radiobuttons
                if (i == Configuration.StConfiguration.Server)
                {
                    mnuChan.IsChecked = true;
                }
            }

            Menu.Items.Add(MenuConnect);

            // Menu channels irc listed database
            MenuChannels = new MenuItem()
            {
                Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuChannels"].ToString(),
                ToolTip =  ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuChannelsToolTip"].ToString()
            };

            // list channels in database
            foreach (string i in SqliteSource.EnumChannelsDataBase())
            {
                MenuItem del = new MenuItem()
                {
                    Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["Mnuitemdelete"].ToString()
                };
                ContextMenu c = new ContextMenu();
                c.Items.Add(del);

                MenuItem temp = new MenuItem()
                {
                    Header = i.ToString(),
                    ContextMenu = c
                };

                MenuChannels.Items.Add(temp);
                temp.Click += delegate { MnuMyChannels(i); };

                del.Click += delegate { MnudelClick(i); };

            }
            Menu.Items.Add(MenuChannels);

            // menu draw message in game
            MenuGames = new MenuItem()
            {
                Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuGames"].ToString(),
                ToolTip = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuGamesToolTip"].ToString()
            };

            Game.EnumGamesIn();

            // list game(s) add menu
            foreach (Games i in Game.GamesAvailable)
            {
                MenuItem gdel = new MenuItem()
                {
                    Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["Mnuitemdelete"].ToString()
                };
                ContextMenu gc = new ContextMenu();
                gc.Items.Add(gdel);
                gdel.Click += delegate { MnuGameDel(i.Name); };

                CheckBox temp = new CheckBox()
                {
                    Content = i.Name,
                    //IsChecked = i.IsEnabled
                    ContextMenu = gc
                };


                _enumBoxsGames.Add(temp);
                // get last status game in database
                //foreach (Games g in SqliteSource.EnumGamesSaved())
                //{
                //    // check name and compare
                //    if (i.Name == g.Name && g.IsEnabled)
                //    {
                //        temp.IsChecked = true;
                //    }
                //}


                MenuGames.Items.Add(temp);
                temp.Click += delegate { MnuGameClick(temp); };
            }
            Menu.Items.Add(MenuGames);

            // MenuUsers
            MenuUsers = new MenuItem()
            {
                Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuUsers"].ToString(),
                ToolTip = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuUsersToolTip"].ToString()
            };



            // get list
            foreach (TemplateMyUsersDataBase i in SqliteSource.GetListMyUsersDataBase())
            {
                MenuItem udel = new MenuItem()
                {
                    Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["Mnuitemdelete"].ToString()
                };
                ContextMenu uc = new ContextMenu();
                uc.Items.Add(udel);

                RadioButton temp = new RadioButton()
                {
                    Content = i.User,
                    ContextMenu = uc
                    
                };
                MenuUsers.Items.Add(temp);
                temp.Click += delegate { MenuUsersClick(temp); };
                udel.Click += delegate { MnuUserdel(i.User); };

            }

            // add in menu
            Menu.Items.Add(MenuUsers);

            MenuClose = new MenuItem()
            {
                Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuClose"].ToString(),
                ToolTip = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuCloseToolTip"].ToString(),
                Visibility = Visibility.Collapsed
            };
            MenuClose.Click += delegate { MenuCloseClick(); };
            Menu.Items.Add(MenuClose);

            // menuflood

            MenuFlood = new MenuItem()
            {
                Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuFlood"].ToString(),
                ToolTip = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuFloodToolTip"].ToString(),
            };
            
            CheckBox mnufl = new CheckBox()
            {
                Content = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuCheckBoxFlood"].ToString(),
            };
            mnufl.Click += delegate { mnufl_onclick(mnufl); };

            ComboBox mnuflc = new ComboBox();
            // 5 -> 10 messagens
            for (int i = 5; i < 11; i++)
            {
                mnuflc.Items.Add(i);
            }
            mnuflc.DropDownClosed += delegate { mnuflc_changed(mnuflc); };

            MenuFlood.Items.Add(mnufl);
            MenuFlood.Items.Add(mnuflc);

            Menu.Items.Add(MenuFlood);

            // Ajude Projeto Shamia
            MenuHelpShamia = new MenuItem()
            {
                Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuHelpShamia"].ToString(),
            };
            MenuHelpShamia.Click += delegate { MenuHelpShamia_Click(); };

            Menu.Items.Add(MenuHelpShamia);

            //// adcionar jogo
            //MenuAddGame = new MenuItem()
            //{
            //    Header = ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuAddGame"].ToString(),
            //};
            //MenuAddGame.Click += delegate { MenuAddgame_click(); };

            //Menu.Items.Add(MenuAddGame);
        }

        private void MnuUserdel(string user)
        {
            if (user != string.Empty)
            {
                SqliteSource.DelMnuUserItem(user);
                CreatTopMenu();
            }
        }

        private void MnuGameDel(string games)
        {
            if (games != string.Empty)
            {
                SqliteSource.DeleteGame(games);
                CreatTopMenu();
            }
        }

        private void MenuAddgame_click()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ajude projeto shamia
        /// </summary>
        private void MenuHelpShamia_Click()
        {
            // paypal donate
            Process.Start(@"https://goo.gl/N7zLN5");
        }

        /// <summary>
        /// combobox
        /// </summary>
        /// <param name="mnuflc"></param>
        private void mnuflc_changed(ComboBox mnuflc)
        {
            if (mnuflc == null || mnuflc.Text.Trim() == string.Empty) { return; }
            Flood.Max = Convert.ToInt32(mnuflc.Text);
        }
        /// <summary>
        /// checkbox
        /// </summary>
        /// <param name="mnufl"></param>
        private void mnufl_onclick(CheckBox mnufl)
        {
            if (mnufl.IsChecked != null && mnufl.IsChecked.Value)
            {
                Flood.IsEnabled = true;
                return;
            }
            Flood.IsEnabled = false;

            //Flood.IsEnabled = mnufl.IsChecked.HasValue;
        }

        private void MenuCloseClick()
        {
            SendToIrc("QUIT",true);
            MenuChannels.Visibility = Visibility.Visible;
            MenuClose.Visibility = Visibility.Collapsed;
        }

        private void MnudelClick(string del)
        {
            //Console.WriteLine(del.ToString());
            if (del != string.Empty)

            SqliteSource.DeleteChanne(del.ToString());
            // creat menuitem and update
            CreatTopMenu();
        }

        /// <summary>
        /// MenuUsers click
        /// </summary>
        /// <param name="temp"></param>
        private void MenuUsersClick(RadioButton temp)
        {
            //Console.WriteLine(temp.Content);

            foreach (TemplateMyUsersDataBase u in SqliteSource.GetListMyUsersDataBase())
            {
                // search in user(s) get user and owner
                if (temp.Content.ToString() == u.User)
                {
                    Configuration.StConfiguration.Nick = u.User; // update user
                    Configuration.StConfiguration.Owner = u.Owner; // owner nick in chat
                    Configuration.StConfiguration.Password = u.Password; // password for auth
                }
            }
            // save config in SQLite
            SqliteSource.UpdateSaved();
        }

        private List<CheckBox> _enumBoxsGames = new List<CheckBox>();

        private void MnuGameClick(CheckBox c)
        {
            foreach (Games game in Game.GamesAvailable)
            {
                if (c.Content.ToString() == game.Name)
                {
                    if (c.IsChecked == true)
                    {
                        game.IsEnabled = true;
                        // disable all games in list (class)
                        foreach (Games g in Game.GamesAvailable)
                        {
                            if (g.Name != game.Name)
                            {
                                g.IsEnabled = false;
                            }
                        }
                        // uncheck all checkbox select one process (check box)
                        foreach (CheckBox i in _enumBoxsGames)
                        {
                            if (i.Content.ToString() != game.Name)
                            {
                                i.IsChecked = false;
                                // destroy object(s) if exist and jump game(s)
                                // destroy all object(s) draw
                                MyDelegates.DestroyObjectInGame(i.Content.ToString());
                            }
                        }
                    }
                    else
                    {
                        game.IsEnabled = false;

                        // destroy all object(s) draw
                        MyDelegates.DestroyObjectInGame(game.Name);
                    }
                }
            }
        }

        private void MnuMyChannels(string s)
        {
            CanvasAllShowHide(false);

            // atualiza o canal 
            if (s != null && s != Configuration.StConfiguration.Channel)
            {
                Configuration.StConfiguration.Channel = s;
            }

            // connect clicked
            // call connecion IRC
            try
            {
                // check se esta em chat send QUIT encerrar conexao
                //if(CanvasContent.Visibility == Visibility.Visible) { _Quit();}

                MenuChannels.Visibility = Visibility.Collapsed;
                MenuClose.Visibility = Visibility.Visible;

                // kill thread if exist
                ThIrc?.Abort();
                ThIrc = new Thread(ThreadIrc)
                {
                    IsBackground = true
                };
                ThIrc.Start();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            // save config in SQLite
            SqliteSource.UpdateSaved();

            //Console.WriteLine(s);
        }

        public Thread ThIrc;
        /// <summary>
        /// menuitem servers
        /// </summary>
        /// <param name="item"></param>
        private void MnuChanClick(string item)
        {
            // udpate object(used connect) field server
            if (item != null && item != Configuration.StConfiguration.Server)
            {   
                Configuration.StConfiguration.Server = item;
            }

            // save config in SQLite
            SqliteSource.UpdateServerUsedAndPort();

            //Console.WriteLine(Configuration.StConfiguration.Server);
        }
        /// <summary>
        /// menuitem language
        /// </summary>
        /// <param name="item"></param>
        private void MnuItemClick(MenuItem item)
        {
            var b = Clanguage.GetLangInClient();
            // check update lang
            if (item.Header.ToString() != Clanguage.ConvertLangInFlag(item.Header.ToString()))
            {
                // call method update resoucedictinary
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action) delegate
                {
                    Clanguage.SetLanguage(Clanguage.ConvertLangInFlag(item.Header.ToString()));
                    RefreshTranslate();
                });
            }
        }
        /// <summary>
        /// refresh object que forao traduzidos
        /// </summary>
        public void RefreshTranslate()
        {
            MenuLanguage.Header =
                ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["L"].ToString();
            MenuConnect.Header =
                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["ChannelUi"].ToString();
            MenuLanguage.ToolTip =
               ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuLanguageToolTip"].ToString();
            MenuConnect.ToolTip =
                ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuConnectToolTip"].ToString();
            LblChan.Content =
                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["Status"].ToString();
            MenuChannels.Header =
                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuChannels"].ToString();
            MenuChannels.ToolTip =
                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuChannelsToolTip"].ToString();

            MenuGames.Header =
                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuGames"].ToString();
            MenuGames.ToolTip =
                ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuGamesToolTip"].ToString();
            MenuUsers.Header =
                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuUsers"].ToString();
            MenuUsers.ToolTip =
                ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuUsersToolTip"].ToString();
            MenuClose.Header =
                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuClose"].ToString();
            MenuClose.ToolTip =
                ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuCloseToolTip"].ToString();
            MenuFlood.Header =
                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuFlood"].ToString();
            MenuFlood.ToolTip =
                ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuFloodToolTip"].ToString();
            MenuHelpShamia.Header =
               ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["MenuHelpShamia"].ToString();
           
        }
        /// <summary>
        /// set default font D3D9
        /// </summary>
        internal void _defaultUiConf()
        {
                
        }


        #region effect

        public enum Efeitos
        {
            Surgir,
            Desaparecer,
        }
        public void Effectfade(Label obj, Efeitos show = 0, string message = null, int time = 5, Brush color = null)
        {
            obj.Foreground = color ?? Color.White;
            if (message != null)
            {
                obj.Content = message;
            }

            // Create a storyboard to contain the animations.
            Storyboard storyboard = new Storyboard();
            TimeSpan duration = new TimeSpan(0, 0, time);

            // Create a DoubleAnimation to fade the not selected option control
            DoubleAnimation animation = new DoubleAnimation();

            // Oculta
            if (show == Efeitos.Desaparecer)
            {
                obj.Opacity = 0;
                animation.From = 1.0;
                animation.To = 0.0;
            }
            // Show
            else
            {
                animation.From = 0.0;
                animation.To = 1.0;
            }

            animation.Duration = new Duration(duration);
            // Configure the animation to target de property Opacity
            Storyboard.SetTargetName(animation, obj.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));
            // Add the animation to the storyboard
            storyboard.Children.Add(animation);

            // Begin the storyboard
            storyboard.Begin(this);

        }
        #endregion


        #endregion

        private void ChkAutoScroll_OnUnchecked(object sender, RoutedEventArgs e)
        {
        }

        private void ChkAutoScroll_OnChecked(object sender, RoutedEventArgs e)
        {
        }

        #region ThreadIRC

        private TextReader _input;
        private TextWriter _output;
        private TcpClient _sock;
        private string Buff { get; set; }
        /// <summary>
        /// thread creat for IRC service
        /// </summary>
        internal void ThreadIrc()
        {
            try
            {
                _sock = new TcpClient();
                _sock.Connect(Configuration.StConfiguration.Server,
                    Configuration.StConfiguration.Port);
                if (!_sock.Connected)
                {
                    MyDelegates.OnDebugMessageCallBack(@"failed to connect " + Environment.NewLine +
                        @"internal void ThreadIrc()");
                }

                _input = new StreamReader(_sock.GetStream());
                _output = new StreamWriter(_sock.GetStream());

                _output.Write(
                   "USER " + Configuration.StConfiguration.Nick + " 0 * :" + Configuration.StConfiguration.Owner + "\r\n" +
                   "NICK " + Configuration.StConfiguration.Owner + "\r\n");
                _output.Flush();

                // check auth ssl
                if (Configuration.StConfiguration.AuthSsl.ToString() == @"true")
                {
                    // auth enabled
                    LoginSsl(Configuration.StConfiguration.Nick,
                        Configuration.StConfiguration.Password);
                    MyDelegates.OnCallStatus(Resources.MergedDictionaries[0]["AuthSslEnabled"].ToString());
                }
                else
                {
                    // auth disabled
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal, (Action) delegate
                        {
                            MyDelegates.OnCallStatus(Resources.MergedDictionaries[0]["AuthSslDisabled"].ToString());
                        });
                }


                for(Buff = _input.ReadLine();; Buff = _input.ReadLine())
                {
                    if(Buff == null) { return;}
                    MyDelegates.OnCallBackOut(Buff);

                    // send pong reply to any ping messages
                    if (Buff != null && Buff.StartsWith("PING"))
                    {
                        _output.Write(Buff.Replace("PING","PONG") + "\r\n");
                        _output.Flush();
                    }
                    if(Buff != null && Buff[0] != ':' ) continue;

                    if (Buff != null && Buff.Split(' ')[1] == "001")
                    {
                        _output.Write("MODE " + Configuration.StConfiguration.Nick + "+o \r\n" +
                                      "JOIN " + Configuration.StConfiguration.Channel + "\r\n");
                        _output.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                if(ex.HResult != -2146233040)
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);    
            }
        }
        /// <summary>
        /// used auth server irc moderate
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="pw">password</param>
        internal void LoginSsl(string user, string pw)
        {
            _output = new StreamWriter(_sock.GetStream());
            _output.Write("OPER " + user + " " + pw + " " + "\r\n");
            _output.Flush();
        }
        #endregion

        
        /// <summary>
        /// add servers and channels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CmdAddServerAndChannel_OnClick(object sender, RoutedEventArgs e)
        {
           string c = await this.ShowInputAsync(Resources.MergedDictionaries[0]["InputBoxAddChannelTitle"].ToString(), Resources.MergedDictionaries[0]["InputBoxAddChannel"].ToString());
            if(c == null) { return;}
            if (c.Trim() != string.Empty)
            {
                // adjust string channel
                // #azubu.channel.en
                if (!c.StartsWith("#azubu."))
                {
                    c = @"#azubu." + c;
                }

                SqliteSource.AddChannel(c);
                // refresh channel(s) menuitem(s)
                CreatTopMenu();
            }
        }

        /// <summary>
        /// users
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmdAddUser_OnClick(object sender, RoutedEventArgs e)
        {
            Users u = new Users()
            {
                //BorderThickness = new Thickness(1, 1, 1, 1),
                //GlowBrush = new SolidColorBrush(Colors.DodgerBlue),
                ShowTitleBar = false,
                TitleCaps = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            u.ShowDialog();

            // update menuitem IF creat or delete user(s)
            CreatTopMenu();
        }
        /// <summary>
        /// about
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cmdabout_OnClick(object sender, RoutedEventArgs e)
        {
            // link facebook page
            Process.Start(@"https://goo.gl/obWDZC");
        }

        public void SendToIrc(string s,bool iscommand= false)
        {
            if(ThIrc == null) { return;}
            string[] r = s.Split(new string[] { "/", " " }, StringSplitOptions.None);

            _output = new StreamWriter(_sock.GetStream());

            try
            {
                // check se for comando
                if (iscommand)
                {
                    if (r[0] == @"QUIT")
                    {
                        Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Normal, (Action)delegate
                            {
                                _output.Write("QUIT" + "\r\n");
                                _output.Flush();

                                 //   
                                 //_Quit();
                                 ListChat.Items.Clear();
                                CanvasWelcome.Visibility = Visibility.Visible;
                                CanvasContent.Visibility = Visibility.Hidden;
                                TxtSend.Visibility = Visibility.Collapsed;
                                CanvasProgressring.Visibility = Visibility.Collapsed;
                                RingP.IsActive = false;
                                 // mensagem de boas vindas
                                 Effectfade(LblChan, Efeitos.Surgir, Resources.MergedDictionaries[0]["Status"].ToString(), 5, Color.DodgerBlue);
                                Title = Resources.MergedDictionaries[0]["Status"].ToString();


                            });
                        return;
                    }

                    switch (r[1])
                    {
                            // AUTH
                        case "AUTH":
                            string[] content = s.Split(new string[] { " ", "/" }, StringSplitOptions.None);
                            _output.Write(@"AUTH " + content[2] + @" " + content[3] + "\r\n");
                            _output.Flush();

                            MyDelegates.OnChatCallBack(@"system :" , "Auth Sucess");
                            break;

                        // kick => KICK <#channel> <nick> <comment banned>
                        // exemplo : /KICK usuario comentario
                        case "KICK":
                            string[] k = s.Split(new string[] {"/", " "}, StringSplitOptions.None);
                            _output.Write(@"KICK " + MainWindow.Configuration.StConfiguration.Channel + " " + k[2] + " " + k[3] + "\r\n" );
                            _output.Flush();
                            break;

                        // autokick
                        // exemplo : /ADDKICK usuario
                        case "ADDKICK":
                            string[] ak = s.Split(new string[] {"/", " "}, StringSplitOptions.None);
                            if(ak[2] != string.Empty)
                            Autokick.AddUser(ak[2]);

                            break;

                        // removeautokick
                        // ex: /REMOVEAUTOKICK usuario
                        case "REMOVEKICK":
                            string[] rk = s.Split(new string[] {"/", " "}, StringSplitOptions.None);
                            if (rk[2] != string.Empty)
                            Autokick.RemoveUser(rk[2]);

                            break;

                        // clearall
                        // ex: /CLEARAUTOKICK
                        case "CLEARKICK":
                            Autokick.ClearAll();

                            break;

                        // clear chat client
                        // exemplo : /CLEAR
                        case "CLEAR":
                            Application.Current.Dispatcher.Invoke(
                                DispatcherPriority.Normal, (Action) delegate
                                {
                                   ListChat.Items.Clear();
                                   MyDelegates.OnChatCallBack(@"system :" ,@"clean client chat");
                                });
                            break;

                        // envia comando TCP direto
                        // exemplos no site quakenet => https://www.quakenet.org/help/q-commands
                        case "CMD":
                            Application.Current.Dispatcher.Invoke(
                               DispatcherPriority.Normal, (Action)delegate
                               {
                                 _output.Write(s.Replace("/CMD",string.Empty) + "\r\n");
                                 _output.Flush();
                               });
                            break;

                        // Debug Show and Hide
                        case "DEBUGON":
                            Showconsole.ShowConsole();
                            break;
                        case "DEBUGOFF":
                            Showconsole.ShowConsole(false);
                            break;
                    }

                    return;
                }


                _output.Write("PRIVMSG " + Configuration.StConfiguration.Channel + " : " + s + "\r\n");
                _output.Flush();
                MyDelegates.OnChatCallBack(@"system :" , s);
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);    
            }

        }

        /// <summary>
        /// fecha objecto(s)
        /// </summary>
        internal void _Quit()
        {
            Application.Current.Dispatcher.Invoke(
                             DispatcherPriority.Normal, (Action)delegate
                             {
                                 _output.Write("QUIT" + "\r\n");
                                 _output.Flush();

                                 //   
                                 //_Quit();
                                 ListChat.Items.Clear();
                                 CanvasWelcome.Visibility = Visibility.Visible;
                                 CanvasContent.Visibility = Visibility.Hidden;
                                 TxtSend.Visibility = Visibility.Collapsed;
                                 CanvasProgressring.Visibility = Visibility.Collapsed;
                                 RingP.IsActive = false;
                                 // mensagem de boas vindas
                                 Effectfade(LblChan, Efeitos.Surgir, Resources.MergedDictionaries[0]["Status"].ToString(), 5, Color.DodgerBlue);
                                 Title = Resources.MergedDictionaries[0]["Status"].ToString();
                             });
        }
        private void CmdAdd_OnClick(object sender, RoutedEventArgs e)
        {
            Uidraw window = new Uidraw()
            {
                //BorderThickness = new Thickness(1, 1, 1, 1),
                //GlowBrush = new SolidColorBrush(Colors.DodgerBlue),
                ShowTitleBar = false,
                TitleCaps = false,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }

        private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Chatkick_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListChat.SelectedItem == null) { return; }
            var l = ListChat.SelectedItem as TemplateListViewChat ?? new TemplateListViewChat();

            SendToIrc(@"/KICK " + l.Yuser.Replace(":",string.Empty) + " " + "kicked",true);
        }

        private void Chataddautokick_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    if (ListChat.SelectedItem == null) { return; }
                    var l = ListChat.SelectedItem as TemplateListViewChat ?? new TemplateListViewChat();
                    SendToIrc("/ADDKICK " + l.Yuser.Replace(":", string.Empty), true);
                });
                
        }

        private void Chatremoveautokick_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListChat.SelectedItem == null) { return; }
            var l = ListChat.SelectedItem as TemplateListViewChat ?? new TemplateListViewChat();
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    //Autokick.RemoveUser(l.Yuser.Replace(":", string.Empty));
                    SendToIrc("/REMOVEKICK " + l.Yuser.Replace(":", string.Empty),true);
                });
        }

        private void ChatClearMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action)delegate
                {
                    //Autokick.RemoveUser(l.Yuser.Replace(":", string.Empty));
                    SendToIrc("/CLEAR", true);
                });
        }

        private void ChatClearKick_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
               DispatcherPriority.Normal, (Action)delegate
               {
                    //Autokick.RemoveUser(l.Yuser.Replace(":", string.Empty));
                    SendToIrc("/CLEARKICK", true);
               });
        }

        private async void CmdGame_OnClick(object sender, RoutedEventArgs e)
        {
            string c = await this.ShowInputAsync(Resources.MergedDictionaries[0]["InputBoxAddGameTitle"].ToString(), Resources.MergedDictionaries[0]["InputBoxAddGame"].ToString());
            if (c == null) { return; }

            if (c.Trim() != string.Empty)
            {
                if (c.Contains(".exe"))
                {
                    c = c.Replace(".exe", string.Empty);
                }
                SqliteSource.AddGame(c);

            }
            CreatTopMenu();
        }
    }
}
