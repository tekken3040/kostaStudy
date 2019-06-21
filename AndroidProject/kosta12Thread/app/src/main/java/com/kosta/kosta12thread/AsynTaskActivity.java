package com.kosta.kosta12thread;

import androidx.appcompat.app.AppCompatActivity;

import android.os.AsyncTask;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ProgressBar;
import android.widget.TextView;

public class AsynTaskActivity extends AppCompatActivity
{
    TextView txtProgressMessage;
    ProgressBar progressDownloadBar;
    Button btnProgress;
    DownloadTask downloadTask;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_asyn_task);
        btnProgress = (Button)findViewById(R.id.btn_progress);
        progressDownloadBar = (ProgressBar)findViewById(R.id.progressDownload);
        txtProgressMessage = (TextView)findViewById(R.id.txtProgressMessage);
        downloadTask = new DownloadTask();
        btnProgress.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                downloadTask.execute("Start");
            }
        });
    }

    class DownloadTask extends AsyncTask<String, Integer,Boolean>
    {
        @Override
        protected void onPreExecute()
        {
            super.onPreExecute();
            progressDownloadBar.setMax(100);
            txtProgressMessage.setText("Download Start......");
        }

        @Override
        protected Boolean doInBackground(String... strings)
        {
            int progress = 0;
            while(progress <= 20)
            {
                publishProgress(progress, progress*5);
                try
                {
                    Thread.sleep(1000);
                }
                catch(InterruptedException e)
                {
                    e.printStackTrace();
                }
                progress++;
            }
            return true;
        }

        @Override
        protected void onProgressUpdate(Integer... values)
        {
            super.onProgressUpdate(values);
            int count = values[0];
            int progress = values[1];
            progressDownloadBar.setProgress(progress);
            txtProgressMessage.setText("Progress : " + progress + "%");
        }

        @Override
        protected void onPostExecute(Boolean result)
        {
            super.onPostExecute(result);
            if(result != null && result)
            {
                progressDownloadBar.setProgress(100);
                txtProgressMessage.setText("Done");
            }
        }
    }
}
