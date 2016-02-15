using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Color = System.Windows.Media.Brushes;
using DX9OverlayAPI;
using MahApps.Metro.Controls.Dialogs;

namespace Shamai_R4.Class
{
    public static class MyDelegates
    {
        #region MyRegion

        public static ColorsHexDefault ColorDefault = new ColorsHexDefault()
        {
            Color = "Default",
            Hex = 0xffff0000
        };

        //0xFF00FFFF
        public static List<ColorsHexDefault> ColorsHex = new List<ColorsHexDefault>()
        {
            new ColorsHexDefault()
            {
                Color = "Black",
                Hex = 0xff000000,
            },
            new ColorsHexDefault()
            {
                Color = "Gray",
                Hex = 0xff808080,
            }
            ,
            new ColorsHexDefault()
            {
                Color = "White",
                Hex = 0xffffffff,
            }
            ,
            new ColorsHexDefault()
            {
                Color = "Aqua",
                Hex = 0xff00ffff,
            }
            ,
            new ColorsHexDefault()
            {
                Color = "Red",
                Hex = 0xffff0000,
            }
            ,
            new ColorsHexDefault()
            {
                Color = "Green",
                Hex = 0xff00ff00,
            }
            ,
            new ColorsHexDefault()
            {
                Color = "Blue",
                Hex = 0xff0000ff,
            }
            ,
            new ColorsHexDefault()
            {
                Color = "Yellow",
                Hex = 0xffffff00,
            }
            ,
            new ColorsHexDefault()
            {
                Color = "Fuchsia",
                Hex = 0xffff00ff,
            }
            ,
            new ColorsHexDefault()
            {
                Color = "Cyan",
                Hex = 0xff00ffff,
            }
        };

        public static FontDefault DefaultFont = new FontDefault()
        {
            Name = "Comic Sans MS"
        };

        public static List<FontDefault> FontDefaults = new List<FontDefault>()
        {
            new FontDefault()
            {
                Name = @"Arial"
            },
            new FontDefault()
            {
                Name = @"Tahoma"
            },
            new FontDefault()
            {
                Name = @"Courier New"
            },
            new FontDefault()
            {
                Name = @"Lucida Console"
            },
            new FontDefault()
            {
                Name = @"Georgia"
            },
            new FontDefault()
            {
                Name = @"Comic Sans MS"
            },
            new FontDefault()
            {
                Name = @"Segoe UI"
            },
            new FontDefault()
            {
                Name = @"Times New Roman"
            },
            new FontDefault()
            {
                Name = @"Verdana"
            },
        };

        #endregion


        #region OnDebugMessageCallBack

        private delegate void DebugMessage(string message);

#pragma warning disable 67
        private static event DebugMessage DebugMessageCallBack;
#pragma warning restore 67
        /// <summary>
        /// simple delegate event display message debug
        /// </summary>
        /// <param name="message"></param>
        public static void OnDebugMessageCallBack(string message)
        {
            try
            {
                if (message != null)
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Normal, (Action) delegate
                        {
                            Console.WriteLine(DateTime.Now + @" " + message);
                        });
            }
            catch (Exception ex)
            {
                OnDebugMessageCallBack(ex.StackTrace);
            }

        }

        #endregion


        private delegate void Status(string message);

#pragma warning disable 67
        private static event Status CallStatus;
#pragma warning restore 67

        public static void OnCallStatus(string message)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    ((MainWindow) Application.Current.MainWindow).Effectfade(
                        ((MainWindow) Application.Current.MainWindow).LblChan,
                        MainWindow.Efeitos.Surgir,
                        message,
                        5,
                        Color.DodgerBlue);
                    ((MainWindow) Application.Current.MainWindow).Title =
                        ((MainWindow) Application.Current.MainWindow).LblChan.Content.ToString();
                });

        }

        #region OnChatCallBack

        private delegate void Chat(string message);

#pragma warning disable 67
        private static event Chat ChatCallBack;
