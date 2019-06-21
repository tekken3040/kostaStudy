package com.kosta.kosta12thread;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

public class BasicThread2Activity extends AppCompatActivity
{
    Button btn_Basic2Start;
    TextView txtBasic2;
    Handler handler;
    BackGroundThread backGroundThread;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_basic_thread2);
        btn_Basic2Start = (Button)findViewById(R.id.btn_startThread2);
        txtBasic2 = (TextView)findViewById(R.id.txtBasic2);
        handler = new Handler();
        backGroundThread = new BackGroundThread();
        btn_Basic2Start.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                backGroundThread.start();
            }
        });
    }

    class BackGroundThread extends Thread
    {
        int value = 0;

        @Override
        public void run()
        {
            for(int i=0; i<100; i++)
            {
                try
                {
                    Thread.sleep(1000);
                }
                catch(InterruptedException e)
                {
                    e.printStackTrace();;
                }
                value++;
                handler.post(new Runnable()
                {
                    @Override
                    public void run()
                    {
                        txtBasic2.setText("Value : " + value);
                    }
                });
            }
        }
    }
}
