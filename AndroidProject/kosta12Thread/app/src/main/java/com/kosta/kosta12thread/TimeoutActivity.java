package com.kosta.kosta12thread;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.widget.Toast;

public class TimeoutActivity extends AppCompatActivity
{
    public static final int MESSAGE_BACK_KEY_TIMEOUT = 1;
    public static final int TIMEOUT_TIME = 2000;
    boolean isBackPress = false;
    Handler handler = new Handler(Looper.getMainLooper())
    {
        @Override
        public void handleMessage(@NonNull Message msg)
        {
            super.handleMessage(msg);
            switch(msg.what)
            {
                case MESSAGE_BACK_KEY_TIMEOUT:
                    isBackPress = false;
                    break;
            }
        }
    };

    @Override
    public void onBackPressed()
    {
        super.onBackPressed();
        if(!isBackPress)
        {
            Toast.makeText(this, "One more press", Toast.LENGTH_SHORT);
            isBackPress = true;
            handler.sendEmptyMessageDelayed(MESSAGE_BACK_KEY_TIMEOUT, TIMEOUT_TIME);
        }
        else
        {
            handler.removeMessages(MESSAGE_BACK_KEY_TIMEOUT);
            finish();
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_timeout);
    }
}
