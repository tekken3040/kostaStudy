package com.kosta.kosta12thread;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.SystemClock;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;

import java.sql.Time;

public class CountdownActivity extends AppCompatActivity
{
    TextView txtCountdownResult;
    Button btnCountdown;
    int count = 10;
    Handler handler = new Handler(Looper.getMainLooper());
    long startTime = -1;
    Runnable downRunnable = new Runnable()
    {
        @Override
        public void run()
        {
            long time = SystemClock.elapsedRealtime();
            if(startTime == -1)
                startTime = time;
            int gap = (int)(time - startTime);
            int count = 10 - gap/1000;
            int rest = 1000 - (gap%1000);
            if(count >= 0)
            {
                txtCountdownResult.setText("Count : " + count);
                handler.postDelayed(this, rest);
            }
            else
            {
                txtCountdownResult.setText("Done");
            }
            /*if(count >= 0)
            {
                txtCountdownResult.setText("Count : " + count);
                count--;
                handler.postDelayed(this,1000);
            }
            else
            {
                txtCountdownResult.setText("Done");
            }*/
        }
    };
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_countdown);
        txtCountdownResult = (TextView)findViewById(R.id.txtCountDown);
        btnCountdown = (Button)findViewById(R.id.btn_Countdown);
        btnCountdown.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                count = 10;
                startTime = -1;
                handler.removeCallbacks(downRunnable);
                handler.post(downRunnable);
            }
        });
    }
}
