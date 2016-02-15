using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Finisar.SQLite;
using MahApps.Metro.Controls.Dialogs;
using Shamai_R4.View;

namespace Shamai_R4.Class
{
    /// <summary>
    /// this file part Shamia SqliteSource.cs
    /// </summary>
    public class SqliteSource
    {
        #region declare

        /// <summary>
        /// object used connect SQLite
        /// </summary>
        internal SQLiteConnection Connection;

        /// <summary>
        /// object SQLite commands
        /// </summary>
        internal SQLiteCommand C;

        /// <summary>
        /// define se o database existe ou nao 
        /// </summary>
        public bool IsDatabase { get; set; }
        /// <summary>
        /// object Datareader
        /// </summary>
        internal SQLiteDataReader DataReader;
        /// <summary>
        /// object game IsEnabled
        /// </summary>
        private bool Enabled { get; set; }

        #endregion
        /// <summary>
        /// construtor
        /// </summary>
        public SqliteSource()
        {
            Connection = new SQLiteConnection();
            if (!ExistDataBase()) { return; }
        }


        #region method
        /// <summary>
        /// check database exist
        /// </summary>
        /// <returns></returns>
        internal bool ExistDataBase()
        {
            return File.Exists(AppDomain.CurrentDomain.BaseDirectory +
                               "Database\\Shamia.db");
        }

