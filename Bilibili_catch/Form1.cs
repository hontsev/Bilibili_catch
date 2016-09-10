using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;

namespace Bilibili_catch
{
    public partial class Form1 : Form
    {
        private string xmlid;
        private FileStream output;
        private StreamWriter w;
        private Boolean isrun=false;
        private int sum1 = 0;
        private int sum2 = 0;
        public Form1()
        {
            xmlid = "";
            InitializeComponent();
            wb.ScriptErrorsSuppressed = true;
        }
        private void printline(string str)
        {
            textBox1.Text = textBox1.Text + str + "\r\n";
            textBox1.Select(textBox1.TextLength, 0);
            textBox1.ScrollToCaret();
        }
        private void print(string str)
        {
            textBox1.Text = textBox1.Text + str;
            textBox1.Select(textBox1.TextLength, 0);
            textBox1.ScrollToCaret();
        }
        private void printstate(string str)
        {
            label4.Text = str;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //开始扫
            xmlid = textBox2.Text;
            printline(xmlid);
            printline("_____开始_____");
            printstate("扫描中..");
            sum1 = sum2 = 0;
            output = new FileStream("output.txt", FileMode.Create, FileAccess.Write);
            w = new StreamWriter(output);
            isrun = true;
            int tid = Int32.Parse(textBox2.Text);
            int times = Int32.Parse(textBox3.Text);
            while(isrun==true && times-- > 0)
            {
                getxmlinfo("http://comment.bilibili.tv/" + tid + ".xml");
                tid++;
                sum1++;
            }
            printline("_____结束_____");
            printline("共计" + sum1 + "个视频，" + sum2 + "条弹幕。已经存入exe文件同目录下的output.txt中。请查收_(:з」∠)_");
            printstate("扫描完毕。共计" + sum1 + "个视频，" + sum2 + "条弹幕。");
            textBox2.Text = tid.ToString();
            w.Close();
            output.Close();
        }
        private void getxmlinfo(string url)
        {
            string res = "";
            wb.Navigate(url);
            while (wb.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();
            res = wb.DocumentText;
            HtmlDocument doc = wb.Document;
            //Regex reg = new Regex(@"</B><SPAN class=""m"">""</SPAN><SPAN class=""m"">&gt;</SPAN><SPAN class=""tx"">([^<]*)</SPAN>");
            MatchCollection mc = Regex.Matches(res, @"</B><SPAN class=""m"">""</SPAN><SPAN class=""m"">&gt;</SPAN><SPAN class=""tx"">([^<]*)</SPAN>", RegexOptions.IgnoreCase);  
            string[] content = new string[mc.Count];
            for(int i=0;i<content.Length;i++)
            {
                content[i] = mc[i].Groups[1].Value;
                w.WriteLine(content[i]);
                sum2++;
                Application.DoEvents();
            }
            printline("finish:"+(sum1+1).ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //停止扫
            isrun = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //按视频id获取弹幕id
            string vid = textBox4.Text;
            string url = "http://www.bilibili.com/video/av"+vid;
            //string url = "http://www.baidu.com";
            string res = "";
            wb.Navigate(url);
            while (wb.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            res = wb.DocumentText;
            HtmlDocument doc = wb.Document;
            MatchCollection mc = Regex.Matches(res, @"cid=([^<]*)&aid=", RegexOptions.IgnoreCase);
            if(mc.Count>0)
            {
                res = mc[0].Groups[1].Value;
                textBox5.Text = res;
                printstate("获取弹幕id完毕");
            }
            else
            {
                textBox5.Text = "";
                printstate("获取弹幕id失败");
            }
        }
    }
}
