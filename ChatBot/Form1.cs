using System;
using System.Diagnostics;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using YoutubeExtractor;
using System.IO;
namespace ChatBot
{
    public partial class ChatBot : Form
    {
        //Speech Synthesizer Class
        SpeechSynthesizer s = new SpeechSynthesizer();
        //Commands List
        Choices list = new Choices();

        Boolean Wakeup = true;
        String temp;
        String condition;
        public int time;
        Boolean Search = false;
        Boolean Search_Youtube = false;
        public String[] Greet = new String[4] { "Hey", "Hello", "Hi", "Hey How Are you" };
        public Boolean nullvalue = false;
        public ChatBot()
        {
            SpeechRecognitionEngine r = new SpeechRecognitionEngine();
            //Commands Recognization.
            list.Add(File.ReadAllLines(@"F:\ChatBotCommands\Commands.txt"));

            // Grammer Class
            Grammar gr = new Grammar(new GrammarBuilder(list));
            // Serial Port Class
            // SerialPort SP = new SerialPort("COM4",9600,Parity.None,8,StopBits.One);
            //try Block

            try
            {
                // Speech Recognization 
                r.RequestRecognizerUpdate();
                r.LoadGrammar(gr);
                r.SpeechRecognized += r_SpeechRecognized;
                r.SetInputToDefaultAudioDevice();
                r.RecognizeAsync(RecognizeMode.Multiple);

            }
            catch
            {
                return;
            }

            s.SelectVoiceByHints(VoiceGender.Male);
            s.Speak("Hello,My Name Is Vin..........Enter Your Name And Age");
            InitializeComponent();
        }
        public void Restart()
        {
            Say("Vin Will Going To Be Updated");
            Process.Start(@"F:\Codes\MyProject\ChatBot\ChatBot\bin\Debug\ChatBot.exe");
            Environment.Exit(0);
        }
        public void Say(String Saying)
        {
            // s.Speak(Saying);
            s.SpeakAsync(Saying);
            for (time = 0; time < 3; time++)
            {
                textBox1.AppendText(Saying + "\n");
            }

        }



        //Get Weather 
        public String GetWeather(String input)
        {
            String query = String.Format("https://query.api.openweathermap.org/data/2.5/weather?q=Jaipur");
            XmlDocument wData = new XmlDocument();
            try
            {
                wData.Load(query);
            }
            catch
            {
                MessageBox.Show("No Internet Connection");
                return "No Internet Connection";
            }

            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            manager.AddNamespace("OpenWeatherMap", "http://api.openweathermap.org/data/2.5/weather?q=London");

            XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
            XmlNodeList nodes = wData.SelectNodes("query/results/channel");
            try
            {
                temp = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value;
                condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
                if (input == "temp")
                {
                    return temp;
                }
                if (input == "cond")
                {
                    return condition;
                }
            }
            catch
            {
                return "Error Reciving data";
            }
            return "error";
        }


        //Greetings Randomly
        public String Voice_Greet()
        {
            Random rnd = new Random();
            return Greet[rnd.Next(4)];
        }



        //Killing Opened Processes
        public static void killProg(String close)
        {
            System.Diagnostics.Process[] procs = null;
            try
            {
                procs = Process.GetProcessesByName(close);

                Process programs = procs[0];

                if (!programs.HasExited)
                {
                    programs.Kill();
                }
            }
            finally
            {
                if (procs != null)
                {
                    foreach (Process p in procs)
                    {
                        p.Dispose();
                    }
                }
            }

        }




        //Speech Recognizations
        private void r_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String result = e.Result.Text;
            String name = textBox3.Text;
            String age = textBox4.Text;

