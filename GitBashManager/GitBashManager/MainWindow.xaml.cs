using LitJson;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Shapes;

namespace GitBashManager
{
    /// <summary>
    /// 用于接收Json数据
    /// </summary>
    public class PathInfo
    {
        public string blogPath = string.Empty;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isJsonData = false;

        public MainWindow()
        {
            InitializeComponent();

            // 初始化
            // 启动程序时检测Json数据是否存在
            isJsonData = CheckFile(AppDomain.CurrentDomain.BaseDirectory, "PathData.json");
            if (isJsonData)
            {
                string jsonStr = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/PathData.json");
                PathInfo info = JsonMapper.ToObject<PathInfo>(jsonStr);
                TextBoxPath.Text = info.blogPath;
            }
        }

        // 检查是否存在Json数据
        public bool CheckFile(string path, string fileName)
        {
            DirectoryInfo jsonFile = new DirectoryInfo(path);

            foreach (FileInfo file in jsonFile.GetFiles())
            {
                if (file.Name == fileName)
                    return true;
            }

            return false;
        }

        // 弹窗提示
        public void ShowTips(string info, bool quit = false)
        {
            MessageBoxButton button = MessageBoxButton.OKCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;

            string messageBoxText = info;
            string caption = "提示";

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
            switch (result)
            {
                case MessageBoxResult.OK:
                    break;
                case MessageBoxResult.Cancel:
                    if (quit)
                        Application.Current.Shutdown();
                    break;
            }
        }

        // 执行git命令
        public void CmdCommand(string command, Action? callBack = null)
        {
            string blog = TextBoxPath.Text;

            Process process = new Process();
            // 设置要启动的应用程序
            process.StartInfo.FileName = "cmd.exe";
            // 接受来自调用程序的输入信息
            process.StartInfo.RedirectStandardInput = true;
            // 指定文件夹
            process.StartInfo.WorkingDirectory = blog;
            // 启动程序
            process.Start();
            // 向cmd窗口发送输入信息
            process.StandardInput.WriteLine(command + " & exit");
            process.StandardInput.AutoFlush = true;
            // 等待程序执行完退出进程
            process.WaitForExit();
            process.Close();
            // 执行回调函数
            if (callBack != null)
                callBack();
        }

        //快速浏览
        private void BtnSView_Clik(object sender, RoutedEventArgs e)
        {
            if (isJsonData == false)
                ShowTips("你还没有设置Git路径和博客路径。");
            else
                CmdCommand("hexo s");
        }

        //本地浏览
        private void BtnView_Clik(object sender, RoutedEventArgs e)
        {
            if (isJsonData == false)
                ShowTips("你还没有设置Git路径和博客路径。");
            else
                CmdCommand("hexo clean && hexo g && hexo s");

        }

        // 博客部署
        private void BtnDeploy_Clik(object sender, RoutedEventArgs e)
        {
            if (isJsonData == false)
                ShowTips("你还没有设置Git路径和博客路径。");
            else
                CmdCommand("hexo clean && hexo g && hexo d");
        }

        // 添加文章
        private void BtnNewPost_Clik(object sender, RoutedEventArgs e)
        {
            if (isJsonData == false)
                ShowTips("你还没有设置Git路径和博客路径。");
            else if (TextBoxPostName.Text != null)
            {
                CmdCommand("hexo new " + TextBoxPostName.Text, () =>
                {
                    ShowTips("文章创建成功，文章名：" + TextBoxPostName.Text);
                    
                    string path = string.Empty;
                    string blogPath = TextBoxPath.Text;
                    if (blogPath[blogPath.Length - 1] == '\\')
                        path = blogPath + "source\\_posts\\";
                    else
                        path = blogPath + "\\source\\_posts\\";

                    CmdCommand(path + TextBoxPostName.Text + ".md");
                });
            }
            else
                ShowTips("需要填入文章名");
        }

        // 保存
        private void BtnSave_Clik(object sender, RoutedEventArgs e)
        {
            if (TextBoxPath.Text == null)
            {
                ShowTips("路径不能留空");
                return;
            }

            if(isJsonData == false)
            {
                PathInfo info = new PathInfo();
                info.blogPath = TextBoxPath.Text;
                string str = JsonMapper.ToJson(info);
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/PathData.json", str);

                ShowTips("数据保存成功，保存路径：" + AppDomain.CurrentDomain.BaseDirectory + "PathData.json");

                isJsonData = true;
            }
            else
            {
                PathInfo info = new PathInfo();
                info.blogPath = TextBoxPath.Text;
                string str = JsonMapper.ToJson(info);
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/PathData.json", str);
                
                ShowTips("数据更新成功，保存路径：" + AppDomain.CurrentDomain.BaseDirectory + "PathData.json");
            }
        }

        // 打开文章文件夹
        private void BtnOpen_Clik(object sender, RoutedEventArgs e)
        {
            string path = TextBoxPath.Text;
            if (path[path.Length - 1] == '\\')
                path += "source\\_posts";
            else
                path += "\\source\\_posts";

            CmdCommand("start " + path);
        }

        // 打开博客根目录
        private void BtnOpenRoot_Clik(object sender, RoutedEventArgs e)
        {
            CmdCommand("start " + TextBoxPath.Text);
        }


    }
}