#pragma warning restore 67
        /// <summary>
        /// method called add chat message
        /// </summary>
        /// <param name="send"></param>
        /// <param name="content"></param>
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public static void OnChatCallBack(string send, string content)
        {

            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    Console.WriteLine(content);



                    #region unicodeList

                    List<string> unicodes = new List<string>()
                    {
                        "\u00030\u0003",
                        "\u00031\u0003",
                        "\u00032\u0003",
                        "\u00033\u0003",
                        "\u00034\u0003",
                        "\u00035\u0003",
                        "\u00036\u0003",
                        "\u00036\u0003",
                        "\u00038\u0003",
                        "\u00039\u0003",

                    };

                    #endregion

                    if (content.Length > 60)
                    {
                        int l = content.Length/2;
                        content = content.Substring(0, l) + Environment.NewLine +
                                  content.Substring(l, content.Length - l);
                    }

                    string[] c = content.Split(new string[] {"\u0003"}, StringSplitOptions.None);
                    content = c[0];

                    ((MainWindow) Application.Current.MainWindow).ListChat.Items.Add(new TemplateListViewChat()
                    {
                        Yuser = send,
                        Ycontent = content
                    });

                    #region games

                    if (!content.StartsWith(".clear"))
                    {
                        TemplateListViewChat t = new TemplateListViewChat()
                        {
                            Yuser = send,
                            Ycontent = content
                        };
                        OnCallCheckGames(send + @" " + t.Ycontent);
                    }

                    #endregion

                    // auto scroll
                    if (((MainWindow) Application.Current.MainWindow).ChkAutoScroll != null &&
                        ((MainWindow) Application.Current.MainWindow).ChkAutoScroll.IsChecked.Value)
                    {
                        ((MainWindow) Application.Current.MainWindow).ListChat.ScrollIntoView(
                            ((MainWindow) Application.Current.MainWindow).ListChat.Items[
                                ((MainWindow) Application.Current.MainWindow).ListChat.Items.Count - 1]
                            );
                    }

                    OnCallEventFlood(send);

                });
        }

        #endregion

        #region OnCallBackOut

        private delegate void CallBack(string message);

#pragma warning disable 67
        private static event CallBack CallBackOut;
#pragma warning restore 67

        public static void OnCallBackOut(string s)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    if (s == null)
                    {
                        return;
                    }
                    // show debug message console delegate
                    OnDebugMessageCallBack(s);

                    // check Part or Join
                    if (s.Contains("PART") || s.Contains("JOIN"))
                    {
                        string[] part = s.Split(new string[] {":", "!"}, StringSplitOptions.None); // nick user



                        // leave channel
                        if (s.Contains("PART"))
                        {
                            //Sdelegates.OnPartJoinCall(Sdelegates.ParJon.Leave,
                            //part[1]);
                        }
                        // enter channel
                        else if (s.Contains("JOIN"))
                        {
                            //Sdelegates.OnPartJoinCall(Sdelegates.ParJon.Enter,
                            //part[1]);
                        }
                    }
                    else if (s.Contains("PRIVMSG") && !s.Contains("CPRIVMSG"))
                    {
                        string[] msg = s.Split(new string[] {":", @"\"}, StringSplitOptions.None);
                        // msg[2] == content message
                        // msg.count() -1 ex content messa irc  remove(\u00038\u0003)

                        /*
                        paullascs!azubu@aa11cbb1.flash.quakenet.org PRIVMSG #azubu.henrytado.br "Henry o que você acha da história do Aruan?\u00038\u0003 


                        */
                        for (int i = 0; i < msg.Count(); i++)
                        {
                            if (i > 2)
                            {
                                msg[2] += msg[i];
                            }
                        }


                        string[] user = s.Split(new string[] {":", "!"}, StringSplitOptions.None);
                        // user[1] == user nick


                        OnChatCallBack(user[1] + ": ", msg[2]);
                        //msg[2]    );

                        // auto kick check list contains user
                        OnCallAutoKick(user[1]);
                    }
                    // welcome to channel
                    else if (s.Contains("CPRIVMSG"))
                    {
                        /*
                        ":servercentral.il.us.quakenet.org 005 teste WHOX WALLCHOPS WALLVOICES USERIP CPRIVMSG CNOTICE SILENCE=15 MODES=6 MAXCHANNELS=20 MAXBANS=45 NICKLEN=15 :are supported by this server"
                        */
                        string[] chan = s.Split(new string[] {" "}, StringSplitOptions.None);
                        //Console.WriteLine(chan[1]); Application.Current.MainWindow.Resources.MergedDictionaries[0]["Welcomechat"].ToString()
                        //Sdelegates.OnUpStatus(Application.Current.MainWindow.Resources.MergedDictionaries[0]["Welcomechat"] + @" " + MainWindow.Core.Conf.Chan);

                        // disable progress ring 
                        Application.Current.Dispatcher.Invoke(
                            DispatcherPriority.Normal, (Action) delegate
                            {

                                OnCallStatus(
                                    Application.Current.MainWindow.Resources.MergedDictionaries[0]["WelcomeChannel"]
                                        .ToString() + @" " +
                                    MainWindow.Configuration.StConfiguration.Channel);

                                ((MainWindow) Application.Current.MainWindow).CanvasAllShowHide();

                                // auth for privilege(s) to channel
                                ((MainWindow) Application.Current.MainWindow).SendToIrc(
                                    @"/AUTH " + MainWindow.Configuration.StConfiguration.Nick + " " +
                                    MainWindow.Configuration.StConfiguration.Password, true);

                            });
                    }
                    else if (s.Contains("Nickname is already in use"))
                    {
                        ((MainWindow) Application.Current.MainWindow).ShowMessageAsync(
                            ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0][
                                "NickInUseTitle"].ToString()
                            ,
                            ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0][
                                "NickInUseTitleContent"].ToString());

                    }


                });
        }


        #endregion

        #region OnCallDrawLeagueOfLegends

        private delegate void DrawLeagueOfLegends(string message);
