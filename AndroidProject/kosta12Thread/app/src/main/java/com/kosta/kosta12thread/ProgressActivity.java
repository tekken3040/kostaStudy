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

public class ProgressActivity extends AppCompatActivity
{
    TextView txtProgressMessage;
    ProgressBar progressDownloadBar;
    Button btnProgress;
    public static final int MESSAGE_PROGRESS = 1;
    public static final int MESSAGE_DONE = 2;

    Handler handler = new Handler(Looper.getMainLooper())
    {
        @Override
        public void handleMessage(@NonNull Message msg)
        {
            super.handleMessage(msg);
            switch(msg.what)
            {
                case MESSAGE_PROGRESS:
                    int progress = msg.arg1;
                    progressDownloadBar.setProgress(progress, true);
                    txtProgressMessage.setText("Progress : " + progress + "%");
                    break;

                case MESSAGE_DONE:
                    progressDownloadBar.setProgress(100, true);
                    txtProgressMessage.setText("Progress Done");
                    break;
            }
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_progress);
        btnProgress = (Button)findViewById(R.id.btn_progress);
        progressDownloadBar = (ProgressBar)findViewById(R.id.progressDownload);
        txtProgressMessage = (TextView)findViewById(R.id.txtProgressMessage);

        btnProgress.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                progressDownloadBar.setMax(100);
                txtProgressMessage.setText("Download Start");
                new Thread(new Runnable()
                {
                    @Override
                    public void run()
                    {
                        int progress = 0;
                        while(progress <= 100)
                        {
                            Message msg;
                            if(progress < 100)
                                msg = handler.obtainMessage(MESSAGE_PROGRESS, progress, 0);
                            else
                                msg = handler.obtainMessage(MESSAGE_DONE);

                            handler.sendMessage(msg);
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
                    }
                }).start();
            }
        });
    }
}
