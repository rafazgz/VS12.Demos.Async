using AA.Demo.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VS12.Demos.Async
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string Message = "Se han encontrado {0} peliculas.";
        private void button1_Click(object sender, EventArgs e)
        {
            var movies = MoviesRepository.GetMovies().Count;
            label1.Text = String.Format(Message, movies);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Func<int> GetMovies = () => MoviesRepository.GetMovies().Count;

            GetMovies.BeginInvoke((result) => {
                var data = (result.AsyncState as Func<int>).EndInvoke(result);
               // label1.Text = String.Format(Message, data);
               this.Invoke(new Action(()=> label1.Text = String.Format(Message, data)));
            }, GetMovies);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var task = Task<int>.Factory.StartNew(() => MoviesRepository.GetMovies().Count());
            //task.ContinueWith((result) => label2.Text = String.Format(Message, result.Result));

            task.ContinueWith((result) => label2.Text = String.Format(Message, result.Result),
    TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            var result = await MoviesRepository.GetMoviesAsync();
            label3.Text = String.Format(Message, result.Count);
        }
        CancellationToken token;
        CancellationTokenSource source;
        private async void button5_Click(object sender, EventArgs e)
        {
            ConfigureCancellation();
            var result = await MoviesRepository.GetMoviesAsync();
            try
            {


                    await MoviesRepository.ManageMovieAsync(result, token: token);
                
            }
            catch (Exception)
            {
                
  
            }
        }

        private void ConfigureCancellation()
        {
            source = new CancellationTokenSource();
            source.Token.Register(() => label3.Text = "Proceso cancelado.", true);
            token = source.Token;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            source.Cancel();
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            var progress = new ProgressReporter<int>((value) => label3.Text = String.Format(Message, value));
            ConfigureCancellation();
            try
            {
                var result = await MoviesRepository.GetMoviesAsync();
                await MoviesRepository.ManageMovieAsync(result, progress, token);
            }
            catch (Exception)
            {
                
      
            }
        }
    }
}