#pragma warning disable 67
        private static event DrawLeagueOfLegends CallDrawLeagueOfLegends;
#pragma warning restore 67
        /// <summary>
        /// method draw text league of legends 
        /// </summary>
        /// <param name="message"></param>
        private static int _overlayText1 = -1;

        private static int _overlayText2 = -1;
        private static int _overlayText3 = -1;
        private static int _overlayText4 = -1;
        private static int _overlayText5 = -1;

        private static bool Initialized { get; set; }

        private static ListBox _listChat = new ListBox()
        {
            //Items = { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty }
        };

        /// <summary>
        /// method draw text chat in league of legends game client
        /// </summary>
        /// <param name="message"></param>
        public static void OnCallDrawLeagueOfLegends(string message)
        {
            if (!IsLeagueOfLegendsProcessOpen())
            {
                return;
            }

            //byte[] bytes = Encoding.Default.GetBytes(message);
            //message = Encoding.UTF8.GetString(bytes);

            //byte[] bytes = Encoding.Default.GetBytes(message);
            //message = Encoding.UTF8.GetString(bytes);


            _listChat.Items.Add(message);

            Dx9Overlay.SetParam("use_window", "1"); // Use the window name to find the process
            Dx9Overlay.SetParam("window", "League of Legends (TM) Client"); // Set the window name
            //Dx9Overlay.SetParam("window", "Test D3D9 window"); // Set the window name
            if (!Initialized)
            {
                InitializedObjectD3();
            }


            //TemplateUiChatD3 chat = MessagesUi[0];
            try
            {
                //Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count].ToString());
                //Dx9Overlay.TextSetString(_overlayText2, _listChat.Items[_listChat.Items.Count - 1].ToString());
                //Dx9Overlay.TextSetString(_overlayText3, _listChat.Items[_listChat.Items.Count - 2].ToString());
                //Dx9Overlay.TextSetString(_overlayText4, _listChat.Items[_listChat.Items.Count - 4].ToString());
                //Dx9Overlay.TextSetString(_overlayText5, _listChat.Items[_listChat.Items.Count - 5].ToString());
                //for (int i = 0; i < _listChat.Items.Count; i++)
                //{
                Console.Clear();
                if (_listChat.Items.Count == 1)
                {
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count -1].ToString());

                    Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count - 1].ToString());
                }

                else if (_listChat.Items.Count == 2)
                {
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 1].ToString());
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 2].ToString());

                    Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count - 1].ToString());
                    Dx9Overlay.TextSetString(_overlayText2, _listChat.Items[_listChat.Items.Count - 2].ToString());
                }
                else if (_listChat.Items.Count == 3)
                {
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 1].ToString());
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 2].ToString());
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 3].ToString());

                    Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count - 1].ToString());
                    Dx9Overlay.TextSetString(_overlayText2, _listChat.Items[_listChat.Items.Count - 2].ToString());
                    Dx9Overlay.TextSetString(_overlayText3, _listChat.Items[_listChat.Items.Count - 3].ToString());
                }
                else if (_listChat.Items.Count == 4)
                {
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 1].ToString());
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 2].ToString());
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 3].ToString());
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 4].ToString());

                    Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count - 1].ToString());
                    Dx9Overlay.TextSetString(_overlayText2, _listChat.Items[_listChat.Items.Count - 2].ToString());
                    Dx9Overlay.TextSetString(_overlayText3, _listChat.Items[_listChat.Items.Count - 3].ToString());
                    Dx9Overlay.TextSetString(_overlayText4, _listChat.Items[_listChat.Items.Count - 4].ToString());
                }
                else if (_listChat.Items.Count > 5 || _listChat.Items.Count == 5)
                {
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 1].ToString());
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 2].ToString());
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 3].ToString());
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 4].ToString());
                    //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 5].ToString());

                    Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count - 1].ToString());
                    Dx9Overlay.TextSetString(_overlayText2, _listChat.Items[_listChat.Items.Count - 2].ToString());
                    Dx9Overlay.TextSetString(_overlayText3, _listChat.Items[_listChat.Items.Count - 3].ToString());
                    Dx9Overlay.TextSetString(_overlayText4, _listChat.Items[_listChat.Items.Count - 4].ToString());
                    Dx9Overlay.TextSetString(_overlayText5, _listChat.Items[_listChat.Items.Count - 5].ToString());

                }
                //}
                //Console.WriteLine(_listChat.Items[_listChat.Items.Count].ToString());
                //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 1].ToString());
                //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 2].ToString());
                //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 3].ToString());
                //Console.WriteLine(_listChat.Items[_listChat.Items.Count - 4].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }




        }

        /// <summary>
        /// creat object Dx9Overlay
        /// </summary>
        internal static void InitializedObjectD3()
        {

            UiFontConf use = new UiFontConf();
            foreach (UiFontConf i in MainWindow.SqliteSource.EnumUiFont())
            {
                use.Fontname = i.Fontname;
                use.FontSize = i.FontSize;
                use.Isbold = i.Isbold;
                use.Isitalic = i.Isitalic;
                // set default brush
                // ReSharper disable once PossibleNullReferenceException
                foreach (ColorsHexDefault c in ColorsHex)
                {
                    if (c.Color == i.ColorName)
                    {
                        use.Brush = c.Hex;
                    }
                }
                //use.Brush = ColorDefault.Hex;
                //0xFF00FFFF
            }




            _overlayText1 = Dx9Overlay.TextCreate(use.Fontname, use.FontSize, use.Isbold, use.Isitalic, 5, 80, use.Brush,
                string.Empty, false,
                true); // Initialize 'overlayText'
            _overlayText2 = Dx9Overlay.TextCreate(use.Fontname, use.FontSize, use.Isbold, use.Isitalic, 5, 100,
                use.Brush, string.Empty,
                false, true); // Initialize 'overlayText'
            _overlayText3 = Dx9Overlay.TextCreate(use.Fontname, use.FontSize, use.Isbold, use.Isitalic, 5, 120,
                use.Brush, string.Empty,
                false, true); // Initialize 'overlayText'
            _overlayText4 = Dx9Overlay.TextCreate(use.Fontname, use.FontSize, use.Isbold, use.Isitalic, 5, 130,
                use.Brush, string.Empty,
                false, true); // Initialize 'overlayText'
            _overlayText5 = Dx9Overlay.TextCreate(use.Fontname, use.FontSize, use.Isbold, use.Isitalic, 5, 150,
                use.Brush, string.Empty,
                false, true); // Initialize 'overlayText'
            Initialized = true;
            _listChat.Items.Add(
                ((MainWindow) Application.Current.MainWindow).Resources.MergedDictionaries[0]["WelcomeShamiaDraw"]
                    .ToString());

        }

        /// <summary>
        /// method called destroy draw object(s) in game
        /// </summary>
        public static void OnCallDestroyLeagueOfLegends()
        {
            if (IsLeagueOfLegendsProcessOpen())
            {
                if (_overlayText1 != -1)
                {
                    Dx9Overlay.TextDestroy(_overlayText1);
                }
                if (_overlayText2 != -1)
                {
                    Dx9Overlay.TextDestroy(_overlayText2);
                }
                if (_overlayText3 != -1)
                {
                    Dx9Overlay.TextDestroy(_overlayText3);
                }
                if (_overlayText4 != -1)
                {
                    Dx9Overlay.TextDestroy(_overlayText4);
                }
                if (_overlayText5 != -1)
                {
                    Dx9Overlay.TextDestroy(_overlayText5);
                }
                // disable Initialized
                Initialized = false;

                // clear list message
                _listChat.Items.Clear();
            }
        }

        internal static bool IsLeagueOfLegendsProcessOpen()
        {
            Process[] p = Process.GetProcessesByName("League of Legends");
            return p.Length != 0;
        }

        private delegate void DestroyLeagueOfLegends();
