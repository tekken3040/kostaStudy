package com.kosta.kosta12thread;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.view.View;
import android.widget.Button;
import android.widget.ProgressBar;
import android.widget.TextView;

public class ProgressPostActivity extends AppCompatActivity
{
    Button btnProgressPost;
    TextView txtProgressPostResult;
    ProgressBar progressPostDownloadBar;
    public static final int MESSAGE_PROGRESS = 1;
    public static final int MESSAGE_DONE = 2;

    Handler handler = new Handler(Looper.getMainLooper());
    Thread thread = new Thread(new Runnable()
    {
        @Override
        public void run()
        {
            int progress = 0;
            while(progress <= 100)
            {
                handler.post(new ProgressRunnable(progress));
                try
                {
                    Thread.sleep(3000);
                }
                catch(InterruptedException e)
                {
                    e.printStackTrace();
                }
                progress += 5;
            }
            handler.post(new DoneRunnable());
        }
    });
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_progress_post);
        btnProgressPost = (Button)findViewById(R.id.btnProgressPost);
        txtProgressPostResult = (TextView)findViewById(R.id.txtProgressPostResult);
        progressPostDownloadBar = (ProgressBar)findViewById(R.id.progressPostDownloadBar);
        btnProgressPost.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                progressPostDownloadBar.setMax(100);
                txtProgressPostResult.setText("Download start .....");
                thread.start();
            }
        });
    }

    class ProgressRunnable implements Runnable
    {
        int progress = 0;
        public ProgressRunnable(int progress)
        {
            this.progress = progress;
        }
        @Override
        public void run()
        {
            progressPostDownloadBar.setProgress(progress);
            txtProgressPostResult.setText("Progress : " + progress);
        }
    }

    class DoneRunnable implements Runnable
    {
        @Override
        public void run()
        {
            progressPostDownloadBar.setProgress(100);
            txtProgressPostResult.setText("Progress : Done");
        }
    }
}
