using NAudio.Wave;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolRing
{
    internal class MelodyFiles
    {

        private static IWavePlayer waveOutDevice = new WaveOutEvent();
        private static AudioFileReader audioFileReader;
        public static bool isPlayingStart = false;
        public static bool isPlayingEnd = false;
        static System.Windows.Forms.Timer timer;

        public static void Reinitialise()
        {
            Stop();
        }
        public static async void PlayStartAsync()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            try
            {
                if (Program.melodyForStartOfClassPath != null)
                {
                    isPlayingStart = true;
                    timer.Start();
                    audioFileReader = new AudioFileReader(Program.melodyForStartOfClassPath);
                    waveOutDevice.Init(audioFileReader);
                    waveOutDevice.Play();
                    waveOutDevice.Volume = 1f;
                    while (isPlayingStart && waveOutDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(100);
                    }
                }
                else
                    throw new ArgumentException("Аудио файлът не е намерен!");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Грешка: " + ex.Message);
            }
            finally
            {
                isPlayingStart = false;
                waveOutDevice.Stop();
                if (timer.Enabled)
                    timer.Stop();
            }

        }

        public static async void PlayEndAsync()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            try
            {
                if (Program.melodyForEndOfClassPath != null)
                {
                    isPlayingEnd = true;
                    timer.Start();
                    audioFileReader = new AudioFileReader(Program.melodyForEndOfClassPath);
                    waveOutDevice.Init(audioFileReader);
                    waveOutDevice.Play();
                    waveOutDevice.Volume = 1f;
                    while (isPlayingEnd && waveOutDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(100);
                    }
                }
                else
                    throw new ArgumentException("Аудио файлът не е намерен!");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Грешка: " + ex.Message);
            }
            finally
            {
                isPlayingEnd = false;
                waveOutDevice.Stop();
                if (timer.Enabled)
                    timer.Stop();
            }

        }
        public static void Stop()
        {
            if (isPlayingStart || isPlayingEnd)
            {
                isPlayingStart = false;
                isPlayingEnd = false;
                if (Program.fixedMelodyLength > 1 || Program.fixedMelodyLength < 1)
                {
                    if (audioFileReader.CurrentTime.TotalMilliseconds > 900)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            if (waveOutDevice.Volume > 0.1)
                            {
                                waveOutDevice.Volume -= 0.02f;
                                Thread.Sleep(1);
                            }
                            else
                                break;
                        }
                    }
                }
                else if (Program.fixedMelodyLength == 1)
                {
                    isPlayingStart = false;
                    isPlayingEnd = false;
                    if (audioFileReader.CurrentTime.TotalMilliseconds > 900)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            if (waveOutDevice.Volume > 0.1)
                            {
                                waveOutDevice.Volume -= 0.03f;
                                Thread.Sleep(1);
                            }
                            else
                                break;
                        }
                    }
                }
                waveOutDevice.Stop();
            }
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            if (Program.fixedMelodyLength > 0)
            {
                if (isPlayingStart || isPlayingEnd)
                {
                    if (Program.fixedMelodyLength > 1)
                    {
                        if (audioFileReader.CurrentTime.TotalSeconds >= Program.fixedMelodyLength - 1)
                        {
                            Stop();
                        }
                    }
                    else
                    {
                        if (audioFileReader.CurrentTime.TotalMilliseconds >= 900)
                        {
                            Stop();
                        }
                    }
                }
            }
        }
    }
}