#pragma warning disable 67
        private static event DestroyLeagueOfLegends CallDestroyLeagueOfLegends;
#pragma warning restore 67

        #endregion

        #region MyRegion

        private delegate void CheckGames();
#pragma warning disable 67
        private static event CheckGames CallCheckGames;
#pragma warning restore 67

        public static void OnCallCheckGames(string message)
        {
            foreach (Games game in Game.GamesAvailable)
            {
                if (game.IsEnabled)
                {
                    if (IsObjectExistProcess(game.Name))
                    {
                        OnCallDrawObjectForName(game.Name, message);
                    }


                    //switch (game.Name)
                    //{
                    //    case @"League Of Legends":
                    //        if (IsLeagueOfLegendsProcessOpen())
                    //        {
                    //            OnCallDrawLeagueOfLegends(message);
                    //        }
                    //        break;
                    //}
                }
            }
        }


        #endregion

        #region MyRegion

        private delegate void DrawObjectForName(string name, string message);
#pragma warning disable 67
        private static event DrawObjectForName CallDrawObjectForName;
#pragma warning restore 67

        /// <summary>
        /// draw text to object 
        /// </summary>
        /// <param name="to">object process</param>
        /// <param name="message">content message display</param>
        public static void OnCallDrawObjectForName(string to, string message)
        {
            if (IsObjectExistProcess(to))
            {
                _listChat.Items.Add(message);

                Dx9Overlay.SetParam("use_window", "1"); // Use the window name to find the process
                Dx9Overlay.SetParam("window", GetWindowNameForProcess(to)); // Set the window name
                if (!Initialized)
                {
                    InitializedObjectD3();
                }

                try
                {
                    if (_listChat.Items.Count == 1)
                    {
                        Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count - 1].ToString());
                    }
                    else if (_listChat.Items.Count == 2)
                    {
                        Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count - 1].ToString());
                        Dx9Overlay.TextSetString(_overlayText2, _listChat.Items[_listChat.Items.Count - 2].ToString());
                    }
                    else if (_listChat.Items.Count == 3)
                    {
                        Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count - 1].ToString());
                        Dx9Overlay.TextSetString(_overlayText2, _listChat.Items[_listChat.Items.Count - 2].ToString());
                        Dx9Overlay.TextSetString(_overlayText3, _listChat.Items[_listChat.Items.Count - 3].ToString());
                    }
                    else if (_listChat.Items.Count == 4)
                    {
                        Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count - 1].ToString());
                        Dx9Overlay.TextSetString(_overlayText2, _listChat.Items[_listChat.Items.Count - 2].ToString());
                        Dx9Overlay.TextSetString(_overlayText3, _listChat.Items[_listChat.Items.Count - 3].ToString());
                        Dx9Overlay.TextSetString(_overlayText4, _listChat.Items[_listChat.Items.Count - 4].ToString());
                    }
                    else if (_listChat.Items.Count > 5 || _listChat.Items.Count == 5)
                    {
                        Dx9Overlay.TextSetString(_overlayText1, _listChat.Items[_listChat.Items.Count - 1].ToString());
                        Dx9Overlay.TextSetString(_overlayText2, _listChat.Items[_listChat.Items.Count - 2].ToString());
                        Dx9Overlay.TextSetString(_overlayText3, _listChat.Items[_listChat.Items.Count - 3].ToString());
                        Dx9Overlay.TextSetString(_overlayText4, _listChat.Items[_listChat.Items.Count - 4].ToString());
                        Dx9Overlay.TextSetString(_overlayText5, _listChat.Items[_listChat.Items.Count - 5].ToString());
                    }
                }
                catch (Exception ex)
                {
                    OnDebugMessageCallBack(ex.StackTrace);
                }

            }
        }

        /// <summary>
        /// return if process open
        /// </summary>
        /// <param name="to">process name</param>
        /// <returns></returns>
        internal static bool IsObjectExistProcess(string to)
        {
            Process[] p = Process.GetProcessesByName(to);
            return p.Length != 0;
        }

        /// <summary>
        /// return title window for process name
        /// </summary>
        /// <param name="to">process name</param>
        /// <returns></returns>
        internal static string GetWindowNameForProcess(string to)
        {
            Process[] p = Process.GetProcessesByName(to);
            return p[0].MainWindowTitle;
        }

        /// <summary>
        /// destroy all object(s) draw window game
        /// </summary>
        internal static void DestroyObjectInGame(string to)
        {
            //if (IsObjectExistProcess(to))
            //{
            if (_overlayText1 != -1)
            {
                Dx9Overlay.TextDestroy(_overlayText1);
            }
            if (_overlayText2 != -1)
            {
                Dx9Overlay.TextDestroy(_overlayText2);
            }
            if (_overlayText3 != -1)
            {
                Dx9Overlay.TextDestroy(_overlayText3);
            }
            if (_overlayText4 != -1)
            {
                Dx9Overlay.TextDestroy(_overlayText4);
            }
            if (_overlayText5 != -1)
            {
                Dx9Overlay.TextDestroy(_overlayText5);
            }

            // disable Initialized
            Initialized = false;

            // clear list message
            _listChat.Items.Clear();

            //_overlayText1 = -1;
            //_overlayText2 = -1;
            //_overlayText3 = -1;
            //_overlayText4 = -1;
            //_overlayText5 = -1;
            //}
            //else
            //{

            //_overlayText1 = -1;
            //_overlayText2 = -1;
            //_overlayText3 = -1;
            //_overlayText4 = -1;
            //_overlayText5 = -1;
            //// disable Initialized
            //Initialized = false;

            //// clear list message
            //_listChat.Items.Clear();

        }


        #region OnCallAutoKick

        private delegate void EventAutoKick(string user);