        /// <summary>
        /// metodo cria ou acessa banco de dados sqlite carregar configs
        /// </summary>
        public void CreateOrAccessDataBase()
        {
            try
            {
                string file = AppDomain.CurrentDomain.BaseDirectory +
                              "Database\\Shamia.db";
                if (!ExistDataBase())
                {
                    Connection = new SQLiteConnection("Data Source=" + file + ";Version=3;New=True;Compress=True");
                    Connection.Open();
                    string[] config = new string[] {string.Empty, string.Empty, string.Empty};
                    // create table(s)
                    // config   => contais configurations
                    config[0] = @"create table config (port interger(4)" +
                                @",server varchar(30) primary key" +
                                @",language varchar(20))";
                    // channels => contais channels
                    config[1] = @"create table channels (channel varchar(30) primary key)";
                    // user(s)  => my login accont user(s) used login auth SSL
                    config[2] = @"create table users (nick varchar(20) primary key" +
                                @",password varchar(50))";

                    foreach (string i in config)
                    {
                        SQLiteCommand c = new SQLiteCommand(i, Connection);
                        c.ExecuteNonQuery();
                    }
                    IsDatabase = true;
                }
                else
                {
                    Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                                      ";New=False;Compress=True");
                    Connection.Open();
                    IsDatabase = true;

                    // select database to list all channels and servers
                    C = Connection.CreateCommand();
                    C.CommandText = @"SELECT channel FROM channels";
                    DataReader = C.ExecuteReader();
                    while (DataReader.Read())
                    {
                        MainWindow.Configuration.Channels.Add(new TemplateChannels
                        {
                            Channels = DataReader["channel"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.ToString());
                IsDatabase = false;
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }

        /// <summary>
        /// retorna bandeira de idioma usado
        /// </summary>
        /// <returns></returns>
        public string GetLang()
        {
            Clanguage._ListLangs();
            string file = AppDomain.CurrentDomain.BaseDirectory +
                          "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"SELECT language FROM config LIMIT 1";
                DataReader = C.ExecuteReader();

                while (DataReader.Read())
                {
                    foreach (TemplateLang i in Clanguage.Langs)
                    {
                        if (i.Lang == DataReader["language"].ToString())
                        {
                            return i.Flag;
                        }
                    }
                    //return DataReader["language"].ToString();
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.ToString());
                return string.Empty;
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
            return string.Empty;
        }

        /// <summary>
        /// retorna a lista de channels no database
        /// </summary>
        /// <returns></returns>
        public List<string> EnumServersDataBase()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                         "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            List<string> temp = new List<string>();

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"SELECT channel FROM channels";
                DataReader = C.ExecuteReader();
                while (DataReader.Read())
                {
                    temp.Add(DataReader["channel"].ToString());
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.ToString());    
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
            return temp;

        }

        /// <summary>
        /// list mychannels , channels in database
        /// </summary>
        /// <returns></returns>
        public List<string> EnumChannelsDataBase()
        {

            List<string> temp = new List<string>();
            string file = AppDomain.CurrentDomain.BaseDirectory +
                        "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"SELECT channel FROM mychannels";
                DataReader = C.ExecuteReader();
                while (DataReader.Read())
                {
                    temp.Add(DataReader["channel"].ToString());
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.ToString());
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
            return temp;
        }

        /// <summary>
        /// list top 1 user => used for Shamia connect
        /// </summary>
        /// <returns></returns>
        public List<TemplateUserDataBase> EnumUserDataBase()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                         "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            List<TemplateUserDataBase> t = new List<TemplateUserDataBase>();
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"SELECT *FROM users TOP1";
                DataReader = C.ExecuteReader();
                while (DataReader.Read())
                {
                   TemplateUserDataBase temp = new TemplateUserDataBase()
                   {
                       Nick = DataReader["nick"].ToString(),
                       Password = DataReader["password"].ToString(),
                       Auth = DataReader["auth"].ToString(),
                       Owner = DataReader["owner"].ToString(),
                       Port = Convert.ToInt32(DataReader["port"].ToString())

                   };
                    t.Add(temp);
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.ToString());
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
            return t;
        }

        /// <summary>
        /// get last server used to connect
        /// </summary>
        /// <returns></returns>
        public string GetLastServerUsedDataBase()
        {
            string temp = string.Empty;
            string file = AppDomain.CurrentDomain.BaseDirectory +
                        "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"SELECT server FROM config";
                DataReader = C.ExecuteReader();
                while (DataReader.Read())
                {
                    temp = DataReader["server"].ToString();
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.ToString());
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
            return temp;
        }

        /// <summary>
        /// return last channel connected saved in database
        /// </summary>
        /// <returns></returns>
        public string GetLastChannelUsedDataBase()
        {
            string temp = string.Empty;
            string file = AppDomain.CurrentDomain.BaseDirectory +
                        "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"SELECT channel FROM users TOP1";
                DataReader = C.ExecuteReader();
                while (DataReader.Read())
                {
                    temp = DataReader["channel"].ToString();
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.ToString());
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
            return temp;
        }

        /// <summary>
        /// list all users in database
        /// </summary>
        /// <returns></returns>
        public List<TemplateMyUsersDataBase> GetListMyUsersDataBase()
        {
            List<TemplateMyUsersDataBase> myUsersDataBases = new List<TemplateMyUsersDataBase>();
            string file = AppDomain.CurrentDomain.BaseDirectory +
                        "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"SELECT user , password , owner FROM myusers";
                DataReader = C.ExecuteReader();
                while (DataReader.Read())
                {
                    TemplateMyUsersDataBase t = new TemplateMyUsersDataBase()
                    {
                        User = DataReader["user"].ToString(),
                        Password = DataReader["password"].ToString(),
                        Owner = DataReader["owner"].ToString()
                    };
                    myUsersDataBases.Add(t);
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);    
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
            return myUsersDataBases;
        }

        /// <summary>
        /// creat user
        /// </summary>
        /// <param name="user"> user name</param>
        /// <param name="password">password</param>
        /// <param name="owner"></param>
        public void CreatUser(string user, string password,string owner)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                       "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"INSERT INTO myusers (user,password,owner) VALUES ('" + user + "','" + password + "','" + owner +"')" ;
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // chave duplicada
                if (ex.HResult == -2146233088)
                {
                    
                    MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
                    ((MainWindow)Application.Current.MainWindow).ShowMessageAsync(((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["ErroKeyDuplicateTitle"].ToString(),
                    ((MainWindow)Application.Current.MainWindow).Resources.MergedDictionaries[0]["ErroKeyDuplicate"].ToString());

                }
                else
                {
                    MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
                }
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }

        /// <summary>
        /// check user exist not INSERT
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns>
        public bool CheckUserExist(string user)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                      "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"SELECT user FROM myusers WHERE user ='" + user + "'";
                DataReader = C.ExecuteReader();
                while (DataReader.Read())
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
                return false;
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
            return false;
        }

        /// <summary>
        /// delete user from user
        /// </summary>
        /// <param name="user"></param>
        public void DeleteUser(string user)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                       "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"delete from myusers where user = '" + user + "'";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }

        /// <summary>
        /// delete all users in database
        /// </summary>
        public void DeleteAllUsers()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                       "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"delete from myusers";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);    
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }
        /// <summary>
        /// retorna enum games in database
        /// </summary>
        /// <returns></returns>
        public List<Games> EnumGamesSaved()
        {
            List<Games> t = new List<Games>();
            string file = AppDomain.CurrentDomain.BaseDirectory +
                     "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"select * from games";
                DataReader = C.ExecuteReader();
                

                while (DataReader.Read())
                {
                    if (DataReader["IsEnabled"].ToString() == @"true")
                    {
                        Enabled = true;
                    }
                    else
                    {
                        Enabled = false;
                    }
                    Games g = new Games()
                    {
                        Name = DataReader["Name"].ToString(),
                        //DisplayMessageCount = Convert.ToInt32(DataReader["DisplayMessaCount"].ToString()),
                        IsEnabled = Enabled
                    };
                    t.Add(g);
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
            return t;
        }

        /// <summary>
        /// add channel SQLite
        /// </summary>
        /// <param name="channel"></param>
        public void AddChannel(string channel)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                       "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"INSERT INTO mychannels VALUES('" + channel + "')";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }
        /// <summary>
        /// adciona jogo
        /// </summary>
        /// <param name="game">jogo</param>
        public void AddGame(string game)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                       "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"INSERT INTO Games VALUES('" + game + "','false')";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }

        /// <summary>
        /// return if exist config in SQLite
        /// </summary>
        /// <returns></returns>
        public List<UiFontConf> EnumUiFont()
        {
            List<UiFontConf> t = new List<UiFontConf>();

            string file = AppDomain.CurrentDomain.BaseDirectory +
                      "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"select * from uifontconf top1";
                DataReader = C.ExecuteReader();
                while (DataReader.Read())
                {
                    UiFontConf f = new UiFontConf()
                    {
                       Fontname = DataReader["fontname"].ToString(),
                       FontSize  = Convert.ToInt32(DataReader["fontsize"].ToString()),
                       Isbold =Convert.ToBoolean(DataReader["isbold"].ToString()),
                       Isitalic = Convert.ToBoolean(DataReader["isitalic"].ToString()),
                       ColorName = DataReader["fontcolor"].ToString()
                    };
                    t.Add(f);
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
                return null;
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
            return t;
        }

        public void UpdateUiConf(string fontname, int fontsize, bool isbold, bool isitalic, string color)
        {
            // delete saved
            DeleteUiConf();

            List<UiFontConf> t = new List<UiFontConf>();

            string file = AppDomain.CurrentDomain.BaseDirectory +
                      "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"INSERT INTO uifontconf VALUES('" + fontname + "','"+
                                fontsize + "','" + isbold + "','" + isitalic + "','" + color +"')";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);    
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }

        public void DeleteUiConf()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                      "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = "DELETE from uifontconf";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }
        /// <summary>
        /// metodo atualiza table users
        /// </summary>
        /// <param name="user"></param>
        public void UpdateSaved()
        {
            // delete users and new insert
            DeleteAllUser();

            string file = AppDomain.CurrentDomain.BaseDirectory +
                      "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = "INSERT INTO users VALUES('" + MainWindow.Configuration.StConfiguration.Nick + "','" +
                    MainWindow.Configuration.StConfiguration.Password +"','"+
                    MainWindow.Configuration.StConfiguration.AuthSsl + "','" +
                    MainWindow.Configuration.StConfiguration.Owner + "','" +
                    MainWindow.Configuration.StConfiguration.Port + "','"+
                    MainWindow.Configuration.StConfiguration.Channel +"')";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }
        /// <summary>
        /// delete table users
        /// </summary>
        internal void DeleteAllUser()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                      "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = "DELETE from users";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }
        /// <summary>
        /// update table config channel used and por
        /// </summary>
        internal void UpdateServerUsedAndPort()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                      "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = "UPDATE config SET port='" + MainWindow.Configuration.StConfiguration.Port + "'," +
                                "server='" + MainWindow.Configuration.StConfiguration.Server + "'";
                    
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }
        
        /// <summary>
        /// delete o channel SQLite table mychannels
        /// </summary>
        /// <param name="channel">channel deleted</param>
        internal void DeleteChanne(string channel)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                      "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = "DELETE from mychannels WHERE channel='" + channel + "'";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }

        internal void DeleteGame(string game)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                      "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = "DELETE from Games WHERE name='" + game + "'";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }

        internal void DelMnuUserItem(string user)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                     "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = "DELETE from myusers WHERE user='" + user + "'";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }

        /// <summary>
        /// retorna o usuario para ser editado ou deletado
        /// </summary>
        /// <returns></returns>
        /// <param name="user">user nick</param>
        internal TemplateMyUsersDataBase GetUserEdit(string user)
        {
            TemplateMyUsersDataBase t = new TemplateMyUsersDataBase();

            string file = AppDomain.CurrentDomain.BaseDirectory +
                     "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");
            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = @"SELECT user, password , owner FROM myusers WHERE user ='" + user + "'";
                DataReader = C.ExecuteReader();
                while (DataReader.Read())
                {
                    t.User = DataReader["user"].ToString();
                    t.Password = DataReader["password"].ToString();
                    t.Owner = DataReader["owner"].ToString();
                }
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
                return null;
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }

            return t;
        }

        /// <summary>
        /// update user 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="owner"></param>
        /// <param name="oldnick"></param>
        internal void UpdateUserIn(string user, string password, string owner ,string oldnick)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory +
                      "Database\\Shamia.db";
            if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
                                              ";New=False;Compress=True");

