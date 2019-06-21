package com.kosta.kosta12thread;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

public class LooperActivity extends AppCompatActivity
{
    Button btn_looper;
    TextView txt_looperResult;
    EditText editLooperMessage;
    Handler handler = new Handler();
    ProcessThread workerThread = new ProcessThread();

    class ProcessThread extends Thread
    {
        ProcessHandler processHandler = new ProcessHandler();
        @Override
        public void run()
        {
            Looper.prepare();
            Looper.loop();
        }

        class ProcessHandler extends Handler
        {
            @Override
            public void handleMessage(@NonNull Message msg)
            {
                final String output = msg.obj + "from MainThread";
                handler.post(new Runnable()
                {
                    @Override
                    public void run()
                    {
                        txt_looperResult.setText(output);
                    }
                });
            }
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_looper);
        editLooperMessage = (EditText)findViewById(R.id.editLooperMessage);
        txt_looperResult = (TextView)findViewById(R.id.txt_looperResult);
        btn_looper = (Button)findViewById(R.id.btn_looper);
        btn_looper.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                String input = editLooperMessage.getText().toString();
                Message message = Message.obtain();
                message.obj = input;
                workerThread.processHandler.sendMessage(message);
            }
        });
    }
}