#pragma warning disable 67
        private static event EventAutoKick CallAutoKick;
#pragma warning restore 67

        /// <summary>
        /// event autokick
        /// </summary>
        /// <param name="user"></param>
        public static void OnCallAutoKick(string user)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    // verificar se sistema ou usuario (owner)
                    if (user == MainWindow.Configuration.StConfiguration.Owner || user == @"system" || user == @"azubu")
                    {
                        return;
                    }
                    foreach (Kick i in Autokick.Kicks)
                    {
                        if (i.User == user)
                        {
                            // exist na lista kika
                            ((MainWindow) Application.Current.MainWindow).SendToIrc(@"/KICK " + user + " " + "kicked",
                                true);
                        }
                    }


                });

            #endregion
        }


        #region MyRegion

        private delegate void EventFlood(string user);

#pragma warning disable 67
        private static event EventFlood CallEventFlood;
#pragma warning restore 67

        #endregion

        public static void OnCallEventFlood(string user)
        {
            if (!Flood.IsEnabled)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal, (Action) delegate
                {
                    var total = ((MainWindow)Application.Current.MainWindow).ListChat.Items.Count;

                    int message = 0;
                    for (int i = 0; i < Flood.Max; i++)
                    {
                        if(total == 0 || total < 0) { return;}
                        var o = ((MainWindow) Application.Current.MainWindow).ListChat.Items[total -1] as TemplateListViewChat ?? new TemplateListViewChat();
                        if (o.Yuser == user && MainWindow.Configuration.StConfiguration.Owner != user)
                        {
                            message ++;
                        }
                        total --;
                    }

                    if (message == Flood.Max || message > Flood.Max)
                    {
                        // kicked
                        ((MainWindow) Application.Current.MainWindow).SendToIrc(
                            @"/KICK " + user.Replace(":", string.Empty) + " " + "flood", true);

                    }


                });
        }

        #endregion

    }
}

public class TemplateListViewChat
        {
            /// <summary>
            /// user or system send message
            /// </summary>
            public string Yuser { get; set; }

            /// <summary>
            /// body message 
            /// </summary>
            public string Ycontent { get; set; }
        }

        public class ColorsHexDefault
        {
            public uint Hex { get; set; }
            public string Color { get; set; }
        }

        public class FontDefault
        {
            public string Name { get; set; }
        }



//public class TemplateUiChatD3
    //{
    //    public string MessageA { get; set; }
    //    public string MessageB { get; set; }
    //    public string MessageC { get; set; }
    //    public string MessageD { get; set; }
    //    public string MessageE { get; set; }

    //}