            try
            {
                Connection.Open();
                C = Connection.CreateCommand();
                C.CommandText = "UPDATE myusers SET user ='" + user + "',"+
                    "password='" + password + "'," +
                    "owner='" + owner + "'" +
                    " WHERE user='" + oldnick + "'";
                C.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyDelegates.OnDebugMessageCallBack(ex.StackTrace);
            }
            finally
            {
                if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
            }
        }
    }
        //public void CreatListChannel()
        //{
        //    string file = AppDomain.CurrentDomain.BaseDirectory +
        //                  "Database\\Shamia.db";
        //    if (Connection.State != ConnectionState.Closed) { Connection.Close(); }
        //    Connection = new SQLiteConnection("Data Source=" + file + ";Version=3" +
        //                                      ";New=False;Compress=True");
        //    try
        //    {
        //        Connection.Open();
        //        C = Connection.CreateCommand();
        //        C.CommandText = @"SELECT channel FROM channels";
        //        DataReader = C.ExecuteReader();
        //        while (DataReader.Read())
        //        {
        //            MainWindow.Config.ListChannel.Add(new Tchannels()
        //            {
        //                Channel = DataReader["channel"].ToString()
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SDelegateEvents.OnDebug(ex.ToString());
        //    }
        //}
        #endregion
    }

    #region class template userDatabse

    public class TemplateUserDataBase
    {
        public string Nick { get; set; }
        public string Password { get; set; }
        public string Auth { get; set; }
        public string Owner { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
    }

    public class TemplateMyUsersDataBase
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Owner { get; set; }
    }

    
    #endregion

