package com.kosta.kosta12thread;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

public class BasicThread1 extends AppCompatActivity
{
    int value = 0;
    TextView txtBasic1;
    MainHandler handler;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_basic_thread1);
        handler = new MainHandler();
        Button btn_BasicThread1 = (Button)findViewById(R.id.btn_BasicThread1);
        btn_BasicThread1.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                BackgroundThread thread1 = new BackgroundThread();
                thread1.start();
            }
        });
        txtBasic1 = (TextView)findViewById(R.id.txtBasic1);
    }
    class BackgroundThread extends Thread
    {
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
                    e.printStackTrace();
                }
                value++;
                Log.d("Thread", "value : " + value);
                Message message = handler.obtainMessage();
                Bundle bundle = new Bundle();
                bundle.putInt("value", value);
                message.setData(bundle);
                handler.sendMessage(message);
                //txtBasic1.setText("valaue : " + value);
            }
        }
    }
    class MainHandler extends Handler
    {
        @Override
        public void handleMessage(@NonNull Message msg)
        {
            super.handleMessage(msg);
            Bundle bundle = msg.getData();
            int value = bundle.getInt("value");
            txtBasic1.setText("Value : " + value);
        }
    }
}
