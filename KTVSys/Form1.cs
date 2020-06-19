using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KTVSys
{
    public partial class Form1 : Form
    {
        #region 定义的变量
        public static string[] yiDian = new string[0];
        public static string[] yiChang = new string[0];
        public int st;
        public int state;                   //当前选中的哪个界面
        public static int yiYe = 7;         //一页显示几条信息
        public static int index = 1;        //当前页
        public static int vol = 100;        //音量100
        public static int singerYe = 8;     //歌手页面一页显示几条信息
        public int quanPing = 0;            //判断是否全屏
        //歌曲查询sql语句
        public static string querySql = string.Format("select top {0} SongName,SingerName from Song_info inner join singer_info on singer_info.singerId = Song_info.singer where singer not in (select top ({0}*({1}-1)) singer from Song_info)", yiYe, index);
        //电影查询sql语句
        public static string movieSql = string.Format("select top {0} Name,MovieUrl from Movie_info where Name not in (select top ({0}*({1}-1)) Name from Movie_info)", yiYe, index);
        //歌手查询sql语句
        public static string singerSql = string.Format("select top {0} SingerName,SingerUrl from Singer_info where SingerName not in (select top ({0}*({1}-1)) SingerName from Singer_info)",singerYe,index);
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        #region 主界面加载时方法
        private void Form1_Load(object sender, EventArgs e)
        {
            mp.URL = Environment.CurrentDirectory+@"\mv\mv1.mp4";
            mp.settings.volume = vol;
            mp.ClickEvent += mp_ClickEvent;
            zhuYe();
            GetSuiJi();
        }
        #endregion

        #region 每1秒判断mediaplayer的状态 如果已停止 则重新播放
        void t_Tick(object sender, EventArgs e)
        {
            if (mp.status == "已停止")
            {
                if (yiDian.Length == 0)
                {
                   Random r = new Random();
                int i = r.Next(1, 6);
                mp.URL = Environment.CurrentDirectory + @"\mv\mv" + i + ".mp4";
                GetSuiJi();
                }
                else
                {
                    string a = yiDian[0];
                    string[] b = new string[yiDian.Length - 1];
                    for (int i = 0; i < b.Length; i++)
                    {
                        b[i] = yiDian[i + 1];
                    }
                    yiDian = b;
                    suiJiPlay();
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "top1")
                        {
                            item.Text = yiDian[0].Substring(1);
                        }
                    }
                    if (yiDian.Length > 1)
                    {
                        foreach (Control item1 in this.Controls)
                        {
                            if (Convert.ToString(item1.Tag) == "top2")
                            {
                                item1.Text = yiDian[1].Substring(1);
                            }
                        }
                    }
                }
                
            }
        }
        #endregion

        #region 获取当前随机播放的MV的文件名
        private void GetSuiJi()
        {
            switch (mp.currentMedia.getItemInfo("title"))
            {
                case "mv1":
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "top1")
                        {
                            item.Text = "Gone Gone Gone";
                        }
                    }
                    break;
                case "mv2":
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "top1")
                        {
                            item.Text = "Say You Again";
                        }
                    }
                    break;
                case "mv3":
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "top1")
                        {
                            item.Text = "烦恼歌";
                        }
                    }
                    break;
                case "mv4":
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "top1")
                        {
                            item.Text = "脱掉";
                        }
                    }
                    break;
                case "mv5":
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "top1")
                        {
                            item.Text = "Say Yes";
                        }
                    }
                    break;
            }
        }
        #endregion

        #region  当点击mediaplayer时最大化
        void mp_ClickEvent(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            if (mp.fullScreen)
            {
                mp.fullScreen = false;
            }
            else
            {
                mp.fullScreen = true;
            }   
        }
        #endregion

        #region 加载除了播放器路径的主页面
        private void zhuYe()
        {
            index = 1;
            moveControls();
            mp.Visible = false;
            #region 主界面picturebox 选项卡动态添加及事件
            for (int i = 0; i < 8; i++)
            {
                PictureBox fenLei = new PictureBox()
                {
                    Size = new Size(248, 183),
                    Location = new Point(160 + 270 * (i % 4), 175 + 200 * (i / 4)),
                    Image = Image.FromFile(Environment.CurrentDirectory + @"\ktv\" + (i + 1) + ".png"),
                    Tag = "fenLei" + i

                };
                fenLei.MouseEnter += fenLei_MouseEnter;
                fenLei.MouseLeave += fenLei_MouseLeave;
                fenLei.Click += fenLei_Click;
                this.Controls.Add(fenLei);
            }
            #endregion

            mainJieMian();
            GetSuiJi();
        }
        #endregion+

        #region 点击 类别时的方法
        void fenLei_Click(object sender, EventArgs e)
        {
            switch (((PictureBox)sender).Tag.ToString())
            {
                case "fenLei0": //电影欣赏
                    
                    state = 1;
                    st = 1;
                    moveControls();
                    dianGeBeiJingAdd();
                    mp.Visible = true;
                    mp.uiMode = "none";
                    mp.enableContextMenu = false;
                    string sql6 = movieSql;
                    ChaXunGeQu(sql6);
                    mainJieMian();
                    GetSuiJi();
                    HuanYeAnNiu();
                    break;
                case "fenLei1": //分类点歌
                   
                    state = 2;
                    st = 2;
                    moveControls();
                    dianGeBeiJingAdd();
                    mp.Visible = true;
                    mp.uiMode = "none";
                    mp.enableContextMenu = false;
                    pinYin();
                    string sql = querySql;
                    ChaXunGeQu(sql);
                    mainJieMian();
                    GetSuiJi();
                    HuanYeAnNiu();
                    break;
                case "fenLei2": //新歌推荐
                    
                    state = 3;
                    st = 3;
                    moveControls();
                    dianGeBeiJingAdd();
                    mp.Visible = true;
                    mp.uiMode = "none";
                    mp.enableContextMenu = false;
                    pinYin();
                    string sql1 = querySql;
                    sql1 += " order by TianJiaSJ desc";
                    ChaXunGeQu(sql1);
                    mainJieMian();
                    GetSuiJi();
                    HuanYeAnNiu();
                    break;
                case "fenLei3": //歌星点歌
                    
                    state = 4;
                    st = 4;
                    moveControls();
                    mp.Visible = true;
                    mp.uiMode = "none";
                    mp.enableContextMenu = false;
                    mainJieMian();
                    GetSuiJi();
                    string sql0 = singerSql;
                    singerAdd(sql0);
                    for (int i = 0; i < 6; i++)
                    {
                        MyLabel lb = new MyLabel()
                        {
                            AutoSize = false,
                            Size = new Size(107,34),
                            BackColor = Color.Red,
                            Location = new Point(118 + 130 * (i % 6), 600),
                            Tag = "dq"+i
                        };
                        lb.Click+=lblb_Click;
                        this.Controls.Add(lb);
                        lb.BringToFront();
                    }
                    pinYin();
                    for (int i = 0; i < 2; i++)
                    {
                        PictureBox pb = new PictureBox()
                        {
                            Image = Image.FromFile(Environment.CurrentDirectory + @"\ktv\z" + (i + 1) + ".png"),
                            Size = new Size(100, 100),
                            Location = new Point(20 + 880 * (i % 2), 340),
                            BackColor = Color.Transparent,
                            Tag = "shy" + i
                        };
                        pb.Click +=singerHuanYe_Click;
                        this.Controls.Add(pb);
                    }
                    break;
                case "fenLei4": //语种点歌
                    
                    state = 5;
                    st = 5;
                    moveControls();
                    dianGeBeiJingAdd();
                    mp.Visible = true;
                    mp.uiMode = "none";
                    mp.enableContextMenu = false;
                    pinYin();
                    string sql4 = querySql;
                    ChaXunGeQu(sql4);
                    mainJieMian();
                    GetSuiJi();
                    HuanYeAnNiu();
                    break;
                case "fenLei5": //金曲排行
                   
                    state = 6;
                    st = 6;
                    moveControls();
                    dianGeBeiJingAdd();
                    mp.Visible = true;
                    mp.uiMode = "none";
                    mp.enableContextMenu = false;
                    pinYin();
                    string sql2 = querySql;
                    sql2 += " order by CiShu desc";
                    ChaXunGeQu(sql2);
                    mainJieMian();
                    GetSuiJi();
                    HuanYeAnNiu();
                    break;
                case "fenLei6": //歌曲搜索
                    index = 1;
                    state = 7;
                    st = 7;
                    moveControls();
                    dianGeBeiJingAdd();
                    mp.Visible = true;
                    mp.uiMode = "none";
                    mp.enableContextMenu = false;
                    pinYin();
                    string sql3 = querySql;
                    ChaXunGeQu(sql3);
                    mainJieMian();
                    GetSuiJi();
                    HuanYeAnNiu();
                    break;
            }
        }
        #endregion

        #region 歌星换页点击事件
        private void singerHuanYe_Click(object sender, EventArgs e)
        {
            string sql = string.Empty;
            switch (((PictureBox)sender).Tag.ToString())
            {
                case "shy0":
                    if (index == 1)
                    {
                        return;
                    }
                    index--;
                    sql = string.Format("select top {0} SingerName,SingerUrl from Singer_info where SingerName not in (select top ({0}*({1}-1)) SingerName from Singer_info)", singerYe, index);
                    moveSinger();
                    singerAdd(sql);
                    break;
                case "shy1":
                    string a = "select CEILING(COUNT(*)/(" + singerYe + "*1.0)) from singer_info";
                    int i = Convert.ToInt32(DBHelper.GetScalar(a));
                    if (index == i)
                    {
                        return;
                    }
                    index++;
                    sql = string.Format("select top {0} SingerName,SingerUrl from Singer_info where SingerName not in (select top ({0}*({1}-1)) SingerName from Singer_info)", singerYe, index);
                    moveSinger();
                    singerAdd(sql);
                    break;
            }
        }
        #endregion

        #region 歌曲换页
        private void HuanYeAnNiu()
        {
            for (int i = 0; i < 2; i++)
            {
                PictureBox pb = new PictureBox()
                {
                    Image = Image.FromFile(Environment.CurrentDirectory + @"\ktv\z" + (i + 1) + ".png"),
                    Size = new Size(100, 100),
                    Location = new Point(20 + 847 * (i % 2), 340),
                    BackColor = Color.Transparent,
                    Tag = "hy" + i
                };
                pb.Click += huanYe_Click;
                this.Controls.Add(pb);
            }
        }
        #endregion

        #region 歌曲换页点击事件
        private void huanYe_Click(object sender, EventArgs e)
        {
            string sql = string.Empty;
            switch (((PictureBox)sender).Tag.ToString())
            {
                case"hy0":
                    if (index == 1)
                    {
                        return;
                    }
                    index--;
                    switch (state)
                    {
                        case 2:
                        case 5:
                        case 7:
                            sql = string.Format("select top {0} SongName,SingerName from Song_info inner join singer_info on singer_info.singerId = Song_info.singer where singer not in (select top ({0}*({1}-1)) singer from Song_info)", yiYe, index);
                            break;
                        case 1:
                            sql = string.Format("select top {0} Name,MovieUrl from Movie_info where Name not in (select top ({0}*({1}-1)) Name from Movie_info)", yiYe, index);
                            break;
                        case 3:
                            sql = string.Format("select top {0} SongName,SingerName from Song_info inner join singer_info on singer_info.singerId = Song_info.singer where SongName  in (( select SongName from (select ROW_NUMBER() OVER (ORDER BY TianJiaSJ desc) AS pos,SongName from Song_info) as T where T.pos >{0}* ( {1}- 1 ) AND T.pos <={0} * {1}))order by TianJiaSJ desc", yiYe, index);
                            break;
                        case 6:
                            sql = string.Format("select top {0} SongName,SingerName from Song_info inner join singer_info on singer_info.singerId = Song_info.singer where SongName  in (( select SongName from (select ROW_NUMBER() OVER (ORDER BY CiShu desc) AS pos,SongName from Song_info) as T where T.pos >{0}* ( {1}- 1 ) AND T.pos <={0} * {1}))order by CiShu desc", yiYe, index);
                            break;
                    }
                    ChaXunGeQu(sql);
                    break;
                case"hy1":
                    string a = "select CEILING(COUNT(*)/(" + yiYe + "*1.0)) from Song_info";
                    int i = Convert.ToInt32(DBHelper.GetScalar(a));
                    if (index == i)
                    {
                        return;
                    }
                    index++;
                    switch (state)
	                {
                        case 2:
                        case 5:
                        case 7:
                            sql = string.Format("select top {0} SongName,SingerName from Song_info inner join singer_info on singer_info.singerId = Song_info.singer where singer not in (select top ({0}*({1}-1)) singer from Song_info)", yiYe, index);
                            break;
                        case 1:
                            sql = string.Format("select top {0} Name,MovieUrl from Movie_info where Name not in (select top ({0}*({1}-1)) Name from Movie_info)", yiYe, index);
                            break;
                        case 3:
                            sql = string.Format("select top {0} SongName,SingerName from Song_info inner join singer_info on singer_info.singerId = Song_info.singer where SongName  in (( select SongName from (select ROW_NUMBER() OVER (ORDER BY TianJiaSJ desc) AS pos,SongName from Song_info) as T where T.pos >{0}* ( {1}- 1 ) AND T.pos <={0} * {1}))order by TianJiaSJ desc", yiYe, index);
                            break;
                        case 6:
                            sql = string.Format("select top {0} SongName,SingerName from Song_info inner join singer_info on singer_info.singerId = Song_info.singer where SongName  in (( select SongName from (select ROW_NUMBER() OVER (ORDER BY CiShu desc) AS pos,SongName from Song_info) as T where T.pos >{0}* ( {1}- 1 ) AND T.pos <={0} * {1}))order by CiShu desc", yiYe, index);
                            break;
	                }
                    ChaXunGeQu(sql);
                    break;
            }
        }
        #endregion

        #region 歌手分类点击查询方法
        private void lblb_Click(object sender, EventArgs e)
        {
            string sql0 = singerSql;
            switch (((MyLabel)sender).Tag.ToString())
            {
                case "dq0":
                    sql0 = singerSql;
                    moveSinger();
                    singerAdd(sql0);
                    pinYin();
                    break;
                case "dq1":
                    sql0 += " and SingerGuoJia = 1";
                    moveSinger();
                    singerAdd(sql0);
                    pinYin();
                    break;
                case "dq2":
                    sql0 += " and SingerGuoJia = 2";
                    moveSinger();
                    singerAdd(sql0);
                    pinYin();
                    break;
                case "dq3":
                    sql0 += " and SingerGuoJia = 4";
                    moveSinger();
                    singerAdd(sql0);
                    pinYin();
                    break;
                case "dq4":
                    sql0 += " and SingerGuoJia = 3";
                    moveSinger();
                    singerAdd(sql0);
                    pinYin();
                    break;
                case "dq5":
                    sql0 += " and SingerGuoJia = 6";
                    moveSinger();
                    singerAdd(sql0);
                    pinYin();
                    break;
            }
        }

        #endregion

        #region 歌星点歌界面加载方法

        private void singerAdd(string sql0)
        {
            SqlDataReader dr = DBHelper.GetReader(sql0);
            int i = 0;
            while (dr.Read())
            {
                PictureBox pic = new PictureBox()
                {
                    Image = Image.FromFile(Environment.CurrentDirectory + @"\ktv\singer0.png"),
                    Size = new Size(175, 247),
                    Location = new Point(100 + 215 * (i % 4), 100 + 250 * (i / 4)),
                    BackColor = Color.Transparent,
                    Tag = "*" + i
                };
                this.Controls.Add(pic);
                Label lb = new Label()
                {
                    AutoSize = false,
                    Size = new Size(137, 18),
                    BackColor = Color.Transparent,
                    Location = new Point(118 + 215 * (i % 4), 317 + 250 * (i / 4)),
                    Font = new Font("黑体", 12, FontStyle.Bold),
                    ForeColor = Color.Black,
                    Text = dr["SingerName"].ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Tag = "**" + i
                };
                this.Controls.Add(lb);
                lb.BringToFront();
                PictureBox pic1 = new PictureBox()
                {
                    Image = Image.FromFile(Environment.CurrentDirectory + @"\image\" + dr["SingerUrl"].ToString() + ""),
                    Size = new Size(139, 192),
                    Location = new Point(118 + 215 * (i % 4), 114 + 250 * (i / 4)),
                    BackColor = Color.Transparent,
                    Tag = "*" + i + "#" + dr["SingerName"]
                };
                pic1.Click += pic1_Click;
                this.Controls.Add(pic1);
                pic1.BringToFront();
                i++;
            }
            dr.Close();
            PictureBox pb = new PictureBox()
            {
                Image = Image.FromFile(Environment.CurrentDirectory + @"\ktv\dg2.png"),
                BackColor = Color.Transparent,
                Location = new Point(1010, 300),
                Size = new Size(323, 354),
                Tag = "***"
            };
            this.Controls.Add(pb);
            PictureBox picb = new PictureBox()
            {
                Image = Image.FromFile(Environment.CurrentDirectory + @"\ktv\singer1.png"),
                Size = new Size(922, 55),
                Location = new Point(100, 590),
                BackColor = Color.Transparent,
                Tag = "**",
            };
            this.Controls.Add(picb);
        }
        #endregion

        #region 点击歌手显示 歌手的歌曲
        void pic1_Click(object sender, EventArgs e)
        {
            string sql = string.Empty;
            switch (((PictureBox)sender).Tag.ToString().Substring(0,2))
            {
                case "*0":
                    sql = querySql;
                    sql += " and SingerName = '" + ((PictureBox)sender).Tag.ToString().Substring(((PictureBox)sender).Tag.ToString().IndexOf("#") + 1) + "'";
                    moveSinger();
                    dianGeBeiJingAdd();
                    pinYin();
                    ChaXunGeQu(sql);
                    break;
                case "*1":
                    sql = querySql;
                    sql += " and SingerName = '" + ((PictureBox)sender).Tag.ToString().Substring(((PictureBox)sender).Tag.ToString().IndexOf("#") + 1) + "'";
                    moveSinger();
                    dianGeBeiJingAdd();
                    pinYin();
                    ChaXunGeQu(sql);
                    break;
                case "*2":
                    sql = querySql;
                    sql += " and SingerName = '" + ((PictureBox)sender).Tag.ToString().Substring(((PictureBox)sender).Tag.ToString().IndexOf("#") + 1) + "'";
                    moveSinger();
                    dianGeBeiJingAdd();
                    pinYin();
                    ChaXunGeQu(sql);
                    break;
                case "*3":
                    sql = querySql;
                    sql += " and SingerName = '" + ((PictureBox)sender).Tag.ToString().Substring(((PictureBox)sender).Tag.ToString().IndexOf("#") + 1) + "'";
                    moveSinger();
                    dianGeBeiJingAdd();
                    pinYin();
                    ChaXunGeQu(sql);
                    break;
                case "*4":
                    sql = querySql;
                    sql += " and SingerName = '" + ((PictureBox)sender).Tag.ToString().Substring(((PictureBox)sender).Tag.ToString().IndexOf("#") + 1) + "'";
                    moveSinger();
                    dianGeBeiJingAdd();
                    pinYin();
                    ChaXunGeQu(sql);
                    break;
                case "*5":
                    sql = querySql;
                    sql += " and SingerName = '" + ((PictureBox)sender).Tag.ToString().Substring(((PictureBox)sender).Tag.ToString().IndexOf("#") + 1) + "'";
                    moveSinger();
                    dianGeBeiJingAdd();
                    pinYin();
                    ChaXunGeQu(sql);
                    break;
                case "*6":
                    sql = querySql;
                    sql += " and SingerName = '" + ((PictureBox)sender).Tag.ToString().Substring(((PictureBox)sender).Tag.ToString().IndexOf("#") + 1) + "'";
                    moveSinger();
                    dianGeBeiJingAdd();
                    pinYin();
                    ChaXunGeQu(sql);
                    break;
                case "*7":
                    sql = querySql;
                    sql += " and SingerName = '" + ((PictureBox)sender).Tag.ToString().Substring(((PictureBox)sender).Tag.ToString().IndexOf("#") + 1) + "'";
                    moveSinger();
                    dianGeBeiJingAdd();
                    pinYin();
                    ChaXunGeQu(sql);
                    break;
            }
        }
        #endregion

        #region 清除歌星点歌界面控件
        private void moveSinger()
        {
            for (int i = this.Controls.Count - 1; i >= 0; i--)
            {
                if (Convert.ToString(Controls[i].Tag).Length>1&& Convert.ToString(Controls[i].Tag).Substring(0, 1) == "*")
                {
                    this.Controls.RemoveAt(i);
                }
            }
        }
        #endregion

        #region 查询歌曲 提取方法
        private void ChaXunGeQu(string sql)
        {
            SqlDataReader dr = DBHelper.GetReader(sql);
            int i = 0;
            for (int j = this.Controls.Count - 1; j >= 0; j--)
            {
                if (this.Controls[j] is MyLabel && Convert.ToString(Controls[j].Tag).Substring(0,1) == "#")
                {
                    this.Controls.RemoveAt(j);
                }
            }
            while (dr.Read())
            {
                if (state == 1)
                {
                    MyLabel lbl1 = new MyLabel()
                    {
                        Size = new Size(565, 38),
                        Font = new Font("黑体", 20),
                        Text = dr["Name"].ToString(),
                        Location = new Point(270, 116 + 73 * (i / 1)),
                        Tag = "#" + dr["Name"].ToString()
                    };
                    this.Controls.Add(lbl1);
                    lbl1.BringToFront();
                    MyLabel lb1 = new MyLabel()
                    {
                        Size = new Size(110, 38),
                        Font = new Font("黑体", 20),
                        Text = "电 影",
                        Location = new Point(132, 116 + 73 * (i / 1)),
                        Tag = "#" + i
                    };
                    this.Controls.Add(lb1);
                    lb1.BringToFront();
                }
                else
                {
                    MyLabel lbl = new MyLabel()
                    {
                        Size = new Size(565, 38),
                        Font = new Font("黑体", 20),
                        Text = dr["SongName"] + " - " + dr["SingerName"],
                        Location = new Point(270, 116 + 73 * (i / 1)),
                        Tag = "#" + dr["SongName"].ToString()
                    };
                    lbl.Click+=Song_Click;
                    this.Controls.Add(lbl);
                    lbl.BringToFront();
                    MyLabel lb = new MyLabel()
                    {
                        Size = new Size(110, 38),
                        Font = new Font("黑体", 20),
                        Text = "演唱会",
                        Location = new Point(132, 116 + 73 * (i / 1)),
                        Tag = "#" + i
                    };
                    this.Controls.Add(lb);
                    lb.BringToFront();
                }
                
                i++;
            }
            dr.Close();
        }
        #endregion

        #region 点歌时候的方法
        private void Song_Click(object sender, EventArgs e)
        {
            string[] yd = new string[yiDian.Length + 1];
            
            for (int i = 0; i < yiDian.Length; i++)
            {
                yd[i] = yiDian[i];
            }
            yd[yiDian.Length] = ((MyLabel)sender).Tag.ToString();
            yiDian = yd;
            if (yiDian.Length == 1)
            {
                suiJiPlay();
                foreach (Control item in this.Controls)
                {
                    if (Convert.ToString(item.Tag) == "top1")
                    {
                        item.Text = yiDian[0].Substring(1);
                    }
                }
            }
            if (yiDian.Length > 1)
            {
                foreach (Control item1 in this.Controls)
                {
                    if (Convert.ToString(item1.Tag) == "top2")
                    {
                        item1.Text = yiDian[1].Substring(1);
                    }
                }
            }
        }
        #endregion

        #region 拼音界面 提取方法
        private void pinYin()
        {
            //添加拼音按钮
            string abc = "ABCDEFGHIGKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < 26; i++)
            {
                MyLabel lbl = new MyLabel()
                {
                    Size = new Size(35, 35),
                    Location = new Point(1031 + 48 * (i % 6), 398 + 50 * (i / 6)),
                    Tag = abc[i]
                };
                lbl.Click += lbl_Click;
                this.Controls.Add(lbl);
                lbl.BringToFront();
            }
            //添加搜索文本框
            Label ss = new Label()
            {
                Font = new Font("黑体", 18),
                ForeColor = Color.FromArgb(33, 33, 33),
                BackColor = Color.Transparent,
                Size = new Size(148, 23),
                Location = new Point(1038, 358),
                Tag = "ss"
            };
            ss.TextChanged += ss_TextChanged;
            this.Controls.Add(ss);
            ss.BringToFront();
            //添加清空回删按钮+
            MyLabel[] lb = new MyLabel[2];
            for (int i = 0; i < lb.Length; i++)
            {
                lb[i] = new MyLabel()
                {
                    Tag = "lb" + i,

                };
                lb[i].Click += lb_Click;
            }
            lb[0].Size = new Size(82, 33);
            lb[0].Location = new Point(1128, 596);
            lb[1].Size = new Size(82, 33);
            lb[1].Location = new Point(1225, 596);
            this.Controls.AddRange(lb);
            lb[0].BringToFront();
            lb[1].BringToFront();
        }
        #endregion

        #region 拼音点歌 文本改变搜索歌曲方法
        void ss_TextChanged(object sender, EventArgs e)
        {
            
            if (state != 4)
            {
                string sql = querySql;
                foreach (Control item in this.Controls)
                {
                    if (Convert.ToString(item.Tag) == "ss" && string.IsNullOrWhiteSpace(item.Text))
                    {
                        sql = querySql;
                        if (state == 3)
                        {
                            sql += " order by TianJiaSJ desc";
                        }
                        else if (state == 6)
                        {
                            sql += " order by CiShu desc";
                        }
                        ChaXunGeQu(sql);
                        return;
                    }
                    if (Convert.ToString(item.Tag) == "ss")
                    {
                        sql += " and SongSuoXie like '" + item.Text + "%'";
                        ChaXunGeQu(sql);
                    }
                }
            }
            else
            {
                string sql1 = singerSql;
                foreach (Control item in this.Controls)
                {
                    if (Convert.ToString(item.Tag) == "ss" && string.IsNullOrWhiteSpace(item.Text))
                    {
                        moveSinger();
                        singerAdd(sql1);
                        return;
                    }
                    if (Convert.ToString(item.Tag) == "ss")
                    {
                        sql1 += " and SingerSuoXie like '" + item.Text + "%'";
                        moveSinger();
                        singerAdd(sql1);
                    }
                }
            }
            
        }
        #endregion

        #region 清空，回删 点击事件
        private void lb_Click(object sender, EventArgs e)
        {
            switch (((MyLabel)sender).Tag.ToString())
            {
                case"lb0":
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "ss" && item.Text.Length>0)
                        {
                            item.Text = item.Text.Substring(0, item.Text.Length - 1);
                        }
                    }
                    break;
                case "lb1":
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "ss")
                        {
                            item.Text = "";
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 拼音 点击事件
        void lbl_Click(object sender, EventArgs e)
        {
            index = 1;
            foreach (Control item in this.Controls)
            {
                if (Convert.ToString(item.Tag) == "ss")
                {
                    item.Text += ((MyLabel)sender).Tag.ToString();
                }
            }
        }
        #endregion

        #region 点歌或电影的背景和拼音点歌提取方法
        private void dianGeBeiJingAdd()
        {
            PictureBox pb1 = new PictureBox()
            {
                Size = new Size(776, 519),
                Location = new Point(110, 100),
                Image = Image.FromFile(Environment.CurrentDirectory + @"\ktv\dg1.png"),
                BackColor = Color.Transparent
            };
            this.Controls.Add(pb1);
            SongFenLei();
            for (int i = 0; i < 4; i++)
            {
                MyLabel lb = new MyLabel()
                {
                    Size = new Size(60,23),
                    AutoSize = false,
                    BackColor = Color.Red,
                    Location = new Point(1025+77*(i%4),308),
                    Tag = "fl"+i
                };
                lb.Click +=fenlei_Click;
                this.Controls.Add(lb);
                lb.BringToFront();
            }
        }
        #endregion

        #region 歌曲分类点歌加载图片方法
        private void SongFenLei()
        {
            PictureBox[] pb = new PictureBox[4];
            for (int i = 0; i < pb.Length; i++)
            {
                pb[i] = new PictureBox()
                {
                    Image = Image.FromFile(Environment.CurrentDirectory + @"\ktv\dg" + (i + 2) + ".png"),
                    BackColor = Color.Transparent,
                    Tag = "*#" + i
                };
            }

            pb[0].Size = new Size(323, 354);
            pb[0].Location = new Point(1010, 300);
            pb[1].Size = new Size(323, 354);
            pb[1].Location = new Point(1000, 290);
            pb[2].Size = new Size(323, 354);
            pb[2].Location = new Point(1000, 290);
            pb[3].Size = new Size(323, 354);
            pb[3].Location = new Point(1000, 300);
            switch (st)
            {
                case 1:
                    pb[0].Visible = true;
                    pb[1].Visible = false;
                    pb[2].Visible = false;
                    pb[3].Visible = false;
                    break;
                case 2:
                    pb[0].Visible = false;
                    pb[1].Visible = false;
                    pb[2].Visible = true;
                    pb[3].Visible = false;
                    LeiXingDianGe();
                    break;
                case 3:
                    pb[0].Visible = true;
                    pb[1].Visible = false;
                    pb[2].Visible = false;
                    pb[3].Visible = false;
                    break;
                case 4:
                    pb[0].Visible = false;
                    pb[1].Visible = true;
                    pb[2].Visible = false;
                    pb[3].Visible = false;
                    break;
                case 5:
                    pb[0].Visible = false;
                    pb[1].Visible = false;
                    pb[2].Visible = false;
                    pb[3].Visible = true;
                    break;
                case 6:
                    pb[0].Visible = true;
                    pb[1].Visible = false;
                    pb[2].Visible = false;
                    pb[3].Visible = false;
                    break;
                case 7:
                    pb[0].Visible = true;
                    pb[1].Visible = false;
                    pb[2].Visible = false;
                    pb[3].Visible = false;
                    break;
            }
            this.Controls.AddRange(pb);
        }
        #endregion

        #region 分类菜单点击事件
        private void fenlei_Click(object sender, EventArgs e)
        {
            switch (((MyLabel)sender).Tag.ToString())
            {
                case"fl0":
                    st = 1;
                    for (int i = this.Controls.Count - 1; i >= 0; i--)
                    {
                        if (Convert.ToString(this.Controls[i].Tag).Length > 2 && Convert.ToString(this.Controls[i].Tag).Substring(0, 2) == "*#")
                        {
                            this.Controls.RemoveAt(i);
                        }
                    }
                    pinYin();
                    SongFenLei();
                    break;
                case "fl1":
                    st = 4;
                    for (int i = this.Controls.Count - 1; i >= 0; i--)
                    {
                        if (Convert.ToString(this.Controls[i].Tag).Length > 2 && Convert.ToString(this.Controls[i].Tag).Substring(0, 2) == "*#" || Convert.ToString(this.Controls[i].Tag).Length == 1 || Convert.ToString(this.Controls[i].Tag) == "ss" || Convert.ToString(this.Controls[i].Tag) == "lb0" || Convert.ToString(this.Controls[i].Tag) == "lb1")
                        {
                            this.Controls.RemoveAt(i);
                        }
                    }
                    SongFenLei();
                    ZiShuDianGe();
                    break;
                case "fl2":
                    st = 2;
                    for (int i = this.Controls.Count - 1; i >= 0; i--)
                    {
                        if (Convert.ToString(this.Controls[i].Tag).Length > 2 && Convert.ToString(this.Controls[i].Tag).Substring(0, 2) == "*#")
                        {
                            this.Controls.RemoveAt(i);
                        }
                    }
                    SongFenLei();
                    LeiXingDianGe();
                    break;
                case "fl3":
                    st = 5;
                    for (int i = this.Controls.Count - 1; i >= 0; i--)
                    {
                        if (Convert.ToString(this.Controls[i].Tag).Length > 2 && Convert.ToString(this.Controls[i].Tag).Substring(0, 2) == "*#")
                        {
                            this.Controls.RemoveAt(i);
                        }
                    }
                    SongFenLei();
                    for (int i = 0; i < 6; i++)
                    {
                        MyLabel lb44 = new MyLabel()
                        {
                            Size = new Size(105,30),
                            AutoSize = false,
                            BackColor = Color.Red,
                            Location = new Point(1037+158*(i%2),423+70*(i/2)),
                            Tag = "yuZhong"+i

                        };
                        lb44.Click += lb44_Click;
                        this.Controls.Add(lb44);
                        lb44.BringToFront();
                    }
                    break;
            }
        }
        #endregion

        #region 语种点歌点击事件
        void lb44_Click(object sender, EventArgs e)
        {
            index = 1;
            string sql = querySql;
            switch (((MyLabel)sender).Tag.ToString())
            {
                case"yuZhong0":
                    sql += " and Lan = 1";
                    ChaXunGeQu(sql);
                    break;
                case "yuZhong1":
                    sql += " and Lan = 2";
                    ChaXunGeQu(sql);
                    break;
                case "yuZhong2":
                    sql += " and Lan = 4";
                    ChaXunGeQu(sql);
                    break;
                case "yuZhong3":
                    sql += " and Lan = 3";
                    ChaXunGeQu(sql);
                    break;
                case "yuZhong4":
                    sql += " and Lan = 5";
                    ChaXunGeQu(sql);
                    break;
                case "yuZhong5":
                    sql += " and Lan = 6";
                    ChaXunGeQu(sql);
                    break;
            }
        }
        #endregion

        #region 类型点歌按钮添加
        private void LeiXingDianGe()
        {
            for (int i = 0; i < 6; i++)
            {
                MyLabel lb33 = new MyLabel()
                {
                    Size = new Size(109, 33),
                    AutoSize = false,
                    BackColor = Color.Red,
                    Location = new Point(1040 + 153 * (i % 2), 420 + 67 * (i / 2)),
                    Tag = "fenlei" + i
                };
                lb33.Click += lb33_Click;
                this.Controls.Add(lb33);
                lb33.BringToFront();
            }
        }
        #endregion

        #region 类型点歌点击方法
        void lb33_Click(object sender, EventArgs e)
        {
            index = 1;
            string sql = querySql;
            switch (((MyLabel)sender).Tag.ToString())
            {
                case "fenlei0":
                    sql += " and Type = 1";
                    ChaXunGeQu(sql);
                    break;
                case "fenlei1":
                    sql += " and Type = 2";
                    ChaXunGeQu(sql);
                    break;
                case "fenlei2":
                    sql += " and Type = 3";
                    ChaXunGeQu(sql);break;
                case "fenlei3":
                    sql += " and Type = 4";
                    ChaXunGeQu(sql);
                    break;
                case "fenlei4":
                    sql += " and Type = 5";
                    ChaXunGeQu(sql);
                    break;
                case "fenlei5":
                    sql += " and Type = 6";
                    ChaXunGeQu(sql);
                    break;
            }
        }
        #endregion

        #region 字数点歌按钮添加
        private void ZiShuDianGe()
        {
            for (int i = 0; i < 12; i++)
            {
                MyLabel lb22 = new MyLabel()
                {
                    Size = new Size(74, 32),
                    AutoSize = false,
                    BackColor = Color.Red,
                    Location = new Point(1037 + 96 * (i % 3), 418 + 57 * (i / 3)),
                    Tag = "ziShu" + i
                };
                lb22.Click += lb22_Click;
                this.Controls.Add(lb22);
                lb22.BringToFront();
            }
        }
        #endregion

        #region 字数点歌点击方法
        void lb22_Click(object sender, EventArgs e)
        {
            index = 1;
            string sql = querySql;
            switch (((MyLabel)sender).Tag.ToString())
            {
                case "ziShu0":
                    sql += " and SongZiShu = 1";
                    ChaXunGeQu(sql);
                    break;
                case "ziShu1":
                    sql += " and SongZiShu = 2";
                    ChaXunGeQu(sql);
                    break;
                case "ziShu2":
                    sql += " and SongZiShu = 3";
                    ChaXunGeQu(sql);
                    break;
                case "ziShu3":
                    sql += " and SongZiShu = 4";
                    ChaXunGeQu(sql);
                    break;
                case "ziShu4":
                    sql += " and SongZiShu = 5";
                    ChaXunGeQu(sql);
                    break;
                case "ziShu5":
                    sql += " and SongZiShu = 6";
                    ChaXunGeQu(sql);
                    break;
                case "ziShu6":
                    sql += " and SongZiShu = 7";
                    ChaXunGeQu(sql);
                    break;
                case "ziShu7":
                    sql += " and SongZiShu = 8";
                    ChaXunGeQu(sql);
                    break;
                case "ziShu8":
                    sql += " and SongZiShu = 9";
                    ChaXunGeQu(sql);
                    break;
                case "ziShu9":
                    sql += " and SongZiShu > 9";
                    ChaXunGeQu(sql);
                    break;
                case "ziShu11":
                    ChaXunGeQu(sql);
                    break;
            }
        }
        #endregion

        #region 清除lable picturebox控件 提取方法
        private void moveControls()
        {
            for (int i = this.Controls.Count - 1; i >= 0; i--)
            {
                if (this.Controls[i] is PictureBox || this.Controls[i] is Label || this.Controls[i] is MyLabel)
                {
                    this.Controls.RemoveAt(i);
                }
            }
        }
        #endregion

        #region 主界面加载的提取方法
        private void mainJieMian()
        {

            this.BackgroundImage = Image.FromFile(Environment.CurrentDirectory + @"\ktv\bj4.png");

            #region 主页面picturebox按钮动态添加（暂停icon0，音量icon1）
            for (int i = 0; i < 2; i++)
            {
                PictureBox pb = new PictureBox()
                {
                    Size = new Size(70, 70),
                    Location = new Point(830 + (i * 100), 692),
                    Image = Image.FromFile(Environment.CurrentDirectory + @"\ktv\icon\" + i + ".png"),
                    BackColor = Color.Transparent,
                    Tag = "icon" + i
                };
                pb.Click += pb_Click;
                this.Controls.Add(pb);
            }
            #endregion

            #region 主页面picturebox按钮动态添加（继续icon00，音量icon01）
            for (int i = 0; i < 2; i++)
            {
                PictureBox pb1 = new PictureBox()
                {
                    Size = new Size(70, 70),
                    Location = new Point(830 + (i * 100), 692),
                    Image = Image.FromFile(Environment.CurrentDirectory + @"\ktv\icon\0" + i + ".png"),
                    BackColor = Color.Transparent,
                    Tag = "icon0" + i,
                    Visible = false
                };
                pb1.Click += pb1_Click;
                this.Controls.Add(pb1);
            }
            #endregion

            #region 主页动态添加label（时间top0，正在播放top1，下一首top2）
            Label[] top = new Label[3];
            for (int i = 0; i < top.Length; i++)
            {
                top[i] = new Label()
                {
                    Font = new Font("黑体", 26),
                    ForeColor = Color.FromArgb(200, 200, 200),
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    Tag = "top" + i
                };
            }
            top[0].Location = new Point(70, 10);
            top[1].Location = new Point(475, 14);
            top[1].Font = new System.Drawing.Font("黑体",20);
            top[2].Location = new Point(840, 14);
            top[2].Font = new System.Drawing.Font("黑体", 20);
            #endregion

            #region 动态添加Timer 显示时间
            Timer tm = new Timer()
            {
                Interval = 1000,
                Enabled = true
            };
            tm.Tick += tm_Tick;
            #endregion

            #region 主页动态添加label假button （已选，已唱，主页，重唱，切歌，声音加减）
            Label[] lblbtn = new Label[7];
            for (int i = 0; i < lblbtn.Length; i++)
            {
                lblbtn[i] = new Label()
                {
                    AutoSize = false,
                    BackColor = Color.Transparent,
                    Tag = "lblbtn"+i
                };
                lblbtn[i].Click+=lblbtn_Click;
            }
            lblbtn[0].Size = new Size(87, 58);
            lblbtn[0].Location = new Point(1114, 0);
            lblbtn[1].Size = new Size(87, 58);
            lblbtn[1].Location = new Point(1207, 0);
            lblbtn[2].Size = new Size(70, 70);
            lblbtn[2].Location = new Point(90, 692);
            lblbtn[3].Size = new Size(70, 70);
            lblbtn[3].Location = new Point(1110, 692);
            lblbtn[4].Size = new Size(70, 70);
            lblbtn[4].Location = new Point(1215, 692);
            lblbtn[5].Size = new Size(50, 50);
            lblbtn[5].Location = new Point(600, 702);
            lblbtn[6].Size = new Size(50, 50);
            lblbtn[6].Location = new Point(720, 702);
            #endregion

            #region 每1秒判断mediaplayer的状态
            Timer t = new Timer()
            {
                Interval = 1000,
                Enabled = true
            };
            t.Tick += t_Tick;
            #endregion

            this.Controls.AddRange(lblbtn);
            this.Controls.AddRange(top);

            Panel p = new Panel()
            {
                AutoSize = false,
                Size = new Size(446,528),
                BackColor = Color.Transparent,
                Location = new Point(917,62),
                BackgroundImage = Image.FromFile(Environment.CurrentDirectory + @"\ktv\5156.png"),
                Visible = false
            };
            this.Controls.Add(p);
            p.BringToFront();
        }
        #endregion

        #region 主页面label点击事件 （已选，已唱，主页，重唱，切歌，声音加减）
        int cli;
        int ilc;
        private void lblbtn_Click(object sender, EventArgs e)
        {
            cli++;
            ilc++;
            switch (((Label)sender).Tag.ToString())
            {
                case "lblbtn0":
                    
                    foreach (Control item in this.Controls)
                    {
                        if (item is Panel&&cli%2==1)
                        {
                            item.Visible = true;
                            item.Controls.Clear();
                            item.BringToFront();
                            for (int ii = 0; ii < yiDian.Length; ii++)
                            {
                                Label llbb = new Label()
                                {
                                    Size = new Size(260,30),
                                    AutoSize = false,
                                    Location = new Point(35,30 + 45*(ii/1)),
                                    BackColor = Color.Transparent,
                                    Font = new Font("黑体",20),
                                    ForeColor = Color.White,
                                    Text = yiDian[ii].Substring(1)
                                };
                                item.Controls.Add(llbb);
                                llbb.BringToFront();
                            }
                        }
                        if (item is Panel && cli %2 == 0)
                        {
                            item.Visible = false;
                        }
                    }
                    break;
                case "lblbtn1":
                    
                    foreach (Control item in this.Controls)
                    {
                        if (item is Panel && ilc % 2 == 1)
                        {
                            item.Visible = true;
                            item.Controls.Clear();
                            for (int iii = 0; iii < yiChang.Length; iii++)
                            {
                                Label llbb = new Label()
                                {
                                    Size = new Size(260, 30),
                                    AutoSize = false,
                                    Location = new Point(35, 30 + 45 * (iii / 1)),
                                    BackColor = Color.Transparent,
                                    Font = new Font("黑体", 20),
                                    ForeColor = Color.White,
                                    Text = yiChang[iii].Substring(1)
                                };
                                item.Controls.Add(llbb);
                                llbb.BringToFront();
                            }
                        }
                        if (item is Panel && ilc % 2 == 0)
                        {
                            item.Visible = false;
                        }
                    }
                    break;
                case "lblbtn2":
                    zhuYe();
                    break;
                case "lblbtn3":
                    mp.Ctlcontrols.stop();
                    mp.Ctlcontrols.play();
                    break;
                case "lblbtn4":
                    if (yiDian.Length == 0)
                    {
                       suiJiPlay();
                       GetSuiJi();
                    }
                    else
                    {
                        string a = yiDian[0];
                        string[] yc = new string[yiChang.Length + 1];
                        string[] b = new string[yiDian.Length - 1];
                        for (int i = 0; i < b.Length; i++)
                        {
                            b[i] = yiDian[i + 1];
                        }
                        for (int i = 0; i < yiChang.Length; i++)
                        {
                            yc[i] = yiChang[i];
                        }
                        yc[yiChang.Length] = a;
                        yiChang = yc;
                        yiDian = b;
                        
                        suiJiPlay();
                        if (yiDian.Length>=1)
                        {
                            foreach (Control item in this.Controls)
                            {
                                if (Convert.ToString(item.Tag) == "top1")
                                {
                                    item.Text = yiDian[0].Substring(1);
                                }
                            }
                        }
                        
                        if (yiDian.Length >1)
                        {
                            foreach (Control item1 in this.Controls)
                            {
                                if (Convert.ToString(item1.Tag) == "top2")
                                {
                                    item1.Text = yiDian[1].Substring(1);
                                }
                            }
                        }
                        else
                        {
                            suiJiPlay();
                            foreach (Control item1 in this.Controls)
                            {
                                if (Convert.ToString(item1.Tag) == "top2")
                                {
                                    item1.Text = "";
                                }
                            }
                        }
                    }
                    
                   
                    break;
                case "lblbtn5":
                    mp.settings.volume -= 10;
                    break;
                case "lblbtn6":
                    mp.settings.volume += 10;
                    break;
                default:
                    break;
            }
        }

        private void suiJiPlay()
        {
            Random r = new Random();
            int i = r.Next(1, 6);
            mp.URL = Environment.CurrentDirectory + @"\mv\mv" + i + ".mp4";
        }
        #endregion

        #region  按钮继续 静音 显示方法
        void pb1_Click(object sender, EventArgs e)
        {
            switch (((PictureBox)sender).Tag.ToString())
            {
                case "icon00":
                    mp.Ctlcontrols.play();
                    ((PictureBox)sender).Visible = false;
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "icon0")
                        {
                            item.Visible = true;
                        }
                    }
                    break;
                case "icon01":
                    ((PictureBox)sender).Visible = false;
                    mp.settings.mute = false;
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "icon1")
                        {
                            item.Visible = true;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region  按钮暂停 静音 显示方法
        void pb_Click(object sender, EventArgs e)
        {
            switch (((PictureBox)sender).Tag.ToString())
            {
                case "icon0":
                    mp.Ctlcontrols.pause();
                    ((PictureBox)sender).Visible = false;
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "icon00")
                        {
                            item.Visible = true;
                        }
                    }
                    break;
                case "icon1":
                    ((PictureBox)sender).Visible = false;
                    mp.settings.mute = true;
                    foreach (Control item in this.Controls)
                    {
                        if (Convert.ToString(item.Tag) == "icon01")
                        {
                            item.Visible = true;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region 主页面分类选择鼠标离开 图片移动方法
        private void fenLei_MouseLeave(object sender, EventArgs e)
        {
            switch (((PictureBox)sender).Tag.ToString())
            {
                case "fenLei0":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X - 2, ((PictureBox)sender).Location.Y - 2);
                    break;
                case "fenLei1":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X - 2, ((PictureBox)sender).Location.Y - 2);
                    break;
                case "fenLei2":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X - 2, ((PictureBox)sender).Location.Y - 2);
                    break;
                case "fenLei3":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X - 2, ((PictureBox)sender).Location.Y - 2);
                    break;
                case "fenLei4":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X - 2, ((PictureBox)sender).Location.Y - 2);
                    break;
                case "fenLei5":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X - 2, ((PictureBox)sender).Location.Y - 2);
                    break;
                case "fenLei6":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X - 2, ((PictureBox)sender).Location.Y - 2);
                    break;
                case "fenLei7":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X - 2, ((PictureBox)sender).Location.Y - 2);
                    break;
            }
        }

        #endregion

        #region 主页面分类选择鼠标进入 图片移动方法
        private void fenLei_MouseEnter(object sender, EventArgs e)
        {
            switch (((PictureBox)sender).Tag.ToString())
            {
                case "fenLei0":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X + 2, ((PictureBox)sender).Location.Y + 2);
                    break;
                case "fenLei1":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X + 2, ((PictureBox)sender).Location.Y + 2);
                    break;
                case "fenLei2":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X + 2, ((PictureBox)sender).Location.Y + 2);
                    break;
                case "fenLei3":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X + 2, ((PictureBox)sender).Location.Y + 2);
                    break;
                case "fenLei4":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X + 2, ((PictureBox)sender).Location.Y + 2);
                    break;
                case "fenLei5":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X + 2, ((PictureBox)sender).Location.Y + 2);
                    break;
                case "fenLei6":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X + 2, ((PictureBox)sender).Location.Y + 2);
                    break;
                case "fenLei7":
                    ((PictureBox)sender).Location = new Point(((PictureBox)sender).Location.X + 2, ((PictureBox)sender).Location.Y + 2);
                    break;
            }
        }
        #endregion

        #region 时钟方法
        void tm_Tick(object sender, EventArgs e)
        {
            foreach (Control item in this.Controls)
            {
                if (Convert.ToString(item.Tag) == "top0")
                {
                    item.Text = DateTime.Now.ToString("HH:mm");
                }
            }
        }
        #endregion

    }
}