            //Wakeup & Sleep
            if (result == "wakeup"||result=="awake")
            {
                Say("Im Awake");
                Wakeup = true;
                label1.Text = "State:Awake Mode";
            }
            if (result == "sleep")
            {
                Say("Im Sleep");
                Wakeup = false;
                label1.Text = "State:Sleep Mode";
            }
            //Search on Google
            if (Search)
            {
                Process.Start("https://www.google.co.in/search?q=" + result + "&oq=" + result + "&aqs=chrome..69i57l2j69i59l2j5l2.818j0j8&sourceid=chrome&ie=UTF-8");
                Search = false;
            }
            //Search on Youtube
            if (Search_Youtube)
            {
                webBrowser1.Navigate("https://www.youtube.com/results?search_query=" + result);
                    Search_Youtube = false;
            }

            if (Wakeup == true && Search == false && Search_Youtube == false)
            {
                if (result == "restart" || result == "update")
                {
                    Restart();
                }

                if (result == "hello")
                {
                    Say("Hi have a nice day");
                }
                if (result == "how are you")
                {
                    Say("Im Good, how About you");
                }
                if (result == "what time is it" || result == "what is the time")
                {
                    Say(DateTime.Now.ToString("h:mm:ss tt"));
                }
                if (result == "what is today" || result == "what is date")
                {
                    Say(DateTime.Now.ToString("M/d/yyyy"));
                }
               if (result == "open google")
                {
                    Process.Start("http://google.com");
                }
                
                if (result == "open monodevelop")
                {
                    Process.Start(@"C:\Program Files\Unity\MonoDevelop\bin\MonoDevelop.exe");
                }
                if (result == "close monodevelop")
                {
                    killProg("MonoDevelop");
                }
                if (result == "whats the weather")
                {
                    Say("The Weather Is " + GetWeather("cond") + ".");
                }
                if (result == "whats the temperature")
                {
                    Say(GetWeather("temp"));
                }
                if (result == "hi")
                {
                    Say(Voice_Greet());
                }
                if (result == "minimize")
                {
                    this.WindowState = FormWindowState.Minimized;
                }
                if (result == "maximize")
                {
                    this.WindowState = FormWindowState.Maximized;

                }
                if (result == "normalize")
                {
                    this.WindowState = FormWindowState.Normal;
                }
                if (result == "open vlc")
                {
                    Process.Start(@"C:\Program Files (x86)\VideoLAN\VLC\vlc.exe");
                }
                if (result == "close vlc")
                {
                    killProg("vlc");
                }
                if (result == "play" || result == "pause")
                {
                    SendKeys.Send(" ");
                }
                if (result == "next")
                {
                    SendKeys.Send("^{RIGHT}");
                }
                if (result == "previous")
                {
                    SendKeys.Send("^{LEFT}");
                }
                if (result == "close chatbot")
                {
                    killProg("Chatbot.exe");
                }
                if (result == "search")
                {
                    Say("Say What You want to search");
                    Search = true;
                }
                if (result == "search for youtube"||result=="youtube")
                {
                    Say("Say What You Want To Search ");
                    Search_Youtube = true;
                }
                if (result == "close google")
                {
                    killProg("chrome");
                }
                if (result == "open unity")
                {
                    Process.Start(@"C:\Program Files\Unity\Editor\Unity.exe");
                }
                if (result == "close unity")
                {
                    killProg("Unity");
                }
                if (result == "open settings")
                {
                    Process.Start(@"C:\Windows\ImmersiveControlPanel\SystemSettings.exe");
                }
                if (result == "close settings")
                {
                    killProg("SystemSettings");
                }
                if (result == "open fileexplorer")
                {
                    Process.Start(@"C:\Windows\File Explorer.exe");
                }
                if (result == "close fileexplorer")
                {
                    killProg("explorer");
                }
                if (result == "play gta4")
                {
                    Process.Start(@"F:\Games\Mr DJ\Grand Theft Auto IV\LaunchGTAIV.exe");
                }
                if (result == "quit gta4")
                {
                    killProg("GTAIV");
                }
                if (result == "play farcry2")
                {
                    Process.Start(@"C:\Program Files (x86)\Mr DJ\Far Cry 2 Fortune's Edition\bin\farcry2.exe");
                }
                if (result == "quit farcry2")
                {
                    killProg("farcry2");
                }
                if (result == "open visual studio")
                {
                    Process.Start(@"F:\VisualStudio\Common7\IDE\devenv.exe");
                }
                if (result == "close visual studio")
                {
                    killProg("devenv");
                }
                if (result == "open antivirus")
                {
                    Process.Start(@"C:\Program Files\Quick Heal\Quick Heal Total Security.exe");
                }
                if (result == "close antivirus")
                {
                    killProg("SCANNER");
                }
            }
            if (result == "hey vin book a ticket")
            {
                Process.Start(@"https://www.google.co.in/search?q=bookmyshow&oq=bookmyshow&aqs=chrome..69i57l2j69i59l2j5l2.818j0j8&sourceid=chrome&ie=UTF-8");
            }
            if (result == "hey vin order a pizza")
            {
                Process.Start(@"https://www.google.co.in/search?q=Dominos&oq=Dominos&aqs=chrome..69i57l2j69i59l2j5l2.818j0j8&sourceid=chrome&ie=UTF-8");
            }
            if (result == "what i do")
            {
                Process.Start(@"https://www.google.co.in/search?q=whatido&oq=whatido&aqs=chrome..69i57l2j69i59l2j5l2.818j0j8&sourceid=chrome&ie=UTF-8");
            }
            if (result == "who you are")
            {
                Say("Im Vin   the Voice Bot    Version 1.0");
            }
           if(textBox3.Text=="")
            {
                Say("Please Enter Your Name First");
            }
            if (result == "refresh")
            {
                webBrowser1.Refresh();
            }
            if (textBox4.Text=="")
            {
                Say("Please Enter Your Age First");
            }
            if (result == "whats my name")
            {
                Say("Your name is " + name);
            }
            if (result == "whats my age")
            {
                Say(age + " is your age");
            }
            if (result == "stop")
            {
                webBrowser1.Stop();
            }
            if (result == "download video" || result == "download")
            {
                videoDownload();
            }
            for (time = 0; time < 3; time++)
            {
                textBox2.AppendText(result + "\n");
            }
          }
        //VideoDownload
        void videoDownload()
        {
            try { 
            IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(UrlTextBox.Text);
            VideoInfo video = videos.First(p => p.VideoType == VideoType.Mp4 && p.Resolution == Convert.ToInt32(480));
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }
            VideoDownloader down = new VideoDownloader(video, Path.Combine("F:\\ChromeDownloads\\" + Environment.UserName + "\\Videos\\", video.Title + video.VideoExtension));
            down.DownloadProgressChanged += Down_DownloadProgressChanged;
            
                Thread t = new Thread(() => { down.Execute(); }) { IsBackground = true };
                t.Start();
            }
            catch { return; }
        }
        private void Down_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
               DownloadStatus.Text = $"Downloading:{string.Format("{0:0.##}", e.ProgressPercentage)}%";
                if (DownloadStatus.Text == "Downloading:100%")
                {
                    DownloadStatus.Text = "";
                }
            }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void UserLabel_Click(object sender, EventArgs e)
        {

        }

            private void button1_Click(object sender, EventArgs e)
            {

                System.Threading.Thread mythread = new System.Threading.Thread(new System.Threading.ThreadStart(OpenFrom));

                mythread.Start();

                this.Close();

            }
            public static void OpenFrom()
            {

                Application.Run(new Form2());//Create an instance of your new form. No need to call show method.

            }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
             
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
         
        }

        private void webBrowser1_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            try
            {
                Progressbar1.Value = Convert.ToInt32(e.CurrentProgress);
                Progressbar1.Maximum = Convert.ToInt32(e.MaximumProgress);
            }
            catch { return; }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            UrlTextBox.Text = webBrowser1.Url.AbsoluteUri;
        }
    }
 }

